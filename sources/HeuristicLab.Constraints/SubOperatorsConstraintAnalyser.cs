#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Constraints;
using HeuristicLab.Data;

namespace HeuristicLab.Constraints {
  /// <summary>
  /// Analyzes the sub-operators for specific constraints.
  /// </summary>
  public class SubOperatorsConstraintAnalyser {
    private ICollection<IOperator> allPossibleOperators;

    /// <summary>
    /// Gets or sets all possible operators. 
    /// </summary>
    public ICollection<IOperator> AllPossibleOperators {
      get { return allPossibleOperators; }
      set { allPossibleOperators = value; }
    }
    
    /// <summary>
    /// Gets all operators that fulfill the expression of the constraints of the given operator. 
    /// </summary>
    /// <param name="op">The operator whose constraints to check.</param>
    /// <param name="childIndex">The index of the child.</param>
    /// <returns>All allowed operators.</returns>
    public IList<IOperator> GetAllowedOperators(IOperator op, int childIndex) {
      AndConstraint andConstraint = new AndConstraint();
      foreach (IConstraint constraint in op.Constraints) {
        andConstraint.Clauses.Add(constraint);
      }

      return GetAllowedOperators(andConstraint, childIndex);
    }

    private IList<IOperator> GetAllowedOperators(IConstraint constraint, int childIndex) {
      // manual dispatch on dynamic type
      if (constraint is AndConstraint)
        return GetAllowedOperators((AndConstraint)constraint, childIndex);
      else if (constraint is OrConstraint)
        return GetAllowedOperators((OrConstraint)constraint, childIndex);
      else if (constraint is NotConstraint)
        return GetAllowedOperators((NotConstraint)constraint, childIndex);
      else if (constraint is AllSubOperatorsTypeConstraint)
        return GetAllowedOperators((AllSubOperatorsTypeConstraint)constraint, childIndex);
      else if (constraint is SubOperatorTypeConstraint)
        return GetAllowedOperators((SubOperatorTypeConstraint)constraint, childIndex);
      else return new List<IOperator>(allPossibleOperators); // ignore all other constraints
    }

    #region static set management methods
    // better to use HashSets from .NET 3.5
    // however we would need to switch the whole Constraints project to .NET 3.5 for that
    private static IList<IOperator> Intersect(ICollection<IOperator> a, ICollection<IOperator> b) {
      if (a.Count > b.Count) {
        return Intersect(b, a);
      }

      List<IOperator> intersection = new List<IOperator>(a.Count);

      foreach (IOperator element in a) {
        if (InSet(element, b)) {
          intersection.Add(element);
        }
      }
      return intersection;
    }

    private static IList<IOperator> Union(ICollection<IOperator> a, ICollection<IOperator> b) {
      List<IOperator> union = new List<IOperator>(a);
      foreach (IOperator candidateElement in b) {
        if (!InSet(candidateElement, union)) {
          union.Add(candidateElement);
        }
      }

      return union;
    }

    private static IList<IOperator> Substract(ICollection<IOperator> minuend, ICollection<IOperator> subtrahend) {
      List<IOperator> difference = new List<IOperator>();
      foreach (IOperator element in minuend) {
        if (!InSet(element, subtrahend)) {
          difference.Add(element);
        }
      }

      return difference;
    }

    private static bool InSet(IOperator op, ICollection<IOperator> set) {
      foreach (IOperator element in set) {
        if (element == op)
          return true;
      }
      return false;
    }
    #endregion

    /// <summary>
    /// Gets all allowed operators that fulfill the expression of the given <c>AndConstraint</c>.
    /// </summary>
    /// <param name="constraint">The constraint that must be fulfilled.</param>
    /// <param name="childIndex">The index of the child.</param>
    /// <returns>All allowed operators.</returns>
    public IList<IOperator> GetAllowedOperators(AndConstraint constraint, int childIndex) {
      IList<IOperator> allowedOperators = new List<IOperator>(allPossibleOperators);
      // keep only the intersection of all subconstraints
      foreach (ConstraintBase clause in constraint.Clauses) {
        allowedOperators = Intersect(allowedOperators, GetAllowedOperators(clause, childIndex));
      }
      return allowedOperators;
    }

    /// <summary>
    /// Gets all allowed operators that fulfill the expression of the given <c>OrConstraint</c>.
    /// </summary>
    /// <param name="constraint">The constraint that must be fulfilled.</param>
    /// <param name="childIndex">The index of the child.</param>
    /// <returns>All allowed operators.</returns>
    public IList<IOperator> GetAllowedOperators(OrConstraint constraint, int childIndex) {
      IList<IOperator> allowedOperators = new List<IOperator>();
      foreach (ConstraintBase clause in constraint.Clauses) {
        allowedOperators = Union(allowedOperators, GetAllowedOperators(clause, childIndex));
      }
      return allowedOperators;
    }

    /// <summary>
    /// Gets all allowed operators that fulfill the expression of the given <c>NotConstraint</c>.
    /// </summary>
    /// <param name="constraint">The constraint that must be fulfilled.</param>
    /// <param name="childIndex">The index of the child.</param>
    /// <returns>All allowed operators.</returns>
    public IList<IOperator> GetAllowedOperators(NotConstraint constraint, int childIndex) {
      return Substract(allPossibleOperators, GetAllowedOperators(constraint.SubConstraint, childIndex));
    }

    /// <summary>
    /// Gets all allowed operators that fulfill the expression of the given <c>AllSubOperatorsTypeConstraint</c>.
    /// </summary>
    /// <param name="constraint">The constraint that must be fulfilled.</param>
    /// <param name="childIndex">The index of the child.</param>
    /// <returns>All allowed operators.</returns>
    public IList<IOperator> GetAllowedOperators(AllSubOperatorsTypeConstraint constraint, int childIndex) {
      return Intersect(allPossibleOperators, constraint.AllowedSubOperators);
    }

    /// <summary>
    /// Gets all allowed operators that fulfill the expression of the given <c>SubOperatorTypeConstraint</c>.
    /// </summary>
    /// <param name="constraint">The constraint that must be fulfilled.</param>
    /// <param name="childIndex">The index of the child.</param>
    /// <returns>All allowed operators.</returns>
    public IList<IOperator> GetAllowedOperators(SubOperatorTypeConstraint constraint, int childIndex) {
      if (childIndex != constraint.SubOperatorIndex.Data) {
        return new List<IOperator>(allPossibleOperators);
      } else {
        return Intersect(allPossibleOperators, constraint.AllowedSubOperators);
      }
    }
  }
}
