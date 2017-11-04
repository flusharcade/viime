// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPageViewModel.cs" company="Flush Arcade Pty Ltd.">
//   Copyright (c) 2016 Flush Arcade Pty Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Viime.Portable.ViewModels
{
	using System;
	using System.Windows.Input;
    using System.Threading.Tasks;

	using Viime.Portable.Enums;
	using Viime.Portable.UI;
	using Viime.Portable.Extras;
    using Viime.Portable.DataAccess.Storage;
    using Viime.Portable.DataAccess.Storable;
    using Viime.Portable.Logging;
    using Viime.Portable.Resources;
    using System.Collections.Generic;

    /// <summary>
    /// Main page view model.
    /// </summary>
    public class MainPageViewModel : ViewModelBase
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

        /// <summary>
        /// The storage.
        /// </summary>
        private ISQLiteStorage _storage;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the location command.
		/// </summary>
		/// <value>The location command.</value>
        public ICommand LoginCommand
		{
            get { return _loginCommand; }
			set { SetProperty(nameof(LoginCommand), ref _loginCommand, value); }
		}

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
		/// Initializes a new instance of the <see cref="T:Camera.Portable.ViewModels.MainPageViewModel"/> class.
		/// </summary>
		/// <param name="navigation">Navigation.</param>
		/// <param name="commandFactory">Command factory.</param>
		/// <param name="methods">Methods.</param>
		public MainPageViewModel (INavigationService navigation, Func<Action, ICommand> commandFactory,
                                  IMethods methods, ILogger logger, ISQLiteStorage storage) 
            : base (navigation, storage)
		{
            _storage = storage;
			_methods = methods;

			_exitCommand = commandFactory (async () =>
			{
				await NotifyAlert("GoodBye!!");

				_methods.Exit();
			});

			_loginCommand = commandFactory (async () => 
            {
                await navigation.Navigate(PageNames.CameraPage, new Dictionary<string, object>());
            });

            SetupSQLite().ConfigureAwait(false);
            GetToken().ConfigureAwait(false);
		}

		#endregion

        /// <summary>
        /// Setups the SQL ite.
        /// </summary>
        /// <returns>The SQL ite.</returns>
        private async Task SetupSQLite()
        {
            // create Sqlite connection
            _storage.CreateSQLiteAsyncConnection();

            // create DB tables
            await _storage.CreateTable<SettingsStorable>();
        }
	}
}