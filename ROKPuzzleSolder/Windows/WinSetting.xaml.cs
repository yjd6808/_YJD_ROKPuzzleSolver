using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using ROKPuzzleSolder.PuzzleSolver;

using OpenCVSize = OpenCvSharp.Size;
using OpenCVRect2D = OpenCvSharp.Rect2d;
using OpenCVPoint = OpenCvSharp.Point;
using OpenCvSharp;
using System.IO;

namespace ROKPuzzleSolder
{
    /// <summary>
    /// WinSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WinSetting : System.Windows.Window
    {
        private static readonly string RectXLabelString = "영역 X 좌표 : ";
        private static readonly string RectYLabelString = "영역 Y 좌표 : ";
        private static readonly string RectWidthLabelString = "영역 너비 : ";
        private static readonly string RectHeightLabelString = "영역 높이 : ";

        private static readonly string Piece1RectXLabelString = "1 영역 X 좌표 : ";
        private static readonly string Piece1RectYLabelString = "1 영역 Y 좌표 : ";
        private static readonly string Piece1RectWidthLabelString = "1 영역 너비 : ";
        private static readonly string Piece1RectHeightLabelString = "1 영역 높이 : ";

        private static readonly string Piece2RectXLabelString = "2 영역 X 좌표 : ";
        private static readonly string Piece2RectYLabelString = "2 영역 Y 좌표 : ";
        private static readonly string Piece2RectWidthLabelString = "2 영역 너비 : ";
        private static readonly string Piece2RectHeightLabelString = "2 영역 높이 : ";



        private OpenCVRect2D _BackgroundRect;
        private OpenCVRect2D _Piece1Rect;
        private OpenCVRect2D _Piece2Rect;

        private Mat _BackgroundMat;
        private Mat _Piece1Mat;
        private Mat _Piece2Mat;

        public WinSetting()
        {
            InitializeComponent();

            SetVisibleStatusBackgroundLabels(false);
            SetVisibleStatusPiece1Labels(false);
            SetVisibleStatusPiece2Labels(false);

            if (ROKPuzzleSolverSetting.BackgroundRect.IsAvaiableRect())
            {
                SetVisibleStatusBackgroundLabels(true);

                _WSBackgroundRectXLabel.Content = RectXLabelString + ROKPuzzleSolverSetting.BackgroundRect.X;
                _WSBackgroundRectYLabel.Content = RectYLabelString + ROKPuzzleSolverSetting.BackgroundRect.Y;
                _WSBackgroundRectWithLabel.Content = RectWidthLabelString + ROKPuzzleSolverSetting.BackgroundRect.Width;
                _WSBackgroundRectHeightLabel.Content = RectHeightLabelString + ROKPuzzleSolverSetting.BackgroundRect.Height;

                _BackgroundRect = ROKPuzzleSolverSetting.BackgroundRect;
            }

            if (ROKPuzzleSolverSetting.Piece1Rect.IsAvaiableRect())
            {
                SetVisibleStatusPiece1Labels(true);

                _WSPiece1RectXLabel.Content = Piece1RectXLabelString + ROKPuzzleSolverSetting.Piece1Rect.X;
                _WSPiece1RectYLabel.Content = Piece1RectYLabelString + ROKPuzzleSolverSetting.Piece1Rect.Y;
                _WSPiece1RectWithLabel.Content = Piece1RectWidthLabelString + ROKPuzzleSolverSetting.Piece1Rect.Width;
                _WSPiece1RectHeightLabel.Content = Piece1RectHeightLabelString + ROKPuzzleSolverSetting.Piece1Rect.Height;

                _Piece1Rect = ROKPuzzleSolverSetting.Piece1Rect;
            }

            if (ROKPuzzleSolverSetting.Piece2Rect.IsAvaiableRect())
            {
                SetVisibleStatusPiece2Labels(true);

                _WSPiece2RectXLabel.Content = Piece2RectXLabelString + ROKPuzzleSolverSetting.Piece2Rect.X;
                _WSPiece2RectYLabel.Content = Piece2RectYLabelString + ROKPuzzleSolverSetting.Piece2Rect.Y;
                _WSPiece2RectWithLabel.Content = Piece2RectWidthLabelString + ROKPuzzleSolverSetting.Piece2Rect.Width;
                _WSPiece2RectHeightLabel.Content = Piece2RectHeightLabelString + ROKPuzzleSolverSetting.Piece2Rect.Height;

                _Piece2Rect = ROKPuzzleSolverSetting.Piece2Rect;
            }

            _WSMatchingMethodsComboBox.SelectedIndex = (int)ROKPuzzleSolverSetting.MatchMethod;
            _WSPieceStandardBinaryThresholdTextBox.Text = ROKPuzzleSolverSetting.PieceStandardBinaryThreshold.ToString();
            _WSMatchSimilarityIntervalTextBox.Text = ROKPuzzleSolverSetting.MatchSimilarityInterval.ToString();
            _WSMinimumMatchValueTextBox.Text = ROKPuzzleSolverSetting.MinimumMatchValue.ToString();
            _WSFitImageWithContourCheckBox.IsChecked = ROKPuzzleSolverSetting.FitImageWithContour;
            _WSUsePieceScalingCheckBox.IsChecked = ROKPuzzleSolverSetting.UsePieceScaling;
            _WSPieceScaleTextBox.Text = ROKPuzzleSolverSetting.PieceScale.ToString();
            _WSSolveDelayTextBox.Text = ROKPuzzleSolverSetting.SolveDelay.ToString();

            _WSMaskStandardBinaryThresholdTextBox.Text = ROKPuzzleSolverSetting.MaskStandardBinaryThreshold.ToString();
            _WSRandomPointsinContourRectCountTextBox.Text = ROKPuzzleSolverSetting.RandomPointCountsInContourRect.ToString();
            _WSRandomPointsAverageUpperPercentTextBox.Text = ROKPuzzleSolverSetting.RandomPointsAverageUpperPercent.ToString();

        }

        #region 이벤트
    

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.Opacity = 0.3f;
                this.DragMove();
                this.Opacity = 1.0f;
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void TextBoxt_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Text = string.Empty;
        }

