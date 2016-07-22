#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics;
using System.Linq;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public class SymbolicExpressionImporter {
    private const string VARSTART = "VAR";
    private const string LAGGEDVARSTART = "LAGVARIABLE";
    private const string INTEGRALSTART = "INTEG";
    private const string DEFUNSTART = "DEFUN";
    private const string ARGSTART = "ARG";
    private const string INVOKESTART = "CALL";
    private const string TIMELAGSTART = "LAG";
    private Dictionary<string, Symbol> knownSymbols = new Dictionary<string, Symbol>() 
      {
        {"+", new Addition()},
        {"/", new Division()},
        {"*", new Multiplication()},
        {"-", new Subtraction()},
        {"EXP", new Exponential()},
        {"LOG", new Logarithm()},
        {"POW", new Power()},
        {"ROOT", new Root()},
        {"SIN",new Sine()},
        {"COS", new Cosine()},
        {"TAN", new Tangent()},
        {"AIRYA", new AiryA()},
        {"AIRYB", new AiryB()},
        {"BESSEL", new Bessel()},
        {"COSINT", new CosineIntegral()},
        {"SININT", new SineIntegral()},
        {"HYPCOSINT", new HyperbolicCosineIntegral()},
        {"HYPSININT", new HyperbolicSineIntegral()},
        {"FRESNELSININT", new FresnelSineIntegral()},
        {"FRESNELCOSINT", new FresnelCosineIntegral()},
        {"NORM", new Norm()},
        {"ERF", new Erf()},
        {"GAMMA", new Gamma()},
        {"PSI", new Psi()},
        {"DAWSON", new Dawson()},
        {"EXPINT", new ExponentialIntegralEi()},
        {"MEAN", new Average()},
        {"IF", new IfThenElse()},
        {">", new GreaterThan()},
        {"<", new LessThan()},
        {"AND", new And()},
        {"OR", new Or()},
        {"NOT", new Not()},
        {"XOR", new Xor()},
        {"DIFF", new Derivative()},
        {"PROG", new ProgramRootSymbol()},
        {"MAIN", new StartSymbol()},
      };

    Constant constant = new Constant();
    Variable variable = new Variable();
    LaggedVariable laggedVariable = new LaggedVariable();
    Defun defun = new Defun();
    TimeLag timeLag = new TimeLag();
    Integral integral = new Integral();

    ProgramRootSymbol programRootSymbol = new ProgramRootSymbol();
    StartSymbol startSymbol = new StartSymbol();

    public ISymbolicExpressionTree Import(string str) {
      str = str.Replace("(", " ( ").Replace(")", " ) ");
      ISymbolicExpressionTreeNode root = programRootSymbol.CreateTreeNode();
      ISymbolicExpressionTreeNode start = startSymbol.CreateTreeNode();
      ISymbolicExpressionTreeNode mainBranch = ParseSexp(new Queue<Token>(GetTokenStream(str)));
      if (mainBranch.Symbol is ProgramRootSymbol) {
        // when a root symbol was parsed => use main branch as root
        root = mainBranch;
      } else {
        // only a main branch was given => insert the main branch into the default tree template
        root.AddSubtree(start);
        start.AddSubtree(mainBranch);
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

    private ISymbolicExpressionTreeNode ParseSexp(Queue<Token> tokens) {
      if (tokens.Peek().Symbol == TokenSymbol.LPAR) {
        ISymbolicExpressionTreeNode tree;
        Expect(Token.LPAR, tokens);
        if (tokens.Peek().StringValue.StartsWith(VARSTART)) {
          tree = ParseVariable(tokens);
        } else if (tokens.Peek().StringValue.StartsWith(LAGGEDVARSTART)) {
          tree = ParseLaggedVariable(tokens);
        } else if (tokens.Peek().StringValue.StartsWith(TIMELAGSTART)) {
          tree = ParseTimeLag(tokens);
          tree.AddSubtree(ParseSexp(tokens));
        } else if (tokens.Peek().StringValue.StartsWith(INTEGRALSTART)) {
          tree = ParseIntegral(tokens);
          tree.AddSubtree(ParseSexp(tokens));
        } else if (tokens.Peek().StringValue.StartsWith(DEFUNSTART)) {
          tree = ParseDefun(tokens);
          while (!tokens.Peek().Equals(Token.RPAR)) {
            tree.AddSubtree(ParseSexp(tokens));
          }
        } else if (tokens.Peek().StringValue.StartsWith(ARGSTART)) {
          tree = ParseArgument(tokens);
        } else if (tokens.Peek().StringValue.StartsWith(INVOKESTART)) {
          tree = ParseInvoke(tokens);
          while (!tokens.Peek().Equals(Token.RPAR)) {
            tree.AddSubtree(ParseSexp(tokens));
          }
        } else {
          Token curToken = tokens.Dequeue();
          tree = CreateTree(curToken);
          while (!tokens.Peek().Equals(Token.RPAR)) {
            tree.AddSubtree(ParseSexp(tokens));
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

    private ISymbolicExpressionTreeNode ParseInvoke(Queue<Token> tokens) {
      Token invokeTok = tokens.Dequeue();
      Debug.Assert(invokeTok.StringValue == "CALL");
      InvokeFunction invokeSym = new InvokeFunction(tokens.Dequeue().StringValue);
      ISymbolicExpressionTreeNode invokeNode = invokeSym.CreateTreeNode();
      return invokeNode;
    }

    private ISymbolicExpressionTreeNode ParseArgument(Queue<Token> tokens) {
      Token argTok = tokens.Dequeue();
      Debug.Assert(argTok.StringValue == "ARG");
      Argument argument = new Argument((int)tokens.Dequeue().DoubleValue);
      ISymbolicExpressionTreeNode argNode = argument.CreateTreeNode();
      return argNode;
    }

    private ISymbolicExpressionTreeNode ParseDefun(Queue<Token> tokens) {
      Token defTok = tokens.Dequeue();
      Debug.Assert(defTok.StringValue == "DEFUN");
      DefunTreeNode t = (DefunTreeNode)defun.CreateTreeNode();
      t.FunctionName = tokens.Dequeue().StringValue;
      return t;
    }

    private ISymbolicExpressionTreeNode ParseTimeLag(Queue<Token> tokens) {
      Token varTok = tokens.Dequeue();
      Debug.Assert(varTok.StringValue == "LAG");
      LaggedTreeNode t = (LaggedTreeNode)timeLag.CreateTreeNode();
      t.Lag = (int)tokens.Dequeue().DoubleValue;
      return t;
    }

    private ISymbolicExpressionTreeNode ParseIntegral(Queue<Token> tokens) {
      Token varTok = tokens.Dequeue();
      Debug.Assert(varTok.StringValue == "INTEGRAL");
      LaggedTreeNode t = (LaggedTreeNode)integral.CreateTreeNode();
      t.Lag = (int)tokens.Dequeue().DoubleValue;
      return t;
    }

    private ISymbolicExpressionTreeNode ParseVariable(Queue<Token> tokens) {
      Token varTok = tokens.Dequeue();
      Debug.Assert(varTok.StringValue == "VARIABLE");
      VariableTreeNode t = (VariableTreeNode)variable.CreateTreeNode();
      t.Weight = tokens.Dequeue().DoubleValue;
      t.VariableName = tokens.Dequeue().StringValue;
      return t;
    }

    private ISymbolicExpressionTreeNode ParseLaggedVariable(Queue<Token> tokens) {
      Token varTok = tokens.Dequeue();
      Debug.Assert(varTok.StringValue == "LAGVARIABLE");
      LaggedVariableTreeNode t = (LaggedVariableTreeNode)laggedVariable.CreateTreeNode();
      t.Weight = tokens.Dequeue().DoubleValue;
      t.VariableName = tokens.Dequeue().StringValue;
      t.Lag = (int)tokens.Dequeue().DoubleValue;
      return t;
    }

    private ISymbolicExpressionTreeNode CreateTree(Token token) {
      if (token.Symbol != TokenSymbol.SYMB) throw new FormatException("Expected function symbol, but got: " + token.StringValue);
      return knownSymbols[token.StringValue].CreateTreeNode();
    }

    private void Expect(Token token, Queue<Token> tokens) {
      Token cur = tokens.Dequeue();
      if (!token.Equals(cur)) throw new FormatException("Expected: " + token.StringValue + ", but got: " + cur.StringValue);
    }
  }
}
