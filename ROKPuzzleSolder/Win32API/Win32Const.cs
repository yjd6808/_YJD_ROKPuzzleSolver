// ===============================
// @AUTHOR      : 윤정도
// @CREATE DATE : 2021-04-06-화 오후 8:29:31   
// @PURPOSE     : WINAPI 상수 정리
// ===============================



using System;

namespace ROKPuzzleSolder
{
    public static partial class Win32Helper
    {
        public static class Const
        {
            //UpdateLayeredWindow 함수 플래그값
            //https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-updatelayeredwindow
            public static readonly uint ULW_COLORKEY                = 0x00000001;
            public static readonly uint ULW_ALPHA                   = 0x00000002;
            public static readonly uint ULW_OPAQUE                  = 0x00000004;
            public static readonly uint ULW_EX_NORESIZE             = 0x00000008;

            //BLENDFUNCTION 구조체에서 사용되는 값? 알파 블랜딩 관련
            //https://docs.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-blendfunction
            public static readonly byte AC_SRC_OVER                 = 0x00;
            public static readonly byte AC_SRC_ALPHA                = 0x01;


            //윈도우 스타일 관련
            //https://docs.microsoft.com/en-us/windows/win32/winmsg/extended-window-styles
            public static readonly uint WS_EX_LAYERED               = 0x00080000;
            public static readonly uint WS_EX_TRANSPARENT           = 0x00000020;

            //SetWindowLong 관련
            //https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowlonga
            public static readonly int GWL_EXSTYLE                  = -20;                  // Sets a new extended window style.
            public static readonly int GWL_HINSTANCE                = -6;                   // Sets a new application instance handle.
            public static readonly int GWL_ID                       = -12;                  // Sets a new identifier of the child window.The window cannot be a top-level window.
            public static readonly int GWL_STYLE                    = -16;                  // Sets a new window style.
            public static readonly int GWL_USERDATA                 = -21;                  // Sets the user data associated with the window. This data is intended for use by the application that created the window. Its value is initially zero.
            public static readonly int GWL_WNDPROC                  = -4;                   // Sets a new address for the window procedure. You cannot change this attribute if the window does not belong to the same process as the calling thread.

            //SetWindowPos 관련
            //https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos
            public static readonly IntPtr HWND_BOTTOM               = (IntPtr)(1);          // Places the window at the bottom of the Z order. 
                                                                                            // If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
            public static readonly IntPtr HWND_NOTOPMOST            = (IntPtr)(-2);         // Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
            public static readonly IntPtr HWND_TOP                  = (IntPtr)(0);          // Places the window at the top of the Z order.
            public static readonly IntPtr HWND_TOPMOST              = (IntPtr)(-1);         // Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.

            public static readonly uint SWP_NOSIZE                  = 0x0001;               // 현재 사이즈 고정 (cx, cy 매개변수 무시)
            public static readonly uint SWP_NOMOVE                  = 0x0002;               // 현재 위치 고정 (x, y 매개변수 무시)
            public static readonly uint SWP_NOZORDER                = 0x0004;               // 현재 Z 오더 고정 (hWndInsertAfter 매개변수 값 무시)
            public static readonly uint SWP_NOACTIVATE              = 0x0010;               // 최소화 상태로 변환시 다시 활성화가 안됨
            public static readonly uint SWP_DRAWFRAME               = 0x0020;               // 윈도우를 그림
        }
    }
}
