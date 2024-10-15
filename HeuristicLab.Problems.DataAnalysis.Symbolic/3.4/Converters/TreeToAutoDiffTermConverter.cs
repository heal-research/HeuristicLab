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
using System.Runtime.Serialization;
using AutoDiff;
using AD = AutoDiff;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public class TreeToAutoDiffTermConverter {
    public delegate double ParametricFunction(double[] vars, double[] @params);

    public delegate Tuple<double[], double> ParametricFunctionGradient(double[] vars, double[] @params);

    #region helper class
    public class DataForVariable {
      public readonly string variableName;
      public readonly string variableValue; // for factor vars
      public readonly int lag;

      public DataForVariable(string varName, string varValue, int lag) {
        this.variableName = varName;
        this.variableValue = varValue;
        this.lag = lag;
      }

      public override bool Equals(object obj) {
        var other = obj as DataForVariable;
        if (other == null) return false;
        return other.variableName.Equals(this.variableName) &&
               other.variableValue.Equals(this.variableValue) &&
               other.lag == this.lag;
      }

      public override int GetHashCode() {
        return variableName.GetHashCode() ^ variableValue.GetHashCode() ^ lag;
      }
    }
    #endregion

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
    private static readonly Func<Term, UnaryFunc> tanh = UnaryFunc.Factory(
      eval: Math.Tanh,
      diff: x => 1 - Math.Tanh(x) * Math.Tanh(x));
    private static readonly Func<Term, UnaryFunc> erf = UnaryFunc.Factory(
      eval: alglib.errorfunction,
      diff: x => 2.0 * Math.Exp(-(x * x)) / Math.Sqrt(Math.PI));

    private static readonly Func<Term, UnaryFunc> norm = UnaryFunc.Factory(
      eval: alglib.normaldistribution,
      diff: x => -(Math.Exp(-(x * x)) * Math.Sqrt(Math.Exp(x * x)) * x) / Math.Sqrt(2 * Math.PI));

    private static readonly Func<Term, UnaryFunc> abs = UnaryFunc.Factory(
      eval: Math.Abs,
      diff: x => Math.Sign(x)
      );

    private static readonly Func<Term, UnaryFunc> cbrt = UnaryFunc.Factory(
      eval: x => x < 0 ? -Math.Pow(-x, 1.0 / 3) : Math.Pow(x, 1.0 / 3),
      diff: x => { var cbrt_x = x < 0 ? -Math.Pow(-x, 1.0 / 3) : Math.Pow(x, 1.0 / 3); return 1.0 / (3 * cbrt_x * cbrt_x); }
      );



    #endregion

    public static bool TryConvertToAutoDiff(ISymbolicExpressionTree tree, bool makeVariableWeightsVariable, bool addLinearScalingTerms,
      out List<DataForVariable> parameters, out double[] initialParamValues,
      out ParametricFunction func,
      out ParametricFunctionGradient func_grad) {

      return TryConvertToAutoDiff(tree, makeVariableWeightsVariable, addLinearScalingTerms, Enumerable.Empty<ISymbolicExpressionTreeNode>(),
        out parameters, out initialParamValues, out func, out func_grad);
    }

    public static bool TryConvertToAutoDiff(ISymbolicExpressionTree tree, bool makeVariableWeightsVariable, bool addLinearScalingTerms, IEnumerable<ISymbolicExpressionTreeNode> excludedNodes,
      out List<DataForVariable> parameters, out double[] initialParamValues,
      out ParametricFunction func,
      out ParametricFunctionGradient func_grad) {

      // use a transformator object which holds the state (variable list, parameter list, ...) for recursive transformation of the tree
      var transformator = new TreeToAutoDiffTermConverter(makeVariableWeightsVariable, addLinearScalingTerms, excludedNodes);
      AD.Term term;
      try {
        term = transformator.ConvertToAutoDiff(tree.Root.GetSubtree(0));
        var parameterEntries = transformator.parameters.ToArray(); // guarantee same order for keys and values
        var compiledTerm = term.Compile(transformator.variables.ToArray(),
          parameterEntries.Select(kvp => kvp.Value).ToArray());
        parameters = new List<DataForVariable>(parameterEntries.Select(kvp => kvp.Key));
        initialParamValues = transformator.initialParamValues.ToArray();
        func = (vars, @params) => compiledTerm.Evaluate(vars, @params);
        func_grad = (vars, @params) => compiledTerm.Differentiate(vars, @params);
        return true;
      } catch (ConversionException) {
        func = null;
        func_grad = null;
        parameters = null;
        initialParamValues = null;
      }
      return false;
    }

    // state for recursive transformation of trees 
    private readonly List<double> initialParamValues;
    private readonly Dictionary<DataForVariable, AD.Variable> parameters;
    private readonly List<AD.Variable> variables;
    private readonly bool makeVariableWeightsVariable;
    private readonly bool addLinearScalingTerms;
    private readonly HashSet<ISymbolicExpressionTreeNode> excludedNodes;

    private TreeToAutoDiffTermConverter(bool makeVariableWeightsVariable, bool addLinearScalingTerms, IEnumerable<ISymbolicExpressionTreeNode> excludedNodes) {
      this.makeVariableWeightsVariable = makeVariableWeightsVariable;
      this.addLinearScalingTerms = addLinearScalingTerms;
      this.excludedNodes = new HashSet<ISymbolicExpressionTreeNode>(excludedNodes);

      this.initialParamValues = new List<double>();
      this.parameters = new Dictionary<DataForVariable, AD.Variable>();
      this.variables = new List<AD.Variable>();
    }

    private AD.Term ConvertToAutoDiff(ISymbolicExpressionTreeNode node) {
      if (node.Symbol is Number) {
        initialParamValues.Add(((NumberTreeNode)node).Value);
        var var = new AD.Variable();
        variables.Add(var);
        return var;
      }
      if (node.Symbol is Constant) {
        // constants are fixed in autodiff
        return (node as ConstantTreeNode).Value;
      }
      if (node.Symbol is Variable || node.Symbol is BinaryFactorVariable) {
        var varNode = node as VariableTreeNodeBase;
        var factorVarNode = node as BinaryFactorVariableTreeNode;
        // factor variable values are only 0 or 1 and set in x accordingly
        var varValue = factorVarNode != null ? factorVarNode.VariableValue : string.Empty;
        var par = FindOrCreateParameter(parameters, varNode.VariableName, varValue);

        if (makeVariableWeightsVariable && !excludedNodes.Contains(node)) {
          initialParamValues.Add(varNode.Weight);
          var w = new AD.Variable();
          variables.Add(w);
          return AD.TermBuilder.Product(w, par);
        } else {
          return varNode.Weight * par;
        }
      }
      if (node.Symbol is FactorVariable) {
        var factorVarNode = node as FactorVariableTreeNode;
        var products = new List<Term>();
        foreach (var variableValue in factorVarNode.Symbol.GetVariableValues(factorVarNode.VariableName)) {
          var par = FindOrCreateParameter(parameters, factorVarNode.VariableName, variableValue);

          if (makeVariableWeightsVariable && !excludedNodes.Contains(node)) {
            initialParamValues.Add(factorVarNode.GetValue(variableValue));
            var wVar = new AD.Variable();
            variables.Add(wVar);

            products.Add(AD.TermBuilder.Product(wVar, par));
          } else {
            var weight = factorVarNode.GetValue(variableValue);
            products.Add(weight * par);
          }

        }
        return AD.TermBuilder.Sum(products);
      }
      if (node.Symbol is LaggedVariable) {
        var varNode = node as LaggedVariableTreeNode;
        var par = FindOrCreateParameter(parameters, varNode.VariableName, string.Empty, varNode.Lag);

        if (makeVariableWeightsVariable && !excludedNodes.Contains(node)) {
          initialParamValues.Add(varNode.Weight);
          var w = new AD.Variable();
          variables.Add(w);
          return AD.TermBuilder.Product(w, par);
        } else {
          return varNode.Weight * par;
        }
      }
      if (node.Symbol is Addition) {
        List<AD.Term> terms = new List<Term>();
        foreach (var subTree in node.Subtrees) {
          terms.Add(ConvertToAutoDiff(subTree));
        }
        return AD.TermBuilder.Sum(terms);
      }
      if (node.Symbol is Subtraction) {
        List<AD.Term> terms = new List<Term>();
        for (int i = 0; i < node.SubtreeCount; i++) {
          AD.Term t = ConvertToAutoDiff(node.GetSubtree(i));
          if (i > 0) t = -t;
          terms.Add(t);
        }
        if (terms.Count == 1) return -terms[0];
        else return AD.TermBuilder.Sum(terms);
      }
      if (node.Symbol is Multiplication) {
        List<AD.Term> terms = new List<Term>();
        foreach (var subTree in node.Subtrees) {
          terms.Add(ConvertToAutoDiff(subTree));
        }
        if (terms.Count == 1) return terms[0];
        else return terms.Aggregate((a, b) => new AD.Product(a, b));
      }
      if (node.Symbol is Division) {
        List<AD.Term> terms = new List<Term>();
        foreach (var subTree in node.Subtrees) {
          terms.Add(ConvertToAutoDiff(subTree));
        }
        if (terms.Count == 1) return 1.0 / terms[0];
        else return terms.Aggregate((a, b) => new AD.Product(a, 1.0 / b));
      }
      if (node.Symbol is Absolute) {
        var x1 = ConvertToAutoDiff(node.GetSubtree(0));
        return abs(x1);
      }
      if (node.Symbol is AnalyticQuotient) {
        var x1 = ConvertToAutoDiff(node.GetSubtree(0));
        var x2 = ConvertToAutoDiff(node.GetSubtree(1));
        return x1 / (TermBuilder.Power(1 + x2 * x2, 0.5));
      }
      if (node.Symbol is Logarithm) {
        return AD.TermBuilder.Log(
          ConvertToAutoDiff(node.GetSubtree(0)));
      }
      if (node.Symbol is Exponential) {
        return AD.TermBuilder.Exp(
          ConvertToAutoDiff(node.GetSubtree(0)));
      }
      if (node.Symbol is Square) {
        return AD.TermBuilder.Power(
          ConvertToAutoDiff(node.GetSubtree(0)), 2.0);
      }
      if (node.Symbol is SquareRoot) {
        return AD.TermBuilder.Power(
          ConvertToAutoDiff(node.GetSubtree(0)), 0.5);
      }
      if (node.Symbol is Cube) {
        return AD.TermBuilder.Power(
          ConvertToAutoDiff(node.GetSubtree(0)), 3.0);
      }
      if (node.Symbol is CubeRoot) {
        return cbrt(ConvertToAutoDiff(node.GetSubtree(0)));
      }
      if (node.Symbol is Power) {
        var powerNode = node.GetSubtree(1) as INumericTreeNode;
        if (powerNode == null)
          throw new NotSupportedException("Only numeric powers are allowed in parameter optimization. Try to use exp() and log() instead of the power symbol.");
        var intPower = Math.Truncate(powerNode.Value);
        if (intPower != powerNode.Value)
          throw new NotSupportedException("Only integer powers are allowed in parameter optimization. Try to use exp() and log() instead of the power symbol.");
        return AD.TermBuilder.Power(ConvertToAutoDiff(node.GetSubtree(0)), intPower);
      }
      if (node.Symbol is Sine) {
        return sin(
          ConvertToAutoDiff(node.GetSubtree(0)));
      }
      if (node.Symbol is Cosine) {
        return cos(
          ConvertToAutoDiff(node.GetSubtree(0)));
      }
      if (node.Symbol is Tangent) {
        return tan(
          ConvertToAutoDiff(node.GetSubtree(0)));
      }
      if (node.Symbol is HyperbolicTangent) {
        return tanh(
          ConvertToAutoDiff(node.GetSubtree(0)));
      }
      if (node.Symbol is Erf) {
        return erf(
          ConvertToAutoDiff(node.GetSubtree(0)));
      }
      if (node.Symbol is Norm) {
        return norm(
          ConvertToAutoDiff(node.GetSubtree(0)));
      }
      if (node.Symbol is StartSymbol) {
        if (addLinearScalingTerms) {
          // scaling variables α, β are given at the beginning of the parameter vector
          var alpha = new AD.Variable();
          var beta = new AD.Variable();
          variables.Add(beta);
          variables.Add(alpha);
          var t = ConvertToAutoDiff(node.GetSubtree(0));
          return t * alpha + beta;
        } else return ConvertToAutoDiff(node.GetSubtree(0));
      }
      if (node.Symbol is SubFunctionSymbol) {
        return ConvertToAutoDiff(node.GetSubtree(0));
      }
      throw new ConversionException();
    }


    // for each factor variable value we need a parameter which represents a binary indicator for that variable & value combination
    // each binary indicator is only necessary once. So we only create a parameter if this combination is not yet available
    private static Term FindOrCreateParameter(Dictionary<DataForVariable, AD.Variable> parameters,
      string varName, string varValue = "", int lag = 0) {
      var data = new DataForVariable(varName, varValue, lag);

      if (!parameters.TryGetValue(data, out AD.Variable par)) {
        // not found -> create new parameter and entries in names and values lists
        par = new AD.Variable();
        parameters.Add(data, par);
      }
      return par;
    }

    public static bool IsCompatible(ISymbolicExpressionTree tree) {
      var containsUnknownSymbol = (
        from n in tree.Root.GetSubtree(0).IterateNodesPrefix()
        where
          !(n.Symbol is Variable) &&
          !(n.Symbol is BinaryFactorVariable) &&
          !(n.Symbol is FactorVariable) &&
          !(n.Symbol is LaggedVariable) &&
          !(n.Symbol is Number) &&
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
          !(n.Symbol is HyperbolicTangent) &&
          !(n.Symbol is Erf) &&
          !(n.Symbol is Norm) &&
          !(n.Symbol is StartSymbol) &&
          !(n.Symbol is Absolute) &&
          !(n.Symbol is AnalyticQuotient) &&
          !(n.Symbol is Cube) &&
          !(n.Symbol is CubeRoot) &&
          !(n.Symbol is Power) &&
          !(n.Symbol is SubFunctionSymbol)
        select n).Any();
      return !containsUnknownSymbol;
    }
    #region exception class
    [Serializable]
    public class ConversionException : Exception {

      public ConversionException() {
      }

      public ConversionException(string message) : base(message) {
      }

      public ConversionException(string message, Exception inner) : base(message, inner) {
      }

      protected ConversionException(
        SerializationInfo info,
        StreamingContext context) : base(info, context) {
      }
    }
    #endregion
  }
}
