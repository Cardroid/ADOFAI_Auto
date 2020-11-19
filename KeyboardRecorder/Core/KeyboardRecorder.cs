using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ADOFAI_Auto.Core.Model;

using NeatInput.Windows;

namespace ADOFAI_Auto.Core
{
    public class KeyboardRecorder : INotifyPropertyChanged
    {
        public KeyboardRecorder()
        {
            KeyboardLogs = new KeyList();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event EventHandler<KeyboardLogInfo> KeyEvent;

        private bool _IsRecording;
        public bool IsRecording
        {
            get => _IsRecording;
            set
            {
                _IsRecording = value;
                OnPropertyChanged("IsRecording");
            }
        }

        public int FirstOffset
        {
            get => KeyboardLogs.InputOffset;
            set => KeyboardLogs.InputOffset = value;
        }
        public int GlobalOffset
        {
            get => KeyboardLogs.GlobalOffset;
            set => KeyboardLogs.GlobalOffset = value;
        }

        private KeyboardEventLogger KeyboardEventLogger;
        private KeyList KeyboardLogs { get; }

        private int LastLogFrame;

        public void StartRecord(KeyboardEventLogger keyboardEventLogger = null)
        {
            if (IsRecording)
                return;

            Release();

            KeyboardLogs.InputOffset = MemoryReader.GetInputOffset();

            if (keyboardEventLogger != null)
            {
                keyboardEventLogger.KeyboardEvent += KeyboardEventLogger_KeyboardEvent;
                KeyboardEventLogger = keyboardEventLogger;
            }
            else if (KeyboardEventLogger != null)
            {
                KeyboardEventLogger.KeyboardEvent += KeyboardEventLogger_KeyboardEvent;
            }

            LastLogFrame = MemoryReader.GetFrame();
            IsRecording = true;
        }

        public void StopRecord()
        {
            Release();
            IsRecording = false;
        }

        private void Release()
        {
            if (KeyboardEventLogger != null)
                KeyboardEventLogger.KeyboardEvent -= KeyboardEventLogger_KeyboardEvent;
        }

        private void KeyboardEventLogger_KeyboardEvent(object sender, NeatInput.Windows.Events.KeyboardEvent e)
        {
            if (IsRecording)
                LogKey(e);
        }

        private void LogKey(NeatInput.Windows.Events.KeyboardEvent e)
        {
            if(e.State == NeatInput.Windows.Processing.Keyboard.Enums.KeyStates.Down)
            {
                int currentFrame = MemoryReader.GetFrame();
                int delay = currentFrame - LastLogFrame;
                KeyboardLogs.Add(new KeyboardLog(delay));
                KeyEvent?.Invoke(null, new KeyboardLogInfo(currentFrame, LastLogFrame, delay, e.Key.ToString(), e.State.ToString()));
                LastLogFrame = currentFrame;
            }
        }

        public void LoadKeyLog(int[] delays, bool isClear = true)
        {
            if (IsRecording)
                return;

            if (isClear)
                RecordClear();

            for (int i = 0; i < delays.Length; i++)
                KeyboardLogs.Add(new KeyboardLog(delays[i]));
        }

        public void RecordClear(bool isOffsetClear = true)
        {
            if (!IsRecording)
            {
                KeyboardLogs.Clear(isOffsetClear);
                OnPropertyChanged("RecordCleared");
            }
        }

        public KeyList GetRecord() => KeyboardLogs;
    }
}
