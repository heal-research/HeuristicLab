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
using HeuristicLab.Core;
using HeuristicLab.GP.Interfaces;

namespace HeuristicLab.GP.SantaFe {
  class SymbolTable : StorableBase {
    public const int IF_FOOD_AHEAD = 1;
    public const int LEFT = 2;
    public const int RIGHT = 3;
    public const int MOVE = 4;
    public const int PROG2 = 5;
    public const int PROG3 = 6;
    public const int UNKNOWN = 7;
    private static Dictionary<Type, int> staticTypes = new Dictionary<Type, int>();

    // needs to be public for persistence mechanism (Activator.CreateInstance needs empty constructor)
    static SymbolTable() {
      staticTypes = new Dictionary<Type, int>();
      staticTypes[typeof(IfFoodAhead)] = IF_FOOD_AHEAD;
      staticTypes[typeof(Left)] = LEFT;
      staticTypes[typeof(Right)] = RIGHT;
      staticTypes[typeof(Move)] = MOVE;
      staticTypes[typeof(Prog2)] = PROG2;
      staticTypes[typeof(Prog3)] = PROG3;
    }

    internal static int MapFunction(IFunction function) {
      if(staticTypes.ContainsKey(function.GetType())) return staticTypes[function.GetType()];
      else return UNKNOWN;
    }
  }
}
