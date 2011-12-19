#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Parameters.Views {
  /// <summary>
  /// The visual representation of a <see cref="Parameter"/>.
  /// </summary>
  [View("ValueParameter View")]
  [Content(typeof(OptionalValueParameter<>), true)]
  [Content(typeof(ValueParameter<>), true)]
  [Content(typeof(IValueParameter<>), false)]
  public partial class ValueParameterView<T> : ParameterView where T : class, IItem {
    protected TypeSelectorDialog typeSelectorDialog;

    /// <summary>
    /// Gets or sets the variable to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public new IValueParameter<T> Content {
      get { return (IValueParameter<T>)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with caption "Variable".
    /// </summary>
    public ValueParameterView() {
      InitializeComponent();
    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (typeSelectorDialog != null) typeSelectorDialog.Dispose();
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="IVariable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterContentEvents() {
      Content.GetsCollectedChanged -= new EventHandler(Content_GetsCollectedChanged);
      Content.ValueChanged -= new EventHandler(Content_ValueChanged);
      base.DeregisterContentEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="IVariable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.GetsCollectedChanged += new EventHandler(Content_GetsCollectedChanged);
      Content.ValueChanged += new EventHandler(Content_ValueChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        showInRunCheckBox.Checked = false;
        valueViewHost.Content = null;
      } else {
        SetDataTypeTextBoxText();
        showInRunCheckBox.Checked = Content.GetsCollected;
        valueViewHost.ViewType = null;
        valueViewHost.Content = Content.Value;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      setValueButton.Enabled = Content != null && !(Content is IFixedValueParameter) && !ReadOnly;
      clearValueButton.Enabled = Content != null && Content.Value != null && !(Content is IFixedValueParameter) && !(Content is ValueParameter<T>) && !ReadOnly;
      showInRunCheckBox.Enabled = Content != null && !ReadOnly;
    }

    protected virtual void Content_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ValueChanged), sender, e);
      else {
        SetDataTypeTextBoxText();
        setValueButton.Enabled = Content != null && !(Content is IFixedValueParameter) && !ReadOnly;
        clearValueButton.Enabled = Content != null && Content.Value != null && !(Content is IFixedValueParameter<T>) && !(Content is ValueParameter<T>) && !ReadOnly;
        valueViewHost.ViewType = null;
        valueViewHost.Content = Content != null ? Content.Value : null;
      }
    }
    protected virtual void Content_GetsCollectedChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_GetsCollectedChanged), sender, e);
      else
        showInRunCheckBox.Checked = Content != null && Content.GetsCollected;
    }

    protected virtual void setValueButton_Click(object sender, EventArgs e) {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.Caption = "Select Value";
        typeSelectorDialog.TypeSelector.Configure(Content.DataType, false, true);
      }
      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          Content.Value = (T)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }
    protected virtual void clearValueButton_Click(object sender, EventArgs e) {
      Content.Value = null;
    }
    protected virtual void showInRunCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (Content != null) Content.GetsCollected = showInRunCheckBox.Checked;
    }
    protected virtual void valueGroupBox_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (Content is IFixedValueParameter) return;
      if (!ReadOnly && (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) != null) && Content.DataType.IsAssignableFrom(e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat).GetType())) {
        if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Link;  // ALT key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) e.Effect = DragDropEffects.Copy;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Move)) e.Effect = DragDropEffects.Move;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Link)) e.Effect = DragDropEffects.Link;
      }
    }
    protected virtual void valueGroupBox_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        T value = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as T;
        if (e.Effect.HasFlag(DragDropEffects.Copy)) value = (T)value.Clone();
        Content.Value = value;
      }
    }

    #region Helpers
    protected void SetDataTypeTextBoxText() {
      if (Content == null) {
        dataTypeTextBox.Text = "-";
      } else {
        if ((Content.Value != null) && (Content.Value.GetType() != Content.DataType))
          dataTypeTextBox.Text = Content.DataType.GetPrettyName() + " (" + Content.Value.GetType().GetPrettyName() + ")";
        else
          dataTypeTextBox.Text = Content.DataType.GetPrettyName();
      }
    }
    #endregion
  }
}
