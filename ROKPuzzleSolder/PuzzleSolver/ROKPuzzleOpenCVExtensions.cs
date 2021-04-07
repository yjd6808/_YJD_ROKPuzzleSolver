// ===============================
// @AUTHOR      : 윤정도
// @CREATE DATE : 2021-04-04-일 오전 12:04:54   
// @PURPOSE     : Mat, Bitmap, BitmapSource간 변환 확장 메소드
// ===============================


using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using OpenCVSize = OpenCvSharp.Size;
using OpenCVRect = OpenCvSharp.Rect;
using OpenCVPoint = OpenCvSharp.Point;

namespace ROKPuzzleSolder.PuzzleSolver
{
    public static class ROKPuzzleOpenCVExtensions
    {
        public static Random _Random = new Random();

        public static BitmapSource ConvertToBitmapSource(this Bitmap _Bitmap)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                _Bitmap.GetHbitmap(),
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        public static Mat ConvertToMat(this Bitmap _Bitmap)
        {
            return BitmapConverter.ToMat(_Bitmap);
        }

        public static Bitmap ConvertToBitmap(this Mat _Src)
        {
            return BitmapConverter.ToBitmap(_Src);
        }

        public static BitmapSource ConvertToBitmapSource(this Mat _Src)
        {
            return ConvertToBitmapSource(BitmapConverter.ToBitmap(_Src));
        }

        public static OpenCVPoint GetRandomPointInRect(this OpenCVRect _Rect)
        {
            return new OpenCVPoint(
                _Rect.X + _Random.Next(0, _Rect.Width), 
                _Rect.Y + _Random.Next(0, _Rect.Height));
        }

        public static bool IsAvaiableRect(this OpenCVRect _Rect)
        {
            return _Rect.Width > 1 && _Rect.Height > 1;
        }
    }
}
