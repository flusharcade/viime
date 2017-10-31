// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileStorable.cs" company="Flush Arcade Pty Ltd.">
//   Copyright (c) 2015 Flush Arcade Pty Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Viime.Portable.DataAccess.Storable
{
	using Newtonsoft.Json;

	using SQLite.Net.Attributes;

    using Viime.Portable.Enums;

    /// <summary>
    /// Settings storable.
    /// </summary>
    public class SettingsStorable : IStorable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Viime.Portable.DataAccess.Storable.SettingsStorable"/> class.
        /// </summary>
        public SettingsStorable()
        {
            Key = StorableKeys.Settings.ToString();
        }

        #region Public Properties

		/// <summary>
		/// Gets or sets the key.
		/// </summary>
		/// <value>The key.</value>
      	[PrimaryKey]
		public string Key { get; set; }

		/// <summary>
        /// Gets or sets the bearer token.
        /// </summary>
        /// <value>The bearer token.</value>
		public string BearerToken { get; set; }

        #endregion
    }
}