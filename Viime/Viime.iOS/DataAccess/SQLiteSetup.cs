// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SQLiteSetup.cs" company="Flush Arcade Pty Ltd.">
//   Copyright (c) 2015 Flush Arcade Pty Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Viime.iOS.DataAccess
{
	using System.IO;
	using System;

	using SQLite.Net.Interop;

    using Viime.Portable.DataAccess;
    using Viime.Portable.DataAccess.Storage;

	/// <summary>
	/// The SQLite setup object.
	/// </summary>
	public class SQLiteSetup : ISQLiteSetup
	{
		public string DatabasePath { get; set; }

		public ISQLitePlatform Platform { get; set; }

		public SQLiteSetup(ISQLitePlatform platform)
		{
			DatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "viime.db3");;
			Platform = platform;
		}
	}
}