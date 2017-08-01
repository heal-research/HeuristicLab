#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Clients.OKB.Administration {
  [View("NamedOKBItem View")]
  [Content(typeof(NamedOKBItem), true)]
  [Content(typeof(INamedOKBItem), false)]
  public partial class NamedOKBItemView : OKBItemView {
    public new INamedOKBItem Content {
      get { return (INamedOKBItem)base.Content; }
      set { base.Content = value; }
    }

    public NamedOKBItemView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        nameTextBox.Text = string.Empty;
        descriptionTextBox.Text = string.Empty;
        toolTip.SetToolTip(descriptionTextBox, string.Empty);
        if (ViewAttribute.HasViewAttribute(this.GetType()))
          this.Caption = ViewAttribute.GetViewName(this.GetType());
        else
          this.Caption = "NamedOKBItem View";
      } else {
        nameTextBox.Text = Content.Name;
        descriptionTextBox.Text = Content.Description;
        toolTip.SetToolTip(descriptionTextBox, Content.Description);
        Caption = Content.Name;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      nameTextBox.Enabled = Content != null;
      nameTextBox.ReadOnly = ReadOnly;
      descriptionTextBox.Enabled = Content != null;
      descriptionTextBox.ReadOnly = ReadOnly;
    }

    protected override void OnContentPropertyChanged(string propertyName) {
      switch (propertyName) {
        case "Name":
          nameTextBox.Text = Content.Name;
          Caption = Content.Name;
          break;
        case "Description":
          descriptionTextBox.Text = Content.Description;
          toolTip.SetToolTip(descriptionTextBox, Content.Description);
          break;
      }
    }

    protected virtual void nameTextBox_TextChanged(object sender, EventArgs e) {
      if (nameTextBox.Text != Content.Name)
        Content.Name = nameTextBox.Text;
    }
    protected virtual void descriptionTextBox_TextChanged(object sender, EventArgs e) {
      if (descriptionTextBox.Text != Content.Description)
        Content.Description = descriptionTextBox.Text;
    }

    protected void descriptionTextBox_DoubleClick(object sender, EventArgs e) {
      using (TextDialog dialog = new TextDialog("Description of " + Content.Name, descriptionTextBox.Text, ReadOnly)) {
        if (dialog.ShowDialog(this) == DialogResult.OK)
          Content.Description = dialog.Content;
      }
    }
  }
}
