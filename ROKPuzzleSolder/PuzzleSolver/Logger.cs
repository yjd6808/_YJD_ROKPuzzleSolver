// ===============================
// @AUTHOR      : 윤정도
// @CREATE DATE : 2021-04-04-일 오전 12:47:39   
// @PURPOSE     : 
// ===============================


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ROKPuzzleSolder.PuzzleSolver
{
    public class Logger
    {
        private ListBox _LogListBox;
        private object _LogLock;

        public Logger(ListBox _ListBox)
        {
            _LogListBox = _ListBox;
            _LogLock = new object();
        }

        public void AddLog(string _LogString)
        {
            if (_LogListBox == null)
            {
                lock (_LogLock)
                {
                    Debug.WriteLine(_LogString);
                }
            }
            else
            {
                _LogListBox.Dispatcher.Invoke(() =>
                {
                    if (_LogListBox.Items.Count > 5)
                        _LogListBox.Items.Clear();

                    _LogListBox.Items.Add(_LogString);
                });
            }
        }
    }
}
