#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// The basic interface that marks all move operators.
  /// </summary>
  /// <remarks>
  /// A group of move operators that belong together should derive an interface from this one
  /// and implement the interface in each operator.<br />
  /// In an algorithm one can thus find out all move operators that belong together, by grouping operators
  /// according to the most specific interface derived from this interface that they implement.<br /><br />
  /// A concrete example:<br />
  /// You have a solution representation <c>MyRep</c> and there you have a move <c>MyRepMove</c> that you want
  /// to make available to the friendly GUIs. So in <c>MyRep</c> you derive an interface <c>IMyRepMoveOperator</c>.<br />
  /// Now you need to implement at least three operators that handle these moves: A MoveGenerator, a MoveMaker, and a MoveEvaluator.
  /// Note: The MoveEvaluator should be implemented in the problem plugin if you choose to separate representation and problem.<br />
  /// In each of these operators you implement <c>IMyRepMoveOperator</c> as well as the appropriate operator specific interface.
  /// For a MoveGenerator that would be one of <c>IExhaustiveMoveGenerator</c>, <c>ISingleMoveGenerator</c>,
  /// or <c>IMultiMoveGenerator</c>, for a MoveMaker that would be <c>IMoveMaker</c>, and for a MoveEvaluator that would
  /// either be <c>ISingleObjectiveMoveEvaluator</c> or <c>IMultiObjectiveMoveEvaluator</c>.<br />
  /// If you have this you need to make sure that an instance of all your operators are loaded in the Operators collection of your IProblem
  /// and you can select them in the respective algorithms.<br /><br />
  /// For Tabu Search support you will need two additional operators: A TabuChecker (e.g. derived from <see cref="TabuChecker" />),
  /// and a TabuMaker.<br /><br />
  /// If you decide later that you want another move, e.g. <c>MyRepMove2</c>, you would do as before and group them under
  /// the interface <c>IMyRepMove2Operator</c>.<br /><br />
  /// If you want to make use of multiple different moves, all your operators would need to know about all the moves that you plan
  /// to use.<br /><br />
  /// Take a look at the Permutation and TSP plugin to see how this looks like in real code.
  /// </remarks>
  public interface IMoveOperator : IOperator {
  }
}
