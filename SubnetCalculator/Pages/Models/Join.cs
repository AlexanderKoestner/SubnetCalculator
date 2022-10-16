using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;

namespace SubnetCalculator.Pages.Models
{
    public class Join
    {
        public int InputSuffix { get; set; }
        public int CurrentIndex { get; set; }
        public string? InputIpAdress { get; set; }
        public List<Subnet>? Subnets { get; set; }

        private Subnet newSubnet;

        public Join(int inputSuffix, int currentIndex, string inputIp, List<Subnet> subnets)
        {
            this.InputSuffix = inputSuffix;
            this.CurrentIndex = currentIndex;
            this.InputIpAdress = inputIp;
            this.Subnets = subnets;
            newSubnet = new();
        }

    }
}
