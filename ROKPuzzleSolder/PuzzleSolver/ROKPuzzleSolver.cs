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
using OpenCVRect2D = OpenCvSharp.Rect2d;
using OpenCVPoint = OpenCvSharp.Point;
using WPFImage = System.Windows.Controls.Image;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ROKPuzzleSolder.PuzzleSolver
{
    public class ROKPuzzleSolver
    {
        private static ROKPuzzleSolver Instance;

        private Logger _Logger = null;
        private WPFImage _ResultImage = null;
        private WPFImage _PieceResultImage = null;
        private WPFImage _Piece1MaskImage = null;
        private WPFImage _Piece2MaskImage = null;
        private Thread _Solve1Thread = null;
        private Thread _Solve2Thread = null;
        private Thread _DrawingThread = null;
        private WinBoardDisplay _BoardWindow = null;
        private MatchTemplateResult _Answer1 = new MatchTemplateResult();
        private MatchTemplateResult _Answer2 = new MatchTemplateResult();
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

        public void InitializeImageControl(WPFImage _Image, WPFImage _Mask1Image, WPFImage _Mask2Image)
        {
            _ResultImage = _Image;
            _Piece1MaskImage = _Mask1Image;
            _Piece2MaskImage = _Mask2Image;
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

            if (ROKPuzzleSolverSetting.Piece1Rect.IsAvaiableRect() == false)
            {
                _Logger.AddLog("1 조각 캡쳐 영역을 설정 후 다시 시도해주세요.");
                return false;
            }

            lock (_SolveThreadLock)
            {
                _IsSolving = true;
            }

            _BoardWindow = new WinBoardDisplay();
            _BoardWindow.Show();

            _Solve1Thread = new Thread(new ParameterizedThreadStart(SolveThreadFunction));
            _Solve1Thread.Start(1);

            if (ROKPuzzleSolverSetting.Piece2Rect.IsAvaiableRect())
            {
                _Solve2Thread = new Thread(new ParameterizedThreadStart(SolveThreadFunction));
                _Solve2Thread.Start(2);
            }
            

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

            if (_Solve1Thread != null)
                _Solve1Thread.Join();

            if (_Solve2Thread != null)
                _Solve2Thread.Join();

            if (_DrawingThread != null)
                _DrawingThread.Join();

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


        private void SolveThreadFunction(object _Index)
        {
            int _CurrentDelay = 0;
            Mat _OriginalBackground = Cv2.ImRead("background.png");
            int _PieceIdx = (int)_Index;


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

                    OpenCVRect2D _PieceRect = new OpenCVRect2D();

                    switch (_PieceIdx)
                    {
                        case 1:
                            _PieceRect = ROKPuzzleSolverSetting.Piece1Rect;
                            break;
                        case 2:
                            _PieceRect = ROKPuzzleSolverSetting.Piece2Rect;
                            break;
                    }

                    if (_PieceRect.IsAvaiableRect() == false)
                        throw new Exception(_PieceIdx + "번 퍼즐 조각의 영역이 제대로 설정되어 있지 않습니다.");

                    //퍼즐 조각 캡쳐
                    Mat _Piece = ROKPuzzleOpenCV.ScreenToBitmap(_PieceRect.ToRectInt()).ConvertToMat();
                    Mat _Background = _OriginalBackground.Clone();
                    Mat _Mask = new Mat();
                    Dispatcher _ControllerDispatcher = _ResultImage.Dispatcher;

                    OpenCVRect2D _ContourRect = new OpenCVRect2D();
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
                            _Piece = _Piece.SubMat(_ContourRect.ToRectInt());
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

                    if (_PieceIdx == 1)
                        _ControllerDispatcher.Invoke(() => _Piece1MaskImage.Source = _Mask.ConvertToBitmapSource());
                    else if (_PieceIdx == 2)
                        _ControllerDispatcher.Invoke(() => _Piece2MaskImage.Source = _Mask.ConvertToBitmapSource());

                    if (ROKPuzzleOpenCV.Contains(_Background, _Piece, _Mask, ROKPuzzleSolverSetting.MatchMethod, ROKPuzzleSolverSetting.MinimumMatchValue, ROKPuzzleSolverSetting.MatchSimilarityInterval, out List<MatchTemplateResult> _FindRects))
                    {
                        _FindRects.Sort((a, b) => a.Similarity.CompareTo(b.Similarity)); //오름차순 정렬

                        lock (_SolveThreadLock)
                        {
                            if (_PieceIdx == 1)
                                _Answer1 = _FindRects.Last();
                            else if (_PieceIdx == 2)
                                _Answer2 = _FindRects.Last();
                        }
                    }
                    else
                    {
                        _Logger.AddLog("찾지 못함");
                    }

                    //가비지 컬렉팅
                    _Piece?.Dispose();
                    _Background?.Dispose();
                    _Mask?.Dispose();
                    _BinaryContourMat?.Dispose();

                    _CurrentDelay = 0;
                }
            }
            catch (Exception _Exception)
            {
                _Logger.AddLog(_PieceIdx + " 번 퍼즐 풀기 쓰레드가 강제 중단되었습니다.");
            }
            finally
            {
                _OriginalBackground?.Dispose();
            }
        }

        // =========================================
        // 그림 그리는 쓰레드
        // =========================================

        private void DrawThreadFunction()
        {
            Mat _OriginalBackground = Cv2.ImRead("background.png");

            try
            {
                while (_IsSolving)
                {
                    if (_BoardWindow == null)
                        continue;

                    // 그리드 지워주기
                    Color _Answer1Color = Color.LightGreen;
                    Color _Answer2Color = Color.LightCyan;

                    _BoardWindow.Dispatcher.Invoke(() =>
                    {
                        _BoardWindow.Clear();
                        _BoardWindow.DrawWhereIsPiece(_Answer1, new SolidBrush(Color.LightGreen));
                        _BoardWindow.DrawWhereIsPiece(_Answer2, new SolidBrush(Color.LightCyan));
                        _BoardWindow.Render();
                    });

                    Dispatcher _ControllerDispatcher = _ResultImage.Dispatcher;

                    Mat _ClonedBacgroundMat = _OriginalBackground.Clone();

                    if (_Answer1.Rect.IsAvaiableRect())
                    {
                        Cv2.Rectangle(_ClonedBacgroundMat, _Answer1.Rect.TopLeft.ToRectInt(), _Answer1.Rect.BottomRight.ToRectInt(), new Scalar(_Answer1Color.R, _Answer1Color.G, _Answer1Color.B), 2);
                        Cv2.PutText(_ClonedBacgroundMat, Math.Round(_Answer1.Similarity, 2).ToString(), new OpenCVPoint(_Answer1.Rect.X, _Answer1.Rect.Y + 20), HersheyFonts.HersheyComplexSmall, 1.0, new Scalar(_Answer1Color.R, _Answer1Color.G, _Answer1Color.B), 2);
                    }

                    if (_Answer2.Rect.IsAvaiableRect())
                    {
                        Cv2.Rectangle(_ClonedBacgroundMat, _Answer2.Rect.TopLeft.ToRectInt(), _Answer2.Rect.BottomRight.ToRectInt(), new Scalar(_Answer2Color.R, _Answer2Color.G, _Answer2Color.B), 2);
                        Cv2.PutText(_ClonedBacgroundMat, Math.Round(_Answer2.Similarity, 2).ToString(), new OpenCVPoint(_Answer2.Rect.X, _Answer2.Rect.Y + 20), HersheyFonts.HersheyComplexSmall, 1.0, new Scalar(_Answer2Color.R, _Answer2Color.G, _Answer2Color.B), 2);
                    }

                    _ControllerDispatcher.Invoke(() => _ResultImage.Source = _ClonedBacgroundMat.ConvertToBitmapSource());
                    _ClonedBacgroundMat?.Dispose();

                    Thread.Sleep(100);
                }
            }
            catch (Exception _Exception)
            {
                _Logger.AddLog("퍼즐 그리기 쓰레드가 강제 중단되었습니다.");
            }
            finally
            {
                _OriginalBackground?.Dispose();
            }
        }
    }
}
