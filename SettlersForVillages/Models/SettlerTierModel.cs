namespace SettlersForVillages.Models
{
    public class SettlerTierModel
    {
        private int _goldPrice;

        public int GoldPrice
        {
            get => _goldPrice * Main.settings.SettlersPriceMultiplier;
            set => _goldPrice = value;
        }

        private float _villagersToAdd;

        public float VillagersToAdd
        {
            get => _villagersToAdd * Main.settings.SettlersAmountMultiplier;
            set => _villagersToAdd = value;
        }

        public SettlerTierModel(int goldPrice, float villagersToAdd)
        {
            GoldPrice = goldPrice;
            VillagersToAdd = villagersToAdd;
        }

        public string GetOptionText()
        {
            return "Settle " + VillagersToAdd + " villagers for " + GoldPrice + " denars";
        }

        public string GetOptionId()
        {
            return "village_settlers_" + VillagersToAdd;
        }
    }
}