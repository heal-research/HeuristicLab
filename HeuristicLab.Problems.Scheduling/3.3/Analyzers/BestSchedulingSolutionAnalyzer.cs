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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.ScheduleEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Scheduling {

  [Item("BestSchedulingSolutionAnalyzer", "An operator for analyzing the best solution of Scheduling Problems given in schedule-representation.")]
  [StorableClass]
  public sealed class BestSchedulingSolutionAnalyzer : SchedulingAnalyzer, IStochasticOperator {
    [StorableConstructor]
    private BestSchedulingSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    private BestSchedulingSolutionAnalyzer(BestSchedulingSolutionAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestSchedulingSolutionAnalyzer(this, cloner);
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IRandom Random {
      get { return RandomParameter.ActualValue; }
    }

    public BestSchedulingSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator."));
    }

    public override IOperation Apply() {
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      ItemArray<Schedule> solutions = ScheduleParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;
      bool max = MaximizationParameter.ActualValue.Value;
      DoubleValue bestKnownQuality = BestKnownQualityParameter.ActualValue;

      int i = -1;
      if (!max)
        i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      else i = qualities.Select((x, index) => new { index, x.Value }).OrderByDescending(x => x.Value).First().index;

      if (bestKnownQuality == null ||
          max && qualities[i].Value > bestKnownQuality.Value ||
          !max && qualities[i].Value < bestKnownQuality.Value) {
        BestKnownQualityParameter.ActualValue = new DoubleValue(qualities[i].Value);
        BestKnownSolutionParameter.ActualValue = (Schedule)solutions[i].Clone();
      }

      Schedule bestSolution = BestSolutionParameter.ActualValue;
      if (bestSolution == null) {
        bestSolution = (Schedule)solutions[i].Clone();
        bestSolution.Quality = (DoubleValue)qualities[i].Clone();
        BestSolutionParameter.ActualValue = bestSolution;
        results.Add(new Result("Best Scheduling Solution", bestSolution));
      } else {
        if (max && bestSolution.Quality.Value < qualities[i].Value ||
          !max && bestSolution.Quality.Value > qualities[i].Value) {
          bestSolution.Quality.Value = qualities[i].Value;
          bestSolution.Resources = (ItemList<Resource>)solutions[i].Resources.Clone();
        }
      }

      return base.Apply();
    }
  }
}
