using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ADOFAI_Auto.Core;
using ADOFAI_Auto.Core.Model;
using System.Windows.Threading;
using System.ComponentModel;

namespace ADOFAI_Auto.ViewModel
{
    public class KeyboardStateViewModel : BaseViewModel
    {
        public KeyboardStateViewModel()
        {
            KeyboardLogInfos = new ObservableCollection<KeyboardLogInfo>();
            KeyboardContorller.GetInstence().KeyEvent += KeyboardRecorder_KeyLogEvent;
            KeyboardContorller.GetInstence().PropertyChanged += KeyboardRecorder_PropertyChanged;
        }

        private void KeyboardRecorder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RecordCleared")
                Dispatcher.Invoke(() => KeyboardLogInfos.Clear());
        }

        private void KeyboardRecorder_KeyLogEvent(object sender, KeyboardLogInfo e) => Dispatcher.Invoke(() => KeyboardLogInfos.Insert(0, e));

        public Dispatcher Dispatcher;

        public ObservableCollection<KeyboardLogInfo> KeyboardLogInfos { get; }
    }
}
