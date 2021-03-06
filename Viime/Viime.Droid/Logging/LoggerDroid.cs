﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerDroid.cs" company="Flush Arcade Pty Ltd.">
//   Copyright (c) 2016 Flush Arcade Pty Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Viime.Droid.Logging
{
	using System;
	using System.Diagnostics;

	using Viime.Portable.Logging;

	/// <summary>
	/// Droid log.
	/// </summary>
	public class LoggerDroid : ILogger
	{
		#region IDebug implementation

		/// <summary>
		/// Writes the line.
		/// </summary>
		/// <returns>The line.</returns>
		/// <param name="text">Text.</param>
		public void WriteLine(string text)
		{
			Debug.WriteLine(text);
		}

		/// <summary>
		/// Writes the line time.
		/// </summary>
		/// <returns>The line time.</returns>
		/// <param name="text">Text.</param>
		/// <param name="args">Arguments.</param>
		public void WriteLineTime(string text, params object[] args)
		{
			Debug.WriteLine(DateTime.Now.Ticks + " " + String.Format(text, args));
		}

		#endregion
	}

}