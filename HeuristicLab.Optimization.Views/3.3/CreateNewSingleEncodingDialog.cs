#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimization.Views {
  public partial class CreateNewSingleEncodingDialog : Form {
    private HashSet<string> forbiddenNames;
    public IEnumerable<string> ForbiddenNames {
      get { return forbiddenNames; }
      set { forbiddenNames = new HashSet<string>(value); }
    }

    public Type EncodingType { get; private set; }
    public string EncodingName { get; private set; }

    public CreateNewSingleEncodingDialog() {
      InitializeComponent();
      forbiddenNames = new HashSet<string>();
    }

    private void CreateNewSingleEncodingDialog_Load(object sender, EventArgs e) {
      encodingTypeComboBox.Items.Clear();
      var encodings = ApplicationManager.Manager.GetTypes(typeof(IEncoding));
      foreach (var enc in encodings) {
        if (enc != typeof(CombinedEncoding))
          encodingTypeComboBox.Items.Add(enc);
      }
      if (encodingTypeComboBox.Items.Count > 0)
        encodingTypeComboBox.SelectedIndex = 0;
      else encodingTypeComboBox.SelectedIndex = -1;
    }

    private void encodingNameTextBox_TextChanged(object sender, EventArgs e) {
      EncodingName = encodingNameTextBox.Text;
      SetEnabledStateOfControls();
    }

    private void encodingTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (encodingTypeComboBox.SelectedIndex >= 0)
        EncodingType = (Type)encodingTypeComboBox.SelectedItem;
      else EncodingType = null;
      SetEnabledStateOfControls();
    }

    private void SetEnabledStateOfControls() {
      okButton.Enabled = EncodingType != null
                         && !string.IsNullOrEmpty(EncodingName)
                         && !forbiddenNames.Contains(EncodingName);
    }

    private void okButton_Click(object sender, EventArgs e) {
      DialogResult = DialogResult.OK;
      Close();
    }

    private void cancelButton_Click(object sender, EventArgs e) {
      DialogResult = DialogResult.Cancel;
      Close();
    }
  }
}
