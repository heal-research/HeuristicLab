#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public enum OpCode : byte {
    Add = 1,
    Sub = 2,
    Mul = 3,
    Div = 4,
    Sin = 5,
    Cos = 6,
    Tan = 7,
    Log = 8,
    Exp = 9,
    IfThenElse = 10,
    GT = 11,
    LT = 12,
    AND = 13,
    OR = 14,
    NOT = 15,
    Average = 16,
    Call = 17,
    Variable = 18,
    LagVariable = 19,
    Constant = 20,
    Arg = 21,
    Power = 22,
    Root = 23,
    TimeLag = 24,
    Integral = 25,
    Derivative = 26,
    VariableCondition = 27,
    Square = 28,
    SquareRoot = 29,
    Gamma = 30,
    Psi = 31,
    Dawson = 32,
    ExponentialIntegralEi = 33,
    CosineIntegral = 34,
    SineIntegral = 35,
    HyperbolicCosineIntegral = 36,
    HyperbolicSineIntegral = 37,
    FresnelCosineIntegral = 38,
    FresnelSineIntegral = 39,
    AiryA = 40,
    AiryB = 41,
    Norm = 42,
    Erf = 43,
    Bessel = 44,
    XOR = 45,
    FactorVariable = 46,
    BinaryFactorVariable = 47,
    Absolute = 48,
    AnalyticQuotient = 49,
    Cube = 50,
    CubeRoot = 51,
    Tanh = 52,
  };
  public static class OpCodes {
    // constants for API compatibility only
    public const byte Add = (byte)OpCode.Add;
    public const byte Sub =(byte)OpCode.Sub;
    public const byte Mul =(byte)OpCode.Mul;
    public const byte Div =(byte)OpCode.Div;
    public const byte Sin =(byte)OpCode.Sin;
    public const byte Cos =(byte)OpCode.Cos;
    public const byte Tan =(byte)OpCode.Tan;
    public const byte Log =(byte)OpCode.Log;
    public const byte Exp = (byte)OpCode.Exp;
    public const byte IfThenElse = (byte)OpCode.IfThenElse;
    public const byte GT = (byte)OpCode.GT;
    public const byte LT = (byte)OpCode.LT;
    public const byte AND = (byte)OpCode.AND;
    public const byte OR = (byte)OpCode.OR;
    public const byte NOT = (byte)OpCode.NOT;
    public const byte Average = (byte)OpCode.Average;
    public const byte Call = (byte)OpCode.Call;
    public const byte Variable = (byte)OpCode.Variable;
    public const byte LagVariable = (byte)OpCode.LagVariable;
    public const byte Constant = (byte)OpCode.Constant;
    public const byte Arg = (byte)OpCode.Arg;
    public const byte Power = (byte)OpCode.Power;
    public const byte Root = (byte)OpCode.Root;
    public const byte TimeLag = (byte)OpCode.TimeLag;
    public const byte Integral = (byte)OpCode.Integral;
    public const byte Derivative = (byte)OpCode.Derivative;
    public const byte VariableCondition = (byte)OpCode.VariableCondition;
    public const byte Square = (byte)OpCode.Square;
    public const byte SquareRoot = (byte)OpCode.SquareRoot;
    public const byte Gamma = (byte)OpCode.Gamma;
    public const byte Psi = (byte)OpCode.Psi;
    public const byte Dawson = (byte)OpCode.Dawson;
    public const byte ExponentialIntegralEi = (byte)OpCode.ExponentialIntegralEi;
    public const byte CosineIntegral = (byte)OpCode.CosineIntegral;
    public const byte SineIntegral = (byte)OpCode.SineIntegral;
    public const byte HyperbolicCosineIntegral = (byte)OpCode.HyperbolicCosineIntegral;
    public const byte HyperbolicSineIntegral = (byte)OpCode.HyperbolicSineIntegral;
    public const byte FresnelCosineIntegral = (byte)OpCode.FresnelCosineIntegral;
    public const byte FresnelSineIntegral = (byte)OpCode.FresnelSineIntegral;
    public const byte AiryA = (byte)OpCode.AiryA;
    public const byte AiryB = (byte)OpCode.AiryB;
    public const byte Norm = (byte)OpCode.Norm;
    public const byte Erf = (byte)OpCode.Erf;
    public const byte Bessel = (byte)OpCode.Bessel;
    public const byte XOR = (byte)OpCode.XOR;
    public const byte FactorVariable = (byte)OpCode.FactorVariable;
    public const byte BinaryFactorVariable = (byte)OpCode.BinaryFactorVariable;
    public const byte Absolute = (byte)OpCode.Absolute;
    public const byte AnalyticQuotient = (byte)OpCode.AnalyticQuotient;
    public const byte Cube = (byte)OpCode.Cube;
    public const byte CubeRoot = (byte)OpCode.CubeRoot;
    public const byte Tanh = (byte)OpCode.Tanh;


    private static Dictionary<Type, byte> symbolToOpcode = new Dictionary<Type, byte>() {
       { typeof(Addition), OpCodes.Add },
      { typeof(Subtraction), OpCodes.Sub },
      { typeof(Multiplication), OpCodes.Mul },
      { typeof(Division), OpCodes.Div },
      { typeof(Sine), OpCodes.Sin },
      { typeof(Cosine), OpCodes.Cos },
      { typeof(Tangent), OpCodes.Tan },
      { typeof (HyperbolicTangent), OpCodes.Tanh},
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
      { typeof(Absolute), OpCodes.Absolute },
      { typeof(AnalyticQuotient), OpCodes.AnalyticQuotient },
      { typeof(Cube), OpCodes.Cube },
      { typeof(CubeRoot), OpCodes.CubeRoot }
    };

    public static byte MapSymbolToOpCode(ISymbolicExpressionTreeNode treeNode) {
      if (symbolToOpcode.TryGetValue(treeNode.Symbol.GetType(), out byte opCode)) return opCode;
      else throw new NotSupportedException("Symbol: " + treeNode.Symbol);
    }
  }
}
