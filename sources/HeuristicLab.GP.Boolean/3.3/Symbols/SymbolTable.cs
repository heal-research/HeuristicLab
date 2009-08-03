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

namespace HeuristicLab.GP.Boolean {
  class SymbolTable : StorableBase {
    public const int VARIABLE = 1;
    public const int NOT = 2;
    public const int AND = 3;
    public const int OR = 4;
    public const int NAND = 5;
    public const int NOR = 6;
    public const int XOR = 7;
    public const int UNKNOWN = 8;
    private static Dictionary<Type, int> staticTypes = new Dictionary<Type, int>();

    // needs to be public for persistence mechanism (Activator.CreateInstance needs empty constructor)
    static SymbolTable() {
      staticTypes = new Dictionary<Type, int>();
      staticTypes[typeof(Variable)] = VARIABLE;
      staticTypes[typeof(Not)] = NOT;
      staticTypes[typeof(And)] = AND;
      staticTypes[typeof(Or)] = OR;
      staticTypes[typeof(Nand)] = NAND;
      staticTypes[typeof(Nor)] = NOR;
      staticTypes[typeof(Xor)] = XOR;
    }

    internal static int MapFunction(IFunction function) {
      if(staticTypes.ContainsKey(function.GetType())) return staticTypes[function.GetType()];
      else return UNKNOWN;
    }
  }
}
