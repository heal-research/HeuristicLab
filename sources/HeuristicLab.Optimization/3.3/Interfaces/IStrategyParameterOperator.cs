#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Optimization {
  /// <summary>
  /// Marks all operators belonging to the group of operators using the same strategy parameters.
  /// </summary>
  /// <remarks>
  /// The mechanism for marking these operators is similar to <see cref="IMoveOperator"/>.<br />
  /// You will make derived interfaces of this interface and implement it in each operator of the same group.
  /// Each operator will then also be marked by its type: <see cref="IStrategyParameterCreator"/>, <see cref="IStrategyParameterCrossover"/>, or
  /// <see cref="IStrategyParameterManipulator"/>.<br /><br />
  /// Currently the <see cref="EvolutionStrategy"/> is the only algorithm to make use of this. If your problem is not
  /// suitable to be solved by ES or you don't want to apply ES there is no need to implement such operators in your
  /// own solution representation.
  /// </remarks>
  public interface IStrategyParameterOperator : IOperator {
  }
}
