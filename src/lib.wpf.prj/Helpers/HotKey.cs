using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lib.WPF.Helpers
{
    public class HotKey : IHotKey
    {
        /// <inheritdoc/>
        public Key Key { get; }

        /// <inheritdoc/>
        public ModifierKeys KeyModifiers { get; }

        /// <inheritdoc/>
        public ICommand? Command { get; }

        /// <inheritdoc/>
        public object? CommandParameter { get; }

        public HotKey(
             Key key,
             ICommand? command,
             object? commandParameter = null,
             ModifierKeys? keyModifiers = null)
        {
            Key = key;
            Command = command;
            CommandParameter = commandParameter;
            KeyModifiers = keyModifiers == null ? ModifierKeys.None : keyModifiers.Value;
        }
    }
}
