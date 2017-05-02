using System;

namespace Galateia.Infra.WindowsAPI
{
    public enum GlobalMemoryFlags : uint
    {
        Fixed = 0x0000,
        Movable = 0x0002,
        ZeroInit = 0x0040
    }

    public enum ShowWindow
    {
        Hide = 0,
        ShowNoActivate = 4,
        Show = 5
    }

    public enum ClassStyles : uint
    {
        ByteAlignClient = 0x1000,
        ByteAlignWindow = 0x2000,
        ClassDC = 0x0040,
        DblClks = 0x0008,
        DropShadow = 0x00020000,
        GlobalClass = 0x4000,
        HRedraw = 0x0002,
        NoClose = 0x0200,
        OwnDC = 0x0020,
        ParentDC = 0x0080,
        SaveBits = 0x0800,
        VRedraw = 0x0001
    }

    [Flags]
    public enum SetWindowPosFlag : uint
    {
        AsyncWindowPos = 0x4000,
        HideWindow = 0x0080,
        NoActivate = 0x0010,
        NoMove = 0x0002,
        NoOwnerZOrder = 0x0200,
        NoSendChanging = 0x0400,
        NoSize = 0x0001,
        NoZOrder = 0x0004,
        ShowWindow = 0x0040
    }

    public enum UpdateLayeredWindowFlags : uint
    {
        ColorKey = 0x00000001,
        Alpha = 0x00000002,
        Opaque = 0x00000004,
        ExNoResize = 0x00000008
    }

    public enum WindowStyles : uint
    {
        Border = 0x00800000,
        Caption = 0x00C00000,
        Child = 0x40000000,
        /* ChildWindow = Child */
        ClipChildren = 0x02000000,
        ClipSiblings = 0x04000000,
        Disabled = 0x08000000,
        DlgFrame = 0x00400000,
        Group = 0x00020000,
        HScroll = 0x00100000,
        /* Iconic = Minimize */
        MaximizeBox = 0x00010000,
        Minimize = 0x20000000,
        MinimizeBox = 0x00020000,
        Overlapped = 0x00000000,
        Popup = 0x80000000,
        SizeBox = 0x00040000,
        SysMenu = 0x00080000,
        TabStop = 0x00010000,
        ThickFrame = 0x00040000,
        /* Tiled = Overlapped */
        Visible = 0x10000000,
        VScroll = 0x00200000
    }

    public enum WindowsHook
    {
        MsgFilter = -1,
        JournalRecord = 0,
        JournalPlayback = 1,
        Keyboard = 2,
        GetMessage = 3,
        CallWndProc = 4,
        CBT = 5,
        SysMsgFilter = 6,
        Mouse = 7,
        Hardware = 8,
        Debug = 9,
        Shell = 10,
        ForegroundIdle = 11,
        CallWndProcRet = 12,
        KeyboardLL = 13,
        MouseLL = 14
    }

    public enum LowLevelKeyboardHookFlags : uint
    {
        /// <summary>
        ///     LLKHF_EXTENDED: Specifies whether the key is an extended key, such as a function key or a key on the numeric
        ///     keypad. The value is 1 if the key is an extended key; otherwise, it is 0.
        /// </summary>
        Extended = 0x00000001,

        /// <summary>
        ///     LLKHF_INJECTED: Specifies whether the event was injected. The value is 1 if the event was injected; otherwise, it
        ///     is 0.
        /// </summary>
        Injected = 0x00000010,

        /// <summary>
        ///     LLKHF_INJECTED: The context code. The value is 1 if the ALT key is pressed; otherwise, it is 0.
        /// </summary>
        AltDown = 0x00000020,

        /// <summary>
        ///     LLKHF_UP: The transition state. The value is 0 if the key is pressed and 1 if it is being released.
        /// </summary>
        Up = 0x00000080
    }

    [Flags]
    public enum WindowStylesEx : uint
    {
        None = 0,

        /// <summary>
        ///     The window accepts drag-drop files.
        /// </summary>
        AcceptFiles = 0x00000010,

        /// <summary>
        ///     Forces a top-level window onto the taskbar when the window is visible.
        /// </summary>
        AppWindow = 0x00040000,

        /// <summary>
        ///     The window has a border with a sunken edge.
        /// </summary>
        ClientEdge = 0x00000200,

        /// <summary>
        ///     Paints all descendants of a window in bottom-to-top painting order using double-buffering.
        /// </summary>
        Composited = 0x02000000,

        /// <summary>
        ///     The title bar of the window includes a question mark.
        /// </summary>
        ContextHelp = 0x00000400,

