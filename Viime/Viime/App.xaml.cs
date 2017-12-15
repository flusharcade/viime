// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.cs" company="Flush Arcade Pty Ltd.">
//   Copyright (c) 2016 Flush Arcade Pty Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

[assembly: Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]

namespace Viime
{
    using Xamarin.Forms;

    using Microsoft.Identity.Client;

    using Viime.Portable.Ioc;
    using Viime.Portable.Resources;

    /// <summary>
    /// The App.
    /// </summary>
    public partial class App : Application
    {
        public static PublicClientApplication PCA = null;

        // Azure AD B2C Coordinates
        public static string PolicySignUpSignIn = "B2C_1_viime-signup-signon-policy";
        public static string PolicyEditProfile = "B2C_1_viime-signup-signon-policy";
        public static string PolicyResetPassword = "B2C_1_viime-password-reset-policy";
        public static string[] Scopes = { "https://viime.onmicrosoft.com/api" };

        public static string AuthorityBase = $"https://login.microsoftonline.com/tfp/{Config.ActiveDirectoryTenantId}/";
        public static string Authority = $"{AuthorityBase}{PolicySignUpSignIn}";
        public static string AuthorityEditProfile = $"{AuthorityBase}{PolicyEditProfile}";
        public static string AuthorityPasswordReset = $"{AuthorityBase}{PolicyResetPassword}";

        public static UIParent UiParent = null;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Camera.App"/> class.
        /// </summary>
        public App()
        {
            // default redirectURI; each platform specific project will have to override it with its own
            PCA = new PublicClientApplication(Config.MediaServicesClientId, Authority);
            PCA.RedirectUri = $"msal{Config.MediaServicesClientId}://auth";

            InitializeComponent();

            // The Application ResourceDictionary is available in Xamarin.Forms 1.3 and later
            if (Current.Resources == null)
            {
                Current.Resources = new ResourceDictionary();
            }

            MainPage = IoC.Resolve<NavigationPage>();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Override the starting function
        /// </summary>
        /// <returns>The start.</returns>
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        /// <summary>
        /// Override the OnSleep function
        /// </summary>
        /// <returns>The sleep.</returns>
        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        /// <summary>
        /// Overrides the OnResume function
        /// </summary>
        /// <returns>The resume.</returns>
        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        #endregion
    }
}