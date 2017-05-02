using System;

namespace Galateia.Infra.WindowsAPI
{
    public static class IDC
    {
        /// <summary>
        ///     Standard arrow and small hourglass
        /// </summary>
        public static readonly IntPtr APPSTARTING = new IntPtr(32650);

        /// <summary>
        ///     Standard arrow
        /// </summary>
        public static readonly IntPtr ARROW = new IntPtr(32512);

        /// <summary>
        ///     Crosshair
        /// </summary>
        public static readonly IntPtr CROSS = new IntPtr(32515);

        /// <summary>
        ///     Hand
        /// </summary>
        public static readonly IntPtr HAND = new IntPtr(32649);

        /// <summary>
        ///     Arrow and question mark
        /// </summary>
        public static readonly IntPtr HELP = new IntPtr(32651);

        /// <summary>
        ///     I-beam
        /// </summary>
        public static readonly IntPtr IBEAM = new IntPtr(32513);

        /// <summary>
        ///     Obsolete for applications marked version 4.0 or later.
        /// </summary>
        public static readonly IntPtr ICON = new IntPtr(32641);

        /// <summary>
        ///     Slashed circle
        /// </summary>
        public static readonly IntPtr NO = new IntPtr(32648);

        /// <summary>
        ///     Obsolete for applications marked version 4.0 or later. Use IDC_SIZEALL.
        /// </summary>
        public static readonly IntPtr SIZE = new IntPtr(32640);

        /// <summary>
        ///     Four-pointed arrow pointing north, south, east, and west
        /// </summary>
        public static readonly IntPtr SIZEALL = new IntPtr(32646);

        /// <summary>
        ///     Double-pointed arrow pointing northeast and southwest
        /// </summary>
        public static readonly IntPtr SIZENESW = new IntPtr(32643);

        /// <summary>
        ///     Double-pointed arrow pointing north and south
        /// </summary>
        public static readonly IntPtr SIZENS = new IntPtr(32645);

        /// <summary>
        ///     Double-pointed arrow pointing northwest and southeast
        /// </summary>
        public static readonly IntPtr SIZENWSE = new IntPtr(32642);

        /// <summary>
        ///     Double-pointed arrow pointing west and east
        /// </summary>
        public static readonly IntPtr SIZEWE = new IntPtr(32644);

        /// <summary>
        ///     Vertical arrow
        /// </summary>
        public static readonly IntPtr UPARROW = new IntPtr(32516);

        /// <summary>
        ///     Hourglass
        /// </summary>
        public static readonly IntPtr WAIT = new IntPtr(32514);
    }

    public static class HSHELL
    {
        public static readonly IntPtr WINDOWCREATED = new IntPtr(1);
        public static readonly IntPtr WINDOWDESTROYED = new IntPtr(2);
        public static readonly IntPtr ACTIVATESHELLWINDOW = new IntPtr(3);
        public static readonly IntPtr WINDOWACTIVATED = new IntPtr(4);
        public static readonly IntPtr GETMINRECT = new IntPtr(5);
        public static readonly IntPtr REDRAW = new IntPtr(6);
        public static readonly IntPtr TASKMAN = new IntPtr(7);
        public static readonly IntPtr LANGUAGE = new IntPtr(8);
        public static readonly IntPtr SYSMENU = new IntPtr(9);
        public static readonly IntPtr ENDTASK = new IntPtr(10);
        public static readonly IntPtr ACCESSIBILITYSTATE = new IntPtr(11);
        public static readonly IntPtr APPCOMMAND = new IntPtr(12);
        public static readonly IntPtr WINDOWREPLACED = new IntPtr(13);
        public static readonly IntPtr WINDOWREPLACING = new IntPtr(14);
        public static readonly IntPtr MONITORCHANGED = new IntPtr(16);
        public static readonly IntPtr HIGHBIT = new IntPtr(0x8000);
        public static readonly IntPtr FLASH = new IntPtr(0x8000 | 6); // (HSHELL_REDRAW|HSHELL_HIGHBIT)

        public static readonly IntPtr RUDEAPPACTIVATED = new IntPtr(0x8000 | 4);
            // (HSHELL_WINDOWACTIVATED|HSHELL_HIGHBIT)
    }
}