        /// <summary>
        ///     The window itself contains child windows that should take part in dialog box navigation.
        /// </summary>
        ControlParent = 0x00010000,

        /// <summary>
        ///     The window has a double border; the window can, optionally, be created with a title bar by specifying the
        ///     WS_CAPTION style in the dwStyle parameter.
        /// </summary>
        DlgModalFrame = 0x00000001,

        /// <summary>
        ///     The window is a layered window.
        /// </summary>
        Layered = 0x00080000,

        /// <summary>
        ///     If the shell language is Hebrew, Arabic, or another language that supports reading order alignment, the horizontal
        ///     origin of the window is on the right edge.Increasing horizontal values advance to the left.
        /// </summary>
        LayoutRTL = 0x00400000,
        // Left = 0x00000000,
        /// <summary>
        ///     If the shell language is Hebrew, Arabic, or another language that supports reading order alignment, the vertical
        ///     scroll bar (if present) is to the left of the client area. For other languages, the style is ignored.
        /// </summary>
        LeftScrollBar = 0x00004000,
        // LTRReading = 0x00000000,
        /// <summary>
        ///     The window is a MDI child window.
        /// </summary>
        MDIChild = 0x00000040,

        /// <summary>
        ///     A top-level window created with this style does not become the foreground window when the user clicks it. The
        ///     system does not bring this window to the foreground when the user minimizes or closes the foreground window.
        /// </summary>
        NoActivate = 0x08000000,

        /// <summary>
        ///     The window does not pass its window layout to its child windows.
        /// </summary>
        NoInheritLayout = 0x00100000,

        /// <summary>
        ///     The child window created with this style does not send the WM_PARENTNOTIFY message to its parent window when it is
        ///     created or destroyed.
        /// </summary>
        NoParentNotify = 0x00000004,

        /// <summary>
        ///     The window does not render to a redirection surface. This is for windows that do not have visible content or that
        ///     use mechanisms other than surfaces to provide their visual.
        /// </summary>
        NoRedirectionBitmap = 0x00200000,

        /// <summary>
        ///     The window has generic "right-aligned" properties. This depends on the window class. This style has an effect only
        ///     if the shell language is Hebrew, Arabic, or another language that supports reading-order alignment; otherwise, the
        ///     style is ignored.
        /// </summary>
        Right = 0x00001000,
        // RightScrollBar = 0x00000000,
        /// <summary>
        ///     If the shell language is Hebrew, Arabic, or another language that supports reading-order alignment, the window text
        ///     is displayed using right-to-left reading-order properties. For other languages, the style is ignored.
        /// </summary>
        RTLReading = 0x00002000,

        /// <summary>
        ///     The window has a three-dimensional border style intended to be used for items that do not accept user input.
        /// </summary>
        StaticEdge = 0x00020000,

        /// <summary>
        ///     The window is intended to be used as a floating toolbar. A tool window has a title bar that is shorter than a
        ///     normal title bar, and the window title is drawn using a smaller font. A tool window does not appear in the taskbar
        ///     or in the dialog that appears when the user presses ALT+TAB. If a tool window has a system menu, its icon is not
        ///     displayed on the title bar. However, you can display the system menu by right-clicking or by typing ALT+SPACE.
        /// </summary>
        ToolWindow = 0x00000080,

        /// <summary>
        ///     The window should be placed above all non-topmost windows and should stay above them, even when the window is
        ///     deactivated. To add or remove this style, use the SetWindowPos function.
        /// </summary>
        TopMost = 0x00000008,

        /// <summary>
        ///     The window should not be painted until siblings beneath the window (that were created by the same thread) have been
        ///     painted. The window appears transparent because the bits of underlying sibling windows have already been painted.
        /// </summary>
        Transparent = 0x00000020,

        /// <summary>
        ///     The window has a border with a raised edge.
        /// </summary>
        WindowEdge = 0x00000100
    }

    public enum WindowMessages : uint
    {
        /// <summary>
        ///     WM_NULL
        /// </summary>
        Null = 0x0000,

        /// <summary>
        ///     WM_CREATE
        /// </summary>
        Create = 0x0001,

        /// <summary>
        ///     WM_DESTROY
        /// </summary>
        Destroy = 0x0002,

        /// <summary>
        ///     WM_MOVE
        /// </summary>
        Move = 0x0003,

        /// <summary>
        ///     WM_SIZE
        /// </summary>
        Size = 0x0005,

        /// <summary>
        ///     WM_ACTIVATE
        /// </summary>
        Activate = 0x0006,

