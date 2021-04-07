// ===============================
// @AUTHOR      : 윤정도
// @CREATE DATE : 2021-04-04-일 오전 12:04:19   
// @PURPOSE     : 퍼즐 풀기 전용 OpenCV 기능
// ===============================


using OpenCvSharp;
using OpenCvSharp.Extensions;
using MoreLinq;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;

using OpenCVSize = OpenCvSharp.Size;
using OpenCVRect = OpenCvSharp.Rect;
using OpenCVPoint = OpenCvSharp.Point;


namespace ROKPuzzleSolder.PuzzleSolver
{
    public enum BlackOrWhite : int
    {
        White,
        Black
    }
    class ROKPuzzleOpenCV
    {
        // =========================================
        // 화면 특정 영역 캡쳐 후 변환
        // =========================================
        public static Mat ScreenToMat(Rect _Area)
        {
            return BitmapConverter.ToMat(ScreenToBitmap(_Area));
        }

        public static BitmapSource ScreenToBitmapSource(Rect _Area)
        {
            if (_Area.Width <= 0.1 || _Area.Height <= 0.1)
                return null;

            Bitmap _Bitmap = ScreenToBitmap(_Area);

            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                _Bitmap.GetHbitmap(),
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        public static Bitmap ScreenToBitmap(Rect _Area)
        {
            Bitmap bitmap = new Bitmap(_Area.Width, _Area.Height);
            Graphics.FromImage(bitmap).CopyFromScreen(_Area.X, _Area.Y, 0, 0, new System.Drawing.Size(_Area.Size.Width, _Area.Size.Height));
            return bitmap;
        }


        // =========================================
        // 사이즈 변경
        // =========================================

        // 속성 의미
        // INTER_NEAREST        이웃 보간법
        // INTER_LINEAR         쌍 선형 보간법
        // INTER_LINEAR_EXACT   비트 쌍 선형 보간법
        // INTER_CUBIC          바이큐빅 보간법
        // INTER_AREA           영역 보간법
        // INTER_LANCZOS4       Lanczos 보간법

        // 일반적으로 쌍 선형 보간법이 가장 많이 사용됩니다.
        // 이미지를 확대하는 경우, 바이큐빅 보간법이나 쌍 선형 보간법을 가장 많이 사용합니다.
        // 이미지를 축소하는 경우, 영역 보간법을 가장 많이 사용합니다.

        public static Mat ResizeRelative(Mat _Src, double _XRatio, double _YRatio)
        {
            InterpolationFlags _Flag = InterpolationFlags.Linear;

            if (_XRatio + _YRatio > 2.0)
                _Flag = InterpolationFlags.Linear;
            else
                _Flag = InterpolationFlags.Area;

            return _Src.Clone().Resize(new OpenCVSize(), _XRatio, _YRatio, _Flag);
        }

        public static Mat ResizeAbsolute(Mat _Src, double _Width, double _Height)
        {
            InterpolationFlags _Flag = InterpolationFlags.Linear;

            if (_Width + _Height > _Src.Width + _Src.Height)
                _Flag = InterpolationFlags.Linear;
            else
                _Flag = InterpolationFlags.Area;

            return _Src.Clone().Resize(new OpenCVSize(_Width, _Height), 0, 0, _Flag);
        }

        public static OpenCVPoint GetRectCenter(OpenCVRect _Rect)
        {
            return new OpenCVPoint(_Rect.X + _Rect.Width / 2, _Rect.Y + _Rect.Height / 2);
        }

        // =========================================
        //  점들의 포함된 최소한의 사각영역 가져오기
        //  @ 인자1 : 점들의 정보가 담긴 리스트
        // =========================================

        public static OpenCVRect GetBoundingBoxPoints(IEnumerable<OpenCVPoint> _Points)
        {
            _Points = _Points.OrderBy((a) => a.X);

            int _X = _Points.First().X;
            int _Width = _Points.Last().X - _X;

            _Points = _Points.OrderBy((a) => a.Y);

            int _Y = _Points.First().Y;
            int _Height = _Points.Last().Y - _Y;

            return new OpenCVRect(_X, _Y, _Width, _Height);
        }

        public static OpenCVRect GetBoundingBoxRects(IEnumerable<OpenCVRect> _Rects)
        {
            _Rects = _Rects.OrderBy((a) => a.X);

            int _X = _Rects.First().X;
            int _Width = _Rects.Last().X + _Rects.Last().Width - _X;

            _Rects = _Rects.OrderBy((a) => a.Y);

            int _Y = _Rects.First().Y;
            int _Height = _Rects.Last().Y + _Rects.Last().Height - _Y;

            return new OpenCVRect(_X, _Y, _Width, _Height);
        }

        // =========================================
        //  윤곽선들 중 길이가 가장 긴 윤곽선의 사각 영역 정보 가져오기
        //  @ 인자1 : 윤곽선을 이루는 점들의 정보가 담긴 리스트
        // =========================================

        //  @ 원리
        //  1. 가장 형태가 뚜렷한 윤곽선을 가져옵니다.(가장 점이 많은 윤곽선)
        //  2. 윤곽선의 X 점들을 오름차순으로 정렬하여 좌측 상단의 X좌표와 사각 영역의 너비를 계산한다.
        //  3. 윤곽선의 Y 점들을 오름차순으로 정렬하여 좌측 상단의 Y좌표와 사각 영역의 높이를 계산한다.
        private static OpenCVRect GetContourRectLogestOne(IEnumerable<Contour> _ContoursList)
        {
            return GetBoundingBoxPoints(_ContoursList.OrderByDescending(x => x.ArcLength).First().Points);
        }


        // =========================================
        //  윤곽선들 중 길이가 긴 순으로 여러개의 사각형 정보 가져오기
        //  @ 인자1 : 윤곽선을 이루는 점들의 정보가 담긴 리스트
        //  @ 인자2 : 몇개의 사각형 정보를 가져올 지
        // =========================================
        private static List<OpenCVRect> GetContourRects(IEnumerable<Contour> _ContoursList, int _ReturnCount)
        {
            int _Count = _ContoursList.Count();

            if (_ReturnCount > _Count)
                _ReturnCount = _Count;

            List<OpenCVRect> _ContourRects = new List<OpenCVRect>();

            _ContoursList.OrderByDescending(x => x.ArcLength).ForEach(x => _ContourRects.Add(GetBoundingBoxPoints(x.Points)));

            //쓸일이 있으면 만들자.
            return _ContourRects;
        }

        // =========================================
        //  인자로 받은 이미지 데이터(Mat)의 윤곽선을 추출하여 사각 영역(Rect)을 가져옴
        //  @ 인자1 : 윤곽선을 추출해낼 원본 이미지
        //  @ 인자2 : 윤곽선의 영역
        //  @ 인자3 : 윤곽선의 영역만큼 원본 이미지에서 잘라진 이진화 처리된 이미지
        // =========================================

        public static bool FitToContourRect(Mat _Src, double _StandardBinaryThreshold, out OpenCVRect _Rect, out Mat _BinaryContourMat)
        {
            _Rect = new OpenCVRect(0, 0, 0, 0);
            _BinaryContourMat = null;

            try
            {
                Mat _ClonedSource = _Src.Clone();

                OpenCVPoint[][] _Contours;
                HierarchyIndex[] _Hierarchy;

                Cv2.CvtColor(_ClonedSource, _ClonedSource, ColorConversionCodes.RGB2GRAY);      //그레이 스케일링
                Cv2.Threshold(_ClonedSource, _ClonedSource, _StandardBinaryThreshold, 255, ThresholdTypes.Binary);   //이진화
                Cv2.FindContours(_ClonedSource, out _Contours, out _Hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxTC89KCOS);

                //  @ 함수 설명
                //  Cv2.FindContours(원본 배열, 검출된 윤곽선, 계층 구조, 검색 방법, 근사 방법, 오프셋)로 윤곽선 검출을 적용합니다.
                //  1. 검출된 윤곽선은 out 키워드를 활용해 함수에서 검출된 윤곽선을 저장합니다.
                //  2. 계층 구조는 out 키워드를 활용해 함수에서 검출된 계층구조를 저장합니다.
                //  3. 검색 방법은 윤곽선을 검출해 어떤 계층 구조의 형태를 사용할지 설정합니다.
                //  4. 근사 방법은 윤곽점의 근사법을 설정합니다.근사 방법에 따라 검출된 윤곽선(contours)에 포함될 좌표의 수나 정교함의 수준이 달라집니다.
                //  5. 오프셋은 반환된 윤곽점들의 좌푯값에 이동할 값을 설정합니다.일반적으로 잘 사용하지 않습니다.

                // 내가 정한 윤곽선 판단 기준
                //  1. 길이 100 이상
                //  2. 구성 점 20개 이상

                int _StandardPointsOfContourCount = 20;
                double _MinimumLengthOfContour = 100;

                List<Contour> _ContoursList = new List<Contour>();

                for (int i =0; i < _Contours.Length; i++)
                {
                    if (_Contours[i].Length >= _StandardPointsOfContourCount)
                    {
                        double _LengthOfContour = Cv2.ArcLength(_Contours[i], true);

                        if (_LengthOfContour >= _MinimumLengthOfContour)
                        {
                            _ContoursList.Add(new Contour(_Contours[i], _LengthOfContour));
                        }
                    }
                }

                if (_ContoursList.Count == 0)
                    return false;

                // @ 코드 설명
                // 반복문을 활용해 검출된 윤곽선(_Contours)의 값을 검사합니다.
                // 윤곽선 길이 함수(Cv2.ArcLength)를 활용해 길이가 100 이상의 값만 _ContoursList 추가합니다.

                //Cv2.DrawContours(dst, new_contours, -1, new Scalar(255, 0, 0), 2, LineTypes.AntiAlias, null, 1); //윤곽선 그리기

                _Rect = GetContourRectLogestOne(_ContoursList); //GetBoundingBoxRects(GetContourRects(_ContoursList, 3)); //윤곽선의 영역에 맞게 크기 조절
                _BinaryContourMat = _ClonedSource.SubMat(_Rect);
                return true;
            }
            catch (Exception _Exception)
            {
#if DEBUG
                throw _Exception;
#else
                return false;
#endif
            }
        }

        // =========================================
        //  흑 백으로만 구성된 마스크 이미지 내의 흑 또는 백에 해당하는 영역에만 점을 생성
        //  @ 인자1 : 흑 백으로만 구성된 마스크 이미지
        //  @ 인자2 : 인정하는 색 - 흑 또는 백 (1 = 백 / 그 외의 수 = 흑)
        //  @ 인자3 : 랜덤으로 가져올 점의 갯수
        //  @ 인자4 : 연속으로 유효한 색의 영역에 점이 생성되지 않는 최대 횟수
        // =========================================



        public static bool GetRandomPointsInBinaryImage(Mat _BlackAndWhiteBinaryMask, BlackOrWhite _BlackOrWhite, int _PointCount, int _ContinuousFailCount, out List<OpenCVPoint> _RandomPoints)
        {
            Random _RandomGenerator = new Random();
            _RandomPoints = new List<OpenCVPoint>();

            if (_BlackAndWhiteBinaryMask == null)
                return false;

            //그레이 스케일링된 이미지여야만 한다.
            if (_BlackAndWhiteBinaryMask.Channels() != 1)
                return false;

            const int _Black = 0;
            const int _White = 255;

            int _True = _BlackOrWhite == BlackOrWhite.White ? _White : _Black;
            int _CurrentFailCount = 0;  //연속적으로 내부에 안들어있을 경우

            while (true)
            {
                int _Column = _RandomGenerator.Next(0, _BlackAndWhiteBinaryMask.Cols);
                int _Row = _RandomGenerator.Next(0, _BlackAndWhiteBinaryMask.Rows);

                byte _BinaryPixelColor = _BlackAndWhiteBinaryMask.At<byte>(_Row, _Column);

                if (_BinaryPixelColor == _True)
                {
                    //내부
                    _RandomPoints.Add(new OpenCVPoint(_Column, _Row));
                    _CurrentFailCount = 0;
                }
                else
                {
                    _CurrentFailCount++;
                }

                if (_RandomPoints.Count >= _PointCount)
                    break;

                if (_CurrentFailCount >= _ContinuousFailCount)
                    return false;
            }


            return true;
        }

        // =========================================
        //  이미지 검색
        //  @ 인자1 : 배경 이미지
        //  @ 인자2 : 조각 이미지
        //  @ 인자3 : 마스크 (조각 이미지에서 이 이미지의 검은 영역을 제외한 영역을 비교하도록 함)
        //  @ 인자4 : 매칭 방식
        //  @ 인자5 : 몇 이상의 유사도 부터 검출 시작할지
        //  @ 인자6 : 검출 간격
        //  @ 인자7 : 매칭 결과값이 담길 리스트
        // =========================================

        public static bool Contains(Mat _Src, Mat _Partition, Mat _Mask, TemplateMatchModes _MatchMethod, double _StartSimilarity, double _SimilarityInterval, out List<MatchTemplateResult> _FindRects)
        {
            _FindRects = new List<MatchTemplateResult>();

            try
            {
                if (_MatchMethod == TemplateMatchModes.SqDiff || _MatchMethod == TemplateMatchModes.SqDiffNormed)
                    _StartSimilarity = 1 - _StartSimilarity;

                // 결과 이미지 행렬 구성
                int _ResultCols = _Src.Cols - _Partition.Cols + 1;
                int _ResultRows = _Src.Rows - _Partition.Rows + 1;

                Mat _Result = new Mat(_ResultCols, _ResultRows, MatType.CV_32FC1);

                //   포함된 이미지 검출
                _Result = _Src.MatchTemplate(_Partition, _MatchMethod, _Mask);
                _Result.MinMaxLoc(
                        out double _MinValue,
                        out double _MaxValue,
                        out OpenCVPoint _MinLocation,
                        out OpenCVPoint _MaxLocation);

                for (int i = 0; i < _Result.Rows; i++)
                {
                    for (int j = 0; j < _Result.Cols; j++)
                    {
                        double _Similarity = _Result.At<float>(i, j);

                        if (_MatchMethod == TemplateMatchModes.SqDiff || _MatchMethod == TemplateMatchModes.SqDiffNormed)
                        {
                            if (_Similarity < 0)
                                continue;

                            if (_Similarity < _StartSimilarity)
                            {
                                _FindRects.Add(new MatchTemplateResult(new OpenCVRect(j, i, _Partition.Width, _Partition.Height), _Similarity));
                                _StartSimilarity -= _SimilarityInterval;
                            }
                        }
                        else
                        {
                            if (_Similarity > 1)
                                continue;

                            if (_Similarity > _StartSimilarity)
                            {
                                _FindRects.Add(new MatchTemplateResult(new OpenCVRect(j, i, _Partition.Width, _Partition.Height), _Similarity));
                                _StartSimilarity += _SimilarityInterval;
                            }
                        }
                    }
                }

                if (_FindRects.Count == 0)
                    return false;

                return true;
            }
            catch (Exception _Exception)
            {
#if DEBUG
                throw _Exception;
#else
                return false;
#endif
            }
        }
    }
}