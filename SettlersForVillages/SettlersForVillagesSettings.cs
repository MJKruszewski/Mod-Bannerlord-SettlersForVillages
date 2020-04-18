using System.Xml.Serialization;

namespace SettlersForVillages
{
    public class SettlersForVillagesSettings
    {
        [XmlElement] public int SettlersPriceMultiplier { get; set; } = 1;

        [XmlElement] public int SettlersAmountMultiplier { get; set; } = 1;

        [XmlElement] public int MilitiaPriceMultiplier { get; set; } = 1;

        [XmlElement] public bool DebugMode { get; set; } = true;
    }
}