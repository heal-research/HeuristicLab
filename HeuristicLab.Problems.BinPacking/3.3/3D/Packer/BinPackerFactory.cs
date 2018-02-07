#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.Packer {

  /// <summary>
  /// This class can be used for creating bin packer insances
  /// </summary>
  public static class BinPackerFactory {

    /// <summary>
    /// Returns the bin packer depending on the given fitting method
    /// </summary>
    /// <param name="fittingMethod"></param>
    /// <returns>Returns a new BinPacker depending on the given fitting method</returns>
    public static IBinPacker CreateBinPacker(FittingMethod fittingMethod) {
      IBinPacker binPacker = null;
      switch (fittingMethod) {
        case FittingMethod.FirstFit:
          binPacker = new BinPackerFirstFit();
          break;
        case FittingMethod.FreeVolumeBestFit:
          binPacker = new BinPackerFreeVolumeBestFit();
          break;
        case FittingMethod.ResidualSpaceBestFit:
          binPacker = new BinPackerResidualSpaceBestFit();
          break;
        case FittingMethod.MinimumResidualSpaceLeft:
          binPacker = new BinPackerMinRSLeft();
          break;
        case FittingMethod.FormClosure:
          binPacker = new BinPackerFormClosure();
          break;
        default:
          throw new ArgumentException("Unknown fitting method: " + fittingMethod);
      }
      return binPacker;
    }
  }
}
