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

using HeuristicLab.GP.Interfaces;

namespace HeuristicLab.GP.SantaFe {
  public class FunctionLibraryInjector : FunctionLibraryInjectorBase {

    public override string Description {
      get { return @"Injects a function library for the ant problem."; }
    }

    public FunctionLibraryInjector()
      : base() {
    }

    protected override FunctionLibrary CreateFunctionLibrary() {
      FunctionLibrary funLib = new FunctionLibrary();
      IfFoodAhead ifFoodAhead = new IfFoodAhead();
      Prog2 prog2 = new Prog2();
      Prog3 prog3 = new Prog3();
      Move move = new Move();
      Left left = new Left();
      Right right = new Right();

      IFunction[] allFunctions = new IFunction[] {
        ifFoodAhead,
        prog2,
        prog3,
        move,
        left,
        right
      };

      SetAllowedSubOperators(ifFoodAhead, allFunctions);
      SetAllowedSubOperators(prog2, allFunctions);
      SetAllowedSubOperators(prog3, allFunctions);

      funLib.AddFunction(ifFoodAhead);
      funLib.AddFunction(prog2);
      funLib.AddFunction(prog3);
      funLib.AddFunction(move);
      funLib.AddFunction(left);
      funLib.AddFunction(right);
      return funLib;
    }
  }
}
