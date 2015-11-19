#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [Item("SymbolicExpressionTreeEncoding", "Describes a symbolic expression tree encoding.")]
  [StorableClass]
  public sealed class SymbolicExpressionTreeEncoding : Encoding<ISymbolicExpressionTreeCreator> {
    #region Encoding Parameters
    [Storable]
    private IFixedValueParameter<IntValue> treeLengthParameter;
    public IFixedValueParameter<IntValue> TreeLengthParameter {
      get { return treeLengthParameter; }
      set {
        if (value == null) throw new ArgumentNullException("Tree length parameter must not be null.");
        if (value.Value == null) throw new ArgumentNullException("Tree length parameter value must not be null.");
        if (treeLengthParameter == value) return;

        if (treeLengthParameter != null)
          Parameters.Remove(treeLengthParameter);

        treeLengthParameter = value;
        Parameters.Add(treeLengthParameter);
        OnLengthParameterChanged();
      }
    }

    [Storable]
    private IFixedValueParameter<IntValue> treeDepthParameter;
    public IFixedValueParameter<IntValue> TreeDepthParameter {
      get { return treeDepthParameter; }
      set {
        if (value == null) throw new ArgumentNullException("Tree length parameter must not be null.");
        if (value.Value == null) throw new ArgumentNullException("Tree length parameter value must not be null.");
        if (treeDepthParameter == value) return;

        if (treeDepthParameter != null)
          Parameters.Remove(treeDepthParameter);

        treeDepthParameter = value;
        Parameters.Add(treeDepthParameter);
        OnDepthParameterChanged();
      }
    }

    [Storable]
    private IValueParameter<ISymbolicExpressionGrammar> grammarParameter;
    public IValueParameter<ISymbolicExpressionGrammar> GrammarParameter {
      get { return grammarParameter; }
      set {
        if (value == null) throw new ArgumentNullException("Grammar parameter must not be null.");
        if (value.Value == null) throw new ArgumentNullException("Grammar parameter value must not be null.");
        if (grammarParameter == value) return;

        if (grammarParameter != null)
          Parameters.Remove(grammarParameter);

        grammarParameter = value;
        Parameters.Add(grammarParameter);
        OnGrammarParameterChanged();
      }
    }


    [Storable]
    private IFixedValueParameter<IntValue> functionDefinitionsParameter;
    public IFixedValueParameter<IntValue> FunctionDefinitionsParameter {
      get { return functionDefinitionsParameter; }
      set {
        if (value == null) throw new ArgumentNullException("Function definitions parameter must not be null.");
        if (value.Value == null) throw new ArgumentNullException("Function definitions parameter value must not be null.");
        if (functionDefinitionsParameter == value) return;

        if (functionDefinitionsParameter != null)
          Parameters.Remove(functionDefinitionsParameter);

        functionDefinitionsParameter = value;
        Parameters.Add(functionDefinitionsParameter);
        OnFunctionDefinitionsParameterChanged();
      }
    }

    [Storable]
    private IFixedValueParameter<IntValue> functionArgumentsParameter;
    public IFixedValueParameter<IntValue> FunctionArgumentsParameter {
      get { return functionArgumentsParameter; }
      set {
        if (value == null) throw new ArgumentNullException("Function arguments parameter must not be null.");
        if (value.Value == null) throw new ArgumentNullException("Function arguments parameter value must not be null.");
        if (functionArgumentsParameter == value) return;

        if (functionArgumentsParameter != null)
          Parameters.Remove(functionArgumentsParameter);

        functionArgumentsParameter = value;
        Parameters.Add(functionArgumentsParameter);
        OnFunctionArgumentsParameterChanged();
      }
    }
    #endregion

    #region Parameter properties
    public int TreeLength {
      get { return TreeLengthParameter.Value.Value; }
      set { TreeLengthParameter.Value.Value = value; }
    }

    public int TreeDepth {
      get { return TreeDepthParameter.Value.Value; }
      set { TreeDepthParameter.Value.Value = value; }
    }

    public ISymbolicExpressionGrammar Grammar {
      get { return GrammarParameter.Value; }
      set { GrammarParameter.Value = value; }
    }

    public int FunctionDefinitions {
      get { return FunctionDefinitionsParameter.Value.Value; }
      set { FunctionDefinitionsParameter.Value.Value = value; }
    }

    public int FunctionArguments {
      get { return FunctionArgumentsParameter.Value.Value; }
      set { FunctionArgumentsParameter.Value.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private SymbolicExpressionTreeEncoding(bool deserializing) : base(deserializing) { }
    public SymbolicExpressionTreeEncoding(ISymbolicExpressionGrammar grammar) : this("SymbolicExpressionTree", grammar, 50, 50) { }
    public SymbolicExpressionTreeEncoding(ISymbolicExpressionGrammar grammar, int maximumLength, int maximumDepth) : this("SymbolicExpressionTree", grammar, maximumLength, maximumDepth) { }
    public SymbolicExpressionTreeEncoding(string name, ISymbolicExpressionGrammar grammar, int maximumLength, int maximumDepth)
      : base(name) {
      treeLengthParameter = new FixedValueParameter<IntValue>("Maximum Tree Length", "Maximal length of the symbolic expression.", new IntValue(maximumLength));
      treeDepthParameter = new FixedValueParameter<IntValue>("Maximum Tree Depth", "Maximal depth of the symbolic expression. The minimum depth needed for the algorithm is 3 because two levels are reserved for the ProgramRoot and the Start symbol.", new IntValue(maximumDepth));
      grammarParameter = new ValueParameter<ISymbolicExpressionGrammar>("Grammar", "The grammar that should be used for symbolic expression tree.", grammar);
      functionDefinitionsParameter = new FixedValueParameter<IntValue>("Function Definitions", "Maximal number of automatically defined functions", new IntValue(0));
      functionArgumentsParameter = new FixedValueParameter<IntValue>("Function Arguments", "Maximal number of arguments of automatically defined functions.", new IntValue(0));

      Parameters.Add(treeLengthParameter);
      Parameters.Add(treeDepthParameter);
      Parameters.Add(grammarParameter);
      Parameters.Add(functionDefinitionsParameter);
      Parameters.Add(functionArgumentsParameter);

      SolutionCreator = new ProbabilisticTreeCreator();
      RegisterParameterEvents();
      DiscoverOperators();
    }

    private SymbolicExpressionTreeEncoding(SymbolicExpressionTreeEncoding original, Cloner cloner)
      : base(original, cloner) {
      treeLengthParameter = cloner.Clone(original.TreeLengthParameter);
      treeDepthParameter = cloner.Clone(original.TreeDepthParameter);
      grammarParameter = cloner.Clone(original.GrammarParameter);
      functionDefinitionsParameter = cloner.Clone(original.FunctionDefinitionsParameter);
      functionArgumentsParameter = cloner.Clone(original.FunctionArgumentsParameter);
      RegisterParameterEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicExpressionTreeEncoding(this, cloner);
    }

    #region changed events
    private void OnLengthParameterChanged() {
      RegisterLengthParameterEvents();
      ConfigureOperators(Operators);
    }
    private void OnDepthParameterChanged() {
      RegisterDepthParameterEvents();
      ConfigureOperators(Operators);
    }
    private void OnGrammarParameterChanged() {
      RegisterGrammarParameterEvents();
      FunctionArguments = Grammar.MaximumFunctionArguments;
      FunctionDefinitions = Grammar.MaximumFunctionDefinitions;
      ConfigureOperators(Operators);
    }

    private void OnFunctionDefinitionsParameterChanged() {
      RegisterFunctionDefinitionsParameterEvents();
      Grammar.MaximumFunctionDefinitions = FunctionDefinitions;
    }

    private void OnFunctionArgumentsParameterChanged() {
      RegisterFunctionArgumentsParameterEvetns();
      Grammar.MaximumFunctionArguments = FunctionArguments;
    }

    private void RegisterParameterEvents() {
      RegisterLengthParameterEvents();
      RegisterLengthParameterEvents();
      RegisterGrammarParameterEvents();
      RegisterFunctionDefinitionsParameterEvents();
      RegisterFunctionArgumentsParameterEvetns();
    }


    private void RegisterLengthParameterEvents() { }
    private void RegisterDepthParameterEvents() { }

    private void RegisterGrammarParameterEvents() {
      GrammarParameter.ValueChanged += (o, s) => {
        FunctionArguments = Grammar.MaximumFunctionArguments;
        FunctionDefinitions = Grammar.MaximumFunctionDefinitions;
        ConfigureOperators(Operators);
      };
      GrammarParameter.Value.Changed += (o, s) => {
        FunctionArguments = Grammar.MaximumFunctionArguments;
        FunctionDefinitions = Grammar.MaximumFunctionDefinitions;
        ConfigureOperators(Operators);
      };
    }

    private void RegisterFunctionDefinitionsParameterEvents() {
      FunctionDefinitionsParameter.Value.ValueChanged += (o, s) => Grammar.MaximumFunctionDefinitions = FunctionDefinitions;
    }
    private void RegisterFunctionArgumentsParameterEvetns() {
      FunctionArgumentsParameter.Value.ValueChanged += (o, s) => Grammar.MaximumFunctionArguments = FunctionArguments;
    }
    #endregion

    #region Operator discovery
    private static readonly IEnumerable<Type> encodingSpecificOperatorTypes;
    static SymbolicExpressionTreeEncoding() {
      encodingSpecificOperatorTypes = new List<Type>
      {
        typeof(ISymbolicExpressionTreeOperator),
        typeof(ISymbolicExpressionTreeCreator),
        typeof(ISymbolicExpressionTreeCrossover),
        typeof(ISymbolicExpressionTreeManipulator),
        typeof(ISymbolicExpressionTreeArchitectureAlteringOperator),
        typeof(ISymbolicExpressionTreeAnalyzer),
      };
    }

    private void DiscoverOperators() {
      var assembly = typeof(ISymbolicExpressionTreeOperator).Assembly;
      var discoveredTypes = ApplicationManager.Manager.GetTypes(encodingSpecificOperatorTypes, assembly, true, false, false);
      var operators = discoveredTypes.Select(t => (IOperator)Activator.CreateInstance(t));
      var newOperators = operators.Except(Operators, new TypeEqualityComparer<IOperator>()).ToList();

      ConfigureOperators(newOperators);
      foreach (var @operator in newOperators)
        AddOperator(@operator);
    }
    #endregion

    #region Specific operator wiring

    public override void ConfigureOperators(IEnumerable<IOperator> operators) {
      ConfigureTreeOperators(operators.OfType<ISymbolicExpressionTreeOperator>());
      ConfigureSizeConstraintOperators(operators.OfType<ISymbolicExpressionTreeSizeConstraintOperator>());
      ConfigureGrammarBaseOperators(operators.OfType<ISymbolicExpressionTreeGrammarBasedOperator>());
      ConfigureArchitectureAlteringOperators(operators.OfType<ISymbolicExpressionTreeArchitectureAlteringOperator>());

      ConfigureAnalyzers(operators.OfType<ISymbolicExpressionTreeAnalyzer>());
      ConfigureCreators(operators.OfType<ISymbolicExpressionTreeCreator>());
      ConfigureCrossovers(operators.OfType<ISymbolicExpressionTreeCrossover>());
      ConfigureManipulators(operators.OfType<ISymbolicExpressionTreeManipulator>());
    }

    private void ConfigureTreeOperators(IEnumerable<ISymbolicExpressionTreeOperator> treeOperators) {
      foreach (var treeOperator in treeOperators) {
        treeOperator.SymbolicExpressionTreeParameter.ActualName = Name;
        treeOperator.SymbolicExpressionTreeParameter.Hidden = true;
      }
    }

    private void ConfigureSizeConstraintOperators(IEnumerable<ISymbolicExpressionTreeSizeConstraintOperator> sizeConstraintOperators) {
      foreach (var sizeConstraintOperator in sizeConstraintOperators) {
        sizeConstraintOperator.MaximumSymbolicExpressionTreeLengthParameter.ActualName = TreeLengthParameter.Name;
        sizeConstraintOperator.MaximumSymbolicExpressionTreeLengthParameter.Hidden = true;
        sizeConstraintOperator.MaximumSymbolicExpressionTreeDepthParameter.ActualName = TreeDepthParameter.Name;
        sizeConstraintOperator.MaximumSymbolicExpressionTreeDepthParameter.Hidden = true;
      }
    }

    private void ConfigureGrammarBaseOperators(IEnumerable<ISymbolicExpressionTreeGrammarBasedOperator> grammarBasedOperators) {
      foreach (var grammarBasedOperator in grammarBasedOperators) {
        grammarBasedOperator.SymbolicExpressionTreeGrammarParameter.ActualName = GrammarParameter.Name;
        grammarBasedOperator.SymbolicExpressionTreeGrammarParameter.Hidden = true;
      }
    }

    private void ConfigureArchitectureAlteringOperators(IEnumerable<ISymbolicExpressionTreeArchitectureAlteringOperator> architectureAlteringOperators) {
      foreach (var architectureAlteringOperator in architectureAlteringOperators) {
        architectureAlteringOperator.MaximumFunctionDefinitionsParameter.ActualName = FunctionDefinitionsParameter.Name;
        architectureAlteringOperator.MaximumFunctionDefinitionsParameter.Hidden = true;
        architectureAlteringOperator.MaximumFunctionArgumentsParameter.ActualName = FunctionArgumentsParameter.Name;
        architectureAlteringOperator.MaximumFunctionArgumentsParameter.Hidden = true;
      }

    }

    private void ConfigureAnalyzers(IEnumerable<ISymbolicExpressionTreeAnalyzer> analyzers) {
      foreach (var analyzer in analyzers) {
        analyzer.SymbolicExpressionTreeParameter.ActualName = Name;
        analyzer.SymbolicExpressionTreeParameter.Hidden = true;
      }
      foreach (var analyzer in analyzers.OfType<SymbolicExpressionTreeLengthAnalyzer>()) {
        analyzer.MaximumSymbolicExpressionTreeLengthParameter.ActualName = TreeLengthParameter.Name;
        analyzer.MaximumSymbolicExpressionTreeLengthParameter.Hidden = true;
      }
    }

    private void ConfigureCreators(IEnumerable<ISymbolicExpressionTreeCreator> creators) {
      //Empty interface
      foreach (var creator in creators) { }
    }

    private void ConfigureCrossovers(IEnumerable<ISymbolicExpressionTreeCrossover> crossovers) {
      foreach (var crossover in crossovers) {
        crossover.ParentsParameter.ActualName = Name;
        crossover.ParentsParameter.Hidden = true;
      }
    }

    private void ConfigureManipulators(IEnumerable<ISymbolicExpressionTreeManipulator> manipulators) {
      //Empty interface
      foreach (var manipulator in manipulators) { }
    }
    #endregion
  }

  public static class IndividualExtensionMethods {
    public static ISymbolicExpressionTree SymbolicExpressionTree(this Individual individual) {
      var encoding = individual.GetEncoding<SymbolicExpressionTreeEncoding>();
      return individual.SymbolicExpressionTree(encoding.Name);
    }

    public static ISymbolicExpressionTree SymbolicExpressionTree(this Individual individual, string name) {
      return (ISymbolicExpressionTree)individual[name];
    }
  }
}
