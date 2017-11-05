using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MediaServices.Client;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.Azure.WebJobs;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Viime.Functions
{
    // Read values from the App.config file.

    static readonly string _AADTenantDomain = Environment.GetEnvironmentVariable("AMSAADTenantDomain");
    static readonly string _RESTAPIEndpoint = Environment.GetEnvironmentVariable("AMSRESTAPIEndpoint");

    static readonly string _mediaservicesClientId = Environment.GetEnvironmentVariable("AMSClientId");
    static readonly string _mediaservicesClientSecret = Environment.GetEnvironmentVariable("AMSClientSecret");

    static readonly string _connectionString = Environment.GetEnvironmentVariable("ConnectionString");

    private static CloudMediaContext _context = null;
    private static CloudStorageAccount _destinationStorageAccount = null;

    public static void Run(CloudBlockBlob myBlob, string fileName, TraceWriter log)
    {
        // NOTE that the variables {fileName} here come from the path setting in function.json
        // and are passed into the  Run method signature above. We can use this to make decisions on what type of file
        // was dropped into the input container for the function. 

        // No need to do any Retry strategy in this function, By default, the SDK calls a function up to 5 times for a 
        // given blob. If the fifth try fails, the SDK adds a message to a queue named webjobs-blobtrigger-poison.

        log.Info($"C# Blob trigger function processed: {fileName}.mp4");
        log.Info($"Media Services REST endpoint : {_RESTAPIEndpoint}");

        try
        {
            AzureAdTokenCredentials tokenCredentials = new AzureAdTokenCredentials(_AADTenantDomain,
                                new AzureAdClientSymmetricKey(_mediaservicesClientId, _mediaservicesClientSecret),
                                AzureEnvironments.AzureCloudEnvironment);

            AzureAdTokenProvider tokenProvider = new AzureAdTokenProvider(tokenCredentials);

            _context = new CloudMediaContext(new Uri(_RESTAPIEndpoint), tokenProvider);

            IAsset newAsset = CreateAssetFromBlob(myBlob, fileName, log).GetAwaiter().GetResult();

            // Step 2: Create an Encoding Job

            // Declare a new encoding job with the Standard encoder
            IJob job = _context.Jobs.Create("Azure Function - MES Job");

            // Get a media processor reference, and pass to it the name of the 
            // processor to use for the specific task.
            IMediaProcessor processor = GetLatestMediaProcessorByName("Media Encoder Standard");

            // Create a task with the encoding details, using a custom preset
            ITask task = job.Tasks.AddNew("Encode with Adaptive Streaming",
                processor,
                "Adaptive Streaming",
                TaskOptions.None);

            // Specify the input asset to be encoded.
            task.InputAssets.Add(newAsset);

            // Add an output asset to contain the results of the job. 
            // This output is specified as AssetCreationOptions.None, which 
            // means the output asset is not encrypted. 
            task.OutputAssets.AddNew(fileName, AssetCreationOptions.None);

            job.Submit();
            log.Info("Job Submitted");

        }
        catch (Exception ex)
        {
            log.Error("ERROR: failed.");
            log.Info($"StackTrace : {ex.StackTrace}");
            throw ex;
        }
    }

    private static IMediaProcessor GetLatestMediaProcessorByName(string mediaProcessorName)
    {
        var processor = _context.MediaProcessors.Where(p => p.Name == mediaProcessorName).
        ToList().OrderBy(p => new Version(p.Version)).LastOrDefault();

        if (processor == null)
            throw new ArgumentException(string.Format("Unknown media processor", mediaProcessorName));

        return processor;
    }

    public static async Task<IAsset> CreateAssetFromBlob(CloudBlockBlob blob, string assetName, TraceWriter log)
    {
        IAsset newAsset = null;

        try
        {
            Task<IAsset> copyAssetTask = CreateAssetFromBlobAsync(blob, assetName, log);
            newAsset = await copyAssetTask;
            log.Info($"Asset Copied : {newAsset.Id}");
        }
        catch (Exception ex)
        {
            log.Info("Copy Failed");
            log.Info($"ERROR : {ex.Message}");
            throw ex;
        }

        return newAsset;
    }

    /// <summary>
    /// Creates a new asset and copies blobs from the specifed storage account.
    /// </summary>
    /// <param name="blob">The specified blob.</param>
    /// <returns>The new asset.</returns>
    public static async Task<IAsset> CreateAssetFromBlobAsync(CloudBlockBlob blob, string assetName, TraceWriter log)
    {
        //Get a reference to the storage account that is associated with the Media Services account. 
        _destinationStorageAccount = CloudStorageAccount.Parse(_connectionString);

        // Create a new asset. 
        var asset = _context.Assets.Create(blob.Name, AssetCreationOptions.None);
        log.Info($"Created new asset {asset.Name}");

        IAccessPolicy writePolicy = _context.AccessPolicies.Create("writePolicy",
        TimeSpan.FromHours(4), AccessPermissions.Write);
        ILocator destinationLocator = _context.Locators.CreateLocator(LocatorType.Sas, asset, writePolicy);
        CloudBlobClient destBlobStorage = _destinationStorageAccount.CreateCloudBlobClient();

        // Get the destination asset container reference
        string destinationContainerName = (new Uri(destinationLocator.Path)).Segments[1];
        CloudBlobContainer assetContainer = destBlobStorage.GetContainerReference(destinationContainerName);

        try
        {
            assetContainer.CreateIfNotExists();
        }
        catch (Exception ex)
        {
            log.Error("ERROR:" + ex.Message);
        }

        log.Info("Created asset.");

        // Get hold of the destination blob
        CloudBlockBlob destinationBlob = assetContainer.GetBlockBlobReference(blob.Name);

        // Copy Blob
        try
        {
            using (var stream = await blob.OpenReadAsync())
            {
                await destinationBlob.UploadFromStreamAsync(stream);
            }

            log.Info("Copy Complete.");

            var assetFile = asset.AssetFiles.Create(blob.Name);
            assetFile.ContentFileSize = blob.Properties.Length;
            assetFile.IsPrimary = true;
            assetFile.Update();
            asset.Update();
        }
        catch (Exception ex)
        {
            log.Error(ex.Message);
            log.Info(ex.StackTrace);
            log.Info("Copy Failed.");
            throw;
        }

        destinationLocator.Delete();
        writePolicy.Delete();

        return asset;
    }
}