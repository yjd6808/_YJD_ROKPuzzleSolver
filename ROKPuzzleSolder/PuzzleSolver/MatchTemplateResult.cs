// ===============================
// @AUTHOR      : 윤정도
// @CREATE DATE : 2021-04-04-일 오전 12:21:56   
// @PURPOSE     : 매칭 템플릿 결과
// ===============================

using OpenCVSize = OpenCvSharp.Size;
using OpenCVRect = OpenCvSharp.Rect;
using OpenCVPoint = OpenCvSharp.Point;

namespace ROKPuzzleSolder.PuzzleSolver
{
    public struct MatchTemplateResult
    {
        public OpenCVRect Rect;
        public double Similarity;

        public MatchTemplateResult(OpenCVRect _Rect, double _Sim)
        {
            Rect = _Rect;
            Similarity = _Sim;
        }
    }
}
