#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Reflection;
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
      if (ViewAttribute.HasViewAttribute(this.GetType()))
        this.Caption = ViewAttribute.GetViewName(this.GetType());
      else
        this.Caption = "View";
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
            this.readOnly = value;
            OnReadOnlyChanged();
            PropertyInfo prop = typeof(IView).GetProperty("ReadOnly");
            PropagateStateChanges(this, typeof(IView), prop);
            SetEnabledStateOfControls();
            OnChanged();
          }
        }
      }
    }

    public new bool Enabled {
      get { return base.Enabled; }
      set {
        if (base.Enabled != value) {
          this.SuspendRepaint();
          base.Enabled = value;
          this.ResumeRepaint(true);
          OnChanged();
        }
      }
    }

    bool IView.Enabled {
      get { return Enabled; }
      set { Enabled = value; }
    }

    protected override void OnEnabledChanged(EventArgs e) {
      base.OnEnabledChanged(e);
      if (Enabled) SetEnabledStateOfControls();
    }

    /// <summary>
    /// This method is called if the ReadyOnly property of the View changes to update the controls of the view.
    /// </summary>
    protected virtual void SetEnabledStateOfControls() {
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
      mainform.ShowView(this);
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
      }
    }
    protected virtual void PropagateStateChanges(Control control, Type type, PropertyInfo propertyInfo) {
      if (!type.GetProperties().Contains(propertyInfo))
        throw new ArgumentException("The specified type " + type + "implement the property " + propertyInfo.Name + ".");
      if (!type.IsAssignableFrom(this.GetType()))
        throw new ArgumentException("The specified type " + type + "must be the same or a base class / interface of this object.");
      if (!propertyInfo.CanWrite)
        throw new ArgumentException("The specified property " + propertyInfo.Name + " must have a setter.");

      foreach (Control c in control.Controls) {
        Type controlType = c.GetType();
        PropertyInfo controlPropertyInfo = controlType.GetProperty(propertyInfo.Name, propertyInfo.PropertyType);
        if (type.IsAssignableFrom(controlType) && controlPropertyInfo != null) {
          var thisValue = propertyInfo.GetValue(this, null);
          controlPropertyInfo.SetValue(c, thisValue, null);
        } else PropagateStateChanges(c, type, propertyInfo);
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

    internal protected virtual void OnShown(ViewShownEventArgs e) {
    }

    internal protected virtual void OnHidden(EventArgs e) {
    }

    private CloseReason closeReason;
    internal CloseReason CloseReason {
      get { return this.closeReason; }
      set { this.closeReason = value; }
    }

    internal void OnClosingHelper(object sender, FormClosingEventArgs e) {
      FormClosingEventArgs eventArgs = new FormClosingEventArgs(this.closeReason, e.Cancel);
      if (this.closeReason != CloseReason.None) {
        this.OnClosing(eventArgs);
        if (eventArgs.Cancel != e.Cancel)
          e.Cancel = eventArgs.Cancel;
      } else
        this.OnClosing(e);
      this.closeReason = CloseReason.None;
    }

    internal protected virtual void OnClosing(FormClosingEventArgs e) {
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

    internal protected virtual void OnClosed(FormClosedEventArgs e) {
    }

    private void View_Load(object sender, EventArgs e) {
      if (!this.initialized && !this.DesignMode) {
        this.OnInitialized(e);
        this.initialized = true;
      }
    }

    protected virtual void OnInitialized(EventArgs e) {
      SetEnabledStateOfControls();
    }


    public void SuspendRepaint() {
      if (InvokeRequired)
        Invoke((MethodInvoker)SuspendRepaint);
      else
        ((Control)this).SuspendRepaint();
    }
    public void ResumeRepaint(bool refresh) {
      if (InvokeRequired)
        Invoke((Action<bool>)ResumeRepaint, refresh);
      else
        ((Control)this).ResumeRepaint(refresh);
    }
  }
}
