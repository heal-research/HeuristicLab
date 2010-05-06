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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// An operator which analyzes the minimum, average and maximum of a value in the scope tree.
  /// </summary>
  [Item("MinAverageMaxValueAnalyzer", "An operator which analyzes the minimum, average and maximum of a value in the scope tree.")]
  [StorableClass]
  public sealed class MinAverageMaxValueAnalyzer : AlgorithmOperator, IAnalyzer {
    #region Parameter properties
    public ScopeTreeLookupParameter<DoubleValue> ValueParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Value"]; }
    }
    public ValueLookupParameter<DoubleValue> MinValueParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["MinValue"]; }
    }
    public ValueLookupParameter<DoubleValue> AverageValueParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["AverageValue"]; }
    }
    public ValueLookupParameter<DoubleValue> MaxValueParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["MaxValue"]; }
    }
    public ValueLookupParameter<DataTable> ValuesParameter {
      get { return (ValueLookupParameter<DataTable>)Parameters["Values"]; }
    }
    public ValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (ValueLookupParameter<VariableCollection>)Parameters["Results"]; }
    }
    #endregion

    #region Properties
    private MinAverageMaxValueCalculator MinAverageMaxValueCalculator {
      get { return (MinAverageMaxValueCalculator)OperatorGraph.InitialOperator; }
    }
    #endregion

    public MinAverageMaxValueAnalyzer()
      : base() {
      #region Create parameters
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Value", "The value contained in the scope tree which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MinValue", "The minimum of the value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("AverageValue", "The average of the value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaxValue", "The maximum of the value."));
      Parameters.Add(new ValueLookupParameter<DataTable>("Values", "The data table to store the minimum, average and maximum of the value."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The results collection where the analysis values should be stored."));
      #endregion

      #region Create operators
      MinAverageMaxValueCalculator minAverageMaxValueCalculator = new MinAverageMaxValueCalculator();
      DataTableValuesCollector dataTableValuesCollector = new DataTableValuesCollector();
      ResultsCollector resultsCollector = new ResultsCollector();

      minAverageMaxValueCalculator.AverageValueParameter.ActualName = AverageValueParameter.Name;
      minAverageMaxValueCalculator.MaxValueParameter.ActualName = MaxValueParameter.Name;
      minAverageMaxValueCalculator.MinValueParameter.ActualName = MinValueParameter.Name;
      minAverageMaxValueCalculator.ValueParameter.ActualName = ValueParameter.Name;
      minAverageMaxValueCalculator.ValueParameter.Depth = ValueParameter.Depth;

      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("MinValue", null, MinValueParameter.Name));
      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("AverageValue", null, AverageValueParameter.Name));
      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("MaxValue", null, MaxValueParameter.Name));
      dataTableValuesCollector.DataTableParameter.ActualName = ValuesParameter.Name;

      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("MinValue", null, MinValueParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("AverageValue", null, AverageValueParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("MaxValue", null, MaxValueParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>(ValuesParameter.Name));
      resultsCollector.ResultsParameter.ActualName = ResultsParameter.Name;
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = minAverageMaxValueCalculator;
      minAverageMaxValueCalculator.Successor = dataTableValuesCollector;
      dataTableValuesCollector.Successor = resultsCollector;
      resultsCollector.Successor = null;
      #endregion

      Initialize();
    }
    [StorableConstructor]
    private MinAverageMaxValueAnalyzer(bool deserializing) : base() { }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      ValueParameter.DepthChanged += new EventHandler(ValueParameter_DepthChanged);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      MinAverageMaxValueAnalyzer clone = (MinAverageMaxValueAnalyzer)base.Clone(cloner);
      clone.Initialize();
      return clone;
    }

    private void ValueParameter_DepthChanged(object sender, System.EventArgs e) {
      MinAverageMaxValueCalculator.ValueParameter.Depth = ValueParameter.Depth;
    }
  }
}
