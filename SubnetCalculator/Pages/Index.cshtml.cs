using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SubnetCalculator.Pages.Model;
using System.ComponentModel.DataAnnotations;

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

        [BindProperty]
        public int SubnetCount { get; set; }

        [BindProperty]
        public int Counter { get; set; }



        public void OnGet()
        {
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
    }
}