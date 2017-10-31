// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainActivity.cs" company="Flush Arcade Pty Ltd.">
//   Copyright (c) 2016 Flush Arcade Pty Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Viime.Droid
{
    using Android.App;
    using Android.Content.PM;
    using Android.OS;

    using Viime.Droid.Modules;

    using Viime.Modules;
    using Viime.Controls;

    using Viime.Portable.Modules;
    using Viime.Portable.Ioc;
    using Microsoft.Identity.Client;
    using Android.Content;

    /// <summary>
    /// Main activity.
    /// </summary>
    [Activity(Label = "Viime.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        #region Protected Methods

        /// <summary>
        /// Called when the activity is created.
        /// </summary>
        /// <returns>The create.</returns>
        /// <param name="bundle">Bundle.</param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            InitIoC();

            LoadApplication(new App());

            App.UiParent = new UIParent(Xamarin.Forms.Forms.Context as Activity);           
        }

        #endregion

        /// <summary>
        /// Ons the configuration changed.
        /// </summary>
        /// <param name="newConfig">New config.</param>
        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            System.Diagnostics.Debug.WriteLine("MainActivity: Orientation changed. " + newConfig.Orientation);

            switch (newConfig.Orientation)
            {
                case Android.Content.Res.Orientation.Portrait:
                    OrientationPage.NotifyOrientationChange(Portable.Enums.Orientation.Portrait);
                    break;
                case Android.Content.Res.Orientation.Landscape:
                    OrientationPage.NotifyOrientationChange(Portable.Enums.Orientation.LandscapeLeft);
                    break;
            }
        }

        #region Private Methods

        /// <summary>
        /// Inits the IoC container and modules.
        /// </summary>
        /// <returns>The io c.</returns>
        private void InitIoC()
        {
            IoC.CreateContainer();
            IoC.RegisterModule(new DroidModule());
            IoC.RegisterModule(new XamFormsModule());
            IoC.RegisterModule(new PortableModule());
            IoC.StartContainer();
        }

        #endregion

        /// <summary>
        /// Ons the activity result.
        /// </summary>
        /// <param name="requestCode">Request code.</param>
        /// <param name="resultCode">Result code.</param>
        /// <param name="data">Data.</param>
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }
    }
}