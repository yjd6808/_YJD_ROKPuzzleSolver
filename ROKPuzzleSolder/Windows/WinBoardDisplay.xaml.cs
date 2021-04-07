using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

using ROKPuzzleSolder.PuzzleSolver;

using DrawingSize = System.Drawing.Size;
using DrawingPoint = System.Drawing.Point;
using System.Drawing.Drawing2D;

namespace ROKPuzzleSolder
{
    public partial class WinBoardDisplay : Window
    {
        public IntPtr WindowHandle;

        private IntPtr _ScreenDC;
        private IntPtr _CanvasDC;
        private IntPtr _CanvasBitmapHandle;

        private Graphics _CanvasGraphic;

        private Brush _BoardBrush;
        private Pen _BoardPen;

        private object _BoardLock = new object();

        private DispatcherTimer _Timer;

        public WinBoardDisplay()
        {
            InitializeComponent();
            InitializeWindow();
            InitializeTimer();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowHandle = new WindowInteropHelper(this).Handle;

            this.SetTopMostEnabled(true);
            this.SetClickThroughEnabled(true);
        }

        private void InitializeWindow()
        {
            this.Left = SystemInformation.VirtualScreen.Left;
            this.Top = SystemInformation.VirtualScreen.Top;
            this.Width = SystemInformation.VirtualScreen.Width;
            this.Height = SystemInformation.VirtualScreen.Height;

            using (Bitmap _Bitmap = new Bitmap((int)this.Width, (int)this.Height))
            {
                _CanvasBitmapHandle = _Bitmap.GetHbitmap(Color.FromArgb(0));
            }

            _ScreenDC = Win32Helper.Method.GetDC(IntPtr.Zero);
            _CanvasDC = Win32Helper.Method.CreateCompatibleDC(_ScreenDC);
            Win32Helper.Method.SelectObject(_CanvasDC, _CanvasBitmapHandle);
            _CanvasGraphic = Graphics.FromHdc(_CanvasDC);


            _BoardBrush = Brushes.Cyan;
            _BoardPen = new Pen(_BoardBrush, 2.0f);
        }

        private void InitializeTimer()
        {
            _Timer = new DispatcherTimer();
            _Timer.Interval = new TimeSpan(0, 0, 0, 0, 16);
            _Timer.Tick += _Timer_Tick;
            _Timer.Start();
        }

       

        public void Clear()
        {
            _CanvasGraphic.Clear(Color.Transparent);
        }

        public void DrawPuzzleBoard()
        {
            if (ROKPuzzleSolverSetting.BackgroundRect.IsAvaiableRect() == false)
                return;

            const int _PuzzleRow = 5;
            const int _PuzzleColumn = 7;

            int _PuzzlePosX = ROKPuzzleSolverSetting.BackgroundRect.X;
            int _PuzzlePosY = ROKPuzzleSolverSetting.BackgroundRect.Y;

            int _PuzzleWidth = ROKPuzzleSolverSetting.BackgroundRect.Width;
            int _PuzzleHeight = ROKPuzzleSolverSetting.BackgroundRect.Height;

            float _WidthInterval = _PuzzleWidth / (float)_PuzzleColumn;
            float _HeightInterval = _PuzzleHeight / (float)_PuzzleRow;

            //가로 줄 그리기
            lock (_BoardLock)
            {
                for (int _Row = 0; _Row < _PuzzleRow + 1; _Row++)
                {
                    float _LeftX = _PuzzlePosX;
                    float _LeftY = _PuzzlePosY + _Row * _HeightInterval;

                    float _RightX = _LeftX + _PuzzleWidth;
                    float _RightY = _LeftY;

                    _CanvasGraphic.DrawLine(_BoardPen, _LeftX, _LeftY, _RightX, _RightY);
                }

                //세로 줄 그리기
                for (int _Column = 0; _Column < _PuzzleColumn + 1; _Column++)
                {
                    float _LeftX = _PuzzlePosX + _Column * _WidthInterval;
                    float _LeftY = _PuzzlePosY;

                    float _RightX = _LeftX;
                    float _RightY = _LeftY + _PuzzleHeight;

                    _CanvasGraphic.DrawLine(_BoardPen, _LeftX, _LeftY, _RightX, _RightY);
                }
            }
        }

