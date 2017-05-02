using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Galateia.Infra.WindowsAPI
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SHELLHOOKINFO
    {
        public IntPtr hwnd;
        public RECT rc;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct COLORREF
    {
        [FieldOffset(0)] public uint dword;
        [FieldOffset(0)] public byte a;
        [FieldOffset(1)] public byte b;
        [FieldOffset(2)] public byte g;
        [FieldOffset(3)] public byte r;

        public COLORREF(byte r, byte g, byte b)
        {
            dword = 0x00000000;
            a = 0x00;
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public COLORREF(byte a, byte r, byte g, byte b)
        {
            dword = 0x00000000;
            this.a = a;
            this.r = r;
            this.g = g;
            this.b = b;
        }
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        /// <summary>
        ///     The x-coordinate of the point.
        /// </summary>
        public Int32 x;

        /// <summary>
        ///     The y-coordinate of the point.
        /// </summary>
        public Int32 y;

        /// <summary>
        ///     Initializes a new instance of the NKYTech.Native.POINT structure with specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the point.</param>
        /// <param name="y">The y-coordinate of the point.</param>
        public POINT(Int32 x, Int32 y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        ///     Initializes a new instance of the NKYTech.Native.POINT structure with a specified instance of the
        ///     System.Drawing.Point class.
        /// </summary>
        /// <param name="point">point.</param>
        /// <exception cref="System.ArgumentNullException">point is null.</exception>
        public POINT(Point point)
        {
            x = point.X;
            y = point.Y;
        }

        /// <summary>
        ///     Converts the specified NKYTech.Native.POINT structure to a System.Drawing.Point structure.
        /// </summary>
        /// <param name="point">The NKYTech.Native.Point to be convertied.</param>
        /// <returns>The System.Drawing.Point that results from the conversion.</returns>
        public static implicit operator Point(POINT point)
        {
            return new Point(point.x, point.y);
        }
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct SIZE
    {
        public Int32 cx;
        public Int32 cy;

        public SIZE(int cx, int cy)
        {
            this.cx = cx;
            this.cy = cy;
        }

        public SIZE(Size size)
        {
            cx = size.Width;
            cy = size.Height;
        }

        public static implicit operator Size(SIZE size)
        {
            return new Size(size.cx, size.cy);
        }
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        /// <summary>
        ///     The x-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public Int32 left;

        /// <summary>
        ///     The y-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public Int32 top;

        /// <summary>
        ///     The x-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public Int32 right;

        /// <summary>
        ///     The y-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public Int32 bottom;

        /// <summary>
        ///     Initializes a new instance of the NKYTech.Native.RECT structure with specified coordinates.
        /// </summary>
        /// <param name="left">The x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="right">The x-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="bottom">The y-coordinate of the lower-right corner of the rectangle.</param>
        public RECT(Int32 left, Int32 top, Int32 right, Int32 bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        /// <summary>
        ///     Initializes a new instance of the NKYTech.Native.RECT structure with a specified instance of the
        ///     System.Drawing.Rectangle class.
        /// </summary>
        /// <param name="rect">rectangle.</param>
        /// <exception cref="System.ArgumentNullException">rect is null.</exception>
        public RECT(Rectangle rect)
        {
            left = rect.Left;
            top = rect.Top;
            right = rect.Right;
            bottom = rect.Bottom;
        }

        /// <summary>
        ///     Returns the width of the current rectangle.
        /// </summary>
        public Int32 Width
        {
            get { return (right - left); }
        }

        /// <summary>
        ///     Returns the height of the current rectangle.
        /// </summary>
        public Int32 Height
        {
            get { return (bottom - top); }
        }

        /// <summary>
        ///     Converts the specified NKYTech.Native.RECT structure to a System.Drawing.Rectangle structure.
        /// </summary>
        /// <param name="rect">The NKYTech.Native.Rectangle to be convertied.</param>
        /// <returns>The System.Drawing.Rectangle that results from the conversion.</returns>
        public static implicit operator Rectangle(RECT rect)
        {
            return new Rectangle(rect.left, rect.top, rect.Width, rect.Height);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WNDCLASSEX
    {
        public UInt32 cbSize;
        public ClassStyles style;
        public IntPtr lpfnWndProc;
        public Int32 cbClsExtra;
        public Int32 cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        [MarshalAs(UnmanagedType.LPWStr)] public string lpszMenuName;
        [MarshalAs(UnmanagedType.LPWStr)] public string lpszClassName;
        public IntPtr hIconSm;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPOS
    {
        public IntPtr hWnd;
        public IntPtr hWndInsertAfter;
        public int x;
        public int y;
        public int cx;
        public int cy;
        public SetWindowPosFlag flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KBDLLHOOKSTRUCT
    {
        public UInt32 vkCode;
        public UInt32 scanCode;
        public LowLevelKeyboardHookFlags flags;
        public UInt32 time;
        public UIntPtr dwExtraInfo;
    }
}