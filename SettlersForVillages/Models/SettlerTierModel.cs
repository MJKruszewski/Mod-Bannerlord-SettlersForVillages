namespace SettlersForVillages.Models
{
    public class SettlerTierModel
    {
        private int _goldPrice;

        public int GoldPrice
        {
            get => (int) (_goldPrice * Main.Settings.SettlersPriceMultiplier);
            set => _goldPrice = value;
        }

        private float _villagersToAdd;

        public float VillagersToAdd
        {
            get => _villagersToAdd * Main.Settings.SettlersAmountMultiplier;
            set => _villagersToAdd = value;
        }

        public SettlerTierModel(int goldPrice, float villagersToAdd)
        {
            GoldPrice = goldPrice;
            VillagersToAdd = villagersToAdd;
        }

        public string GetOptionText()
        {
            return Main.Localization.GetTranslation(Localization.SettlersMenuTiers, VillagersToAdd, GoldPrice);
        }

        public string GetOptionId()
        {
            return "village_settlers_" + VillagersToAdd;
        }
    }
}