        /// <summary>
        ///     WM_SETFOCUS
        /// </summary>
        SetFocus = 0x0007,

        /// <summary>
        ///     WM_KILLFOCUS
        /// </summary>
        KillFocus = 0x0008,

        /// <summary>
        ///     WM_ENABLE
        /// </summary>
        Enable = 0x000A,

        /// <summary>
        ///     WM_SETREDRAW
        /// </summary>
        SetRedraw = 0x000B,

        /// <summary>
        ///     WM_SETTEXT
        /// </summary>
        SetText = 0x000C,

        /// <summary>
        ///     WM_GETTEXT
        /// </summary>
        GetText = 0x000D,

        /// <summary>
        ///     WM_GETTEXTLENGTH
        /// </summary>
        GetTextLength = 0x000E,

        /// <summary>
        ///     WM_PAINT
        /// </summary>
        Paint = 0x000F,

        /// <summary>
        ///     WM_CLOSE
        /// </summary>
        Close = 0x0010,

        /// <summary>
        ///     WM_QUERYENDSESSION
        /// </summary>
        QueryEndSession = 0x0011,

        /// <summary>
        ///     WM_QUERYOPEN
        /// </summary>
        QueryOpen = 0x0013,

        /// <summary>
        ///     WM_ENDSESSION
        /// </summary>
        EndSession = 0x0016,

        /// <summary>
        ///     WM_QUIT
        /// </summary>
        Quit = 0x0012,

        /// <summary>
        ///     WM_ERASEBKGND
        /// </summary>
        EraseBkgnd = 0x0014,

        /// <summary>
        ///     WM_SYSCOLORCHANGE
        /// </summary>
        SysColorChange = 0x0015,

        /// <summary>
        ///     WM_SHOWWINDOW
        /// </summary>
        ShowWindow = 0x0018,

        /// <summary>
        ///     WM_SETTINGCHANGE
        /// </summary>
        SettingChange = 0x001A,

        /// <summary>
        ///     WM_DEVMODECHANGE
        /// </summary>
        DevModeChange = 0x001B,

        /// <summary>
        ///     WM_ACTIVATEAPP
        /// </summary>
        ActivateApp = 0x001C,

        /// <summary>
        ///     WM_FONTCHANGE
        /// </summary>
        FontChange = 0x001D,

        /// <summary>
        ///     WM_TIMECHANGE
        /// </summary>
        TimeChange = 0x001E,

        /// <summary>
        ///     WM_CANCELMODE
        /// </summary>
        CancelMode = 0x001F,

        /// <summary>
        ///     WM_SETCURSOR
        /// </summary>
        SetCursor = 0x0020,

        /// <summary>
        ///     WM_MOUSEACTIVATE
        /// </summary>
        MouseActivate = 0x0021,

        /// <summary>
        ///     WM_CHILDACTIVATE
        /// </summary>
        ChildActivate = 0x0022,

        /// <summary>
        ///     WM_QUEUESYNC
        /// </summary>
        QueueSync = 0x0023,

        /// <summary>
        ///     WM_GETMINMAXINFO
        /// </summary>
        GetMinMaxInfo = 0x0024,

        /// <summary>
        ///     WM_PAINTICON
        /// </summary>
        PaintIcon = 0x0026,

        /// <summary>
        ///     WM_ICONERASEBKGND
        /// </summary>
        IconEraseBkgnd = 0x0027,

        /// <summary>
        ///     WM_NEXTDLGCTL
        /// </summary>
        NextDlgCtl = 0x0028,

        /// <summary>
        ///     WM_SPOOLERSTATUS
        /// </summary>
        SpoolerStatus = 0x002A,

        /// <summary>
        ///     WM_DRAWITEM
        /// </summary>
        DrawItem = 0x002B,

        /// <summary>
        ///     WM_MEASUREITEM
        /// </summary>
        MeasureItem = 0x002C,

        /// <summary>
        ///     WM_DELETEITEM
        /// </summary>
        DeleteItem = 0x002D,

        /// <summary>
        ///     WM_VKEYTOITEM
        /// </summary>
        VKeyToItem = 0x002E,

        /// <summary>
        ///     WM_CHARTOITEM
        /// </summary>
        CharToItem = 0x002F,

        /// <summary>
        ///     WM_SETFONT
        /// </summary>
        SetFont = 0x0030,

        /// <summary>
        ///     WM_GETFONT
        /// </summary>
        GetFont = 0x0031,

        /// <summary>
        ///     WM_SETHOTKEY
        /// </summary>
        SetHotkey = 0x0032,

        /// <summary>
        ///     WM_GETHOTKEY
        /// </summary>
        GetHotkey = 0x0033,

