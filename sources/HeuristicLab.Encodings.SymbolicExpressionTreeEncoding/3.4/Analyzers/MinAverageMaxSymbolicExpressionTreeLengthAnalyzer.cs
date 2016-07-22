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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// An operator that tracks the min average and max length of symbolic expression trees.
  /// </summary>
  [Item("MinAverageMaxSymbolicExpressionTreeLengthAnalyzer", "An operator that tracks the min avgerage and max length of symbolic expression trees.")]
  [StorableClass]
  public sealed class MinAverageMaxSymbolicExpressionTreeLengthAnalyzer : AlgorithmOperator, ISymbolicExpressionTreeAnalyzer {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string SymbolicExpressionTreeLengthParameterName = "SymbolicExpressionTreeLength";
    private const string SymbolicExpressionTreeLengthsParameterName = "Symbolic expression tree length";
    private const string MinTreeLengthParameterName = "Minimal symbolic expression tree length";
    private const string AverageTreeLengthParameterName = "Average symbolic expression tree length";
    private const string MaxTreeLengthParameterName = "Maximal symbolic expression tree length";
    private const string ResultsParameterName = "Results";

    public bool EnabledByDefault {
      get { return true; }
    }

    #region parameter properties
    public IScopeTreeLookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (IScopeTreeLookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> SymbolicExpressionTreeLengthParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters[SymbolicExpressionTreeLengthParameterName]; }
    }
    public ValueLookupParameter<DataTable> SymbolicExpressionTreeLengthsParameter {
      get { return (ValueLookupParameter<DataTable>)Parameters[SymbolicExpressionTreeLengthsParameterName]; }
    }
    public ValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (ValueLookupParameter<VariableCollection>)Parameters[ResultsParameterName]; }
    }

    [Storable]
    private MinAverageMaxValueAnalyzer valueAnalyzer;
    [Storable]
    private UniformSubScopesProcessor subScopesProcessor;

    #endregion

    [StorableConstructor]
    private MinAverageMaxSymbolicExpressionTreeLengthAnalyzer(bool deserializing) : base() { }
    private MinAverageMaxSymbolicExpressionTreeLengthAnalyzer(MinAverageMaxSymbolicExpressionTreeLengthAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      valueAnalyzer = cloner.Clone(original.valueAnalyzer);
      subScopesProcessor = cloner.Clone(original.subScopesProcessor);
      AfterDeserialization();
    }
    public MinAverageMaxSymbolicExpressionTreeLengthAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression tree whose length should be calculated."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(SymbolicExpressionTreeLengthParameterName, "The length of the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<DataTable>(SymbolicExpressionTreeLengthsParameterName, "The data table to store the symbolic expression tree lengths."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>(ResultsParameterName, "The results collection where the analysis values should be stored."));

      subScopesProcessor = new UniformSubScopesProcessor();
      SymbolicExpressionTreeLengthCalculator lengthCalculator = new SymbolicExpressionTreeLengthCalculator();
      valueAnalyzer = new MinAverageMaxValueAnalyzer();

      subScopesProcessor.Depth.Value = SymbolicExpressionTreeParameter.Depth;
      lengthCalculator.SymbolicExpressionTreeParameter.ActualName = SymbolicExpressionTreeParameter.Name;
      lengthCalculator.SymbolicExpressionTreeLengthParameter.ActualName = SymbolicExpressionTreeLengthParameter.Name;
      valueAnalyzer.ValueParameter.ActualName = lengthCalculator.SymbolicExpressionTreeLengthParameter.Name;
      valueAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeLengthParameter.Depth;
      valueAnalyzer.AverageValueParameter.ActualName = AverageTreeLengthParameterName;
      valueAnalyzer.CollectAverageValueInResultsParameter.Value = new BoolValue(false);
      valueAnalyzer.MaxValueParameter.ActualName = MaxTreeLengthParameterName;
      valueAnalyzer.CollectMaxValueInResultsParameter.Value = new BoolValue(false);
      valueAnalyzer.MinValueParameter.ActualName = MinTreeLengthParameterName;
      valueAnalyzer.CollectMinValueInResultsParameter.Value = new BoolValue(false);
      valueAnalyzer.ValuesParameter.ActualName = SymbolicExpressionTreeLengthsParameter.Name;

      OperatorGraph.InitialOperator = subScopesProcessor;
      subScopesProcessor.Operator = lengthCalculator;
      lengthCalculator.Successor = null;
      subScopesProcessor.Successor = valueAnalyzer;
      valueAnalyzer.Successor = null;

      AfterDeserialization();
    }


    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      SymbolicExpressionTreeParameter.DepthChanged += new EventHandler(SymbolicExpressionTreeParameter_DepthChanged);
      SymbolicExpressionTreeLengthParameter.DepthChanged += new EventHandler(SymbolicExpressionTreeLengthParameter_DepthChanged);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MinAverageMaxSymbolicExpressionTreeLengthAnalyzer(this, cloner);
    }

    private void SymbolicExpressionTreeParameter_DepthChanged(object sender, EventArgs e) {
      OnDepthParameterChanged();
    }

    private void SymbolicExpressionTreeLengthParameter_DepthChanged(object sender, EventArgs e) {
      OnDepthParameterChanged();
    }

    private void OnDepthParameterChanged() {
      valueAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      subScopesProcessor.Depth.Value = SymbolicExpressionTreeParameter.Depth;
    }
  }
}
