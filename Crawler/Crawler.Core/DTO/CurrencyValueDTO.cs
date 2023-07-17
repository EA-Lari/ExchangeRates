using System.Xml.Serialization;

namespace Crawler.Core.DTO
{
    [XmlRoot("ValCurs")]
    public class CurrencyRateDTO
    {
        [XmlElement("Valute")]
        public CurrencyValueDTO[] Values { get; set; }
    }

    public class CurrencyValueDTO
    {
        [XmlElement("CharCode")]
        public string CharCode { get; set; }

        [XmlElement("Value")]
        public string Value { get; set; }
    }
}
