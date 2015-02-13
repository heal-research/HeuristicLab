#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// An operator which analyzes a value in the scope tree (current scope and parents).
  /// </summary>
  [Item("SingleValueAnalyzer", "An operator which analyzes a value in the scope tree (current scope and parents).")]
  [StorableClass]
  public sealed class SingleValueAnalyzer : AlgorithmOperator, IAnalyzer {
    #region Parameter properties
    public ILookupParameter<DoubleValue> ValueParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Value"]; }
    }
    public IValueLookupParameter<DataTable> ValuesParameter {
      get { return (IValueLookupParameter<DataTable>)Parameters["Values"]; }
    }
    public IValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (IValueLookupParameter<VariableCollection>)Parameters["Results"]; }
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
    private SingleValueAnalyzer(bool deserializing) : base(deserializing) { }
    private SingleValueAnalyzer(SingleValueAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleValueAnalyzer(this, cloner);
    }
    #endregion
    public SingleValueAnalyzer()
      : base() {
      #region Create parameters
      Parameters.Add(new LookupParameter<DoubleValue>("Value", "The value contained in the scope tree which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<DataTable>("Values", "The data table to store the values."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The results collection where the analysis values should be stored."));
      #endregion

      #region Create operators
      DataTableValuesCollector dataTableValuesCollector = new DataTableValuesCollector();
      ResultsCollector resultsCollector = new ResultsCollector();

      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Value", null, ValueParameter.Name));
      dataTableValuesCollector.DataTableParameter.ActualName = ValuesParameter.Name;

      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Value", null, ValueParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>(ValuesParameter.Name));
      resultsCollector.ResultsParameter.ActualName = ResultsParameter.Name;
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = dataTableValuesCollector;
      dataTableValuesCollector.Successor = resultsCollector;
      resultsCollector.Successor = null;
      #endregion
    }
  }
}
