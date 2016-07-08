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
using System.Globalization;
using System.Linq;
using System.Text;
using HeuristicLab.Collections;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// Parses mathematical expressions in infix form. E.g. x1 * (3.0 * x2 + x3)
  /// Identifier format (functions or variables): '_' | letter { '_' | letter | digit }
  /// Variables names can be set under quotes "" or '' because variable names might contain spaces. 
  /// It is also possible to use functions e.g. log("x1") or real-valued constants e.g. 3.1415 . 
  /// Variable names are case sensitive. Function names are not case sensitive.
  /// </summary>
  public sealed class InfixExpressionParser {
    private enum TokenType { Operator, Identifier, Number, LeftPar, RightPar, End, NA };
    private class Token {
      internal double doubleVal;
      internal string strVal;
      internal TokenType TokenType;
    }

    private class SymbolNameComparer : IEqualityComparer<ISymbol>, IComparer<ISymbol> {
      public int Compare(ISymbol x, ISymbol y) {
        return x.Name.CompareTo(y.Name);
      }

      public bool Equals(ISymbol x, ISymbol y) {
        return Compare(x, y) == 0;
      }

      public int GetHashCode(ISymbol obj) {
        return obj.Name.GetHashCode();
      }
    }
    // format name <-> symbol 
    // the lookup table is also used in the corresponding formatter
    internal static readonly BidirectionalLookup<string, ISymbol>
      knownSymbols = new BidirectionalLookup<string, ISymbol>(StringComparer.InvariantCulture, new SymbolNameComparer());

    private Constant constant = new Constant();
    private Variable variable = new Variable();

    private ProgramRootSymbol programRootSymbol = new ProgramRootSymbol();
    private StartSymbol startSymbol = new StartSymbol();

    static InfixExpressionParser() {
      // populate bidirectional lookup
      var dict = new Dictionary<string, ISymbol>
      {
        { "+", new Addition()},
        { "/", new Division()},
        { "*", new Multiplication()},
        { "-", new Subtraction()},
        { "EXP", new Exponential()},
        { "LOG", new Logarithm()},
        { "POW", new Power()},
        { "ROOT", new Root()},
        { "SQR", new Square() },
        { "SQRT", new SquareRoot() },
        { "SIN",new Sine()},
        { "COS", new Cosine()},
        { "TAN", new Tangent()},
        { "AIRYA", new AiryA()},
        { "AIRYB", new AiryB()},
        { "BESSEL", new Bessel()},
        { "COSINT", new CosineIntegral()},
        { "SININT", new SineIntegral()},
        { "HYPCOSINT", new HyperbolicCosineIntegral()},
        { "HYPSININT", new HyperbolicSineIntegral()},
        { "FRESNELSININT", new FresnelSineIntegral()},
        { "FRESNELCOSINT", new FresnelCosineIntegral()},
        { "NORM", new Norm()},
        { "ERF", new Erf()},
        { "GAMMA", new Gamma()},
        { "PSI", new Psi()},
        { "DAWSON", new Dawson()},
        { "EXPINT", new ExponentialIntegralEi()},
        { "MEAN", new Average()},
        { "IF", new IfThenElse()},
        { ">", new GreaterThan()},
        { "<", new LessThan()},
        { "AND", new And()},
        { "OR", new Or()},
        { "NOT", new Not()},
        { "XOR", new Xor()},
        { "DIFF", new Derivative()},
      };


      foreach (var kvp in dict) {
        knownSymbols.Add(kvp.Key, kvp.Value);
      }
    }

    public ISymbolicExpressionTree Parse(string str) {
      ISymbolicExpressionTreeNode root = programRootSymbol.CreateTreeNode();
      ISymbolicExpressionTreeNode start = startSymbol.CreateTreeNode();
      var allTokens = GetAllTokens(str).ToArray();
      ISymbolicExpressionTreeNode mainBranch = ParseS(new Queue<Token>(allTokens));

      // only a main branch was given => insert the main branch into the default tree template
      root.AddSubtree(start);
      start.AddSubtree(mainBranch);
      return new SymbolicExpressionTree(root);
    }

    private IEnumerable<Token> GetAllTokens(string str) {
      int pos = 0;
      while (true) {
        while (pos < str.Length && Char.IsWhiteSpace(str[pos])) pos++;
        if (pos >= str.Length) {
          yield return new Token { TokenType = TokenType.End, strVal = "" };
          yield break;
        }
        if (char.IsDigit(str[pos])) {
          // read number (=> read until white space or operator)
          var sb = new StringBuilder();
          sb.Append(str[pos]);
          pos++;
          while (pos < str.Length && !char.IsWhiteSpace(str[pos])
            && (str[pos] != '+' || str[pos-1] == 'e' || str[pos-1] == 'E')     // continue reading exponents
            && (str[pos] != '-' || str[pos - 1] == 'e' || str[pos - 1] == 'E')
            && str[pos] != '*'           
            && str[pos] != '/'
            && str[pos] != ')') {
            sb.Append(str[pos]);
            pos++;
          }
          double dblVal;
          if (double.TryParse(sb.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out dblVal))
            yield return new Token { TokenType = TokenType.Number, strVal = sb.ToString(), doubleVal = dblVal };
          else yield return new Token { TokenType = TokenType.NA, strVal = sb.ToString() };
        } else if (char.IsLetter(str[pos]) || str[pos] == '_') {
          // read ident
          var sb = new StringBuilder();
          sb.Append(str[pos]);
          pos++;
          while (pos < str.Length &&
            (char.IsLetter(str[pos]) || str[pos] == '_' || char.IsDigit(str[pos]))) {
            sb.Append(str[pos]);
            pos++;
          }
          yield return new Token { TokenType = TokenType.Identifier, strVal = sb.ToString() };
        } else if (str[pos] == '"') {
          // read to next " 
          pos++;
          var sb = new StringBuilder();
          while (pos < str.Length && str[pos] != '"') {
            sb.Append(str[pos]);
            pos++;
          }
          if (pos < str.Length && str[pos] == '"') {
            pos++; // skip "
            yield return new Token { TokenType = TokenType.Identifier, strVal = sb.ToString() };
          } else
            yield return new Token { TokenType = TokenType.NA };

        } else if (str[pos] == '\'') {
          // read to next '
          pos++;
          var sb = new StringBuilder();
          while (pos < str.Length && str[pos] != '\'') {
            sb.Append(str[pos]);
            pos++;
          }
          if (pos < str.Length && str[pos] == '\'') {
            pos++; // skip '
            yield return new Token { TokenType = TokenType.Identifier, strVal = sb.ToString() };
          } else
            yield return new Token { TokenType = TokenType.NA };
        } else if (str[pos] == '+') {
          pos++;
          yield return new Token { TokenType = TokenType.Operator, strVal = "+" };
        } else if (str[pos] == '-') {
          pos++;
          yield return new Token { TokenType = TokenType.Operator, strVal = "-" };
        } else if (str[pos] == '/') {
          pos++;
          yield return new Token { TokenType = TokenType.Operator, strVal = "/" };
        } else if (str[pos] == '*') {
          pos++;
          yield return new Token { TokenType = TokenType.Operator, strVal = "*" };
        } else if (str[pos] == '(') {
          pos++;
          yield return new Token { TokenType = TokenType.LeftPar, strVal = "(" };
        } else if (str[pos] == ')') {
          pos++;
          yield return new Token { TokenType = TokenType.RightPar, strVal = ")" };
        }
      }
    }

    // S = Expr EOF
    // Expr = ['-' | '+'] Term { '+' Term | '-' Term }
    // Term = Fact { '*' Fact | '/' Fact }
    // Fact = '(' Expr ')' | funcId '(' Expr ')' | varId | number
    private ISymbolicExpressionTreeNode ParseS(Queue<Token> tokens) {
      var expr = ParseExpr(tokens);

      var endTok = tokens.Dequeue();
      if (endTok.TokenType != TokenType.End)
        throw new ArgumentException(string.Format("Expected end of expression (got {0})", endTok.strVal));

      return expr;
    }
    private ISymbolicExpressionTreeNode ParseExpr(Queue<Token> tokens) {
      var next = tokens.Peek();
      var posTerms = new List<ISymbolicExpressionTreeNode>();
      var negTerms = new List<ISymbolicExpressionTreeNode>();
      bool negateFirstTerm = false;
      if (next.TokenType == TokenType.Operator && (next.strVal == "+" || next.strVal == "-")) {
        tokens.Dequeue();
        if (next.strVal == "-")
          negateFirstTerm = true;
      }
      var t = ParseTerm(tokens);
      if (negateFirstTerm) negTerms.Add(t);
      else posTerms.Add(t);

      next = tokens.Peek();
      while (next.strVal == "+" || next.strVal == "-") {
        switch (next.strVal) {
          case "+": {
              tokens.Dequeue();
              var term = ParseTerm(tokens);
              posTerms.Add(term);
              break;
            }
          case "-": {
              tokens.Dequeue();
              var term = ParseTerm(tokens);
              negTerms.Add(term);
              break;
            }
        }
        next = tokens.Peek();
      }

      var sum = GetSymbol("+").CreateTreeNode();
      foreach (var posTerm in posTerms) sum.AddSubtree(posTerm);
      if (negTerms.Any()) {
        if (negTerms.Count == 1) {
          var sub = GetSymbol("-").CreateTreeNode();
          sub.AddSubtree(negTerms.Single());
          sum.AddSubtree(sub);
        } else {
          var sumNeg = GetSymbol("+").CreateTreeNode();
          foreach (var negTerm in negTerms) sumNeg.AddSubtree(negTerm);

          var constNode = (ConstantTreeNode)constant.CreateTreeNode();
          constNode.Value = -1.0;
          var prod = GetSymbol("*").CreateTreeNode();
          prod.AddSubtree(constNode);
          prod.AddSubtree(sumNeg);

          sum.AddSubtree(prod);
        }
      }
      if (sum.SubtreeCount == 1) return sum.Subtrees.First();
      else return sum;
    }

    private ISymbol GetSymbol(string tok) {
      var symb = knownSymbols.GetByFirst(tok).FirstOrDefault();
      if (symb == null) throw new ArgumentException(string.Format("Unknown token {0} found.", tok));
      return symb;
    }

    // Term = Fact { '*' Fact | '/' Fact }
    private ISymbolicExpressionTreeNode ParseTerm(Queue<Token> tokens) {
      var factors = new List<ISymbolicExpressionTreeNode>();
      var firstFactor = ParseFact(tokens);
      factors.Add(firstFactor);

      var next = tokens.Peek();
      while (next.strVal == "*" || next.strVal == "/") {
        switch (next.strVal) {
          case "*": {
              tokens.Dequeue();
              var fact = ParseFact(tokens);
              factors.Add(fact);
              break;
            }
          case "/": {
              tokens.Dequeue();
              var invFact = ParseFact(tokens);
              var divNode = GetSymbol("/").CreateTreeNode(); // 1/x
              divNode.AddSubtree(invFact);
              factors.Add(divNode);
              break;
            }
        }

        next = tokens.Peek();
      }
      if (factors.Count == 1) return factors.First();
      else {
        var prod = GetSymbol("*").CreateTreeNode();
        foreach (var f in factors) prod.AddSubtree(f);
        return prod;
      }
    }

    // Fact = '(' Expr ')' | funcId '(' Expr ')' | varId | number
    private ISymbolicExpressionTreeNode ParseFact(Queue<Token> tokens) {
      var next = tokens.Peek();
      if (next.TokenType == TokenType.LeftPar) {
        tokens.Dequeue();
        var expr = ParseExpr(tokens);
        var rPar = tokens.Dequeue();
        if (rPar.TokenType != TokenType.RightPar)
          throw new ArgumentException("expected )");
        return expr;
      } else if (next.TokenType == TokenType.Identifier) {
        var idTok = tokens.Dequeue();
        if (tokens.Peek().TokenType == TokenType.LeftPar) {
          // function identifier
          var funcId = idTok.strVal.ToUpperInvariant();

          var funcNode = GetSymbol(funcId).CreateTreeNode();
          var lPar = tokens.Dequeue();
          if (lPar.TokenType != TokenType.LeftPar)
            throw new ArgumentException("expected (");
          var expr = ParseExpr(tokens);
          var rPar = tokens.Dequeue();
          if (rPar.TokenType != TokenType.RightPar)
            throw new ArgumentException("expected )");

          funcNode.AddSubtree(expr);
          return funcNode;
        } else {
          // variable
          var varNode = (VariableTreeNode)variable.CreateTreeNode();
          varNode.Weight = 1.0;
          varNode.VariableName = idTok.strVal;
          return varNode;
        }
      } else if (next.TokenType == TokenType.Number) {
        var numTok = tokens.Dequeue();
        var constNode = (ConstantTreeNode)constant.CreateTreeNode();
        constNode.Value = numTok.doubleVal;
        return constNode;
      } else {
        throw new ArgumentException(string.Format("unexpected token in expression {0}", next.strVal));
      }
    }
  }
}
