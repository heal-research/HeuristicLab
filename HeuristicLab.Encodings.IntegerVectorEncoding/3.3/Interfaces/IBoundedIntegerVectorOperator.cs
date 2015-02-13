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

using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  public interface IBoundedIntegerVectorOperator : IIntegerVectorOperator {
    /// <summary>
    /// The bounds parameter must contain at least one row and at least two columns. The first two columns specify min and max values, the last column specifies the step size, but is optional (1 is assumed if omitted).
    /// </summary>
    IValueLookupParameter<IntMatrix> BoundsParameter { get; }
  }
}
