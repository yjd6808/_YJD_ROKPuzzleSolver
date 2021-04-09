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

namespace ROKPuzzleSolder
{
    public partial class WinCapture : Window
    {
        public OpenCvSharp.Rect2d _CapturedRect;

        public WinCapture(OpenCvSharp.Rect2d _Rect)
        {
            InitializeComponent();

            if (_Rect.IsAvaiableRect())
            {
                this.Left = _Rect.X;
                this.Top = _Rect.Y;
                this.Width = _Rect.Width;
                this.Height = _Rect.Height;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.Opacity = 0.3f;
                this.DragMove();
                this.Opacity = 1.0f;
            }
        }

        private void _CSCaptureButton_Click(object sender, RoutedEventArgs e)
        {
            _CapturedRect = new OpenCvSharp.Rect2d(this.Left, this.Top, this.Width, this.Height);

            if (_CapturedRect.IsAvaiableRect() == false)
            {
                MessageBox.Show("유효한 영역이 아닙니다.");
                return;
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}
