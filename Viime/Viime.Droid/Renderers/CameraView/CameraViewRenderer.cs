// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraViewRenderer.cs" company="Flush Arcade Pty Ltd.">
//   Copyright (c) 2016 Flush Arcade Pty Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

[assembly: Xamarin.Forms.ExportRenderer (typeof(Viime.Controls.CameraView), typeof(Viime.Droid.Renderers.CameraView.CameraViewRenderer))]

namespace Viime.Droid.Renderers.CameraView
{
	using System;

	using Xamarin.Forms;
	using Xamarin.Forms.Platform.Android;

	using Viime.Controls;

	/// <summary>
	/// Bodyshop camera renderer.
	/// </summary>
	public class CameraViewRenderer : ViewRenderer<CameraView, CameraDroid>
	{
		#region Private Properties

		/// <summary>
		/// The bodyshop camera droid.
		/// </summary>
		private CameraDroid Camera;

		#endregion

		#region Protected Methods

		/// <summary>
		/// Raises the element changed event.
		/// </summary>
		/// <param name="e">E.</param>
		protected override void OnElementChanged(ElementChangedEventArgs<CameraView> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
			{
				Camera = new CameraDroid(Context);

				SetNativeControl(Camera);
			}

			if (e.OldElement != null)
			{
				// something wrong here, not being called on disposal
			}

			if (e.NewElement != null)
			{
				Camera.Available += e.NewElement.NotifyAvailability;
				Camera.Photo += e.NewElement.NotifyPhoto;
				Camera.Busy += e.NewElement.NotifyBusy;

				e.NewElement.Flash += HandleFlashChange;
				e.NewElement.OpenCamera += HandleCameraInitialisation;
				e.NewElement.Focus += HandleFocus;
				e.NewElement.Shutter += HandleShutter;
			}
		}

		/// <summary>
		/// Dispose the specified disposing.
		/// </summary>
		/// <param name="disposing">If set to <c>true</c> disposing.</param>
		protected override void Dispose(bool disposing)
		{
			Element.Flash -= HandleFlashChange;
			Element.OpenCamera -= HandleCameraInitialisation;
			Element.Focus -= HandleFocus;
			Element.Shutter -= HandleShutter;

			Camera.Available -= Element.NotifyAvailability;
			Camera.Photo -= Element.NotifyPhoto;
			Camera.Busy -= Element.NotifyBusy;

			base.Dispose(disposing);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Handles the camera initialisation.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">If set to <c>true</c> arguments.</param>
		private void HandleCameraInitialisation (object sender, bool args)
		{
			Camera.OpenCamera();
		}

		/// <summary>
		/// Handles the flash change.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">If set to <c>true</c> arguments.</param>
		private void HandleFlashChange (object sender, bool args)
		{
			Camera.SwitchFlash (args);
		}

		/// <summary>
		/// Handles the shutter.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void HandleShutter (object sender, EventArgs e)
		{
			Camera.TakePhoto();
		}

		/// <summary>
		/// Handles the focus.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void HandleFocus (object sender, Point e)
		{
			Camera.ChangeFocusPoint(e);
		}

		#endregion
	}
}