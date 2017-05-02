using System;
using System.Xml.Serialization;

namespace Galateia.Ghost
{
    public class GhostConfig
    {
        [XmlIgnore]
        public string BaseDirectory { get; set; }

        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Craftsman { get; set; }
        public string Contact { get; set; }
        public Guid Guid { get; set; }
    }
}