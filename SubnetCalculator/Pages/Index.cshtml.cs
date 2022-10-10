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

        public List<Subnet> Subnets { get; set; } = new();

        public string EmptyStringDot { get; set; } = " . ";
        public string EmptyStringDash { get; set; } = " / ";

        public string IpAdress { get; set; }

        public StringBuilder Adress { get; set; } = new();

        [BindProperty]
        public int SubnetCount { get; set; }

        [BindProperty]
        public int Counter { get; set; }

        [BindProperty]
        public ButtonList Buttons { get; set; }

        public void OnGet()
        {
            Buttons = new();
            SubnetCount = Subnets.Count;
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                IpAdress = IpAdressFirstByte.ToString() + "." + IpAdressSecondByte.ToString() + "." + IpAdressThirdByte.ToString() + "." + IpAdressFourthByte.ToString();
                Subnets.Add(new Subnet());
                Subnets[^1].IpAdress = IpAdress;
                Subnets[^1].Suffix = IpAdressSuffix;
                Subnets[^1].CalcAndWriteAll();
                this.SubnetCount = Subnets.Count();

                return Page();
            }
            else
            {
                return RedirectToPage("Index");
            }
        }

        public IActionResult OnPostButtonOne()
        {
            OnPost();
            Subnets.Add(new Subnet());

            if (Convert.ToInt32(Subnets[0].Broadcast.ToString().Split('.')[1]) + 1 > 255)
            {
                Adress.Append(Convert.ToInt32(Subnets[0].Broadcast.ToString().Split('.')[0]) + 1);
                Adress.Append('.');
            }
            else
            {
                Adress.Append(Convert.ToInt32(Subnets[0].Broadcast.ToString().Split('.')[0]));
                Adress.Append('.');
            }

            if (Convert.ToInt32(Subnets[0].Broadcast.ToString().Split('.')[2]) + 1 > 255)
            {
                Adress.Append(Convert.ToInt32(Subnets[0].Broadcast.ToString().Split('.')[1]) + 1);
                Adress.Append('.');
            }
            else
            {
                Adress.Append(Convert.ToInt32(Subnets[0].Broadcast.ToString().Split('.')[1]));
                Adress.Append('.');
            }

            if (Convert.ToInt32(Subnets[0].Broadcast.ToString().Split('.')[3]) + 1 > 255)
            {
                Adress.Append(Convert.ToInt32(Subnets[0].Broadcast.ToString().Split('.')[2]) + 1);
                Adress.Append('.');
                Adress.Append('0');
            }
            else
            {
                Adress.Append(Convert.ToInt32(Subnets[0].Broadcast.ToString().Split('.')[2]));
                Adress.Append('.');
                Adress.Append(Convert.ToInt32(Subnets[0].Broadcast.ToString().Split('.')[3]) + 1);
            }

            Subnets[^1].IpAdress = Adress.ToString();
            Subnets[^1].Suffix = Subnets[0].Suffix + 1;
            Subnets[^1].CalcAndWriteAll();

            Subnets.Add(new Subnet());

            Adress.Clear();

            if (Convert.ToInt32(Subnets[1].Broadcast.ToString().Split('.')[1]) + 1 > 255)
            {
                Adress.Append(Convert.ToInt32(Subnets[1].Broadcast.ToString().Split('.')[0]) + 1);
                Adress.Append('.');
            }
            else
            {
                Adress.Append(Convert.ToInt32(Subnets[1].Broadcast.ToString().Split('.')[0]));
                Adress.Append('.');
            }

            if (Convert.ToInt32(Subnets[1].Broadcast.ToString().Split('.')[2]) + 1 > 255)
            {
                Adress.Append(Convert.ToInt32(Subnets[1].Broadcast.ToString().Split('.')[1]) + 1);
                Adress.Append('.');
            }
            else
            {
                Adress.Append(Convert.ToInt32(Subnets[1].Broadcast.ToString().Split('.')[1]));
                Adress.Append('.');
            }

            if (Convert.ToInt32(Subnets[1].Broadcast.ToString().Split('.')[3]) + 1 > 255)
            {
                Adress.Append(Convert.ToInt32(Subnets[1].Broadcast.ToString().Split('.')[2]) + 1);
                Adress.Append('.');
                Adress.Append('0');
            }
            else
            {
                Adress.Append(Convert.ToInt32(Subnets[1].Broadcast.ToString().Split('.')[2]));
                Adress.Append('.');
                Adress.Append(Convert.ToInt32(Subnets[1].Broadcast.ToString().Split('.')[3]) + 1);
            }

            Subnets[^1].IpAdress = Adress.ToString();
            Subnets[^1].Suffix = Subnets[0].Suffix + 1;
            Subnets[^1].CalcAndWriteAll();
            this.SubnetCount = Subnets.Count();

            return Page();
        }

        public IActionResult OnPostButtonTwo()
        {
            OnPost();
            OnPostButtonOne();

            Adress.Clear();

            if (Convert.ToInt32(Subnets[2].Broadcast.ToString().Split('.')[1]) + 1 > 255)
            {
                Adress.Append(Convert.ToInt32(Subnets[2].Broadcast.ToString().Split('.')[0]) + 1);
                Adress.Append('.');
            }
            else
            {
                Adress.Append(Convert.ToInt32(Subnets[2].Broadcast.ToString().Split('.')[0]));
                Adress.Append('.');
            }

            if (Convert.ToInt32(Subnets[2].Broadcast.ToString().Split('.')[2]) + 1 > 255)
            {
                Adress.Append(Convert.ToInt32(Subnets[2].Broadcast.ToString().Split('.')[1]) + 1);
                Adress.Append('.');
            }
            else
            {
                Adress.Append(Convert.ToInt32(Subnets[2].Broadcast.ToString().Split('.')[1]));
                Adress.Append('.');
            }

            if (Convert.ToInt32(Subnets[2].Broadcast.ToString().Split('.')[3]) + 1 > 255)
            {
                Adress.Append(Convert.ToInt32(Subnets[2].Broadcast.ToString().Split('.')[2]) + 1);
                Adress.Append('.');
                Adress.Append('0');
            }
            else
            {
                Adress.Append(Convert.ToInt32(Subnets[2].Broadcast.ToString().Split('.')[2]));
                Adress.Append('.');
                Adress.Append(Convert.ToInt32(Subnets[2].Broadcast.ToString().Split('.')[3]) + 1);
            }

            Subnets[^1].IpAdress = Adress.ToString();
            Subnets[^1].Suffix = Subnets[1].Suffix + 1;
            Subnets[^1].CalcAndWriteAll();

            Subnets.Add(new Subnet());

            Adress.Clear();

            if (Convert.ToInt32(Subnets[3].Broadcast.ToString().Split('.')[1]) + 1 > 255)
            {
                Adress.Append(Convert.ToInt32(Subnets[3].Broadcast.ToString().Split('.')[0]) + 1);
                Adress.Append('.');
            }
            else
            {
                Adress.Append(Convert.ToInt32(Subnets[3].Broadcast.ToString().Split('.')[0]));
                Adress.Append('.');
            }

            if (Convert.ToInt32(Subnets[3].Broadcast.ToString().Split('.')[2]) + 1 > 255)
            {
                Adress.Append(Convert.ToInt32(Subnets[3].Broadcast.ToString().Split('.')[1]) + 1);
                Adress.Append('.');
            }
            else
            {
                Adress.Append(Convert.ToInt32(Subnets[3].Broadcast.ToString().Split('.')[1]));
                Adress.Append('.');
            }

            if (Convert.ToInt32(Subnets[3].Broadcast.ToString().Split('.')[3]) + 1 > 255)
            {
                Adress.Append(Convert.ToInt32(Subnets[3].Broadcast.ToString().Split('.')[2]) + 1);
                Adress.Append('.');
                Adress.Append('0');
            }
            else
            {
                Adress.Append(Convert.ToInt32(Subnets[3].Broadcast.ToString().Split('.')[2]));
                Adress.Append('.');
                Adress.Append(Convert.ToInt32(Subnets[3].Broadcast.ToString().Split('.')[3]) + 1);
            }

            Subnets[^1].IpAdress = Adress.ToString();
            Subnets[^1].Suffix = Subnets[1].Suffix + 1;
            Subnets[^1].CalcAndWriteAll();
            this.SubnetCount = Subnets.Count();

            return Page();
        }
    }
}