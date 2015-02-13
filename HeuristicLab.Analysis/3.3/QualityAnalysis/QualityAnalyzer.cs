#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// An operator which analyzes the quality of solutions in the scope tree.
  /// </summary>
  [Item("QualityAnalyzer", "An operator which analyzes the quality of solutions in the scope tree.")]
  [StorableClass]
  public sealed class QualityAnalyzer : AlgorithmOperator, IAnalyzer, ISingleObjectiveOperator {
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

    #region Properties
    public bool EnabledByDefault {
      get { return true; }
    }
    private BestQualityMemorizer BestQualityMemorizer {
      get { return (BestQualityMemorizer)OperatorGraph.InitialOperator; }
    }
    private BestQualityMemorizer BestKnownQualityMemorizer {
      get { return (BestQualityMemorizer)BestQualityMemorizer.Successor; }
    }
    private DataTableValuesCollector DataTableValuesCollector {
      get { return (DataTableValuesCollector)BestKnownQualityMemorizer.Successor; }
    }
    private ResultsCollector ResultsCollector {
      get { return (ResultsCollector)((QualityDifferenceCalculator)DataTableValuesCollector.Successor).Successor; }
    }
    #endregion

    #region Storing & Cloning
    [StorableConstructor]
    private QualityAnalyzer(bool deserializing) : base(deserializing) { }
    private QualityAnalyzer(QualityAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new QualityAnalyzer(this, cloner);
    }
    #endregion
    public QualityAnalyzer()
      : base() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestKnownQuality", "The best known quality value found so far."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestQuality", "The best quality value found in the current run."));
      Parameters.Add(new ValueLookupParameter<DataTable>("Qualities", "The data table to store the current best, current average, current worst, best and best known quality value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("AbsoluteDifferenceBestKnownToBest", "The absolute difference of the best known quality value to the best quality value."));
      Parameters.Add(new ValueLookupParameter<PercentValue>("RelativeDifferenceBestKnownToBest", "The relative difference of the best known quality value to the best quality value."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The results collection where the analysis values should be stored."));
      #endregion

      #region Create operators
      BestQualityMemorizer bestQualityMemorizer = new BestQualityMemorizer();
      BestQualityMemorizer bestKnownQualityMemorizer = new BestQualityMemorizer();
      DataTableValuesCollector dataTableValuesCollector = new DataTableValuesCollector();
      QualityDifferenceCalculator qualityDifferenceCalculator = new QualityDifferenceCalculator();
      ResultsCollector resultsCollector = new ResultsCollector();

      bestQualityMemorizer.BestQualityParameter.ActualName = BestQualityParameter.Name;
      bestQualityMemorizer.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestQualityMemorizer.QualityParameter.ActualName = QualityParameter.Name;
      bestQualityMemorizer.QualityParameter.Depth = QualityParameter.Depth;

      bestKnownQualityMemorizer.BestQualityParameter.ActualName = BestKnownQualityParameter.Name;
      bestKnownQualityMemorizer.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestKnownQualityMemorizer.QualityParameter.ActualName = QualityParameter.Name;
      bestKnownQualityMemorizer.QualityParameter.Depth = QualityParameter.Depth;

      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("BestQuality", null, BestQualityParameter.Name));
      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("BestKnownQuality", null, BestKnownQualityParameter.Name));
      dataTableValuesCollector.CollectedValues.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", null, QualityParameter.Name));
      ((ScopeTreeLookupParameter<DoubleValue>)dataTableValuesCollector.CollectedValues["Quality"]).Depth = QualityParameter.Depth;
      dataTableValuesCollector.DataTableParameter.ActualName = QualitiesParameter.Name;

      qualityDifferenceCalculator.AbsoluteDifferenceParameter.ActualName = AbsoluteDifferenceBestKnownToBestParameter.Name;
      qualityDifferenceCalculator.FirstQualityParameter.ActualName = BestKnownQualityParameter.Name;
      qualityDifferenceCalculator.RelativeDifferenceParameter.ActualName = RelativeDifferenceBestKnownToBestParameter.Name;
      qualityDifferenceCalculator.SecondQualityParameter.ActualName = BestQualityParameter.Name;

      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("BestQuality", null, BestQualityParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("BestKnownQuality", null, BestKnownQualityParameter.Name));
      resultsCollector.CollectedValues.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", null, QualityParameter.Name));
      ((ScopeTreeLookupParameter<DoubleValue>)resultsCollector.CollectedValues["Quality"]).Depth = QualityParameter.Depth;
      resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("AbsoluteDifferenceBestKnownToBest", null, AbsoluteDifferenceBestKnownToBestParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<PercentValue>("RelativeDifferenceBestKnownToBest", null, RelativeDifferenceBestKnownToBestParameter.Name));
      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>(QualitiesParameter.Name));
      resultsCollector.ResultsParameter.ActualName = ResultsParameter.Name;
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = bestQualityMemorizer;
      bestQualityMemorizer.Successor = bestKnownQualityMemorizer;
      bestKnownQualityMemorizer.Successor = dataTableValuesCollector;
      dataTableValuesCollector.Successor = qualityDifferenceCalculator;
      qualityDifferenceCalculator.Successor = resultsCollector;
      resultsCollector.Successor = null;
      #endregion

      Initialize();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }

    private void Initialize() {
      QualityParameter.DepthChanged += new EventHandler(QualityParameter_DepthChanged);
    }

    private void QualityParameter_DepthChanged(object sender, System.EventArgs e) {
      BestQualityMemorizer.QualityParameter.Depth = QualityParameter.Depth;
      BestKnownQualityMemorizer.QualityParameter.Depth = QualityParameter.Depth;
      ((ScopeTreeLookupParameter<DoubleValue>)DataTableValuesCollector.CollectedValues["Quality"]).Depth = QualityParameter.Depth;
      ((ScopeTreeLookupParameter<DoubleValue>)ResultsCollector.CollectedValues["Quality"]).Depth = QualityParameter.Depth;
    }
  }
}
