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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// An operator for analyzing the solution diversity in a population.
  /// </summary>
  [Item("PopulationDiversityAnalyzer", "An operator for analyzing the solution diversity in a population.")]
  [StorableClass]
  public abstract class PopulationDiversityAnalyzer<T> : SingleSuccessorOperator, IAnalyzer where T : class, IItem {
    public LookupParameter<BoolValue> MaximizationParameter {
      get { return (LookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ScopeTreeLookupParameter<T> SolutionParameter {
      get { return (ScopeTreeLookupParameter<T>)Parameters["Solution"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    public ValueParameter<BoolValue> StoreHistoryParameter {
      get { return (ValueParameter<BoolValue>)Parameters["StoreHistory"]; }
    }
    public ValueParameter<IntValue> UpdateIntervalParameter {
      get { return (ValueParameter<IntValue>)Parameters["UpdateInterval"]; }
    }
    public LookupParameter<IntValue> UpdateCounterParameter {
      get { return (LookupParameter<IntValue>)Parameters["UpdateCounter"]; }
    }

    [StorableConstructor]
    protected PopulationDiversityAnalyzer(bool deserializing) : base(deserializing) { }
    public PopulationDiversityAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem."));
      Parameters.Add(new ScopeTreeLookupParameter<T>("Solution", "The solutions whose diversity should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the solutions which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the population diversity analysis results should be stored."));
      Parameters.Add(new ValueParameter<BoolValue>("StoreHistory", "True if the history of the population diversity analysis should be stored.", new BoolValue(false)));
      Parameters.Add(new ValueParameter<IntValue>("UpdateInterval", "The interval in which the population diversity analysis should be applied.", new IntValue(1)));
      Parameters.Add(new LookupParameter<IntValue>("UpdateCounter", "The value which counts how many times the operator was called since the last update.", "PopulationDiversityAnalyzerUpdateCounter"));
    }

    public override IOperation Apply() {
      int updateInterval = UpdateIntervalParameter.Value.Value;
      IntValue updateCounter = UpdateCounterParameter.ActualValue;
      if (updateCounter == null) {
        updateCounter = new IntValue(updateInterval);
        UpdateCounterParameter.ActualValue = updateCounter;
      } else updateCounter.Value++;

      if (updateCounter.Value == updateInterval) {
        updateCounter.Value = 0;

        bool max = MaximizationParameter.ActualValue.Value;
        ItemArray<T> solutions = SolutionParameter.ActualValue;
        ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
        bool storeHistory = StoreHistoryParameter.Value.Value;

        // sort solutions by quality
        T[] sortedSolutions = null;
        if (max)
          sortedSolutions = solutions.Select((x, index) => new { Solution = x, Quality = qualities[index] }).OrderByDescending(x => x.Quality).Select(x => x.Solution).ToArray();
        else
          sortedSolutions = solutions.Select((x, index) => new { Solution = x, Quality = qualities[index] }).OrderBy(x => x.Quality).Select(x => x.Solution).ToArray();

        // calculate solution similarities
        double[,] similarities = CalculateSimilarities(sortedSolutions);

        // calculate maximum similarities, average maximum similarity and average similarity
        double similarity;
        int count = sortedSolutions.Length;
        double[] maxSimilarities = new double[sortedSolutions.Length];
        double avgMaxSimilarity;
        double avgSimilarity = 0;
        maxSimilarities.Initialize();
        for (int i = 0; i < count; i++) {
          for (int j = i + 1; j < count; j++) {
            similarity = similarities[i, j];
            avgSimilarity += similarity;
            if (maxSimilarities[i] < similarity) maxSimilarities[i] = similarity;
            if (maxSimilarities[j] < similarity) maxSimilarities[j] = similarity;
          }
        }
        avgMaxSimilarity = maxSimilarities.Average();
        avgSimilarity = avgSimilarity / ((count - 1) * count / 2);

        // fetch results collection
        ResultCollection results;
        if (!ResultsParameter.ActualValue.ContainsKey("Population Diversity Analysis Results")) {
          results = new ResultCollection();
          ResultsParameter.ActualValue.Add(new Result("Population Diversity Analysis Results", results));
        } else {
          results = (ResultCollection)ResultsParameter.ActualValue["Population Diversity Analysis Results"].Value;
        }

        // store similarities
        HeatMap similaritiesHeatMap = new HeatMap(similarities);
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

        // store average similarity
        if (!results.ContainsKey("Average Population Similarity"))
          results.Add(new Result("Average Population Similarity", new DoubleValue(avgSimilarity)));
        else
          ((DoubleValue)results["Average Population Similarity"].Value).Value = avgSimilarity;

        // store average maximum similarity
        if (!results.ContainsKey("Average Maximum Solution Similarity"))
          results.Add(new Result("Average Maximum Solution Similarity", new DoubleValue(avgMaxSimilarity)));
        else
          ((DoubleValue)results["Average Maximum Solution Similarity"].Value).Value = avgMaxSimilarity;

        // store population similarity data table
        DataTable similarityDataTable;
        if (!results.ContainsKey("Population Similarity")) {
          similarityDataTable = new DataTable("Population Similarity");
          results.Add(new Result("Population Similarity", similarityDataTable));
          DataRowVisualProperties visualProperties = new DataRowVisualProperties();
          visualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Line;
          visualProperties.StartIndexZero = true;
          similarityDataTable.Rows.Add(new DataRow("Average Population Similarity", null, visualProperties));
          similarityDataTable.Rows.Add(new DataRow("Average Maximum Solution Similarity", null, visualProperties));
        } else {
          similarityDataTable = (DataTable)results["Population Similarity"].Value;
        }
        similarityDataTable.Rows["Average Population Similarity"].Values.Add(avgSimilarity);
        similarityDataTable.Rows["Average Maximum Solution Similarity"].Values.Add(avgMaxSimilarity);

        // store maximum similarities
        DataTable maxSimilaritiesDataTable = new DataTable("Maximum Solution Similarities");
        maxSimilaritiesDataTable.Rows.Add(new DataRow("Maximum Solution Similarity"));
        maxSimilaritiesDataTable.Rows["Maximum Solution Similarity"].VisualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Columns;
        maxSimilaritiesDataTable.Rows["Maximum Solution Similarity"].Values.AddRange(maxSimilarities);
        if (!results.ContainsKey("Maximum Solution Similarities")) {
          results.Add(new Result("Maximum Solution Similarities", maxSimilaritiesDataTable));
        } else {
          results["Maximum Solution Similarities"].Value = maxSimilaritiesDataTable;
        }

        // store maximum similarities history
        if (storeHistory) {
          if (!results.ContainsKey("Maximum Solution Similarities History")) {
            DataTableHistory history = new DataTableHistory();
            history.Add(maxSimilaritiesDataTable);
            results.Add(new Result("Maximum Solution Similarities History", history));
          } else {
            ((DataTableHistory)results["Maximum Solution Similarities History"].Value).Add(maxSimilaritiesDataTable);
          }
        }
      }
      return base.Apply();
    }

    protected abstract double[,] CalculateSimilarities(T[] solutions);
  }
}
