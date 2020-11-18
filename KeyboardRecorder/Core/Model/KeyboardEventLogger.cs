using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NeatInput.Windows;
using NeatInput.Windows.Events;

namespace ADOFAI_Auto.Core.Model
{
    public class KeyboardEventLogger : IKeyboardEventReceiver
    {
        public event EventHandler<KeyboardEvent> KeyboardEvent;

        public void Receive(KeyboardEvent @event) => KeyboardEvent?.Invoke(this, @event);
    }
}
