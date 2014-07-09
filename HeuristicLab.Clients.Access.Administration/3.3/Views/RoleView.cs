#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Clients.Access.Administration {
  [View("Role View")]
  [Content(typeof(Role), true)]
  public partial class RoleView : ItemView {
    public new Role Content {
      get { return (Role)base.Content; }
      set { base.Content = value; }
    }

    public RoleView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();

      if (Content == null) {
        roleNameTextBox.Clear();
      } else {
        roleNameTextBox.Text = Content.Name;
      }
    }

    private void roleNameTextBox_TextChanged(object sender, System.EventArgs e) {
      if (Content.Name != roleNameTextBox.Text)
        Content.Name = roleNameTextBox.Text;
    }
  }
}
