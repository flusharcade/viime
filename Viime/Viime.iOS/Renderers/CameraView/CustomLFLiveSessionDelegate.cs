// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomLFLiveSessionDelegate.cs" company="Flush Arcade Pty Ltd.">
//   Copyright (c) 2016 Flush Arcade Pty Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Viime.iOS.Renderers.CameraView
{
    using System;

    using LFLiveKit;

    /// <summary>
    /// Custom LFL ive session delegate.
    /// </summary>
    public class CustomLFLiveSessionDelegate : LFLiveSessionDelegate, ILFLiveSessionDelegate
    {
        /// <summary>
        /// The live session changed.
        /// </summary>
        private Action<string, bool> _liveSessionChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Viime.iOS.Renderers.CameraView.CustomLFLiveSessionDelegate"/> class.
        /// </summary>
        /// <param name="LiveSessionChanged">Live session changed.</param>
        public CustomLFLiveSessionDelegate(Action<string, bool> LiveSessionChanged)
        {
            _liveSessionChanged = LiveSessionChanged;
        }

        /// <summary>
        /// Lives the state did change.
        /// </summary>
        /// <param name="session">Session.</param>
        /// <param name="state">State.</param>
        public override void LiveStateDidChange(LFLiveSession session, LFLiveState state)
        {
            string stateChangeDescription = null;
            bool turnLiveOff = false;

            switch (state)
            {
                case LFLiveState.Refresh:
                case LFLiveState.Pending:
                    stateChangeDescription = "Connecting";
                    break;
                case LFLiveState.Start:
                    stateChangeDescription = "Connected";
                    break;
                case LFLiveState.Error:
                    stateChangeDescription = "Connection Error";
                    turnLiveOff = true;
                    break;
                case LFLiveState.Ready:
                case LFLiveState.Stop:
                    stateChangeDescription = "Not Connected";
                    turnLiveOff = true;
                    break;
            }

            if (!string.IsNullOrEmpty(stateChangeDescription))
                _liveSessionChanged?.Invoke(stateChangeDescription, turnLiveOff);
        }

        /// <summary>
        /// Errors the code.
        /// </summary>
        /// <param name="session">Session.</param>
        /// <param name="errorCode">Error code.</param>
        public override void ErrorCode(LFLiveSession session, LFLiveSocketErrorCode errorCode)
        {
            Console.WriteLine("ERROR: " + errorCode);
        }

        /// <summary>
        /// Debugs the info.
        /// </summary>
        /// <param name="session">Session.</param>
        /// <param name="debugInfo">Debug info.</param>
        public override void DebugInfo(LFLiveSession session, LFLiveDebug debugInfo)
        {
            base.DebugInfo(session, debugInfo);
        }
    }
}
