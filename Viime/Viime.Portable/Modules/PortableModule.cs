// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortableModule.cs" company="Flush Arcade Pty Ltd.">
//   Copyright (c) 2016 Flush Arcade Pty Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Viime.Portable.Modules
{
	using System;

	using Autofac;

	using Viime.Portable.Ioc;
	using Viime.Portable.ViewModels;
	using Viime.Portable.UI;
    using Viime.Portable.DataAccess.Storage;

    /// <summary>
    /// Portable module.
    /// </summary>
    public class PortableModule : IModule
	{
		#region Public Methods

		/// <summary>
		/// Register the specified builder.
		/// </summary>
		/// <param name="builder">builder.</param>
		public void Register(ContainerBuilder builder)
		{
			builder.RegisterType<MainPageViewModel> ().SingleInstance();
			builder.RegisterType<CameraPageViewModel> ().SingleInstance();

            builder.RegisterType<SQLiteStorage>().As<ISQLiteStorage>().SingleInstance();
		}

		#endregion
	}
}