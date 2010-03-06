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
using HeuristicLab.MainForm;

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
      Caption = "Variable";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with the given <paramref name="variable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="VariableView()"/>.</remarks>
    /// <param name="variable">The variable to represent visually.</param>
    public VariableView(IVariable content)
      : this() {
      Content = content;
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
        Caption = "Variable";
        dataTypeTextBox.Text = "-";
        dataTypeTextBox.Enabled = false;
        setValueButton.Enabled = false;
        clearValueButton.Enabled = false;
        valueGroupBox.Enabled = false;
        viewHost.Content = null;
      } else {
        Caption = Content.Name + " (" + Content.GetType().Name + ")";
        dataTypeTextBox.Text = Content.Value == null ? "-" : Content.Value.GetType().GetPrettyName();
        dataTypeTextBox.Enabled = Content.Value != null;
        setValueButton.Enabled = true;
        clearValueButton.Enabled = Content.Value != null;
        valueGroupBox.Enabled = true;
        viewHost.Content = Content.Value;
      }
    }

    protected virtual void Content_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ValueChanged), sender, e);
      else {
        dataTypeTextBox.Text = Content.Value == null ? "-" : Content.Value.GetType().GetPrettyName();
        dataTypeTextBox.Enabled = Content.Value != null;
        clearValueButton.Enabled = Content.Value != null;
        viewHost.Content = Content.Value;
      }
    }

    protected virtual void setValueButton_Click(object sender, EventArgs e) {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.Caption = "Select Value Type";
        typeSelectorDialog.TypeSelector.Configure(typeof(IItem), false, false);
      }
      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        Content.Value = (IItem)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
      }
    }
    protected virtual void clearValueButton_Click(object sender, EventArgs e) {
      Content.Value = null;
    }
    protected virtual void valuePanel_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      Type type = e.Data.GetData("Type") as Type;
      if ((type != null) && (typeof(IItem).IsAssignableFrom(type))) {
        if ((e.KeyState & 8) == 8) e.Effect = DragDropEffects.Copy;  // CTRL key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if ((e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link) e.Effect = DragDropEffects.Link;
        else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) e.Effect = DragDropEffects.Copy;
        else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move) e.Effect = DragDropEffects.Move;
      }
    }
    protected virtual void valuePanel_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        IItem item = e.Data.GetData("Value") as IItem;
        if ((e.Effect & DragDropEffects.Copy) == DragDropEffects.Copy) item = (IItem)item.Clone();
        Content.Value = item;
      }
    }
  }
}
