using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ADOFAI_Auto.Core.Model;

using NeatInput.Windows;
using NeatInput.Windows.Processing.Keyboard.Enums;

namespace ADOFAI_Auto.Core
{
    public class KeyboardContorller : INotifyPropertyChanged
    {
        private KeyboardContorller()
        {
            KeyboardEventLogger = new KeyboardEventLogger();
            InputSource = new InputSource(KeyboardEventLogger);
            KeyboardEventLogger.KeyboardEvent += KeyboardEventLogger_KeyboardEvent;
            InputSource.Listen();
        }

        private static KeyboardContorller Instence;
        public static KeyboardContorller GetInstence()
        {
            if (Instence == null)
                Instence = new KeyboardContorller();
            return Instence;
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                Recorder.PropertyChanged += value;
                Player.PropertyChanged += value;
            }
            remove
            {
                Recorder.PropertyChanged -= value;
                Player.PropertyChanged -= value;
            }
        }
        public event EventHandler<KeyboardLogInfo> KeyEvent
        {
            add
            {
                Recorder.KeyEvent += value;
                Player.KeyEvent += value;
            }
            remove
            {
                Recorder.KeyEvent -= value;
                Player.KeyEvent -= value;
            }
        }

        private InputSource InputSource { get; }
        private KeyboardEventLogger KeyboardEventLogger { get; }
        public KeyboardRecorder Recorder { get; } = new KeyboardRecorder();
        public KeyboardPlayer Player { get; } = new KeyboardPlayer();

        public bool IsRecording => Recorder.IsRecording;
        public bool IsPlaying => Player.IsPlaying;

        private void KeyboardEventLogger_KeyboardEvent(object sender, NeatInput.Windows.Events.KeyboardEvent e)
        {
            if (e.State == KeyStates.Down)
            {
                if (IsPlaying)
                {
                    if (e.Key == Keys.E)
                        Player.Stop();
                }
                else if (!IsRecording)
                {
                    if (e.Key == Keys.R)
                        Recorder.StartRecord(KeyboardEventLogger);
                    else if (e.Key == Keys.E)
                        Player.Play(Recorder.GetRecord().ToArray());
                    else if (e.Key == Keys.Delete)
                        Recorder.RecordClear();
                }
                else
                {
                    if (e.Key == Keys.R)
                        Recorder.StopRecord();
                }
            }
        }
    }
}
