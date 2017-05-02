using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace Galateia.Shell
{
    public class ScreenChangedEventArgs : EventArgs
    {
        private readonly ReadOnlyCollection<RectangleF> areas;

        public ScreenChangedEventArgs(IEnumerable<RectangleF> areas)
        {
            this.areas = new ReadOnlyCollection<RectangleF>(areas.ToArray());
        }

        public ReadOnlyCollection<RectangleF> Areas
        {
            get { return areas; }
        }
    }
}