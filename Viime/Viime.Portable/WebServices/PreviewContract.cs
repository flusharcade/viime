using System;
using System.Collections.Generic;

namespace Viime.Portable.WebServices
{
    public class PreviewContract
    {
        public AccessControlContract AccessControl { get; set; }
        public List<EndpointContract> Endpoints { get; set; }
    }
}
