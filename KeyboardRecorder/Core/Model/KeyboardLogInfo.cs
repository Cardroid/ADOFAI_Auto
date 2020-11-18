using NeatInput.Windows.Processing.Keyboard.Enums;

namespace ADOFAI_Auto.Core.Model
{
    public class KeyboardLogInfo
    {
        public KeyboardLogInfo(int currentFrame, int beforeFrame, int delay, string key, string state)
        {
            this.CurrentFrame = currentFrame;
            this.BeforeFrame = beforeFrame;
            this.Delay = delay;
            this.Key = key;
            this.State = state;
        }

        public KeyboardLogInfo(int currentFrame, int beforeFrame, int errorFrame, int delay, string key, string state)
            : this(currentFrame, beforeFrame, delay, key, state)
        {
            this.ErrorFrame = errorFrame;
        }

        public int CurrentFrame { get; }
        public int BeforeFrame { get; }
        public int ErrorFrame { get; }
        public int Delay { get; }
        public string Key { get; }
        public string State { get; }
    }
}
