#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public partial class ItemWizardForm : Form {
    private string itemName;
    public string ItemName {
      get { return itemName; }
      set {
        itemName = value;
        if (nameTextBox.Text != itemName)
          nameTextBox.Text = itemName;
      }
    }
    private string itemDescription;
    public string ItemDescription {
      get { return itemDescription; }
      set {
        itemDescription = value;
        if (descriptionTextBox.Text != itemDescription)
          descriptionTextBox.Text = itemDescription;
      }
    }

    public ItemWizardForm() {
      InitializeComponent();
      ItemName = nameTextBox.Text;
      ItemDescription = descriptionTextBox.Text;
    }

    private void finishButton_Click(object sender, System.EventArgs e) {
      Close();
    }

    private void cancelButton_Click(object sender, System.EventArgs e) {
      Close();
    }

    private void contentTypeTextBox_TextChanged(object sender, System.EventArgs e) {
      ItemDescription = descriptionTextBox.Text;
    }

    private void nameTextBox_TextChanged(object sender, System.EventArgs e) {
      ItemName = nameTextBox.Text;
    }
  }
}
