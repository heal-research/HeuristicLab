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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Core;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// This class provides some methods useful when dealing with moves, such as getting compatible move makers to a move generator.
  /// Compatibility is generally ensured in that both the generator and the move maker implement the same specific interface which is derived from IMoveOperator
  /// </summary>
  public static class MoveHelper {

    public static IEnumerable<IMoveMaker> GetCompatibleMoveMakers(IMoveGenerator generator, IEnumerable<IOperator> operators) {
      return GetCompatibleOperators<IMoveMaker>(generator, operators);
    }

    public static IEnumerable<ISingleObjectiveMoveEvaluator> GetCompatibleSingleObjectiveMoveEvaluators(IMoveGenerator generator, IEnumerable<IOperator> operators) {
      return GetCompatibleOperators<ISingleObjectiveMoveEvaluator>(generator, operators);
    }

    public static IEnumerable<ITabuChecker> GetCompatibleTabuChekers(IMoveGenerator generator, IEnumerable<IOperator> operators) {
      return GetCompatibleOperators<ITabuChecker>(generator, operators);
    }

    public static IEnumerable<ITabuMaker> GetCompatibleTabuMakers(IMoveGenerator generator, IEnumerable<IOperator> operators) {
      return GetCompatibleOperators<ITabuMaker>(generator, operators);
    }

    private static IEnumerable<T> GetCompatibleOperators<T>(IMoveGenerator generator, IEnumerable<IOperator> operators) where T : IOperator {
      foreach (Type type in GetMostSpecificMoveOperatorTypes(generator)) {
        foreach (T op in operators.Where(x => type.IsAssignableFrom(x.GetType())).OfType<T>())
          yield return op;
      }
    }

    private static List<Type> GetMostSpecificMoveOperatorTypes(IMoveGenerator generator) {
      List<Type> moveTypes = generator.GetType().GetInterfaces().Where(x => typeof(IMoveOperator).IsAssignableFrom(x)).ToList();
      foreach (Type type in moveTypes.ToList()) {
        if (moveTypes.Any(t => t != type && type.IsAssignableFrom(t)))
          moveTypes.Remove(type);
      }
      return moveTypes;
    }
  }
}
