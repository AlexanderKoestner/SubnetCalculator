using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
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

        [BindProperty]
        public List<Subnet>? Subnets { get; set; }

        [BindProperty]
        public string? JsonString { get; set; }

        [BindProperty]
        public int Counter { get; set; }

        [BindProperty]
        public int Index { get; set; }

        public string? IpAdress { get; set; }
        public string? EmptyStringDot { get; set; } = " . ";
        public string? EmptyStringDash { get; set; } = " / ";

        public void OnGet()
        {
        }

        // Creates the first Subnet based on the typed in IP Adress

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

                JsonString = JsonConvert.SerializeObject(Subnets, Formatting.Indented);
                return Page();
            }
            else
            {
                return RedirectToPage("Index");
            }
        }

        // Retrieves a Json String from Post containing the List Subnets from the Last Post and deserializes into a Subnet List and
        // retrieves the Index from Post of the Subnet that needs to be divided. Then divides the Subnet into two smaller Subnets and
        // removes the divided Subnet and serializes the changed Subnet List into a Json String

        public IActionResult OnPostButton_Divide()
        {
            Subnets = JsonConvert.DeserializeObject<List<Subnet>>(JsonString);
            Subnets.Insert(Index, new Subnet());
            Subnets[Index].IpAdress = Subnets[Index + 1].IpAdress;

            if(Subnets[Index + 1].Suffix > 31)
            {
                Subnets[Index].Suffix = Subnets[Index + 1].Suffix;
            }
            else
            {
                Subnets[Index].Suffix = Subnets[Index + 1].Suffix + 1;
            }

            Subnets[Index].CalcAndWriteAll();

            Subnets.Insert(Index + 1, new Subnet());

            if (Subnets[Index].Suffix > 31)
            {
                Subnets[Index + 1].IpAdress = GetNewIpAdress(Subnets[Index].SubnetID.ToString());
            }
            else
            {
                Subnets[Index + 1].IpAdress = GetNewIpAdress(Subnets[Index].Broadcast.ToString());
            }

            Subnets[Index + 1].Suffix = Subnets[Index].Suffix;
            Subnets[Index + 1].CalcAndWriteAll();

            Subnets.RemoveAt(Index + 2);

            JsonString = JsonConvert.SerializeObject(Subnets, Formatting.Indented);

            return Page();
        }

        // Retrieves a Json String from Post containing the List Subnets from the Last Post and deserializes into a Subnet List and
        // retrieves the Index from Post of the Subnet that needs to be joined.

        public IActionResult OnPostButton_Join()
        {
            Subnets = JsonConvert.DeserializeObject<List<Subnet>>(JsonString);
            Subnets.Insert(Index + 1, new Subnet());
            Subnets[Index + 1].IpAdress = Subnets[0].SubnetID.ToString();
            if(Index == Subnets.Count - 2)
            {
                Subnets[Index + 1].Suffix = IpAdressSuffix;
            }
            else
            {
                Subnets[Index + 1].Suffix = Subnets[Index + 2].Suffix;
            }
            Subnets[Index + 1].CalcAndWriteAll();

            int subnetCount = Subnets.Count;

            for (int i = 0; i < subnetCount; i++)
            {
                if(i == Index + 1)
                {
                    break;
                }

                Subnets.RemoveAt(0);
            }

            JsonString = JsonConvert.SerializeObject(Subnets, Formatting.Indented);

            return Page();
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