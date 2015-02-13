#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    protected Variables variables;
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
      variables = vars = new Variables(variableStore);
      Main();
    }

    protected internal event EventHandler<EventArgs<string>> ConsoleOutputChanged;
    private void OnConsoleOutputChanged(string args) {
      var handler = ConsoleOutputChanged;
      if (handler != null) handler(null, new EventArgs<string>(args));
    }

    protected class EventWriter : TextWriter {
      private readonly CSharpScriptBase script;

      public EventWriter(CSharpScriptBase script) {
        this.script = script;
      }

      public override Encoding Encoding {
        get { return Encoding.UTF8; }
      }

      #region Write/WriteLine Overrides
      #region Write
      public override void Write(bool value) { script.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(char value) { script.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(char[] buffer) { script.OnConsoleOutputChanged(new string(buffer)); }
      public override void Write(char[] buffer, int index, int count) { script.OnConsoleOutputChanged(new string(buffer, index, count)); }
      public override void Write(decimal value) { script.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(double value) { script.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(float value) { script.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(int value) { script.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(long value) { script.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(object value) { script.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(string value) { script.OnConsoleOutputChanged(value); }
      public override void Write(string format, object arg0) { script.OnConsoleOutputChanged(string.Format(format, arg0)); }
      public override void Write(string format, object arg0, object arg1) { script.OnConsoleOutputChanged(string.Format(format, arg0, arg0)); }
      public override void Write(string format, object arg0, object arg1, object arg2) { script.OnConsoleOutputChanged(string.Format(format, arg0, arg1, arg2)); }
      public override void Write(string format, params object[] arg) { script.OnConsoleOutputChanged(string.Format(format, arg)); }
      public override void Write(uint value) { script.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(ulong value) { script.OnConsoleOutputChanged(value.ToString()); }
      #endregion

      #region WriteLine
      public override void WriteLine() { script.OnConsoleOutputChanged(Environment.NewLine); }
      public override void WriteLine(bool value) { script.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(char value) { script.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(char[] buffer) { script.OnConsoleOutputChanged(new string(buffer) + Environment.NewLine); }
      public override void WriteLine(char[] buffer, int index, int count) { script.OnConsoleOutputChanged(new string(buffer, index, count) + Environment.NewLine); }
      public override void WriteLine(decimal value) { script.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(double value) { script.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(float value) { script.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(int value) { script.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(long value) { script.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(object value) { script.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(string value) { script.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(string format, object arg0) { script.OnConsoleOutputChanged(string.Format(format, arg0) + Environment.NewLine); }
      public override void WriteLine(string format, object arg0, object arg1) { script.OnConsoleOutputChanged(string.Format(format, arg0, arg1) + Environment.NewLine); }
      public override void WriteLine(string format, object arg0, object arg1, object arg2) { script.OnConsoleOutputChanged(string.Format(format, arg0, arg1, arg2) + Environment.NewLine); }
      public override void WriteLine(string format, params object[] arg) { script.OnConsoleOutputChanged(string.Format(format, arg) + Environment.NewLine); }
      public override void WriteLine(uint value) { script.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(ulong value) { script.OnConsoleOutputChanged(value + Environment.NewLine); }
      #endregion
      #endregion
    }
  }
}
