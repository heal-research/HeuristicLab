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
using System.Linq;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public static class DerivativeCalculator {
    public static ISymbolicExpressionTree Derive(ISymbolicExpressionTree tree, string variableName) {
      var mainBranch = tree.Root.GetSubtree(0).GetSubtree(0);
      var root = new ProgramRootSymbol().CreateTreeNode();
      root.AddSubtree(new StartSymbol().CreateTreeNode());
      var dTree = TreeSimplifier.GetSimplifiedTree(Derive(mainBranch, variableName));
      // var dTree = Derive(mainBranch, variableName);
      root.GetSubtree(0).AddSubtree(dTree);
      return new SymbolicExpressionTree(root);
    }

    private static Constant constantSy = new Constant();
    private static Addition addSy = new Addition();
    private static Subtraction subSy = new Subtraction();
    private static Multiplication mulSy = new Multiplication();
    private static Division divSy = new Division();

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
          var g = (ISymbolicExpressionTreeNode)branch.GetSubtree(1).Clone();
          var fprime = Derive(f, variableName);
          var gprime = Derive(g, variableName);
          var fgPrime = Sum(Product(f, gprime), Product(fprime, g));
          for (int i = 2; i < branch.SubtreeCount; i++) {
            var fg = Product((ISymbolicExpressionTreeNode)f.Clone(), (ISymbolicExpressionTreeNode)g.Clone());
            var h = (ISymbolicExpressionTreeNode)branch.GetSubtree(i).Clone();
            var hPrime = Derive(h, variableName);
            fgPrime = Sum(Product(fgPrime, h), Product(fg, hPrime));
          }
          return fgPrime;
        } else throw new ArgumentException();
      }
      if (branch.Symbol is Division) {
        // (f/g)' = (f'g - g'f) / g²
        if (branch.SubtreeCount == 1) {
          var g = (ISymbolicExpressionTreeNode)branch.GetSubtree(0).Clone();
          var gPrime = Product(CreateConstant(-1.0), Derive(g, variableName));
          var sqrNode = new Square().CreateTreeNode();
          sqrNode.AddSubtree(g);
          return Div(gPrime, sqrNode);
        } else if (branch.SubtreeCount == 2) {
          var f = (ISymbolicExpressionTreeNode)branch.GetSubtree(0).Clone();
          var g = (ISymbolicExpressionTreeNode)branch.GetSubtree(1).Clone();
          var fprime = Derive(f, variableName);
          var gprime = Derive(g, variableName);
          var sqrNode = new Square().CreateTreeNode();
          sqrNode.AddSubtree((ISymbolicExpressionTreeNode)branch.GetSubtree(1).Clone());
          return Div(Subtract(Product(fprime, g), Product(f, gprime)), sqrNode);
        } else throw new NotSupportedException();
      }
      if (branch.Symbol is Logarithm) {
        var f = (ISymbolicExpressionTreeNode)branch.GetSubtree(0).Clone();
        return Product(Div(CreateConstant(1.0), f), Derive(f, variableName));
      }
      if (branch.Symbol is Exponential) {
        var f = (ISymbolicExpressionTreeNode)branch.Clone();
        return Product(f, Derive(branch.GetSubtree(0), variableName));
      }
      if(branch.Symbol is Square) {
        var f = (ISymbolicExpressionTreeNode)branch.GetSubtree(0).Clone();
        return Product(Product(CreateConstant(2.0), f), Derive(f, variableName));
      }     
      if(branch.Symbol is SquareRoot) {
        var f = (ISymbolicExpressionTreeNode)branch.Clone();
        var u = (ISymbolicExpressionTreeNode)branch.GetSubtree(0).Clone();
        return Product(Div(CreateConstant(1.0), Product(CreateConstant(2.0), f)), Derive(u, variableName));
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
      throw new NotSupportedException(string.Format("Symbol {0} is not supported.", branch.Symbol));
    }


    private static ISymbolicExpressionTreeNode Product(ISymbolicExpressionTreeNode f, ISymbolicExpressionTreeNode g) {
      var product = mulSy.CreateTreeNode();
      product.AddSubtree(f);
      product.AddSubtree(g);
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
          !(n.Symbol is Sine) &&
          !(n.Symbol is Cosine) &&
          !(n.Symbol is StartSymbol)
        select n).Any();
      return !containsUnknownSymbol;
    }
  }
}