        private void _WSSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_BackgroundRect.IsAvaiableRect() == false)
            {
                MessageBox.Show("배경 영역이 설정되지 않았습니다.");
                return;
            }

            if (_Piece1Rect.IsAvaiableRect() == false)
            {
                MessageBox.Show("조각 1 영역이 설정되지 않았습니다.");
                return;
            }

            if (_Piece2Rect.IsAvaiableRect() == false)
            {
                MessageBox.Show("조각 2 영역이 설정되지 않았습니다.");
                return;
            }

            if (_WSMatchingMethodsComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("매칭 방식이 선택되지 않았습니다");
                return;
            }

            double _PieceStandardBinaryThreshold = 0.0;
            double _MaskStandardBinaryThreshold = 0.0;
            double _MatchSimilarityInterval = 0.0;
            double _MinimumMatchValue = 0.0;
            double _PieceScale = 0.0;
            int _SolveDelay = 0;

            int _RandomPointCountsInContourRect = 0;
            double _RandomPointsAverageUpperPercent = 0.0;

            //퍼즐 조각 이진화 기준 임계값
            if (double.TryParse(_WSPieceStandardBinaryThresholdTextBox.Text, out _PieceStandardBinaryThreshold))
            {
                if (_PieceStandardBinaryThreshold < 0)
                {
                    MessageBox.Show("이진화 기준 임계값은 0보다 크거나 같아야합니다.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("이진화 기준 임계값 텍스트를 숫자로 바꾸는데 실패했습니다.");
                return;
            }

            //매칭 유사도 간격
            if (double.TryParse(_WSMatchSimilarityIntervalTextBox.Text, out _MatchSimilarityInterval))
            {
                if (_MatchSimilarityInterval < 0.01 || _MatchSimilarityInterval > 1)
                {
                    MessageBox.Show("매칭 유사도 간격은 0.01 ~ 1 사이의 값이어야 합니다.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("매칭 유사도 간격 텍스트를 숫자로 바꾸는데 실패했습니다.");
                return;
            }

            //이미지 검출 최소 유사도
            if (double.TryParse(_WSMinimumMatchValueTextBox.Text, out _MinimumMatchValue))
            {
                if (_MinimumMatchValue < 0 || _MinimumMatchValue > 1)
                {
                    MessageBox.Show("이미지 검출 최소 유사도는 0 ~ 1 사이값이여야 합니다.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("이미지 검출 최소 유사도 텍스트를 숫자로 바꾸는데 실패했습니다.");
                return;
            }

            //검출시 퍼즐조각 확대 수치
            if (double.TryParse(_WSPieceScaleTextBox.Text, out _PieceScale))
            {
                if (_PieceScale < 0 || _PieceScale > 1000)
                {
                    MessageBox.Show("검출시 퍼즐조각 확대 수치는 0 ~ 1000 사이여야 합니다.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("검출시 퍼즐조각 확대 수치 텍스트를 숫자로 바꾸는데 실패했습니다.");
                return;
            }

            //검출 주기
            if (int.TryParse(_WSSolveDelayTextBox.Text, out _SolveDelay))
            {
                if (_SolveDelay < 50)
                {
                    MessageBox.Show("검출 주기는 50 이상이어야 합니다.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("검출 주기 텍스트를 숫자로 바꾸는데 실패했습니다.");
                return;
            }

            //마스크 이진화 기준 임계값
            if (double.TryParse(_WSMaskStandardBinaryThresholdTextBox.Text, out _MaskStandardBinaryThreshold))
            {
                if (_MaskStandardBinaryThreshold < 0)
                {
                    MessageBox.Show("이진화 기준 임계값은 0보다 크거나 같아야합니다.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("이진화 기준 임계값 텍스트를 숫자로 바꾸는데 실패했습니다.");
                return;
            }

            //마스크 이진화 기준 임계값
            if (double.TryParse(_WSMaskStandardBinaryThresholdTextBox.Text, out _MaskStandardBinaryThreshold))
            {
                if (_MaskStandardBinaryThreshold < 0)
                {
                    MessageBox.Show("이진화 기준 임계값은 0보다 크거나 같아야합니다.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("이진화 기준 임계값 텍스트를 숫자로 바꾸는데 실패했습니다.");
                return;
            }

            //윤곽선 영역내부의 랜덤으로 생성할 점의 개수는 100이상 이어야합니다.
            if (int.TryParse(_WSRandomPointsinContourRectCountTextBox.Text, out _RandomPointCountsInContourRect))
            {
                if (_RandomPointCountsInContourRect < 100)
                {
                    MessageBox.Show("윤곽선 영역내부의 랜덤으로 생성할 점의 개수는 100이상 이어야합니다.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("이진화 기준 임계값 텍스트를 숫자로 바꾸는데 실패했습니다.");
                return;
            }

            //윤곽선 영역내부의 랜덤으로 생성할 점의 픽셀값의 상의 몇퍼센트의 점들을 평균을 낼건지
            if (double.TryParse(_WSRandomPointsAverageUpperPercentTextBox.Text, out _RandomPointsAverageUpperPercent))
            {
                if (_RandomPointsAverageUpperPercent < 1 || _RandomPointsAverageUpperPercent > 100)
                {
                    MessageBox.Show("윤곽선 영역내부의 랜덤으로 생성할 점의 픽셀값의 상의 몇퍼센트의 점들을 평균을 낼건지는 1이상 100이하 여야합니다.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("이진화 기준 임계값 텍스트를 숫자로 바꾸는데 실패했습니다.");
                return;
            }

            ROKPuzzleSolverSetting.BackgroundRect = _BackgroundRect;
            ROKPuzzleSolverSetting.Piece1Rect = _Piece1Rect;
            ROKPuzzleSolverSetting.Piece2Rect = _Piece2Rect;

            ROKPuzzleSolverSetting.MatchMethod = (TemplateMatchModes)_WSMatchingMethodsComboBox.SelectedIndex;
            ROKPuzzleSolverSetting.PieceStandardBinaryThreshold = _PieceStandardBinaryThreshold;
            ROKPuzzleSolverSetting.MatchSimilarityInterval = _MatchSimilarityInterval;

            ROKPuzzleSolverSetting.MinimumMatchValue = _MinimumMatchValue;
            ROKPuzzleSolverSetting.FitImageWithContour = _WSFitImageWithContourCheckBox.IsChecked.Value;
            ROKPuzzleSolverSetting.UsePieceScaling = _WSUsePieceScalingCheckBox.IsChecked.Value;
            ROKPuzzleSolverSetting.PieceScale = _PieceScale;
            ROKPuzzleSolverSetting.SolveDelay = _SolveDelay;

            ROKPuzzleSolverSetting.MaskStandardBinaryThreshold = _MaskStandardBinaryThreshold;
            ROKPuzzleSolverSetting.RandomPointCountsInContourRect = _RandomPointCountsInContourRect;
            ROKPuzzleSolverSetting.RandomPointsAverageUpperPercent = _RandomPointsAverageUpperPercent;

            this.DialogResult = true;
            this.Close();
        }

        private void _WSCancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        #endregion


        private void SetVisibleStatusBackgroundLabels(bool _IsVisible)
        {
            _WSBackgroundRectXLabel.Visibility = (Visibility)Convert.ToInt32(!_IsVisible);
            _WSBackgroundRectYLabel.Visibility = (Visibility)Convert.ToInt32(!_IsVisible);
            _WSBackgroundRectWithLabel.Visibility = (Visibility)Convert.ToInt32(!_IsVisible);
            _WSBackgroundRectHeightLabel.Visibility = (Visibility)Convert.ToInt32(!_IsVisible);
        }

        private void SetVisibleStatusPiece1Labels(bool _IsVisible)
        {
            _WSPiece1RectXLabel.Visibility = (Visibility)Convert.ToInt32(!_IsVisible);
            _WSPiece1RectYLabel.Visibility = (Visibility)Convert.ToInt32(!_IsVisible);
            _WSPiece1RectWithLabel.Visibility = (Visibility)Convert.ToInt32(!_IsVisible);
            _WSPiece1RectHeightLabel.Visibility = (Visibility)Convert.ToInt32(!_IsVisible);
        }

        private void SetVisibleStatusPiece2Labels(bool _IsVisible)
        {
            _WSPiece2RectXLabel.Visibility = (Visibility)Convert.ToInt32(!_IsVisible);
            _WSPiece2RectYLabel.Visibility = (Visibility)Convert.ToInt32(!_IsVisible);
            _WSPiece2RectWithLabel.Visibility = (Visibility)Convert.ToInt32(!_IsVisible);
            _WSPiece2RectHeightLabel.Visibility = (Visibility)Convert.ToInt32(!_IsVisible);
        }


        private void _WSSetBackgroundRectButton_Click(object sender, RoutedEventArgs e)
        {
            WinCapture _CaptureWindow = new WinCapture(_BackgroundRect);

            if (_CaptureWindow.ShowDialog().Value)
            {
                _BackgroundRect = _CaptureWindow._CapturedRect;

                if (_BackgroundRect.IsAvaiableRect())
                {
                    SetVisibleStatusBackgroundLabels(true);

                    _WSBackgroundRectXLabel.Content = RectXLabelString + _BackgroundRect.X;
                    _WSBackgroundRectYLabel.Content = RectYLabelString + _BackgroundRect.Y;
                    _WSBackgroundRectWithLabel.Content = RectWidthLabelString + _BackgroundRect.Width;
                    _WSBackgroundRectHeightLabel.Content = RectHeightLabelString + _BackgroundRect.Height;
                }
            }
        }

        private void _WSSettPieceRectButton_Click(object sender, RoutedEventArgs e)
        {
            WinCapture _CaptureWindow = new WinCapture(_Piece1Rect);

            if (_CaptureWindow.ShowDialog().Value)
            {
                _Piece1Rect = _CaptureWindow._CapturedRect;

                if (_Piece1Rect.IsAvaiableRect())
                {
                    SetVisibleStatusPiece1Labels(true);

                    _WSPiece1RectXLabel.Content = Piece1RectXLabelString + _Piece1Rect.X;
                    _WSPiece1RectYLabel.Content = Piece1RectYLabelString + _Piece1Rect.Y;
                    _WSPiece1RectWithLabel.Content = Piece1RectWidthLabelString + _Piece1Rect.Width;
                    _WSPiece1RectHeightLabel.Content = Piece1RectHeightLabelString + _Piece1Rect.Height;
                }
            }
        }

        private void _WSCaptureBackgroundImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_BackgroundRect.IsAvaiableRect() == false)
            {
                MessageBox.Show("배경 캡쳐 영역을 우선 지정해주세요!");
                return;
            }

            _BackgroundMat = ROKPuzzleOpenCV.ScreenToMat(_BackgroundRect.ToRectInt());
            _WSBackgroundImage.Source = _BackgroundMat.ConvertToBitmapSource();
        }

        private void _WSCapturePieceRectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_Piece1Rect.IsAvaiableRect() == false)
            {
                MessageBox.Show("1 조각 캡쳐 영역을 우선 지정해주세요!");
                return;
            }

            _Piece1Mat = ROKPuzzleOpenCV.ScreenToMat(_Piece1Rect.ToRectInt());
            _WSPiece1Image.Source = _Piece1Mat.ConvertToBitmapSource();
        }

        private void _WSShowBackgroundImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_BackgroundMat == null)
            {
                MessageBox.Show("배경을 우선 캡쳐해주세요!");
                return;
            }

            Cv2.ImShow("Background", _BackgroundMat);
            Cv2.WaitKey(0);
        }

        private void _WSShowPieceImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_Piece1Mat == null)
            {
                MessageBox.Show("1 조각을 우선 캡쳐해주세요!");
                return;
            }


            Cv2.ImShow("Piece 1", _Piece1Mat);

            if (_Piece2Mat != null)
                Cv2.ImShow("Piece 2", _Piece2Mat);

            Cv2.WaitKey(0);
        }

        private void _WSSaveBackgroundImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_BackgroundMat == null)
            {
                MessageBox.Show("배경을 우선 캡쳐해주세요!");
                return;
            }

            if (Directory.Exists("Backgrounds") == false)
                Directory.CreateDirectory("Backgrounds");

            Cv2.ImWrite("Backgrounds/" + DateTime.Now.ToString("MM-dd-yyyy h-mm tt") + ".png", _BackgroundMat);

            if (MessageBox.Show("퍼즐 배경 이미지를 교체하시겠습니까?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Cv2.ImWrite("background.png", _BackgroundMat);

            MessageBox.Show("저장을 완료했습니다.");
        }

        private void _WSSettPiece2RectButton_Click(object sender, RoutedEventArgs e)
        {
            WinCapture _CaptureWindow = new WinCapture(_Piece2Rect);

            if (_CaptureWindow.ShowDialog().Value)
            {
                _Piece2Rect = _CaptureWindow._CapturedRect;

                if (_Piece2Rect.IsAvaiableRect())
                {
                    SetVisibleStatusPiece2Labels(true);

                    _WSPiece2RectXLabel.Content = Piece2RectXLabelString + _Piece2Rect.X;
                    _WSPiece2RectYLabel.Content = Piece2RectYLabelString + _Piece2Rect.Y;
                    _WSPiece2RectWithLabel.Content = Piece2RectWidthLabelString + _Piece2Rect.Width;
                    _WSPiece2RectHeightLabel.Content = Piece2RectHeightLabelString + _Piece2Rect.Height;
                }
            }
        }

        private void _WSCapturePiece2RectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_Piece2Rect.IsAvaiableRect() == false)
            {
                MessageBox.Show("2 조각 캡쳐 영역을 우선 지정해주세요!");
                return;
            }

            _Piece2Mat = ROKPuzzleOpenCV.ScreenToMat(_Piece2Rect.ToRectInt());
            _WSPiece2Image.Source = _Piece2Mat.ConvertToBitmapSource();
        }
    }
}
