using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        public async void Play(KeyboardLog[] keyboardLogs)
        {
            if (keyboardLogs == null || IsPlaying)
                return;

            IsPlaying = true;
            for (int i = 0; i < keyboardLogs.Length; i++)
            {
                if (IsSuspendPlay)
                    break;
                await KeyLogHandle(keyboardLogs[i]);
            }
            IsSuspendPlay = false;
            IsPlaying = false;
        }

        public void Stop() => IsSuspendPlay = true;

        private async Task KeyLogHandle(KeyboardLog keyboardLog)
        {
            int beforeFrame = MemoryReader.GetFrame();
            int errorFrame = await FrameWait(keyboardLog.FrameDelay);
            KeyboardInput.PressKey();
            KeyEvent?.Invoke(null, new KeyboardLogInfo(MemoryReader.GetFrame(), beforeFrame, errorFrame, keyboardLog.FrameDelay, "Auto", "Down"));
        }

        private async Task<int> FrameWait(int frame, int frmaeReadDelay = 1)
        {
            int originalFrame = MemoryReader.GetFrame();
            int newFrame;
            int CalculateDelay;

            do
            {
                newFrame = MemoryReader.GetFrame();
                CalculateDelay = newFrame - originalFrame;
                if (CalculateDelay >= frame) break;
                await Task.Delay(frmaeReadDelay);
            }
            while (!IsSuspendPlay);
            return CalculateDelay - frame; /* Return Error Frame */
        }
    }
}
