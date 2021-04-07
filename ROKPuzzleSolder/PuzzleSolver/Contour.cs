// ===============================
// @AUTHOR      : 윤정도
// @CREATE DATE : 2021-04-08-목 오전 12:17:52   
// @PURPOSE     : 윤곽선 정보
// ===============================

using System.Collections.Generic;
using OpenCvSharp;

using OpenCVSize = OpenCvSharp.Size;
using OpenCVRect = OpenCvSharp.Rect;
using OpenCVPoint = OpenCvSharp.Point;

namespace ROKPuzzleSolder.PuzzleSolver
{
    public class Contour
    {
        public OpenCVPoint[] Points;
        public double ArcLength;

        public Contour(OpenCVPoint[] _Points)
        {
            Points = _Points;
            ArcLength = Cv2.ArcLength(_Points, true);
        }

        public Contour(OpenCVPoint[] _Points, double _ArcLen)
        {
            Points = _Points;
            ArcLength = _ArcLen;
        }
    }
}
