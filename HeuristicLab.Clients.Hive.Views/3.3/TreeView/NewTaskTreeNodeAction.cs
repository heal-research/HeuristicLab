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

using System.Drawing;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.Clients.Hive {
  public class NewTaskTreeNodeAction : IItemTreeNodeAction<HiveTask> {
    private ItemCollection<HiveTask> hiveJobs;

    public NewTaskTreeNodeAction(ItemCollection<HiveTask> hiveJobs) {
      this.hiveJobs = hiveJobs;
    }

    public string Name {
      get { return "New Task..."; }
    }

    public Image Image {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.NewDocument; }
    }

    public void Execute(HiveTask item, HiveTask parentItem) {
      if (item != null) {
        var experiment = item.ItemTask.Item as Experiment;
        experiment.Optimizers.Add(new Experiment());
      } else {
        hiveJobs.Add(new OptimizerHiveTask(new Experiment()));
      }
    }
  }
}
