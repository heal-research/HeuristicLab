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
using HeuristicLab.GP.Interfaces;
using System.IO;
using System.Diagnostics;
using HeuristicLab.GP.StructureIdentification;

namespace HeuristicLab.GP.Test {
  class SymbolicExpressionImporter {
    private const string DIFFSTART = "dif";
    private const string VARSTART = "var";
    private Dictionary<string, IFunction> knownFunctions = new Dictionary<string, IFunction>() 
      {
        {"+", new Addition()},
        {"and", new And()},
        {"mean", new Average()},
        {"cos", new Cosinus()},
        {"/", new Division()},
        {"equ", new Equal()},
        {"exp", new Exponential()},
        {">", new GreaterThan()},
        {"if", new IfThenElse()},
        {"<", new LessThan()},
        {"log", new Logarithm()},
        {"*", new Multiplication()},
        {"not", new Not()},
        {"or", new Or()},
        {"expt", new Power()},
        {"sign", new Signum()},
        {"sin",new Sinus()},
        {"sqrt", new Sqrt()},
        {"-", new Subtraction()},
        {"tan", new Tangens()},
        {"xor", new Xor()}
      };
    Constant constant = new Constant();
    HeuristicLab.GP.StructureIdentification.Variable variable = new HeuristicLab.GP.StructureIdentification.Variable();
    Differential differential = new Differential();

    public SymbolicExpressionImporter() {
    }

    internal IFunctionTree Import(string str) {
      str = str.Replace("(", " ( ").Replace(")", " ) ");
      return ParseSexp(new Queue<Token>(GetTokenStream(str)));
    }

    private IEnumerable<Token> GetTokenStream(string str) {
      return
             from strToken in str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).AsEnumerable()
             let t = Token.Parse(strToken)
             where t != null
             select t;
    }

    private HeuristicLab.GP.Interfaces.IFunctionTree ParseSexp(Queue<Token> tokens) {
      if (tokens.Peek().Symbol == TokenSymbol.LPAR) {
        IFunctionTree tree;
        Expect(Token.LPAR, tokens);
        if (tokens.Peek().StringValue.StartsWith(VARSTART)) {
          tree = ParseVariable(tokens);
        } else if (tokens.Peek().StringValue.StartsWith(DIFFSTART)) {
          tree = ParseDifferential(tokens);
        } else {
          Token curToken = tokens.Dequeue();
          tree = CreateTree(curToken);
          while (!tokens.Peek().Equals(Token.RPAR)) {
            tree.AddSubTree(ParseSexp(tokens));
          }
        }
        Expect(Token.RPAR, tokens);
        return tree;
      } else if (tokens.Peek().Symbol == TokenSymbol.NUMBER) {
        ConstantFunctionTree t = (ConstantFunctionTree)constant.GetTreeNode();
        t.Value = tokens.Dequeue().DoubleValue;
        return t;
      } else throw new FormatException("Expected function or constant symbol");
    }

    private IFunctionTree ParseDifferential(Queue<Token> tokens) {
      Token diffTok = tokens.Dequeue();
      Debug.Assert(diffTok.StringValue == "differential");
      VariableFunctionTree t = (VariableFunctionTree)differential.GetTreeNode();
      t.Weight = tokens.Dequeue().DoubleValue;
      t.VariableName = tokens.Dequeue().StringValue;
      t.SampleOffset = (int)tokens.Dequeue().DoubleValue;
      return t;
    }

    private IFunctionTree ParseVariable(Queue<Token> tokens) {
      Token varTok = tokens.Dequeue();
      Debug.Assert(varTok.StringValue == "variable");
      VariableFunctionTree t = (VariableFunctionTree)variable.GetTreeNode();
      t.Weight = tokens.Dequeue().DoubleValue;
      t.VariableName = tokens.Dequeue().StringValue;
      t.SampleOffset = (int)tokens.Dequeue().DoubleValue;
      return t;
    }

    private IFunctionTree CreateTree(Token token) {
      if (token.Symbol != TokenSymbol.SYMB) throw new FormatException("Expected function symbol, but got: " + token.StringValue);
      return knownFunctions[token.StringValue].GetTreeNode();
    }

    private void Expect(Token token, Queue<Token> tokens) {
      Token cur = tokens.Dequeue();
      if (!token.Equals(cur)) throw new FormatException("Expected: " + token.StringValue + ", but got found: " + cur.StringValue);
    }
  }
}
