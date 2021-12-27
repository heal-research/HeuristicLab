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
using System.Globalization;
using System.Linq;
using System.Text;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// Parses mathematical expressions in infix form. E.g. x1 * (3.0 * x2 + x3)
  /// Identifier format (functions or variables): '_' | letter { '_' | letter | digit }
  /// Variables names and variable values can be set under quotes "" or '' because variable names might contain spaces. 
  ///   Variable = ident | " ident " | ' ident ' 
  /// It is also possible to use functions e.g. log("x1") or real-valued constants e.g. 3.1415 . 
  /// Variable names are case sensitive. Function names are not case sensitive.
  /// 
  /// 
  /// S             = Expr EOF
  /// Expr          = Term { '+' Term | '-' Term }
  /// Term          = Fact { '*' Fact | '/' Fact }
  /// Fact          = SimpleFact [ '^' SimpleFact ]
  /// SimpleFact    = '(' Expr ')'
  ///                 | '{' Expr '}'
  ///                 | 'LAG' '(' varId ',' ['+' | '-' ] number ')
  ///                 | funcId '(' ArgList ')'
  ///                 | VarExpr
  ///                 | number
  ///                 | ['+' | '-'] SimpleFact
  /// ArgList       = Expr { ',' Expr }
  /// VarExpr       = varId OptFactorPart
  /// OptFactorPart = [ ('=' varVal | '[' ['+' | '-' ]  number {',' ['+' | '-' ]  number } ']' ) ]
  /// varId         =  ident | ' ident ' | " ident "
  /// varVal        =  ident | ' ident ' | " ident "
  /// ident         =  '_' | letter { '_' | letter | digit }
  /// </summary>
  public sealed class InfixExpressionParser {
    private enum TokenType { Operator, Identifier, Number, LeftPar, RightPar, LeftBracket, RightBracket, LeftAngleBracket, RightAngleBracket, Comma, Eq, End, NA };
    private class Token {
      internal double doubleVal;
      internal string strVal;
      internal TokenType TokenType;
    }

    private class SymbolComparer : IEqualityComparer<ISymbol>, IComparer<ISymbol> {
      public int Compare(ISymbol x, ISymbol y) {
        return x.Name.CompareTo(y.Name);
      }

      public bool Equals(ISymbol x, ISymbol y) {
        return x.GetType() == y.GetType();
      }

      public int GetHashCode(ISymbol obj) {
        return obj.GetType().GetHashCode();
      }
    }
    // format name <-> symbol 
    // the lookup table is also used in the corresponding formatter
    internal static readonly BidirectionalLookup<string, ISymbol>
      knownSymbols = new BidirectionalLookup<string, ISymbol>(StringComparer.InvariantCulture, new SymbolComparer());

    private Number number = new Number();
    private Constant minusOne = new Constant() { Value = -1 };
    private Variable variable = new Variable();
    private BinaryFactorVariable binaryFactorVar = new BinaryFactorVariable();
    private FactorVariable factorVar = new FactorVariable();

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
        { "^", new Power() },
        { "ABS", new Absolute() },
        { "EXP", new Exponential()},
        { "LOG", new Logarithm()},
        { "POW", new Power() },
        { "ROOT", new Root()},
        { "SQR", new Square() },
        { "SQRT", new SquareRoot() },
        { "CUBE", new Cube() },
        { "CUBEROOT", new CubeRoot() },
        { "SIN",new Sine()},
        { "COS", new Cosine()},
        { "TAN", new Tangent()},
        { "TANH", new HyperbolicTangent()},
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
        { "AQ", new AnalyticQuotient() },
        { "MEAN", new Average()},
        { "IF", new IfThenElse()},
        { "GT", new GreaterThan()},
        { "LT", new LessThan()},
        { "AND", new And()},
        { "OR", new Or()},
        { "NOT", new Not()},
        { "XOR", new Xor()},
        { "DIFF", new Derivative()},
        { "LAG", new LaggedVariable() },
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
        while (pos < str.Length && char.IsWhiteSpace(str[pos])) pos++;
        if (pos >= str.Length) {
          yield return new Token { TokenType = TokenType.End, strVal = "" };
          yield break;
        }
        if (char.IsDigit(str[pos])) {
          // read number (=> read until white space or other symbol)
          var sb = new StringBuilder();
          sb.Append(str[pos]);
          pos++;
          while (pos < str.Length && !char.IsWhiteSpace(str[pos])
            && (str[pos] != '+' || str[pos - 1] == 'e' || str[pos - 1] == 'E')     // continue reading exponents
            && (str[pos] != '-' || str[pos - 1] == 'e' || str[pos - 1] == 'E')
            && str[pos] != '*'
            && str[pos] != '/'
            && str[pos] != '^'
            && str[pos] != ')'
            && str[pos] != ']'
            && str[pos] != '}'
            && str[pos] != ','
            && str[pos] != '>') {
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
        } else if (str[pos] == '^') {
          pos++;
          yield return new Token { TokenType = TokenType.Operator, strVal = "^" };
        } else if (str[pos] == '(') {
          pos++;
          yield return new Token { TokenType = TokenType.LeftPar, strVal = "(" };
        } else if (str[pos] == ')') {
          pos++;
          yield return new Token { TokenType = TokenType.RightPar, strVal = ")" };
        } else if (str[pos] == '[') {
          pos++;
          yield return new Token { TokenType = TokenType.LeftBracket, strVal = "[" };
        } else if (str[pos] == ']') {
          pos++;
          yield return new Token { TokenType = TokenType.RightBracket, strVal = "]" };
        } else if (str[pos] == '{') {
          pos++;
          yield return new Token { TokenType = TokenType.LeftPar, strVal = "{" };
        } else if (str[pos] == '}') {
          pos++;
          yield return new Token { TokenType = TokenType.RightPar, strVal = "}" };
        } else if (str[pos] == '=') {
          pos++;
          yield return new Token { TokenType = TokenType.Eq, strVal = "=" };
        } else if (str[pos] == ',') {
          pos++;
          yield return new Token { TokenType = TokenType.Comma, strVal = "," };
        } else if (str[pos] == '<') {
          pos++;
          yield return new Token { TokenType = TokenType.LeftAngleBracket, strVal = "<" };
        } else if (str[pos] == '>') {
          pos++;
          yield return new Token { TokenType = TokenType.RightAngleBracket, strVal = ">" };
        } else {
          throw new ArgumentException("Invalid character: " + str[pos]);
        }
      }
    }
    /// S             = Expr EOF
    private ISymbolicExpressionTreeNode ParseS(Queue<Token> tokens) {
      var expr = ParseExpr(tokens);

      var endTok = tokens.Dequeue();
      if (endTok.TokenType != TokenType.End)
        throw new ArgumentException(string.Format("Expected end of expression (got {0})", endTok.strVal));

      return expr;
    }

    /// Expr          = Term { '+' Term | '-' Term }
    private ISymbolicExpressionTreeNode ParseExpr(Queue<Token> tokens) {
      // build tree from bottom to top and left to right
      // a + b - c => ((a + b) - c)
      // a - b - c => ((a - b) - c)
      // and then flatten as far as possible
      var left = ParseTerm(tokens);

      var next = tokens.Peek();
      while (next.strVal == "+" || next.strVal == "-") {
        switch (next.strVal) {
          case "+": {
              tokens.Dequeue();
              var right = ParseTerm(tokens);
              var op = GetSymbol("+").CreateTreeNode();
              op.AddSubtree(left);
              op.AddSubtree(right);
              left = op;
              break;
            }
          case "-": {
              tokens.Dequeue();
              var right = ParseTerm(tokens);
              var op = GetSymbol("-").CreateTreeNode();
              op.AddSubtree(left);
              op.AddSubtree(right);
              left = op;
              break;
            }
        }
        next = tokens.Peek();
      }

      FoldLeftRecursive(left);
      return left;
    }

    private ISymbol GetSymbol(string tok) {
      var symb = knownSymbols.GetByFirst(tok).FirstOrDefault();
      if (symb == null) throw new ArgumentException(string.Format("Unknown token {0} found.", tok));
      return symb;
    }

    /// Term          = Fact { '*' Fact | '/' Fact }
    private ISymbolicExpressionTreeNode ParseTerm(Queue<Token> tokens) {
      // build tree from bottom to top and left to right
      // a / b * c => ((a / b) * c)
      // a / b / c => ((a / b) / c)
      // and then flatten as far as possible

      var left = ParseFact(tokens);

      var next = tokens.Peek();
      while (next.strVal == "*" || next.strVal == "/") {
        switch (next.strVal) {
          case "*": {
              tokens.Dequeue();
              var right = ParseFact(tokens);

              var op = GetSymbol("*").CreateTreeNode();
              op.AddSubtree(left);
              op.AddSubtree(right);
              left = op;
              break;
            }
          case "/": {
              tokens.Dequeue();
              var right = ParseFact(tokens);
              var op = GetSymbol("/").CreateTreeNode();
              op.AddSubtree(left);
              op.AddSubtree(right);
              left = op;
              break;
            }
        }

        next = tokens.Peek();
      }
      // remove all nodes where the child op is the same as the parent op
      // (a * b) * c) => (a * b * c)
      // (a / b) / c) => (a / b / c)

      FoldLeftRecursive(left);
      return left;
    }

    private void FoldLeftRecursive(ISymbolicExpressionTreeNode parent) {
      if (parent.SubtreeCount > 0) {
        var child = parent.GetSubtree(0);
        FoldLeftRecursive(child);
        if(parent.Symbol == child.Symbol) {
          parent.RemoveSubtree(0);
          for(int i=0;i<child.SubtreeCount; i++) {
            parent.InsertSubtree(i, child.GetSubtree(i));
          }
        }
      }
    }

    // Fact = SimpleFact ['^' SimpleFact]
    private ISymbolicExpressionTreeNode ParseFact(Queue<Token> tokens) {
      var expr = ParseSimpleFact(tokens);
      var next = tokens.Peek();
      if (next.TokenType == TokenType.Operator && next.strVal == "^") {
        tokens.Dequeue(); // skip;

        var p = GetSymbol("^").CreateTreeNode();
        p.AddSubtree(expr);
        p.AddSubtree(ParseSimpleFact(tokens));
        expr = p;
      }
      return expr;
    }


    /// SimpleFact   = '(' Expr ')' 
    ///                 | '{' Expr '}'
    ///                 | 'LAG' '(' varId ',' ['+' | '-' ] number ')'
    ///                 | funcId '(' ArgList ')
    ///                 | VarExpr
    ///                 | '<' 'num' [ '=' [ '+' | '-' ] number ] '>' 
    ///                 | number
    ///                 | ['+' | '-' ] SimpleFact
    /// ArgList       = Expr { ',' Expr }
    /// VarExpr       = varId OptFactorPart
    /// OptFactorPart = [ ('=' varVal | '[' ['+' | '-' ] number {',' ['+' | '-' ] number } ']' ) ]
    /// varId         =  ident | ' ident ' | " ident "
    /// varVal        =  ident | ' ident ' | " ident "
    /// ident         =  '_' | letter { '_' | letter | digit }
    private ISymbolicExpressionTreeNode ParseSimpleFact(Queue<Token> tokens) {
      var next = tokens.Peek();
      if (next.TokenType == TokenType.LeftPar) {
        var initPar = tokens.Dequeue(); // match par type
        var expr = ParseExpr(tokens);
        var rPar = tokens.Dequeue();
        if (rPar.TokenType != TokenType.RightPar)
          throw new ArgumentException("expected closing parenthesis");
        if (initPar.strVal == "(" && rPar.strVal == "}")
          throw new ArgumentException("expected closing )");
        if (initPar.strVal == "{" && rPar.strVal == ")")
          throw new ArgumentException("expected closing }");
        return expr;
      } else if (next.TokenType == TokenType.Identifier) {
        var idTok = tokens.Dequeue();
        if (tokens.Peek().TokenType == TokenType.LeftPar) {
          // function identifier or LAG
          return ParseFunctionOrLaggedVariable(tokens, idTok);
        } else {
          return ParseVariable(tokens, idTok);
        }
      } else if (next.TokenType == TokenType.LeftAngleBracket) {
        // '<' 'num' [ '=' ['+'|'-'] number ] '>'
        return ParseNumber(tokens);
      } else if(next.TokenType == TokenType.Operator && (next.strVal == "-" || next.strVal == "+")) {
        // ['+' | '-' ] SimpleFact
        if (tokens.Dequeue().strVal == "-") {
          var arg = ParseSimpleFact(tokens);
          if (arg is NumberTreeNode numNode) {
            numNode.Value *= -1;
            return numNode;
          } else if (arg is ConstantTreeNode constNode) {
            var constSy = new Constant { Value = -constNode.Value };
            return constSy.CreateTreeNode();
          } else if (arg is VariableTreeNode varNode) {
            varNode.Weight *= -1;
            return varNode;
          } else {
            var mul = GetSymbol("*").CreateTreeNode();
            var neg = minusOne.CreateTreeNode();
            mul.AddSubtree(neg);
            mul.AddSubtree(arg);
            return mul;
          }
        } else {
          return ParseSimpleFact(tokens);
        }
      } else if (next.TokenType == TokenType.Number) {
        // number
        var numTok = tokens.Dequeue();
        var constSy = new Constant { Value = numTok.doubleVal };
        return constSy.CreateTreeNode();
      } else {
        throw new ArgumentException(string.Format("unexpected token in expression {0}", next.strVal));
      }
    }

    private ISymbolicExpressionTreeNode ParseNumber(Queue<Token> tokens) {
      // we distinguish parameters and constants. The values of parameters can be changed.
      // a parameter is written as '<' 'num' [ '=' ['+'|'-'] number ] '>' with an optional initialization 
      Token numberTok = null;
      var leftAngleBracket = tokens.Dequeue();
      if (leftAngleBracket.TokenType != TokenType.LeftAngleBracket)
        throw new ArgumentException("opening bracket < expected");

      var idTok = tokens.Dequeue();
      if (idTok.TokenType != TokenType.Identifier || idTok.strVal.ToLower() != "num")
        throw new ArgumentException("string 'num' expected");

      var numNode = (NumberTreeNode)number.CreateTreeNode();

      if (tokens.Peek().TokenType == TokenType.Eq) {
        tokens.Dequeue(); // skip "="
        var next = tokens.Peek();
        if (next.strVal != "+" && next.strVal != "-" && next.TokenType != TokenType.Number)
          throw new ArgumentException("Expected '+', '-' or number.");

        var sign = 1.0;
        if (next.strVal == "+" || next.strVal == "-") {
          if (tokens.Dequeue().strVal == "-") sign = -1.0;
        } 
        if(tokens.Peek().TokenType != TokenType.Number) {
          throw new ArgumentException("Expected number.");
        }
        numberTok = tokens.Dequeue();
        numNode.Value = sign * numberTok.doubleVal;
      }

      var rightAngleBracket = tokens.Dequeue();
      if (rightAngleBracket.TokenType != TokenType.RightAngleBracket)
        throw new ArgumentException("closing bracket > expected");

      return numNode;
    }

    private ISymbolicExpressionTreeNode ParseVariable(Queue<Token> tokens, Token idTok) {
      // variable
      if (tokens.Peek().TokenType == TokenType.Eq) {
        // binary factor
        tokens.Dequeue(); // skip Eq
        var valTok = tokens.Dequeue();
        if (valTok.TokenType != TokenType.Identifier) throw new ArgumentException("expected identifier");
        var binFactorNode = (BinaryFactorVariableTreeNode)binaryFactorVar.CreateTreeNode();
        binFactorNode.Weight = 1.0;
        binFactorNode.VariableName = idTok.strVal;
        binFactorNode.VariableValue = valTok.strVal;
        return binFactorNode;
      } else if (tokens.Peek().TokenType == TokenType.LeftBracket) {
        // factor variable
        var factorVariableNode = (FactorVariableTreeNode)factorVar.CreateTreeNode();
        factorVariableNode.VariableName = idTok.strVal;

        tokens.Dequeue(); // skip [
        var weights = new List<double>();
        // at least one weight is necessary
        var sign = 1.0;
        if (tokens.Peek().TokenType == TokenType.Operator) {
          var opToken = tokens.Dequeue();
          if (opToken.strVal == "+") sign = 1.0;
          else if (opToken.strVal == "-") sign = -1.0;
          else throw new ArgumentException();
        }
        if (tokens.Peek().TokenType != TokenType.Number) throw new ArgumentException("number expected");
        var weightTok = tokens.Dequeue();
        weights.Add(sign * weightTok.doubleVal);
        while (tokens.Peek().TokenType == TokenType.Comma) {
          // skip comma
          tokens.Dequeue();
          if (tokens.Peek().TokenType == TokenType.Operator) {
            var opToken = tokens.Dequeue();
            if (opToken.strVal == "+") sign = 1.0;
            else if (opToken.strVal == "-") sign = -1.0;
            else throw new ArgumentException();
          }
          weightTok = tokens.Dequeue();
          if (weightTok.TokenType != TokenType.Number) throw new ArgumentException("number expected");
          weights.Add(sign * weightTok.doubleVal);
        }
        var rightBracketToken = tokens.Dequeue();
        if (rightBracketToken.TokenType != TokenType.RightBracket) throw new ArgumentException("closing bracket ] expected");
        factorVariableNode.Weights = weights.ToArray();
        return factorVariableNode;
      } else {
        // variable
        var varNode = (VariableTreeNode)variable.CreateTreeNode();
        varNode.Weight = 1.0;
        varNode.VariableName = idTok.strVal;
        return varNode;
      }
    }

    private ISymbolicExpressionTreeNode ParseFunctionOrLaggedVariable(Queue<Token> tokens, Token idTok) {
      var funcId = idTok.strVal.ToUpperInvariant();

      var funcNode = GetSymbol(funcId).CreateTreeNode();
      var lPar = tokens.Dequeue();
      if (lPar.TokenType != TokenType.LeftPar)
        throw new ArgumentException("expected (");

      // handle 'lag' specifically
      if (funcNode.Symbol is LaggedVariable) {
        ParseLaggedVariable(tokens, funcNode);
      } else {
        // functions
        var args = ParseArgList(tokens);
        // check number of arguments
        if (funcNode.Symbol.MinimumArity > args.Length || funcNode.Symbol.MaximumArity < args.Length) {
          throw new ArgumentException(string.Format("Symbol {0} requires between {1} and  {2} arguments.", funcId,
            funcNode.Symbol.MinimumArity, funcNode.Symbol.MaximumArity));
        }
        foreach (var arg in args) funcNode.AddSubtree(arg);
      }

      var rPar = tokens.Dequeue();
      if (rPar.TokenType != TokenType.RightPar)
        throw new ArgumentException("expected )");


      return funcNode;
    }

    private static void ParseLaggedVariable(Queue<Token> tokens, ISymbolicExpressionTreeNode funcNode) {
      var varId = tokens.Dequeue();
      if (varId.TokenType != TokenType.Identifier) throw new ArgumentException("Identifier expected. Format for lagged variables: \"lag(x, -1)\"");
      var comma = tokens.Dequeue();
      if (comma.TokenType != TokenType.Comma) throw new ArgumentException("',' expected, Format for lagged variables: \"lag(x, -1)\"");
      double sign = 1.0;
      if (tokens.Peek().strVal == "+" || tokens.Peek().strVal == "-") {
        // read sign
        var signTok = tokens.Dequeue();
        if (signTok.strVal == "-") sign = -1.0;
      }
      var lagToken = tokens.Dequeue();
      if (lagToken.TokenType != TokenType.Number) throw new ArgumentException("Number expected, Format for lagged variables: \"lag(x, -1)\"");
      if (!lagToken.doubleVal.IsAlmost(Math.Round(lagToken.doubleVal)))
        throw new ArgumentException("Time lags must be integer values");
      var laggedVarNode = funcNode as LaggedVariableTreeNode;
      laggedVarNode.VariableName = varId.strVal;
      laggedVarNode.Lag = (int)Math.Round(sign * lagToken.doubleVal);
      laggedVarNode.Weight = 1.0;
    }

    // ArgList = Expr { ',' Expr }
    private ISymbolicExpressionTreeNode[] ParseArgList(Queue<Token> tokens) {
      var exprList = new List<ISymbolicExpressionTreeNode>();
      exprList.Add(ParseExpr(tokens));
      while (tokens.Peek().TokenType != TokenType.RightPar) {
        var comma = tokens.Dequeue();
        if (comma.TokenType != TokenType.Comma) throw new ArgumentException("expected ',' ");
        exprList.Add(ParseExpr(tokens));
      }
      return exprList.ToArray();
    }
  }
}
