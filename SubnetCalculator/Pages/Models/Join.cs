using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using System.Text;

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

        // Check Position of Current Index in List Subnets

        public int CheckPositon()
        {
            if(CurrentIndex == 0)
            {
                return 0;
            }

            if(CurrentIndex == Subnets.Count - 1)
            {
                return 1;
            }

            return -1;
        }

        // Rewrite the Subnet List based on the current Index to Join

        public void WriteNewSubnetList()
        {
            newSubnet.Suffix = Subnets[CurrentIndex].Suffix - 1;
            newSubnet.IpAdress = Subnets[CurrentIndex].SubnetID.ToString();
            newSubnet.CalcAndWriteAll();

            if (CheckPositon() == 0)
            {
                for(int i = 1; i < Subnets.Count; i++)
                {
                    if (newSubnet.Broadcast.Equals(Subnets[i].Broadcast))
                    {
                        Subnets.RemoveRange(CurrentIndex, i + 1);
                        Subnets.Insert(0, newSubnet);
                        break;
                    }
                }
            }
            else if(CheckPositon() == 1)
            {
                for (int i = CurrentIndex - 1; i >= 0; i--)
                {
                    if (newSubnet.SubnetID.Equals(Subnets[i].SubnetID))
                    {
                        newSubnet = new();
                        newSubnet.Suffix = Subnets[CurrentIndex].Suffix - 1;
                        newSubnet.IpAdress = Subnets[i].SubnetID.ToString();
                        newSubnet.CalcAndWriteAll();

                        Subnets.RemoveRange(i, CurrentIndex + 1 - i);
                        Subnets.Add(newSubnet);
                        break;
                    }
                }
            }
            else
            {
                bool up = false;
                bool down = false;

                for(int i = 0; i < Subnets.Count - 1; i++)
                {
                    if (newSubnet.SubnetID.Equals(Subnets[i].SubnetID) && i != CurrentIndex)
                    {
                        if(i < CurrentIndex)
                        {
                            up = true;
                        }
                        break;
                    }
                }

                if(up)
                {
                    for (int i = CurrentIndex - 1; i >= 0; i--)
                    {
                        if (newSubnet.SubnetID.Equals(Subnets[i].SubnetID))
                        {
                            newSubnet = new();
                            newSubnet.Suffix = Subnets[CurrentIndex].Suffix - 1;
                            newSubnet.IpAdress = Subnets[i].SubnetID.ToString();
                            newSubnet.CalcAndWriteAll();

                            Subnets.RemoveRange(i, CurrentIndex + 1 - i);
                            Subnets.Insert(i, newSubnet);
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = CurrentIndex + 1; i < Subnets.Count; i++)
                    {
                        if (newSubnet.Broadcast.Equals(Subnets[i].Broadcast))
                        {
                            Subnets.RemoveRange(CurrentIndex, i - CurrentIndex + 1);
                            Subnets.Insert(CurrentIndex, newSubnet);
                            break;
                        }
                    }
                }
            }
        }

    }
}
