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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

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
      Caption = "ValueParameter";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with the given <paramref name="variable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="VariableView()"/>.</remarks>
    /// <param name="variable">The variable to represent visually.</param>
    public ValueParameterView(IValueParameter<T> content)
      : this() {
      Content = content;
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="IVariable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterContentEvents() {
      Content.ValueChanged -= new EventHandler(Content_ValueChanged);
      base.DeregisterContentEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="IVariable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ValueChanged += new EventHandler(Content_ValueChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        Caption = "ValueParameter";
        clearValueButton.Visible = true;
        viewHost.Content = null;
      } else {
        Caption = Content.Name + " (" + Content.GetType().Name + ")";
        clearValueButton.Visible = !(Content is ValueParameter<T>);
        viewHost.ViewType = null;
        viewHost.Content = Content.Value;
      }
      SetEnabledStateOfControls();
    }

    protected override void OnReadOnlyChanged() {
      base.OnReadOnlyChanged();
      SetEnabledStateOfControls();
    }

    private void SetEnabledStateOfControls() {
      setValueButton.Enabled = Content != null && !ReadOnly;
      clearValueButton.Enabled = Content != null && Content.Value != null && !ReadOnly;
      valueGroupBox.Enabled = Content != null;
    }

    protected virtual void Content_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ValueChanged), sender, e);
      else {
        clearValueButton.Enabled = Content.Value != null && !ReadOnly;
        viewHost.ViewType = null;
        viewHost.Content = Content.Value;
      }
    }

    protected virtual void changeValueButton_Click(object sender, EventArgs e) {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.Caption = "Select Value";
        typeSelectorDialog.TypeSelector.Configure(Content.DataType, false, false);
      }
      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          Content.Value = (T)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
        }
        catch (Exception ex) {
          Auxiliary.ShowErrorMessageBox(ex);
        }
      }
    }
    protected virtual void setValueButton_Click(object sender, EventArgs e) {
      Content.Value = null;
    }
    protected virtual void valuePanel_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      Type type = e.Data.GetData("Type") as Type;
      if (!ReadOnly && (type != null) && (Content.DataType.IsAssignableFrom(type))) {
        if ((e.KeyState & 8) == 8) e.Effect = DragDropEffects.Copy;  // CTRL key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if ((e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link) e.Effect = DragDropEffects.Link;
        else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) e.Effect = DragDropEffects.Copy;
        else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move) e.Effect = DragDropEffects.Move;
      }
    }
    protected virtual void valuePanel_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        T value = e.Data.GetData("Value") as T;
        if ((e.Effect & DragDropEffects.Copy) == DragDropEffects.Copy) value = (T)value.Clone();
        Content.Value = value;
      }
    }
  }
}
