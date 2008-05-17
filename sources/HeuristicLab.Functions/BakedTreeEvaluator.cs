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
    private int[] codeArr;
    private double[] dataArr;
    private static EvaluatorSymbolTable symbolTable = EvaluatorSymbolTable.SymbolTable;

    // for persistence mechanism only
    public BakedTreeEvaluator() {
    }

    public BakedTreeEvaluator(List<int> code, List<double> data) {
      codeArr = code.ToArray();
      dataArr = data.ToArray();
    }

    private int PC;
    private int DP;
    private Dataset dataset;
    private int sampleIndex;

    internal double Evaluate(Dataset _dataset, int _sampleIndex) {
      PC = 0;
      DP = 0;
      sampleIndex = _sampleIndex;
      dataset = _dataset;
      return EvaluateBakedCode();
    }

    private double EvaluateBakedCode() {
      int arity = codeArr[PC];
      int functionSymbol = codeArr[PC + 1];
      int nLocalVariables = codeArr[PC + 2];
      PC += 3;
      switch(functionSymbol) {
        case EvaluatorSymbolTable.VARIABLE: {
            int var = (int)dataArr[DP];
            double weight = dataArr[DP + 1];
            int row = sampleIndex + (int)dataArr[DP + 2];
            DP += 3;
            if(row < 0 || row >= dataset.Rows) return double.NaN;
            else return weight * dataset.GetValue(row, var);
          }
        case EvaluatorSymbolTable.CONSTANT: {
            return dataArr[DP++];
          }
        case EvaluatorSymbolTable.MULTIPLICATION: {
            double result = EvaluateBakedCode();
            for(int i = 1; i < arity; i++) {
              result *= EvaluateBakedCode();
            }
            return result;
          }
        case EvaluatorSymbolTable.ADDITION: {
            double sum = EvaluateBakedCode();
            for(int i = 1; i < arity; i++) {
              sum += EvaluateBakedCode();
            }
            return sum;
          }
        case EvaluatorSymbolTable.SUBSTRACTION: {
            if(arity == 1) {
              return -EvaluateBakedCode();
            } else {
              double result = EvaluateBakedCode();
              for(int i = 1; i < arity; i++) {
                result -= EvaluateBakedCode();
              }
              return result;
            }
          }
        case EvaluatorSymbolTable.DIVISION: {
            double result;
            if(arity == 1) {
              result = 1.0 / EvaluateBakedCode();
            } else {
              result = EvaluateBakedCode();
              for(int i = 1; i < arity; i++) {
                result /= EvaluateBakedCode();
              }
            }
            if(double.IsInfinity(result)) return 0.0;
            else return result;
          }
        case EvaluatorSymbolTable.AVERAGE: {
            double sum = EvaluateBakedCode();
            for(int i = 1; i < arity; i++) {
              sum += EvaluateBakedCode();
            }
            return sum / arity;
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
            for(int i = 0; i < arity; i++) {
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
            for(int i = 0; i < arity; i++) {
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
            IFunction function = symbolTable.MapSymbol(functionSymbol);
            double[] args = new double[nLocalVariables + arity];
            for(int i = 0; i < nLocalVariables; i++) {
              args[i] = dataArr[DP++];
            }
            for(int j = 0; j < arity; j++) {
              args[nLocalVariables + j] = EvaluateBakedCode();
            }
            return function.Apply(dataset, sampleIndex, args);
          }
      }
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      throw new NotImplementedException();
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("SymbolTable", symbolTable, document, persistedObjects));
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      PersistenceManager.Restore(node.SelectSingleNode("SymbolTable"), restoredObjects);
    }
  }
}