        public void DrawWhereIsPiece(MatchTemplateResult _Result, Brush _Brush)
        {
            const int _PuzzleRow = 5;
            const int _PuzzleColumn = 7;


            int _PuzzlePosX = ROKPuzzleSolverSetting.BackgroundRect.X;
            int _PuzzlePosY = ROKPuzzleSolverSetting.BackgroundRect.Y;

            int _PuzzleWidth = ROKPuzzleSolverSetting.BackgroundRect.Width;
            int _PuzzleHeight = ROKPuzzleSolverSetting.BackgroundRect.Height;

            float _WidthInterval = _PuzzleWidth / (float)_PuzzleColumn;
            float _HeightInterval = _PuzzleHeight / (float)_PuzzleRow;

            int _X = -1;
            int _Y = -1;

            int _ContainX = _Result.Rect.Left + _Result.Rect.Width / 2;
            int _ContainY = _Result.Rect.Top + _Result.Rect.Height / 2;

            //어디 블록에 속하는지 계산
            for (int _Column = 0; _Column < _PuzzleColumn + 1; _Column++)
            {
                if (_WidthInterval * _Column <= _ContainX && _WidthInterval * (_Column + 1) > _ContainX)
                {
                    _X = _Column;
                    break;
                }
            }

            for (int _Row = 0; _Row < _PuzzleRow + 1; _Row++)
            {
                if (_WidthInterval * _Row <= _ContainY && _WidthInterval * (_Row + 1) > _ContainY)
                {
                    _Y = _Row;
                    break;
                }
            }


            float _LeftX = _PuzzlePosX + _X * _WidthInterval;
            float _LeftY = _PuzzlePosY + _Y * _HeightInterval;

            lock (_BoardLock)
            {
                _CanvasGraphic.FillEllipse(_Brush, new RectangleF(
                    _LeftX + _WidthInterval / 2 - 10.0f,
                    _LeftY + _HeightInterval / 2 - 10.0f,
                    20.0f,
                    20.0f));
            }
        }

        private void _Timer_Tick(object sender, EventArgs e)
        {
            DrawingPoint _PointSrc = new DrawingPoint(0, 0);
            DrawingPoint _PointDst = new DrawingPoint((int)this.Left, (int)this.Top);
            DrawingSize _SizeDst = new DrawingSize((int)this.Width, (int)this.Height);

            Win32Helper.Struct.BlendFunction _Blend = new Win32Helper.Struct.BlendFunction();
            _Blend.BlendOp = Win32Helper.Const.AC_SRC_OVER;
            _Blend.BlendFlags = 0;
            _Blend.SourceConstantAlpha = 255;  // additional alpha multiplier to the whole image. value 255 means multiply with 1.
            _Blend.AlphaFormat = Win32Helper.Const.AC_SRC_ALPHA;

            if (Win32Helper.Method.UpdateLayeredWindow(WindowHandle, _ScreenDC, ref _PointDst, ref _SizeDst, _CanvasDC, ref _PointSrc, 0, ref _Blend, Win32Helper.Const.ULW_ALPHA))
            {
                //업데이트 완료
            }
            else
            {
                //업데이트 실패
            }

            //Clean-up
            Win32Helper.Method.ReleaseDC(IntPtr.Zero, _ScreenDC);

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.Opacity = 0.3f;
                this.DragMove();
                this.Opacity = 1.0f;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Win32Helper.Method.DeleteObject(_CanvasBitmapHandle);
            Win32Helper.Method.DeleteDC(_CanvasDC);

            _CanvasGraphic.Dispose();
        }
    }
}
