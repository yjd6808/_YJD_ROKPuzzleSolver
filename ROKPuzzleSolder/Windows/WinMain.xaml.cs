using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ROKPuzzleSolder.PuzzleSolver;

using OpenCvSharp;

using OpenCVSize = OpenCvSharp.Size;
using OpenCVRect2D = OpenCvSharp.Rect2d;
using OpenCVPoint = OpenCvSharp.Point;
using WPFImage = System.Windows.Controls.Image;
using WPFWindow = System.Windows.Window;


namespace ROKPuzzleSolder
{
    public partial class WinMain : WPFWindow
    {
        ROKPuzzleSolver _Solver;

        public WinMain()
        {
            InitializeComponent();

            _Solver = ROKPuzzleSolver.GetInstance();
            _Solver.InitializeLogger(_WMListBox);
            _Solver.InitializeImageControl(_WMImage, _WMPiece1MaskImage, _WMPiece2MaskImage);
            _Solver.InitializeSetting();
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
            _Solver.StopSolve();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _WMSettingButton_Click(object sender, RoutedEventArgs e)
        {
            WinSetting _SettingWindow = new WinSetting();

            _SettingWindow.ShowDialog();

            if (_SettingWindow.DialogResult.HasValue && _SettingWindow.DialogResult.Value)
            {
                if (ROKPuzzleSolverSetting.Save())
                {
                    _Solver.GetLogger().AddLog("설정을 성공적으로 저장하였습니다.");
                }
                else
                {
                    _Solver.GetLogger().AddLog("설정을 저장하는데 실패했습니다.");
                }
            }
        }

        private void _WMStartButton_Click(object sender, RoutedEventArgs e)
        {

            if (_Solver.IsSolving())
            {
                _Solver.StopSolve();
                _WMStartButton.Content = "풀기 시작";
            }
            else
            {
                if (_Solver.StartSolve())
                {
                    _WMStartButton.Content = "중지";
                }
            }
        }

        private void _WMDrawButton_Click(object sender, RoutedEventArgs e)
        {
            //Mat _Piece = new Mat("piece.png");
            //OpenCVRect2D _ContourRect = new OpenCVRect2D();
            //Mat _BinaryContourMat = null;
            //List<OpenCVPoint> _RandPoints = new List<OpenCVPoint>();


            ////스케일링
            //if (ROKPuzzleSolverSetting.UsePieceScaling)
            //{
            //    _Piece = ROKPuzzleOpenCV.ResizeRelative(_Piece, ROKPuzzleSolverSetting.PieceScale, ROKPuzzleSolverSetting.PieceScale);
            //}

            ////윤곽선 자르기
            //if (ROKPuzzleOpenCV.FitToContourRect(_Piece, 230, out _ContourRect, out _BinaryContourMat))
            //{
            //    _Piece = _Piece.SubMat(_ContourRect);
            //}

            //List<int> _PixelColorsOfRandPoints = new List<int>();

            //if (ROKPuzzleOpenCV.GetRandomPointsInBinaryImage(_BinaryContourMat, BlackOrWhite.Black, 100, 100, out _RandPoints))
            //{
            //    Mat _N = new Mat();
            //    Cv2.CvtColor(_Piece, _N, ColorConversionCodes.BGR2GRAY);


            //    for (int i = 0; i < _RandPoints.Count; i++)
            //    {
            //        PixelColors.Add(_N.At<byte>(_RandPoints[i].Y, _RandPoints[i].X));
            //    }

            //    _RandPoints.ForEach(x => _PixelColorsOfRandPoints.Add())
            //    _PixelColorsOfRandPoints.Sort((a, b) => b.CompareTo(a));
            //}

            //MessageBox.Show(PixelColors.Take(20).Average().ToString());

            //Mat _Mask = new Mat();

            //Cv2.CvtColor(_Piece, _Piece, ColorConversionCodes.BGR2GRAY);
            //Cv2.Threshold(_Piece, _Mask, PixelColors.Take(20).Average(), 255, ThresholdTypes.BinaryInv);

            //Cv2.ImShow("a", _Mask);
        }

        private void _WMEraseButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _WMDrawCircleButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _WMEraseCircleButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
