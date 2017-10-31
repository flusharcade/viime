// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FocusViewRenderer.cs" company="Flush Arcade Pty Ltd.">
//   Copyright (c) 2016 Flush Arcade Pty Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

[assembly: Xamarin.Forms.ExportRenderer(typeof(Viime.Controls.FocusView), typeof(Viime.Droid.Renderers.FocusViewRenderer))]

namespace Viime.Droid.Renderers
{
	using System.Reactive.Linq;
	using System;

	using Xamarin.Forms;
	using Xamarin.Forms.Platform.Android;

	using Android.Views;
	using Android.Widget;

	using Viime.Controls;

	/// <summary>
	/// Focus view renderer.
	/// </summary>
	public class FocusViewRenderer : ViewRenderer<FocusView, LinearLayout>
	{
		#region Private Methods

		/// <summary>
		/// The listener.
		/// </summary>
		private FocusViewGestureDetector _gestureDetector;

		/// <summary>
		/// The detector.
		/// </summary>
		private GestureDetector _detector;

		/// <summary>
		/// The layout.
		/// </summary>
		private LinearLayout _layout;

		#endregion

		#region Protected Methods

		/// <summary>
		/// Raises the element changed event.
		/// </summary>
		/// <param name="e">E.</param>
		protected override void OnElementChanged(ElementChangedEventArgs<FocusView> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
			{
				SetGestureDetectorListener();
				_layout = new LinearLayout(Context);

				SetNativeControl (_layout);
			}

			if (e.NewElement != null)
			{
				_layout.Touch += HandleTouch;
			}
		}

		/// <summary>
		/// Dispose the specified disposing.
		/// </summary>
		/// <param name="disposing">If set to <c>true</c> disposing.</param>
		protected override void Dispose(bool disposing)
		{
			_layout.Touch -= HandleTouch;

			base.Dispose(disposing);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Converts the pixels to dp.
		/// </summary>
		/// <returns>The pixels to dp.</returns>
		/// <param name="pixelValue">Pixel value.</param>
		private int ConvertPixelsToDp(float pixelValue)
		{
			return (int) ((pixelValue)/Resources.DisplayMetrics.Density);
		}

		/// <summary>
		/// Sets the gesture detector listener.
		/// </summary>
		private void SetGestureDetectorListener()
		{
			// assign gesture for detecting touch events on camera view
			_gestureDetector = new FocusViewGestureDetector ();
			_detector = new GestureDetector (_gestureDetector);

			// allow focus change every 0.7 seconds
			Observable.FromEventPattern<MotionEvent> (_gestureDetector, "Touch")
				.Window (() => Observable.Interval (TimeSpan.FromSeconds (0.7)))
				.SelectMany (x => x.Take (1))
				.Subscribe (e => Element.NotifyFocus (new Point (ConvertPixelsToDp (e.EventArgs.GetX ()), 
			                                                          ConvertPixelsToDp (e.EventArgs.GetY ()))));
		}

		/// <summary>
		/// Handles the touch.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void HandleTouch (object sender, TouchEventArgs e)
		{
			_detector.OnTouchEvent (e.Event);
		}

		#endregion
	}
}