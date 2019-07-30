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
using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.MOCMAEvolutionStrategy {
  [StorableType("5f3f2453-dbf0-46d8-be15-8b6bb9f88592")]
  public interface IIndicator : IItem {
    /// <summary>
    ///  Selects the least contributing Point of a front. Please keep in mind, that the contribution of a point depends on its value as well as on the other points of a front. 
    ///  A particularly good point may not be as benefical in another front.
    /// </summary>
    /// <typeparam name="TR"></typeparam>
    /// <param name="front">a front which will be evaluated</param>
    /// <param name="problem">The problem on which the front is evaluated (!! The function itself will NOT be evluated only bounds referencePoints & other metadata will be used</param>
    /// <returns>the index of the least contributing point according to any type of quality criteria</returns>
    int LeastContributer(IReadOnlyList<Individual> front, MultiObjectiveBasicProblem<RealVectorEncoding> problem);
  }
}
