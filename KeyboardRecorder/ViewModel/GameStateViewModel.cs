using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ADOFAI_Auto.Core;
using ADOFAI_Auto.Core.Model;

namespace ADOFAI_Auto.ViewModel
{
    public class GameStateViewModel : BaseViewModel
    {
        public GameStateViewModel()
        {
            GameStateGetter();
            KeyboardContorller.GetInstence().PropertyChanged += Recorder_PropertyChanged;
        }

        private async void GameStateGetter()
        {
            //int beforeFrame;
            while (true)
            {
                IsGamePause = MemoryReader.IsPause();
                //beforeFrame = GameFrame;
                GameFrame = MemoryReader.GetFrame();
                //System.Diagnostics.Debug.WriteLine($"Frame Gap: {GameFrame - beforeFrame}");
                //System.Diagnostics.Debug.WriteLine($"GameFrame: {GameFrame}");
                InputOffset = MemoryReader.GetInputOffset();
                await Task.Delay(100);
            }
        }

        private void Recorder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var controller = KeyboardContorller.GetInstence();

            if(!controller.Recorder.IsRecording && !controller.Player.IsPlaying)
                CurrentState = "Ready";
            else if (e.PropertyName == "IsRecording" && controller.Recorder.IsRecording)
                CurrentState = "Recording";
            else if (e.PropertyName == "IsPlaying" && controller.Player.IsPlaying)
                CurrentState = "Playing";
        }

        private bool _IsGamePause = false;
        public bool IsGamePause
        {
            get => _IsGamePause;
            set
            {
                _IsGamePause = value;
                OnPropertyChanged("IsGamePause");
            }
        }

        private int _GameFrame = -1;
        public int GameFrame
        {
            get => _GameFrame;
            set
            {
                _GameFrame = value;
                OnPropertyChanged("GameFrame");
            }
        }
        
        private int _InputOffset = -1;
        public int InputOffset
        {
            get => _InputOffset;
            set
            {
                _InputOffset = value;
                OnPropertyChanged("InputOffset");
            }
        }

        private string _CurrentState = "Ready";
        public string CurrentState
        {
            get => _CurrentState;
            set
            {
                _CurrentState = value;
                OnPropertyChanged("CurrentState");
            }
        }
    }
}
