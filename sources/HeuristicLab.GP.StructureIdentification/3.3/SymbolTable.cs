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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Xml;

namespace HeuristicLab.GP.StructureIdentification {
  class EvaluatorSymbolTable : StorableBase {
    public const byte ADDITION = 1;
    public const byte AND = 2;
    public const byte AVERAGE = 3;
    public const byte CONSTANT = 4;
    public const byte COSINUS = 5;
    public const byte DIFFERENTIAL = 25;
    public const byte DIVISION = 6;
    public const byte EQU = 7;
    public const byte EXP = 8;
    public const byte GT = 9;
    public const byte IFTE = 10;
    public const byte LT = 11;
    public const byte LOG = 12;
    public const byte MULTIPLICATION = 13;
    public const byte NOT = 14;
    public const byte OR = 15;
    public const byte POWER = 16;
    public const byte SIGNUM = 17;
    public const byte SINUS = 18;
    public const byte SQRT = 19;
    public const byte SUBTRACTION = 20;
    public const byte TANGENS = 21;
    public const byte VARIABLE = 22;
    public const byte XOR = 23;
    public const byte UNKNOWN = 24;

    private static Dictionary<Type, byte> staticTypes = new Dictionary<Type, byte>();

    // needs to be public for persistence mechanism (Activator.CreateInstance needs empty constructor)
    static EvaluatorSymbolTable() {
      staticTypes = new Dictionary<Type, byte>();
      staticTypes[typeof(Addition)] = ADDITION;
      staticTypes[typeof(And)] = AND;
      staticTypes[typeof(Average)] = AVERAGE;
      staticTypes[typeof(Constant)] = CONSTANT;
      staticTypes[typeof(Cosinus)] = COSINUS;
      staticTypes[typeof(Division)] = DIVISION;
      staticTypes[typeof(Equal)] = EQU;
      staticTypes[typeof(Exponential)] = EXP;
      staticTypes[typeof(GreaterThan)] = GT;
      staticTypes[typeof(IfThenElse)] = IFTE;
      staticTypes[typeof(LessThan)] = LT;
      staticTypes[typeof(Logarithm)] = LOG;
      staticTypes[typeof(Multiplication)] = MULTIPLICATION;
      staticTypes[typeof(Not)] = NOT;
      staticTypes[typeof(Or)] = OR;
      staticTypes[typeof(Power)] = POWER;
      staticTypes[typeof(Signum)] = SIGNUM;
      staticTypes[typeof(Sinus)] = SINUS;
      staticTypes[typeof(Sqrt)] = SQRT;
      staticTypes[typeof(Subtraction)] = SUBTRACTION;
      staticTypes[typeof(Tangens)] = TANGENS;
      staticTypes[typeof(Variable)] = VARIABLE;
      staticTypes[typeof(Xor)] = XOR;
      staticTypes[typeof(Differential)] = DIFFERENTIAL;
    }

    internal static byte MapFunction(IFunction function) {
      if(staticTypes.ContainsKey(function.GetType())) return staticTypes[function.GetType()];
      else return UNKNOWN;
    }
  }
}
