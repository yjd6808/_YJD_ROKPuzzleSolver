// ===============================
// @AUTHOR      : 윤정도
// @CREATE DATE : 2021-04-06-화 오후 8:28:49   
// @PURPOSE     : WINAPI 구조체 정리
// ===============================


using System.Runtime.InteropServices;

namespace ROKPuzzleSolder
{
    public static partial class Win32Helper
    {
        public static class Struct
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct BlendFunction
            {
                public byte BlendOp;
                public byte BlendFlags;
                public byte SourceConstantAlpha;
                public byte AlphaFormat;

                public BlendFunction(byte op, byte flags, byte alpha, byte format)
                {
                    BlendOp = op;
                    BlendFlags = flags;
                    SourceConstantAlpha = alpha;
                    AlphaFormat = format;
                }
            }





        }
    }
}
