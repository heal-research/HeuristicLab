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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// An operator which analyzes a value in the current population.
  /// </summary>
  [Item("PopulationValueAnalyzer", "An operator which analyzes a value in the current population.")]
  [StorableClass]
  public sealed class PopulationValueAnalyzer : AlgorithmOperator, IAnalyzer {
    #region Parameter properties
    public SubScopesLookupParameter<DoubleValue> ValueParameter {
      get { return (SubScopesLookupParameter<DoubleValue>)Parameters["Value"]; }
    }
    public ValueLookupParameter<DataTable> ValuesParameter {
      get { return (ValueLookupParameter<DataTable>)Parameters["Values"]; }
    }
    public ValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (ValueLookupParameter<VariableCollection>)Parameters["Results"]; }
    }
    #endregion

    [StorableConstructor]
    private PopulationValueAnalyzer(bool deserializing) : base() { }
    public PopulationValueAnalyzer()
      : base() {
      Initialize();
    }

    private void Initialize() {
      #region Create parameters
      Parameters.Add(new SubScopesLookupParameter<DoubleValue>("Value", "The value contained in each solution which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<DataTable>("Values", "The data table to store the values."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The results collection where the analysis values should be stored."));
      #endregion

      #region Create operators
      DataTableValuesCollector dataTableValuesCollector = new DataTableValuesCollector();
      ResultsCollector resultsCollector = new ResultsCollector();

      dataTableValuesCollector.CollectedValues.Add(new SubScopesLookupParameter<DoubleValue>("Value", null, "Value"));
      dataTableValuesCollector.DataTableParameter.ActualName = "Values";

      resultsCollector.CollectedValues.Add(new SubScopesLookupParameter<DoubleValue>("Value", null, "Value"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("Values"));
      resultsCollector.ResultsParameter.ActualName = "Results";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = dataTableValuesCollector;
      dataTableValuesCollector.Successor = resultsCollector;
      resultsCollector.Successor = null;
      #endregion
    }
  }
}
