// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamFormsModule.cs" company="Flush Arcade Pty Ltd.">
//   Copyright (c) 2016 Flush Arcade Pty Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Viime.Modules
{
	using System.Windows.Input;

	using Autofac;

	using Xamarin.Forms;

	using Viime.Portable.Ioc;
	using Viime.Pages;
	using Viime.UI;

	using Viime.Portable.UI;

	/// <summary>
	/// Xamarin forms module.
	/// </summary>
	public class XamFormsModule : IModule
	{
		#region Public Methods

		/// <summary>
		/// Register the specified builder.
		/// </summary>
		/// <param name="builder">builder.</param>
		public void Register(ContainerBuilder builder)
		{
            // pages
            builder.RegisterType<LoginPage>().SingleInstance();
			builder.RegisterType<MainPage> ().SingleInstance();
			builder.RegisterType<CameraPage> ().SingleInstance();
			builder.RegisterType<NavigationService> ().As<INavigationService>().SingleInstance();

            builder.Register(x => new NavigationPage(x.Resolve<LoginPage>())).AsSelf().SingleInstance();

            builder.RegisterType<Command>().As<ICommand>().InstancePerDependency();
		}

		#endregion
	}
}