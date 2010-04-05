using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace HeuristicLab.MainForm.WindowsForms {
  public partial class AsynchronousContentView : ContentView {
    public AsynchronousContentView() {
      InitializeComponent();
    }

    public AsynchronousContentView(object content)
      : this() {
      this.Content = content;
    }

    /// <summary>
    /// Asynchronous call of GUI updating.
    /// </summary>
    /// <param name="method">The delegate to invoke.</param>
    protected new void Invoke(Delegate method) {
      // prevent blocking of worker thread in Invoke, if the control is disposed
      IAsyncResult result = BeginInvoke(method);
      result.AsyncWaitHandle.WaitOne(1000, false);
      if (result.IsCompleted) try { EndInvoke(result); } catch (ObjectDisposedException) { }
      else {
        ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle,
          new WaitOrTimerCallback((x, b) => { try { EndInvoke(result); } catch (ObjectDisposedException) { } }),
          null, -1, true);
      }
    }

    /// <summary>
    /// Asynchronous call of GUI updating.
    /// </summary>
    /// <param name="method">The delegate to invoke.</param>
    /// <param name="args">The invoke arguments.</param>
    protected new void Invoke(Delegate method, params object[] args) {
      // prevent blocking of worker thread in Invoke, if the control is disposed
      IAsyncResult result = BeginInvoke(method, args);
      result.AsyncWaitHandle.WaitOne(1000, false);
      if (result.IsCompleted) try { EndInvoke(result); } catch (ObjectDisposedException) { }
      else {
        ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle,
          new WaitOrTimerCallback((x, b) => { try { EndInvoke(result); } catch (ObjectDisposedException) { } }),
          null, -1, true);
      }
    }
  }
}
