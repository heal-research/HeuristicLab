#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Problems.TravelingSalesman.Views;

namespace HeuristicLab.Problems.PTSP.Views {
  [View("Matrix pTSP Data View")]
  [Content(typeof(MatrixPTSPData), IsDefaultView = true)]
  public partial class MatrixPTSPDataView : MatrixTSPDataView {

    public new MatrixPTSPData Content {
      get { return (MatrixPTSPData)base.Content; }
      set { base.Content = value; }
    }

    public MatrixPTSPDataView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        probabilitiesView.Content = null;
      } else {
        probabilitiesView.Content = Content.Probabilities;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      probabilitiesView.Enabled = !ReadOnly && !Locked && Content != null;
    }

    protected override void GenerateImage() {
      if ((coordinatesPictureBox.Width > 0) && (coordinatesPictureBox.Height > 0)) {
        if (Content == null) {
          coordinatesPictureBox.Image = null;
        } else {
          DoubleMatrix coordinates = Content.DisplayCoordinates;
          DoubleArray probabilities = Content.Probabilities;
          Bitmap bitmap = new Bitmap(coordinatesPictureBox.Width, coordinatesPictureBox.Height);

          if ((coordinates != null) && (coordinates.Rows > 0) && (coordinates.Columns == 2) && (probabilities.Length == coordinates.Rows)) {
            double xMin = double.MaxValue, yMin = double.MaxValue, xMax = double.MinValue, yMax = double.MinValue;
            for (int i = 0; i < coordinates.Rows; i++) {
              if (xMin > coordinates[i, 0]) xMin = coordinates[i, 0];
              if (yMin > coordinates[i, 1]) yMin = coordinates[i, 1];
              if (xMax < coordinates[i, 0]) xMax = coordinates[i, 0];
              if (yMax < coordinates[i, 1]) yMax = coordinates[i, 1];
            }

            int border = 20;
            double xStep = xMax != xMin ? (coordinatesPictureBox.Width - 2 * border) / (xMax - xMin) : 1;
            double yStep = yMax != yMin ? (coordinatesPictureBox.Height - 2 * border) / (yMax - yMin) : 1;

            Point[] points = new Point[coordinates.Rows];
            for (int i = 0; i < coordinates.Rows; i++)
              points[i] = new Point(border + ((int)((coordinates[i, 0] - xMin) * xStep)),
                                    bitmap.Height - (border + ((int)((coordinates[i, 1] - yMin) * yStep))));

            using (Graphics graphics = Graphics.FromImage(bitmap)) {
              for (int i = 0; i < points.Length; i++)
                graphics.FillRectangle(Brushes.Red, points[i].X - 2, points[i].Y - 2, Convert.ToInt32(probabilities[i] * 20), Convert.ToInt32(probabilities[i] * 20));
            }
          } else {
            using (Graphics graphics = Graphics.FromImage(bitmap)) {
              graphics.Clear(Color.White);
              Font font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular);
              string text = "No coordinates defined or in wrong format.";
              SizeF strSize = graphics.MeasureString(text, font);
              graphics.DrawString(text, font, Brushes.Black, (float)(coordinatesPictureBox.Width - strSize.Width) / 2.0f, (float)(coordinatesPictureBox.Height - strSize.Height) / 2.0f);
            }
          }
          coordinatesPictureBox.Image = bitmap;
        }
      }
    }
  }
}
