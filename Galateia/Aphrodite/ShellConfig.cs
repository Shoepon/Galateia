using System;
using System.Xml.Serialization;

namespace Galateia.Shell
{
    public class ShellConfig
    {
        [XmlIgnore]
        public string BaseDirectory { get; set; }

        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Craftsman { get; set; }
        public string Contact { get; set; }
        public Guid Guid { get; set; }

        public string Model { get; set; }
    }
}