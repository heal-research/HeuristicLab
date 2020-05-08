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
using HeuristicLab.Common;
using HeuristicLab.Problems.TravelingSalesman.Views;

namespace HeuristicLab.Problems.Orienteering.Views {
  public class OrienteeringVisualizer : TSPVisualizer {
    public IOrienteeringProblemData Data { get; set; }
    public bool IsFeasible { get; set; }

    public override Bitmap Draw(int width, int height) {
      var coordinates = Coordinates;
      var integerVector = Tour;
      var bitmap = new Bitmap(width, height);

      if ((coordinates != null) && (coordinates.Rows > 0) && (coordinates.Columns == 2)
        && (coordinates.Rows == Data.Cities)) {
        double xMin = double.MaxValue, yMin = double.MaxValue, xMax = double.MinValue, yMax = double.MinValue;
        for (int i = 0; i < coordinates.Rows; i++) {
          if (xMin > coordinates[i, 0]) xMin = coordinates[i, 0];
          if (yMin > coordinates[i, 1]) yMin = coordinates[i, 1];
          if (xMax < coordinates[i, 0]) xMax = coordinates[i, 0];
          if (yMax < coordinates[i, 1]) yMax = coordinates[i, 1];
        }

        int border = 20;
        double xStep = xMax != xMin ? (width - 2 * border) / (xMax - xMin) : 1;
        double yStep = yMax != yMin ? (height - 2 * border) / (yMax - yMin) : 1;

        Point[] points = new Point[coordinates.Rows];
        for (int i = 0; i < coordinates.Rows; i++)
          points[i] = new Point(border + ((int)((coordinates[i, 0] - xMin) * xStep)),
                                bitmap.Height - (border + ((int)((coordinates[i, 1] - yMin) * yStep))));

        using (Graphics graphics = Graphics.FromImage(bitmap)) {
          if (integerVector != null && integerVector.Length > 1) {
            Point[] tour = new Point[integerVector.Length];
            for (int i = 0; i < integerVector.Length; i++) {
              tour[i] = points[integerVector[i]];
            }
            bool visualizePenalty = !IsFeasible;
            graphics.DrawLines(visualizePenalty ? Pens.Red : Pens.Black, tour);
          }

          double scoreMin = double.MaxValue;
          double scoreMax = double.MinValue;
          for (int i = 0; i < points.Length; i++) {
            var score = Data.GetScore(i);
            if (score < scoreMin) scoreMin = score;
            if (score > scoreMax) scoreMax = score;
          }
          double scoreRange = scoreMax - scoreMin;
          for (int i = 0; i < points.Length; i++) {
            double score = Data.GetScore(i);
            int size = scoreRange.IsAlmost(0.0)
              ? 6
              : (int)Math.Round(((score - scoreMin) / scoreRange) * 8 + 2);
            graphics.FillRectangle(Brushes.Red, points[i].X - size / 2, points[i].Y - size / 2, size, size);
          }
          int startingPoint = Data.StartingPoint;
          int terminalPoint = Data.TerminalPoint;
          Font font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);
          var beginSize = graphics.MeasureString("Begin", font);
          if (startingPoint >= 0 && startingPoint < points.Length)
            graphics.DrawString("Begin", font, Brushes.Black, points[startingPoint].X - beginSize.Width, points[startingPoint].Y - beginSize.Height);
          if (terminalPoint >= 0 && terminalPoint < points.Length)
            graphics.DrawString("End", font, Brushes.Black, points[terminalPoint].X, points[terminalPoint].Y);
        }
      } else {
        using (Graphics graphics = Graphics.FromImage(bitmap)) {
          graphics.Clear(Color.White);
          Font font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular);
          string text = "No coordinates defined or in wrong format.";
          SizeF strSize = graphics.MeasureString(text, font);
          graphics.DrawString(text, font, Brushes.Black, (float)(width - strSize.Width) / 2.0f, (float)(height - strSize.Height) / 2.0f);
        }
      }
      return bitmap;
    }
  }
}
