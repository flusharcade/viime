// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DroidModule.cs" company="Flush Arcade Pty Ltd.">
//   Copyright (c) 2016 Flush Arcade Pty Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Viime.Droid.Modules
{
	using Autofac;

    using SQLite.Net.Interop;
    using SQLite.Net.Platform.XamarinAndroid;

	using Viime.Droid.Extras;
	using Viime.Droid.Logging;
    using Viime.Droid.DataAccess;

	using Viime.Portable.Extras;
	using Viime.Portable.Logging;
	using Viime.Portable.Ioc;
    using Viime.Portable.DataAccess.Storage;

    /// <summary>
    /// Droid module.
    /// </summary>
    public class DroidModule : IModule
	{
		#region Public Methods

		/// <summary>
		/// Register the specified builder.
		/// </summary>
		/// <param name="builder">builder.</param>
		public void Register(ContainerBuilder builder)
		{
			builder.RegisterType<DroidMethods>().As<IMethods>().SingleInstance();
			builder.RegisterType<LoggerDroid>().As<ILogger>().SingleInstance();

            builder.RegisterType<SQLiteSetup>().As<ISQLiteSetup>().SingleInstance();
            builder.RegisterType<SQLitePlatformAndroid>().As<ISQLitePlatform>().SingleInstance();
		}

		#endregion
	}
}