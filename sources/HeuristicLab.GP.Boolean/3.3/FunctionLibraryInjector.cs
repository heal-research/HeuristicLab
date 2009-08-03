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
using HeuristicLab.GP.Interfaces;
using HeuristicLab.GP;

namespace HeuristicLab.GP.Boolean {
  public class FunctionLibraryInjector : FunctionLibraryInjectorBase {
    public override string Description {
      get { return @"Injects a function library for boolean logic."; }
    }

    public FunctionLibraryInjector()
      : base() {
    }

    protected override FunctionLibrary CreateFunctionLibrary() {
      And and = new And();
      Or or = new Or();
      Not not = new Not();
      Nand nand = new Nand();
      Nor nor = new Nor();
      Xor xor = new Xor();
      Variable variable = new Variable();

      IFunction[] allFunctions = new IFunction[] {
        and,
        or,
        not,
        nand,
        nor,
        xor,
        variable
      };

      SetAllowedSubOperators(and, allFunctions);
      SetAllowedSubOperators(or, allFunctions);
      SetAllowedSubOperators(not, allFunctions);
      SetAllowedSubOperators(nand, allFunctions);
      SetAllowedSubOperators(nor, allFunctions);
      SetAllowedSubOperators(xor, allFunctions);

      var functionLibrary = new FunctionLibrary();
      functionLibrary.AddFunction(and);
      functionLibrary.AddFunction(or);
      functionLibrary.AddFunction(not);
      functionLibrary.AddFunction(nand);
      functionLibrary.AddFunction(nor);
      functionLibrary.AddFunction(xor);
      functionLibrary.AddFunction(variable);
      return functionLibrary;
    }
  }
}
