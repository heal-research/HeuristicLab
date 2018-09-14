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
using System.Windows;
using System.Windows.Media;
using HeuristicLab.Problems.BinPacking2D;
using Point = System.Windows.Point;
using Size = System.Windows.Size;
using UserControl = System.Windows.Controls.UserControl;

namespace HeuristicLab.Problems.BinPacking.Views {
  public partial class Container2DView : UserControl {
    private int selectedItemKey;
    private Size renderSize;
    private BinPacking<BinPacking2D.PackingPosition, PackingShape, PackingItem> packing;
    public BinPacking<BinPacking2D.PackingPosition, PackingShape, PackingItem> Packing {
      get { return packing; }
      set {
        if (packing != value) {
          this.packing = value;
          ClearSelection(); // also updates visualization
        }
      }
    }

    public Container2DView() {
      InitializeComponent();
    }


    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
      base.OnRenderSizeChanged(sizeInfo);
      var s = Math.Min(sizeInfo.NewSize.Height, sizeInfo.NewSize.Width);
      renderSize = new Size(s, s);
      InvalidateVisual();
    }

    // similar implementation to Container3DView (even though item selection is not supported for 2d packings TODO)
    public void SelectItem(int itemKey) {
      // selection of an item should make all other items semi-transparent
      selectedItemKey = itemKey;
      InvalidateVisual();
    }
    public void ClearSelection() {
      // remove all transparency
      selectedItemKey = -1;
      InvalidateVisual();
    }

    protected override void OnRender(DrawingContext drawingContext) {
      base.OnRender(drawingContext);
      if (packing == null) return;
      // the container should fill the whole size
      var scalingX = renderSize.Width / Packing.BinShape.Width;
      var scalingY = renderSize.Height / Packing.BinShape.Width;
      // draw container
      drawingContext.DrawRectangle(Brushes.LightGray, new Pen(Brushes.Black, 1), new Rect(new Point(0, 0), renderSize));

      var selectedBrush = Brushes.MediumSeaGreen;
      var unselectedBrush = selectedItemKey < 0 ? selectedBrush : Brushes.DarkGray;

      foreach (var t in Packing.Items) {
        var key = t.Key;
        var item = t.Value;
        var pos = Packing.Positions[key];

        var scaledPos = new Point(pos.X * scalingX, pos.Y * scalingY);

        var scaledSize = pos.Rotated ?
          new Size(item.Height * scalingX, item.Width * scalingY) :
          new Size(item.Width * scalingX, item.Height * scalingY);

        var brush = key == selectedItemKey ? selectedBrush : unselectedBrush;
        drawingContext.DrawRectangle(brush, new Pen(Brushes.Black, 1), new Rect(scaledPos, scaledSize));
      }
    }
  }
}
