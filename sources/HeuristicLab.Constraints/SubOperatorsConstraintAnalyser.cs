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
  public class SubOperatorsConstraintAnalyser {
    private ICollection<IOperator> allPossibleOperators;

    public ICollection<IOperator> AllPossibleOperators {
      get { return allPossibleOperators; }
      set { allPossibleOperators = value; }
    }

    public IList<IOperator> GetAllowedOperators(IOperator op, int childIndex) {

      AndConstraint andConstraint = new AndConstraint();
      foreach(IConstraint constraint in op.Constraints) {
        andConstraint.Clauses.Add(constraint);
      }

      GetAllowedOperatorsVisitor visitor = new GetAllowedOperatorsVisitor(allPossibleOperators, childIndex);
      andConstraint.Accept(visitor);

      return visitor.AllowedOperators;
    }

    #region static set management methods
    // better to use HashSets from .NET 3.5
    // however we would need to switch the whole Constraints project to .NET 3.5 for that
    private static IList<IOperator> Intersect(ICollection<IOperator> a, ICollection<IOperator> b) {
      if(a.Count > b.Count) {
        return Intersect(b, a);
      }

      List<IOperator> intersection = new List<IOperator>(a.Count);

      foreach(IOperator element in a) {
        if(InSet(element, b)) {
          intersection.Add(element);
        }
      }
      return intersection;
    }

    private static IList<IOperator> Union(ICollection<IOperator> a, ICollection<IOperator> b) {
      List<IOperator> union = new List<IOperator>(a);
      foreach(IOperator candidateElement in b) {
        if(!InSet(candidateElement, union)) {
          union.Add(candidateElement);
        }
      }

      return union;
    }

    private static IList<IOperator> Substract(ICollection<IOperator> minuend, ICollection<IOperator> subtrahend) {
      List<IOperator> difference = new List<IOperator>();
      foreach(IOperator element in minuend) {
        if(!InSet(element, subtrahend)) {
          difference.Add(element);
        }
      }

      return difference;
    }

    private static bool InSet(IOperator op, ICollection<IOperator> set) {
      foreach(IOperator element in set) {
        if(((StringData)element.GetVariable("TypeId").Value).Data ==
          ((StringData)op.GetVariable("TypeId").Value).Data) {
          return true;
        }
      }

      return false;
    }
    #endregion

    #region visitor
    /// <summary>
    /// The visitor builds a set of allowed operators based on a tree of constraints.
    /// </summary>
    private class GetAllowedOperatorsVisitor : ConstraintVisitorBase {
      private IList<IOperator> allowedOperators;

      public IList<IOperator> AllowedOperators {
        get { return allowedOperators; }
      }
      private ICollection<IOperator> possibleOperators;
      private int childIndex;

      public GetAllowedOperatorsVisitor(ICollection<IOperator> possibleOperators, int childIndex) {
        // default is that all possible operators are allowed
        allowedOperators = new List<IOperator>(possibleOperators);
        this.possibleOperators = possibleOperators;
        this.childIndex = childIndex;
      }

      public override void Visit(AndConstraint constraint) {
        base.Visit(constraint);

        // keep only the intersection of all subconstraints
        foreach(ConstraintBase clause in constraint.Clauses) {
          GetAllowedOperatorsVisitor visitor = new GetAllowedOperatorsVisitor(possibleOperators, childIndex);
          clause.Accept(visitor);
          allowedOperators = Intersect(allowedOperators, visitor.allowedOperators);
        }
      }
      
      public override void Visit(OrConstraint constraint) {
        base.Visit(constraint);

        // allowed operators is the union of all allowed operators as defined by the subconstraints
        allowedOperators.Clear();

        foreach(ConstraintBase clause in constraint.Clauses) {
          GetAllowedOperatorsVisitor visitor = new GetAllowedOperatorsVisitor(possibleOperators, childIndex);
          clause.Accept(visitor);
          allowedOperators = Union(allowedOperators, visitor.allowedOperators);
        }
      }

      public override void Visit(NotConstraint constraint) {
        base.Visit(constraint);
        GetAllowedOperatorsVisitor visitor = new GetAllowedOperatorsVisitor(possibleOperators, childIndex);
        constraint.SubConstraint.Accept(visitor);

        allowedOperators = Substract(possibleOperators, visitor.allowedOperators);
      }

      public override void Visit(AllSubOperatorsTypeConstraint constraint) {
        base.Visit(constraint);

        allowedOperators = Intersect(possibleOperators, constraint.AllowedSubOperators);
      }

      public override void Visit(SubOperatorTypeConstraint constraint) {
        if(childIndex != constraint.SubOperatorIndex.Data) {
          allowedOperators = new List<IOperator>(possibleOperators);
        } else {
          allowedOperators = Intersect(possibleOperators, constraint.AllowedSubOperators);
        }
      }
    }
    #endregion
  }
}
