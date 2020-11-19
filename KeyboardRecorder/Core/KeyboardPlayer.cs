using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ADOFAI_Auto.Core.Model;

namespace ADOFAI_Auto.Core
{
    public class KeyboardPlayer : INotifyPropertyChanged
    {
        public event EventHandler<KeyboardLogInfo> KeyEvent;
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool IsSuspendPlay;

        private bool _IsPlaying;
        public bool IsPlaying
        {
            get => _IsPlaying;
            private set
            {
                _IsPlaying = value;
                OnPropertyChanged("IsPlaying");
            }
        }

        private int ErrorFrame = 0;
        private int OriginalFrame = 0;

        private int BeforeInputOffset = 0;
        private int GlobalOffset = 0;

        private Thread RunThread;

        public void Play(KeyList keyboardLogs)
        {
            if (RunThread != null)
                return;

            RunThread = new Thread(() =>
            {
                if (keyboardLogs == null || IsPlaying)
                    return;

                IsPlaying = true;
                ErrorFrame = 0;

                BeforeInputOffset = MemoryReader.GetInputOffset();
                MemoryReader.SetInputOffset(keyboardLogs.InputOffset);
                GlobalOffset = keyboardLogs.GlobalOffset;

                OriginalFrame = MemoryReader.GetFrame();
                for (int i = 0; i < keyboardLogs.Count; i++)
                {
                    if (IsSuspendPlay)
                        break;
                    KeyLogHandle(keyboardLogs[i], GlobalOffset);
                }
                IsSuspendPlay = false;
                IsPlaying = false;

                RunThread = null;
            });

            RunThread.Start();
        }

        public void Stop()
        {
            IsSuspendPlay = true;
            MemoryReader.SetInputOffset(BeforeInputOffset);
        }

        private void KeyLogHandle(KeyboardLog keyboardLog, int offset = 0)
        {
            int delay = keyboardLog.FrameDelay + offset;
            FrameWait(delay);
            KeyboardInput.PressKey();
            KeyEvent?.Invoke(null, new KeyboardLogInfo(MemoryReader.GetFrame(), -1, ErrorFrame, delay, "Auto", "Down"));
        }

        private int FrameReadDelay = 1;
        private bool IsFixErrorFrame = true;

        private void FrameWait(int frame)
        {
            int newFrame;
            int CalculateDelay;

            if (IsFixErrorFrame)
                frame -= ErrorFrame;

            do
            {
                newFrame = MemoryReader.GetFrame();
                CalculateDelay = newFrame - OriginalFrame;
                if (CalculateDelay >= frame) break;
                Thread.Sleep(10);
            }
            while (!IsSuspendPlay);
            OriginalFrame = newFrame;
            ErrorFrame = CalculateDelay - frame;
        }
    }
}
