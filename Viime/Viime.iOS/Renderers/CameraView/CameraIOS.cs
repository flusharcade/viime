// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraIOS.cs" company="Flush Arcade Pty Ltd.">
//   Copyright (c) 2016 Flush Arcade Pty Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Viime.iOS.Renderers.CameraView
{
    using System;
    using System.Threading.Tasks;
    using System.Globalization;

    using Foundation;
    using UIKit;
    using AVFoundation;
    using CoreGraphics;

    using LFLiveKit;

    using Viime.Portable.Enums;
    using Viime.Portable.Logging;
    using Viime.Portable.Ioc;
    using Viime.Portable.Resources;

    /// <summary>
    /// Camera ios.
    /// </summary>
    public sealed class CameraIOS : UIView, ILFLiveSessionDelegate
    {
        #region Private Properties

        /// <summary>
        /// The tag.
        /// </summary>
        private readonly string _tag;

        /// <summary>
        /// The log.
        /// </summary>
        private readonly ILogger _log;

        /// <summary>
        /// The main view.
        /// </summary>
        private UIView _mainView;

        /// <summary>
        /// The camera busy.
        /// </summary>
        private bool _cameraBusy;

        /// <summary>
        /// The camera available.
        /// </summary>
        private bool _cameraAvailable;

        /// <summary>
        /// The width of the camera button container.
        /// </summary>
        private float _cameraButtonContainerWidth;

        /// <summary>
        /// The system version.
        /// </summary>
        private double _systemVersion;

        /// <summary>
        /// The width.
        /// </summary>
        private nint _width;

        /// <summary>
        /// The height.
        /// </summary>
        private nint _height;

        /// <summary>
        /// The rtmp URL.
        /// </summary>
        private string rtmpUrl = Config.MediaServicesRtmpUrl + "/myStream";

        /// <summary>
        /// The session.
        /// </summary>
        private LFLiveSession _session;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when busy.
        /// </summary>
        public event EventHandler<bool> Busy;

        /// <summary>
        /// Occurs when available.
        /// </summary>
        public event EventHandler<bool> Available;

        /// <summary>
        /// Occurs when photo.
        /// </summary>
        public event EventHandler<byte[]> Photo;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Camera.iOS.Renderers.CameraView.CameraIOS"/> class.
        /// </summary>
        public CameraIOS()
        {
            _log = IoC.Resolve<ILogger>();
            _tag = $"{GetType()} ";

            // retrieve system version 
            var versionParts = UIDevice.CurrentDevice.SystemVersion.Split ('.');
            var versionString = versionParts [0] + "." + versionParts [1];
            _systemVersion = Convert.ToDouble (versionString, CultureInfo.InvariantCulture);

            _mainView = new UIView () { TranslatesAutoresizingMaskIntoConstraints = false };
            AutoresizingMask = UIViewAutoresizing.FlexibleMargins;

            _session = new LFLiveSession(LFLiveAudioConfiguration.DefaultConfigurationForQuality(LFLiveAudioQuality.High),
                    LFLiveVideoConfiguration.DefaultConfigurationForQuality((LFLiveVideoQuality.Low3)));

            // retrieve camera device if available
            _cameraAvailable = true;

            Add (_mainView);

            // set layout constraints for main view
            AddConstraints (NSLayoutConstraint.FromVisualFormat("V:|[mainView]|", NSLayoutFormatOptions.DirectionLeftToRight, null, new NSDictionary("mainView", _mainView)));
            AddConstraints (NSLayoutConstraint.FromVisualFormat("H:|[mainView]|", NSLayoutFormatOptions.AlignAllTop, null, new NSDictionary ("mainView", _mainView)));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the start orientation.
        /// </summary>
        private void SetStartOrientation()
        {
            Orientation sOrientation = Orientation.None;

            switch (UIApplication.SharedApplication.StatusBarOrientation)
            {
                case UIInterfaceOrientation.Portrait:
                case UIInterfaceOrientation.PortraitUpsideDown:
                    sOrientation = Orientation.Portrait;
                    break;
                case UIInterfaceOrientation.LandscapeLeft:
                    sOrientation = Orientation.LandscapeLeft;
                    break;
                case UIInterfaceOrientation.LandscapeRight:
                    sOrientation = Orientation.LandscapeRight;
                    break;
            }

            //HandleOrientationChange(sOrientation);
        }

        /// <summary>
        /// Sets the busy.
        /// </summary>
        /// <param name="busy">If set to <c>true</c> busy.</param>
        private void SetBusy(bool busy)
        {
            _cameraBusy = busy;

            // set camera busy 
            //Busy?.Invoke(this, _cameraBusy);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Takes the video.
        /// </summary>
        /// <returns>The photo.</returns>
        public void TakeVideo()
        {
            try
            {
                if (!_cameraBusy)
                {
                    SetBusy(true);

                    var stream = new LFLiveStreamInfo();

                    stream.Url = rtmpUrl;

                    var audioConfiguration = new LFLiveAudioConfiguration
                    {
                        AudioBitrate = LFLiveAudioBitRate.LFLiveAudioBitRate_96Kbps,
                        AudioSampleRate = LFLiveAudioSampleRate.LFLiveAudioSampleRate_44100Hz
                    };

                    var videoConfiguration = new LFLiveVideoConfiguration
                    {
                        VideoFrameRate = 30,
                        VideoMaxFrameRate = 30,
                        VideoMinFrameRate = 30,
                        VideoBitRate = 4200,
                        VideoMaxBitRate = 4200,
                        VideoMinBitRate = 4200,
                        VideoMaxKeyframeInterval = 60,
                        SessionPreset = LFLiveVideoSessionPreset.LFCaptureSessionPreset720x1280,
                        VideoSize = new CGSize(720, 1280)
                    };

                    stream.AudioConfiguration = audioConfiguration;
                    stream.VideoConfiguration = videoConfiguration;

                    _session.AdaptiveBitrate = false;
                    _session.ReconnectCount = 10;

                    ToggleLiveStream(true, stream);

                    var info = _session.StreamInfo;
                }
                else
                {
                    ToggleLiveStream(false);
                }
            }
            catch (Exception error)
            {
                _log.WriteLineTime(_tag + "\n" +
                    "CaptureImageWithMetadata() Failed to take photo \n " +
                    "ErrorMessage: \n" +
                    error.Message + "\n" +
                    "Stacktrace: \n " +
                    error.StackTrace);
            }
        }

        /// <summary>
        /// Sets the bounds.
        /// </summary>
        /// <returns>The bounds.</returns>
        public void SetBounds(nint width, nint height)
        {
            _height = height;
            _width = width;
        }

        /// <summary>
        /// Initializes the camera.
        /// </summary>
        /// <returns>The camera.</returns>
        public async Task InitializeCamera()
        {
            try 
            {
                var lfLiveSessionDelegate = new CustomLFLiveSessionDelegate(LiveSessionChanged);

                _session.Delegate = lfLiveSessionDelegate;
                _session.PreView = _mainView;

                await RequestAccessForVideo();
            }
            catch (Exception error) 
            {
                _log.WriteLineTime(_tag + "\n" +
                    "InitializeCamera() Camera failed to initialise \n " +
                    "ErrorMessage: \n" +
                    error.Message + "\n" +
                    "Stacktrace: \n " +
                    error.StackTrace);    
            }

            Available?.Invoke(this, _cameraAvailable);

            _log.WriteLineTime(_tag + "\n" + "RetrieveCameraDevice() Camera initalised \n ");
        }

        /// <summary>
        /// Lives the session changed.
        /// </summary>
        /// <param name="stateChangeDescription">State change description.</param>
        /// <param name="turnLiveOff">If set to <c>true</c> turn live off.</param>
        void LiveSessionChanged(string stateChangeDescription, bool turnLiveOff)
        {
            if (turnLiveOff)
                ToggleLiveStream(false);
        }

        /// <summary>
        /// Toggles the live stream.
        /// </summary>
        /// <param name="startLive">If set to <c>true</c> start live.</param>
        /// <param name="stream">Stream.</param>
        void ToggleLiveStream(bool startLive, LFLiveStreamInfo stream = null)
        {
            if (stream != null) 
            {
                if (startLive)
                {
                    _session.StartLive(stream);
                }
                else
                {
                    _session.StopLive();
                }
            }
        }

        async Task RequestAccessForVideo()
        {
            var status = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);

            switch (status)
            {
                case AVAuthorizationStatus.NotDetermined:
                    var granted = await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
                    if (granted)
                        _session.Running = true;
                    break;
                case AVAuthorizationStatus.Authorized:
                    _session.Running = true;
                    break;
                case AVAuthorizationStatus.Denied:
                case AVAuthorizationStatus.Restricted:
                default:
                    break;
            }
        }

        /// <summary>
        /// Sets the widths.
        /// </summary>
        /// <param name="cameraButtonContainerWidth">Camera button container width.</param>
        public void SetWidths(float cameraButtonContainerWidth)
        {
            _cameraButtonContainerWidth = cameraButtonContainerWidth;
        }

        /// <summary>
        /// Stops the and dispose.
        /// </summary>
        public void StopAndDispose()
        {
            _session.StopLive();
        }

        #endregion
    }
}