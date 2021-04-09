// ===============================
// @AUTHOR      : 윤정도
// @CREATE DATE : 2021-04-04-일 오전 12:21:56   
// @PURPOSE     : 매칭 템플릿 결과
// ===============================

using OpenCVSize = OpenCvSharp.Size;
using OpenCVRect2D = OpenCvSharp.Rect2d;
using OpenCVPoint = OpenCvSharp.Point;

namespace ROKPuzzleSolder.PuzzleSolver
{
    public struct MatchTemplateResult
    {
        public OpenCVRect2D Rect;
        public double Similarity;

        public MatchTemplateResult(OpenCVRect2D _Rect, double _Sim)
        {
            Rect = _Rect;
            Similarity = _Sim;
        }
    }
}
