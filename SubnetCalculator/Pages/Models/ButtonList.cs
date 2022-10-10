namespace SubnetCalculator.Pages.Models
{
    public class ButtonList
    {
        public List<String> Buttons { get; set; }

        public ButtonList()
        {
            Buttons = new();
            Buttons.Add("ButtonOne");
            Buttons.Add("ButtonTwo");
            Buttons.Add("ButtonThree");
            Buttons.Add("ButtonFour");
            Buttons.Add("ButtonFive");
            Buttons.Add("ButtonSix");
            Buttons.Add("ButtonSeven");

        }
    }
}
