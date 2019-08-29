#region License Information
/* HeuristicLab
 * Copyright (C) Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.BinPacking3D;

namespace HeuristicLab.Problems.BinPacking.Views {
  [View("3-dimensional packing plan View")]
  [Content(typeof(PackingPlan<BinPacking3D.PackingPosition, PackingShape, PackingItem>), true)]
  public partial class PackingPlan3DView : ItemView {

    public PackingPlan3DView() {
      InitializeComponent();
    }

    public new PackingPlan<BinPacking3D.PackingPosition, PackingShape, PackingItem> Content {
      get { return (PackingPlan<BinPacking3D.PackingPosition, PackingShape, PackingItem>)base.Content; }
      set { base.Content = value; }
    }


    protected override void OnContentChanged() {
      base.OnContentChanged();
      binSelection.Items.Clear();
      if (Content == null) {
        packingPlan3D.Packing = null;
      } else {
        int i = 0;
        foreach (var bp in Content.Bins)
          binSelection.Items.Add(i++ + " (" + Math.Round(bp.PackingDensity * 100, 2) + "%)");

        binSelection.SelectedIndex = 0;
        ShowSelectedPacking();
      }
    }

    private void ShowSelectedPacking() {
      int currentBin = (binSelection != null) ? (int)(binSelection.SelectedIndex) : 0;
      packingPlan3D.Packing = Content.Bins[currentBin];
    }

    private void binSelection_SelectedIndexChanged(object sender, EventArgs e) {
      itemSelection.SelectedIndex = -1;
      itemSelection.Items.Clear();

      // add items of this container
      int currentBin = (binSelection != null) ? (int)(binSelection.SelectedIndex) : 0;
      var packing = Content.Bins[currentBin];
      foreach (var item in packing.Items) {
        itemSelection.Items.Add(item.Key);
      }

      ShowSelectedPacking();
    }

    private void itemSelection_SelectedIndexChanged(object sender, EventArgs e) {
      int selectedItem = -1;
      if ((itemSelection != null && itemSelection.SelectedItem != null) && Int32.TryParse(itemSelection.SelectedItem.ToString(), out selectedItem)) {
        packingPlan3D.SelectItem(selectedItem);
      } else
        packingPlan3D.ClearSelection();
    }
  }
}
