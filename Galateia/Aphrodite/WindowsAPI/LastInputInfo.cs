using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Aphrodite.WindowsAPI
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LastInputInfo
    {
        public int cbSize;
        public uint dwTime;
    }
}
