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
using System.ComponentModel;
using System.Windows.Forms;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  [View("NamedItem View")]
  [Content(typeof(NamedItem), false)]
  [Content(typeof(INamedItem), false)]
  public partial class NamedItemView : ItemView {
    private const string infoLabelToolTipSuffix = "Double-click to open description editor.";

    public new INamedItem Content {
      get { return (INamedItem)base.Content; }
      set { base.Content = value; }
    }

    public NamedItemView() {
      InitializeComponent();
      errorProvider.SetIconAlignment(nameTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(nameTextBox, 2);
    }

    protected override void DeregisterContentEvents() {
      Content.NameChanged -= new EventHandler(Content_NameChanged);
      Content.DescriptionChanged -= new EventHandler(Content_DescriptionChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.NameChanged += new EventHandler(Content_NameChanged);
      Content.DescriptionChanged += new EventHandler(Content_DescriptionChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        nameTextBox.Text = string.Empty;
        toolTip.SetToolTip(infoLabel, string.Empty);
        if (ViewAttribute.HasViewAttribute(this.GetType()))
          this.Caption = ViewAttribute.GetViewName(this.GetType());
        else
          this.Caption = "NamedItem View";
      } else {
        nameTextBox.Text = Content.Name;
        toolTip.SetToolTip(infoLabel, string.IsNullOrEmpty(Content.Description) ? infoLabelToolTipSuffix : Content.Description + Environment.NewLine + Environment.NewLine + infoLabelToolTipSuffix);
        Caption = Content.Name;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      if (Content == null) {
        nameTextBox.Enabled = false;
        infoLabel.Enabled = false;
      } else {
        nameTextBox.Enabled = true;
        nameTextBox.ReadOnly = ReadOnly || !Content.CanChangeName;
        infoLabel.Enabled = true;
      }
    }

    protected virtual void Content_NameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_NameChanged), sender, e);
      else {
        nameTextBox.Text = Content.Name;
        Caption = Content.Name;
      }
    }
    protected virtual void Content_DescriptionChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_DescriptionChanged), sender, e);
      else {
        toolTip.SetToolTip(infoLabel, string.IsNullOrEmpty(Content.Description) ? infoLabelToolTipSuffix : Content.Description + Environment.NewLine + Environment.NewLine + infoLabelToolTipSuffix);
      }
    }

    protected virtual void nameTextBox_Validating(object sender, CancelEventArgs e) {
      if ((Content != null) && (Content.CanChangeName)) {
        if (string.IsNullOrEmpty(nameTextBox.Text)) {
          e.Cancel = true;
          errorProvider.SetError(nameTextBox, "Name cannot be empty");
          nameTextBox.SelectAll();
          return;
        }
        Content.Name = nameTextBox.Text;
        // check if variable name was set successfully
        if (!Content.Name.Equals(nameTextBox.Text)) {
          e.Cancel = true;
          errorProvider.SetError(nameTextBox, "Invalid Name");
          nameTextBox.SelectAll();
        }
      }
    }
    protected virtual void nameTextBox_Validated(object sender, EventArgs e) {
      errorProvider.SetError(nameTextBox, string.Empty);
    }
    protected virtual void nameTextBox_KeyDown(object sender, KeyEventArgs e) {
      if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
        nameLabel.Focus();  // set focus on label to validate data
      if (e.KeyCode == Keys.Escape) {
        nameTextBox.Text = Content.Name;
        nameLabel.Focus();  // set focus on label to validate data
      }
    }
    protected virtual void infoLabel_DoubleClick(object sender, EventArgs e) {
      using (TextDialog dialog = new TextDialog("Description of " + Content.Name, Content.Description, ReadOnly || !Content.CanChangeDescription)) {
        if (dialog.ShowDialog(this) == DialogResult.OK)
          Content.Description = dialog.Content;
      }
    }
  }
}
