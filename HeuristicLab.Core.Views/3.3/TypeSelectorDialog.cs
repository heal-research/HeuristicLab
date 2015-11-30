#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// A dialog to select a specific type.
  /// </summary>
  public partial class TypeSelectorDialog : Form {
    /// <summary>
    /// Gets or sets the caption of the dialog.
    /// </summary>
    /// <remarks>Uses property <see cref="Form.Text"/> of base class <see cref="Form"/>.
    /// No own data storage present.</remarks>
    public string Caption {
      get { return Text; }
      set {
        if (InvokeRequired)
          Invoke(new Action<string>(delegate(string s) { Caption = s; }), value);
        else
          Text = value;
      }
    }
    public TypeSelector TypeSelector {
      get { return typeSelector; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ChooseTypeDialog"/>.
    /// </summary>
    public TypeSelectorDialog() {
      InitializeComponent();
      okButton.Enabled = typeSelector.SelectedType != null;
    }

    protected virtual void TypeSelectorDialog_Load(object sender, EventArgs e) {
      this.typeSelector.TypesTreeView.DoubleClick += new System.EventHandler(TypesTreeView_DoubleClick);
    }
    protected virtual void typeSelector_SelectedTypeChanged(object sender, EventArgs e) {
      var selectedType = typeSelector.SelectedType;
      okButton.Enabled = selectedType != null && !selectedType.IsGenericTypeDefinition;
    }
    protected virtual void TypesTreeView_DoubleClick(object sender, System.EventArgs e) {
      if (typeSelector.SelectedType != null) {
        DialogResult = DialogResult.OK;
        Close();
      }
    }
  }
}
