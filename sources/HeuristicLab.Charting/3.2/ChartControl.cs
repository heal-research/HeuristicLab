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

namespace HeuristicLab.Charting {
  public partial class ChartControl : UserControl {
    private Bitmap bitmap;
    private bool renderingRequired;
    private Point buttonDownPoint;
    private Point mousePosition;
    private int mouseClickCount;

    private Chart myChart;
    public Chart Chart {
      get { return myChart; }
      set {
        if (myChart != null) myChart.Update -= new EventHandler(myChart_Update);
        myChart = value;
        if (myChart != null) {
          myChart.Update += new EventHandler(myChart_Update);
          SetMode(Chart.Mode);
        }
        GenerateImage();
      }
    }
    private bool myScaleOnResize;
    public bool ScaleOnResize {
      get { return myScaleOnResize; }
      set { myScaleOnResize = value; }
    }

    public ChartControl() {
      InitializeComponent();
      myScaleOnResize = true;
      GenerateImage();
    }

    private void myChart_Update(object sender, EventArgs e) {
      GenerateImage();
    }
    private void pictureBox_SizeChanged(object sender, EventArgs e) {
      if (ScaleOnResize) {
        if ((pictureBox.Width > 0) && (pictureBox.Height > 0) && (Chart != null)) {
          PointD point = Chart.TransformPixelToWorld(new Point(pictureBox.Width, Chart.SizeInPixels.Height - pictureBox.Height));
          Chart.SetPosition(Chart.LowerLeft, point);
        }
      }
      GenerateImage();
    }
    private void pictureBox_VisibleChanged(object sender, EventArgs e) {
      if (pictureBox.Visible && renderingRequired) {
        GenerateImage();
      }
    }

    private void pictureBox_MouseDown(object sender, MouseEventArgs e) {
      buttonDownPoint = e.Location;
      mouseClickCount = e.Clicks;
    }
    private void pictureBox_MouseUp(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Left) {
        if (Chart.Mode == ChartMode.Zoom) {
          pictureBox.Refresh();
          Point lowerLeft = new Point(Math.Min(e.X, buttonDownPoint.X),
                                      Math.Max(e.Y, buttonDownPoint.Y));
          Point upperRight = new Point(Math.Max(e.X, buttonDownPoint.X),
                                       Math.Min(e.Y, buttonDownPoint.Y));
          if ((lowerLeft.X != upperRight.X) && (lowerLeft.Y != upperRight.Y)) {
            Chart.ZoomIn(lowerLeft, upperRight);
          }
        }
      } else if (e.Button == MouseButtons.Right) {
        propertiesToolStripMenuItem.Enabled = Chart.Group.SelectedPrimitives.Count == 1;
        oneLayerUpToolStripMenuItem.Enabled = Chart.Group.SelectedPrimitives.Count == 1;
        oneLayerDownToolStripMenuItem.Enabled = Chart.Group.SelectedPrimitives.Count == 1;
        intoForegroundToolStripMenuItem.Enabled = Chart.Group.SelectedPrimitives.Count == 1;
        intoBackgroundToolStripMenuItem.Enabled = Chart.Group.SelectedPrimitives.Count == 1;
      } else if (e.Button == MouseButtons.Middle) {
        if (Chart.Mode == ChartMode.Zoom) {
          if (mouseClickCount == 1) Chart.ZoomOut();
          else if (mouseClickCount == 2) Chart.Unzoom();
        }
      }
      if (buttonDownPoint == e.Location) {
        if (mouseClickCount == 1)
          Chart.MouseClick(e.Location, e.Button);
        else if (mouseClickCount == 2)
          Chart.MouseDoubleClick(e.Location, e.Button);
      }
    }
    private void pictureBox_MouseMove(object sender, MouseEventArgs e) {
      toolTip.SetToolTip(pictureBox, Chart.GetToolTipText(e.Location));
      Cursor cursor = Chart.GetCursor(e.Location);
      if (cursor != null) pictureBox.Cursor = cursor;
      else pictureBox.Cursor = Cursors.Default;

      if (e.Button != MouseButtons.None) {
        if ((Chart.Mode == ChartMode.Zoom) && (e.Button == MouseButtons.Left)) {
          pictureBox.Refresh();
          Graphics graphics = pictureBox.CreateGraphics();
          Pen pen = new Pen(Color.Gray);
          pen.DashStyle = DashStyle.Dash;
          graphics.DrawRectangle(pen,
                                 Math.Min(e.X, buttonDownPoint.X),
                                 Math.Min(e.Y, buttonDownPoint.Y),
                                 Math.Abs(e.X - buttonDownPoint.X),
                                 Math.Abs(e.Y - buttonDownPoint.Y));
          pen.Dispose();
          graphics.Dispose();
        }
        Chart.MouseDrag(mousePosition, e.Location, e.Button);
      } else {
        Chart.MouseMove(mousePosition, e.Location);
      }
      mousePosition = e.Location;
    }

