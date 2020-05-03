using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace SettlersForVillages
{
    public class VillageDetailsModel
    {
        [SaveableField(1)] public string SettlementId;

        [SaveableField(2)] public int TaxReliefTier;

        [SaveableField(3)] public int SettlementToday = 0;

        public VillageDetailsModel(string settlementId, int taxReliefTierEnum)
        {
            SettlementId = settlementId;
            TaxReliefTier = taxReliefTierEnum;
        }

        private Settlement GetSelf()
        {
            return Settlement.Find(SettlementId);
        }

        public float TaxReliefBaseValue()
        {
            var self = GetSelf();
         
            return Campaign.Current.Models.SettlementTaxModel.CalculateVillageTaxFromIncome(self.Village, self.Village.TradeTaxAccumulated) 
                   / Campaign.Current.Models.SettlementTaxModel.GetVillageTaxRatio();
        }

        public float TaxReliefAmount()
        {
            return TaxReliefBaseValue() * TierPercentage();
        }

        private float TierPercentage()
        {
            switch (TaxReliefTier)
            {
                case 1:
                    return 0.1f;
                case 2:
                    return 0.25f;
                case 3:
                    return 0.5f;
            }

            return 0f;
        }
    }
}