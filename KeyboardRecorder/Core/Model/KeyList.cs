using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOFAI_Auto.Core.Model
{
    public class KeyList
    {
        public KeyList()
        {
            Logs = new List<KeyboardLog>();
        }

        private List<KeyboardLog> Logs;

        public int InputOffset = 0;
        public int GlobalOffset = 0;

        public void Add(KeyboardLog log) => Logs.Add(log);
        public void Clear(bool isOffsetClear = true)
        {
            if (isOffsetClear)
            {
                InputOffset = 0;
                GlobalOffset = 0;
            }
            Logs.Clear();
        }
        public IReadOnlyList<KeyboardLog> GetRecord() => Logs;
        public int Count => Logs.Count;
        public KeyboardLog this[int index]
        {
            get => Logs[index];
            set => Logs[index] = value;
        }
    }
}
