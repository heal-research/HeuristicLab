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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoDiff;
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

    private static readonly Func<Term, UnaryFunc> erf = UnaryFunc.Factory(
      eval: alglib.errorfunction,
      diff: x => 2.0 * Math.Exp(-(x * x)) / Math.Sqrt(Math.PI));

    private static readonly Func<Term, UnaryFunc> norm = UnaryFunc.Factory(
      eval: alglib.normaldistribution,
      diff: x => -(Math.Exp(-(x * x)) * Math.Sqrt(Math.Exp(x * x)) * x) / Math.Sqrt(2 * Math.PI));

    #endregion

    public static bool TryConvertToAutoDiff(ISymbolicExpressionTree tree, bool makeVariableWeightsVariable, bool addLinearScalingTerms,
      out List<DataForVariable> parameters, out double[] initialConstants,
      out ParametricFunction func,
      out ParametricFunctionGradient func_grad) {

      // use a transformator object which holds the state (variable list, parameter list, ...) for recursive transformation of the tree
      var transformator = new TreeToAutoDiffTermConverter(makeVariableWeightsVariable, addLinearScalingTerms);
      AutoDiff.Term term;
      try {
        term = transformator.ConvertToAutoDiff(tree.Root.GetSubtree(0));
        var parameterEntries = transformator.parameters.ToArray(); // guarantee same order for keys and values
        var compiledTerm = term.Compile(transformator.variables.ToArray(),
          parameterEntries.Select(kvp => kvp.Value).ToArray());
        parameters = new List<DataForVariable>(parameterEntries.Select(kvp => kvp.Key));
        initialConstants = transformator.initialConstants.ToArray();
        func = (vars, @params) => compiledTerm.Evaluate(vars, @params);
        func_grad = (vars, @params) => compiledTerm.Differentiate(vars, @params);
        return true;
      } catch (ConversionException) {
        func = null;
        func_grad = null;
        parameters = null;
        initialConstants = null;
      }
      return false;
    }

    // state for recursive transformation of trees 
    private readonly
    List<double> initialConstants;
    private readonly Dictionary<DataForVariable, AutoDiff.Variable> parameters;
    private readonly List<AutoDiff.Variable> variables;
    private readonly bool makeVariableWeightsVariable;
    private readonly bool addLinearScalingTerms;

    private TreeToAutoDiffTermConverter(bool makeVariableWeightsVariable, bool addLinearScalingTerms) {
      this.makeVariableWeightsVariable = makeVariableWeightsVariable;
      this.addLinearScalingTerms = addLinearScalingTerms;
      this.initialConstants = new List<double>();
      this.parameters = new Dictionary<DataForVariable, AutoDiff.Variable>();
      this.variables = new List<AutoDiff.Variable>();
    }

    private AutoDiff.Term ConvertToAutoDiff(ISymbolicExpressionTreeNode node) {
      if (node.Symbol is Constant) {
        initialConstants.Add(((ConstantTreeNode)node).Value);
        var var = new AutoDiff.Variable();
        variables.Add(var);
        return var;
      }
      if (node.Symbol is Variable || node.Symbol is BinaryFactorVariable) {
        var varNode = node as VariableTreeNodeBase;
        var factorVarNode = node as BinaryFactorVariableTreeNode;
        // factor variable values are only 0 or 1 and set in x accordingly
        var varValue = factorVarNode != null ? factorVarNode.VariableValue : string.Empty;
        var par = FindOrCreateParameter(parameters, varNode.VariableName, varValue);

        if (makeVariableWeightsVariable) {
          initialConstants.Add(varNode.Weight);
          var w = new AutoDiff.Variable();
          variables.Add(w);
          return AutoDiff.TermBuilder.Product(w, par);
        } else {
          return varNode.Weight * par;
        }
      }
      if (node.Symbol is FactorVariable) {
        var factorVarNode = node as FactorVariableTreeNode;
        var products = new List<Term>();
        foreach (var variableValue in factorVarNode.Symbol.GetVariableValues(factorVarNode.VariableName)) {
          var par = FindOrCreateParameter(parameters, factorVarNode.VariableName, variableValue);

          initialConstants.Add(factorVarNode.GetValue(variableValue));
          var wVar = new AutoDiff.Variable();
          variables.Add(wVar);

          products.Add(AutoDiff.TermBuilder.Product(wVar, par));
        }
        return AutoDiff.TermBuilder.Sum(products);
      }
      if (node.Symbol is LaggedVariable) {
        var varNode = node as LaggedVariableTreeNode;
        var par = FindOrCreateParameter(parameters, varNode.VariableName, string.Empty, varNode.Lag);

        if (makeVariableWeightsVariable) {
          initialConstants.Add(varNode.Weight);
          var w = new AutoDiff.Variable();
          variables.Add(w);
          return AutoDiff.TermBuilder.Product(w, par);
        } else {
          return varNode.Weight * par;
        }
      }
      if (node.Symbol is Addition) {
        List<AutoDiff.Term> terms = new List<Term>();
        foreach (var subTree in node.Subtrees) {
          terms.Add(ConvertToAutoDiff(subTree));
        }
        return AutoDiff.TermBuilder.Sum(terms);
      }
      if (node.Symbol is Subtraction) {
        List<AutoDiff.Term> terms = new List<Term>();
        for (int i = 0; i < node.SubtreeCount; i++) {
          AutoDiff.Term t = ConvertToAutoDiff(node.GetSubtree(i));
          if (i > 0) t = -t;
          terms.Add(t);
        }
        if (terms.Count == 1) return -terms[0];
        else return AutoDiff.TermBuilder.Sum(terms);
      }
      if (node.Symbol is Multiplication) {
        List<AutoDiff.Term> terms = new List<Term>();
        foreach (var subTree in node.Subtrees) {
          terms.Add(ConvertToAutoDiff(subTree));
        }
        if (terms.Count == 1) return terms[0];
        else return terms.Aggregate((a, b) => new AutoDiff.Product(a, b));
      }
      if (node.Symbol is Division) {
        List<AutoDiff.Term> terms = new List<Term>();
        foreach (var subTree in node.Subtrees) {
          terms.Add(ConvertToAutoDiff(subTree));
        }
        if (terms.Count == 1) return 1.0 / terms[0];
        else return terms.Aggregate((a, b) => new AutoDiff.Product(a, 1.0 / b));
      }
      if (node.Symbol is Logarithm) {
        return AutoDiff.TermBuilder.Log(
          ConvertToAutoDiff(node.GetSubtree(0)));
      }
      if (node.Symbol is Exponential) {
        return AutoDiff.TermBuilder.Exp(
          ConvertToAutoDiff(node.GetSubtree(0)));
      }
      if (node.Symbol is Square) {
        return AutoDiff.TermBuilder.Power(
          ConvertToAutoDiff(node.GetSubtree(0)), 2.0);
      }
      if (node.Symbol is SquareRoot) {
        return AutoDiff.TermBuilder.Power(
          ConvertToAutoDiff(node.GetSubtree(0)), 0.5);
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
          var alpha = new AutoDiff.Variable();
          var beta = new AutoDiff.Variable();
          variables.Add(beta);
          variables.Add(alpha);
          var t = ConvertToAutoDiff(node.GetSubtree(0));
          return t * alpha + beta;
        } else return ConvertToAutoDiff(node.GetSubtree(0));
      }
      throw new ConversionException();
    }


    // for each factor variable value we need a parameter which represents a binary indicator for that variable & value combination
    // each binary indicator is only necessary once. So we only create a parameter if this combination is not yet available
    private static Term FindOrCreateParameter(Dictionary<DataForVariable, AutoDiff.Variable> parameters,
      string varName, string varValue = "", int lag = 0) {
      var data = new DataForVariable(varName, varValue, lag);

      AutoDiff.Variable par = null;
      if (!parameters.TryGetValue(data, out par)) {
        // not found -> create new parameter and entries in names and values lists
        par = new AutoDiff.Variable();
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
