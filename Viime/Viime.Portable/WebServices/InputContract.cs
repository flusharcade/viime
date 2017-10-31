using System;
using System.Collections.Generic;

namespace Viime.Portable.WebServices
{
    public class InputContract
    {
        public object KeyFrameInterval { get; set; }
        public string StreamingProtocol { get; set; }
        public AccessControlContract AccessControl { get; set; }
        public List<EndpointContract> Endpoints { get; set; }
    }
}
