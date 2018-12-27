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

using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.RAPGA {
  /// <summary>
  /// An operator which analyzes the size of the population in a scope tree.
  /// </summary>
  [Item("PopulationSizeAnalyzer", "An operator which analyzes the size of the population in a scope tree.")]
  [StorableClass]
  public sealed class PopulationSizeAnalyzer : AlgorithmOperator, IAnalyzer {
    #region Parameter properties
    public ILookupParameter<IntValue> CurrentPopulationSizeParameter {
      get { return (ILookupParameter<IntValue>)Parameters["CurrentPopulationSize"]; }
    }
    public ILookupParameter<IntValue> MaximumPopulationSizeParameter {
      get { return (ILookupParameter<IntValue>)Parameters["MaximumPopulationSize"]; }
    }
    public ILookupParameter<IntValue> MinimumPopulationSizeParameter {
      get { return (ILookupParameter<IntValue>)Parameters["MinimumPopulationSize"]; }
    }
    public IValueLookupParameter<DataTable> PopulationSizesParameter {
      get { return (IValueLookupParameter<DataTable>)Parameters["PopulationSizes"]; }
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
    private PopulationSizeAnalyzer(bool deserializing) : base(deserializing) { }
    private PopulationSizeAnalyzer(PopulationSizeAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PopulationSizeAnalyzer(this, cloner);
    }
    #endregion
    public PopulationSizeAnalyzer()
      : base() {
      #region Create parameters
      Parameters.Add(new LookupParameter<IntValue>("CurrentPopulationSize", "The current size of the population in the scope tree which should be analyzed."));
      Parameters.Add(new LookupParameter<IntValue>("MaximumPopulationSize", "The maximum size of the population in the scope tree which should be analyzed."));
      Parameters.Add(new LookupParameter<IntValue>("MinimumPopulationSize", "The minimum size of the population in the scope tree which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<DataTable>("PopulationSizes", "The data table to store the values."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The results collection where the analysis values should be stored."));
      #endregion

      #region Create operators
      DataTableValuesCollector dataTableValuesCollector = new DataTableValuesCollector();
      ResultsCollector resultsCollector = new ResultsCollector();

      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<IntValue>("CurrentPopulationSize", null, CurrentPopulationSizeParameter.Name));
      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<IntValue>("MaximumPopulationSize", null, MaximumPopulationSizeParameter.Name));
      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<IntValue>("MinimumPopulationSize", null, MinimumPopulationSizeParameter.Name));
      dataTableValuesCollector.DataTableParameter.ActualName = PopulationSizesParameter.Name;

      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>(PopulationSizesParameter.Name));
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
