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
using StructId = HeuristicLab.GP.StructureIdentification;

namespace HeuristicLab.GP.StructureIdentification {
  public class FunctionLibraryInjector : OperatorBase {
    private const string FUNCTIONLIBRARY = "FunctionLibrary";
    private const string TARGETVARIABLE = "TargetVariable";
    private const string ALLOWEDFEATURES = "AllowedFeatures";
    private const string AUTOREGRESSIVE = "Autoregressive";
    private const string MINTIMEOFFSET = "MinTimeOffset";
    private const string MAXTIMEOFFSET = "MaxTimeOffset";

    private const string DIFFERENTIALS_ALLOWED = "Differentials";
    private const string VARIABLES_ALLOWED = "Variables";
    private const string CONSTANTS_ALLOWED = "Constants";
    private const string ADDITION_ALLOWED = "Addition";
    private const string AVERAGE_ALLOWED = "Average";
    private const string AND_ALLOWED = "And";
    private const string COSINUS_ALLOWED = "Cosinus";
    private const string DIVISION_ALLOWED = "Division";
    private const string EQUAL_ALLOWED = "Equal";
    private const string EXPONENTIAL_ALLOWED = "Exponential";
    private const string GREATERTHAN_ALLOWED = "GreaterThan";
    private const string IFTHENELSE_ALLOWED = "IfThenElse";
    private const string LESSTHAN_ALLOWED = "LessThan";
    private const string LOGARTIHM_ALLOWED = "Logarithm";
    private const string MULTIPLICATION_ALLOWED = "Multiplication";
    private const string NOT_ALLOWED = "Not";
    private const string POWER_ALLOWED = "Power";
    private const string OR_ALLOWED = "Or";
    private const string SIGNUM_ALLOWED = "Signum";
    private const string SINUS_ALLOWED = "Sinus";
    private const string SQRT_ALLOWED = "SquareRoot";
    private const string SUBTRACTION_ALLOWED = "Subtraction";
    private const string TANGENS_ALLOWED = "Tangens";
    private const string XOR_ALLOWED = "Xor";


    public override string Description {
      get { return @"Injects a configurable function library for regression and classification problems."; }
    }

