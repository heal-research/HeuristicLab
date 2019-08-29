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
using HeuristicLab.Clients.Access;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Clients.Hive.Administrator.Views {
  [View("Resource View")]
  [Content(typeof(Resource), IsDefaultView = true)]
  public partial class ResourceView : ItemView {
    public new Resource Content {
      get { return (Resource)base.Content; }
      set { base.Content = value; }
    }

    public ResourceView() {
      InitializeComponent();
    }

    #region Overrides
    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        idTextBox.Clear();
        nameTextBox.Clear();
        descriptionTextBox.Clear();
        heartbeatIntervalNumericUpDown.Value = 0;
        publicCheckBox.Checked = false;
      } else {
        idTextBox.Text = Content.Id.ToString();
        nameTextBox.Text = Content.Name;
        descriptionTextBox.Text = Content.Description;
        heartbeatIntervalNumericUpDown.Value = Content.HbInterval;
        publicCheckBox.Checked = Content.OwnerUserId == null;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      bool enabled = Content != null && !Locked;
      idTextBox.ReadOnly = true;
      nameTextBox.ReadOnly = !enabled;
      descriptionTextBox.ReadOnly = !enabled;
      heartbeatIntervalNumericUpDown.ReadOnly = !enabled;
      publicCheckBox.Enabled = enabled;
    }
    #endregion

    #region Event Handlers
    private void nameTextBox_TextChanged(object sender, EventArgs e) {
      if (Content != null && Content.Name != nameTextBox.Text)
        Content.Name = nameTextBox.Text;
    }

    private void descriptionTextBox_TextChanged(object sender, EventArgs e) {
      if (Content != null && Content.Description != descriptionTextBox.Text)
        Content.Description = descriptionTextBox.Text;
    }

    private void heartbeatIntervalNumericUpDown_ValueChanged(object sender, EventArgs e) {
      if (Content != null && Content.HbInterval != (int)heartbeatIntervalNumericUpDown.Value)
        Content.HbInterval = (int)heartbeatIntervalNumericUpDown.Value;
    }

    private void publicCheckBox_CheckedChanged(object sender, EventArgs e) {
      var newOwnerUserId = publicCheckBox.Checked ? null : (Guid?)UserInformation.Instance.User.Id;
      if (Content != null && Content.OwnerUserId != newOwnerUserId)
        Content.OwnerUserId = newOwnerUserId;
    }
    #endregion
  }
}
