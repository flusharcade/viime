using System;
namespace Viime.Portable.WebServices
{
    public class AllowContract
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int SubnetPrefixLength { get; set; }
    }
}
