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
using System.Windows.Forms;

namespace HeuristicLab.MainForm.WindowsForms {
  public partial class View : UserControl, IView {
    private bool initialized;
    public View() {
      InitializeComponent();
      this.initialized = false;
      this.isShown = false;
      this.closeReason = CloseReason.None;
      this.readOnly = false;
    }

    public View(bool readOnly)
      : this() {
      this.readOnly = readOnly;
    }

    private string caption;
    public string Caption {
      get { return caption; }
      set {
        if (InvokeRequired) {
          Action<string> action = delegate(string s) { this.Caption = s; };
          Invoke(action, value);
        } else {
          if (value != caption) {
            caption = value;
            OnCaptionChanged();
          }
        }
      }
    }

    private bool readOnly;
    public virtual bool ReadOnly {
      get { return this.readOnly; }
       set {
        if (InvokeRequired) {
          Action<bool> action = delegate(bool b) { this.ReadOnly = b; };
          Invoke(action, value);
        } else {
          if (value != readOnly) {
            readOnly = value;
            OnReadOnlyChanged();
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
      if (InvokeRequired)
        Invoke((MethodInvoker)OnCaptionChanged);
      else {
        EventHandler handler = CaptionChanged;
        if (handler != null)
          handler(this, EventArgs.Empty);
      }
    }
    public event EventHandler ReadOnlyChanged;
    protected virtual void OnReadOnlyChanged() {
      if (InvokeRequired)
        Invoke((MethodInvoker)OnReadOnlyChanged);
      else {
        EventHandler handler = ReadOnlyChanged;
        if (handler != null)
          handler(this, EventArgs.Empty);
        foreach (Control control in this.Controls) {
          IView view = control as IView;
          if (view != null)
            view.ReadOnly = this.readOnly;
          ViewHost viewHost = control as ViewHost;
          if (viewHost != null)
            viewHost.ReadOnly = this.readOnly;
        }
      }
    }
    public event EventHandler Changed;
    protected virtual void OnChanged() {
      if (InvokeRequired)
        Invoke((MethodInvoker)OnChanged);
      else {
        EventHandler handler = Changed;
        if (handler != null)
          handler(this, EventArgs.Empty);
      }
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

    internal void OnClosingHelper(object sender, FormClosingEventArgs e) {
      FormClosingEventArgs eventArgs = new FormClosingEventArgs(this.closeReason, e.Cancel);
      if (this.closeReason != CloseReason.None)
        this.OnClosing(eventArgs);
      else
        this.OnClosing(e);

      if (eventArgs.Cancel != e.Cancel)
        e.Cancel = eventArgs.Cancel;
      this.closeReason = CloseReason.None;
    }

    protected virtual void OnClosing(FormClosingEventArgs e) {
    }

    internal void OnClosedHelper(object sender, FormClosedEventArgs e) {
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

    public new bool Enabled {
      get { return base.Enabled; }
      set {
        SuspendRepaint();
        base.Enabled = value;
        ResumeRepaint(true);
      }
    }

    public void SuspendRepaint() {
      ((Control)this).SuspendRepaint();
    }
    public void ResumeRepaint(bool refresh) {
      ((Control)this).ResumeRepaint(refresh);
    }
  }
}
