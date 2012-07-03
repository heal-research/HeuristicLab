#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// The visual representation of a <see cref="Variable"/>.
  /// </summary>
  [View("Variable View")]
  [Content(typeof(Variable), true)]
  [Content(typeof(IVariable), false)]
  public partial class VariableView : NamedItemView {
    protected TypeSelectorDialog typeSelectorDialog;

    /// <summary>
    /// Gets or sets the variable to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public new IVariable Content {
      get { return (IVariable)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with caption "Variable".
    /// </summary>
    public VariableView() {
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
    /// Removes the eventhandlers from the underlying <see cref="Variable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterContentEvents() {
      Content.ValueChanged -= new EventHandler(Content_ValueChanged);
      base.DeregisterContentEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="Variable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ValueChanged += new EventHandler(Content_ValueChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        dataTypeTextBox.Text = "-";
        viewHost.Content = null;
      } else {
        dataTypeTextBox.Text = Content.Value == null ? "-" : Content.Value.GetType().GetPrettyName();
        viewHost.ViewType = null;
        viewHost.Content = Content.Value;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      dataTypeTextBox.Enabled = Content != null && Content.Value != null;
      setValueButton.Enabled = Content != null && !ReadOnly;
      clearValueButton.Enabled = Content != null && Content.Value != null && !ReadOnly;
      valueGroupBox.Enabled = Content != null;
    }

    protected virtual void Content_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ValueChanged), sender, e);
      else {
        dataTypeTextBox.Text = Content.Value == null ? "-" : Content.Value.GetType().GetPrettyName();
        dataTypeTextBox.Enabled = Content.Value != null;
        clearValueButton.Enabled = Content.Value != null;
        viewHost.ViewType = null;
        viewHost.Content = Content.Value;
      }
    }

    protected virtual void setValueButton_Click(object sender, EventArgs e) {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.Caption = "Select Value";
        typeSelectorDialog.TypeSelector.Configure(typeof(IItem), false, true);
      }
      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          Content.Value = (IItem)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }
    protected virtual void clearValueButton_Click(object sender, EventArgs e) {
      Content.Value = null;
    }
    protected virtual void valuePanel_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (!ReadOnly && (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IItem)) {
        if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Link;  // ALT key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) e.Effect = DragDropEffects.Copy;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Move)) e.Effect = DragDropEffects.Move;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Link)) e.Effect = DragDropEffects.Link;
      }
    }
    protected virtual void valuePanel_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        IItem item = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as IItem;
        if (e.Effect.HasFlag(DragDropEffects.Copy)) item = (IItem)item.Clone();
        Content.Value = item;
      }
    }
  }
}
