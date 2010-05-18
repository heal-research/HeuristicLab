#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Diagnostics;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Tests {
  internal class SymbolicExpressionImporter {
    private const string VARSTART = "VAR";
    private const string DEFUNSTART = "DEFUN";
    private const string ARGSTART = "ARG";
    private const string INVOKESTART = "CALL";
    private Dictionary<string, Symbol> knownSymbols = new Dictionary<string, Symbol>() 
      {
        {"+", new Addition()},
        {"/", new Division()},
        {"*", new Multiplication()},
        {"-", new Subtraction()},
        {"EXP", new Exponential()},
        {"LOG", new Logarithm()},
        {"SIN",new Sine()},
        {"COS", new Cosine()},
        {"TAN", new Tangent()},
        {"MEAN", new Average()},
        {"IF", new IfThenElse()},
        {">", new GreaterThan()},
        {"<", new LessThan()},
        {"AND", new And()},
        {"OR", new Or()},
        {"NOT", new Not()},
        {"PROG", new ProgramRootSymbol()},
        {"MAIN", new StartSymbol()},
      };

    Constant constant = new Constant();
    Variable variable = new Variable();
    Defun defun = new Defun();

    ProgramRootSymbol programRootSymbol = new ProgramRootSymbol();
    StartSymbol startSymbol = new StartSymbol();

    public SymbolicExpressionImporter() {
    }

    internal SymbolicExpressionTree Import(string str) {
      str = str.Replace("(", " ( ").Replace(")", " ) ");
      SymbolicExpressionTreeNode root = programRootSymbol.CreateTreeNode();
      SymbolicExpressionTreeNode start = startSymbol.CreateTreeNode();
      SymbolicExpressionTreeNode mainBranch = ParseSexp(new Queue<Token>(GetTokenStream(str)));
      if (mainBranch.Symbol is ProgramRootSymbol) {
        // when a root symbol was parsed => use main branch as root
        root = mainBranch;
      } else {
        // only a main branch was given => insert the main branch into the default tree template
        root.AddSubTree(start);
        start.AddSubTree(mainBranch);
      }
      return new SymbolicExpressionTree(root);
    }

    private IEnumerable<Token> GetTokenStream(string str) {
      return
             from strToken in str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).AsEnumerable()
             let t = Token.Parse(strToken)
             where t != null
             select t;
    }

    private SymbolicExpressionTreeNode ParseSexp(Queue<Token> tokens) {
      if (tokens.Peek().Symbol == TokenSymbol.LPAR) {
        SymbolicExpressionTreeNode tree;
        Expect(Token.LPAR, tokens);
        if (tokens.Peek().StringValue.StartsWith(VARSTART)) {
          tree = ParseVariable(tokens);
        } else if (tokens.Peek().StringValue.StartsWith(DEFUNSTART)) {
          tree = ParseDefun(tokens);
          while (!tokens.Peek().Equals(Token.RPAR)) {
            tree.AddSubTree(ParseSexp(tokens));
          }
        } else if (tokens.Peek().StringValue.StartsWith(ARGSTART)) {
          tree = ParseArgument(tokens);
        } else if (tokens.Peek().StringValue.StartsWith(INVOKESTART)) {
          tree = ParseInvoke(tokens);
          while (!tokens.Peek().Equals(Token.RPAR)) {
            tree.AddSubTree(ParseSexp(tokens));
          }
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
        ConstantTreeNode t = (ConstantTreeNode)constant.CreateTreeNode();
        t.Value = tokens.Dequeue().DoubleValue;
        return t;
      } else throw new FormatException("Expected function or constant symbol");
    }

    private SymbolicExpressionTreeNode ParseInvoke(Queue<Token> tokens) {
      Token invokeTok = tokens.Dequeue();
      Debug.Assert(invokeTok.StringValue == "CALL");
      InvokeFunction invokeSym = new InvokeFunction(tokens.Dequeue().StringValue);
      SymbolicExpressionTreeNode invokeNode = invokeSym.CreateTreeNode();
      return invokeNode;
    }

    private SymbolicExpressionTreeNode ParseArgument(Queue<Token> tokens) {
      Token argTok = tokens.Dequeue();
      Debug.Assert(argTok.StringValue == "ARG");
      Argument argument = new Argument((int)tokens.Dequeue().DoubleValue);
      SymbolicExpressionTreeNode argNode = argument.CreateTreeNode();
      return argNode;
    }

    private SymbolicExpressionTreeNode ParseDefun(Queue<Token> tokens) {
      Token defTok = tokens.Dequeue();
      Debug.Assert(defTok.StringValue == "DEFUN");
      DefunTreeNode t = (DefunTreeNode)defun.CreateTreeNode();
      t.FunctionName = tokens.Dequeue().StringValue;
      return t;
    }

    private SymbolicExpressionTreeNode ParseVariable(Queue<Token> tokens) {
      Token varTok = tokens.Dequeue();
      Debug.Assert(varTok.StringValue == "VARIABLE");
      VariableTreeNode t = (VariableTreeNode)variable.CreateTreeNode();
      t.Weight = tokens.Dequeue().DoubleValue;
      t.VariableName = tokens.Dequeue().StringValue;
      return t;
    }

    private SymbolicExpressionTreeNode CreateTree(Token token) {
      if (token.Symbol != TokenSymbol.SYMB) throw new FormatException("Expected function symbol, but got: " + token.StringValue);
      return knownSymbols[token.StringValue].CreateTreeNode();
    }

    private void Expect(Token token, Queue<Token> tokens) {
      Token cur = tokens.Dequeue();
      if (!token.Equals(cur)) throw new FormatException("Expected: " + token.StringValue + ", but got: " + cur.StringValue);
    }
  }
}