        /// <summary>
        ///     WM_QUERYDRAGICON
        /// </summary>
        QueryDragIcon = 0x0037,

        /// <summary>
        ///     WM_COMPAREITEM
        /// </summary>
        CompareItem = 0x0039,

        /// <summary>
        ///     WM_GETOBJECT
        /// </summary>
        GetObject = 0x003D,

        /// <summary>
        ///     WM_COMPACTING
        /// </summary>
        Compacting = 0x0041,

        /// <summary>
        ///     WM_WINDOWPOSCHANGING
        /// </summary>
        WindowPosChanging = 0x0046,

        /// <summary>
        ///     WM_WINDOWPOSCHANGED
        /// </summary>
        WindowPosChanged = 0x0047,

        /// <summary>
        ///     WM_POWER
        /// </summary>
        Power = 0x0048,

        /// <summary>
        ///     WM_COPYDATA
        /// </summary>
        CopyData = 0x004A,

        /// <summary>
        ///     WM_CANCELJOURNAL
        /// </summary>
        CancelJournal = 0x004B,

        /// <summary>
        ///     WM_NOTIFY
        /// </summary>
        Notify = 0x004E,

        /// <summary>
        ///     WM_INPUTLANGCHANGEREQUEST
        /// </summary>
        InputLangChangeRequest = 0x0050,

        /// <summary>
        ///     WM_INPUTLANGCHANGE
        /// </summary>
        InputLangChange = 0x0051,

        /// <summary>
        ///     WM_TCARD
        /// </summary>
        TCard = 0x0052,

        /// <summary>
        ///     WM_HELP
        /// </summary>
        Help = 0x0053,

        /// <summary>
        ///     WM_USERCHANGED
        /// </summary>
        UserChanged = 0x0054,

        /// <summary>
        ///     WM_NOTIFYFORMAT
        /// </summary>
        NotifyFormat = 0x0055,

        /// <summary>
        ///     WM_CONTEXTMENU
        /// </summary>
        ContextMenu = 0x007B,

        /// <summary>
        ///     WM_STYLECHANGING
        /// </summary>
        StyleChanging = 0x007C,

        /// <summary>
        ///     WM_STYLECHANGED
        /// </summary>
        StyleChanged = 0x007D,

        /// <summary>
        ///     WM_DISPLAYCHANGE
        /// </summary>
        DisplayChange = 0x007E,

        /// <summary>
        ///     WM_GETICON
        /// </summary>
        GetIcon = 0x007F,

        /// <summary>
        ///     WM_SETICON
        /// </summary>
        SetIcon = 0x0080,

        /// <summary>
        ///     WM_NCCREATE
        /// </summary>
        NCCreate = 0x0081,

        /// <summary>
        ///     WM_NCDESTROY
        /// </summary>
        NCDestroy = 0x0082,

        /// <summary>
        ///     WM_NCCALCSIZE
        /// </summary>
        NCCalcSize = 0x0083,

        /// <summary>
        ///     WM_NCHITTEST
        /// </summary>
        NCHitTest = 0x0084,

        /// <summary>
        ///     WM_NCPAINT
        /// </summary>
        NCPaint = 0x0085,

        /// <summary>
        ///     WM_NCACTIVATE
        /// </summary>
        NCActivate = 0x0086,

        /// <summary>
        ///     WM_GETDLGCODE
        /// </summary>
        GetDlgCode = 0x0087,

        /// <summary>
        ///     WM_SYNCPAINT
        /// </summary>
        SyncPaint = 0x0088,

        /// <summary>
        ///     WM_NCMOUSEMOVE
        /// </summary>
        NCMouseMove = 0x00A0,

        /// <summary>
        ///     WM_NCLBUTTONDOWN
        /// </summary>
        NCLButtonDown = 0x00A1,

        /// <summary>
        ///     WM_NCLBUTTONUP
        /// </summary>
        NCLButtonUp = 0x00A2,

        /// <summary>
        ///     WM_NCLBUTTONDBLCLK
        /// </summary>
        NCLButtonDblClk = 0x00A3,

        /// <summary>
        ///     WM_NCRBUTTONDOWN
        /// </summary>
        NCRButtonDown = 0x00A4,

        /// <summary>
        ///     WM_NCRBUTTONUP
        /// </summary>
        NCRButtonUp = 0x00A5,

        /// <summary>
        ///     WM_NCRBUTTONDBLCLK
        /// </summary>
        NCRButtonDblClk = 0x00A6,

        /// <summary>
        ///     WM_NCMBUTTONDOWN
        /// </summary>
        NCMButtonDown = 0x00A7,

