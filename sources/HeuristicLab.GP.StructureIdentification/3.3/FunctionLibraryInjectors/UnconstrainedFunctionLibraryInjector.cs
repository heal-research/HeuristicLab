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
  public class UnconstrainedFunctionLibraryInjector : FunctionLibraryInjectorBase {
    public override string Description {
      get { return @"Injects a function library for regression and classification problems."; }
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

      List<IFunction> doubleFunctions = new List<IFunction>() {
      differential, variable, constant, addition, average, cosinus, division, exponential, ifThenElse,logarithm, multiplication, 
      power, signum, sinus, sqrt, subtraction, tangens, and, equal, greaterThan, lessThan, not, or, xor
      };

      SetAllowedSubOperators(and, doubleFunctions);
      SetAllowedSubOperators(equal, doubleFunctions);
      SetAllowedSubOperators(greaterThan, doubleFunctions);
      SetAllowedSubOperators(lessThan, doubleFunctions);
      SetAllowedSubOperators(not, doubleFunctions);
      SetAllowedSubOperators(or, doubleFunctions);
      SetAllowedSubOperators(xor, doubleFunctions);
      SetAllowedSubOperators(addition, doubleFunctions);
      SetAllowedSubOperators(average, doubleFunctions);
      SetAllowedSubOperators(cosinus, doubleFunctions);
      SetAllowedSubOperators(division, doubleFunctions);
      SetAllowedSubOperators(exponential, doubleFunctions);
      SetAllowedSubOperators(ifThenElse, doubleFunctions);
      SetAllowedSubOperators(logarithm, doubleFunctions);
      SetAllowedSubOperators(multiplication, doubleFunctions);
      SetAllowedSubOperators(power, doubleFunctions);
      SetAllowedSubOperators(signum, doubleFunctions);
      SetAllowedSubOperators(sinus, doubleFunctions);
      SetAllowedSubOperators(sqrt, doubleFunctions);
      SetAllowedSubOperators(subtraction, doubleFunctions);
      SetAllowedSubOperators(tangens, doubleFunctions);

      doubleFunctions.ForEach(fun => functionLibrary.AddFunction(fun));

      return functionLibrary;
    }
  }
}
