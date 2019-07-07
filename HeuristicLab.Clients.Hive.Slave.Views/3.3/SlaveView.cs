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
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;


namespace HeuristicLab.Clients.Hive.SlaveCore.Views {

  [View("HeuristicLab Slave View")]
  [Content(typeof(SlaveItem), IsDefaultView = false)]
  public partial class SlaveView : ItemView {

    public SlaveView() {
      InitializeComponent();
    }

    #region Register Content Events
    protected override void DeregisterContentEvents() {

      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();


    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        slaveStats.Content = null;
        jobsView.Content = null;
      } else {
        slaveStats.Content = (SlaveItem)Content;
        jobsView.Content = (SlaveItem)Content;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
    }
  }
}
