﻿#region License Information
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

using System.Windows.Forms;

namespace HeuristicLab.Clients.Hive.Views.Extensions {
  // Bugfix for NumericUpDown to disable arrow controls correctly
  // cf. https://www.codeproject.com/Articles/16321/NumericUpDown-with-correct-read-only-behavior
  public class FixedNumericUpDown : NumericUpDown {
    public override void DownButton() {
      if (ReadOnly)
        return;
      base.DownButton();
    }

    public override void UpButton() {
      if (ReadOnly)
        return;
      base.UpButton();
    }
  }
}
