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

namespace HeuristicLab.Core.Views {
  public partial class CreateParameterDialog : Form {
    protected TypeSelectorDialog typeSelectorDialog;

    public TypeSelector ParameterTypeSelector {
      get { return parameterTypeSelector; }
    }
    public IParameter Parameter {
      get {
        try {
          Type type = parameterTypeSelector.SelectedType;
          if (type.ContainsGenericParameters) {
            type = type.MakeGenericType((Type)dataTypeTextBox.Tag);
          }
          return (IParameter)Activator.CreateInstance(type, nameTextBox.Text, descriptionTextBox.Text);
        }
        catch (Exception ex) {
          Auxiliary.ShowErrorMessageBox(ex);
        }
        return null;
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="AddVariableInfoDialog"/>.
    /// </summary>
    public CreateParameterDialog() {
      InitializeComponent();
      parameterTypeSelector.Configure(typeof(IParameter), false, true);
    }

    protected virtual void setDataTypeButton_Click(object sender, EventArgs e) {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.Caption = "Select Data Type";
        typeSelectorDialog.TypeSelector.Configure(typeof(IItem), true, true);
      }

      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        dataTypeTextBox.Text = typeSelectorDialog.TypeSelector.SelectedType.FullName;
        dataTypeTextBox.Tag = typeSelectorDialog.TypeSelector.SelectedType;
      }
      okButton.Enabled = dataTypeTextBox.Tag != null;
    }

    protected virtual void parameterTypeSelector_SelectedTypeChanged(object sender, EventArgs e) {
      dataTypeTextBox.Text = "-";
      dataTypeTextBox.Tag = null;
      dataTypeTextBox.Enabled = false;
      setDataTypeButton.Enabled = false;
      okButton.Enabled = false;

      if (parameterTypeSelector.SelectedType != null) {
        if (parameterTypeSelector.SelectedType.ContainsGenericParameters) {
          dataTypeTextBox.Enabled = true;
          setDataTypeButton.Enabled = true;
        } else {
          okButton.Enabled = true;
        }
      }
    }
  }
}
