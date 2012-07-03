#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.LawnMower.Views {
  [View("Lawn Mower Solution View")]
  [Content(typeof(Solution), IsDefaultView = true)]
  public sealed partial class SolutionLawnView : NamedItemView {
    public new Solution Content {
      get { return (Solution)base.Content; }
      set { base.Content = value; }
    }

    public SolutionLawnView() : base() {
      InitializeComponent();
      pictureBox.Image = new Bitmap(200, 200);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        using (var g = Graphics.FromImage(pictureBox.Image))
          g.Clear(DefaultBackColor);
      } else {
        bool[,] lawn = Interpreter.EvaluateLawnMowerProgram(Content.Length, Content.Width, Content.Tree);
        PaintTiles(pictureBox.Image, lawn);
      }
    }

    private void PaintTiles(Image lawnImage, bool[,] mowed) {
      int w = lawnImage.Width;
      int h = lawnImage.Height;

      float tileHeight = h / (float)mowed.GetLength(0);
      float tileWidth = w / (float)mowed.GetLength(1);

      tileWidth = Math.Min(tileWidth, tileHeight);
      tileHeight = tileWidth; // draw square tiles

      using (var g = Graphics.FromImage(lawnImage)) {
        g.Clear(DefaultBackColor);
        for (int i = 0; i < Content.Length; i++)
          for (int j = 0; j < Content.Width; j++)
            if (mowed[i, j]) {
              g.FillRectangle(Brushes.Chartreuse, tileWidth * j, tileHeight * i, tileWidth, tileHeight);
            } else {
              g.FillRectangle(Brushes.DarkGreen, tileWidth * j, tileHeight * i, tileWidth, tileHeight);
            }
      }
    }   
  }
}
