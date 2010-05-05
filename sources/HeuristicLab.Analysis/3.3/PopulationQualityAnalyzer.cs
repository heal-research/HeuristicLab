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
  /// An operator which analyzes the solution qualities in the current population.
  /// </summary>
  [Item("PopulationQualityAnalyzer", "An operator which analyzes the solution qualities in the current population.")]
  [StorableClass]
  public sealed class PopulationQualityAnalyzer : AlgorithmOperator, IAnalyzer {
    #region Parameter properties
    public ValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public SubScopesLookupParameter<DoubleValue> QualityParameter {
      get { return (SubScopesLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ValueLookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public ValueLookupParameter<DoubleValue> BestQualityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestQuality"]; }
    }
    public ValueLookupParameter<DataTable> QualitiesParameter {
      get { return (ValueLookupParameter<DataTable>)Parameters["Qualities"]; }
    }
    public ValueLookupParameter<DoubleValue> AbsoluteDifferenceBestKnownToBestParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["AbsoluteDifferenceBestKnownToBest"]; }
    }
    public ValueLookupParameter<PercentValue> RelativeDifferenceBestKnownToBestParameter {
      get { return (ValueLookupParameter<PercentValue>)Parameters["RelativeDifferenceBestKnownToBest"]; }
    }
    public ValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (ValueLookupParameter<VariableCollection>)Parameters["Results"]; }
    }
    #endregion

    [StorableConstructor]
    private PopulationQualityAnalyzer(bool deserializing) : base() { }
    public PopulationQualityAnalyzer()
      : base() {
      Initialize();
    }

    private void Initialize() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new SubScopesLookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestKnownQuality", "The best known quality value found so far."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestQuality", "The best quality value found in the current run."));
      Parameters.Add(new ValueLookupParameter<DataTable>("Qualities", "The data table to store the best, best known and all other quality values."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("AbsoluteDifferenceBestKnownToBest", "The absolute difference of the best known quality value to the best quality value."));
      Parameters.Add(new ValueLookupParameter<PercentValue>("RelativeDifferenceBestKnownToBest", "The relative difference of the best known quality value to the best quality value."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The results collection where the analysis values should be stored."));
      #endregion

      #region Create operators
      BestQualityMemorizer bestQualityMemorizer1 = new BestQualityMemorizer();
      BestQualityMemorizer bestQualityMemorizer2 = new BestQualityMemorizer();
      DataTableValuesCollector dataTableValuesCollector = new DataTableValuesCollector();
      QualityDifferenceCalculator qualityDifferenceCalculator = new QualityDifferenceCalculator();
      ResultsCollector resultsCollector = new ResultsCollector();

      bestQualityMemorizer1.BestQualityParameter.ActualName = "BestQuality";
      bestQualityMemorizer1.MaximizationParameter.ActualName = "Maximization";
      bestQualityMemorizer1.QualityParameter.ActualName = "Quality";

      bestQualityMemorizer2.BestQualityParameter.ActualName = "BestKnownQuality";
      bestQualityMemorizer2.MaximizationParameter.ActualName = "Maximization";
      bestQualityMemorizer2.QualityParameter.ActualName = "Quality";

      dataTableValuesCollector.CollectedValues.Add(new SubScopesLookupParameter<DoubleValue>("Quality", null, "Quality"));
      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, "BestKnownQuality"));
      dataTableValuesCollector.DataTableParameter.ActualName = "Qualities";

      qualityDifferenceCalculator.AbsoluteDifferenceParameter.ActualName = "AbsoluteDifferenceBestKnownToBest";
      qualityDifferenceCalculator.FirstQualityParameter.ActualName = "BestKnownQuality";
      qualityDifferenceCalculator.RelativeDifferenceParameter.ActualName = "RelativeDifferenceBestKnownToBest";
      qualityDifferenceCalculator.SecondQualityParameter.ActualName = "BestQuality";

      resultsCollector.CollectedValues.Add(new SubScopesLookupParameter<DoubleValue>("Quality", null, "Quality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Quality", null, "BestQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Best Known Quality", null, "BestKnownQuality"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Absolute Difference of Best Known Quality to Best Quality", null, "AbsoluteDifferenceBestKnownToBest"));
      resultsCollector.CollectedValues.Add(new LookupParameter<PercentValue>("Relative Difference of Best Known Quality to Best Quality", null, "RelativeDifferenceBestKnownToBest"));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>("Qualities"));
      resultsCollector.ResultsParameter.ActualName = "Results";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = bestQualityMemorizer1;
      bestQualityMemorizer1.Successor = bestQualityMemorizer2;
      bestQualityMemorizer2.Successor = dataTableValuesCollector;
      dataTableValuesCollector.Successor = qualityDifferenceCalculator;
      qualityDifferenceCalculator.Successor = resultsCollector;
      resultsCollector.Successor = null;
      #endregion
    }
  }
}
