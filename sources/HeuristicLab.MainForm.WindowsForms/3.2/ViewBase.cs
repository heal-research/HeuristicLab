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

namespace HeuristicLab.MainForm.WindowsForms {
  public partial class ViewBase : UserControl, IView {
    private bool initialized;
    public ViewBase() {
      InitializeComponent();
      this.initialized = false;
      this.closeReason = CloseReason.None;
    }

    private string myCaption;
    public string Caption {
      get { return myCaption; }
      set {
        if (InvokeRequired) {
          Action<string> action = delegate(string s) { this.Caption = s; };
          Invoke(action, value);
        } else {
          if (value != myCaption) {
            myCaption = value;
            OnCaptionChanged();
          }
        }
      }
    }

    public event EventHandler CaptionChanged;
    protected virtual void OnCaptionChanged() {
      if (CaptionChanged != null)
        CaptionChanged(this, new EventArgs());
    }

    public event EventHandler Changed;
    protected virtual void OnChanged() {
      if (InvokeRequired)
        Invoke((MethodInvoker)OnChanged);
      else if (Changed != null)
        Changed(this, new EventArgs());
    }

    public virtual void OnClosing(object sender, CancelEventArgs e) {
    }

    internal CloseReason closeReason;
    internal void OnClosingHelper(object sender, FormClosingEventArgs e) {
      if (this.closeReason != CloseReason.None)
        this.OnClosing(sender, new FormClosingEventArgs(this.closeReason, e.Cancel));
      else
        this.OnClosing(sender, e);

      this.closeReason = CloseReason.None;
    }

    public virtual void OnClosing(object sender, FormClosingEventArgs e) {
    }

    public virtual void OnClosed(object sender, EventArgs e) {
    }

    public event EventHandler Initialized;

    private void ViewBase_Load(object sender, EventArgs e) {
      if (!this.initialized && !this.DesignMode) {
        if (this.Initialized != null)
          this.Initialized(this, new EventArgs());
        this.initialized = true;
      }
    }
  }
}