    private void ChartControl_KeyDown(object sender, KeyEventArgs e) {
      if (Chart.Mode == ChartMode.Select) {
        if (e.KeyCode == Keys.Delete) {
          IList<IPrimitive> selected = Chart.Group.SelectedPrimitives;
          Chart.UpdateEnabled = false;
          foreach (IPrimitive primitive in selected)
            Chart.Group.Remove(primitive);
          Chart.UpdateEnabled = true;
          Chart.EnforceUpdate();
        }
      }
    }

    private void moveToolStripMenuItem_Click(object sender, EventArgs e) {
      SetMode(ChartMode.Move);
    }
    private void zoomToolStripMenuItem_Click(object sender, EventArgs e) {
      SetMode(ChartMode.Zoom);
    }
    private void selectToolStripMenuItem_Click(object sender, EventArgs e) {
      SetMode(ChartMode.Select);
    }
    private void oneLayerUpToolStripMenuItem_Click(object sender, EventArgs e) {
      if (Chart.Group.SelectedPrimitives.Count == 1) {
        Chart.Group.SelectedPrimitives[0].OneLayerUp();
      }
    }
    private void oneLayerDownToolStripMenuItem_Click(object sender, EventArgs e) {
      if (Chart.Group.SelectedPrimitives.Count == 1) {
        Chart.Group.SelectedPrimitives[0].OneLayerDown();
      }
    }
    private void intoForegroundToolStripMenuItem_Click(object sender, EventArgs e) {
      if (Chart.Group.SelectedPrimitives.Count == 1) {
        Chart.Group.SelectedPrimitives[0].IntoForeground();
      }
    }
    private void intoBackgroundToolStripMenuItem_Click(object sender, EventArgs e) {
      if (Chart.Group.SelectedPrimitives.Count == 1) {
        Chart.Group.SelectedPrimitives[0].IntoBackground();
      }
    }
    private void propertiesToolStripMenuItem_Click(object sender, EventArgs e) {
      if (Chart.Group.SelectedPrimitives.Count == 1) {
        PropertiesDialog dialog = new PropertiesDialog(Chart.Group.SelectedPrimitives[0]);
        dialog.ShowDialog(this);
        dialog.Dispose();
      }
    }

    private void GenerateImage() {
      if (!Visible) {
        renderingRequired = true;
      } else {
        if ((pictureBox.Width == 0) || (pictureBox.Height == 0)) {
          bitmap = null;
        } else {
          bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
          if (Chart != null) {
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

    private void SetMode(ChartMode mode) {
      moveToolStripMenuItem.Checked = false;
      zoomToolStripMenuItem.Checked = false;
      selectToolStripMenuItem.Checked = false;
      if (mode == ChartMode.Move) moveToolStripMenuItem.Checked = true;
      else if (mode == ChartMode.Zoom) zoomToolStripMenuItem.Checked = true;
      else if (mode == ChartMode.Select) selectToolStripMenuItem.Checked = true;
      Chart.Mode = mode;
    }
  }
}
