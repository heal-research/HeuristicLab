#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.BinPacking2D;

namespace HeuristicLab.Problems.BinPacking.Views {
  [View("2-dimensional packing plan view")]
  [Content(typeof(PackingPlan<BinPacking2D.PackingPosition, PackingShape, PackingItem>), true)]
  public partial class PackingPlan2DView : ItemView {

    public PackingPlan2DView() {
      InitializeComponent();
    }

    public new PackingPlan<BinPacking2D.PackingPosition, PackingShape, PackingItem> Content {
      get { return (PackingPlan<BinPacking2D.PackingPosition, PackingShape, PackingItem>)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      binSelection.Items.Clear();
      if (Content == null) {
        ClearState();
      } else {
        for (int i = 0; i < Content.NrOfBins; i++)
          binSelection.Items.Add(i);
        UpdateState(Content);
      }
    }

    private void ClearState() {
      container2DView.Packing = null;
    }

    private void UpdateState(PackingPlan<BinPacking2D.PackingPosition, PackingShape, PackingItem> plan) {
      int currentBin = (binSelection != null && binSelection.SelectedItem != null) ? (int)(binSelection.SelectedItem) : 0;
      container2DView.Packing = plan.Bins[currentBin];
    }

    private void binSelection_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateState(Content);
    }
  }
}
