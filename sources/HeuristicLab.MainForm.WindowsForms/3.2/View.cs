#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public partial class View : UserControl, IView {
    private bool initialized;
    public View() {
      InitializeComponent();
      this.initialized = false;
      this.isShown = false;
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

    private bool isShown;
    public bool IsShown {
      get { return this.isShown; }
      private set { this.isShown = value; }
    }

    public new void Show() {
      MainForm mainform = MainFormManager.GetMainForm<MainForm>();
      bool firstTimeShown = mainform.GetForm(this) == null;

      this.IsShown = true;
      mainform.ShowView(this, firstTimeShown);
      if (firstTimeShown) {
        Form form = mainform.GetForm(this);
        form.FormClosed += new FormClosedEventHandler(OnClosedHelper);
        form.FormClosing += new FormClosingEventHandler(OnClosingHelper);
      }
      this.OnShown(new ViewShownEventArgs(this, firstTimeShown));
    }

    public void Close() {
      MainForm mainform = MainFormManager.GetMainForm<MainForm>();
      Form form = mainform.GetForm(this);
      if (form != null) {
        this.IsShown = false;
        mainform.CloseView(this);
      }
    }

    public void Close(CloseReason closeReason) {
      MainForm mainform = MainFormManager.GetMainForm<MainForm>();
      Form form = mainform.GetForm(this);
      if (form != null) {
        this.IsShown = false;
        mainform.CloseView(this, closeReason);
      }
    }

    public new void Hide() {
      this.IsShown = false;
      MainFormManager.GetMainForm<MainForm>().HideView(this);
      this.OnHidden(EventArgs.Empty);
    }

    public event EventHandler CaptionChanged;
    protected virtual void OnCaptionChanged() {
      if (CaptionChanged != null)
        CaptionChanged(this, EventArgs.Empty);
    }

    public event EventHandler Changed;
    protected virtual void OnChanged() {
      if (InvokeRequired)
        Invoke((MethodInvoker)OnChanged);
      else if (Changed != null)
        Changed(this, EventArgs.Empty);
    }

    protected virtual void OnShown(ViewShownEventArgs e) {
    }

    protected virtual void OnHidden(EventArgs e) {
    }

    private CloseReason closeReason;
    internal CloseReason CloseReason {
      get { return this.closeReason; }
      set { this.closeReason = value; }
    }

    private void OnClosingHelper(object sender, FormClosingEventArgs e) {
      if (this.closeReason != CloseReason.None)
        this.OnClosing(new FormClosingEventArgs(this.closeReason, e.Cancel));
      else
        this.OnClosing(e);

      this.closeReason = CloseReason.None;
    }

    protected virtual void OnClosing(FormClosingEventArgs e) {
    }

    private void OnClosedHelper(object sender, FormClosedEventArgs e) {
      if (this.closeReason != CloseReason.None)
        this.OnClosed(new FormClosedEventArgs(this.closeReason));
      else
        this.OnClosed(e);

      Form form = (Form)sender;
      form.FormClosed -= new FormClosedEventHandler(OnClosedHelper);
      form.FormClosing -= new FormClosingEventHandler(OnClosingHelper);
      this.closeReason = CloseReason.None;
    }

    protected virtual void OnClosed(FormClosedEventArgs e) {
    }

    private void View_Load(object sender, EventArgs e) {
      if (!this.initialized && !this.DesignMode) {
        this.OnInitialized(e);
        this.initialized = true;
      }
    }

    protected virtual void OnInitialized(EventArgs e) {
    }
  }
}
