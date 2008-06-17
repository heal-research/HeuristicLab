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
using HeuristicLab.DataAnalysis;
using HeuristicLab.Core;
using System.Xml;

namespace HeuristicLab.Functions {
  internal class BakedTreeEvaluator : StorableBase {
    private struct Instr {
      public double d_arg0;
      public int i_arg0;
      public int i_arg1;
      public int arity;
      public int symbol;
    }

    private Instr[] codeArr;
    private int PC;
    private Dataset dataset;
    private int sampleIndex;

    // for persistence mechanism only
    public BakedTreeEvaluator() {
    }

    public BakedTreeEvaluator(List<LightWeightFunction> linearRepresentation) {
      codeArr = new Instr[linearRepresentation.Count];
      int i = 0;
      foreach(LightWeightFunction f in linearRepresentation) {
        codeArr[i++] = TranslateToInstr(f);
      }
    }

    private Instr TranslateToInstr(LightWeightFunction f) {
      Instr instr = new Instr();
      instr.arity = f.arity;
      instr.symbol = EvaluatorSymbolTable.MapFunction(f.functionType);
      switch(instr.symbol) {
        case EvaluatorSymbolTable.VARIABLE: {
            instr.i_arg0 = (int)f.data[0]; // var
            instr.d_arg0 = f.data[1]; // weight
            instr.i_arg1 = (int)f.data[2]; // sample-offset
            break;
          }
        case EvaluatorSymbolTable.CONSTANT: {
            instr.d_arg0 = f.data[0]; // value
            break;
          }
      }
      return instr;
    }

    internal double Evaluate(Dataset dataset, int sampleIndex) {
      PC = 0;
      this.sampleIndex = sampleIndex;
      this.dataset = dataset;
      return EvaluateBakedCode();
    }

    private double EvaluateBakedCode() {
      Instr currInstr = codeArr[PC++];
      switch(currInstr.symbol) {
        case EvaluatorSymbolTable.VARIABLE: {
            int row = sampleIndex + currInstr.i_arg1;
            if(row < 0 || row >= dataset.Rows) return double.NaN;
            else return currInstr.d_arg0 * dataset.GetValue(row, currInstr.i_arg0);
          }
        case EvaluatorSymbolTable.CONSTANT: {
            return currInstr.d_arg0;
          }
        case EvaluatorSymbolTable.MULTIPLICATION: {
            double result = EvaluateBakedCode();
            for(int i = 1; i < currInstr.arity; i++) {
              result *= EvaluateBakedCode();
            }
            return result;
          }
        case EvaluatorSymbolTable.ADDITION: {
            double sum = EvaluateBakedCode();
            for(int i = 1; i < currInstr.arity; i++) {
              sum += EvaluateBakedCode();
            }
            return sum;
          }
        case EvaluatorSymbolTable.SUBTRACTION: {
            if(currInstr.arity == 1) {
              return -EvaluateBakedCode();
            } else {
              double result = EvaluateBakedCode();
              for(int i = 1; i < currInstr.arity; i++) {
                result -= EvaluateBakedCode();
              }
              return result;
            }
          }
        case EvaluatorSymbolTable.DIVISION: {
            double result;
            if(currInstr.arity == 1) {
              result = 1.0 / EvaluateBakedCode();
            } else {
              result = EvaluateBakedCode();
              for(int i = 1; i < currInstr.arity; i++) {
                result /= EvaluateBakedCode();
              }
            }
            if(double.IsInfinity(result)) return 0.0;
            else return result;
          }
        case EvaluatorSymbolTable.AVERAGE: {
            double sum = EvaluateBakedCode();
            for(int i = 1; i < currInstr.arity; i++) {
              sum += EvaluateBakedCode();
            }
            return sum / currInstr.arity;
          }
        case EvaluatorSymbolTable.COSINUS: {
            return Math.Cos(EvaluateBakedCode());
          }
        case EvaluatorSymbolTable.SINUS: {
            return Math.Sin(EvaluateBakedCode());
          }
        case EvaluatorSymbolTable.EXP: {
            return Math.Exp(EvaluateBakedCode());
          }
        case EvaluatorSymbolTable.LOG: {
            return Math.Log(EvaluateBakedCode());
          }
        case EvaluatorSymbolTable.POWER: {
            double x = EvaluateBakedCode();
            double p = EvaluateBakedCode();
            return Math.Pow(x, p);
          }
        case EvaluatorSymbolTable.SIGNUM: {
            double value = EvaluateBakedCode();
            if(double.IsNaN(value)) return double.NaN;
            else return Math.Sign(value);
          }
        case EvaluatorSymbolTable.SQRT: {
            return Math.Sqrt(EvaluateBakedCode());
          }
        case EvaluatorSymbolTable.TANGENS: {
            return Math.Tan(EvaluateBakedCode());
          }
        case EvaluatorSymbolTable.AND: {
            double result = 1.0;
            // have to evaluate all sub-trees, skipping would probably not lead to a big gain because 
            // we have to iterate over the linear structure anyway
            for(int i = 0; i < currInstr.arity; i++) {
              double x = Math.Round(EvaluateBakedCode());
              if(x == 0 || x == 1.0) result *= x;
              else result = double.NaN;
            }
            return result;
          }
        case EvaluatorSymbolTable.EQU: {
            double x = EvaluateBakedCode();
            double y = EvaluateBakedCode();
            if(x == y) return 1.0; else return 0.0;
          }
        case EvaluatorSymbolTable.GT: {
            double x = EvaluateBakedCode();
            double y = EvaluateBakedCode();
            if(x > y) return 1.0;
            else return 0.0;
          }
        case EvaluatorSymbolTable.IFTE: {
            double condition = Math.Round(EvaluateBakedCode());
            double x = EvaluateBakedCode();
            double y = EvaluateBakedCode();
            if(condition < .5) return x;
            else if(condition >= .5) return y;
            else return double.NaN;
          }
        case EvaluatorSymbolTable.LT: {
            double x = EvaluateBakedCode();
            double y = EvaluateBakedCode();
            if(x < y) return 1.0;
            else return 0.0;
          }
        case EvaluatorSymbolTable.NOT: {
            double result = Math.Round(EvaluateBakedCode());
            if(result == 0.0) return 1.0;
            else if(result == 1.0) return 0.0;
            else return double.NaN;
          }
        case EvaluatorSymbolTable.OR: {
            double result = 0.0; // default is false
            for(int i = 0; i < currInstr.arity; i++) {
              double x = Math.Round(EvaluateBakedCode());
              if(x == 1.0 && result == 0.0) result = 1.0; // found first true (1.0) => set to true
              else if(x != 0.0) result = double.NaN; // if it was not true it can only be false (0.0) all other cases are undefined => (NaN)
            }
            return result;
          }
        case EvaluatorSymbolTable.XOR: {
            double x = Math.Round(EvaluateBakedCode());
            double y = Math.Round(EvaluateBakedCode());
            if(x == 0.0 && y == 0.0) return 0.0;
            if(x == 1.0 && y == 0.0) return 1.0;
            if(x == 0.0 && y == 1.0) return 1.0;
            if(x == 1.0 && y == 1.0) return 0.0;
            return double.NaN;
          }
        default: {
            throw new NotImplementedException();
          }
      }
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      throw new NotImplementedException();
    }
  }
}
