#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimizer.Properties;

namespace HeuristicLab.Optimizer {
  public enum OptimizerMainFormTypes {
    DockingMainForm,
    MultipleDocumentMainForm,
    SingleDocumentMainForm
  }

  public partial class MainFormTypeSelectionDialog : Form {
    public MainFormTypeSelectionDialog() {
      InitializeComponent();
    }

    private void okButton_Click(object sender, EventArgs e) {
      if (rbDockingMainForm.Checked) {
        Settings.Default.MainFormType = OptimizerMainFormTypes.DockingMainForm;
      } else if (rbMultipleDocumentMainForm.Checked) {
        Settings.Default.MainFormType = OptimizerMainFormTypes.MultipleDocumentMainForm;
      } else if (rbSingleDocumentMainForm.Checked) {
        Settings.Default.MainFormType = OptimizerMainFormTypes.SingleDocumentMainForm;
      }
      Settings.Default.Save();
    }

    private void MainFormTypeSelectionDialog_Load(object sender, EventArgs e) {
      if (Settings.Default.MainFormType == OptimizerMainFormTypes.DockingMainForm) {
        rbDockingMainForm.Checked = true;
      } else if (Settings.Default.MainFormType == OptimizerMainFormTypes.MultipleDocumentMainForm) {
        rbMultipleDocumentMainForm.Checked = true;
      } else if (Settings.Default.MainFormType == OptimizerMainFormTypes.SingleDocumentMainForm) {
        rbSingleDocumentMainForm.Checked = true;
      }
    }
  }
}
