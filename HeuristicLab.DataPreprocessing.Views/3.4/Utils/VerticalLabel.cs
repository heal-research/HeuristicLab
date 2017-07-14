#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.DataPreprocessing.Views {
  public class VerticalLabel : Label {
    protected override void OnPaint(PaintEventArgs e) {
      Brush b = new SolidBrush(ForeColor);
      var stringFormat = new StringFormat(StringFormatFlags.DirectionVertical);

      var textSize = e.Graphics.MeasureString(Text, Font, PointF.Empty, stringFormat);
      e.Graphics.TranslateTransform(Width / 2 + textSize.Width / 2, Height / 2 + textSize.Height / 2);
      e.Graphics.RotateTransform(180);

      e.Graphics.DrawString(Text, Font, b, PointF.Empty, stringFormat);
    }
  }
}

