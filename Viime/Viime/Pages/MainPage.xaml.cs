// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.cs" company="Flush Arcade Pty Ltd.">
//   Copyright (c) 2016 Flush Arcade Pty Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Viime.Pages
{
    using System.Collections.Generic;

    using Viime.Portable.ViewModels;
    using Viime.Portable.Ioc;
    using Viime.UI;
    using Microsoft.Identity.Client;
    using Viime.Portable.Resources;
    using System;
    using Newtonsoft.Json.Linq;
    using System.Linq;

    /// <summary>
    /// Main page.
    /// </summary>
    public partial class MainPage : ExtendedContentPage, INavigableXamarinFormsPage
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Camera.Pages.MainPage"/> class.
        /// </summary>
        /// <param name="model">Model.</param>
        public MainPage(MainPageViewModel model) : base(model)
        {
            BindingContext = model;
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            UpdateSignInState(false);

            // Check to see if we have a User
            // in the cache already.
            try
            {
                AuthenticationResult ar = await App.PCA.AcquireTokenSilentAsync(App.Scopes, GetUserByPolicy(App.PCA.Users, App.PolicySignUpSignIn), App.Authority, false);
                UpdateUserInfo(ar);
                UpdateSignInState(true);
            }
            catch (Exception ex)
            {
                // Uncomment for debugging purposes
                //await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");

                // Doesn't matter, we go in interactive mode
                UpdateSignInState(false);
            }
        }

        #endregion

        async void OnSignInSignOut(object sender, EventArgs e)
        {
            try
            {
                //if (btnSignInSignOut.Text == "Sign in")
                if (true)
                {
                    AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes, GetUserByPolicy(App.PCA.Users, App.PolicySignUpSignIn), App.UiParent);
                    UpdateUserInfo(ar);
                    UpdateSignInState(true);
                }
                else
                {
                    foreach (var user in App.PCA.Users)
                    {
                        App.PCA.Remove(user);
                    }
                    UpdateSignInState(false);
                }
            }
            catch (Exception ex)
            {
                // Checking the exception message 
                // should ONLY be done for B2C
                // reset and not any other error.
                if (ex.Message.Contains("AADB2C90118"))
                    OnPasswordReset();
                // Alert if any exception excludig user cancelling sign-in dialog
                else if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

        private IUser GetUserByPolicy(IEnumerable<IUser> users, string policy)
        {
            foreach (var user in users)
            {
                string userIdentifier = Base64UrlDecode(user.Identifier.Split('.')[0]);
                if (userIdentifier.EndsWith(policy.ToLower())) return user;
            }

            return null;
        }

        private string Base64UrlDecode(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
            var byteArray = Convert.FromBase64String(s);
            var decoded = System.Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
            return decoded;
        }

        public void UpdateUserInfo(AuthenticationResult ar)
        {
            JObject user = ParseIdToken(ar.IdToken);
            //lblName.Text = user["name"]?.ToString();
            //lblId.Text = user["oid"]?.ToString();
        }

        JObject ParseIdToken(string idToken)
        {
            // Get the piece with actual user info
            idToken = idToken.Split('.')[1];
            idToken = Base64UrlDecode(idToken);
            return JObject.Parse(idToken);
        }

        async void OnEditProfile(object sender, EventArgs e)
        {
            try
            {
                // KNOWN ISSUE:
                // User will get prompted 
                // to pick an IdP again.
                AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes, GetUserByPolicy(App.PCA.Users, App.PolicyEditProfile), UIBehavior.SelectAccount, string.Empty, null, App.AuthorityEditProfile, App.UiParent);
                UpdateUserInfo(ar);
            }
            catch (Exception ex)
            {
                // Alert if any exception excludig user cancelling sign-in dialog
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }
        async void OnPasswordReset()
        {
            try
            {
                AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes, (IUser)null, UIBehavior.SelectAccount, string.Empty, null, App.AuthorityPasswordReset, App.UiParent);
                UpdateUserInfo(ar);
            }
            catch (Exception ex)
            {
                // Alert if any exception excludig user cancelling sign-in dialog
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

        void UpdateSignInState(bool isSignedIn)
        {
            //btnSignInSignOut.Text = isSignedIn ? "Sign out" : "Sign in";
            //btnEditProfile.IsVisible = isSignedIn;
            //btnCallApi.IsVisible = isSignedIn;
            //slUser.IsVisible = isSignedIn;
            //lblApi.Text = "";
        }

        #region INavigableXamarinFormsPage interface

        /// <summary>
        /// Called when page is navigated to.
        /// </summary>
        /// <returns>The navigated to.</returns>
        /// <param name="navigationParameters">Navigation parameters.</param>
        public void OnNavigatedTo(IDictionary<string, object> navigationParameters)
        {
            this.Show(navigationParameters);
        }

        #endregion
    }
}