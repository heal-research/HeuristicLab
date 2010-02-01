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

using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.GP;

namespace HeuristicLab.GP.StructureIdentification {
  [SymbolicRegressionFunctionLibraryInjector]
  public class DefaultFunctionLibraryInjector : FunctionLibraryInjectorBase {
    public const string MINTIMEOFFSET = "MinTimeOffset";
    public const string MAXTIMEOFFSET = "MaxTimeOffset";

    private int minTimeOffset;
    private int maxTimeOffset;

    public override string Description {
      get { return @"Injects a default function library for regression and classification problems."; }
    }

    public DefaultFunctionLibraryInjector()
      : base() {
      AddVariableInfo(new VariableInfo(MINTIMEOFFSET, "Minimal time offset for all features", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo(MAXTIMEOFFSET, "Maximal time offset for all feature", typeof(IntData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      // try to get minTimeOffset (use 0 as default if not available)
      IItem minTimeOffsetItem = GetVariableValue(MINTIMEOFFSET, scope, true, false);
      minTimeOffset = minTimeOffsetItem == null ? 0 : ((IntData)minTimeOffsetItem).Data;
      // try to get maxTimeOffset (use 0 as default if not available)
      IItem maxTimeOffsetItem = GetVariableValue(MAXTIMEOFFSET, scope, true, false);
      maxTimeOffset = maxTimeOffsetItem == null ? 0 : ((IntData)maxTimeOffsetItem).Data;

      return base.Apply(scope);
    }

    protected override FunctionLibrary CreateFunctionLibrary() {
      FunctionLibrary functionLibrary = new FunctionLibrary();

      Variable variable = new Variable();
      Constant constant = new Constant();
      Differential differential = new Differential();
      Addition addition = new Addition();
      And and = new And();
      Average average = new Average();
      Cosinus cosinus = new Cosinus();
      Division division = new Division();
      Equal equal = new Equal();
      Exponential exponential = new Exponential();
      GreaterThan greaterThan = new GreaterThan();
      IfThenElse ifThenElse = new IfThenElse();
      LessThan lessThan = new LessThan();
      Logarithm logarithm = new Logarithm();
      Multiplication multiplication = new Multiplication();
      Not not = new Not();
      Or or = new Or();
      Power power = new Power();
      Signum signum = new Signum();
      Sinus sinus = new Sinus();
      Sqrt sqrt = new Sqrt();
      Subtraction subtraction = new Subtraction();
      Tangens tangens = new Tangens();
      Xor xor = new Xor();


      List<IFunction> booleanFunctions = new List<IFunction>() {
        and, equal, greaterThan, lessThan, not, or, xor
      };


      List<IFunction> doubleFunctions = new List<IFunction>() {
        differential, variable, constant, addition, average, cosinus, division, exponential,
        ifThenElse, logarithm, multiplication, power, signum, sinus, sqrt, subtraction, tangens};

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

      doubleFunctions.ForEach(fun => functionLibrary.AddFunction(fun));
      booleanFunctions.ForEach(fun => functionLibrary.AddFunction(fun));

      variable.SetConstraints(minTimeOffset, maxTimeOffset);
      differential.SetConstraints(minTimeOffset, maxTimeOffset);

      return functionLibrary;
    }

    private void ConditionalAddFunction(string condName, IFunction fun, List<IFunction> list) {
      if (GetVariableValue<BoolData>(condName, null, false).Data) list.Add(fun);
    }

    private void ConditionalAddOperator(string condName, FunctionLibrary functionLib, IFunction op) {
      if (GetVariableValue<BoolData>(condName, null, false).Data) functionLib.AddFunction(op);
    }
  }
}
