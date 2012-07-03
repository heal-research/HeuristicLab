#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.LinearAssignment {
  [Item("BestLAPSolutionAnalyzer", "Analyzes the best solution found.")]
  [StorableClass]
  public class BestLAPSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer {
    public bool EnabledByDefault { get { return true; } }

    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ILookupParameter<DoubleMatrix> CostsParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Costs"]; }
    }
    public IValueLookupParameter<StringArray> RowNamesParameter {
      get { return (IValueLookupParameter<StringArray>)Parameters["RowNames"]; }
    }
    public IValueLookupParameter<StringArray> ColumnNamesParameter {
      get { return (IValueLookupParameter<StringArray>)Parameters["ColumnNames"]; }
    }
    public IScopeTreeLookupParameter<Permutation> AssignmentParameter {
      get { return (IScopeTreeLookupParameter<Permutation>)Parameters["Assignment"]; }
    }
    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<LAPAssignment> BestSolutionParameter {
      get { return (ILookupParameter<LAPAssignment>)Parameters["BestSolution"]; }
    }
    public ILookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public ILookupParameter<ItemSet<Permutation>> BestKnownSolutionsParameter {
      get { return (ILookupParameter<ItemSet<Permutation>>)Parameters["BestKnownSolutions"]; }
    }
    public ILookupParameter<Permutation> BestKnownSolutionParameter {
      get { return (ILookupParameter<Permutation>)Parameters["BestKnownSolution"]; }
    }
    public IValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (IValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    [StorableConstructor]
    protected BestLAPSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    protected BestLAPSolutionAnalyzer(BestLAPSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public BestLAPSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Costs", LinearAssignmentProblem.CostsDescription));
      Parameters.Add(new ValueLookupParameter<StringArray>("RowNames", LinearAssignmentProblem.RowNamesDescription));
      Parameters.Add(new ValueLookupParameter<StringArray>("ColumnNames", LinearAssignmentProblem.ColumnNamesDescription));
      Parameters.Add(new ScopeTreeLookupParameter<Permutation>("Assignment", "The LAP solutions from which the best solution should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the LAP solutions which should be analyzed."));
      Parameters.Add(new LookupParameter<LAPAssignment>("BestSolution", "The best LAP solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the best LAP solution should be stored."));
      Parameters.Add(new LookupParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this LAP instance."));
      Parameters.Add(new LookupParameter<ItemSet<Permutation>>("BestKnownSolutions", "The best known solutions (there may be multiple) of this LAP instance."));
      Parameters.Add(new LookupParameter<Permutation>("BestKnownSolution", "The best known solution of this LAP instance."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestLAPSolutionAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      var costs = CostsParameter.ActualValue;
      var rowNames = RowNamesParameter.ActualValue;
      var columnNames = ColumnNamesParameter.ActualValue;
      var permutations = AssignmentParameter.ActualValue;
      var qualities = QualityParameter.ActualValue;
      var results = ResultsParameter.ActualValue;
      bool max = MaximizationParameter.ActualValue.Value;
      DoubleValue bestKnownQuality = BestKnownQualityParameter.ActualValue;
      
      var sorted = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).ToArray();
      if (max) sorted = sorted.Reverse().ToArray();
      int i = sorted.First().index;

      if (bestKnownQuality == null
          || max && qualities[i].Value > bestKnownQuality.Value
          || !max && qualities[i].Value < bestKnownQuality.Value) {
        // if there isn't a best-known quality or we improved the best-known quality we'll add the current solution as best-known
        BestKnownQualityParameter.ActualValue = new DoubleValue(qualities[i].Value);
        BestKnownSolutionParameter.ActualValue = (Permutation)permutations[i].Clone();
        BestKnownSolutionsParameter.ActualValue = new ItemSet<Permutation>(new PermutationEqualityComparer());
        BestKnownSolutionsParameter.ActualValue.Add((Permutation)permutations[i].Clone());
      } else if (bestKnownQuality.Value == qualities[i].Value) {
        // if we matched the best-known quality we'll try to set the best-known solution if it isn't null
        // and try to add it to the pool of best solutions if it is different
        if (BestKnownSolutionParameter.ActualValue == null)
          BestKnownSolutionParameter.ActualValue = (Permutation)permutations[i].Clone();
        if (BestKnownSolutionsParameter.ActualValue == null)
          BestKnownSolutionsParameter.ActualValue = new ItemSet<Permutation>(new PermutationEqualityComparer());
        foreach (var k in sorted) { // for each solution that we found check if it is in the pool of best-knowns
          if (!max && k.Value > qualities[i].Value
            || max && k.Value < qualities[i].Value) break; // stop when we reached a solution worse than the best-known quality
          Permutation p = permutations[k.index];
          if (!BestKnownSolutionsParameter.ActualValue.Contains(p))
            BestKnownSolutionsParameter.ActualValue.Add((Permutation)permutations[k.index].Clone());
        }
      }

      LAPAssignment assignment = BestSolutionParameter.ActualValue;
      if (assignment == null) {
        assignment = new LAPAssignment(costs, rowNames, columnNames, (Permutation)permutations[i].Clone(), new DoubleValue(qualities[i].Value));
        BestSolutionParameter.ActualValue = assignment;
        results.Add(new Result("Best LAP Solution", assignment));
      } else {
        if (max && assignment.Quality.Value < qualities[i].Value ||
          !max && assignment.Quality.Value > qualities[i].Value) {
          assignment.Costs = costs;
          assignment.Assignment = (Permutation)permutations[i].Clone();
          assignment.Quality.Value = qualities[i].Value;
          if (rowNames != null)
            assignment.RowNames = rowNames;
          else assignment.RowNames = null;
          if (columnNames != null)
            assignment.ColumnNames = columnNames;
          else assignment.ColumnNames = null;
        }
      }

      return base.Apply();
    }
  }
}
