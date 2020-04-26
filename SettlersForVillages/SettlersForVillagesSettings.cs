using System.Xml.Serialization;
using ModLib;
using ModLib.Attributes;

namespace SettlersForVillages
{
    public class SettlersForVillagesSettings: SettingsBase
    {
        public const string InstanceId = "SettlersForVillagesSettings";

        public override string ModName => "SettlersForVillages";
        public override string ModuleFolderName => "SettlersForVillages";
        [XmlElement]
        public override string ID { get; set; } = InstanceId;
        
        [XmlElement] 
        [SettingProperty("Settlers price multiplier",1f,20f)]
        public float SettlersPriceMultiplier { get; set; } = 1f;

        [XmlElement] 
        [SettingProperty("Settlers amount multiplier",0.1f,10f)]
        public float SettlersAmountMultiplier { get; set; } = 1f;

        [XmlElement] 
        [SettingProperty("Militia price multiplier",1f,20f)]
        public float MilitiaPriceMultiplier { get; set; } = 1f;
        
        [XmlElement] 
        [SettingProperty("Ai enabled","Tells if mod enabled for AI")]
        public bool AiEnabled { get; set; } = true;
        
        [XmlElement] 
        [SettingProperty("Prosperity affection","Tells if settling new villagers affects prosperity of nearby town")]
        public bool ProsperityAffection { get; set; } = true;

        [XmlElement] 
        [SettingProperty("Debug mode")]
        public bool DebugMode { get; set; } = false;
    }
}