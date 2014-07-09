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

using System;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Clients.Hive.JobManager.Views {
  [View("Hive Job Permission List")]
  [Content(typeof(HiveItemCollection<JobPermission>), false)]
  public partial class HiveJobPermissionListView : ItemCollectionView<JobPermission> {

    private Guid hiveExperimentId;
    public Guid HiveExperimentId {
      get { return hiveExperimentId; }
      set { hiveExperimentId = value; }
    }

    public HiveJobPermissionListView() {
      InitializeComponent();
      itemsGroupBox.Text = "Permissions";
    }

    protected override JobPermission CreateItem() {
      return new JobPermission() { JobId = this.hiveExperimentId };
    }

    protected override void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<JobPermission> e) {
      base.Content_ItemsRemoved(sender, e);
      foreach (var item in e.Items) {
        if (item.GrantedUserId != Guid.Empty) {
          HiveClient.Delete(item);
        }
      }
    }
  }
}
