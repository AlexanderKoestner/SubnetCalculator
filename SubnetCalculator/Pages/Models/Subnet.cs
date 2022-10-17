using Newtonsoft.Json;
using System.Text;

namespace SubnetCalculator.Pages.Models
{
    public class Subnet
    {
        public string? IpAdress { get; set; }
        public StringBuilder? LastHostIP { get; set; }
        public StringBuilder? SubnetID { get; set; }
        public StringBuilder? SubnetIDBinary { get; set; }
        public StringBuilder? SubnetMask { get; set; }
        public StringBuilder? RangeOfAdresses { get; set; }
        public StringBuilder? UsableIPAdresses { get; set; }
        public StringBuilder? Broadcast { get; set; }
        public int Hosts { get; set; }
        public int Suffix { get; set; }

        private List<string>? IpAdressBlocks { get; set; }

        private List<StringBuilder>? BinaryStringsSubnetMask { get; set; }

        public int FirstBlock { get; set; }
        public int SecondBlock { get; set; }
        public int ThirdBlock { get; set; }
        public int FourthBlock { get; set; }

        public Subnet()
        {
            SubnetID = new();
            LastHostIP = new();
            RangeOfAdresses = new();
            UsableIPAdresses = new();
            SubnetMask = new();
            SubnetIDBinary = new();
            Broadcast = new();
            BinaryStringsSubnetMask = new();
            IpAdressBlocks = new();
        }

        // Executes all Functions in correct Sequence

        public void CalcAndWriteAll()
        {
            CalcHosts();
            WriteSubnetMask();
            WriteSubnetID();
            WriteBroadCast();
            WriteRangeOfAdresses();
            WriteUsableIpAdresses();
        }

        // Increments the Subnet ID by the number of Hosts to generate a Broadcast IP

        private int AdressIncrement(int first, int second, int third, int fourth, int count)
        {
            // Check if Incrementing is completed
            // If true: Stop Recursion


            if(count == 0)
            {
                FirstBlock = first;
                SecondBlock = second;
                ThirdBlock = third;
                FourthBlock = fourth;
                return 0;
            }

            // Check if the fourth Byte block needs to be incremented

            if (count <= Math.Pow(2, 8))
            {
                AdressIncrement(first, second, third, fourth + count, 0);
                return 0;
            }

            // Check if the third Byte block needs to be incremented

            if (count <= Math.Pow(2, 16))
            {
                AdressIncrement(first, second, third + (count / 256), fourth, count % 256);
                return 0;
            }

            // Check if the second Byte block needs to be incremented

            if (count <= Math.Pow(2, 24))
            {
                AdressIncrement(first, second + (count / 256 / 256), third, fourth, count % (256 * 256));
                return 0;
            }

            // Check if the first Byte block needs to be incremented

            if (count <= Math.Pow(2, 31))
            {
                AdressIncrement(first + (count / 256 / 256 / 256), second, third, fourth, count % (256 * 256 * 256));
                return 0;
            }
            return 0;
        }

        // Writes the Broadcast String

        private void WriteBroadCast()
        {   
            // Splits the Subnet ID into 4 seperate Byte blocks
            FirstBlock = Convert.ToInt32(SubnetID.ToString().Split('.')[0]);
            SecondBlock = Convert.ToInt32(SubnetID.ToString().Split('.')[1]);
            ThirdBlock = Convert.ToInt32(SubnetID.ToString().Split('.')[2]);
            FourthBlock = Convert.ToInt32(SubnetID.ToString().Split('.')[3]);

            // Increments the seperated Blocks

            AdressIncrement(FirstBlock, SecondBlock, ThirdBlock, FourthBlock, Hosts + 1);

            // Check if the Suffix equals 31 (special case for the /31 IP Adresses

            if (Suffix == 31)
            {
                FourthBlock -= 2;
            }

            // Creates the Broadcast from Byte blocks

            Broadcast.Clear();

            Broadcast.Append(Convert.ToString(FirstBlock));
            Broadcast.Append('.');
            Broadcast.Append(Convert.ToString(SecondBlock));
            Broadcast.Append('.');
            Broadcast.Append(Convert.ToString(ThirdBlock));
            Broadcast.Append('.');
            Broadcast.Append(Convert.ToString(FourthBlock));
        }

        // Writes the Range Of Adresses String

        private void WriteRangeOfAdresses()
        {
            RangeOfAdresses.Clear();
            RangeOfAdresses.Append(SubnetID);

            if(Suffix < 32)
            {
                
                RangeOfAdresses.Append(" - ");
                RangeOfAdresses.Append(Broadcast);
            }
        }

        // Writes the usable IP Adresses String

