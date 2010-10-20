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
  /// An operator for analyzing the frequency of alleles.
  /// </summary>
  [Item("AlleleFrequencyAnalyzer", "An operator for analyzing the frequency of alleles.")]
  [StorableClass]
  public abstract class AlleleFrequencyAnalyzer<T> : SingleSuccessorOperator, IAnalyzer where T : class, IItem {
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
    public LookupParameter<T> BestKnownSolutionParameter {
      get { return (LookupParameter<T>)Parameters["BestKnownSolution"]; }
    }
    public LookupParameter<AlleleFrequencyArray> AlleleFrequenciesParameter {
      get { return (LookupParameter<AlleleFrequencyArray>)Parameters["AlleleFrequencies"]; }
    }

    public AlleleFrequencyAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem."));
      Parameters.Add(new ScopeTreeLookupParameter<T>("Solution", "The solutions whose alleles should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the solutions which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the allele frequency analysis results should be stored."));
      Parameters.Add(new LookupParameter<T>("BestKnownSolution", "The best known solution."));
      Parameters.Add(new LookupParameter<AlleleFrequencyArray>("AlleleFrequencies", "The frequencies of the alleles in the current iteration."));
    }

    public override IOperation Apply() {
      bool max = MaximizationParameter.ActualValue.Value;
      ItemArray<T> solutions = SolutionParameter.ActualValue;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;
      T bestKnownSolution = BestKnownSolutionParameter.ActualValue;

      int bestIndex = -1;
      if (!max) bestIndex = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      else bestIndex = qualities.Select((x, index) => new { index, x.Value }).OrderByDescending(x => x.Value).First().index;

      Allele[] bestAlleles = CalculateAlleles(solutions[bestIndex]);
      Allele[] bestKnownAlleles = null;
      if (bestKnownSolution != null) {
        bestKnownAlleles = CalculateAlleles(bestKnownSolution);
      }

      var frequencies = solutions.SelectMany((s, index) => CalculateAlleles(s).Select(a => new { Allele = a, Quality = qualities[index] })).
                        GroupBy(x => x.Allele.Id).
                        Select(x => new AlleleFrequency(x.Key,
                                                        x.Count() / ((double)solutions.Length),
                                                        x.Average(a => a.Allele.Impact),
                                                        x.Average(a => a.Quality.Value),
                                                        bestKnownAlleles == null ? false : bestKnownAlleles.Any(a => a.Id == x.Key),
                                                        bestAlleles.Any(a => a.Id == x.Key)));

      AlleleFrequencyArray frequenciesArray = new AlleleFrequencyArray(frequencies.OrderBy(x => x.AverageImpact));
      AlleleFrequenciesParameter.ActualValue = frequenciesArray;
      if (results.ContainsKey("Allele Frequencies"))
        results["Allele Frequencies"].Value = frequenciesArray;
      else
        results.Add(new Result("Allele Frequencies", frequenciesArray));

      return base.Apply();
    }

    protected abstract Allele[] CalculateAlleles(T solution);
  }
}
