// ===============================
// @AUTHOR      : 윤정도
// @CREATE DATE : 2021-04-06-화 오후 8:21:47   
// @PURPOSE     : WINAPI 함수 정리
// ===============================

using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ROKPuzzleSolder
{
    public static partial class Win32Helper
    {
        public static class Method
        {

            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();

            [DllImport("user32.dll")]
            public static extern IntPtr GetDC(IntPtr hWnd);
            [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true)]
            public static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

            [DllImport("user32.dll")]
            public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

            [DllImport("Gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hdc);

            [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DeleteObject([In] IntPtr hObject);

            [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
            public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);


            [DllImport("user32.dll")]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll")]
            public static extern int SetWindowLong(IntPtr hWnd, int nIndex, UInt32 dwNewLong);

            [DllImport("user32.dll")]
            public extern static bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

            [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pptSrc, uint crKey, [In] ref Struct.BlendFunction pblend, uint dwFlags);
        }
    }
}
