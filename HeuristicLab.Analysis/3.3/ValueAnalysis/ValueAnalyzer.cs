#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// An operator which analyzes a value in the scope tree.
  /// </summary>
  [Item("ValueAnalyzer", "An operator which analyzes a value in the scope tree (current scope and children).")]
  [StorableType("A906F087-28A5-4E0B-897B-B0FD1B286ACE")]
  public sealed class ValueAnalyzer : AlgorithmOperator, IAnalyzer {
    #region Parameter properties
    public ScopeTreeLookupParameter<DoubleValue> ValueParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Value"]; }
    }
    public ValueLookupParameter<DataTable> ValuesParameter {
      get { return (ValueLookupParameter<DataTable>)Parameters["Values"]; }
    }
    public ValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (ValueLookupParameter<VariableCollection>)Parameters["Results"]; }
    }
    #endregion

    #region Properties
    public bool EnabledByDefault {
      get { return true; }
    }
    private DataTableValuesCollector DataTableValuesCollector {
      get { return (DataTableValuesCollector)OperatorGraph.InitialOperator; }
    }
    private ResultsCollector ResultsCollector {
      get { return (ResultsCollector)DataTableValuesCollector.Successor; }
    }
    #endregion

    #region Storing & Cloning
    [StorableConstructor]
    private ValueAnalyzer(StorableConstructorFlag _) : base(_) { }
    private ValueAnalyzer(ValueAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ValueAnalyzer(this, cloner);
    }
    #endregion
    public ValueAnalyzer()
      : base() {
      #region Create parameters
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Value", "The value contained in the scope tree which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<DataTable>("Values", "The data table to store the values."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The results collection where the analysis values should be stored."));
      #endregion

      #region Create operators
      DataTableValuesCollector dataTableValuesCollector = new DataTableValuesCollector();
      ResultsCollector resultsCollector = new ResultsCollector();

      dataTableValuesCollector.CollectedValues.Add(new ScopeTreeLookupParameter<DoubleValue>("Value", null, ValueParameter.Name));
      ((ScopeTreeLookupParameter<DoubleValue>)dataTableValuesCollector.CollectedValues["Value"]).Depth = ValueParameter.Depth;
      dataTableValuesCollector.DataTableParameter.ActualName = ValuesParameter.Name;

      resultsCollector.CollectedValues.Add(new ScopeTreeLookupParameter<DoubleValue>("Value", null, ValueParameter.Name));
      ((ScopeTreeLookupParameter<DoubleValue>)resultsCollector.CollectedValues["Value"]).Depth = ValueParameter.Depth;
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>(ValuesParameter.Name));
      resultsCollector.ResultsParameter.ActualName = ResultsParameter.Name;
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = dataTableValuesCollector;
      dataTableValuesCollector.Successor = resultsCollector;
      resultsCollector.Successor = null;
      #endregion

      Initialize();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }

    private void Initialize() {
      ValueParameter.DepthChanged += new EventHandler(ValueParameter_DepthChanged);
    }

    private void ValueParameter_DepthChanged(object sender, System.EventArgs e) {
      ((ScopeTreeLookupParameter<DoubleValue>)DataTableValuesCollector.CollectedValues["Value"]).Depth = ValueParameter.Depth;
      ((ScopeTreeLookupParameter<DoubleValue>)ResultsCollector.CollectedValues["Value"]).Depth = ValueParameter.Depth;
    }
  }
}
