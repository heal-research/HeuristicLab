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
using System.Drawing;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.LinearAssignment {
  [Item("Linear Assignment Problem (LAP)", "In the linear assignment problem (LAP) an assignment of workers to jobs has to be found such that each worker is assigned to exactly one job, each job is assigned to exactly one worker and the sum of the resulting costs is minimal (or maximal).")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 130)]
  [StorableClass]
  public sealed class LinearAssignmentProblem : SingleObjectiveHeuristicOptimizationProblem<ILAPEvaluator, IPermutationCreator>,
    ISingleObjectiveProblem<PermutationEncoding,Permutation>, IStorableContent {
    public static readonly string CostsDescription = "The cost matrix that describes the assignment of rows to columns.";
    public static readonly string RowNamesDescription = "The elements represented by the rows of the costs matrix.";
    public static readonly string ColumnNamesDescription = "The elements represented by the columns of the costs matrix.";

    public string Filename { get; set; }

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

    [Storable]
    private BestLAPSolutionAnalyzer bestLAPSolutionAnalyzer;

    [StorableConstructor]
    private LinearAssignmentProblem(bool deserializing) : base(deserializing) { }
    private LinearAssignmentProblem(LinearAssignmentProblem original, Cloner cloner)
      : base(original, cloner) {
      this.bestLAPSolutionAnalyzer = cloner.Clone(original.bestLAPSolutionAnalyzer);
      AttachEventHandlers();
    }
    public LinearAssignmentProblem()
      : base(new LAPEvaluator(), new RandomPermutationCreator()) {
      Parameters.Add(new ValueParameter<DoubleMatrix>("Costs", CostsDescription, new DoubleMatrix(3, 3)));
      Parameters.Add(new OptionalValueParameter<ItemSet<Permutation>>("BestKnownSolutions", "The list of best known solutions which is updated whenever a new better solution is found or may be the optimal solution if it is known beforehand.", null));
      Parameters.Add(new OptionalValueParameter<Permutation>("BestKnownSolution", "The best known solution which is updated whenever a new better solution is found or may be the optimal solution if it is known beforehand.", null));
      Parameters.Add(new OptionalValueParameter<StringArray>("RowNames", RowNamesDescription));
      Parameters.Add(new OptionalValueParameter<StringArray>("ColumnNames", ColumnNamesDescription));

      ((ValueParameter<DoubleMatrix>)CostsParameter).ReactOnValueToStringChangedAndValueItemImageChanged = false;
      ((OptionalValueParameter<StringArray>)RowNamesParameter).ReactOnValueToStringChangedAndValueItemImageChanged = false;
      ((OptionalValueParameter<StringArray>)ColumnNamesParameter).ReactOnValueToStringChangedAndValueItemImageChanged = false;

      RowNames = new StringArray(new string[] { "Eric", "Robert", "Allison" });
      ColumnNames = new StringArray(new string[] { "MRI", "Blood test", "Angiogram" });
      Costs[0, 0] = 4; Costs[0, 1] = 5; Costs[0, 2] = 3;
      Costs[1, 0] = 6; Costs[1, 1] = 6; Costs[1, 2] = 4;
      Costs[2, 0] = 5; Costs[2, 1] = 5; Costs[2, 2] = 1;

      bestLAPSolutionAnalyzer = new BestLAPSolutionAnalyzer();
      SolutionCreator.PermutationParameter.ActualName = "Assignment";
      InitializeOperators();
      Parameterize();
      AttachEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LinearAssignmentProblem(this, cloner);
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
    protected override void OnSolutionCreatorChanged() {
      base.OnSolutionCreatorChanged();
      SolutionCreator.PermutationParameter.ActualNameChanged += new EventHandler(SolutionCreator_PermutationParameter_ActualNameChanged);
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
      SolutionCreator.PermutationParameter.ActualNameChanged += new EventHandler(SolutionCreator_PermutationParameter_ActualNameChanged);
    }

    private void InitializeOperators() {
      Operators.AddRange(ApplicationManager.Manager.GetInstances<IPermutationOperator>());
      Operators.RemoveAll(x => x is IMoveOperator);
      Operators.Add(bestLAPSolutionAnalyzer);
    }

    private void Parameterize() {
      SolutionCreator.LengthParameter.Value = new IntValue(Costs.Rows);
      SolutionCreator.LengthParameter.Hidden = true;
      SolutionCreator.PermutationTypeParameter.Value = new PermutationType(PermutationTypes.Absolute);
      SolutionCreator.PermutationTypeParameter.Hidden = true;
      Evaluator.CostsParameter.ActualName = CostsParameter.Name;
      Evaluator.CostsParameter.Hidden = true;
      Evaluator.AssignmentParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
      Evaluator.AssignmentParameter.Hidden = true;

      foreach (var op in Operators.OfType<IPermutationCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
        op.ParentsParameter.Hidden = true;
        op.ChildParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
        op.ChildParameter.Hidden = true;
      }

      foreach (var op in Operators.OfType<IPermutationManipulator>()) {
        op.PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
        op.PermutationParameter.Hidden = true;
      }

      foreach (var op in Operators.OfType<IPermutationMultiNeighborhoodShakingOperator>()) {
        op.PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
        op.PermutationParameter.Hidden = true;
      }

      bestLAPSolutionAnalyzer.AssignmentParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
      bestLAPSolutionAnalyzer.BestKnownQualityParameter.ActualName = BestKnownQualityParameter.Name;
      bestLAPSolutionAnalyzer.BestKnownSolutionParameter.ActualName = BestKnownSolutionParameter.Name;
      bestLAPSolutionAnalyzer.BestKnownSolutionsParameter.ActualName = BestKnownSolutionsParameter.Name;
      bestLAPSolutionAnalyzer.CostsParameter.ActualName = CostsParameter.Name;
      bestLAPSolutionAnalyzer.MaximizationParameter.ActualName = MaximizationParameter.Name;
      bestLAPSolutionAnalyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
    }
    #endregion
  }
}
