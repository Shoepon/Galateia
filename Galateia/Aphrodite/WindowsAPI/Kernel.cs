using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Aphrodite.WindowsAPI
{
    public static class Kernel
    {
        [DllImport("kernel32.dll", EntryPoint = "GetTickCount", SetLastError = true)]
        public static extern uint GetTickCount();
    }
}
