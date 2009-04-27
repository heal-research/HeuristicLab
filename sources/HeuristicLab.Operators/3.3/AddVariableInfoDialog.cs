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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Operators {
  /// <summary>
  /// Dialog to add a variable info.
  /// </summary>
  public partial class AddVariableInfoDialog : Form {
    private IVariableInfo myVariableInfo;
    /// <summary>
    /// Gets the current variable info.
    /// </summary>
    public IVariableInfo VariableInfo {
      get { return myVariableInfo; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="AddVariableInfoDialog"/>.
    /// </summary>
    public AddVariableInfoDialog() {
      InitializeComponent();
      dataTypeTextBox.Text = typeof(BoolData).FullName;
      dataTypeTextBox.Tag = typeof(BoolData);
    }

    private void formalNameTextBox_Validating(object sender, CancelEventArgs e) {
      if (formalNameTextBox.Text == "")
        e.Cancel = true;
    }

    private void setDataTypeButton_Click(object sender, EventArgs e) {
      ChooseTypeDialog dialog = new ChooseTypeDialog();
      dialog.Caption = "Set Data Type";
      if (dialog.ShowDialog(this) == DialogResult.OK) {
        dataTypeTextBox.Text = dialog.Type.FullName;
        dataTypeTextBox.Tag = dialog.Type;
      }
      dialog.Dispose();
    }

    private void okButton_Click(object sender, EventArgs e) {
      VariableKind kind = VariableKind.None;
      if (kindNewCheckBox.Checked) kind = kind | VariableKind.New;
      if (kindInCheckBox.Checked) kind = kind | VariableKind.In;
      if (kindOutCheckBox.Checked) kind = kind | VariableKind.Out;
      if (kindDeletedCheckBox.Checked) kind = kind | VariableKind.Deleted;

      myVariableInfo = new VariableInfo(formalNameTextBox.Text, descriptionTextBox.Text, (Type)dataTypeTextBox.Tag, kind);
    }
  }
}
