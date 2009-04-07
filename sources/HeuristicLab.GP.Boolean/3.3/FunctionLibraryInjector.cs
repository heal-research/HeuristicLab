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

namespace HeuristicLab.GP.Boolean {
  public class FunctionLibraryInjector : OperatorBase {
    private const string TARGETVARIABLE = "TargetVariable";
    private const string ALLOWEDFEATURES = "AllowedFeatures";
    private const string OPERATORLIBRARY = "FunctionLibrary";

    private GPOperatorLibrary operatorLibrary;
    private Variable variable;

    public override string Description {
      get { return @"Injects a function library for boolean logic."; }
    }

    public FunctionLibraryInjector()
      : base() {
      AddVariableInfo(new VariableInfo(TARGETVARIABLE, "The target variable", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo(ALLOWEDFEATURES, "List of indexes of allowed features", typeof(ItemList<IntData>), VariableKind.In));
      AddVariableInfo(new VariableInfo(OPERATORLIBRARY, "Preconfigured default operator library", typeof(GPOperatorLibrary), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      ItemList<IntData> allowedFeatures = GetVariableValue<ItemList<IntData>>(ALLOWEDFEATURES, scope, true);
      int targetVariable = GetVariableValue<IntData>(TARGETVARIABLE, scope, true).Data;

      // remove the target-variable in case it occures in allowed features
      List<IntData> ts = allowedFeatures.FindAll(d => d.Data == targetVariable);
      foreach (IntData t in ts) allowedFeatures.Remove(t);

      InitDefaultOperatorLibrary();

      int[] allowedIndexes = new int[allowedFeatures.Count];
      for (int i = 0; i < allowedIndexes.Length; i++) {
        allowedIndexes[i] = allowedFeatures[i].Data;
      }

      variable.SetConstraints(allowedIndexes);

      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(OPERATORLIBRARY), operatorLibrary));
      return null;
    }

    private void InitDefaultOperatorLibrary() {
      And and = new And();
      Or or = new Or();
      //Not not = new Not();
      Nand nand = new Nand();
      Nor nor = new Nor();
      //Xor xor = new Xor();
      variable = new HeuristicLab.GP.Boolean.Variable();

      IFunction[] allFunctions = new IFunction[] {
        and,
        or,
        //not,
        nand,
        nor,
        //xor,
        variable
      };

      SetAllowedSubOperators(and, allFunctions);
      SetAllowedSubOperators(or, allFunctions);
      //SetAllowedSubOperators(not, allFunctions);
      SetAllowedSubOperators(nand, allFunctions);
      SetAllowedSubOperators(nor, allFunctions);
      //SetAllowedSubOperators(xor, allFunctions);

      operatorLibrary = new GPOperatorLibrary();
      operatorLibrary.GPOperatorGroup.AddOperator(and);
      operatorLibrary.GPOperatorGroup.AddOperator(or);
      //operatorLibrary.GPOperatorGroup.AddOperator(not);
      operatorLibrary.GPOperatorGroup.AddOperator(nand);
      operatorLibrary.GPOperatorGroup.AddOperator(nor);
      //operatorLibrary.GPOperatorGroup.AddOperator(xor);
      operatorLibrary.GPOperatorGroup.AddOperator(variable);
    }

    private void SetAllowedSubOperators(IFunction f, IFunction[] gs) {
      foreach (IConstraint c in f.Constraints) {
        if (c is SubOperatorTypeConstraint) {
          SubOperatorTypeConstraint typeConstraint = c as SubOperatorTypeConstraint;
          typeConstraint.Clear();
          foreach (IFunction g in gs) {
            typeConstraint.AddOperator(g);
          }
        } else if (c is AllSubOperatorsTypeConstraint) {
          AllSubOperatorsTypeConstraint typeConstraint = c as AllSubOperatorsTypeConstraint;
          typeConstraint.Clear();
          foreach (IFunction g in gs) {
            typeConstraint.AddOperator(g);
          }
        }
      }
    }
  }
}
