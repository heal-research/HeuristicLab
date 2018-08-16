#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public static class OpCodes {
    public const byte Add = 1;
    public const byte Sub = 2;
    public const byte Mul = 3;
    public const byte Div = 4;

    public const byte Sin = 5;
    public const byte Cos = 6;
    public const byte Tan = 7;

    public const byte Log = 8;
    public const byte Exp = 9;

    public const byte IfThenElse = 10;

    public const byte GT = 11;
    public const byte LT = 12;

    public const byte AND = 13;
    public const byte OR = 14;
    public const byte NOT = 15;
    public const byte XOR = 45;


    public const byte Average = 16;

    public const byte Call = 17;

    public const byte Variable = 18;
    public const byte LagVariable = 19;
    public const byte Constant = 20;
    public const byte Arg = 21;

    public const byte Power = 22;
    public const byte Root = 23;
    public const byte TimeLag = 24;
    public const byte Integral = 25;
    public const byte Derivative = 26;

    public const byte VariableCondition = 27;

    public const byte Square = 28;
    public const byte SquareRoot = 29;
    public const byte Gamma = 30;
    public const byte Psi = 31;
    public const byte Dawson = 32;
    public const byte ExponentialIntegralEi = 33;
    public const byte CosineIntegral = 34;
    public const byte SineIntegral = 35;
    public const byte HyperbolicCosineIntegral = 36;
    public const byte HyperbolicSineIntegral = 37;
    public const byte FresnelCosineIntegral = 38;
    public const byte FresnelSineIntegral = 39;
    public const byte AiryA = 40;
    public const byte AiryB = 41;
    public const byte Norm = 42;
    public const byte Erf = 43;
    public const byte Bessel = 44;
    public const byte FactorVariable = 46;
    public const byte BinaryFactorVariable = 47;
    public const byte AnalyticalQuotient = 48;


    private static Dictionary<Type, byte> symbolToOpcode = new Dictionary<Type, byte>() {
       { typeof(Addition), OpCodes.Add },
      { typeof(Subtraction), OpCodes.Sub },
      { typeof(Multiplication), OpCodes.Mul },
      { typeof(Division), OpCodes.Div },
      { typeof(Sine), OpCodes.Sin },
      { typeof(Cosine), OpCodes.Cos },
      { typeof(Tangent), OpCodes.Tan },
      { typeof(Logarithm), OpCodes.Log },
      { typeof(Exponential), OpCodes.Exp },
      { typeof(IfThenElse), OpCodes.IfThenElse },
      { typeof(GreaterThan), OpCodes.GT },
      { typeof(LessThan), OpCodes.LT },
      { typeof(And), OpCodes.AND },
      { typeof(Or), OpCodes.OR },
      { typeof(Not), OpCodes.NOT},
      { typeof(Xor),OpCodes.XOR},
      { typeof(Average), OpCodes.Average},
      { typeof(InvokeFunction), OpCodes.Call },
      { typeof(Variable), OpCodes.Variable },
      { typeof(LaggedVariable), OpCodes.LagVariable },
      { typeof(AutoregressiveTargetVariable),OpCodes.LagVariable},
      { typeof(Constant), OpCodes.Constant },
      { typeof(Argument), OpCodes.Arg },
      { typeof(Power),OpCodes.Power},
      { typeof(Root),OpCodes.Root},
      { typeof(TimeLag), OpCodes.TimeLag},
      { typeof(Integral), OpCodes.Integral},
      { typeof(Derivative), OpCodes.Derivative},
      { typeof(VariableCondition),OpCodes.VariableCondition},
      { typeof(Square),OpCodes.Square},
      { typeof(SquareRoot),OpCodes.SquareRoot},
      { typeof(Gamma), OpCodes.Gamma },
      { typeof(Psi), OpCodes.Psi },
      { typeof(Dawson), OpCodes.Dawson},
      { typeof(ExponentialIntegralEi), OpCodes.ExponentialIntegralEi },
      { typeof(CosineIntegral), OpCodes.CosineIntegral },
      { typeof(SineIntegral), OpCodes.SineIntegral },
      { typeof(HyperbolicCosineIntegral), OpCodes.HyperbolicCosineIntegral },
      { typeof(HyperbolicSineIntegral), OpCodes.HyperbolicSineIntegral },
      { typeof(FresnelCosineIntegral), OpCodes.FresnelCosineIntegral },
      { typeof(FresnelSineIntegral), OpCodes.FresnelSineIntegral },
      { typeof(AiryA), OpCodes.AiryA },
      { typeof(AiryB), OpCodes.AiryB },
      { typeof(Norm), OpCodes.Norm},
      { typeof(Erf), OpCodes.Erf},
      { typeof(Bessel), OpCodes.Bessel},
      { typeof(FactorVariable), OpCodes.FactorVariable },
      { typeof(BinaryFactorVariable), OpCodes.BinaryFactorVariable },
      {typeof(AnalyticalQuotient), OpCodes.AnalyticalQuotient }
    };

    public static byte MapSymbolToOpCode(ISymbolicExpressionTreeNode treeNode) {
      byte opCode;
      if (symbolToOpcode.TryGetValue(treeNode.Symbol.GetType(), out opCode)) return opCode;
      else throw new NotSupportedException("Symbol: " + treeNode.Symbol);
    }
  }
}
