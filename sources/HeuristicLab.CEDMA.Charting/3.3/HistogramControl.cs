#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Charting;

namespace HeuristicLab.CEDMA.Charting {
  public partial class HistogramControl : UserControl {
    private Bitmap bitmap;
    private bool renderingRequired;

    private Histogram myChart;
    public Histogram Chart {
      get { return myChart; }
      set {
        if(myChart != null) myChart.Update -= new EventHandler(myChart_Update);
        myChart = value;
        if(myChart != null) {
          myChart.Update += new EventHandler(myChart_Update);
        }
        GenerateImage();
      }
    }

    public HistogramControl() {
      InitializeComponent();
      GenerateImage();
    }

    private void myChart_Update(object sender, EventArgs e) {
      GenerateImage();
    }

    private void pictureBox_SizeChanged(object sender, EventArgs e) {
      GenerateImage();
    }
    private void pictureBox_VisibleChanged(object sender, EventArgs e) {
      if(pictureBox.Visible && renderingRequired) {
        GenerateImage();
      }
    }

    private void pictureBox_MouseMove(object sender, MouseEventArgs e) {
      toolTip.SetToolTip(pictureBox, Chart.GetToolTipText(e.Location));
    }

    private void GenerateImage() {
      if(!Visible) {
        renderingRequired = true;
      } else {
        if((pictureBox.Width == 0) || (pictureBox.Height == 0)) {
          bitmap = null;
        } else {
          bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
          if(Chart != null) {
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SetClip(new System.Drawing.Rectangle(0, 0, pictureBox.Width, pictureBox.Height));
            Chart.Render(graphics, pictureBox.Width, pictureBox.Height);
            graphics.Dispose();
          }
        }
        pictureBox.Image = bitmap;
        renderingRequired = false;
      }
    }

    private void pictureBox_MouseClick(object sender, MouseEventArgs e) {
      Chart.MouseClick(e.Location, e.Button);
    }
  }
}
