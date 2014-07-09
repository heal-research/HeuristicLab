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

namespace HeuristicLab.Optimization {
  /// <summary>
  /// The basic interface that marks all move operators. Derived interfaces
  /// are used to group move operators together.
  /// </summary>
  /// <remarks>
  /// A group of move operators that belong together should derive a new
  /// interface from IMoveOperator and implement the new interface in each
  /// operator.<br />
  /// 
  /// E.g. a new move is to be implemented and INewMove is created as a derived
  /// type of IMoveOperator. Then you create the additional following types<br />
  /// <list type="bullet">
  /// <item>
  ///   <term>NewMoveGenerator</term>
  ///   <description>
  ///   implements INewMove as well as either IExhaustiveMoveGenerator,
  ///   IMultiMoveGenerator or ISingleMoveGenerator
  ///   </description>
  /// </item>
  /// <item>
  ///   <term>NewMoveMaker</term>
  ///   <description>
  ///   implements INewMove as well as IMoveMaker
  ///   </description>
  /// </item>
  /// <item>
  ///   <term>NewMoveEvaluator</term>
  ///   <description>
  ///   (problem plugin) implements INewMove as well as ISingleObjectiveMoveEvaluator
  ///   </description>
  /// </item>
  /// <item>
  ///   <term>NewMoveTabuMaker</term>
  ///   <description>
  ///   (only for Tabu Search) implements INewMove as well as TabuMaker
  ///   </description>
  /// </item>
  /// <item>
  ///   <term>NewMoveTabuChecker</term>
  ///   <description>
  ///   (only for Tabu Search) implements INewMove as well as ITabuChecker
  ///   </description>
  /// </item>
  /// </list>
  /// 
  /// These operators should not implement further move types. For further moves
  /// a new interface derived from IMoveOperator should be created.
  public interface IMoveOperator : IOperator {
  }
}
