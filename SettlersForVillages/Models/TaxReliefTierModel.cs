namespace SettlersForVillages.Models
{
    public class TaxReliefTierModel
    {
        public int TierEnum { get; }
        public string Label { get; }

        public TaxReliefTierModel(int tierEnum, string label)
        {
            TierEnum = tierEnum;
            Label = label;
        }
    }
}