        /// <summary>
        ///     WM_NCMBUTTONPUP
        /// </summary>
        NCMButtonUp = 0x00A8,

        /// <summary>
        ///     WM_NCMBUTTONDBLCLK
        /// </summary>
        NCMButtonDblClk = 0x00A9,

        /// <summary>
        ///     WM_NCXBUTTONDOWN
        /// </summary>
        NCXButtonDown = 0x00AB,

        /// <summary>
        ///     WM_NCXBUTTONUP
        /// </summary>
        NCXButtonUp = 0x00AC,

        /// <summary>
        ///     WM_NCXBUTTONDBLCLK
        /// </summary>
        NCXButtonDblClk = 0x00AD,

        /// <summary>
        ///     WM_INPUTDEVICECHANGE
        /// </summary>
        InputDeviceChange = 0x00FE,

        /// <summary>
        ///     WM_INPUT
        /// </summary>
        Input = 0x00FF,

        /// <summary>
        ///     WM_KEYFIRST
        /// </summary>
        KeyFirst = 0x0100,

        /// <summary>
        ///     WM_KEYDOWN
        /// </summary>
        KeyDown = 0x0100,

        /// <summary>
        ///     WM_KEYUP
        /// </summary>
        KeyUp = 0x0101,

        /// <summary>
        ///     WM_CHAR
        /// </summary>
        Char = 0x0102,

        /// <summary>
        ///     WM_DEADCHAR
        /// </summary>
        DeadChar = 0x0103,

        /// <summary>
        ///     WM_SYSKEYDOWN
        /// </summary>
        SysKeyDown = 0x0104,

        /// <summary>
        ///     WM_SYSKEYUP
        /// </summary>
        SysKeyUp = 0x0105,

        /// <summary>
        ///     WM_SYSCHAR
        /// </summary>
        SysChar = 0x0106,

        /// <summary>
        ///     WM_SYSDEADCHAR
        /// </summary>
        SysDeadChar = 0x0107,

        /// <summary>
        ///     WM_UNICHAR
        /// </summary>
        UniChar = 0x0109,

        /// <summary>
        ///     WM_KEYLAST
        /// </summary>
        KeyLast = 0x0109,

        /// <summary>
        ///     WM_IME_STARTCOMPOSITION
        /// </summary>
        IMEStartComposition = 0x010D,

        /// <summary>
        ///     WM_IME_ENDCOMPOSITION
        /// </summary>
        IMEEndComposition = 0x010E,

        /// <summary>
        ///     WM_IME_COMPOSITION
        /// </summary>
        IMEComposition = 0x010F,

        /// <summary>
        ///     WM_IME_KEYLAST
        /// </summary>
        IMEKeyLast = 0x010F,

        /// <summary>
        ///     WM_INITDIALOG
        /// </summary>
        InitDialog = 0x0110,

        /// <summary>
        ///     WM_COMMAND
        /// </summary>
        Command = 0x0111,

        /// <summary>
        ///     WM_SYSCOMMAND
        /// </summary>
        SysCommand = 0x0112,

        /// <summary>
        ///     WM_TIMER
        /// </summary>
        Timer = 0x0113,

        /// <summary>
        ///     WM_HSCROLL
        /// </summary>
        HScroll = 0x0114,

        /// <summary>
        ///     WM_VSCROLL
        /// </summary>
        VScroll = 0x0115,

        /// <summary>
        ///     WM_INITMENU
        /// </summary>
        InitMenu = 0x0116,

        /// <summary>
        ///     WM_INITMENUPOPUP
        /// </summary>
        InitMenuPopup = 0x0117,

        /// <summary>
        ///     WM_GESTURE
        /// </summary>
        Gesture = 0x0119,

        /// <summary>
        ///     WM_GESTURENOTIFY
        /// </summary>
        GestureNotify = 0x011A,

        /// <summary>
        ///     WM_MENUSELECT
        /// </summary>
        MenuSelect = 0x011F,

        /// <summary>
        ///     WM_MENUCHAR
        /// </summary>
        MenuChar = 0x0120,

        /// <summary>
        ///     WM_ENTERIDLE
        /// </summary>
        EnterIdle = 0x0121,

        /// <summary>
        ///     WM_MENURBUTTONUP
        /// </summary>
        MenuRButtonUp = 0x0122,

        /// <summary>
        ///     WM_MENUDRAG
        /// </summary>
        MenuDrag = 0x0123,

        /// <summary>
        ///     WM_MENUGETOBJECT
        /// </summary>
        MenuGetObject = 0x0124,