        private void WriteUsableIpAdresses()
        {
            UsableIPAdresses.Clear();
            UsableIPAdresses.Append(SubnetID.ToString().Split('.')[0]);
            UsableIPAdresses.Append('.');
            UsableIPAdresses.Append(SubnetID.ToString().Split('.')[1]);
            UsableIPAdresses.Append('.');
            UsableIPAdresses.Append(SubnetID.ToString().Split('.')[2]);
            UsableIPAdresses.Append('.');


            if (Suffix < 32)
            {
                if(Suffix == 31)
                {
                    UsableIPAdresses.Append(SubnetID.ToString().Split('.')[3]);
                    UsableIPAdresses.Append(" - ");
                    UsableIPAdresses.Append(Broadcast.ToString().ToString().Split('.')[0]);
                    UsableIPAdresses.Append('.');
                    UsableIPAdresses.Append(Broadcast.ToString().ToString().Split('.')[1]);
                    UsableIPAdresses.Append('.');
                    UsableIPAdresses.Append(Broadcast.ToString().ToString().Split('.')[2]);
                    UsableIPAdresses.Append('.');
                    UsableIPAdresses.Append(Convert.ToString(Convert.ToInt32(Broadcast.ToString().Split('.')[3])));
                }
                else
                {
                    UsableIPAdresses.Append(Convert.ToString(Convert.ToInt32(SubnetID.ToString().Split('.')[3]) + 1));
                    UsableIPAdresses.Append(" - ");
                    UsableIPAdresses.Append(Broadcast.ToString().ToString().Split('.')[0]);
                    UsableIPAdresses.Append('.');
                    UsableIPAdresses.Append(Broadcast.ToString().ToString().Split('.')[1]);
                    UsableIPAdresses.Append('.');
                    UsableIPAdresses.Append(Broadcast.ToString().ToString().Split('.')[2]);
                    UsableIPAdresses.Append('.');
                    UsableIPAdresses.Append(Convert.ToString(Convert.ToInt32(Broadcast.ToString().Split('.')[3]) - 1));
                }
            }
            else
            {
                UsableIPAdresses.Append(SubnetID.ToString().Split('.')[3]);
            }
        }

        // Calculates the number of usable Hosts depending on the Suffix Input

        private void CalcHosts()
        {
            int exponent = 32 - Suffix;

            if(Suffix == 32)
            {
                Hosts = 1;
            }
            else if(Suffix == 31)
            {
                Hosts = 2;
            }
            else
            {
                Hosts = (int)Math.Pow(2, exponent) - 2;
            }
        }

        // Writes the Subnet Mask depending on the Suffix Input

        private void WriteSubnetMask()
        {
            SubnetMask.Clear();
            BinaryStringsSubnetMask.Clear();

            BinaryStringsSubnetMask.Add(new StringBuilder());
            BinaryStringsSubnetMask.Add(new StringBuilder());
            BinaryStringsSubnetMask.Add(new StringBuilder());
            BinaryStringsSubnetMask.Add(new StringBuilder());

            int stringCounter = 0;

            BinaryStringsSubnetMask[stringCounter].Append('1');

            // Writes the Subnet Mask in a binary represantion

            for (int i = 2; i < 33; i++)
            {
                if(i <= Suffix)
                {
                    BinaryStringsSubnetMask[stringCounter].Append('1');
                }
                else
                {
                    BinaryStringsSubnetMask[stringCounter].Append('0');
                }

                if(i % 8 == 0)
                {
                    stringCounter++;
                }
            }

            // Writes the Subnet Mask from Binary into Base 10

            for (int i = 0; i < BinaryStringsSubnetMask.Count; i++)
            {
                SubnetMask.Append(Convert.ToString(Convert.ToInt32(BinaryStringsSubnetMask[i].ToString(), 2)));

                if(i < BinaryStringsSubnetMask.Count-1)
                {
                    SubnetMask.Append('.');
                }
            }
        }

        private void WriteSubnetID()
        {
            SubnetID.Clear();
            IpAdressBlocks.Clear();

            // Splits the IP Adress into 4 separate Byte blocks in binary

            IpAdressBlocks.Add(new string(Convert.ToString(Convert.ToInt32(IpAdress.Split('.')[0], 10), 2).PadLeft(8, '0')));
            IpAdressBlocks.Add(new string(Convert.ToString(Convert.ToInt32(IpAdress.Split('.')[1], 10), 2).PadLeft(8, '0')));
            IpAdressBlocks.Add(new string(Convert.ToString(Convert.ToInt32(IpAdress.Split('.')[2], 10), 2).PadLeft(8, '0')));
            IpAdressBlocks.Add(new string(Convert.ToString(Convert.ToInt32(IpAdress.Split('.')[3], 10), 2).PadLeft(8, '0')));

            // Writes the Subnet ID by comparing the Binary Version of the Subnet Mask to the Binary Version of the IP Adress

            for (int i = 0; i < IpAdressBlocks.Count; i++)
            {
                for(int k = 0; k < BinaryStringsSubnetMask[i].ToString().Length; k++)
                {
                    if (IpAdressBlocks[i].ToCharArray()[k].Equals('1') && BinaryStringsSubnetMask[i].ToString().ToCharArray()[k].Equals('1'))
                    {
                        SubnetIDBinary.Append('1');
                    }
                    else
                    {
                        SubnetIDBinary.Append('0');
                    }
                }

                if(i < IpAdressBlocks.Count-1)
                {
                    SubnetIDBinary.Append('.');
                }
            }

            // Creates the Subnet ID in Base 10 from the 4 Subnet ID Binary blocks

            SubnetID.Append(Convert.ToString(Convert.ToInt32(SubnetIDBinary.ToString().Split('.')[0], 2), 10));
            SubnetID.Append('.');
            SubnetID.Append(Convert.ToString(Convert.ToInt32(SubnetIDBinary.ToString().Split('.')[1], 2), 10));
            SubnetID.Append('.');
            SubnetID.Append(Convert.ToString(Convert.ToInt32(SubnetIDBinary.ToString().Split('.')[2], 2), 10));
            SubnetID.Append('.');
            SubnetID.Append(Convert.ToString(Convert.ToInt32(SubnetIDBinary.ToString().Split('.')[3], 2), 10));
        }
    }
}
