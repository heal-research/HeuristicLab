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
using HeuristicLab.Data;
using HeuristicLab.Problems.TravelingSalesman.Views;

namespace HeuristicLab.Problems.PTSP.Views {
  public class ProbabilisticTSPVisualizer : TSPVisualizer {
    public PercentArray Probabilities { get; set; }

    public override Bitmap Draw(int width, int height) {
      var bitmap = new Bitmap(width, height);
      if ((Coordinates != null) && (Coordinates.Rows > 0) && (Coordinates.Columns == 2) && (Probabilities.Length == Coordinates.Rows)) {
        double xMin = double.MaxValue, yMin = double.MaxValue, xMax = double.MinValue, yMax = double.MinValue;
        for (int i = 0; i < Coordinates.Rows; i++) {
          if (xMin > Coordinates[i, 0]) xMin = Coordinates[i, 0];
          if (yMin > Coordinates[i, 1]) yMin = Coordinates[i, 1];
          if (xMax < Coordinates[i, 0]) xMax = Coordinates[i, 0];
          if (yMax < Coordinates[i, 1]) yMax = Coordinates[i, 1];
        }

        int border = 20;
        double xStep = xMax != xMin ? (width - 2 * border) / (xMax - xMin) : 1;
        double yStep = yMax != yMin ? (height - 2 * border) / (yMax - yMin) : 1;

        Point[] points = new Point[Coordinates.Rows];
        for (int i = 0; i < Coordinates.Rows; i++)
          points[i] = new Point(border + ((int)((Coordinates[i, 0] - xMin) * xStep)),
                                bitmap.Height - (border + ((int)((Coordinates[i, 1] - yMin) * yStep))));

        using (Graphics graphics = Graphics.FromImage(bitmap)) {
          if (Tour != null && Tour.Length > 1) {
            Point[] tour = new Point[Tour.Length];
            for (int i = 0; i < Tour.Length; i++) {
              if (Tour[i] >= 0 && Tour[i] < points.Length)
                tour[i] = points[Tour[i]];
            }
            graphics.DrawPolygon(Pens.Black, tour);
          }
          for (int i = 0; i < points.Length; i++)
            graphics.FillRectangle(Brushes.Red, points[i].X - 2, points[i].Y - 2, Convert.ToInt32(Probabilities[i] * 20), Convert.ToInt32(Probabilities[i] * 20));
        }
      } else {
        using (Graphics graphics = Graphics.FromImage(bitmap)) {
          graphics.Clear(Color.White);
          Font font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular);
          string text = "No coordinates defined or in wrong format.";
          SizeF strSize = graphics.MeasureString(text, font);
          graphics.DrawString(text, font, Brushes.Black, (width - strSize.Width) / 2.0f, (height - strSize.Height) / 2.0f);
        }
      }

      return bitmap;
    }
  }
}
