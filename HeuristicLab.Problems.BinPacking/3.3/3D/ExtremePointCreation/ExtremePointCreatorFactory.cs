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

namespace HeuristicLab.Problems.BinPacking3D.ExtremePointCreation {
  public static class ExtremePointCreatorFactory {
    
    /// <summary>
    /// Returns an extreme point creator depending on the given creation method.
    /// </summary>
    /// <param name="epCreationMethod"></param>
    /// <param name="useStackingConstraints"></param>
    /// <returns></returns>
    public static IExtremePointCreator CreateExtremePointCreator(ExtremePointCreationMethod epCreationMethod, bool useStackingConstraints) {
      if (epCreationMethod == ExtremePointCreationMethod.PointProjection) {
        return new PointProjectionBasedEPCreator();
      } else if (epCreationMethod == ExtremePointCreationMethod.LineProjection) {
        return new LineProjectionBasedEPCreator();
      }
      return new PointProjectionBasedEPCreator();
    }
  }
}
