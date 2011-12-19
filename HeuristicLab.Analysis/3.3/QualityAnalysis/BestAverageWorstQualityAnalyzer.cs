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
  /// An operator which analyzes the best, average and worst quality of solutions in the scope tree.
  /// </summary>
  [Item("BestAverageWorstQualityAnalyzer", "An operator which analyzes the best, average and worst quality of solutions in the scope tree.")]
  [StorableClass]
  public sealed class BestAverageWorstQualityAnalyzer : AlgorithmOperator, IAnalyzer {
    #region Parameter properties
    public ValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ValueLookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public ValueLookupParameter<DoubleValue> BestQualityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestQuality"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentBestQualityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentBestQuality"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentAverageQualityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentAverageQuality"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentWorstQualityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentWorstQuality"]; }
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
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    #endregion

    #region Properties
    public bool EnabledByDefault {
      get { return true; }
    }
    private BestQualityMemorizer BestQualityMemorizer {
      get { return (BestQualityMemorizer)OperatorGraph.InitialOperator; }
    }
    private BestAverageWorstQualityCalculator BestAverageWorstQualityCalculator {
      get { return (BestAverageWorstQualityCalculator)BestQualityMemorizer.Successor; }
    }
    #endregion

    #region Storing & Cloning
    [StorableConstructor]
    private BestAverageWorstQualityAnalyzer(bool deserializing) : base(deserializing) { }
    private BestAverageWorstQualityAnalyzer(BestAverageWorstQualityAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestAverageWorstQualityAnalyzer(this, cloner);
    }
    #endregion
    public BestAverageWorstQualityAnalyzer()
      : base() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestKnownQuality", "The best known quality value found so far."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestQuality", "The best quality value found in the current run."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentBestQuality", "The best quality value found in the current population."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentAverageQuality", "The average quality value of all solutions in the current population."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentWorstQuality", "The worst quality value found in the current population."));
      Parameters.Add(new ValueLookupParameter<DataTable>("Qualities", "The data table to store the current best, current average, current worst, best and best known quality value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("AbsoluteDifferenceBestKnownToBest", "The absolute difference of the best known quality value to the best quality value."));
      Parameters.Add(new ValueLookupParameter<PercentValue>("RelativeDifferenceBestKnownToBest", "The relative difference of the best known quality value to the best quality value."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The results collection where the analysis values should be stored."));

      BestQualityParameter.Hidden = true;
      CurrentBestQualityParameter.Hidden = true;
      CurrentAverageQualityParameter.Hidden = true;
      CurrentWorstQualityParameter.Hidden = true;
      QualitiesParameter.Hidden = true;
      AbsoluteDifferenceBestKnownToBestParameter.Hidden = true;
      RelativeDifferenceBestKnownToBestParameter.Hidden = true;
      #endregion

      #region Create operators
      BestQualityMemorizer bestQualityMemorizer = new BestQualityMemorizer();
      BestAverageWorstQualityCalculator bestAverageWorstQualityCalculator = new BestAverageWorstQualityCalculator();
      DataTableValuesCollector dataTableValuesCollector = new DataTableValuesCollector();
      QualityDifferenceCalculator qualityDifferenceCalculator = new QualityDifferenceCalculator();
      ResultsCollector resultsCollector = new ResultsCollector();

      bestQualityMemorizer.BestQualityParameter.ActualName = BestQualityParameter.Name;
      bestQualityMemorizer.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer.QualityParameter.ActualName = QualityParameter.Name;
      bestQualityMemorizer.QualityParameter.Depth = QualityParameter.Depth;

      bestAverageWorstQualityCalculator.AverageQualityParameter.ActualName = CurrentAverageQualityParameter.Name;
      bestAverageWorstQualityCalculator.BestQualityParameter.ActualName = CurrentBestQualityParameter.Name;
      bestAverageWorstQualityCalculator.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestAverageWorstQualityCalculator.QualityParameter.ActualName = QualityParameter.Name;
      bestAverageWorstQualityCalculator.QualityParameter.Depth = QualityParameter.Depth;
      bestAverageWorstQualityCalculator.WorstQualityParameter.ActualName = CurrentWorstQualityParameter.Name;

      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentBestQuality", null, CurrentBestQualityParameter.Name));
      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentAverageQuality", null, CurrentAverageQualityParameter.Name));
      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentWorstQuality", null, CurrentWorstQualityParameter.Name));
      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("BestQuality", null, BestQualityParameter.Name));
      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("BestKnownQuality", null, BestKnownQualityParameter.Name));
      dataTableValuesCollector.DataTableParameter.ActualName = QualitiesParameter.Name;

      qualityDifferenceCalculator.AbsoluteDifferenceParameter.ActualName = AbsoluteDifferenceBestKnownToBestParameter.Name;
      qualityDifferenceCalculator.FirstQualityParameter.ActualName = BestKnownQualityParameter.Name;
      qualityDifferenceCalculator.RelativeDifferenceParameter.ActualName = RelativeDifferenceBestKnownToBestParameter.Name;
      qualityDifferenceCalculator.SecondQualityParameter.ActualName = BestQualityParameter.Name;

      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentBestQuality", null, CurrentBestQualityParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentAverageQuality", null, CurrentAverageQualityParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentWorstQuality", null, CurrentWorstQualityParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("BestQuality", null, BestQualityParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("BestKnownQuality", null, BestKnownQualityParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("AbsoluteDifferenceBestKnownToBest", null, AbsoluteDifferenceBestKnownToBestParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<PercentValue>("RelativeDifferenceBestKnownToBest", null, RelativeDifferenceBestKnownToBestParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>(QualitiesParameter.Name));
      resultsCollector.ResultsParameter.ActualName = ResultsParameter.Name;
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = bestQualityMemorizer;
      bestQualityMemorizer.Successor = bestAverageWorstQualityCalculator;
      bestAverageWorstQualityCalculator.Successor = dataTableValuesCollector;
      dataTableValuesCollector.Successor = qualityDifferenceCalculator;
      qualityDifferenceCalculator.Successor = resultsCollector;
      resultsCollector.Successor = null;
      #endregion

      Initialize();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (Parameters["Results"] is ValueLookupParameter<VariableCollection>) {
        Parameters.Remove("Results");
        Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The results collection where the analysis values should be stored."));
      }
      #endregion
    }

    private void Initialize() {
      QualityParameter.DepthChanged += new EventHandler(QualityParameter_DepthChanged);
    }

    private void QualityParameter_DepthChanged(object sender, System.EventArgs e) {
      BestQualityMemorizer.QualityParameter.Depth = QualityParameter.Depth;
      BestAverageWorstQualityCalculator.QualityParameter.Depth = QualityParameter.Depth;
    }
  }
}
