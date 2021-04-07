// ===============================
// @AUTHOR      : 윤정도
// @CREATE DATE : 2021-04-03-토 오후 11:49:26   
// @PURPOSE     : 라오킹 퍼즐 풀어주는 클라스
// ===============================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using OpenCvSharp;
using System.IO;
using System.Drawing;
using MoreLinq;
using System.Windows.Controls;

using OpenCVSize = OpenCvSharp.Size;
using OpenCVRect = OpenCvSharp.Rect;
using OpenCVPoint = OpenCvSharp.Point;
using WPFImage = System.Windows.Controls.Image;
using System.Windows.Threading;
using System.Windows;

namespace ROKPuzzleSolder.PuzzleSolver
{
    public class ROKPuzzleSolver
    {
        private static ROKPuzzleSolver Instance;

        private Logger _Logger = null;
        private WPFImage _ResultImage = null;
        private WPFImage _PieceResultImage = null;
        private WPFImage _PieceMaskImage = null;
        private Thread _SolveThread = null;
        private Thread _DrawingThread = null;
        private WinBoardDisplay _BoardWindow = null;
        private MatchTemplateResult _Answer = new MatchTemplateResult();
        private object _SolveThreadLock = new object();
        private volatile bool _IsSolving = false;

        private ROKPuzzleSolver()
        {
        }

        public static ROKPuzzleSolver GetInstance()
        {
            if (Instance == null)
                Instance = new ROKPuzzleSolver();

            return Instance;
        }


        public void InitializeLogger(ListBox _ListBox)
        {
            _Logger = new Logger(_ListBox);
        }

        public void InitializeImageControl(WPFImage _Image, WPFImage _PieceImage, WPFImage _MaskImage)
        {
            _ResultImage = _Image;
            _PieceResultImage = _PieceImage;
            _PieceMaskImage = _MaskImage;
        }

        public void InitializeSetting()
        {
            ROKPuzzleSolverSetting.Initialize();

            if (ROKPuzzleSolverSetting.Load() == false)
            {
                _Logger.AddLog("로딩할 세팅 파일이 없습니다.");
            }
            else
            {
                _Logger.AddLog("세팅파일을 성공적으로 로딩하였습니다.");
            }
        }

        public bool StartSolve()
        {
            if (_IsSolving)
            {
                _Logger.AddLog("이미 퍼즐을 푸는 중입니다.");
                return false;
            }

            if (File.Exists("background.png") == false)
            {
                _Logger.AddLog("배경 파일(background.png)파일이 존재하지 않습니다.");
                return false;
            }

            if (ROKPuzzleSolverSetting.PieceRect.IsAvaiableRect() == false)
            {
                _Logger.AddLog("조각 캡쳐 영역을 설정 후 다시 시도해주세요.");
                return false;
            }

            lock (_SolveThreadLock)
            {
                _IsSolving = true;
            }

            _BoardWindow = new WinBoardDisplay();
            _BoardWindow.Show();
            _BoardWindow.DrawPuzzleBoard();

            _SolveThread = new Thread(SolveThreadFunction);
            _SolveThread.Start();

            _DrawingThread = new Thread(DrawThreadFunction);
            _DrawingThread.Start();

            _Logger.AddLog("퍼즐 풀기가 시작되었습니다.");

            
            return true;
        }

        public void StopSolve()
        {
            lock (_SolveThreadLock)
            {
                _IsSolving = false;
            }

            if (_BoardWindow != null)
                _BoardWindow.Close();

            if (_SolveThread != null)
                _SolveThread.Abort();

            if (_DrawingThread != null)
                _DrawingThread.Abort();

            _Logger.AddLog("퍼즐 풀기가 중지되었습니다.");
        }

        public Logger GetLogger()
        {
            return _Logger;
        }

        public bool IsSolving()
        {
            return _IsSolving;
        }


        // =========================================
        // 퍼즐 푸는 쓰레드
        // =========================================


