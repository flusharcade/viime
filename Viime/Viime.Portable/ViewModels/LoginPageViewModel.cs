// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginPageViewModel.cs" company="Flush Arcade Pty Ltd.">
//   Copyright (c) 2016 Flush Arcade Pty Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Viime.Portable.ViewModels
{
	using System;
	using System.Windows.Input;

	using Viime.Portable.Enums;
	using Viime.Portable.UI;
	using Viime.Portable.Extras;
    using Viime.Portable.Logging;
    using Viime.Portable.DataAccess.Storage;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    /// <summary>
    /// Main page view model.
    /// </summary>
    public class LoginPageViewModel : ViewModelBase
	{
		#region Private Properties

		/// <summary>
		/// The methods.
		/// </summary>
		private readonly IMethods _methods;

		/// <summary>
		/// The location command.
		/// </summary>
        private ICommand _loginCommand;

		/// <summary>
		/// The exit command.
		/// </summary>
		private ICommand _exitCommand;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the exit command.
		/// </summary>
		/// <value>The exit command.</value>
		public ICommand ExitCommand
		{
			get { return _exitCommand; }
			set { SetProperty(nameof(ExitCommand), ref _exitCommand, value); }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Camera.Portable.ViewModels.LoginPageViewModel"/> class.
		/// </summary>
		/// <param name="navigation">Navigation.</param>
		/// <param name="commandFactory">Command factory.</param>
		/// <param name="methods">Methods.</param>
		public LoginPageViewModel (INavigationService navigation, Func<Action, ICommand> commandFactory,
                                   IMethods methods, ILogger logger, ISQLiteStorage storage) 
            : base (navigation, storage)
		{
			_methods = methods;

			_exitCommand = commandFactory (async () =>
			{
				await NotifyAlert("GoodBye!!");

				_methods.Exit();
			});
		}

		#endregion

        public async Task AuthFinished(bool success)
        {
            if (success)
            {
                await Navigation.Navigate(PageNames.MainPage, new Dictionary<string, object>());
            }
        }
	}
}