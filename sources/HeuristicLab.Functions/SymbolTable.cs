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

namespace HeuristicLab.Functions {
  class EvaluatorSymbolTable : StorableBase{
    public const int ADDITION = 1;
    public const int AND = 2;
    public const int AVERAGE = 3;
    public const int CONSTANT = 4;
    public const int COSINUS = 5;
    public const int DIVISION = 6;
    public const int EQU = 7;
    public const int EXP = 8;
    public const int GT = 9;
    public const int IFTE = 10;
    public const int LT = 11;
    public const int LOG = 12;
    public const int MULTIPLICATION = 13;
    public const int NOT = 14;
    public const int OR = 15;
    public const int POWER = 16;
    public const int SIGNUM = 17;
    public const int SINUS = 18;
    public const int SQRT = 19;
    public const int SUBTRACTION = 20;
    public const int TANGENS = 21;
    public const int VARIABLE = 22;
    public const int XOR = 23;
    public const int UNKNOWN = 24;

    private static Dictionary<Type, int> staticTypes = new Dictionary<Type,int>();

    // needs to be public for persistence mechanism (Activator.CreateInstance needs empty constructor)
    static EvaluatorSymbolTable () {
      staticTypes = new Dictionary<Type, int>();
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
    }

    internal static int MapFunction(IFunction function) {
      return staticTypes[function.GetType()];
    }
  }
}
