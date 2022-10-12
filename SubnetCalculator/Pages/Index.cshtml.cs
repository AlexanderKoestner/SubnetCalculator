using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using SubnetCalculator.Pages.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
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

        // Properties to pass to page and retrive from Post via Modelbinding.
        // Also validates Input for IP Adress

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
        public int CounterY { get; set; }

        [BindProperty]
        public int CounterX { get; set; }

        [BindProperty]
        public int Index { get; set; }

        public string? IpAdress { get; set; }
        public string? EmptyStringDot { get; set; } = " . ";
        public string? EmptyStringDash { get; set; } = " / ";

        public void OnGet()
        {
        }

        // Creates the first Subnet based on the typed in IP Adress
        // Returns back to a clean Index Page if validation fails

        public IActionResult OnPost()
        {
            // Checks if Input is valid
            // If true: Proceed
            // If false: Return to new Index Page

            if (ModelState.IsValid)
            {   
                // Initializes Subnet List

                Subnets = new();

                // Create IP Adress String from Input

                IpAdress = IpAdressFirstByte.ToString() + "." + IpAdressSecondByte.ToString() + "." + IpAdressThirdByte.ToString() + "." + IpAdressFourthByte.ToString();

                // Adds first Subnet into Subnet List

                Subnets.Add(new Subnet());

                // Takes IP Adress from Input and passes it to new Subnet

                Subnets[^1].IpAdress = IpAdress;

                // Takes Suffix from Input and passes it to new Subnet

                Subnets[^1].Suffix = IpAdressSuffix;

                // Calculates and writes the new Subnet Propoerties for List item at Index
                // See Subnet.cs in Folder Models for more Information

                Subnets[^1].CalcAndWriteAll();

                // Serializes Json String from Subnet List

                JsonString = JsonConvert.SerializeObject(Subnets, Formatting.Indented);

                // Return to Page

                return Page();
            }
            else
            {
                // Return to new Page

                return RedirectToPage("Index");
            }
        }

        // Retrieves a Json String from Post containing the List Subnets from the Last Post and deserializes into a Subnet List and
        // retrieves the Index from Post of the Subnet that needs to be divided. Then divides the Subnet into two smaller Subnets and
        // removes the divided Subnet and serializes the changed Subnet List into a Json String

        public IActionResult OnPostButton_Divide()
        {
            // Creates List of Subnets by deserializing Json String

            Subnets = JsonConvert.DeserializeObject<List<Subnet>>(JsonString);

            // Inserts first new divided Subnet into List at Index

            Subnets.Insert(Index, new Subnet());

            // Takes IP Adress from List Item after Index and passes it to new Subnet

            Subnets[Index].IpAdress = Subnets[Index + 1].IpAdress;

            // Checks if Suffix is greater then 31
            // If true: Takes Suffix from List Item after Index passes it to new Subnet
            // If false: Takes Suffix from List Item after Index, then increments it by 1 and passes it to new Subnet

            if(Subnets[Index + 1].Suffix > 31)
            {
                Subnets[Index].Suffix = Subnets[Index + 1].Suffix;
            }
            else
            {
                Subnets[Index].Suffix = Subnets[Index + 1].Suffix + 1;
            }

            // Calculates and writes the new Subnet Propoerties for List item at Index
            // See Subnet.cs in Folder Models for more Information

            Subnets[Index].CalcAndWriteAll();

            // Inserts second new divided Subnet into List after Index

            Subnets.Insert(Index + 1, new Subnet());

            // Checks if Suffix is greater then 31
            // If true: Takes the Subnet ID from List Item at Index and passes it new Subnet as IP Adress
            // If false: Takes the Broadcast Adress from List at Index and passes it to new Subnet as IP Adress

            if (Subnets[Index].Suffix > 31)
            {
                Subnets[Index + 1].IpAdress = GetNewIpAdress(Subnets[Index].SubnetID.ToString());
            }
            else
            {
                Subnets[Index + 1].IpAdress = GetNewIpAdress(Subnets[Index].Broadcast.ToString());
            }

            // Takes Suffix from List Item at Index passes it to new Subnet

            Subnets[Index + 1].Suffix = Subnets[Index].Suffix;

            // Calculates and writes the new Subnet Propoerties for List item after Index
            // See Subnet.cs in Folder Models for more Information

            Subnets[Index + 1].CalcAndWriteAll();

            // Remove divided Subnet from List

            Subnets.RemoveAt(Index + 2);

            // Serializes Json String from Subnet List

            JsonString = JsonConvert.SerializeObject(Subnets, Formatting.Indented);

            // Returns to Page

            return Page();
        }

        // Retrieves a Json String from Post containing the List Subnets from the Last Post and deserializes into a Subnet List and
        // retrieves the Index from Post of the Subnet that needs to be joined. Then removes all Subnets that have been joined from the List
        // and serializes the List back in to a Json String

        public IActionResult OnPostButton_Join()
        {
            // Creates List of Subnets by deserializing Json String

            Subnets = JsonConvert.DeserializeObject<List<Subnet>>(JsonString);

            // Inserts new Subnet into List after Index

            Subnets.Insert(Index + 1, new Subnet());

            // Takes Subnet ID Property from the first Index and passes it to the new Subnet

            Subnets[Index + 1].IpAdress = Subnets[0].SubnetID.ToString();

            //if (Index == 1)
            //{
            //    Subnets[Index].IpAdress = Subnets[Index - 1].SubnetID.ToString();
            //}
            //else
            //{
            //    Subnets[Index].IpAdress = Subnets[Index - 1].Broadcast.ToString();
            //}

            // Checks if the Index equals the last Item in the List
            // If true: Takes IP Adress Suffix from first Input
            // If false: Takes IP Adress Suffix from second List item after Index

            if (Index == Subnets.Count - 2)
            {
                Subnets[Index + 1].Suffix = IpAdressSuffix;
            }
            else
            {
                Subnets[Index + 1].Suffix = Subnets[Index + 2].Suffix;
            }

            // Calculates and writes the new Subnet Propoerties for List item after Index
            // See Subnet.cs in Folder Models for more Information

            Subnets[Index].CalcAndWriteAll();

            int subnetCount = Subnets.Count;

            // Removes all joined Subnets from List

            for (int i = 0; i < subnetCount; i++)
            {
                if (i == Index + 1)
                {
                    break;
                }

                Subnets.RemoveAt(0);
            }

            // Serializes Json String from Subnet List

            JsonString = JsonConvert.SerializeObject(Subnets, Formatting.Indented);

            // Returns to Page

            return Page();
        }

        // Takes Broadcast Adress From previous Subnet Object and returns the next Subnet ID

        private string GetNewIpAdress(string broadCastFromIndex)
        {
            StringBuilder adress = new();

            // Checks if the second Byte block of the IP Adress equals 255
            // If true: Increments the first Byte block of the IP Adress by 1 and sets the second Byte block of the IP Adress to 0
            // if false: Increments the second Byte block of the IP Adress by 1

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

            // Checks if the third Byte block of the IP Adress equals 255
            // If true: Increments the second Byte block of the IP Adress by 1 and sets the tird Byte block of the IP Adress to 0
            // if false: Increments the third Byte block of the IP Adress by 1

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

            // Checks if the fourth Byte block of the IP Adress equals 255
            // If true: Increments the third Byte block of the IP Adress by 1 and sets the fourth Byte block of the IP Adress to 0
            // if false: Increments the second Byte block of the IP Adress by 1

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

            // Returns the passed IP Adress incremented by 1

            return adress.ToString();
        }
    }
}