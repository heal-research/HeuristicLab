#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Core.Views {
  public partial class CreateParameterDialog : Form {
    public TypeSelector ParameterTypeSelector {
      get { return parameterTypeSelector; }
    }
    public IParameter Parameter {
      get {
        try {
          IParameter parameter = (IParameter)Activator.CreateInstance(parameterTypeSelector.SelectedType, nameTextBox.Text, descriptionTextBox.Text);
          parameter.Hidden = false;
          return parameter;
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
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

    protected virtual void parameterTypeSelector_SelectedTypeChanged(object sender, EventArgs e) {
      okButton.Enabled = (parameterTypeSelector.SelectedType != null) && !parameterTypeSelector.SelectedType.ContainsGenericParameters;
    }
  }
}
