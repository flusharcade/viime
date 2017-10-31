// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOSModule.cs" company="Flush Arcade Pty Ltd.">
//   Copyright (c) 2016 Flush Arcade Pty Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Viime.iOS.Modules
{
	using Autofac;

    using SQLite.Net.Interop;
    using SQLite.Net.Platform.XamarinIOS;

    using Viime.iOS.DataAccess;
    using Viime.iOS.Extras;
	using Viime.iOS.Logging;
    using Viime.Portable.DataAccess.Storage;
    using Viime.Portable.Extras;
	using Viime.Portable.Ioc;
	using Viime.Portable.Logging;

	/// <summary>
	/// IOS Module.
	/// </summary>
	public class IOSModule : IModule
	{
		#region Public Methods

		/// <summary>
		/// Register the specified builder.
		/// </summary>
		/// <param name="builder">builder.</param>
		public void Register(ContainerBuilder builder)
		{
			builder.RegisterType<IOSMethods>().As<IMethods>().SingleInstance();
			builder.RegisterType<LoggeriOS>().As<ILogger>().SingleInstance();

            builder.RegisterType<SQLiteSetup>().As<ISQLiteSetup>().SingleInstance();
            builder.RegisterType<SQLitePlatformIOS>().As<ISQLitePlatform>().SingleInstance();
		}

		#endregion
	}
}