        /// <summary>
        ///     WM_UNINITMENUPOPUP
        /// </summary>
        UninitMenuPopup = 0x0125,

        /// <summary>
        ///     WM_MENUCOMMAND
        /// </summary>
        MenuCommand = 0x0126,

        /// <summary>
        ///     WM_CHANGEUISTATE
        /// </summary>
        ChangeUIState = 0x0127,

        /// <summary>
        ///     WM_UPDATEUISTATE
        /// </summary>
        UpdateUIState = 0x0128,

        /// <summary>
        ///     WM_QUERYUISTATE
        /// </summary>
        QueryUIState = 0x0129,

        /// <summary>
        ///     WM_CTLCOLORMSGBOX
        /// </summary>
        CtlColorMsgBox = 0x0132,

        /// <summary>
        ///     WM_CTLCOLOREDIT
        /// </summary>
        CtlColorEdit = 0x0133,

        /// <summary>
        ///     WM_CTLCOLORLISTBOX
        /// </summary>
        CtlColorListBox = 0x0134,

        /// <summary>
        ///     WM_CTLCOLORBTN
        /// </summary>
        CtlColorBtn = 0x0135,

        /// <summary>
        ///     WM_CTLCOLORDLG
        /// </summary>
        CtlColorDlg = 0x0136,

        /// <summary>
        ///     WM_CTLCOLORSCROLLBAR
        /// </summary>
        CtlColorScrollbar = 0x0137,

        /// <summary>
        ///     WM_CTLCOLORSTATIC
        /// </summary>
        CtlColorStatic = 0x0138,

        /// <summary>
        ///     WM_GETHMENU
        /// </summary>
        GetHMenu = 0x01E1,

        /// <summary>
        ///     WM_MOUSEFIRST
        /// </summary>
        MouseFirst = 0x0200,

        /// <summary>
        ///     WM_MOUSEMOVE
        /// </summary>
        MouseMove = 0x0200,

        /// <summary>
        ///     WM_LBUTTONDOWN
        /// </summary>
        LButtonDown = 0x0201,

        /// <summary>
        ///     WM_LBUTTONUP
        /// </summary>
        LButtonUp = 0x0202,

        /// <summary>
        ///     WM_LBUTTONDBLCLK
        /// </summary>
        LButtonDblClk = 0x0203,

        /// <summary>
        ///     WM_RBUTTONDOWN
        /// </summary>
        RButtonDown = 0x0204,

        /// <summary>
        ///     WM_RBUTTONUP
        /// </summary>
        RButtonUp = 0x0205,

        /// <summary>
        ///     WM_RBUTTONDBLCLK
        /// </summary>
        RButtonDblClk = 0x0206,

        /// <summary>
        ///     WM_MBUTTONDOWN
        /// </summary>
        MButtonDown = 0x0207,

        /// <summary>
        ///     WM_MBUTTONUP
        /// </summary>
        MButtonUp = 0x0208,

        /// <summary>
        ///     WM_MBUTTONDBLCLK
        /// </summary>
        MButtonDblClk = 0x0209,

        /// <summary>
        ///     WM_MOUSEWHEEL
        /// </summary>
        MouseWheel = 0x020A,

        /// <summary>
        ///     WM_XBUTTONDOWN
        /// </summary>
        XButtonDown = 0x020B,

        /// <summary>
        ///     WM_XBUTTONUP
        /// </summary>
        XButtonUp = 0x020C,

        /// <summary>
        ///     WM_XBUTTONDBLCLK
        /// </summary>
        XButtonDblClk = 0x020D,

        /// <summary>
        ///     WM_MOUSEHWHEEL
        /// </summary>
        MouseHWheel = 0x020E,

        /// <summary>
        ///     WM_MOUSELAST
        /// </summary>
        MouseLast = 0x020E,

        /// <summary>
        ///     WM_PARENTNOTIFY
        /// </summary>
        ParentNotify = 0x0210,

        /// <summary>
        ///     WM_ENTERMENULOOP
        /// </summary>
        EnterMenuLoop = 0x0211,

        /// <summary>
        ///     WM_EXITMENULOOP
        /// </summary>
        ExitMenuLoop = 0x0212,

        /// <summary>
        ///     WM_NEXTMENU
        /// </summary>
        NextMenu = 0x0213,

        /// <summary>
        ///     WM_SIZING
        /// </summary>
        Sizing = 0x0214,

        /// <summary>
        ///     WM_CAPTURECHANGED
        /// </summary>
        CaptureChanged = 0x0215,

