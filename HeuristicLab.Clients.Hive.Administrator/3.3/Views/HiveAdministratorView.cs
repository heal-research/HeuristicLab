#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Clients.Hive.Administrator.Views {
  [View("Hive Administrator")]
  [Content(typeof(HiveAdminClient), IsDefaultView = true)]
  public partial class HiveAdministratorView : AsynchronousContentView {
    public new HiveAdminClient Content {
      get { return (HiveAdminClient)base.Content; }
      set { base.Content = value; }
    }

    public HiveAdministratorView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        projectsView.Content = Content.Projects;
        resourcesView.Content = Content.Resources;
      } else {
        projectsView.Content = null;
        resourcesView.Content = null;
      }
    }
  }
}
