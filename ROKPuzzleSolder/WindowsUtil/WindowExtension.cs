// ===============================
// @AUTHOR      : 윤정도
// @CREATE DATE : 2021-04-06-화 오후 9:06:01   
// @PURPOSE     : WPF 윈도우 확장기능 추가
// ===============================


using System;
using System.Windows;
using System.Windows.Interop;

namespace ROKPuzzleSolder
{
    public static class WindowExtension
    {
        public static void SetTopMostEnabled(this Window _Window, bool _Enable)
        {
            IntPtr _WindowHandle = new WindowInteropHelper(_Window).Handle;

            if (_WindowHandle == IntPtr.Zero)
                throw new Exception(_Window.Title + "의 핸들이 null입니다.");

            uint _Flags = 0x0;

            if (_Enable)
            {
                _Flags |= Win32Helper.Const.SWP_NOSIZE;
                _Flags |= Win32Helper.Const.SWP_NOMOVE;
                _Flags |= Win32Helper.Const.SWP_NOACTIVATE;
                _Flags |= Win32Helper.Const.SWP_DRAWFRAME;
                _Flags |= Win32Helper.Const.SWP_NOZORDER;

                Win32Helper.Method.SetWindowPos(_WindowHandle, Win32Helper.Const.HWND_TOP, 0, 0, 0, 0, _Flags);

                _Flags &= ~Win32Helper.Const.SWP_NOZORDER;

                Win32Helper.Method.SetWindowPos(_WindowHandle, Win32Helper.Const.HWND_TOPMOST, 0, 0, 0, 0, _Flags);
            }
            else
            {
                _Flags |= Win32Helper.Const.SWP_NOSIZE;
                _Flags |= Win32Helper.Const.SWP_NOMOVE;
                _Flags |= Win32Helper.Const.SWP_NOACTIVATE;
                _Flags |= Win32Helper.Const.SWP_DRAWFRAME;
                _Flags |= Win32Helper.Const.SWP_NOZORDER;

                Win32Helper.Method.SetWindowPos(_WindowHandle, Win32Helper.Const.HWND_TOP, 0, 0, 0, 0, _Flags);

                _Flags &= ~Win32Helper.Const.SWP_NOZORDER;

                Win32Helper.Method.SetWindowPos(_WindowHandle, Win32Helper.Const.HWND_TOPMOST, 0, 0, 0, 0, _Flags);
            }
        }

        public static void SetClickThroughEnabled(this Window _Window, bool _Enable)
        {
            IntPtr _WindowHandle = new WindowInteropHelper(_Window).Handle;

            if (_WindowHandle == IntPtr.Zero)
                throw new Exception(_Window.Title + "의 핸들이 null입니다.");

            int _Index = Win32Helper.Const.GWL_EXSTYLE;
            uint _ExStyle = Win32Helper.Method.GetWindowLong(_WindowHandle, _Index);
            

            if (_Enable)
            {
                _ExStyle |= Win32Helper.Const.WS_EX_LAYERED;
                _ExStyle |= Win32Helper.Const.WS_EX_TRANSPARENT;
            }
            else
            {
                _ExStyle &= ~Win32Helper.Const.WS_EX_TRANSPARENT;
            }

            if (Win32Helper.Method.SetWindowLong(_WindowHandle, _Index, _ExStyle) == 0)
            {
                throw new Exception(_Window.Title + "의 SetClickThroughEnabled 설정에 실패했습니다.");
            }
        }
    }
}
