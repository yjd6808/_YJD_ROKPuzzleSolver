// ===============================
// @AUTHOR      : 윤정도
// @CREATE DATE : 2021-04-03-토 오후 11:50:24   
// @PURPOSE     : 퍼즐 세팅
// ===============================


using OpenCvSharp;
using Newtonsoft;
using Newtonsoft.Json.Linq;

using OpenCVSize = OpenCvSharp.Size;
using OpenCVRect = OpenCvSharp.Rect;
using OpenCVPoint = OpenCvSharp.Point;
using System.IO;
using System;

namespace ROKPuzzleSolder.PuzzleSolver
{
    public class ROKPuzzleSolverSetting
    {
        private static readonly string SaveFileName = "ROKPuzzleSolderSetting.json";

        public static OpenCVRect BackgroundRect;                   //퍼즐 완성본 검출 영역
        public static OpenCVRect PieceRect;                        //퍼즐 조각 검출 영역

        public static TemplateMatchModes MatchMethod;              //매칭 방식
        public static double PieceStandardBinaryThreshold;         //퍼즐 조각 이진화 기준 임계값
        public static double MatchSimilarityInterval;              //매칭 유사도 간격

        public static double    MinimumMatchValue;                 //검출 시작 최소 수치
        public static bool      FitImageWithContour;               //윤곽선 따라 이미지를 자를지 여부
        public static bool      UsePieceScaling;                   //조각 검출 후 확대 할지 여부
        public static double    PieceScale;                        //조각 확대 수치 ( 1.1 = 110% )
        public static int       SolveDelay;                        //조각 검출 주기 ( 1000 = 1s )

        public static double MaskStandardBinaryThreshold;          //마스킹 이진화 기준 임계값
        public static int RandomPointCountsInContourRect;          //윤곽선 영역내부의 랜덤으로 생성할 점의 개수
        public static double RandomPointsAverageUpperPercent;      //윤곽선 영역내부의 랜덤으로 생성할 점의 픽셀 값의 평균을 몇퍼센트로 잡을지

        //초기 설정
        public static void Initialize()
        {
            BackgroundRect = new OpenCVRect(0, 0, 0, 0);
            PieceRect = new OpenCVRect(0, 0, 0, 0);

            MatchMethod = TemplateMatchModes.CCoeffNormed;
            PieceStandardBinaryThreshold = 200;
            MatchSimilarityInterval = 0.03;

            MinimumMatchValue = 0.2;
            FitImageWithContour = true;
            UsePieceScaling = true;
            PieceScale = 1.36;
            SolveDelay = 240;

            MaskStandardBinaryThreshold = 125;
            RandomPointCountsInContourRect = 500;
            RandomPointsAverageUpperPercent = 20;
        }

        public static bool Save()
        {
            JObject _Root = new JObject();

            try
            {
                _Root.Add("background_rect_x", BackgroundRect.X);
                _Root.Add("background_rect_y", BackgroundRect.Y);
                _Root.Add("background_rect_width", BackgroundRect.Width);
                _Root.Add("background_rect_height", BackgroundRect.Height);

                _Root.Add("piece_rect_x", PieceRect.X);
                _Root.Add("piece_rect_y", PieceRect.Y);
                _Root.Add("piece_rect_width", PieceRect.Width);
                _Root.Add("piece_rect_height", PieceRect.Height);

                _Root.Add("match_method", (int)MatchMethod);
                _Root.Add("piece_standard_binary_threshold", PieceStandardBinaryThreshold);
                _Root.Add("match_similarity_interval", MatchSimilarityInterval);

                _Root.Add("minimum_match_value", MinimumMatchValue);
                _Root.Add("fit_image_with_contour", FitImageWithContour);
                _Root.Add("use_piece_scaling", UsePieceScaling);
                _Root.Add("piece_scale", PieceScale);
                _Root.Add("solve_delay", SolveDelay);

                _Root.Add("mask_standard_binary_threshold", MaskStandardBinaryThreshold);
                _Root.Add("random_points_count_in_contourrect", RandomPointCountsInContourRect);
                _Root.Add("random_points_average_upper_percent", RandomPointsAverageUpperPercent);

                File.WriteAllText(SaveFileName, _Root.ToString());

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

        public static bool Load()
        {
            if (File.Exists(SaveFileName) == false)
                return false;

            try
            {
                JObject _Root = JObject.Parse(File.ReadAllText(SaveFileName));

                BackgroundRect.X = int.Parse(_Root["background_rect_x"].ToString());
                BackgroundRect.Y = int.Parse(_Root["background_rect_y"].ToString());
                BackgroundRect.Width = int.Parse(_Root["background_rect_width"].ToString());
                BackgroundRect.Height = int.Parse(_Root["background_rect_height"].ToString());

                PieceRect.X = int.Parse(_Root["piece_rect_x"].ToString());
                PieceRect.Y = int.Parse(_Root["piece_rect_y"].ToString());
                PieceRect.Width = int.Parse(_Root["piece_rect_width"].ToString());
                PieceRect.Height = int.Parse(_Root["piece_rect_height"].ToString());

                MatchMethod = (TemplateMatchModes)int.Parse(_Root["match_method"].ToString());
                PieceStandardBinaryThreshold = double.Parse(_Root["piece_standard_binary_threshold"].ToString());
                MatchSimilarityInterval = double.Parse(_Root["match_similarity_interval"].ToString());

                MinimumMatchValue = double.Parse(_Root["minimum_match_value"].ToString());
                FitImageWithContour = bool.Parse(_Root["fit_image_with_contour"].ToString());
                UsePieceScaling = bool.Parse(_Root["use_piece_scaling"].ToString());
                PieceScale = double.Parse(_Root["piece_scale"].ToString());
                SolveDelay = int.Parse(_Root["solve_delay"].ToString());

                MaskStandardBinaryThreshold = double.Parse(_Root["mask_standard_binary_threshold"].ToString());
                RandomPointCountsInContourRect = int.Parse(_Root["random_points_count_in_contourrect"].ToString());
                RandomPointsAverageUpperPercent = int.Parse(_Root["random_points_average_upper_percent"].ToString());

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

