#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.LinearAssignment {
  [Item("Linear Assignment Problem (LAP)", "In the linear assignment problem (LAP) an assignment of workers to jobs has to be found such that each worker is assigned to exactly one job, each job is assigned to exactly one worker and the sum of the resulting costs is minimal (or maximal).")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 130)]
  [StorableType("7766E004-A93D-4CA6-8012-AE5E8F4C4D85")]
  public sealed class LinearAssignmentProblem : PermutationProblem {
    public static readonly string CostsDescription = "The cost matrix that describes the assignment of rows to columns.";
    public static readonly string RowNamesDescription = "The elements represented by the rows of the costs matrix.";
    public static readonly string ColumnNamesDescription = "The elements represented by the columns of the costs matrix.";

    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Type; }
    }

    #region Parameter Properties
    public IValueParameter<DoubleMatrix> CostsParameter {
      get { return (IValueParameter<DoubleMatrix>)Parameters["Costs"]; }
    }
    public IValueParameter<ItemSet<Permutation>> BestKnownSolutionsParameter {
      get { return (IValueParameter<ItemSet<Permutation>>)Parameters["BestKnownSolutions"]; }
    }
    public IValueParameter<Permutation> BestKnownSolutionParameter {
      get { return (IValueParameter<Permutation>)Parameters["BestKnownSolution"]; }
    }
    public IValueParameter<StringArray> RowNamesParameter {
      get { return (IValueParameter<StringArray>)Parameters["RowNames"]; }
    }
    public IValueParameter<StringArray> ColumnNamesParameter {
      get { return (IValueParameter<StringArray>)Parameters["ColumnNames"]; }
    }
    private IResultParameter<LAPAssignment> BestLAPSolutionParameter {
      get { return (IResultParameter<LAPAssignment>)Parameters["Best LAP Solution"]; }
    }
    //public IResultDefinition<LAPAssignment> BestLAPSolution => BestLAPSolutionParameter;
    #endregion

    #region Properties
    public DoubleMatrix Costs {
      get { return CostsParameter.Value; }
      set { CostsParameter.Value = value; }
    }
    public StringArray RowNames {
      get { return RowNamesParameter.Value; }
      set { RowNamesParameter.Value = value; }
    }
    public StringArray ColumnNames {
      get { return ColumnNamesParameter.Value; }
      set { ColumnNamesParameter.Value = value; }
    }
    public ItemSet<Permutation> BestKnownSolutions {
      get { return BestKnownSolutionsParameter.Value; }
      set { BestKnownSolutionsParameter.Value = value; }
    }
    public Permutation BestKnownSolution {
      get { return BestKnownSolutionParameter.Value; }
      set { BestKnownSolutionParameter.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private LinearAssignmentProblem(StorableConstructorFlag _) : base(_) { }
    private LinearAssignmentProblem(LinearAssignmentProblem original, Cloner cloner)
      : base(original, cloner) {
      AttachEventHandlers();
    }
    public LinearAssignmentProblem()
      : base(new PermutationEncoding("Assignment")) {
      Parameters.Add(new ValueParameter<DoubleMatrix>("Costs", CostsDescription, new DoubleMatrix(3, 3)));
      Parameters.Add(new OptionalValueParameter<ItemSet<Permutation>>("BestKnownSolutions", "The list of best known solutions which is updated whenever a new better solution is found or may be the optimal solution if it is known beforehand.", null));
      Parameters.Add(new OptionalValueParameter<Permutation>("BestKnownSolution", "The best known solution which is updated whenever a new better solution is found or may be the optimal solution if it is known beforehand.", null));
      Parameters.Add(new OptionalValueParameter<StringArray>("RowNames", RowNamesDescription));
      Parameters.Add(new OptionalValueParameter<StringArray>("ColumnNames", ColumnNamesDescription));
      Parameters.Add(new ResultParameter<LAPAssignment>("Best LAP Solution", "The best so far LAP solution found."));

      ((ValueParameter<DoubleMatrix>)CostsParameter).ReactOnValueToStringChangedAndValueItemImageChanged = false;
      ((OptionalValueParameter<StringArray>)RowNamesParameter).ReactOnValueToStringChangedAndValueItemImageChanged = false;
      ((OptionalValueParameter<StringArray>)ColumnNamesParameter).ReactOnValueToStringChangedAndValueItemImageChanged = false;

      RowNames = new StringArray(new string[] { "Eric", "Robert", "Allison" });
      ColumnNames = new StringArray(new string[] { "MRI", "Blood test", "Angiogram" });
      Costs[0, 0] = 4; Costs[0, 1] = 5; Costs[0, 2] = 3;
      Costs[1, 0] = 6; Costs[1, 1] = 6; Costs[1, 2] = 4;
      Costs[2, 0] = 5; Costs[2, 1] = 5; Costs[2, 2] = 1;

      InitializeOperators();
      Parameterize();
      AttachEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LinearAssignmentProblem(this, cloner);
    }

    public override ISingleObjectiveEvaluationResult Evaluate(Permutation assignment, IRandom random, CancellationToken cancellationToken) {
      var costs = Costs;

      int len = assignment.Length;
      double quality = 0;
      for (int i = 0; i < len; i++) {
        quality += costs[i, assignment[i]];
      }

      return new SingleObjectiveEvaluationResult(quality);
    }

    public override void Analyze(Permutation[] permutations, double[] qualities, ResultCollection results, IRandom random) {
      base.Analyze(permutations, qualities, results, random);

      var costs = Costs;
      var rowNames = RowNames;
      var columnNames = ColumnNames;
      bool max = Maximization;

      var sorted = qualities.Select((x, index) => new { Index = index, Quality = x }).OrderBy(x => x.Quality).ToArray();
      if (max) Array.Reverse(sorted);
      int i = sorted[0].Index;
      var best = Tuple.Create(permutations[i], qualities[i]);

      if (double.IsNaN(BestKnownQuality) || IsBetter(best.Item2, BestKnownQuality)) {
        // if there isn't a best-known quality or we improved the best-known quality we'll add the current solution as best-known
        BestKnownQuality = best.Item2;
        BestKnownSolution = (Permutation)best.Item1.Clone();
        BestKnownSolutions = new ItemSet<Permutation>(new PermutationEqualityComparer());
        BestKnownSolutions.Add((Permutation)best.Item1.Clone());
      } else if (BestKnownQuality == best.Item2) {
        // if we matched the best-known quality we'll try to set the best-known solution if it isn't null
        // and try to add it to the pool of best solutions if it is different
        if (BestKnownSolution == null) BestKnownSolution = (Permutation)best.Item1.Clone();
        if (BestKnownSolutions == null) BestKnownSolutions = new ItemSet<Permutation>(new PermutationEqualityComparer());

        foreach (var k in sorted) { // for each solution that we found check if it is in the pool of best-knowns
          if (IsBetter(best.Item2, k.Quality)) break; // stop when we reached a solution worse than the best-known quality
          var p = permutations[k.Index];
          if (!BestKnownSolutions.Contains(p))
            BestKnownSolutions.Add((Permutation)permutations[k.Index].Clone());
        }
      }

      LAPAssignment solution = BestLAPSolutionParameter.ActualValue;
      if (solution == null) {
        solution = new LAPAssignment(costs, rowNames, columnNames, (Permutation)best.Item1.Clone(), new DoubleValue(best.Item2));
        BestLAPSolutionParameter.ActualValue = solution;
      } else {
        if (IsBetter(best.Item2, solution.Quality.Value)) {
          solution.Costs = costs;
          solution.Assignment = (Permutation)best.Item1.Clone();
          solution.Quality.Value = best.Item2;
          if (rowNames != null)
            solution.RowNames = rowNames;
          else solution.RowNames = null;
          if (columnNames != null)
            solution.ColumnNames = columnNames;
          else solution.ColumnNames = null;
        }
      }

    }

    #region Events
    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      Parameterize();
    }
    protected override void OnOperatorsChanged() {
      base.OnOperatorsChanged();
      Parameterize();
    }
    private void Costs_RowsChanged(object sender, EventArgs e) {
      if (Costs.Rows != Costs.Columns) {
        ((IStringConvertibleMatrix)Costs).Columns = Costs.Rows;
        Parameterize();
      }
    }
    private void Costs_ColumnsChanged(object sender, EventArgs e) {
      if (Costs.Rows != Costs.Columns) {
        ((IStringConvertibleMatrix)Costs).Rows = Costs.Columns;
        Parameterize();
      }
    }
    private void Costs_Reset(object sender, EventArgs e) {
      Parameterize();
    }
    private void SolutionCreator_PermutationParameter_ActualNameChanged(object sender, EventArgs e) {
      Parameterize();
    }
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      AttachEventHandlers();
    }

    private void AttachEventHandlers() {
      Costs.RowsChanged += new EventHandler(Costs_RowsChanged);
      Costs.ColumnsChanged += new EventHandler(Costs_ColumnsChanged);
      Costs.Reset += new EventHandler(Costs_Reset);
    }

    private void InitializeOperators() {
      Operators.AddRange(ApplicationManager.Manager.GetInstances<IPermutationOperator>());
      Operators.RemoveAll(x => x is IMoveOperator);

      Operators.Add(new HammingSimilarityCalculator());
      Operators.Add(new QualitySimilarityCalculator());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));
    }

    private void Parameterize() {
      if (Costs.Rows != Dimension) Dimension = Costs.Rows;
      foreach (var similarityCalculator in Operators.OfType<ISolutionSimilarityCalculator>()) {
        similarityCalculator.SolutionVariableName = Encoding.Name;
        similarityCalculator.QualityVariableName = Evaluator.QualityParameter.ActualName;
      }
    }
    #endregion
  }
}
