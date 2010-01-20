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
using System.Windows.Forms;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// The visual representation of a <see cref="Parameter"/>.
  /// </summary>
  [Content(typeof(Parameter), true)]
  public partial class ParameterView : ParameterBaseView {
    private TypeSelectorDialog typeSelectorDialog;

    /// <summary>
    /// Gets or sets the variable to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public new Parameter Parameter {
      get { return (Parameter)Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with caption "Variable".
    /// </summary>
    public ParameterView() {
      InitializeComponent();
      Caption = "Parameter";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with the given <paramref name="variable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="VariableView()"/>.</remarks>
    /// <param name="variable">The variable to represent visually.</param>
    public ParameterView(Parameter parameter)
      : this() {
      Parameter = parameter;
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="IVariable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterObjectEvents() {
      Parameter.ActualNameChanged -= new EventHandler(Parameter_ActualNameChanged);
      Parameter.ValueChanged -= new EventHandler(Parameter_ValueChanged);
      base.DeregisterObjectEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="IVariable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterObjectEvents() {
      base.RegisterObjectEvents();
      Parameter.ActualNameChanged += new EventHandler(Parameter_ActualNameChanged);
      Parameter.ValueChanged += new EventHandler(Parameter_ValueChanged);
    }

    protected override void OnObjectChanged() {
      base.OnObjectChanged();
      if (Parameter == null) {
        Caption = "Parameter";
        actualNameTextBox.Text = "-";
        actualNameTextBox.Enabled = false;
        setValueButton.Enabled = false;
        clearValueButton.Enabled = false;
        valueGroupBox.Enabled = false;
        viewHost.Object = null;
      } else {
        Caption = Parameter.Name + " (" + Parameter.GetType().Name + ")";
        actualNameTextBox.Text = Parameter.ActualName;
        actualNameTextBox.Enabled = Parameter.Value == null;
        setValueButton.Enabled = Parameter.Value == null;
        clearValueButton.Enabled = Parameter.Value != null;
        valueGroupBox.Enabled = true;
        viewHost.Object = Parameter.Value;
      }
    }

    private void Parameter_ActualNameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Parameter_ActualNameChanged), sender, e);
      else
        actualNameTextBox.Text = Parameter.ActualName;
    }
    private void Parameter_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Parameter_ValueChanged), sender, e);
      else {
        actualNameTextBox.Enabled = Parameter.Value == null;
        setValueButton.Enabled = Parameter.Value == null;
        clearValueButton.Enabled = Parameter.Value != null;
        viewHost.Object = Parameter.Value;
      }
    }

    private void actualNameTextBox_Validated(object sender, EventArgs e) {
      Parameter.ActualName = actualNameTextBox.Text;
    }
    private void setValueButton_Click(object sender, EventArgs e) {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.Caption = "Select Value Type";
      }
      typeSelectorDialog.TypeSelector.Configure(Parameter.DataType, false, false);
      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK)
        Parameter.Value = (IItem)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
    }
    private void clearValueButton_Click(object sender, EventArgs e) {
      Parameter.Value = null;
    }
    private void valuePanel_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      Type type = e.Data.GetData("Type") as Type;
      if ((type != null) && (Parameter.DataType.IsAssignableFrom(type))) {
        if ((e.KeyState & 8) == 8) e.Effect = DragDropEffects.Copy;  // CTRL key
        else if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Move;  // ALT key
        else e.Effect = DragDropEffects.Link;
      }
    }
    private void valuePanel_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        IItem item = e.Data.GetData("Value") as IItem;
        if ((e.Effect & DragDropEffects.Copy) == DragDropEffects.Copy) item = (IItem)item.Clone();
        Parameter.Value = item;
      }
    }
  }
}
