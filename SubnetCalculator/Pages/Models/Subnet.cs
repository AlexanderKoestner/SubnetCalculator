using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace SubnetCalculator.Pages.Model
{
    public class Subnet
    {
        public string? IpAdress { get; set; }
        public StringBuilder? LastHostIP { get; set; }
        public StringBuilder? SubnetID { get; set; }
        public StringBuilder? SubnetMask { get; set; }
        public StringBuilder? RangeOfAdresses { get; set; }
        public StringBuilder? UsableIPAdresses { get; set; }
        public double Hosts { get; set; }
        public int Suffix { get; set; }

        private List<StringBuilder> binaryStringsSubnetMask = new();

        public Subnet()
        {
            SubnetID = new();
            LastHostIP = new();
            RangeOfAdresses = new();
            UsableIPAdresses = new();
            SubnetMask = new();

            binaryStringsSubnetMask.Add(new StringBuilder());
            binaryStringsSubnetMask.Add(new StringBuilder());
            binaryStringsSubnetMask.Add(new StringBuilder());
            binaryStringsSubnetMask.Add(new StringBuilder());
        }

        public void CalcAndWriteAll()
        {
            CalcHosts();
            CalcSubnetIdAndMask();
            WriteRangeOfAdresses();
            WriteUsableIpAdresses();
        }

        private void WriteRangeOfAdresses()
        {
            RangeOfAdresses.Append(SubnetID);

            if(Hosts > 1)
            {
                RangeOfAdresses.Append(" - ");

                int firstByte = Convert.ToInt32(SubnetID.ToString().Split('.')[0]);
                int secondByte = Convert.ToInt32(SubnetID.ToString().Split('.')[1]);
                int thirdByte = Convert.ToInt32(SubnetID.ToString().Split('.')[2]);
                int fourthByte = Convert.ToInt32(SubnetID.ToString().Split('.')[3]);

                for (int i = 0; i < Hosts; i++)
                {
                    fourthByte++;

                    if(fourthByte == 256)
                    {
                        fourthByte = 0;
                        thirdByte++;

                        if(thirdByte == 256)
                        {
                            thirdByte = 0;
                            secondByte++;

                            if(secondByte == 256)
                            {
                                secondByte = 0;
                                firstByte++;
                            }
                        }
                    }
                }

                LastHostIP.Append(firstByte.ToString() + '.');
                LastHostIP.Append(secondByte.ToString() + '.');
                LastHostIP.Append(thirdByte.ToString() + '.');
                LastHostIP.Append((fourthByte - 1).ToString());

                int broadCast = Convert.ToInt32(LastHostIP.ToString().Last() + 1);

                LastHostIP.Length--;

                RangeOfAdresses.Append(LastHostIP.ToString() + broadCast.ToString());

                LastHostIP.Length++;
            }
        }

        private void WriteUsableIpAdresses()
        {
            string subnetID = SubnetID.ToString();

            if (Hosts > 1)
            {
                int newLastChar = Convert.ToInt32(subnetID.Last()) + 1;

                subnetID = subnetID.Remove(subnetID.Length-1);

                UsableIPAdresses.Append(subnetID);
                UsableIPAdresses.Append(newLastChar.ToString());

                UsableIPAdresses.Append(" - ");

                UsableIPAdresses.Append(LastHostIP);
            }
            else
            {
                UsableIPAdresses.Append(subnetID);
            }
        }

        private void CalcHosts()
        {
            Hosts = Math.Pow(2, 32 - Suffix) - 2;
        }

        private void CalcSubnetIdAndMask()
        {
            int stringCounter = 0;

            binaryStringsSubnetMask[stringCounter].Append('1');

            for (int i = 1; i < 32; i++)
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

            StringBuilder binaryStringNewSubnetID = new();

            for(int i = 0; i < binaryStringsSubnetMask.Count; i++)
            {
                SubnetMask.Append(Convert.ToString(Convert.ToInt32(binaryStringsSubnetMask[i].ToString()), 10));
                if (i < binaryStringsSubnetMask.Count - 1)
                {
                    SubnetMask.Append('.');
                }

                if (binaryStringsSubnetMask[i].Equals(Convert.ToString(Convert.ToInt32(IpAdress.Split('.')[i], 10), 2)))
                {
                    SubnetID.Append(IpAdress.Split('.')[i]);
                    if(i < binaryStringsSubnetMask.Count - 1)
                    {
                        SubnetID.Append('.');
                    }
                }
                else
                {
                    foreach(char value in IpAdress.Split('.')[i])
                    {
                        if(value.Equals('1') && IpAdress.Split('.')[i].Equals('1'))
                        {
                            binaryStringNewSubnetID.Append('1');
                        }
                        else
                        {
                            binaryStringNewSubnetID.Append('0');
                        }
                    }

                    SubnetID.Append(Convert.ToString(Convert.ToInt32(binaryStringNewSubnetID.ToString()), 10));
                    if (i < binaryStringsSubnetMask.Count - 1)
                    {
                        SubnetID.Append('.');
                    }
                }
            }
        }
    }
}
