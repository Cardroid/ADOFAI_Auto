using System;
using System.Diagnostics;
using System.Threading.Tasks;

using ProcessMemoryScanner;

namespace ADOFAI_Auto.Core
{
    public static class MemoryReader
    {
        static MemoryReader()
        {
            FrameSync();
        }

        private static MemoryScanner MS;
        private static Process TargetProcess;
        private static MemoryAddress FrameAddress;
        private static MemoryAddress IsPauseAddress;
        private static MemoryAddress InputOffset;

        private static bool Init()
        {
            if (IsProcessNullCheck())
                Dispose();

            var processes = Process.GetProcessesByName("A Dance of Fire and Ice");
            if (!(processes.Length > 0))
                return false;

            try
            {
                TargetProcess = processes[0];
                MS = new MemoryScanner(p => p.ProcessName == TargetProcess.ProcessName);

                //FrameAddress = new MemoryAddress(new IntPtr(0x1E52CFC38B0), null);
                FrameAddress = new MemoryAddress(GetModuleBaseAddress("UnityPlayer.dll") + 0x017EEDD0, new int[] { 0x3D8, 0x0, 0x578, 0xE30 });
                //FrameAddress = new MemoryAddress(GetModuleBaseAddress("UnityPlayer.dll") + 0x0173E000, new int[] { 0x0, 0x20, 0x118 });

                IsPauseAddress = new MemoryAddress(GetModuleBaseAddress("UnityPlayer.dll") + 0x0179C458, new int[] { 0x170, 0x18, 0x18, 0x94 });
                InputOffset = new MemoryAddress(GetModuleBaseAddress("UnityPlayer.dll") + 0x01731010, new int[] { 0x270, 0x10, 0xA50, 0x70, 0x7C0 });
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsProcessNullCheck() => MS == null || TargetProcess == null || FrameAddress == null || IsPauseAddress == null;

        private static IntPtr GetModuleBaseAddress(string moduleName)
        {
            IntPtr addr = IntPtr.Zero;

            foreach (ProcessModule module in TargetProcess.Modules)
            {
                if (module.ModuleName == moduleName)
                {
                    addr = module.BaseAddress;
                    break;
                }
            }
            return addr;
        }

        private static IntPtr GetPointerAddress(MemoryAddress memoryAddress)
        {
            if (memoryAddress.Offsets == null)
                return memoryAddress.Address;

            IntPtr addr = MS.ReadMemory<IntPtr>(memoryAddress.Address);
            for (int i = 0; i < memoryAddress.Offsets.Length; i++)
            {
                addr = IntPtr.Add(addr, memoryAddress.Offsets[i]);
                if (memoryAddress.Offsets.Length - 1 == i)
                    break;
                addr = MS.ReadMemory<IntPtr>(addr);
            }
            return addr;
        }

        private static T ReadMemory<T>(MemoryAddress memoryAddress) where T : IConvertible
        {
            if ((IsProcessNullCheck() && !Init()) || memoryAddress == null)
                return default;

            if (HighAccuracyMode)
            {
                MS.SuspendProcess();
                var result = MS.ReadMemory<T>(GetPointerAddress(memoryAddress));
                MS.ResumeProcess();
                return result;
            }
            else
                return MS.ReadMemory<T>(GetPointerAddress(memoryAddress));
        }

        private static void WriteMemory<T>(MemoryAddress memoryAddress, T value) where T : IConvertible
        {
            if ((IsProcessNullCheck() && !Init()) || memoryAddress == null)
                return;

            if (HighAccuracyMode)
            {
                MS.SuspendProcess();
                MS.WriteMemory(GetPointerAddress(memoryAddress), value);
                MS.ResumeProcess();
                return;
            }
            else
                MS.WriteMemory(GetPointerAddress(memoryAddress), value);
        }

        public static bool HighAccuracyMode = false;

        public static bool IsPause() => ReadMemory<bool>(IsPauseAddress);

        private static async void FrameSync()
        {
            while (true)
            {
                PreloadFrame = ReadMemory<int>(FrameAddress);
                await Task.Delay(1);
            }
        }
        private static int PreloadFrame = 0;
        public static int GetFrame() => PreloadFrame;
        public static int GetInputOffset() => ReadMemory<int>(InputOffset);
        public static void SetInputOffset(int value) => WriteMemory(InputOffset, value);

        private class MemoryAddress
        {
            public MemoryAddress(IntPtr address, int[] offsets)
            {
                this.Address = address;
                this.Offsets = offsets;
            }

            public IntPtr Address { get; }
            public int[] Offsets { get; }
        }

        public static void Dispose()
        {
            if (MS != null)
                MS.Dispose();
            if (TargetProcess != null)
                TargetProcess.Dispose();

            MS = null;
            TargetProcess = null;
        }
    }
}
