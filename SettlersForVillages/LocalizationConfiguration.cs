using System.Xml.Serialization;

namespace SettlersForVillages
{
    public class LocalizationConfiguration
    {
        [XmlElement] public string LanguageCode = "EN";
    }
}