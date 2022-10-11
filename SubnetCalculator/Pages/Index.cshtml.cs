using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SubnetCalculator.Pages.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SubnetCalculator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        [BindProperty, Range(1, 255)]
        public int IpAdressFirstByte { get; set; }

        [BindProperty, Range(0, 255)]
        public int IpAdressSecondByte { get; set; }

        [BindProperty, Range(0, 255)]
        public int IpAdressThirdByte { get; set; }

        [BindProperty, Range(0, 255)]
        public int IpAdressFourthByte { get; set; }

        [BindProperty, Range(1, 32)]
        public int IpAdressSuffix { get; set; }

        public List<Subnet>? Subnets { get; set; }

        public List<Subnet>? OldSubnets { get; set; }

        public string? EmptyStringDot { get; set; } = " . ";
        public string? EmptyStringDash { get; set; } = " / ";

        public string? IpAdress { get; set; }

        [BindProperty]
        public int Counter { get; set; }

        [BindProperty]
        public int Index { get; set; }

        public void OnGet(List<Subnet> Subnets)
        {
            if(Subnets != null)
            {
                OldSubnets = Subnets;
            }
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                Subnets = new();
                IpAdress = IpAdressFirstByte.ToString() + "." + IpAdressSecondByte.ToString() + "." + IpAdressThirdByte.ToString() + "." + IpAdressFourthByte.ToString();
                Subnets.Add(new Subnet());
                Subnets[^1].IpAdress = IpAdress;
                Subnets[^1].Suffix = IpAdressSuffix;
                Subnets[^1].CalcAndWriteAll();

                Counter = 0;

                return RedirectToPage("Index");
            }
            else
            {
                return RedirectToPage("Index");
            }
        }

        public IActionResult OnPostButton_One(IFormCollection Data)
        {
            OldSubnets.Insert(Index, new Subnet());
            OldSubnets[Index].IpAdress = GetNewIpAdress(OldSubnets[Index - 1].Broadcast.ToString());
            OldSubnets[Index].Suffix = Subnets[Index - 1].Suffix;
            OldSubnets[Index].CalcAndWriteAll();

            OldSubnets.Insert(Index + 1, new Subnet());
            OldSubnets[Index].IpAdress = GetNewIpAdress(OldSubnets[Index].Broadcast.ToString());
            OldSubnets[Index].Suffix = Subnets[Index - 1].Suffix;
            OldSubnets[Index].CalcAndWriteAll();

            OldSubnets.RemoveAt(Index - 1);
            Counter = 0;

            return RedirectToPage("Index", new { OldSubnets = this.OldSubnets });
        }

        // Takes Broadcast Adress From previous Subnet Object and returns the next Subnet ID

        private string GetNewIpAdress(string broadCastFromIndex)
        {
            StringBuilder adress = new();

            if (Convert.ToInt32(broadCastFromIndex.Split('.')[1]) + 1 > 255)
            {
                adress.Append(Convert.ToInt32(broadCastFromIndex.Split('.')[0]) + 1);
                adress.Append('.');
            }
            else
            {
                adress.Append(Convert.ToInt32(broadCastFromIndex.Split('.')[0]));
                adress.Append('.');
            }

            if (Convert.ToInt32(broadCastFromIndex.Split('.')[2]) + 1 > 255)
            {
                adress.Append(Convert.ToInt32(broadCastFromIndex.Split('.')[1]) + 1);
                adress.Append('.');
            }
            else
            {
                adress.Append(Convert.ToInt32(broadCastFromIndex.Split('.')[1]));
                adress.Append('.');
            }

            if (Convert.ToInt32(broadCastFromIndex.Split('.')[3]) + 1 > 255)
            {
                adress.Append(Convert.ToInt32(broadCastFromIndex.ToString().Split('.')[2]) + 1);
                adress.Append('.');
                adress.Append('0');
            }
            else
            {
                adress.Append(Convert.ToInt32(broadCastFromIndex.ToString().Split('.')[2]));
                adress.Append('.');
                adress.Append(Convert.ToInt32(broadCastFromIndex.ToString().Split('.')[3]) + 1);
            }

            return adress.ToString();
        }
    }
}