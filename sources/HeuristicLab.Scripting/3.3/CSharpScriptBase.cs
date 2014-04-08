#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.IO;
using System.Text;
using HeuristicLab.Common;

namespace HeuristicLab.Scripting {
  public abstract class CSharpScriptBase {
    protected dynamic vars;

    private readonly EventWriter console;
    protected EventWriter Console {
      get { return console; }
    }

    protected CSharpScriptBase() {
      console = new EventWriter(this);
    }

    public abstract void Main();

    internal void Execute(VariableStore variableStore) {
      vars = new Variables(variableStore);
      Main();
    }

    protected internal event EventHandler<EventArgs<string>> ConsoleOutputChanged;
    private void OnConsoleOutputChanged(string args) {
      var handler = ConsoleOutputChanged;
      if (handler != null) handler(null, new EventArgs<string>(args));
    }

    protected class EventWriter : TextWriter {
      private readonly CSharpScriptBase usb;

      public EventWriter(CSharpScriptBase usb) {
        this.usb = usb;
      }

      public override Encoding Encoding {
        get { return Encoding.UTF8; }
      }

      #region Write/WriteLine Overrides
      #region Write
      public override void Write(bool value) { usb.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(char value) { usb.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(char[] buffer) { usb.OnConsoleOutputChanged(new string(buffer)); }
      public override void Write(char[] buffer, int index, int count) { usb.OnConsoleOutputChanged(new string(buffer, index, count)); }
      public override void Write(decimal value) { usb.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(double value) { usb.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(float value) { usb.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(int value) { usb.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(long value) { usb.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(object value) { usb.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(string value) { usb.OnConsoleOutputChanged(value); }
      public override void Write(string format, object arg0) { usb.OnConsoleOutputChanged(string.Format(format, arg0)); }
      public override void Write(string format, object arg0, object arg1) { usb.OnConsoleOutputChanged(string.Format(format, arg0, arg0)); }
      public override void Write(string format, object arg0, object arg1, object arg2) { usb.OnConsoleOutputChanged(string.Format(format, arg0, arg1, arg2)); }
      public override void Write(string format, params object[] arg) { usb.OnConsoleOutputChanged(string.Format(format, arg)); }
      public override void Write(uint value) { usb.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(ulong value) { usb.OnConsoleOutputChanged(value.ToString()); }
      #endregion

      #region WriteLine
      public override void WriteLine() { usb.OnConsoleOutputChanged(Environment.NewLine); }
      public override void WriteLine(bool value) { usb.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(char value) { usb.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(char[] buffer) { usb.OnConsoleOutputChanged(new string(buffer) + Environment.NewLine); }
      public override void WriteLine(char[] buffer, int index, int count) { usb.OnConsoleOutputChanged(new string(buffer, index, count) + Environment.NewLine); }
      public override void WriteLine(decimal value) { usb.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(double value) { usb.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(float value) { usb.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(int value) { usb.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(long value) { usb.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(object value) { usb.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(string value) { usb.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(string format, object arg0) { usb.OnConsoleOutputChanged(string.Format(format, arg0) + Environment.NewLine); }
      public override void WriteLine(string format, object arg0, object arg1) { usb.OnConsoleOutputChanged(string.Format(format, arg0, arg1) + Environment.NewLine); }
      public override void WriteLine(string format, object arg0, object arg1, object arg2) { usb.OnConsoleOutputChanged(string.Format(format, arg0, arg1, arg2) + Environment.NewLine); }
      public override void WriteLine(string format, params object[] arg) { usb.OnConsoleOutputChanged(string.Format(format, arg) + Environment.NewLine); }
      public override void WriteLine(uint value) { usb.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(ulong value) { usb.OnConsoleOutputChanged(value + Environment.NewLine); }
      #endregion
      #endregion
    }
  }
}
