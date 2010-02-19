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
    public override string Description {
      get { return @"Injects a simple function library for regression and classification problems."; }
    }

    protected override FunctionLibrary CreateFunctionLibrary() {
      return Create();
    }

    public static FunctionLibrary Create() {
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

      List<IFunction> valueNodes = new List<IFunction>() {
        differential, variable, constant
      };
      List<IFunction> arithmeticFunctions = new List<IFunction>() {
      addition, division, multiplication, subtraction,
      };

      List<IFunction> complexFunctions = new List<IFunction>() {
        cosinus, exponential, logarithm, power, sinus, sqrt, tangens
      };

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

      allFunctions.ForEach(x => functionLibrary.AddFunction(x));

      return functionLibrary;
    }
  }
}
