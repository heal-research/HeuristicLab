#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;

namespace HeuristicLab.MainForm.WindowsForms {
  public partial class AsynchronousContentView : ContentView {
    public AsynchronousContentView() {
      InitializeComponent();
    }

    /// <summary>
    /// Asynchronous call of GUI updating.
    /// </summary>
    /// <param name="method">The delegate to invoke.</param>
    protected new void Invoke(Delegate method) {
      // prevents blocking of worker thread in Invoke, if the control is disposed
      IAsyncResult result = BeginInvoke(method);
      result.AsyncWaitHandle.WaitOne(1000, false);
      if (result.IsCompleted) try { EndInvoke(result); }
        catch (ObjectDisposedException) { } else {
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
      // prevents blocking of worker thread in Invoke, if the control is disposed
      IAsyncResult result = BeginInvoke(method, args);
      result.AsyncWaitHandle.WaitOne(1000, false);
      if (result.IsCompleted) try { EndInvoke(result); }
        catch (ObjectDisposedException) { } else {
        ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle,
          new WaitOrTimerCallback((x, b) => { try { EndInvoke(result); } catch (ObjectDisposedException) { } }),
          null, -1, true);
      }
    }
  }
}