        private void SolveThreadFunction()
        {
            int _CurrentDelay = 0;
            Mat _OriginalBackground = Cv2.ImRead("background.png");
            try
            {
                while (_IsSolving)
                {
                    if (_CurrentDelay < ROKPuzzleSolverSetting.SolveDelay)
                    {
                        _CurrentDelay += 10;
                        Thread.Sleep(10);
                        continue;
                    }

                    //퍼즐 조각 캡쳐
                    Mat _Piece = ROKPuzzleOpenCV.ScreenToBitmap(ROKPuzzleSolverSetting.PieceRect).ConvertToMat();
                    Mat _Background = _OriginalBackground.Clone();
                    Mat _Mask = new Mat();
                    Dispatcher _ControllerDispatcher = _ResultImage.Dispatcher;

                    OpenCVRect _ContourRect = new OpenCVRect();
                    Mat _BinaryContourMat = null;
                    //스케일링
                    if (ROKPuzzleSolverSetting.UsePieceScaling)
                    {
                        _Piece = ROKPuzzleOpenCV.ResizeRelative(_Piece, ROKPuzzleSolverSetting.PieceScale, ROKPuzzleSolverSetting.PieceScale);
                    }

                    //윤곽선 자르기
                    if (ROKPuzzleSolverSetting.FitImageWithContour)
                    {
                        //라오킹 퍼즐이 있는 배경영역의 평균 색값이 240정도로 나오던데 230정도로 잡으면 될듯?
                        if (ROKPuzzleOpenCV.FitToContourRect(_Piece, ROKPuzzleSolverSetting.PieceStandardBinaryThreshold, out _ContourRect, out _BinaryContourMat))
                        {
                            _Piece = _Piece.SubMat(_ContourRect);
                        }
                    }

                    //그레이 스케일링
                    Cv2.CvtColor(_Piece, _Piece, ColorConversionCodes.BGR2GRAY);
                    Cv2.CvtColor(_Background, _Background, ColorConversionCodes.RGB2GRAY);

                    //_ContourRect 내부의 점 랜덤으로 생성
                    //_BinaryContourMat 안의 점인지는 255이면 내부의 점으로 판단하자.
                    //100개의 점의 픽셀값을 추출해서 평균값을 BinaryThreadHold값으로 하자.
                    //Cv2.PointPolygonTest() //폴리곤 안에 점이 있는지 검사하는 함수

                    List<OpenCVPoint> _RandomPointsInBinaryMat = new List<OpenCVPoint>();       // 윤곽선의 사각영역 내부에 랜덤으로 생성된 점
                    List<int> _PixelColorsOfRandPoints = new List<int>();                       // 윤곽선의 사각영역 내부에 랜덤으로 생성된 점들의 픽셀값들

                    if (ROKPuzzleOpenCV.GetRandomPointsInBinaryImage(_BinaryContourMat, BlackOrWhite.Black, ROKPuzzleSolverSetting.RandomPointCountsInContourRect, 100, out _RandomPointsInBinaryMat))
                    {
                        _RandomPointsInBinaryMat.ForEach(x => _PixelColorsOfRandPoints.Add(_Piece.At<byte>(x.Y, x.X)));
                        _PixelColorsOfRandPoints.Sort((a, b) => b.CompareTo(a));

                        int _TakeCount = (int)((double)ROKPuzzleSolverSetting.RandomPointCountsInContourRect * ROKPuzzleSolverSetting.RandomPointsAverageUpperPercent / 100.0);
                        double _StandardThreshHold = _PixelColorsOfRandPoints.Take(_TakeCount).Average();
                        Cv2.Threshold(_Piece, _Mask, _StandardThreshHold, 255, ThresholdTypes.BinaryInv);
                    }
                    else
                    {
                        Cv2.Threshold(_Piece, _Mask, ROKPuzzleSolverSetting.MaskStandardBinaryThreshold, 255, ThresholdTypes.BinaryInv);
                    }
                    
                    _ControllerDispatcher.Invoke(() => _PieceMaskImage.Source = _Mask.ConvertToBitmapSource());

                    if (ROKPuzzleOpenCV.Contains(_Background, _Piece, _Mask, ROKPuzzleSolverSetting.MatchMethod, ROKPuzzleSolverSetting.MinimumMatchValue, ROKPuzzleSolverSetting.MatchSimilarityInterval, out List <MatchTemplateResult> _FindRects))
                    {
                        _FindRects.Sort((a, b) => a.Similarity.CompareTo(b.Similarity)); //오름차순 정렬

                        lock (_SolveThreadLock)
                        {
                            _Answer = _FindRects.Last();
                        }

                        //Mat _ClonedBacgroundMat = _OriginalBackground.Clone();

                        //for (int i = 0; i < _FindRects.Count - 1; i++)
                        //{
                        //    Cv2.Rectangle(_ClonedBacgroundMat, _FindRects[i].Rect.TopLeft, _FindRects[i].Rect.BottomRight, new Scalar(30, 20, 240), 2);
                        //    Cv2.PutText(_ClonedBacgroundMat, Math.Round(_FindRects[i].Similarity, 2).ToString(), new OpenCVPoint(_FindRects[i].Rect.X, _FindRects[i].Rect.Y + 20), HersheyFonts.HersheyComplexSmall, 1.0, new Scalar(30, 20, 240), 2);
                        //}

                        //Cv2.Rectangle(_ClonedBacgroundMat, _Answer.Rect.TopLeft, _Answer.Rect.BottomRight, new Scalar(20, 255, 40), 2);
                        //Cv2.PutText(_ClonedBacgroundMat, Math.Round(_Answer.Similarity, 2).ToString(), new OpenCVPoint(_Answer.Rect.X, _Answer.Rect.Y + 20), HersheyFonts.HersheyComplexSmall, 1.0, new Scalar(20, 255, 40), 2);

                        //_ControllerDispatcher.Invoke(() =>
                        //{
                        //    _ResultImage.Source = _ClonedBacgroundMat.ConvertToBitmapSource();
                        //    _PieceResultImage.Source = _Piece.ConvertToBitmapSource();
                        //});
                    }
                    else
                    {
                        _Logger.AddLog("찾지 못함");
                        //찾지 못함
                    }

                    _CurrentDelay = 0;
                }
            }
            catch (Exception _Exception)
            {
                try
                {
                    _ResultImage.Dispatcher.Invoke(() => _ResultImage.Source = _OriginalBackground.ConvertToBitmapSource());
                    _Logger.AddLog("퍼즐 풀기 쓰레드가 강제 중단되었습니다.");
                    MessageBox.Show(_Exception.Message + "\n\n" + _Exception.StackTrace);
                }
                catch  { }
            }
        }

        // =========================================
        // 그림 그리는 쓰레드
        // =========================================

        private void DrawThreadFunction()
        {
            try
            {
                bool _BlinkAnswer = false;

                while (_IsSolving)
                {
                    if (_BoardWindow == null)
                        continue;

                    _BoardWindow.Clear();

                    _BoardWindow.DrawPuzzleBoard();

                    if (_BlinkAnswer)
                    {
                        _BoardWindow.DrawWhereIsPiece(_Answer, new SolidBrush(Color.Yellow));
                        _BlinkAnswer = false;
                    }
                    else
                    {
                        _BoardWindow.DrawWhereIsPiece(_Answer, new SolidBrush(Color.FromArgb(0, 0, 0, 0)));
                        _BlinkAnswer = true;
                    }

                    Thread.Sleep(100);
                }
            }
            catch (Exception _Exception)
            {
                try
                {
                    _Logger.AddLog("퍼즐 그리기 쓰레드가 강제 중단되었습니다.");
                }
                catch { }
            }
        }
    }
}
