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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Views {
  [View("VRPProblemInstance View")]
  [Content(typeof(IVRPProblemInstance), true)]
  public abstract partial class VRPProblemInstanceView : ParameterizedNamedItemView {
    public new IVRPProblemInstance Content {
      get { return (IVRPProblemInstance)base.Content; }
      set { base.Content = value; }
    }

    private IVRPEncoding solution;
    public IVRPEncoding Solution {
      get {
        return solution;
      }
      set {
        solution = value;
        GenerateImage();
      }
    }

    public VRPProblemInstanceView() {
      InitializeComponent();
      Solution = null;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        pictureBox.Image = null;
      } else {
        GenerateImage();
      }
    }

    private void pictureBox_SizeChanged(object sender, EventArgs e) {
      GenerateImage();
    }

    protected virtual void pictureBox_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) {
    }

    protected abstract void DrawVisualization(Bitmap bitmap);

    protected List<Pen> GetColors(int count) {
      List<Pen> result = new List<Pen>();

      int i = 0;

      int r = 255;
      int g = 255;
      int b = 255;

      int step = Math.Max(1, 200 * 6 / count);

      while (result.Count != count) {
        switch (i) {
          case 0: result.Add(new Pen(Color.FromArgb(0, 0, b)));
            break;
          case 1: result.Add(new Pen(Color.FromArgb(0, g, 0)));
            break;
          case 2: result.Add(new Pen(Color.FromArgb(r, 0, 0)));
            break;
          case 3: result.Add(new Pen(Color.FromArgb(0, g, b)));
            break;
          case 4: result.Add(new Pen(Color.FromArgb(r, 0, b)));
            break;
          case 5: result.Add(new Pen(Color.FromArgb(r, g, 0)));
            break;
        }

        i++;
        if (i == 6) {
          i = 0;

          if (r >= step)
            r -= step;

          if (g >= step)
            g -= step;

          if (b >= step)
            b -= step;
        }
      }

      return result;
    }

    protected void GenerateImage() {
      if ((pictureBox.Width > 0) && (pictureBox.Height > 0)) {
        if (Content == null) {
          pictureBox.Image = null;
        } else {
          Bitmap bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);

          DrawVisualization(bitmap);

          pictureBox.Image = bitmap;
        }
      }
    }
  }
}
