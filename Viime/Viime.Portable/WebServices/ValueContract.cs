using System;

namespace Viime.Portable.WebServices
{
    public class ValueContract
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public string State { get; set; }
        public InputContract Input { get; set; }
        public PreviewContract Preview { get; set; }
        public object Output { get; set; }
        public CrossSiteAccessPoliciesContract CrossSiteAccessPolicies { get; set; }
        public string EncodingType { get; set; }
        public object Encoding { get; set; }
        public object Slate { get; set; }
    }
}
