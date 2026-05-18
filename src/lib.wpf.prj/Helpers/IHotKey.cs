using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lib.WPF.Helpers
{
    public interface IHotKey
    {
        /// <summary>
        /// Горячая клавиша.
        /// </summary>
        public Key Key { get; }

        /// <summary>
        /// Горячая клавиша-модификатор. Может быть несколько, ибо это enum flags.
        /// Например: (ModifierKeys.Alt | ModifierKeys.Shift | ...), - если нужно больше одной доп.клавиши.
        /// </summary>
        public ModifierKeys KeyModifiers { get; }

        /// <summary>
        /// Команда, исполняемая при нажатии.
        /// </summary>
        public ICommand? Command { get; }

        /// <summary>
        /// Параметр команды. Может быть null.
        /// </summary>
        public object? CommandParameter { get; }
    }
}
