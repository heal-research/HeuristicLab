#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace HeuristicLab.Common {
  public static class ColorGradient {
    private const int shades = 256;

    private static List<Color> colors = new List<Color>();
    private static ReadOnlyCollection<Color> readOnlyColors;
    public static IList<Color> Colors {
      get {
        if (readOnlyColors == null) readOnlyColors = colors.AsReadOnly();
        return readOnlyColors;
      }
    }

    private static List<Color> grayscaledColors = new List<Color>();
    private static ReadOnlyCollection<Color> readOnlyGrayScaledColors;
    public static IList<Color> GrayscaledColors {
      get {
        if (readOnlyGrayScaledColors == null) readOnlyGrayScaledColors = grayscaledColors.AsReadOnly();
        return readOnlyGrayScaledColors;
      }
    }

    static ColorGradient() {
      int stepWidth = (256 * 4) / shades;
      int currentValue;
      int currentClass;
      for (int i = 0; i < shades; i++) {
        currentValue = (i * stepWidth) % 256;
        currentClass = (i * stepWidth) / 256;
        switch (currentClass) {
          case 0: { colors.Add(Color.FromArgb(0, currentValue, 255)); break; }        // blue -> cyan
          case 1: { colors.Add(Color.FromArgb(0, 255, 255 - currentValue)); break; }  // cyan -> green
          case 2: { colors.Add(Color.FromArgb(currentValue, 255, 0)); break; }        // green -> yellow
          case 3: { colors.Add(Color.FromArgb(255, 255 - currentValue, 0)); break; }  // yellow -> red
        }
      }
      for (int i = 0; i < 256; i++)
        grayscaledColors.Add(Color.FromArgb(255 - i, 255 - i, 255 - i));  // white -> black
    }
  }
}
