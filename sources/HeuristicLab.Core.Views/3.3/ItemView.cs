#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// Base class for all visual representations.
  /// </summary>
  public partial class ItemView : ContentView {
    public new IItem Content {
      get { return (IItem)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ViewBase"/> with the caption "View".
    /// </summary>
    public ItemView() {
      InitializeComponent();
      Caption = "View";
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        Caption = "View";
      } else {
        Caption = Content.ItemName;
      }
    }

    /// <summary>
    /// Asynchron call of GUI updating.
    /// </summary>
    /// <param name="method">The delegate to invoke.</param>
    protected new void Invoke(Delegate method) {
      // enforce context switch to improve GUI response time
      System.Threading.Thread.Sleep(0);

      // prevent blocking of worker thread in Invoke, if the control is disposed
      IAsyncResult result = BeginInvoke(method);
      while ((!result.AsyncWaitHandle.WaitOne(100, false)) && (!IsDisposed)) { }
      if (!IsDisposed) EndInvoke(result);
    }
    /// <summary>
    /// Asynchron call of GUI updating.
    /// </summary>
    /// <param name="method">The delegate to invoke.</param>
    /// <param name="args">The invoke arguments.</param>
    protected new void Invoke(Delegate method, params object[] args) {
      // enforce context switch to improve GUI response time
      System.Threading.Thread.Sleep(0);

      // prevent blocking of worker thread in Invoke, if the control is disposed
      IAsyncResult result = BeginInvoke(method, args);
      while ((!result.AsyncWaitHandle.WaitOne(100, false)) && (!IsDisposed)) { }
      if (!IsDisposed) EndInvoke(result);
    }
  }
}
