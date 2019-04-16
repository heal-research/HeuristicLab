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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// An operator for analyzing the solution similarity in a population.
  /// </summary>
  [Item("PopulationSimilarityAnalyzer", "An operator for analyzing the solution similarity in a population.")]
  [StorableType("56A495B0-573F-421A-99E2-24D67D8DB686")]
  public class PopulationSimilarityAnalyzer : SingleSuccessorOperator, IAnalyzer, ISimilarityBasedOperator {
    private const string DiversityResultNameParameterName = "DiversityResultsName";
    private const string ExecuteInParallelParameterName = "ExecuteInParallel";
    private const string MaxDegreeOfParallelismParameterName = "MaxDegreeOfParallelism";

    #region Backwards compatible code, remove with 3.4
    private ISolutionSimilarityCalculator oldSimilarityCalculator;
    [Storable(OldName = "SimilarityCalculator")]
    [Obsolete]
    private ISolutionSimilarityCalculator SimilarityCalculator { set { oldSimilarityCalculator = value; } }
    #endregion

    public virtual bool EnabledByDefault {
      get { return false; }
    }
    public ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (IValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    public IConstrainedValueParameter<ISolutionSimilarityCalculator> SimilarityCalculatorParameter {
      get { return (IConstrainedValueParameter<ISolutionSimilarityCalculator>)Parameters["SimilarityCalculator"]; }
    }
    public IValueParameter<BoolValue> StoreHistoryParameter {
      get { return (IValueParameter<BoolValue>)Parameters["StoreHistory"]; }
    }
    public IValueParameter<IntValue> UpdateIntervalParameter {
      get { return (IValueParameter<IntValue>)Parameters["UpdateInterval"]; }
    }
    public ILookupParameter<IntValue> UpdateCounterParameter {
      get { return (ILookupParameter<IntValue>)Parameters["UpdateCounter"]; }
    }
    public IFixedValueParameter<StringValue> DiversityResultNameParameter {
      get { return (FixedValueParameter<StringValue>)Parameters[DiversityResultNameParameterName]; }
    }
    public IFixedValueParameter<BoolValue> ExecuteInParallelParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[ExecuteInParallelParameterName]; }
    }
    public IFixedValueParameter<IntValue> MaxDegreeOfParallelismParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaxDegreeOfParallelismParameterName]; }
    }

    public string DiversityResultName {
      get { return DiversityResultNameParameter.Value.Value; }
      set { DiversityResultNameParameter.Value.Value = value; }
    }

    public bool ExecuteInParallel {
      get { return ExecuteInParallelParameter.Value.Value; }
      set { ExecuteInParallelParameter.Value.Value = value; }
    }

    public int MaxDegreeOfParallelism {
      get { return MaxDegreeOfParallelismParameter.Value.Value; }
      set { MaxDegreeOfParallelismParameter.Value.Value = value; }
    }

    [StorableConstructor]
    protected PopulationSimilarityAnalyzer(StorableConstructorFlag _) : base(_) { }

    protected PopulationSimilarityAnalyzer(PopulationSimilarityAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      RegisterParametersEventHandlers();
    }

    public PopulationSimilarityAnalyzer(IEnumerable<ISolutionSimilarityCalculator> validSimilarityCalculators)
      : base() {
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope that contains the solutions which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the population diversity analysis results should be stored."));
      Parameters.Add(new ConstrainedValueParameter<ISolutionSimilarityCalculator>("SimilarityCalculator", "The similarity calculator that should be used to calculate population similarity."));
      Parameters.Add(new ValueParameter<BoolValue>("StoreHistory", "True if the history of the population diversity analysis should be stored.", new BoolValue(false)));
      Parameters.Add(new ValueParameter<IntValue>("UpdateInterval", "The interval in which the population diversity analysis should be applied.", new IntValue(1)));
      Parameters.Add(new LookupParameter<IntValue>("UpdateCounter", "The value which counts how many times the operator was called since the last update.", "PopulationDiversityAnalyzerUpdateCounter"));
      Parameters.Add(new FixedValueParameter<StringValue>(DiversityResultNameParameterName, "Specifies how the diversity results should be named.", new StringValue("PopulationDiversity")));
      Parameters.Add(new FixedValueParameter<BoolValue>(ExecuteInParallelParameterName, "Specifies whether similarity calculations should be parallelized.", new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<IntValue>(MaxDegreeOfParallelismParameterName, "Specifies the maximum number of threads when calculating similarities in parallel.", new IntValue(-1)));

      var similarityCalculators = SimilarityCalculatorParameter.ValidValues;
      foreach (var sc in validSimilarityCalculators) {
        similarityCalculators.Add(sc);
      }

      ResultsParameter.Hidden = true;
      UpdateCounterParameter.Hidden = true;
      ExecuteInParallelParameter.Hidden = true;
      MaxDegreeOfParallelismParameter.Hidden = true;

      RegisterParametersEventHandlers();
    }

    private void RegisterParametersEventHandlers() {
      ExecuteInParallelParameter.Value.ValueChanged += Value_ValueChanged;
      MaxDegreeOfParallelismParameter.Value.ValueChanged += Value_ValueChanged;
    }

    private void Value_ValueChanged(object sender, EventArgs e) {
      var similarityCalculators = SimilarityCalculatorParameter.ValidValues;
      foreach (var similarityCalculator in similarityCalculators) {
        similarityCalculator.ExecuteInParallel = ExecuteInParallel;
        similarityCalculator.MaxDegreeOfParallelism = MaxDegreeOfParallelism;
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PopulationSimilarityAnalyzer(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey(DiversityResultNameParameterName))
        Parameters.Add(new FixedValueParameter<StringValue>(DiversityResultNameParameterName, "Specifies how the diversity results should be named.", new StringValue("PopulationDiversity")));

      if (!Parameters.ContainsKey("SimilarityCalculator"))
        Parameters.Add(new ConstrainedValueParameter<ISolutionSimilarityCalculator>("SimilarityCalculator", "The similarity calculator that should be used to calculate solution similarity."));

      if (oldSimilarityCalculator != null)
        SimilarityCalculatorParameter.ValidValues.Add(oldSimilarityCalculator);

      if (!Parameters.ContainsKey(ExecuteInParallelParameterName)) {
        Parameters.Add(new FixedValueParameter<BoolValue>(ExecuteInParallelParameterName,
          "Specifies whether similarity calculations should be parallelized.", new BoolValue(false)));
        ExecuteInParallelParameter.Hidden = true;
      }
      if (!Parameters.ContainsKey(MaxDegreeOfParallelismParameterName)) {
        Parameters.Add(new FixedValueParameter<IntValue>(MaxDegreeOfParallelismParameterName,
          "Specifies the maximum number of threads when calculating similarities in parallel.", new IntValue(-1)));
        MaxDegreeOfParallelismParameter.Hidden = true;
      }

      RegisterParametersEventHandlers();
      #endregion
    }

    public override IOperation Apply() {
      int updateInterval = UpdateIntervalParameter.Value.Value;
      IntValue updateCounter = UpdateCounterParameter.ActualValue;
      // if counter does not yet exist then initialize it with update interval
      // to make sure the solutions are analyzed on the first application of this operator
      if (updateCounter == null) {
        updateCounter = new IntValue(updateInterval);
        UpdateCounterParameter.ActualValue = updateCounter;
      }
      //analyze solutions only every 'updateInterval' times
      if (updateCounter.Value != updateInterval) {
        updateCounter.Value++;
        return base.Apply();
      }
      updateCounter.Value = 1;

      bool storeHistory = StoreHistoryParameter.Value.Value;
      int count = CurrentScopeParameter.ActualValue.SubScopes.Count;

      if (count > 1) {
        var similarityCalculator = SimilarityCalculatorParameter.Value;
        // calculate solution similarities
        var similarityMatrix = similarityCalculator.CalculateSolutionCrowdSimilarity(CurrentScopeParameter.ActualValue);
        double[,] similarities = new double[similarityMatrix.Length, similarityMatrix[0].Length];
        for (int i = 0; i < similarityMatrix.Length; i++)
          for (int j = 0; j < similarityMatrix[0].Length; j++)
            similarities[i, j] = similarityMatrix[i][j];

        // calculate minimum, average and maximum similarities
        double similarity;
        double[] minSimilarities = new double[count];
        double[] avgSimilarities = new double[count];
        double[] maxSimilarities = new double[count];
        for (int i = 0; i < count; i++) {
          minSimilarities[i] = 1;
          avgSimilarities[i] = 0;
          maxSimilarities[i] = 0;
          for (int j = 0; j < count; j++) {
            if (i != j) {
              similarity = similarities[i, j];

              if ((similarity < 0) || (similarity > 1))
                throw new InvalidOperationException("Solution similarities have to be in the interval [0;1].");

              if (minSimilarities[i] > similarity) minSimilarities[i] = similarity;
              avgSimilarities[i] += similarity;
              if (maxSimilarities[i] < similarity) maxSimilarities[i] = similarity;
            }
          }
          avgSimilarities[i] = avgSimilarities[i] / (count - 1);
        }
        double avgMinSimilarity = minSimilarities.Average();
        double avgAvgSimilarity = avgSimilarities.Average();
        double avgMaxSimilarity = maxSimilarities.Average();

        // fetch results collection
        ResultCollection results;
        if (!ResultsParameter.ActualValue.ContainsKey(DiversityResultName)) {
          results = new ResultCollection();
          ResultsParameter.ActualValue.Add(new Result(DiversityResultName, results));
        } else {
          results = (ResultCollection)ResultsParameter.ActualValue[DiversityResultName].Value;
        }

        // store similarities
        HeatMap similaritiesHeatMap = new HeatMap(similarities, "Solution Similarities", 0.0, 1.0);
        if (!results.ContainsKey("Solution Similarities"))
          results.Add(new Result("Solution Similarities", similaritiesHeatMap));
        else
          results["Solution Similarities"].Value = similaritiesHeatMap;

        // store similarities history
        if (storeHistory) {
          if (!results.ContainsKey("Solution Similarities History")) {
            HeatMapHistory history = new HeatMapHistory();
            history.Add(similaritiesHeatMap);
            results.Add(new Result("Solution Similarities History", history));
          } else {
            ((HeatMapHistory)results["Solution Similarities History"].Value).Add(similaritiesHeatMap);
          }
        }

        // store average minimum, average and maximum similarity
        if (!results.ContainsKey("Average Minimum Solution Similarity"))
          results.Add(new Result("Average Minimum Solution Similarity", new DoubleValue(avgMinSimilarity)));
        else
          ((DoubleValue)results["Average Minimum Solution Similarity"].Value).Value = avgMinSimilarity;

        if (!results.ContainsKey("Average Average Solution Similarity"))
          results.Add(new Result("Average Average Solution Similarity", new DoubleValue(avgAvgSimilarity)));
        else
          ((DoubleValue)results["Average Average Solution Similarity"].Value).Value = avgAvgSimilarity;

        if (!results.ContainsKey("Average Maximum Solution Similarity"))
          results.Add(new Result("Average Maximum Solution Similarity", new DoubleValue(avgMaxSimilarity)));
        else
          ((DoubleValue)results["Average Maximum Solution Similarity"].Value).Value = avgMaxSimilarity;

        // store average minimum, average and maximum solution similarity data table
        DataTable minAvgMaxSimilarityDataTable;
        if (!results.ContainsKey("Average Minimum/Average/Maximum Solution Similarity")) {
          minAvgMaxSimilarityDataTable = new DataTable("Average Minimum/Average/Maximum Solution Similarity");
          minAvgMaxSimilarityDataTable.VisualProperties.XAxisTitle = "Iteration";
          minAvgMaxSimilarityDataTable.VisualProperties.YAxisTitle = "Solution Similarity";
          minAvgMaxSimilarityDataTable.Rows.Add(new DataRow("Average Minimum Solution Similarity", null));
          minAvgMaxSimilarityDataTable.Rows["Average Minimum Solution Similarity"].VisualProperties.StartIndexZero = true;
          minAvgMaxSimilarityDataTable.Rows.Add(new DataRow("Average Average Solution Similarity", null));
          minAvgMaxSimilarityDataTable.Rows["Average Average Solution Similarity"].VisualProperties.StartIndexZero = true;
          minAvgMaxSimilarityDataTable.Rows.Add(new DataRow("Average Maximum Solution Similarity", null));
          minAvgMaxSimilarityDataTable.Rows["Average Maximum Solution Similarity"].VisualProperties.StartIndexZero = true;
          results.Add(new Result("Average Minimum/Average/Maximum Solution Similarity", minAvgMaxSimilarityDataTable));
        } else {
          minAvgMaxSimilarityDataTable = (DataTable)results["Average Minimum/Average/Maximum Solution Similarity"].Value;
        }
        minAvgMaxSimilarityDataTable.Rows["Average Minimum Solution Similarity"].Values.Add(avgMinSimilarity);
        minAvgMaxSimilarityDataTable.Rows["Average Average Solution Similarity"].Values.Add(avgAvgSimilarity);
        minAvgMaxSimilarityDataTable.Rows["Average Maximum Solution Similarity"].Values.Add(avgMaxSimilarity);

        // store minimum, average, maximum similarities data table
        DataTable minAvgMaxSimilaritiesDataTable = new DataTable("Minimum/Average/Maximum Solution Similarities");
        minAvgMaxSimilaritiesDataTable.VisualProperties.XAxisTitle = "Solution Index";
        minAvgMaxSimilaritiesDataTable.VisualProperties.YAxisTitle = "Solution Similarity";
        minAvgMaxSimilaritiesDataTable.Rows.Add(new DataRow("Minimum Solution Similarity", null, minSimilarities));
        minAvgMaxSimilaritiesDataTable.Rows["Minimum Solution Similarity"].VisualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Points;
        minAvgMaxSimilaritiesDataTable.Rows.Add(new DataRow("Average Solution Similarity", null, avgSimilarities));
        minAvgMaxSimilaritiesDataTable.Rows["Average Solution Similarity"].VisualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Points;
        minAvgMaxSimilaritiesDataTable.Rows.Add(new DataRow("Maximum Solution Similarity", null, maxSimilarities));
        minAvgMaxSimilaritiesDataTable.Rows["Maximum Solution Similarity"].VisualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Points;
        if (!results.ContainsKey("Minimum/Average/Maximum Solution Similarities")) {
          results.Add(new Result("Minimum/Average/Maximum Solution Similarities", minAvgMaxSimilaritiesDataTable));
        } else {
          results["Minimum/Average/Maximum Solution Similarities"].Value = minAvgMaxSimilaritiesDataTable;
        }

        // store minimum, average, maximum similarities history
        if (storeHistory) {
          if (!results.ContainsKey("Minimum/Average/Maximum Solution Similarities History")) {
            DataTableHistory history = new DataTableHistory();
            history.Add(minAvgMaxSimilaritiesDataTable);
            results.Add(new Result("Minimum/Average/Maximum Solution Similarities History", history));
          } else {
            ((DataTableHistory)results["Minimum/Average/Maximum Solution Similarities History"].Value).Add(minAvgMaxSimilaritiesDataTable);
          }
        }
      }
      return base.Apply();
    }
  }
}