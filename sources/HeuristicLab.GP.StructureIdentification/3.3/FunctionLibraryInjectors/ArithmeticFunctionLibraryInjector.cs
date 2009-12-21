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
  public class ArithmeticFunctionLibraryInjector : FunctionLibraryInjectorBase {
    public const string MINTIMEOFFSET = "MinTimeOffset";
    public const string MAXTIMEOFFSET = "MaxTimeOffset";

    public const string DIFFERENTIALS_ALLOWED = "Differentials";

    private int minTimeOffset;
    private int maxTimeOffset;

    public override string Description {
      get { return @"Injects a function library with (+, -, *,  /) symbols."; }
    }

    public ArithmeticFunctionLibraryInjector()
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
      FunctionLibrary functionLibrary = new FunctionLibrary();

      Variable variable = new Variable();
      Constant constant = new Constant();
      Differential differential = new Differential();
      Addition addition = new Addition();
      Division division = new Division();
      Multiplication multiplication = new Multiplication();
      Subtraction subtraction = new Subtraction();

      List<IFunction> doubleFunctions = new List<IFunction>();
      if (GetVariableValue<BoolData>(DIFFERENTIALS_ALLOWED, null, false).Data)
        doubleFunctions.Add(differential);
      doubleFunctions.Add(variable);
      doubleFunctions.Add(constant);
      doubleFunctions.Add(addition);
      doubleFunctions.Add(division);
      doubleFunctions.Add(multiplication);
      doubleFunctions.Add(subtraction);

      SetAllowedSubOperators(addition, doubleFunctions);
      SetAllowedSubOperators(division, doubleFunctions);
      SetAllowedSubOperators(multiplication, doubleFunctions);
      SetAllowedSubOperators(subtraction, doubleFunctions);

      if (GetVariableValue<BoolData>(DIFFERENTIALS_ALLOWED, null, false).Data)
        functionLibrary.AddFunction(differential);
      functionLibrary.AddFunction(variable);
      functionLibrary.AddFunction(constant);
      functionLibrary.AddFunction(addition);
      functionLibrary.AddFunction(division);
      functionLibrary.AddFunction(multiplication);
      functionLibrary.AddFunction(subtraction);

      variable.SetConstraints(minTimeOffset, maxTimeOffset);
      differential.SetConstraints(minTimeOffset, maxTimeOffset);

      return functionLibrary;
    }
  }
}
