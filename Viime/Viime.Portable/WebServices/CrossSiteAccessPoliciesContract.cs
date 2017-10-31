using System;
namespace Viime.Portable.WebServices
{
    public class CrossSiteAccessPoliciesContract
    {
        public object ClientAccessPolicy { get; set; }
        public object CrossDomainPolicy { get; set; }
    }
}
