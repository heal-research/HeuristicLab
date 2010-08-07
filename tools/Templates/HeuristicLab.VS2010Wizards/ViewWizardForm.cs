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

using System.Windows.Forms;

namespace HeuristicLab.VS2010Wizards {
  public partial class ViewWizardForm : Form {
    public string BaseClass {
      get;
      private set;
    }
    public string ViewContentType {
      get;
      private set;
    }
    public bool IsDefaultView {
      get;
      private set;
    }

    public ViewWizardForm() {
      InitializeComponent();
      BaseClass = baseClassTextBox.Text;
      ViewContentType = viewContentTypeTextBox.Text;
      IsDefaultView = isDefaultViewCheckBox.Checked;
    }

    private void okButton_Click(object sender, System.EventArgs e) {
      DialogResult = System.Windows.Forms.DialogResult.OK;
      Close();
    }

    private void cancelButton_Click(object sender, System.EventArgs e) {
      DialogResult = System.Windows.Forms.DialogResult.Cancel;
      Close();
    }

    private void baseClassTextBox_TextChanged(object sender, System.EventArgs e) {
      BaseClass = baseClassTextBox.Text;
    }

    private void viewContentTypeTextBox_TextChanged(object sender, System.EventArgs e) {
      ViewContentType = viewContentTypeTextBox.Text;
    }

    private void isDefaultViewCheckBox_CheckedChanged(object sender, System.EventArgs e) {
      IsDefaultView = isDefaultViewCheckBox.Checked;
    }
  }
}
