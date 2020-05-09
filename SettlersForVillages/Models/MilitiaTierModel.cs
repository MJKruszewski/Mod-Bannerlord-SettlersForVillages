namespace SettlersForVillages.Models
{
    public class MilitiaTierModel
    {
        private int _goldPrice;

        public int GoldPrice
        {
            get => (int) (_goldPrice * Main.Settings.MilitiaPriceMultiplier);
            set => _goldPrice = value;
        }

        public float MilitiaToAdd { get; }

        public MilitiaTierModel(int goldPrice, float militiaToAdd)
        {
            GoldPrice = goldPrice;
            MilitiaToAdd = militiaToAdd;
        }

        public string GetOptionText()
        {
            return Main.Localization.GetTranslation(Localization.MilitiaMenuTiers, MilitiaToAdd, GoldPrice);
        }

        public string GetOptionId()
        {
            return "village_militia_recruit_" + MilitiaToAdd;
        }
    }
}