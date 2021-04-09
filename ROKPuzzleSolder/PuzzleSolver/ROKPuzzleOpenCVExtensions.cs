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
using OpenCVRect2D = OpenCvSharp.Rect2d;
using OpenCVRect = OpenCvSharp.Rect;
using OpenCVPoint = OpenCvSharp.Point;
using OpenCVPoint2D = OpenCvSharp.Point2d;
using System.Windows;

namespace ROKPuzzleSolder.PuzzleSolver
{
    public static class ROKPuzzleOpenCVExtensions
    {
        public static Random _Random = new Random();

        public static BitmapSource ConvertToBitmapSource(this Bitmap _Bitmap)
        {
            IntPtr _BitmapHandle = _Bitmap.GetHbitmap();

            BitmapSource _BitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                _BitmapHandle,
                IntPtr.Zero,
                Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            Win32Helper.Method.DeleteObject(_BitmapHandle); //기존 비트맵 해제

            return _BitmapSource;
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

      
        public static bool IsAvaiableRect(this OpenCVRect2D _Rect)
        {
            return _Rect.Width > 1 && _Rect.Height > 1;
        }

  
        public static OpenCVRect ToRectInt(this OpenCVRect2D _Rect2D)
        {
            return new OpenCVRect(
                (int)Math.Round(_Rect2D.X, 0), 
                (int)Math.Round(_Rect2D.Y, 0), 
                (int)Math.Round(_Rect2D.Width, 0), 
                (int)Math.Round(_Rect2D.Height, 0));
        }

        public static OpenCVPoint ToRectInt(this OpenCVPoint2D _Point2D)
        {
            return new OpenCVPoint(
                (int)Math.Round(_Point2D.X, 0),
                (int)Math.Round(_Point2D.Y, 0));
        }
    }
}
