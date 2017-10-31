// <copyright file="ViewModelBase.cs" company="Flush Arcade Pty Ltd.">
//   Copyright (c) 2016 Flush Arcade Pty Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Viime.Portable.ViewModels
{
	using System;
	using System.ComponentModel;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using System.Reactive.Threading.Tasks;
	using System.Runtime.CompilerServices;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Refit;

	using Viime.Portable.UI;
    using Viime.Portable.Resources;
    using Viime.Portable.WebServices;
    using Viime.Portable.DataAccess.Storage;
    using Viime.Portable.DataAccess.Storable;


    /// <summary>
    /// The base class of all view models
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
	{
        #region Private 

        /// <summary>
        /// The media service API.
        /// </summary>
        private IActiveDirectoryApi _activeDirectoryApi;

        #endregion

        #region Public Events

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Occurs when alert.
		/// </summary>
		public event EventHandler<AlertArgs> Alert;

		#endregion

		#region Public Properties

		/// <summary>
		/// The navigation.
		/// </summary>
		public INavigationService Navigation;

        /// <summary>
        /// The storage.
        /// </summary>
        public ISQLiteStorage Storage;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Camera.Portable.ViewModels.ViewModelBase"/> class.
		/// </summary>
		/// <param name="navigation">Navigation.</param>
        public ViewModelBase(INavigationService navigation, ISQLiteStorage storage)
		{
			Navigation = navigation;
            Storage = storage;

            //Context = new AuthenticationContext("https://login.windows.net/common");
		}

        #endregion

        #region Protected Methods

        /*AuthenticationContext Context;

        protected async Task<string> GetToken()
        {
            // The AquireTokenAsync call will prompt with a UI if necessary
            // Or otherwise silently use a refresh token to return
            // a valid access token 
            var authenticationResult = await Context.AcquireTokenAsync(Config.ActiveDirectoryBaseUrl, 
                                                                       Config.MediaServicesClientId, 
                                                                       new Uri("http://viime.com"), 
                                                                       new PlatformParameters());

            return authenticationResult.AccessToken;
        }*/

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <returns>The token.</returns>
        //protected async Task<string> GetToken()
        protected async Task GetToken()
        {
            var url = string.Format("{0}{1}", Config.ActiveDirectoryBaseUrl, Config.ActiveDirectoryTenantId);
            _activeDirectoryApi = RestService.For<IActiveDirectoryApi>(url, new RefitSettings
            {
                JsonSerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            });

            var auth = new AuthBody()
            {
                GrantType = "client_credentials",
                ClientId = Config.MediaServicesClientId,
                ClientSecret = Config.MediaServicesClientSecret,
                Resource = Config.MediaServicesResourceUrl
            };

            var result = await _activeDirectoryApi.GetToken(auth);

            var settings = new SettingsStorable()
            {
                BearerToken = result.AccessToken
            };
            await Storage.InsertObject(settings);
        }

		/// <summary>
		/// Sets the property.
		/// </summary>
		/// <returns>The property.</returns>
		/// <param name="propertyName">Property name.</param>
		/// <param name="referenceProperty">Reference property.</param>
		/// <param name="newProperty">New property.</param>
		protected void SetProperty<T>(string propertyName, ref T referenceProperty, T newProperty)
		{
			if (!newProperty.Equals(referenceProperty))
			{
				referenceProperty = newProperty;
			}

			OnPropertyChanged(propertyName);
		}

		/// <summary>
		/// Raises the property changed event.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;

			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Loads the async.
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="parameters">Parameters.</param>
		protected virtual async Task LoadAsync(IDictionary<string, object> parameters)
		{
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Notifies the alert.
		/// </summary>
		/// <returns>The alert.</returns>
		/// <param name="message">Message.</param>
		public Task<bool> NotifyAlert(string message)
		{
			var tcs = new TaskCompletionSource<bool>();

			Alert?.Invoke(this, new AlertArgs()
			{
				Message = message,
				Tcs = tcs
			});

			return tcs.Task;
		}

		/// <summary>
		/// </summary>
		/// <param name="parameters">
		/// </param>
		public void OnShow(IDictionary<string, object> parameters)
		{
			LoadAsync(parameters).ToObservable().Subscribe(
				result =>
				{
					// we can add things to do after we load the view model
				}, 
				ex =>
				{
					// we can handle any areas from the load async function
				});
		}

		#endregion
	}
}