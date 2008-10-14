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
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Constraints;

namespace HeuristicLab.GP.SantaFe {
  public class FunctionLibraryInjector : OperatorBase {
    private const string OPERATORLIBRARY = "FunctionLibrary";

    private GPOperatorLibrary operatorLibrary;

    public override string Description {
      get { return @"Injects a function library for the ant problem."; }
    }

    public FunctionLibraryInjector()
      : base() {
      AddVariableInfo(new VariableInfo(OPERATORLIBRARY, "Preconfigured default operator library", typeof(GPOperatorLibrary), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      InitDefaultOperatorLibrary();
      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(OPERATORLIBRARY), operatorLibrary));
      return null;
    }

    private void InitDefaultOperatorLibrary() {
      IfFoodAhead ifFoodAhead = new IfFoodAhead();
      Prog2 prog2 = new Prog2();
      Prog3 prog3 = new Prog3();
      Move move = new Move();
      Left left = new Left();
      Right right = new Right();

      IFunction[] allFunctions = new IFunction[] {
        ifFoodAhead,
        prog2,
        prog3,
        move,
        left,
        right
      };

      SetAllowedSubOperators(ifFoodAhead, allFunctions);
      SetAllowedSubOperators(prog2, allFunctions);
      SetAllowedSubOperators(prog3, allFunctions);

      operatorLibrary = new GPOperatorLibrary();
      operatorLibrary.GPOperatorGroup.AddOperator(ifFoodAhead);
      operatorLibrary.GPOperatorGroup.AddOperator(prog2);
      operatorLibrary.GPOperatorGroup.AddOperator(prog3);
      operatorLibrary.GPOperatorGroup.AddOperator(move);
      operatorLibrary.GPOperatorGroup.AddOperator(left);
      operatorLibrary.GPOperatorGroup.AddOperator(right);
    }

    private void SetAllowedSubOperators(IFunction f, IFunction[] gs) {
      foreach(IConstraint c in f.Constraints) {
        if(c is SubOperatorTypeConstraint) {
          SubOperatorTypeConstraint typeConstraint = c as SubOperatorTypeConstraint;
          typeConstraint.Clear();
          foreach(IFunction g in gs) {
            typeConstraint.AddOperator(g);
          }
        } else if(c is AllSubOperatorsTypeConstraint) {
          AllSubOperatorsTypeConstraint typeConstraint = c as AllSubOperatorsTypeConstraint;
          typeConstraint.Clear();
          foreach(IFunction g in gs) {
            typeConstraint.AddOperator(g);
          }
        }
      }
    }
  }
}
