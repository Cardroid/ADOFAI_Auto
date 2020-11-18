using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOFAI_Auto.Core.Model
{
    public struct KeyboardLog
    {
        public KeyboardLog(int frameDelay)
        {
            this.FrameDelay = frameDelay;
        }

        public int FrameDelay { get; }
    }
}
