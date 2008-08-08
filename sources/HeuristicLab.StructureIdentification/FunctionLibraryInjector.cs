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
using HeuristicLab.Operators;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Constraints;

namespace HeuristicLab.StructureIdentification {
  public class FunctionLibraryInjector : OperatorBase {
    private const string ALLOWEDFEATURES = "AllowedFeatures";
    private const string MINTIMEOFFSET = "MinTimeOffset";
    private const string MAXTIMEOFFSET = "MaxTimeOffset";
    private const string OPERATORLIBRARY = "OperatorLibrary";

    private HeuristicLab.Functions.Variable variable;
    private HeuristicLab.Functions.Differential differential;
    private GPOperatorLibrary operatorLibrary;

    public override string Description {
      get { return @"Injects a default function library."; }
    }

    public FunctionLibraryInjector()
      : base() {
      AddVariableInfo(new VariableInfo(ALLOWEDFEATURES, "List of indexes of allowed features", typeof(ItemList<IntData>), VariableKind.In));
      GetVariableInfo(ALLOWEDFEATURES).Local = true;
      AddVariable(new Variable(ALLOWEDFEATURES, new ItemList<IntData>()));

      AddVariableInfo(new VariableInfo(MINTIMEOFFSET, "Minimal time offset for all features", typeof(IntData), VariableKind.In));
      GetVariableInfo(MINTIMEOFFSET).Local = true;
      AddVariable(new Variable(MINTIMEOFFSET, new IntData()));

      AddVariableInfo(new VariableInfo(MAXTIMEOFFSET, "Maximal time offset for all feature", typeof(IntData), VariableKind.In));
      GetVariableInfo(MAXTIMEOFFSET).Local = true;
      AddVariable(new Variable(MAXTIMEOFFSET, new IntData()));

      AddVariableInfo(new VariableInfo("OperatorLibrary", "Preconfigured default operator library", typeof(GPOperatorLibrary), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      IntData minTimeOffset = GetVariableValue<IntData>(MINTIMEOFFSET, scope, true);
      IntData maxTimeOffset = GetVariableValue<IntData>(MAXTIMEOFFSET, scope, true);
      ItemList<IntData> allowedFeatures = GetVariableValue<ItemList<IntData>>(ALLOWEDFEATURES, scope, true);

      InitDefaultOperatorLibrary();

      int[] allowedIndexes = new int[allowedFeatures.Count];
      for(int i = 0; i < allowedIndexes.Length; i++) {
        allowedIndexes[i] = allowedFeatures[i].Data;
      }

      variable.SetConstraints(allowedIndexes, minTimeOffset.Data, maxTimeOffset.Data);
      differential.SetConstraints(allowedIndexes, minTimeOffset.Data, maxTimeOffset.Data);

      scope.AddVariable(new Variable(scope.TranslateName(OPERATORLIBRARY), operatorLibrary));
      return null;
    }

    private void InitDefaultOperatorLibrary() {
      variable = new HeuristicLab.Functions.Variable();
      differential = new HeuristicLab.Functions.Differential();
      HeuristicLab.Functions.Constant constant = new HeuristicLab.Functions.Constant();

      HeuristicLab.Functions.Addition addition = new HeuristicLab.Functions.Addition();
      HeuristicLab.Functions.And and = new HeuristicLab.Functions.And();
      HeuristicLab.Functions.Average average = new HeuristicLab.Functions.Average();
      HeuristicLab.Functions.Cosinus cosinus = new HeuristicLab.Functions.Cosinus();
      HeuristicLab.Functions.Division division = new HeuristicLab.Functions.Division();
      HeuristicLab.Functions.Equal equal = new HeuristicLab.Functions.Equal();
      HeuristicLab.Functions.Exponential exponential = new HeuristicLab.Functions.Exponential();
      HeuristicLab.Functions.GreaterThan greaterThan = new HeuristicLab.Functions.GreaterThan();
      HeuristicLab.Functions.IfThenElse ifThenElse = new HeuristicLab.Functions.IfThenElse();
      HeuristicLab.Functions.LessThan lessThan = new HeuristicLab.Functions.LessThan();
      HeuristicLab.Functions.Logarithm logarithm = new HeuristicLab.Functions.Logarithm();
      HeuristicLab.Functions.Multiplication multiplication = new HeuristicLab.Functions.Multiplication();
      HeuristicLab.Functions.Not not = new HeuristicLab.Functions.Not();
      HeuristicLab.Functions.Or or = new HeuristicLab.Functions.Or();
      HeuristicLab.Functions.Power power = new HeuristicLab.Functions.Power();
      HeuristicLab.Functions.Signum signum = new HeuristicLab.Functions.Signum();
      HeuristicLab.Functions.Sinus sinus = new HeuristicLab.Functions.Sinus();
      HeuristicLab.Functions.Sqrt sqrt = new HeuristicLab.Functions.Sqrt();
      HeuristicLab.Functions.Subtraction subtraction = new HeuristicLab.Functions.Subtraction();
      HeuristicLab.Functions.Tangens tangens = new HeuristicLab.Functions.Tangens();
      HeuristicLab.Functions.Xor xor = new HeuristicLab.Functions.Xor();

      HeuristicLab.Functions.IFunction[] booleanFunctions = new HeuristicLab.Functions.IFunction[] {
        and,
        equal,
        greaterThan,
        lessThan,
        not,
        or,
        xor };
      HeuristicLab.Functions.IFunction[] doubleFunctions = new HeuristicLab.Functions.IFunction[] {
        variable,
        differential,
        constant,
        addition,
        average,
        cosinus,
        division,
        exponential,
        ifThenElse,
        logarithm,
        multiplication,
        power,
        signum,
        sinus,
        sqrt,
        subtraction,
        tangens};

      SetAllowedSubOperators(and, booleanFunctions);
      SetAllowedSubOperators(equal, doubleFunctions);
      SetAllowedSubOperators(greaterThan, doubleFunctions);
      SetAllowedSubOperators(lessThan, doubleFunctions);
      SetAllowedSubOperators(not, booleanFunctions);
      SetAllowedSubOperators(or, booleanFunctions);
      SetAllowedSubOperators(xor, booleanFunctions);
      SetAllowedSubOperators(addition, doubleFunctions);
      SetAllowedSubOperators(average, doubleFunctions);
      SetAllowedSubOperators(cosinus, doubleFunctions);
      SetAllowedSubOperators(division, doubleFunctions);
      SetAllowedSubOperators(exponential, doubleFunctions);
      SetAllowedSubOperators(ifThenElse, 0, booleanFunctions);
      SetAllowedSubOperators(ifThenElse, 1, doubleFunctions);
      SetAllowedSubOperators(ifThenElse, 2, doubleFunctions);
      SetAllowedSubOperators(logarithm, doubleFunctions);
      SetAllowedSubOperators(multiplication, doubleFunctions);
      SetAllowedSubOperators(power, doubleFunctions);
      SetAllowedSubOperators(signum, doubleFunctions);
      SetAllowedSubOperators(sinus, doubleFunctions);
      SetAllowedSubOperators(sqrt, doubleFunctions);
      SetAllowedSubOperators(subtraction, doubleFunctions);
      SetAllowedSubOperators(tangens, doubleFunctions);

      operatorLibrary = new GPOperatorLibrary();
      operatorLibrary.GPOperatorGroup.AddOperator(variable);
      operatorLibrary.GPOperatorGroup.AddOperator(differential);
      operatorLibrary.GPOperatorGroup.AddOperator(constant);
      operatorLibrary.GPOperatorGroup.AddOperator(addition);
      operatorLibrary.GPOperatorGroup.AddOperator(average);
      operatorLibrary.GPOperatorGroup.AddOperator(and);
      operatorLibrary.GPOperatorGroup.AddOperator(cosinus);
      operatorLibrary.GPOperatorGroup.AddOperator(division);
      operatorLibrary.GPOperatorGroup.AddOperator(equal);
      operatorLibrary.GPOperatorGroup.AddOperator(exponential);
      operatorLibrary.GPOperatorGroup.AddOperator(greaterThan);
      operatorLibrary.GPOperatorGroup.AddOperator(ifThenElse);
      operatorLibrary.GPOperatorGroup.AddOperator(lessThan);
      operatorLibrary.GPOperatorGroup.AddOperator(logarithm);
      operatorLibrary.GPOperatorGroup.AddOperator(multiplication);
      operatorLibrary.GPOperatorGroup.AddOperator(not);
      operatorLibrary.GPOperatorGroup.AddOperator(power);
      operatorLibrary.GPOperatorGroup.AddOperator(or);
      operatorLibrary.GPOperatorGroup.AddOperator(signum);
      operatorLibrary.GPOperatorGroup.AddOperator(sinus);
      operatorLibrary.GPOperatorGroup.AddOperator(sqrt);
      operatorLibrary.GPOperatorGroup.AddOperator(subtraction);
      operatorLibrary.GPOperatorGroup.AddOperator(tangens);
      operatorLibrary.GPOperatorGroup.AddOperator(xor);
    }

    private void SetAllowedSubOperators(HeuristicLab.Functions.IFunction f, HeuristicLab.Functions.IFunction[] gs) {
      foreach(IConstraint c in f.Constraints) {
        if(c is SubOperatorTypeConstraint) {
          SubOperatorTypeConstraint typeConstraint = c as SubOperatorTypeConstraint;
          typeConstraint.Clear();
          foreach(HeuristicLab.Functions.IFunction g in gs) {
            typeConstraint.AddOperator(g);
          }
        } else if(c is AllSubOperatorsTypeConstraint) {
          AllSubOperatorsTypeConstraint typeConstraint = c as AllSubOperatorsTypeConstraint;
          typeConstraint.Clear();
          foreach(HeuristicLab.Functions.IFunction g in gs) {
            typeConstraint.AddOperator(g);
          }
        }
      }
    }

    private void SetAllowedSubOperators(HeuristicLab.Functions.IFunction f, int p, HeuristicLab.Functions.IFunction[] gs) {
      foreach(IConstraint c in f.Constraints) {
        if(c is SubOperatorTypeConstraint) {
          SubOperatorTypeConstraint typeConstraint = c as SubOperatorTypeConstraint;
          if(typeConstraint.SubOperatorIndex.Data == p) {
            typeConstraint.Clear();
            foreach(HeuristicLab.Functions.IFunction g in gs) {
              typeConstraint.AddOperator(g);
            }
          }
        }
      }
    }
  }
}
