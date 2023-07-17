using System.Collections.Generic;
using System.Xml.Serialization;

namespace Crawler.Core.DTO
{
    [XmlRoot("Valuta")]
    public class CurrencyInfoDTO
    {
        [XmlElement("Item")]
        public CurrencyItemInfoDTO[] Items { get; set; }
    }

    public class CurrencyItemInfoDTO
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("EngName")]
        public string EngName { get; set; }

        [XmlElement("Nominal")]
        public int Nominal { get; set; }

        [XmlElement("ISO_Char_Code")]
        public string IsoCharCode { get; set; }

        [XmlElement("ParentCode")]
        public string ParentCode { get; set; }
    }
}
