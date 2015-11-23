#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.GeneticProgramming.Robocode;

namespace HeuristicLab.Problems.GeneticProgramming.Views.Robocode {
  [View("EnemyCollection View")]
  [Content(typeof(EnemyCollection), true)]
  public partial class EnemyCollectionView : CheckedItemListView<StringValue> {
    public new EnemyCollection Content {
      get { return (EnemyCollection)base.Content; }
      set { base.Content = value; }
    }

    public EnemyCollectionView() {
      InitializeComponent();
    }

    private void reloadButton_Click(object sender, System.EventArgs e) {
      Content.Clear();
      Content.AddRange(EnemyCollection.ReloadEnemies(Content.RobocodePath));
      foreach (var robot in Content) {
        Content.SetItemCheckedState(robot, false);
      }
    }
  }
}
