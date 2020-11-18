using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WindowsInput;

namespace ADOFAI_Auto.Core
{
    public static class KeyboardInput
    {
        static KeyboardInput()
        {
            Simulator = new KeyboardSimulator(new InputSimulator());
        }

        private static KeyboardSimulator Simulator;

        private static bool Switch;

        public static void PressKey()
        {
            if (Switch)
                Simulator.KeyPress(WindowsInput.Native.VirtualKeyCode.VK_A);
            else
                Simulator.KeyPress(WindowsInput.Native.VirtualKeyCode.VK_B);

            Switch = !Switch;
        }
    }
}
