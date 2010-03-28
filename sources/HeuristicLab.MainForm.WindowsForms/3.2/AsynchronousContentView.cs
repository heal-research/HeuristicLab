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
      IAsyncResult res = base.BeginInvoke(method);
      ThreadPool.RegisterWaitForSingleObject(res.AsyncWaitHandle,
        new WaitOrTimerCallback((x, b) => { EndInvoke(res); }),
        null, -1, true);
    }
    /// <summary>
    /// Asynchronous call of GUI updating.
    /// </summary>
    /// <param name="method">The delegate to invoke.</param>
    /// <param name="args">The invoke arguments.</param>
    protected new void Invoke(Delegate method, params object[] args) {
      IAsyncResult res = base.BeginInvoke(method, args);
      ThreadPool.RegisterWaitForSingleObject(res.AsyncWaitHandle,
        new WaitOrTimerCallback((x, b) => { EndInvoke(res); }),
        null, -1, true);
    }
  }
}
