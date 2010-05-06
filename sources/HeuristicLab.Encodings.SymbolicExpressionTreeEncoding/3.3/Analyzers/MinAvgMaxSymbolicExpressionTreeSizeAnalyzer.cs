#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using System.Collections.Generic;
using HeuristicLab.Analysis;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Interfaces;
using System;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Analyzers {
  /// <summary>
  /// An operator that tracks the min avgerage and max tree size.
  /// </summary>
  [Item("MinAvgMaxSymbolicExpressionTreeSizeAnalyzer", "An operator that tracks the min avgerage and max tree size.")]
  [StorableClass]
  public sealed class MinAvgMaxSymbolicExpressionTreeSizeAnalyzer : AlgorithmOperator, ISymbolicExpressionTreeAnalyzer {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string SymbolicExpressionTreeSizeParameterName = "SymbolicExpressionTreeSize";
    private const string SymbolicExpressionTreeSizesParameterName = "SymbolicExpressionTreeSizes";
    private const string ResultsParameterName = "Results";

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
    private SymbolicExpressionTreeSizeCalculator sizeCalculator;

    #endregion
    public MinAvgMaxSymbolicExpressionTreeSizeAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression tree whose size should be calculated."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(SymbolicExpressionTreeSizeParameterName, "The tree size of the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<DataTable>(SymbolicExpressionTreeSizesParameterName, "The data table to store the tree sizes."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>(ResultsParameterName, "The results collection where the analysis values should be stored."));

      sizeCalculator = new SymbolicExpressionTreeSizeCalculator();
      valueAnalyzer = new MinAverageMaxValueAnalyzer();
      sizeCalculator.SymbolicExpressionTreeParameter.ActualName = SymbolicExpressionTreeParameter.Name;
      sizeCalculator.SymbolicExpressionTreeParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      sizeCalculator.SymbolicExpressionTreeSizeParameter.ActualName = SymbolicExpressionTreeSizeParameter.Name;
      sizeCalculator.SymbolicExpressionTreeSizeParameter.Depth = SymbolicExpressionTreeSizeParameter.Depth;
      valueAnalyzer.ValueParameter.ActualName = sizeCalculator.SymbolicExpressionTreeSizeParameter.Name;
      valueAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeSizeParameter.Depth;
      valueAnalyzer.ValuesParameter.ActualName = SymbolicExpressionTreeSizesParameter.Name;
      valueAnalyzer.ResultsParameter.ActualName = ResultsParameter.Name;
      valueAnalyzer.AverageValueParameter.ActualName = "Avg. Tree Size";
      valueAnalyzer.MaxValueParameter.ActualName = "Max Tree Size";
      valueAnalyzer.MinValueParameter.ActualName = "Min Tree Size";

      OperatorGraph.InitialOperator = sizeCalculator;
      sizeCalculator.Successor = valueAnalyzer;
      valueAnalyzer.Successor = null;

      Initialize();
    }

    [StorableConstructor]
    private MinAvgMaxSymbolicExpressionTreeSizeAnalyzer(bool deserializing) : base() { }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      SymbolicExpressionTreeParameter.DepthChanged += new EventHandler(SymbolicExpressionTreeParameter_DepthChanged);
      SymbolicExpressionTreeSizeParameter.DepthChanged += new EventHandler(SymbolicExpressionTreeSizeParameter_DepthChanged);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      MinAvgMaxSymbolicExpressionTreeSizeAnalyzer clone = (MinAvgMaxSymbolicExpressionTreeSizeAnalyzer)base.Clone(cloner);
      clone.Initialize();
      return clone;
    }

    private void SymbolicExpressionTreeParameter_DepthChanged(object sender, EventArgs e) {
      OnDepthParameterChanged();
    }

    private void SymbolicExpressionTreeSizeParameter_DepthChanged(object sender, EventArgs e) {
      OnDepthParameterChanged();
    }

    private void OnDepthParameterChanged() {
      valueAnalyzer.ValueParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      sizeCalculator.SymbolicExpressionTreeParameter.Depth = SymbolicExpressionTreeParameter.Depth;
      sizeCalculator.SymbolicExpressionTreeSizeParameter.Depth = SymbolicExpressionTreeSizeParameter.Depth;
    }
  }
}
