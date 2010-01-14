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

namespace HeuristicLab.GP.StructureIdentification.Networks {
  public class FunctionLibraryInjector : FunctionLibraryInjectorBase {
    public const string MINTIMEOFFSET = "MinTimeOffset";
    public const string MAXTIMEOFFSET = "MaxTimeOffset";

    public const string DIFFERENTIALS_ALLOWED = "Differentials";

    private int minTimeOffset;
    private int maxTimeOffset;

    public override string Description {
      get { return @"Injects a function library for network structure identification."; }
    }

    public FunctionLibraryInjector()
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
      return Create(GetVariableValue<BoolData>(DIFFERENTIALS_ALLOWED, null, false).Data, minTimeOffset, maxTimeOffset);
    }

    public static FunctionLibrary Create(bool includeDifferential, int minTimeOffset, int maxTimeOffset) {
      FunctionLibrary functionLibrary = new FunctionLibrary();

      #region f0 (...) -> double
      Variable variable = new Variable();
      Constant constant = new Constant();
      Differential differential = new Differential();
      Addition addition = new Addition();
      Division division = new Division();
      Multiplication multiplication = new Multiplication();
      Subtraction subtraction = new Subtraction();

      List<IFunction> f0Functions =
        new List<IFunction>() { 
          variable, constant, addition, subtraction,
          division, multiplication};

      if (includeDifferential)
        f0Functions.Add(differential);
      #endregion

      #region f1: (...) -> (double -> double)
      OpenParameter openPar = new OpenParameter();
      OpenExp openExp = new OpenExp();
      OpenLog openLog = new OpenLog();
      //OpenSqrt openSqrt = new OpenSqrt();
      //OpenSqr openSqr = new OpenSqr();
      Flip flip = new Flip();
      AdditionF1 addF1 = new AdditionF1();
      SubtractionF1 subF1 = new SubtractionF1();
      MultiplicationF1 mulF1 = new MultiplicationF1();
      DivisionF1 divF1 = new DivisionF1();

      List<IFunction> f1Functions = new List<IFunction>() { 
        openPar,
        openExp, openLog, flip, // openSqrt, openSqr, 
        addF1, subF1, mulF1, divF1
      };
      #endregion

      #region f2: (...) -> (double, double -> double)
      Cycle cycle = new Cycle();
      OpenAddition openAdd = new OpenAddition();
      OpenSubtraction openSub = new OpenSubtraction();
      OpenMultiplication openMul = new OpenMultiplication();
      OpenDivision openDivision = new OpenDivision();
      List<IFunction> f2Functions = new List<IFunction>() { openAdd, openSub, openMul, openDivision, cycle };
      #endregion


      SetAllowedSubOperators(addition, f0Functions);
      SetAllowedSubOperators(division, f0Functions);
      SetAllowedSubOperators(multiplication, f0Functions);
      SetAllowedSubOperators(subtraction, f0Functions);

      SetAllowedSubOperators(openExp, f1Functions);
      SetAllowedSubOperators(openLog, f1Functions);
      //SetAllowedSubOperators(openSqrt, f1Functions);
      //SetAllowedSubOperators(openSqr, f1Functions);
      SetAllowedSubOperators(flip, f1Functions);
      SetAllowedSubOperators(addF1, 0, f1Functions);
      SetAllowedSubOperators(addF1, 1, f0Functions);
      SetAllowedSubOperators(subF1, 0, f1Functions);
      SetAllowedSubOperators(subF1, 1, f0Functions);
      SetAllowedSubOperators(mulF1, 0, f1Functions);
      SetAllowedSubOperators(mulF1, 1, f0Functions);
      SetAllowedSubOperators(divF1, 0, f1Functions);
      SetAllowedSubOperators(divF1, 1, f0Functions);

      SetAllowedSubOperators(cycle, f2Functions);
      SetAllowedSubOperators(openAdd, f1Functions);
      SetAllowedSubOperators(openDivision, f1Functions);
      SetAllowedSubOperators(openMul, f1Functions);
      SetAllowedSubOperators(openSub, f1Functions);

      if (includeDifferential)
        functionLibrary.AddFunction(differential);
      f0Functions.ForEach(x => functionLibrary.AddFunction(x));
      f1Functions.ForEach(x => functionLibrary.AddFunction(x));
      f2Functions.ForEach(x => functionLibrary.AddFunction(x));

      variable.SetConstraints(minTimeOffset, maxTimeOffset);
      differential.SetConstraints(minTimeOffset, maxTimeOffset);
      openPar.SetConstraints(minTimeOffset, maxTimeOffset);


      return functionLibrary;
    }
  }
}
