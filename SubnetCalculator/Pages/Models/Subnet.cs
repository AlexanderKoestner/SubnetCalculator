using System.Diagnostics.Metrics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;

namespace SubnetCalculator.Pages.Model
{
    public class Subnet
    {
        public string? Test { get; set; }
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

        private List<string>? IpAdressBlocks { get; set; } = new();

        private List<StringBuilder> binaryStringsSubnetMask = new();

        private int firstBlock;
        private int secondBlock;
        private int thirdBlock;
        private int fourthBlock;

        public Subnet()
        {
            SubnetID = new();
            LastHostIP = new();
            RangeOfAdresses = new();
            UsableIPAdresses = new();
            SubnetMask = new();
            SubnetIDBinary = new();
            Broadcast = new();

            binaryStringsSubnetMask.Add(new StringBuilder());
            binaryStringsSubnetMask.Add(new StringBuilder());
            binaryStringsSubnetMask.Add(new StringBuilder());
            binaryStringsSubnetMask.Add(new StringBuilder());
        }

        public void CalcAndWriteAll()
        {
            CalcHosts();
            WriteSubnetMask();
            WriteSubnetID();
            WriteBroadCast();
            WriteRangeOfAdresses();
            WriteUsableIpAdresses();
        }

        private int AdressIncrement(int first, int second, int third, int fourth, int count)
        {
            if(count == 0)
            {
                firstBlock = first;
                secondBlock = second;
                thirdBlock = third;
                fourthBlock = fourth;
                return 0;
            }

            if(count > 256 * 256 * 256)
            {
                AdressIncrement(first + (count / 256 / 256 / 256), second, third, fourth, count / 2);
                return 0;
            }

            if (count > 256 * 256)
            {
                AdressIncrement(first, second + (count / 256 / 256), third, fourth, count / 256);
                return 0;
            }

            if (count > 256)
            {
                AdressIncrement(first, second , third + (count / 256), fourth, count % 256);
                return 0;
            }

            if (count > 1)
            {
                AdressIncrement(first, second, third, fourth + (int)count, 0);
                return 0;
            }

            return 0;
        }

        private void WriteBroadCast()
        {
            firstBlock = Convert.ToInt32(SubnetID.ToString().Split('.')[0]);
            secondBlock = Convert.ToInt32(SubnetID.ToString().Split('.')[1]);
            thirdBlock = Convert.ToInt32(SubnetID.ToString().Split('.')[2]);
            fourthBlock = Convert.ToInt32(SubnetID.ToString().Split('.')[3]);

            AdressIncrement(firstBlock, secondBlock, thirdBlock, fourthBlock, (Hosts + 1));

            if (Suffix == 31)
            {
                fourthBlock -= 2;
            }

            Broadcast.Append(Convert.ToString(firstBlock));
            Broadcast.Append('.');
            Broadcast.Append(Convert.ToString(secondBlock));
            Broadcast.Append('.');
            Broadcast.Append(Convert.ToString(thirdBlock));
            Broadcast.Append('.');
            Broadcast.Append(Convert.ToString(fourthBlock));
        }

        private void WriteRangeOfAdresses()
        {
            RangeOfAdresses.Append(SubnetID);

            if(Suffix < 32)
            {
                
                RangeOfAdresses.Append(" - ");
                RangeOfAdresses.Append(Broadcast);
            }
        }

        private void WriteUsableIpAdresses()
        {
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

        private void WriteSubnetMask()
        {
            int stringCounter = 0;

            binaryStringsSubnetMask[stringCounter].Append('1');

            for (int i = 2; i < 33; i++)
            {
                if(i <= Suffix)
                {
                    binaryStringsSubnetMask[stringCounter].Append('1');
                }
                else
                {
                    binaryStringsSubnetMask[stringCounter].Append('0');
                }

                if(i % 8 == 0)
                {
                    stringCounter++;
                }
            }

            for (int i = 0; i < binaryStringsSubnetMask.Count; i++)
            {
                SubnetMask.Append(Convert.ToString(Convert.ToInt32(binaryStringsSubnetMask[i].ToString(), 2)));

                if(i < binaryStringsSubnetMask.Count-1)
                {
                    SubnetMask.Append('.');
                }
            }

            Test = Convert.ToString(Convert.ToInt32(IpAdress.Split('.')[0], 10), 2).PadLeft(8, '0');
        }

        private void WriteSubnetID()
        {
            IpAdressBlocks.Add(new string(Convert.ToString(Convert.ToInt32(IpAdress.Split('.')[0], 10), 2).PadLeft(8, '0')));
            IpAdressBlocks.Add(new string(Convert.ToString(Convert.ToInt32(IpAdress.Split('.')[1], 10), 2).PadLeft(8, '0')));
            IpAdressBlocks.Add(new string(Convert.ToString(Convert.ToInt32(IpAdress.Split('.')[2], 10), 2).PadLeft(8, '0')));
            IpAdressBlocks.Add(new string(Convert.ToString(Convert.ToInt32(IpAdress.Split('.')[3], 10), 2).PadLeft(8, '0')));

            for (int i = 0; i < IpAdressBlocks.Count; i++)
            {
                for(int k = 0; k < binaryStringsSubnetMask[i].ToString().Length; k++)
                {
                    if (IpAdressBlocks[i].ToCharArray()[k].Equals('1') && binaryStringsSubnetMask[i].ToString().ToCharArray()[k].Equals('1'))
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
