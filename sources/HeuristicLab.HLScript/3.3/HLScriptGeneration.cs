using System;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Threading;
using HeuristicLab.Common;

namespace HeuristicLab.HLScript {
  public abstract class HLScriptGeneration {
    protected dynamic vars;

    private readonly EventWriter console;
    protected EventWriter Console {
      get { return console; }
    }

    protected HLScriptGeneration() {
      console = new EventWriter(this);
    }

    public abstract void Main();

    private void Execute(VariableStore variableStore) {
      vars = new Variables(variableStore);
      try {
        Main();
      } catch (ThreadAbortException) {
      } catch (Exception e) {
        Console.WriteLine("---");
        Console.WriteLine(e);
      }
    }

    protected internal event EventHandler<EventArgs<string>> ConsoleOutputChanged;
    private void OnConsoleOutputChanged(string args) {
      var handler = ConsoleOutputChanged;
      if (handler != null) handler(null, new EventArgs<string>(args));
    }

    private class Variables : DynamicObject {
      private readonly VariableStore variableStore;

      public Variables(VariableStore variableStore) {
        this.variableStore = variableStore;
      }

      public override bool TryGetMember(GetMemberBinder binder, out object result) {
        return variableStore.TryGetValue(binder.Name, out result);
      }

      public override bool TrySetMember(SetMemberBinder binder, object value) {
        variableStore[binder.Name] = value;
        return true;
      }
    }

    protected class EventWriter : TextWriter {
      private readonly HLScriptGeneration hlsg;

      public EventWriter(HLScriptGeneration hlsg) {
        this.hlsg = hlsg;
      }

      public override Encoding Encoding {
        get { return Encoding.UTF8; }
      }

      #region Write/WriteLine Overrides
      #region Write
      public override void Write(bool value) { hlsg.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(char value) { hlsg.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(char[] buffer) { hlsg.OnConsoleOutputChanged(new string(buffer)); }
      public override void Write(char[] buffer, int index, int count) { hlsg.OnConsoleOutputChanged(new string(buffer, index, count)); }
      public override void Write(decimal value) { hlsg.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(double value) { hlsg.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(float value) { hlsg.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(int value) { hlsg.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(long value) { hlsg.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(object value) { hlsg.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(string value) { hlsg.OnConsoleOutputChanged(value); }
      public override void Write(string format, object arg0) { hlsg.OnConsoleOutputChanged(string.Format(format, arg0)); }
      public override void Write(string format, object arg0, object arg1) { hlsg.OnConsoleOutputChanged(string.Format(format, arg0, arg0)); }
      public override void Write(string format, object arg0, object arg1, object arg2) { hlsg.OnConsoleOutputChanged(string.Format(format, arg0, arg1, arg2)); }
      public override void Write(string format, params object[] arg) { hlsg.OnConsoleOutputChanged(string.Format(format, arg)); }
      public override void Write(uint value) { hlsg.OnConsoleOutputChanged(value.ToString()); }
      public override void Write(ulong value) { hlsg.OnConsoleOutputChanged(value.ToString()); }
      #endregion

      #region WriteLine
      public override void WriteLine() { hlsg.OnConsoleOutputChanged(Environment.NewLine); }
      public override void WriteLine(bool value) { hlsg.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(char value) { hlsg.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(char[] buffer) { hlsg.OnConsoleOutputChanged(new string(buffer) + Environment.NewLine); }
      public override void WriteLine(char[] buffer, int index, int count) { hlsg.OnConsoleOutputChanged(new string(buffer, index, count) + Environment.NewLine); }
      public override void WriteLine(decimal value) { hlsg.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(double value) { hlsg.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(float value) { hlsg.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(int value) { hlsg.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(long value) { hlsg.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(object value) { hlsg.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(string value) { hlsg.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(string format, object arg0) { hlsg.OnConsoleOutputChanged(string.Format(format, arg0) + Environment.NewLine); }
      public override void WriteLine(string format, object arg0, object arg1) { hlsg.OnConsoleOutputChanged(string.Format(format, arg0, arg1) + Environment.NewLine); }
      public override void WriteLine(string format, object arg0, object arg1, object arg2) { hlsg.OnConsoleOutputChanged(string.Format(format, arg0, arg1, arg2) + Environment.NewLine); }
      public override void WriteLine(string format, params object[] arg) { hlsg.OnConsoleOutputChanged(string.Format(format, arg) + Environment.NewLine); }
      public override void WriteLine(uint value) { hlsg.OnConsoleOutputChanged(value + Environment.NewLine); }
      public override void WriteLine(ulong value) { hlsg.OnConsoleOutputChanged(value + Environment.NewLine); }
      #endregion
      #endregion
    }
  }
}