        /// <summary>
        ///     WM_MOVING
        /// </summary>
        Moving = 0x0216,

        /// <summary>
        ///     WM_POWERBROADCAST
        /// </summary>
        PowerBroadcast = 0x0218,

        /// <summary>
        ///     WM_DEVICECHANGED
        /// </summary>
        DeviceChange = 0x0219,

        /// <summary>
        ///     WM_MDICREATE
        /// </summary>
        MDICreate = 0x0220,

        /// <summary>
        ///     WM_MDIDESTROY
        /// </summary>
        MDIDestroy = 0x0221,

        /// <summary>
        ///     WM_MDIACTIVATE
        /// </summary>
        MDIActivate = 0x0222,

        /// <summary>
        ///     WM_MDIRESTORE
        /// </summary>
        MDIRestore = 0x0223,

        /// <summary>
        ///     WM_MDINEXT
        /// </summary>
        MDINext = 0x0224,

        /// <summary>
        ///     WM_MDIMAXIMIZE
        /// </summary>
        MDIMaximize = 0x0225,

        /// <summary>
        ///     WM_MDITILE
        /// </summary>
        MDITile = 0x0226,

        /// <summary>
        ///     WM_MDICASCADE
        /// </summary>
        MDICascade = 0x0227,

        /// <summary>
        ///     WM_MDIICONARRANGE
        /// </summary>
        MDIIconArrange = 0x0228,

        /// <summary>
        ///     WM_MDIGETACTIVE
        /// </summary>
        MDIGetActive = 0x0229,

        /// <summary>
        ///     WM_MDISETMENU
        /// </summary>
        MDISetMenu = 0x0230,

        /// <summary>
        ///     WM_ENTERSIZEMOVE
        /// </summary>
        EnterSizeMove = 0x0231,

        /// <summary>
        ///     WM_EXITSIZEMOVE
        /// </summary>
        ExitSizeMove = 0x0232,

        /// <summary>
        ///     WM_DROPFILES
        /// </summary>
        DropFiles = 0x0233,

        /// <summary>
        ///     WM_MDIREFRESHMENU
        /// </summary>
        MDIRefResHMenu = 0x0234,

        /// <summary>
        ///     WM_TOUCH
        /// </summary>
        Touch = 0x0240,

        /// <summary>
        ///     WM_IME_SETCONTEXT
        /// </summary>
        IMESetContext = 0x0281,

        /// <summary>
        ///     WM_IME_NOTIFY
        /// </summary>
        IMENotify = 0x0282,

        /// <summary>
        ///     WM_IME_CONTROL
        /// </summary>
        IMEControl = 0x0283,

        /// <summary>
        ///     WM_IME_COMPOSITIONFULL
        /// </summary>
        IMECompositionFull = 0x0284,

        /// <summary>
        ///     WM_IME_SELECT
        /// </summary>
        IMESelect = 0x0285,

        /// <summary>
        ///     WM_IME_CHAR
        /// </summary>
        IMEChar = 0x0286,

        /// <summary>
        ///     WM_IME_REQUEST
        /// </summary>
        IMERequest = 0x0288,

        /// <summary>
        ///     WM_IME_KEYDOWN
        /// </summary>
        IMEKeyDown = 0x0290,

        /// <summary>
        ///     WM_IME_KEYUP
        /// </summary>
        IMEKeyUp = 0x0291,

        /// <summary>
        ///     WM_MOUSEHOVER
        /// </summary>
        MouseHover = 0x02A1,

        /// <summary>
        ///     WM_MOUSELEAVE
        /// </summary>
        MouseLeave = 0x02A3,

        /// <summary>
        ///     WM_NCMOUSEHOVER
        /// </summary>
        NCMouseHover = 0x02A0,

        /// <summary>
        ///     WM_NCMOUSELEAVE
        /// </summary>
        NCMouseLeave = 0x02A2,

        /// <summary>
        ///     WM_WTLSESSIONCHANGE
        /// </summary>
        WTLSessionChange = 0x02B1,

        /// <summary>
        ///     WM_TABLETFIRST
        /// </summary>
        TabletFirst = 0x02c0,

        /// <summary>
        ///     WM_TABLETLAST
        /// </summary>
        TabletLast = 0x02df,

        /// <summary>
        ///     WM_CUT
        /// </summary>
        Cut = 0x0300,

        /// <summary>
        ///     WM_COPY
        /// </summary>
        Copy = 0x0301,

        /// <summary>
        ///     WM_PASTE
        /// </summary>
        Paste = 0x0302,

        /// <summary>
        ///     WM_CLEAR
        /// </summary>
        Clear = 0x0303,

