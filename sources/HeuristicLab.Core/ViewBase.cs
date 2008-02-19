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

namespace HeuristicLab.Core {
  public partial class ViewBase : UserControl, IView {
    private IItem myItem;
    public IItem Item {
      get { return myItem; }
      protected set {
        if (value != myItem) {
          if (myItem != null)
            RemoveItemEvents();
          myItem = value;
          if (myItem != null)
            AddItemEvents();
          OnItemChanged();
          Refresh();
        }
      }
    }
    private string myCaption;
    public string Caption {
      get { return myCaption; }
      set {
        if (value != myCaption) {
          myCaption = value;
          OnCaptionChanged();
        }
      }
    }

    public ViewBase() {
      InitializeComponent();
      Caption = "View";
    }

    protected virtual void RemoveItemEvents() { }
    protected virtual void AddItemEvents() { }

    public override void Refresh() {
      if (InvokeRequired) {
        Invoke(new MethodInvoker(Refresh));
      } else {
        UpdateControls();
        base.Refresh();
      }
    }
    protected virtual void UpdateControls() {
      if (Item == null)
        Caption = "View";
      else
        Caption = "View (" + Item.GetType().Name + ")";
      
    }

    public event EventHandler ItemChanged;
    protected virtual void OnItemChanged() {
      if (ItemChanged != null)
        ItemChanged(this, new EventArgs());
    }
    public event EventHandler CaptionChanged;
    protected virtual void OnCaptionChanged() {
      if (CaptionChanged != null)
        CaptionChanged(this, new EventArgs());
    }

    protected new void Invoke(Delegate method) {
      // enforce context switch to improve GUI response time
      System.Threading.Thread.Sleep(0);

      // prevent blocking of worker thread in Invoke, if the control is disposed
      IAsyncResult result = BeginInvoke(method);
      while ((!result.AsyncWaitHandle.WaitOne(100, false)) && (!IsDisposed)) { }
      if (!IsDisposed) EndInvoke(result);
    }
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
