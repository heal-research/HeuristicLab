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
  public static class DerivativeCalculator {
    public static ISymbolicExpressionTree Derive(ISymbolicExpressionTree tree, string variableName) {
      if (tree.Root.SubtreeCount != 1)
        throw new NotImplementedException("Derive is not implemented for symbolic expressions with automatically defined functions (ADF)");
      if (tree.Root.GetSubtree(0).SubtreeCount != 1)
        throw new NotImplementedException("Derive is not implemented for multi-variate symbolic expressions");
      var mainBranch = tree.Root.GetSubtree(0).GetSubtree(0);
      var root = new ProgramRootSymbol().CreateTreeNode();
      root.AddSubtree(new StartSymbol().CreateTreeNode());
      var dTree = TreeSimplifier.GetSimplifiedTree(Derive(mainBranch, variableName));
      //var dTree = Derive(mainBranch, variableName);
      root.GetSubtree(0).AddSubtree(dTree);
      return new SymbolicExpressionTree(root);
    }

    private static readonly Constant constantSy = new Constant();
    private static readonly Addition addSy = new Addition();
    private static readonly Subtraction subSy = new Subtraction();
    private static readonly Multiplication mulSy = new Multiplication();
    private static readonly Division divSy = new Division();
    private static readonly Cosine cosSy = new Cosine();
    private static readonly Square sqrSy = new Square();
    private static readonly Absolute absSy = new Absolute();
    private static readonly SquareRoot sqrtSy = new SquareRoot();

    public static ISymbolicExpressionTreeNode Derive(ISymbolicExpressionTreeNode branch, string variableName) {
      if (branch.Symbol is Constant) {
        return CreateConstant(0.0);
      }
      if (branch.Symbol is Variable) {
        var varNode = branch as VariableTreeNode;
        if (varNode.VariableName == variableName) {
          return CreateConstant(varNode.Weight);
        } else {
          return CreateConstant(0.0);
        }
      }
      if (branch.Symbol is Addition) {
        var sum = addSy.CreateTreeNode();
        foreach (var subTree in branch.Subtrees) {
          sum.AddSubtree(Derive(subTree, variableName));
        }
        return sum;
      }
      if (branch.Symbol is Subtraction) {
        var sum = subSy.CreateTreeNode();
        foreach (var subTree in branch.Subtrees) {
          sum.AddSubtree(Derive(subTree, variableName));
        }
        return sum;
      }
      if (branch.Symbol is Multiplication) {
        // (f * g)' = f'*g + f*g'
        // for multiple factors: (f * g * h)' = ((f*g) * h)' = (f*g)' * h + (f*g) * h' 

        if (branch.SubtreeCount >= 2) {
          var f = (ISymbolicExpressionTreeNode)branch.GetSubtree(0).Clone();
          var fprime = Derive(f, variableName);
          for (int i = 1; i < branch.SubtreeCount; i++) {
            var g = (ISymbolicExpressionTreeNode)branch.GetSubtree(i).Clone();
            var fg = Product((ISymbolicExpressionTreeNode)f.Clone(), (ISymbolicExpressionTreeNode)g.Clone());
            var gPrime = Derive(g, variableName);
            var fgPrime = Sum(Product(fprime, g), Product(gPrime, f));
            // prepare for next iteration
            f = fg;
            fprime = fgPrime;
          }
          return fprime;
        } else
          // multiplication with only one argument has no effect -> derive the argument
          return Derive(branch.GetSubtree(0), variableName);
      }
      if (branch.Symbol is Division) {
        // (f/g)' = (f'g - g'f) / g²
        if (branch.SubtreeCount == 1) {
          var g = (ISymbolicExpressionTreeNode)branch.GetSubtree(0).Clone();
          var gPrime = Product(CreateConstant(-1.0), Derive(g, variableName));
          var sqrNode = new Square().CreateTreeNode();
          sqrNode.AddSubtree(g);
          return Div(gPrime, sqrNode);
        } else {
          // for two subtrees:
          // (f/g)' = (f'g - fg')/g²

          // if there are more than 2 subtrees
          // div(x,y,z) is interpretered as (x/y)/z
          // which is the same as x / (y*z)

          // --> make a product of all but the first subtree and differentiate as for the 2-argument case above
          var f = (ISymbolicExpressionTreeNode)branch.GetSubtree(0).Clone();
          var g = Product(branch.Subtrees.Skip(1).Select(n => (ISymbolicExpressionTreeNode)n.Clone()));
          var fprime = Derive(f, variableName);
          var gprime = Derive(g, variableName);
          var gSqr = Square(g);
          return Div(Subtract(Product(fprime, g), Product(f, gprime)), gSqr);
        }
      }
      if (branch.Symbol is Logarithm) {
        var f = (ISymbolicExpressionTreeNode)branch.GetSubtree(0).Clone();
        return Product(Div(CreateConstant(1.0), f), Derive(f, variableName));
      }
      if (branch.Symbol is Exponential) {
        var f = (ISymbolicExpressionTreeNode)branch.Clone();
        return Product(f, Derive(branch.GetSubtree(0), variableName));
      }
      if (branch.Symbol is Square) {
        var f = (ISymbolicExpressionTreeNode)branch.GetSubtree(0).Clone();
        return Product(Product(CreateConstant(2.0), f), Derive(f, variableName));
      }
      if (branch.Symbol is SquareRoot) {
        var f = (ISymbolicExpressionTreeNode)branch.Clone();
        var u = (ISymbolicExpressionTreeNode)branch.GetSubtree(0).Clone();
        return Product(Div(CreateConstant(1.0), Product(CreateConstant(2.0), f)), Derive(u, variableName));
      }
      if (branch.Symbol is CubeRoot) {
        var f = (ISymbolicExpressionTreeNode)branch.Clone();
        var u = (ISymbolicExpressionTreeNode)branch.GetSubtree(0).Clone();
        return Product(Div(CreateConstant(1.0), Product(CreateConstant(3.0), Square(f))), Derive(u, variableName));  // 1/3 1/cbrt(f(x))^2 d/dx f(x)
      }
      if (branch.Symbol is Cube) {
        var f = (ISymbolicExpressionTreeNode)branch.GetSubtree(0).Clone();
        return Product(Product(CreateConstant(3.0), Square(f)), Derive(f, variableName));
      }
      if (branch.Symbol is Absolute) {
        var f = (ISymbolicExpressionTreeNode)branch.GetSubtree(0).Clone();
        var absf = Abs((ISymbolicExpressionTreeNode)f.Clone());
        return Product(Div(f, absf), Derive(f, variableName));
      }
      if (branch.Symbol is AnalyticQuotient) {
        // aq(a(x), b(x)) = a(x) / sqrt(b(x)²+1)
        var a = (ISymbolicExpressionTreeNode)branch.GetSubtree(0).Clone();
        var b = (ISymbolicExpressionTreeNode)branch.GetSubtree(1).Clone();

        var definition = Div(a, SquareRoot(Sum(Square(b), CreateConstant(1.0))));
        return Derive(definition, variableName);
      }
      if (branch.Symbol is Sine) {
        var u = (ISymbolicExpressionTreeNode)branch.GetSubtree(0).Clone();
        var cos = (new Cosine()).CreateTreeNode();
        cos.AddSubtree(u);
        return Product(cos, Derive(u, variableName));
      }
      if (branch.Symbol is Cosine) {
        var u = (ISymbolicExpressionTreeNode)branch.GetSubtree(0).Clone();
        var sin = (new Sine()).CreateTreeNode();
        sin.AddSubtree(u);
        return Product(CreateConstant(-1.0), Product(sin, Derive(u, variableName)));
      }
      if (branch.Symbol is Tangent) {
        // tan(x)' = 1 / cos²(x)
        var fxp = Derive(branch.GetSubtree(0), variableName);
        var u = (ISymbolicExpressionTreeNode)branch.GetSubtree(0).Clone();
        return Div(fxp, Square(Cosine(u)));
      }
      if (branch.Symbol is HyperbolicTangent) {
        // tanh(f(x))' = f(x)'sech²(f(x)) = f(x)'(1 - tanh²(f(x)))
        var fxp = Derive(branch.GetSubtree(0), variableName);
        var tanh = (ISymbolicExpressionTreeNode)branch.Clone();
        return Product(fxp, Subtract(CreateConstant(1.0), Square(tanh)));
      }
      throw new NotSupportedException(string.Format("Symbol {0} is not supported.", branch.Symbol));
    }


    private static ISymbolicExpressionTreeNode Product(ISymbolicExpressionTreeNode f, ISymbolicExpressionTreeNode g) {
      var product = mulSy.CreateTreeNode();
      product.AddSubtree(f);
      product.AddSubtree(g);
      return product;
    }
    private static ISymbolicExpressionTreeNode Product(IEnumerable<ISymbolicExpressionTreeNode> fs) {
      var product = mulSy.CreateTreeNode();
      foreach (var f in fs) product.AddSubtree(f);
      return product;
    }
    private static ISymbolicExpressionTreeNode Div(ISymbolicExpressionTreeNode f, ISymbolicExpressionTreeNode g) {
      var div = divSy.CreateTreeNode();
      div.AddSubtree(f);
      div.AddSubtree(g);
      return div;
    }

    private static ISymbolicExpressionTreeNode Sum(ISymbolicExpressionTreeNode f, ISymbolicExpressionTreeNode g) {
      var sum = addSy.CreateTreeNode();
      sum.AddSubtree(f);
      sum.AddSubtree(g);
      return sum;
    }
    private static ISymbolicExpressionTreeNode Subtract(ISymbolicExpressionTreeNode f, ISymbolicExpressionTreeNode g) {
      var sum = subSy.CreateTreeNode();
      sum.AddSubtree(f);
      sum.AddSubtree(g);
      return sum;
    }
    private static ISymbolicExpressionTreeNode Cosine(ISymbolicExpressionTreeNode f) {
      var cos = cosSy.CreateTreeNode();
      cos.AddSubtree(f);
      return cos;
    }
    private static ISymbolicExpressionTreeNode Abs(ISymbolicExpressionTreeNode f) {
      var abs = absSy.CreateTreeNode();
      abs.AddSubtree(f);
      return abs;
    }
    private static ISymbolicExpressionTreeNode Square(ISymbolicExpressionTreeNode f) {
      var sqr = sqrSy.CreateTreeNode();
      sqr.AddSubtree(f);
      return sqr;
    }
    private static ISymbolicExpressionTreeNode SquareRoot(ISymbolicExpressionTreeNode f) {
      var sqrt = sqrtSy.CreateTreeNode();
      sqrt.AddSubtree(f);
      return sqrt;
    }

    private static ISymbolicExpressionTreeNode CreateConstant(double v) {
      var constNode = (ConstantTreeNode)constantSy.CreateTreeNode();
      constNode.Value = v;
      return constNode;
    }

    public static bool IsCompatible(ISymbolicExpressionTree tree) {
      var containsUnknownSymbol = (
        from n in tree.Root.GetSubtree(0).IterateNodesPrefix()
        where
          !(n.Symbol is Variable) &&
          !(n.Symbol is Constant) &&
          !(n.Symbol is Addition) &&
          !(n.Symbol is Subtraction) &&
          !(n.Symbol is Multiplication) &&
          !(n.Symbol is Division) &&
          !(n.Symbol is Logarithm) &&
          !(n.Symbol is Exponential) &&
          !(n.Symbol is Square) &&
          !(n.Symbol is SquareRoot) &&
          !(n.Symbol is Cube) &&
          !(n.Symbol is CubeRoot) &&
          !(n.Symbol is Absolute) &&
          !(n.Symbol is AnalyticQuotient) &&
          !(n.Symbol is Sine) &&
          !(n.Symbol is Cosine) &&
          !(n.Symbol is Tangent) &&
          !(n.Symbol is StartSymbol)
        select n).Any();
      return !containsUnknownSymbol;
    }
  }
}
