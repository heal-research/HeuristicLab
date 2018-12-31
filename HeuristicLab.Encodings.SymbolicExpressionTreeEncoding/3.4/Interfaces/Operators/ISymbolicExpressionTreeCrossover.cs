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
using HeuristicLab.Optimization;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableType("bd6d2c3c-a6dd-4e73-abd5-dc6c204e2034")]
  /// <summary>
  /// Interface for crossover operators that can be applied to symbolic expression trees.
  /// </summary>
  public interface ISymbolicExpressionTreeCrossover : ISymbolicExpressionTreeOperator, ICrossover {
    ILookupParameter<ItemArray<ISymbolicExpressionTree>> ParentsParameter { get; }
    ISymbolicExpressionTree Crossover(IRandom random, ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1);
  }
}
