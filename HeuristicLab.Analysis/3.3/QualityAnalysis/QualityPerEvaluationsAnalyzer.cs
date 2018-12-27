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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  [Item("QualityPerEvaluationsAnalyzer", @"Creates a plot of the solution quality with respect to the number of evaluated solutions.")]
  [StorableClass]
  public class QualityPerEvaluationsAnalyzer : SingleSuccessorOperator, IAnalyzer, ISingleObjectiveOperator {
    public virtual bool EnabledByDefault {
      get { return false; }
    }

    public ILookupParameter<DoubleValue> BestQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["BestQuality"]; }
    }
    public ILookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }
    public ILookupParameter<IntValue> EvaluatedMovesParameter {
      get { return (ILookupParameter<IntValue>)Parameters["EvaluatedMoves"]; }
    }
    public IValueLookupParameter<DoubleValue> MoveCostPerSolutionParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["MoveCostPerSolution"]; }
    }
    public IResultParameter<IndexedDataTable<double>> QualityPerEvaluationsParameter {
      get { return (IResultParameter<IndexedDataTable<double>>)Parameters["QualityPerEvaluations"]; }
    }

    [StorableConstructor]
    protected QualityPerEvaluationsAnalyzer(bool deserializing) : base(deserializing) { }
    protected QualityPerEvaluationsAnalyzer(QualityPerEvaluationsAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public QualityPerEvaluationsAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("BestQuality", "The quality value that should be compared."));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The number of evaluated solutions."));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedMoves", "The number of evaluated moves."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MoveCostPerSolution", "The cost to evaluate a move as a ratio to the cost of evaluating a solution.", new DoubleValue(1)));
      Parameters.Add(new ResultParameter<IndexedDataTable<double>>("QualityPerEvaluations", "Data table containing the first hitting graph with evaluations as the x-axis."));
      QualityPerEvaluationsParameter.DefaultValue = new IndexedDataTable<double>("Quality per Evaluations") {
        VisualProperties = {
          XAxisTitle = "Evaluations",
          YAxisTitle = "Quality"
        },
        Rows = { new IndexedDataRow<double>("First-hit Graph") { VisualProperties = {
          ChartType = DataRowVisualProperties.DataRowChartType.StepLine,
          LineWidth = 2
        } } }
      };
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QualityPerEvaluationsAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      var bestQuality = BestQualityParameter.ActualValue.Value;
      var evalSols = EvaluatedSolutionsParameter.ActualValue;
      var evalMoves = EvaluatedMovesParameter.ActualValue;
      var evaluations = 0.0;
      if (evalSols != null) evaluations += evalSols.Value;
      if (evalMoves != null) evaluations += evalMoves.Value * MoveCostPerSolutionParameter.ActualValue.Value;

      if (evaluations > 0) {
        var dataTable = QualityPerEvaluationsParameter.ActualValue;
        var values = dataTable.Rows["First-hit Graph"].Values;

        var newEntry = Tuple.Create(evaluations, bestQuality);

        if (values.Count == 0) {
          values.Add(newEntry); // record the first data
          values.Add(Tuple.Create(evaluations, bestQuality)); // last entry records max number of evaluations
          return base.Apply();
        }

        var improvement = values.Last().Item2 != bestQuality;
        if (improvement) {
          values[values.Count - 1] = newEntry; // record the improvement
          values.Add(Tuple.Create(evaluations, bestQuality)); // last entry records max number of evaluations
        } else {
          values[values.Count - 1] = Tuple.Create(evaluations, bestQuality); // the last entry is updated
        }
      }
      return base.Apply();
    }
  }
}
