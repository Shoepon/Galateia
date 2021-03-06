﻿using System;
using System.Xml.Serialization;

namespace Galateia.Baloon
{
    public class BaloonConfig
    {
        [XmlIgnore]
        public string BaseDirectory { get; set; }

        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Craftsman { get; set; }
        public string Contact { get; set; }

        public Guid Guid { get; set; }

        public BaloonDesign LeftBaloon { get; set; }

        public BaloonDesign RightBaloon { get; set; }
    }
}