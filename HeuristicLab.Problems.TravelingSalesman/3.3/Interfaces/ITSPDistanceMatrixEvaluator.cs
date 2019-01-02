#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;

namespace HeuristicLab.Problems.TravelingSalesman {
  [StorableType("ADEAB327-E563-42D6-845C-E9D412586A4C")]
  /// <summary>
  /// An interface which represents an evaluation operator which evaluates TSP solutions given in path representation using a distance matrix.
  /// </summary>
  public interface ITSPDistanceMatrixEvaluator : ITSPPathEvaluator {
    ILookupParameter<DistanceMatrix> DistanceMatrixParameter { get; }
  }
}
