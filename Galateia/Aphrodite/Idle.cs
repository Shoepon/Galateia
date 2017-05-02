using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Aphrodite.WindowsAPI;

namespace Aphrodite
{
    public static class Idle
    {
        public static TimeSpan Duration 
        {
            get
            {
                var lastInputInfo = new LastInputInfo
                {cbSize = Marshal.SizeOf(typeof (LastInputInfo))};
                User.GetLastInputInfo(out lastInputInfo);
                return TimeSpan.FromMilliseconds(Kernel.GetTickCount() - lastInputInfo.dwTime);
            }
        }
    }
}
