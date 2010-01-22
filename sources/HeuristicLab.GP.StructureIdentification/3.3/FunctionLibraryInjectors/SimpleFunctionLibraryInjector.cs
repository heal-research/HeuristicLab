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
  public class SimpleFunctionLibraryInjector : FunctionLibraryInjectorBase {
    public const string MINTIMEOFFSET = "MinTimeOffset";
    public const string MAXTIMEOFFSET = "MaxTimeOffset";

    public const string DIFFERENTIALS_ALLOWED = "Differentials";

    private int minTimeOffset;
    private int maxTimeOffset;

    public override string Description {
      get { return @"Injects a simple function library for regression and classification problems."; }
    }

    public SimpleFunctionLibraryInjector()
      : base() {
      AddVariableInfo(new VariableInfo(MINTIMEOFFSET, "Minimal time offset for all features", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo(MAXTIMEOFFSET, "Maximal time offset for all feature", typeof(IntData), VariableKind.In));

      AddVariable(DIFFERENTIALS_ALLOWED, false);
    }

    private void AddVariable(string name, bool allowed) {
      AddVariableInfo(new VariableInfo(name, name + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(name).Local = true;
      AddVariable(new Core.Variable(name, new BoolData(allowed)));
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
      return Create(
        GetVariableValue<BoolData>(DIFFERENTIALS_ALLOWED, null, false).Data,
        minTimeOffset,
        maxTimeOffset);
    }

    public static FunctionLibrary Create(bool differentialAllowed, int minTimeOffset, int maxTimeOffset) {
      FunctionLibrary functionLibrary = new FunctionLibrary();

      Variable variable = new Variable();
      Constant constant = new Constant();
      Differential differential = new Differential();
      Addition addition = new Addition();
      Cosinus cosinus = new Cosinus();
      Division division = new Division();
      Exponential exponential = new Exponential();
      Logarithm logarithm = new Logarithm();
      Multiplication multiplication = new Multiplication();
      Power power = new Power();
      Sinus sinus = new Sinus();
      Sqrt sqrt = new Sqrt();
      Subtraction subtraction = new Subtraction();
      Tangens tangens = new Tangens();

      List<IFunction> valueNodes = new List<IFunction>();
      if (differentialAllowed) valueNodes.Add(differential);
      valueNodes.Add(variable);
      valueNodes.Add(constant);

      List<IFunction> arithmeticFunctions = new List<IFunction>();
      arithmeticFunctions.Add(addition);
      arithmeticFunctions.Add(division);
      arithmeticFunctions.Add(multiplication);
      arithmeticFunctions.Add(subtraction);

      List<IFunction> complexFunctions = new List<IFunction>();
      complexFunctions.Add(cosinus);
      complexFunctions.Add(exponential);
      complexFunctions.Add(logarithm);
      complexFunctions.Add(power);
      complexFunctions.Add(sinus);
      complexFunctions.Add(sqrt);
      complexFunctions.Add(tangens);

      List<IFunction> allFunctions = new List<IFunction>();
      allFunctions.AddRange(arithmeticFunctions);
      allFunctions.AddRange(complexFunctions);
      allFunctions.AddRange(valueNodes);

      SetAllowedSubOperators(addition, allFunctions);
      SetAllowedSubOperators(division, allFunctions);
      SetAllowedSubOperators(multiplication, allFunctions);
      SetAllowedSubOperators(subtraction, allFunctions);

      SetAllowedSubOperators(cosinus, valueNodes);
      SetAllowedSubOperators(exponential, valueNodes);
      SetAllowedSubOperators(logarithm, valueNodes);
      SetAllowedSubOperators(power, valueNodes);
      SetAllowedSubOperators(sinus, valueNodes);
      SetAllowedSubOperators(sqrt, valueNodes);
      SetAllowedSubOperators(tangens, valueNodes);

      if (differentialAllowed) functionLibrary.AddFunction(differential);

      allFunctions.ForEach(x => functionLibrary.AddFunction(x));

      variable.SetConstraints(minTimeOffset, maxTimeOffset);
      differential.SetConstraints(minTimeOffset, maxTimeOffset);

      return functionLibrary;
    }
  }
}