    public FunctionLibraryInjector()
      : base() {
      AddVariableInfo(new VariableInfo(TARGETVARIABLE, "The target variable", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo(ALLOWEDFEATURES, "List of indexes of allowed features", typeof(ItemList<IntData>), VariableKind.In));
      AddVariableInfo(new Core.VariableInfo(AUTOREGRESSIVE, "Switch to turn on/off autoregressive modeling (wether to allow the target variable as input)", typeof(BoolData), Core.VariableKind.In));
      AddVariableInfo(new Core.VariableInfo(MINTIMEOFFSET, "Minimal time offset for all features", typeof(IntData), Core.VariableKind.In));
      AddVariableInfo(new Core.VariableInfo(MAXTIMEOFFSET, "Maximal time offset for all feature", typeof(IntData), Core.VariableKind.In));
      AddVariableInfo(new VariableInfo(FUNCTIONLIBRARY, "Preconfigured default operator library", typeof(GPOperatorLibrary), VariableKind.New));

      AddVariable(DIFFERENTIALS_ALLOWED, false);
      AddVariable(VARIABLES_ALLOWED, true);
      AddVariable(CONSTANTS_ALLOWED, true);
      AddVariable(ADDITION_ALLOWED, true);
      AddVariable(AVERAGE_ALLOWED, false);
      AddVariable(AND_ALLOWED, true);
      AddVariable(COSINUS_ALLOWED, true);
      AddVariable(DIVISION_ALLOWED, true);
      AddVariable(EQUAL_ALLOWED, true);
      AddVariable(EXPONENTIAL_ALLOWED, true);
      AddVariable(GREATERTHAN_ALLOWED, true);
      AddVariable(IFTHENELSE_ALLOWED, true);
      AddVariable(LESSTHAN_ALLOWED, true);
      AddVariable(LOGARTIHM_ALLOWED, true);
      AddVariable(MULTIPLICATION_ALLOWED, true);
      AddVariable(NOT_ALLOWED, true);
      AddVariable(POWER_ALLOWED, true);
      AddVariable(OR_ALLOWED, true);
      AddVariable(SIGNUM_ALLOWED, true);
      AddVariable(SINUS_ALLOWED, true);
      AddVariable(SQRT_ALLOWED, true);
      AddVariable(SUBTRACTION_ALLOWED, true);
      AddVariable(TANGENS_ALLOWED, true);
      AddVariable(XOR_ALLOWED, false);
    }

    private void AddVariable(string name, bool allowed) {
      AddVariableInfo(new VariableInfo(name, name + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(name).Local = true;
      AddVariable(new Core.Variable(name, new BoolData(allowed)));
    }

    public override IOperation Apply(IScope scope) {
      StructId.Variable variable;
      GPOperatorLibrary operatorLibrary;

      ItemList<IntData> allowedFeatures = GetVariableValue<ItemList<IntData>>(ALLOWEDFEATURES, scope, true);
      int targetVariable = GetVariableValue<IntData>(TARGETVARIABLE, scope, true).Data;

      // try to get minTimeOffset (use 0 as default if not available)
      IItem minTimeOffsetItem = GetVariableValue(MINTIMEOFFSET, scope, true, false);
      int minTimeOffset = minTimeOffsetItem == null ? 0 : ((IntData)minTimeOffsetItem).Data;
      // try to get maxTimeOffset (use 0 as default if not available)
      IItem maxTimeOffsetItem = GetVariableValue(MAXTIMEOFFSET, scope, true, false);
      int maxTimeOffset = maxTimeOffsetItem == null ? 0 : ((IntData)maxTimeOffsetItem).Data;
      // try to get flag autoregressive (use false as default if not available)
      IItem autoRegItem = GetVariableValue(AUTOREGRESSIVE, scope, true, false);
      bool autoregressive = autoRegItem == null ? false : ((BoolData)autoRegItem).Data;

      if (autoregressive) {
        // make sure the target-variable occures in list of allowed features
        if (!allowedFeatures.Exists(d => d.Data == targetVariable)) allowedFeatures.Add(new IntData(targetVariable));
      } else {
        // remove the target-variable in case it occures in allowed features
        List<IntData> ts = allowedFeatures.FindAll(d => d.Data == targetVariable);
        foreach (IntData t in ts) allowedFeatures.Remove(t);
      }

      variable = new StructId.Variable();
      StructId.Constant constant = new StructId.Constant();
      StructId.Differential differential = new Differential();
      StructId.Addition addition = new StructId.Addition();
      StructId.And and = new StructId.And();
      StructId.Average average = new StructId.Average();
      StructId.Cosinus cosinus = new StructId.Cosinus();
      StructId.Division division = new StructId.Division();
      StructId.Equal equal = new StructId.Equal();
      StructId.Exponential exponential = new StructId.Exponential();
      StructId.GreaterThan greaterThan = new StructId.GreaterThan();
      StructId.IfThenElse ifThenElse = new StructId.IfThenElse();
      StructId.LessThan lessThan = new StructId.LessThan();
      StructId.Logarithm logarithm = new StructId.Logarithm();
      StructId.Multiplication multiplication = new StructId.Multiplication();
      StructId.Not not = new StructId.Not();
      StructId.Or or = new StructId.Or();
      StructId.Power power = new StructId.Power();
      StructId.Signum signum = new StructId.Signum();
      StructId.Sinus sinus = new StructId.Sinus();
      StructId.Sqrt sqrt = new StructId.Sqrt();
      StructId.Subtraction subtraction = new StructId.Subtraction();
      StructId.Tangens tangens = new StructId.Tangens();
      StructId.Xor xor = new StructId.Xor();


      List<IOperator> booleanFunctions = new List<IOperator>();
      ConditionalAddOperator(AND_ALLOWED, and, booleanFunctions);
      ConditionalAddOperator(EQUAL_ALLOWED, equal, booleanFunctions);
      ConditionalAddOperator(GREATERTHAN_ALLOWED, greaterThan, booleanFunctions);
      ConditionalAddOperator(LESSTHAN_ALLOWED, lessThan, booleanFunctions);
      ConditionalAddOperator(NOT_ALLOWED, not, booleanFunctions);
      ConditionalAddOperator(OR_ALLOWED, or, booleanFunctions);
      ConditionalAddOperator(XOR_ALLOWED, xor, booleanFunctions);

      List<IOperator> doubleFunctions = new List<IOperator>();
      ConditionalAddOperator(DIFFERENTIALS_ALLOWED, differential, doubleFunctions);
      ConditionalAddOperator(VARIABLES_ALLOWED, variable, doubleFunctions);
      ConditionalAddOperator(CONSTANTS_ALLOWED, constant, doubleFunctions);
      ConditionalAddOperator(ADDITION_ALLOWED, addition, doubleFunctions);
      ConditionalAddOperator(AVERAGE_ALLOWED, average, doubleFunctions);
      ConditionalAddOperator(COSINUS_ALLOWED, cosinus, doubleFunctions);
      ConditionalAddOperator(DIVISION_ALLOWED, division, doubleFunctions);
      ConditionalAddOperator(EXPONENTIAL_ALLOWED, exponential, doubleFunctions);
      ConditionalAddOperator(IFTHENELSE_ALLOWED, ifThenElse, doubleFunctions);
      ConditionalAddOperator(LOGARTIHM_ALLOWED, logarithm, doubleFunctions);
      ConditionalAddOperator(MULTIPLICATION_ALLOWED, multiplication, doubleFunctions);
      ConditionalAddOperator(POWER_ALLOWED, power, doubleFunctions);
      ConditionalAddOperator(SIGNUM_ALLOWED, signum, doubleFunctions);
      ConditionalAddOperator(SINUS_ALLOWED, sinus, doubleFunctions);
      ConditionalAddOperator(SQRT_ALLOWED, sqrt, doubleFunctions);
      ConditionalAddOperator(SUBTRACTION_ALLOWED, subtraction, doubleFunctions);
      ConditionalAddOperator(TANGENS_ALLOWED, tangens, doubleFunctions);

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
      ConditionalAddOperator(DIFFERENTIALS_ALLOWED, operatorLibrary, differential);
      ConditionalAddOperator(VARIABLES_ALLOWED, operatorLibrary, variable);
      ConditionalAddOperator(CONSTANTS_ALLOWED, operatorLibrary, constant);
      ConditionalAddOperator(ADDITION_ALLOWED, operatorLibrary, addition);
      ConditionalAddOperator(AVERAGE_ALLOWED, operatorLibrary, average);
      ConditionalAddOperator(AND_ALLOWED, operatorLibrary, and);
      ConditionalAddOperator(COSINUS_ALLOWED, operatorLibrary, cosinus);
      ConditionalAddOperator(DIVISION_ALLOWED, operatorLibrary, division);
      ConditionalAddOperator(EQUAL_ALLOWED, operatorLibrary, equal);
      ConditionalAddOperator(EXPONENTIAL_ALLOWED, operatorLibrary, exponential);
      ConditionalAddOperator(GREATERTHAN_ALLOWED, operatorLibrary, greaterThan);
      ConditionalAddOperator(IFTHENELSE_ALLOWED, operatorLibrary, ifThenElse);
      ConditionalAddOperator(LESSTHAN_ALLOWED, operatorLibrary, lessThan);
      ConditionalAddOperator(LOGARTIHM_ALLOWED, operatorLibrary, logarithm);
      ConditionalAddOperator(MULTIPLICATION_ALLOWED, operatorLibrary, multiplication);
      ConditionalAddOperator(NOT_ALLOWED, operatorLibrary, not);
      ConditionalAddOperator(POWER_ALLOWED, operatorLibrary, power);
      ConditionalAddOperator(OR_ALLOWED, operatorLibrary, or);
      ConditionalAddOperator(SIGNUM_ALLOWED, operatorLibrary, signum);
      ConditionalAddOperator(SINUS_ALLOWED, operatorLibrary, sinus);
      ConditionalAddOperator(SQRT_ALLOWED, operatorLibrary, sqrt);
      ConditionalAddOperator(SUBTRACTION_ALLOWED, operatorLibrary, subtraction);
      ConditionalAddOperator(TANGENS_ALLOWED, operatorLibrary, tangens);
      ConditionalAddOperator(XOR_ALLOWED, operatorLibrary, xor);

      int[] allowedIndexes = new int[allowedFeatures.Count];
      for (int i = 0; i < allowedIndexes.Length; i++) {
        allowedIndexes[i] = allowedFeatures[i].Data;
      }

      variable.SetConstraints(allowedIndexes, minTimeOffset, maxTimeOffset);
      differential.SetConstraints(allowedIndexes, minTimeOffset, maxTimeOffset);

      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(FUNCTIONLIBRARY), operatorLibrary));

      return null;
    }

    private void ConditionalAddOperator(string condName, IOperator op, List<IOperator> list) {
      if (GetVariableValue<BoolData>(condName, null, false).Data) list.Add(op);
    }

    private void ConditionalAddOperator(string condName, GPOperatorLibrary operatorLibrary, IOperator op) {
      if (GetVariableValue<BoolData>(condName, null, false).Data) operatorLibrary.GPOperatorGroup.AddOperator(op);
    }

    private void SetAllowedSubOperators(IFunction f, List<IOperator> gs) {
      foreach (IConstraint c in f.Constraints) {
        if (c is SubOperatorTypeConstraint) {
          SubOperatorTypeConstraint typeConstraint = c as SubOperatorTypeConstraint;
          typeConstraint.Clear();
          foreach (IOperator g in gs) {
            typeConstraint.AddOperator(g);
          }
        } else if (c is AllSubOperatorsTypeConstraint) {
          AllSubOperatorsTypeConstraint typeConstraint = c as AllSubOperatorsTypeConstraint;
          typeConstraint.Clear();
          foreach (IOperator g in gs) {
            typeConstraint.AddOperator(g);
          }
        }
      }
    }

    private void SetAllowedSubOperators(IFunction f, int p, List<IOperator> gs) {
      foreach (IConstraint c in f.Constraints) {
        if (c is SubOperatorTypeConstraint) {
          SubOperatorTypeConstraint typeConstraint = c as SubOperatorTypeConstraint;
          if (typeConstraint.SubOperatorIndex.Data == p) {
            typeConstraint.Clear();
            foreach (IOperator g in gs) {
              typeConstraint.AddOperator(g);
            }
          }
        }
      }
    }
  }
}
