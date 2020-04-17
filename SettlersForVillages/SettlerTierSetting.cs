namespace SettlersForVillages
{
    public class SettlerTierSetting
    {
        public int GoldPrice { get; }
        public float VillagersToAdd { get; }

        public SettlerTierSetting(int goldPrice, float villagersToAdd)
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