        /// <summary>
        ///     WM_UNDO
        /// </summary>
        Undo = 0x0304,

        /// <summary>
        ///     WM_RENDERFORMAT
        /// </summary>
        RenderFormat = 0x0305,

        /// <summary>
        ///     WM_RENDERALLFORMATS
        /// </summary>
        RenderAllFormats = 0x0306,

        /// <summary>
        ///     WM_DESTROYCLIPBOARD
        /// </summary>
        DestroyClipBoard = 0x0307,

        /// <summary>
        ///     WM_DRAWCILPBOARD
        /// </summary>
        DrawClipBoard = 0x0308,

        /// <summary>
        ///     WM_PAINTCLIPBOARD
        /// </summary>
        PaintClipBoard = 0x0309,

        /// <summary>
        ///     WM_VSCROLLCLIPBOARD
        /// </summary>
        VScrollClipBoard = 0x030A,

        /// <summary>
        ///     WM_SIZECLIPBOARD
        /// </summary>
        SizeClipBoard = 0x030B,

        /// <summary>
        ///     WM_ASKCBFORMATNAME
        /// </summary>
        AskCBFormatName = 0x030C,

        /// <summary>
        ///     WM_CHANGECBCHAIN
        /// </summary>
        ChangeCBChain = 0x030D,

        /// <summary>
        ///     WM_HSCROLLCLIPBOARD
        /// </summary>
        HScrollClipBoard = 0x030E,

        /// <summary>
        ///     WM_QUERYNEWPALETTE
        /// </summary>
        QueryNewPalette = 0x030F,

        /// <summary>
        ///     WM_PALETTEISCHANGING
        /// </summary>
        PaletteIsChanging = 0x0310,

        /// <summary>
        ///     WM_PALETTECHANGED
        /// </summary>
        PaletteChanged = 0x0311,

        /// <summary>
        ///     WM_HOTKEY
        /// </summary>
        HotKey = 0x0312,

        /// <summary>
        ///     WM_PRINT
        /// </summary>
        Print = 0x0317,

        /// <summary>
        ///     WM_PRINTCLIENT
        /// </summary>
        PrintClient = 0x0318,

        /// <summary>
        ///     WM_APPCOMMAND
        /// </summary>
        AppCommand = 0x0319,

        /// <summary>
        ///     WM_THEMECHANGED
        /// </summary>
        ThemeChanged = 0x031A,

        /// <summary>
        ///     WM_CLIPBOARDUPDATE
        /// </summary>
        ClipBoardUpdate = 0x031D,

        /// <summary>
        ///     WM_DWMCOMPOSITIONCHANGED
        /// </summary>
        DWMCompositionChanged = 0x031E,

        /// <summary>
        ///     WM_DWMNCRENDERINGCHANGED
        /// </summary>
        DWMNCRenderingChanged = 0x031F,

        /// <summary>
        ///     WM_DWMCOLORIZATIONCOLORCHANGED
        /// </summary>
        DWMColorizationColorChanged = 0x0320,

        /// <summary>
        ///     WM_DWMWINDOWMAXIMIZEDCHANGE
        /// </summary>
        DWMWindowMaximizedChange = 0x0321,

        /// <summary>
        ///     WM_DWMSENDICONICTHUMBNAIL
        /// </summary>
        DWMSendIconicThumbnail = 0x0323,

        /// <summary>
        ///     WM_DWMSENDICONICLIVEPREVIEWBITMAP
        /// </summary>
        DWMSendIconicLivePreviewBitmap = 0x0326,

        /// <summary>
        ///     WM_GETTITLEBARINFOEX
        /// </summary>
        GetTitlebarInfoEx = 0x033F,

        /// <summary>
        ///     WM_HANDHELDFIRST
        /// </summary>
        HandheldFirst = 0x0358,

        /// <summary>
        ///     WM_HANDHELDLAST
        /// </summary>
        HandheldLast = 0x035F,

        /// <summary>
        ///     WM_AFXFIRST
        /// </summary>
        AFXFirst = 0x0360,

        /// <summary>
        ///     WM_AFXLAST
        /// </summary>
        AFXLast = 0x037F,

        /// <summary>
        ///     WM_PENWINFIRST
        /// </summary>
        PenWinFirst = 0x0380,

        /// <summary>
        ///     WM_PENWINLAST
        /// </summary>
        PenWinLast = 0x038F,

        /// <summary>
        ///     WM_APP
        /// </summary>
        App = 0x8000,

        /// <summary>
        ///     WM_USER
        /// </summary>
        User = 0x0400
    }
}