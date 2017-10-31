using System;

using Refit;

namespace Viime.Portable.WebServices
{
    /// <summary>
    /// Auth body.
    /// </summary>
    public class AuthBody
    {
        [AliasAs("grant_type")]
        public string GrantType { get; set; }

        [AliasAs("client_id")]
        public string ClientId { get; set; }

        [AliasAs("client_secret")]
        public string ClientSecret { get; set; }

        [AliasAs("resource")]
        public string Resource { get; set; }
    }
}
