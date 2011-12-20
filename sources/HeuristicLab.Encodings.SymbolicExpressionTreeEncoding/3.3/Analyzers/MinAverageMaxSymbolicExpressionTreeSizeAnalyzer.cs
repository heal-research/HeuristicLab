#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Interfaces;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Analyzers {
  /// <summary>
  /// An operator that tracks the min average and max tree size.
  /// </summary>
  [Item("MinAverageMaxSymbolicExpressionTreeSizeAnalyzer", "An operator that tracks the min avgerage and max tree size.")]
  [StorableClass]
  [NonDiscoverableType]
  public sealed class MinAverageMaxSymbolicExpressionTreeSizeAnalyzer : AlgorithmOperator, ISymbolicExpressionTreeAnalyzer {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string SymbolicExpressionTreeSizeParameterName = "SymbolicExpressionTreeSize";
    private const string SymbolicExpressionTreeSizesParameterName = "Symbolic expression tree size";
    private const string MinTreeSizeParameterName = "Min tree size";
    private const string AverageTreeSizeParameterName = "Average tree size";
    private const string MaxTreeSizeParameterName = "Max tree size";
    private const string ResultsParameterName = "Results";

    public bool EnabledByDefault {
      get { return true; }
    }

    #region parameter properties
    public ScopeTreeLookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ScopeTreeLookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> SymbolicExpressionTreeSizeParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters[SymbolicExpressionTreeSizeParameterName]; }
    }
    public ValueLookupParameter<DataTable> SymbolicExpressionTreeSizesParameter {
      get { return (ValueLookupParameter<DataTable>)Parameters[SymbolicExpressionTreeSizesParameterName]; }
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
    private MinAverageMaxSymbolicExpressionTreeSizeAnalyzer(bool deserializing) : base() { }
    private MinAverageMaxSymbolicExpressionTreeSizeAnalyzer(MinAverageMaxSymbolicExpressionTreeSizeAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      AfterDeserialization();
    }
    public MinAverageMaxSymbolicExpressionTreeSizeAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression tree whose size should be calculated."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(SymbolicExpressionTreeSizeParameterName, "The tree size of the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<DataTable>(SymbolicExpressionTreeSizesParameterName, "The data table to store the tree sizes."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>(ResultsParameterName, "The results collection where the analysis values should be stored."));

      subScopesProcessor = new UniformSubScopesProcessor();
      SymbolicExpressionTreeSizeCalculator sizeCalculator = new SymbolicExpressionTreeSizeCalculator();
      valueAnalyzer = new MinAverageMaxValueAnalyzer();

      subScopesProcessor.Depth.Value = SymbolicExpressionTreeParameter.Depth;
      sizeCalculator.SymbolicExpressionTreeParameter.ActualName = SymbolicExpressionTreeParameter.Name;
      sizeCalculator.SymbolicExpressionTreeSizeParameter.ActualName = SymbolicExpressionTreeSizeParameter.Name;
      valueAnalyzer.ValueParameter.ActualName = sizeCalculator.SymbolicExpressionTreeSizeParameter.Name;
      valueAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeSizeParameter.Depth;
      valueAnalyzer.AverageValueParameter.ActualName = AverageTreeSizeParameterName;
      valueAnalyzer.CollectAverageValueInResultsParameter.Value = new BoolValue(false);
      valueAnalyzer.MaxValueParameter.ActualName = MaxTreeSizeParameterName;
      valueAnalyzer.CollectMaxValueInResultsParameter.Value = new BoolValue(false);
      valueAnalyzer.MinValueParameter.ActualName = MinTreeSizeParameterName;
      valueAnalyzer.CollectMinValueInResultsParameter.Value = new BoolValue(false);
      valueAnalyzer.ValuesParameter.ActualName = SymbolicExpressionTreeSizesParameter.Name;

      OperatorGraph.InitialOperator = subScopesProcessor;
      subScopesProcessor.Operator = sizeCalculator;
      sizeCalculator.Successor = null;
      subScopesProcessor.Successor = valueAnalyzer;
      valueAnalyzer.Successor = null;

      AfterDeserialization();
    }


    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      SymbolicExpressionTreeParameter.DepthChanged += new EventHandler(SymbolicExpressionTreeParameter_DepthChanged);
      SymbolicExpressionTreeSizeParameter.DepthChanged += new EventHandler(SymbolicExpressionTreeSizeParameter_DepthChanged);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MinAverageMaxSymbolicExpressionTreeSizeAnalyzer(this, cloner);
    }

    private void SymbolicExpressionTreeParameter_DepthChanged(object sender, EventArgs e) {
      OnDepthParameterChanged();
    }

    private void SymbolicExpressionTreeSizeParameter_DepthChanged(object sender, EventArgs e) {
      OnDepthParameterChanged();
    }

    private void OnDepthParameterChanged() {
      valueAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      subScopesProcessor.Depth.Value = SymbolicExpressionTreeParameter.Depth;
    }
  }
}
