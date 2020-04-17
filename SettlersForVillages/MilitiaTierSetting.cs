namespace SettlersForVillages
{
    public class MilitiaTierSetting
    {
            public int GoldPrice { get; }
            public float MilitiaToAdd { get; }

            public MilitiaTierSetting(int goldPrice, float militiaToAdd)
            {
                GoldPrice = goldPrice;
                MilitiaToAdd = militiaToAdd;
            }

            public string GetOptionText()
            {
                return "Recruit " + MilitiaToAdd + " militia from peasants for " + GoldPrice + " denars";
            }

            public string GetOptionId()
            {
                return "village_militia_recruit_" + MilitiaToAdd;
            }
    }
}