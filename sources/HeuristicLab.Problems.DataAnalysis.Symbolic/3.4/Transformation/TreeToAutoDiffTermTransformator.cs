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
using System.Linq;
using AutoDiff;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public class TreeToAutoDiffTermTransformator {
    public delegate double ParametricFunction(double[] vars, double[] @params);
    public delegate Tuple<double[], double> ParametricFunctionGradient(double[] vars, double[] @params);

    #region derivations of functions
    // create function factory for arctangent
    private static readonly Func<Term, UnaryFunc> arctan = UnaryFunc.Factory(
      eval: Math.Atan,
      diff: x => 1 / (1 + x * x));
    private static readonly Func<Term, UnaryFunc> sin = UnaryFunc.Factory(
      eval: Math.Sin,
      diff: Math.Cos);
    private static readonly Func<Term, UnaryFunc> cos = UnaryFunc.Factory(
       eval: Math.Cos,
       diff: x => -Math.Sin(x));
    private static readonly Func<Term, UnaryFunc> tan = UnaryFunc.Factory(
      eval: Math.Tan,
      diff: x => 1 + Math.Tan(x) * Math.Tan(x));
    private static readonly Func<Term, UnaryFunc> erf = UnaryFunc.Factory(
      eval: alglib.errorfunction,
      diff: x => 2.0 * Math.Exp(-(x * x)) / Math.Sqrt(Math.PI));
    private static readonly Func<Term, UnaryFunc> norm = UnaryFunc.Factory(
      eval: alglib.normaldistribution,
      diff: x => -(Math.Exp(-(x * x)) * Math.Sqrt(Math.Exp(x * x)) * x) / Math.Sqrt(2 * Math.PI));

    #endregion

    public static bool TryTransformToAutoDiff(ISymbolicExpressionTree tree, bool makeVariableWeightsVariable,
      out string[] variableNames, out int[] lags, out double[] initialConstants,
      out ParametricFunction func,
      out ParametricFunctionGradient func_grad) {

      // use a transformator object which holds the state (variable list, parameter list, ...) for recursive transformation of the tree
      var transformator = new TreeToAutoDiffTermTransformator(makeVariableWeightsVariable);
      AutoDiff.Term term;
      var success = transformator.TryTransformToAutoDiff(tree.Root.GetSubtree(0), out term);
      if (success) {
        var compiledTerm = term.Compile(transformator.variables.ToArray(), transformator.parameters.ToArray());
        variableNames = transformator.variableNames.ToArray();
        lags = transformator.lags.ToArray();
        initialConstants = transformator.initialConstants.ToArray();
        func = (vars, @params) => compiledTerm.Evaluate(vars, @params);
        func_grad = (vars, @params) => compiledTerm.Differentiate(vars, @params);
      } else {
        func = null;
        func_grad = null;
        variableNames = null;
        lags = null;
        initialConstants = null;
      }
      return success;
    }

    // state for recursive transformation of trees 
    private readonly List<string> variableNames;
    private readonly List<int> lags;
    private readonly List<double> initialConstants;
    private readonly List<AutoDiff.Variable> parameters;
    private readonly List<AutoDiff.Variable> variables;
    private readonly bool makeVariableWeightsVariable;

    private TreeToAutoDiffTermTransformator(bool makeVariableWeightsVariable) {
      this.makeVariableWeightsVariable = makeVariableWeightsVariable;
      this.variableNames = new List<string>();
      this.lags = new List<int>();
      this.initialConstants = new List<double>();
      this.parameters = new List<AutoDiff.Variable>();
      this.variables = new List<AutoDiff.Variable>();
    }

    private bool TryTransformToAutoDiff(ISymbolicExpressionTreeNode node, out AutoDiff.Term term) {
      if (node.Symbol is Constant) {
        initialConstants.Add(((ConstantTreeNode)node).Value);
        var var = new AutoDiff.Variable();
        variables.Add(var);
        term = var;
        return true;
      }
      if (node.Symbol is Variable) {
        var varNode = node as VariableTreeNode;
        var par = new AutoDiff.Variable();
        parameters.Add(par);
        variableNames.Add(varNode.VariableName);
        lags.Add(0);

        if (makeVariableWeightsVariable) {
          initialConstants.Add(varNode.Weight);
          var w = new AutoDiff.Variable();
          variables.Add(w);
          term = AutoDiff.TermBuilder.Product(w, par);
        } else {
          term = varNode.Weight * par;
        }
        return true;
      }
      if (node.Symbol is LaggedVariable) {
        var varNode = node as LaggedVariableTreeNode;
        var par = new AutoDiff.Variable();
        parameters.Add(par);
        variableNames.Add(varNode.VariableName);
        lags.Add(varNode.Lag);

        if (makeVariableWeightsVariable) {
          initialConstants.Add(varNode.Weight);
          var w = new AutoDiff.Variable();
          variables.Add(w);
          term = AutoDiff.TermBuilder.Product(w, par);
        } else {
          term = varNode.Weight * par;
        }
        return true;
      }
      if (node.Symbol is Addition) {
        List<AutoDiff.Term> terms = new List<Term>();
        foreach (var subTree in node.Subtrees) {
          AutoDiff.Term t;
          if (!TryTransformToAutoDiff(subTree, out t)) {
            term = null;
            return false;
          }
          terms.Add(t);
        }
        term = AutoDiff.TermBuilder.Sum(terms);
        return true;
      }
      if (node.Symbol is Subtraction) {
        List<AutoDiff.Term> terms = new List<Term>();
        for (int i = 0; i < node.SubtreeCount; i++) {
          AutoDiff.Term t;
          if (!TryTransformToAutoDiff(node.GetSubtree(i), out t)) {
            term = null;
            return false;
          }
          if (i > 0) t = -t;
          terms.Add(t);
        }
        if (terms.Count == 1) term = -terms[0];
        else term = AutoDiff.TermBuilder.Sum(terms);
        return true;
      }
      if (node.Symbol is Multiplication) {
        List<AutoDiff.Term> terms = new List<Term>();
        foreach (var subTree in node.Subtrees) {
          AutoDiff.Term t;
          if (!TryTransformToAutoDiff(subTree, out t)) {
            term = null;
            return false;
          }
          terms.Add(t);
        }
        if (terms.Count == 1) term = terms[0];
        else term = terms.Aggregate((a, b) => new AutoDiff.Product(a, b));
        return true;

      }
      if (node.Symbol is Division) {
        List<AutoDiff.Term> terms = new List<Term>();
        foreach (var subTree in node.Subtrees) {
          AutoDiff.Term t;
          if (!TryTransformToAutoDiff(subTree, out t)) {
            term = null;
            return false;
          }
          terms.Add(t);
        }
        if (terms.Count == 1) term = 1.0 / terms[0];
        else term = terms.Aggregate((a, b) => new AutoDiff.Product(a, 1.0 / b));
        return true;
      }
      if (node.Symbol is Logarithm) {
        AutoDiff.Term t;
        if (!TryTransformToAutoDiff(node.GetSubtree(0), out t)) {
          term = null;
          return false;
        } else {
          term = AutoDiff.TermBuilder.Log(t);
          return true;
        }
      }
      if (node.Symbol is Exponential) {
        AutoDiff.Term t;
        if (!TryTransformToAutoDiff(node.GetSubtree(0), out t)) {
          term = null;
          return false;
        } else {
          term = AutoDiff.TermBuilder.Exp(t);
          return true;
        }
      }
      if (node.Symbol is Square) {
        AutoDiff.Term t;
        if (!TryTransformToAutoDiff(node.GetSubtree(0), out t)) {
          term = null;
          return false;
        } else {
          term = AutoDiff.TermBuilder.Power(t, 2.0);
          return true;
        }
      }
      if (node.Symbol is SquareRoot) {
        AutoDiff.Term t;
        if (!TryTransformToAutoDiff(node.GetSubtree(0), out t)) {
          term = null;
          return false;
        } else {
          term = AutoDiff.TermBuilder.Power(t, 0.5);
          return true;
        }
      }
      if (node.Symbol is Sine) {
        AutoDiff.Term t;
        if (!TryTransformToAutoDiff(node.GetSubtree(0), out t)) {
          term = null;
          return false;
        } else {
          term = sin(t);
          return true;
        }
      }
      if (node.Symbol is Cosine) {
        AutoDiff.Term t;
        if (!TryTransformToAutoDiff(node.GetSubtree(0), out t)) {
          term = null;
          return false;
        } else {
          term = cos(t);
          return true;
        }
      }
      if (node.Symbol is Tangent) {
        AutoDiff.Term t;
        if (!TryTransformToAutoDiff(node.GetSubtree(0), out t)) {
          term = null;
          return false;
        } else {
          term = tan(t);
          return true;
        }
      }
      if (node.Symbol is Erf) {
        AutoDiff.Term t;
        if (!TryTransformToAutoDiff(node.GetSubtree(0), out t)) {
          term = null;
          return false;
        } else {
          term = erf(t);
          return true;
        }
      }
      if (node.Symbol is Norm) {
        AutoDiff.Term t;
        if (!TryTransformToAutoDiff(node.GetSubtree(0), out t)) {
          term = null;
          return false;
        } else {
          term = norm(t);
          return true;
        }
      }
      if (node.Symbol is StartSymbol) {
        var alpha = new AutoDiff.Variable(); // TODO
        var beta = new AutoDiff.Variable();
        variables.Add(beta);
        variables.Add(alpha);
        AutoDiff.Term branchTerm;
        if (TryTransformToAutoDiff(node.GetSubtree(0), out branchTerm)) {
          term = branchTerm * alpha + beta;
          return true;
        } else {
          term = null;
          return false;
        }
      }
      term = null;
      return false;
    }


    public static bool IsCompatible(ISymbolicExpressionTree tree) {
      var containsUnknownSymbol = (
        from n in tree.Root.GetSubtree(0).IterateNodesPrefix()
        where
        !(n.Symbol is Variable) &&
        !(n.Symbol is LaggedVariable) &&
        !(n.Symbol is Constant) &&
        !(n.Symbol is Addition) &&
        !(n.Symbol is Subtraction) &&
        !(n.Symbol is Multiplication) &&
        !(n.Symbol is Division) &&
        !(n.Symbol is Logarithm) &&
        !(n.Symbol is Exponential) &&
        !(n.Symbol is SquareRoot) &&
        !(n.Symbol is Square) &&
        !(n.Symbol is Sine) &&
        !(n.Symbol is Cosine) &&
        !(n.Symbol is Tangent) &&
        !(n.Symbol is Erf) &&
        !(n.Symbol is Norm) &&
        !(n.Symbol is StartSymbol)
        select n).Any();
      return !containsUnknownSymbol;
    }
  }
}
