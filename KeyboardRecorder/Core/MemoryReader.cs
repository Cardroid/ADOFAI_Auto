using System;
using System.Diagnostics;

using ProcessMemoryScanner;

namespace ADOFAI_Auto.Core
{
    public static class MemoryReader
    {
        private static MemoryScanner MS;
        private static Process TargetProcess;
        private static MemoryAddress FrameAddress;
        private static MemoryAddress IsPauseAddress;

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
                FrameAddress = new MemoryAddress(GetModuleBaseAddress("UnityPlayer.dll") + 0x0179C468, new int[] { 0x138, 0x30, 0x28, 0xC78 });
                IsPauseAddress = new MemoryAddress(GetModuleBaseAddress("UnityPlayer.dll") + 0x0179C458, new int[] { 0x170, 0x18, 0x18, 0x94 });
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

        private static T Reader<T>(MemoryAddress memoryAddress) where T : IConvertible
        {
            if (memoryAddress.Offsets == null)
                return MS.ReadMemory<T>(memoryAddress.Address);

            IntPtr addr = MS.ReadMemory<IntPtr>(memoryAddress.Address);
            for (int i = 0; i < memoryAddress.Offsets.Length; i++)
            {
                addr = IntPtr.Add(addr, memoryAddress.Offsets[i]);
                if (memoryAddress.Offsets.Length - 1 == i)
                    break;
                addr = MS.ReadMemory<IntPtr>(addr);
            }

            return MS.ReadMemory<T>(addr);
        }

        private static T ReadMemory<T>(MemoryAddress memoryAddress) where T : IConvertible
        {
            if ((IsProcessNullCheck() && !Init()) || memoryAddress == null)
                return default;

            //MS.SuspendProcess();
            //var result = Reader<T>(memoryAddress);
            //MS.ResumeProcess();
            //return result;

            return Reader<T>(memoryAddress);
        }

        public static bool IsPause() => ReadMemory<bool>(IsPauseAddress);
        public static int GetFrame() => ReadMemory<int>(FrameAddress);

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
