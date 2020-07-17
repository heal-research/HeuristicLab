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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.QuadraticAssignment {
  [Item("Quadratic Assignment Problem (QAP)", "The Quadratic Assignment Problem (QAP) can be described as the problem of assigning N facilities to N fixed locations such that there is exactly one facility in each location and that the sum of the distances multiplied by the connection strength between the facilities becomes minimal.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 140)]
  [StorableType("A86B1F49-D8E6-45E4-8EFB-8F5CCA2F9DC7")]
  public sealed class QuadraticAssignmentProblem : PermutationProblem,
    IProblemInstanceConsumer<QAPData>,
    IProblemInstanceConsumer<TSPData>, IProblemInstanceExporter<QAPData> {
    public static int ProblemSizeLimit = 1000;

    public static new Image StaticItemImage {
      get { return Common.Resources.VSImageLibrary.Type; }
    }

    #region Parameter Properties
    [Storable] public IValueParameter<ItemSet<Permutation>> BestKnownSolutionsParameter { get; private set; }
    [Storable] public IValueParameter<Permutation> BestKnownSolutionParameter { get; private set; }
    [Storable] public IValueParameter<DoubleMatrix> WeightsParameter { get; private set; }
    [Storable] public IValueParameter<DoubleMatrix> DistancesParameter { get; private set; }
    [Storable] public IValueParameter<DoubleValue> LowerBoundParameter { get; private set; }
    [Storable] public IValueParameter<DoubleValue> AverageQualityParameter { get; private set; }
    #endregion

    #region Properties
    public ItemSet<Permutation> BestKnownSolutions {
      get { return BestKnownSolutionsParameter.Value; }
      set { BestKnownSolutionsParameter.Value = value; }
    }
    public Permutation BestKnownSolution {
      get { return BestKnownSolutionParameter.Value; }
      set { BestKnownSolutionParameter.Value = value; }
    }
    public DoubleMatrix Weights {
      get { return WeightsParameter.Value; }
      set { WeightsParameter.Value = value; }
    }
    public DoubleMatrix Distances {
      get { return DistancesParameter.Value; }
      set { DistancesParameter.Value = value; }
    }
    public DoubleValue LowerBound {
      get { return LowerBoundParameter.Value; }
      set { LowerBoundParameter.Value = value; }
    }
    public DoubleValue AverageQuality {
      get { return AverageQualityParameter.Value; }
      set { AverageQualityParameter.Value = value; }
    }

    private BestQAPSolutionAnalyzer BestQAPSolutionAnalyzer {
      get { return Operators.OfType<BestQAPSolutionAnalyzer>().FirstOrDefault(); }
    }

    private QAPAlleleFrequencyAnalyzer QAPAlleleFrequencyAnalyzer {
      get { return Operators.OfType<QAPAlleleFrequencyAnalyzer>().FirstOrDefault(); }
    }
    #endregion

    [StorableConstructor]
    private QuadraticAssignmentProblem(StorableConstructorFlag _) : base(_) { }
    private QuadraticAssignmentProblem(QuadraticAssignmentProblem original, Cloner cloner)
      : base(original, cloner) {
      BestKnownSolutionsParameter = cloner.Clone(original.BestKnownSolutionsParameter);
      BestKnownSolutionParameter = cloner.Clone(original.BestKnownSolutionParameter);
      WeightsParameter = cloner.Clone(original.WeightsParameter);
      DistancesParameter = cloner.Clone(original.DistancesParameter);
      LowerBoundParameter = cloner.Clone(original.LowerBoundParameter);
      AverageQualityParameter = cloner.Clone(original.AverageQualityParameter);
    }
    public QuadraticAssignmentProblem()
      : base(new PermutationEncoding("Assignment") { Length = 5 }) {
      Maximization = false;
      Encoding.LengthParameter.ReadOnly = DimensionRefParameter.ReadOnly = true;
      Encoding.PermutationTypeParameter.ReadOnly = PermutationTypeRefParameter.ReadOnly = true;
      PermutationTypeRefParameter.Hidden = true;

      Parameters.Add(BestKnownSolutionsParameter = new OptionalValueParameter<ItemSet<Permutation>>("BestKnownSolutions", "The list of best known solutions which is updated whenever a new better solution is found or may be the optimal solution if it is known beforehand.", null));
      Parameters.Add(BestKnownSolutionParameter = new OptionalValueParameter<Permutation>("BestKnownSolution", "The best known solution which is updated whenever a new better solution is found or may be the optimal solution if it is known beforehand.", null));
      Parameters.Add(WeightsParameter = new ValueParameter<DoubleMatrix>("Weights", "The strength of the connection between the facilities."));
      Parameters.Add(DistancesParameter = new ValueParameter<DoubleMatrix>("Distances", "The distance matrix which can either be specified directly without the coordinates, or can be calculated automatically from the coordinates."));
      Parameters.Add(LowerBoundParameter = new OptionalValueParameter<DoubleValue>("LowerBound", "The Gilmore-Lawler lower bound to the solution quality."));
      Parameters.Add(AverageQualityParameter = new OptionalValueParameter<DoubleValue>("AverageQuality", "The expected quality of a random solution."));

      WeightsParameter.GetsCollected = false;
      Weights = new DoubleMatrix(new double[,] {
        { 0, 1, 0, 0, 1 },
        { 1, 0, 1, 0, 0 },
        { 0, 1, 0, 1, 0 },
        { 0, 0, 1, 0, 1 },
        { 1, 0, 0, 1, 0 }
      }, @readonly: true);

      DistancesParameter.GetsCollected = false;
      Distances = new DoubleMatrix(new double[,] {
        {   0, 360, 582, 582, 360 },
        { 360,   0, 360, 582, 582 },
        { 582, 360,   0, 360, 582 },
        { 582, 582, 360,   0, 360 },
        { 360, 582, 582, 360,   0 }
      }, @readonly: true);

      InitializeOperators();
    }

    public override ISingleObjectiveEvaluationResult Evaluate(Permutation assignment, IRandom random, CancellationToken cancellationToken) {
      var quality = Evaluate(assignment, cancellationToken);
      return new SingleObjectiveEvaluationResult(quality);
    }

    public double Evaluate(Permutation assignment, CancellationToken cancellationToken) {
      double quality = 0;
      for (int i = 0; i < assignment.Length; i++) {
        for (int j = 0; j < assignment.Length; j++) {
          quality += Weights[i, j] * Distances[assignment[i], assignment[j]];
        }
      }
      return quality;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QuadraticAssignmentProblem(this, cloner);
    }

    #region Events
    protected override void OnEncodingChanged() {
      base.OnEncodingChanged();
      Dimension = Weights.Rows;
      Parameterize();
    }
    protected override void OnEvaluatorChanged() {
      Evaluator.QualityParameter.ActualNameChanged += Evaluator_QualityParameter_ActualNameChanged;
      Parameterize();
      base.OnEvaluatorChanged();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      Parameterize();
    }
    #endregion

    protected override void DimensionOnChanged() {
      base.DimensionOnChanged();
      if (Dimension != Weights.Rows) Dimension = Weights.Rows;
    }

    #region Helpers
    private void InitializeOperators() {
      Operators.AddRange(ApplicationManager.Manager.GetInstances<IQAPMoveEvaluator>());
      Operators.AddRange(ApplicationManager.Manager.GetInstances<IQAPLocalImprovementOperator>());
      Operators.Add(new BestQAPSolutionAnalyzer());
      Operators.Add(new QAPAlleleFrequencyAnalyzer());

      Operators.Add(new QAPSimilarityCalculator());
      Parameterize();
    }
    private void Parameterize() {
      var operators = new List<IItem>();
      if (BestQAPSolutionAnalyzer != null) {
        operators.Add(BestQAPSolutionAnalyzer);
        BestQAPSolutionAnalyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        BestQAPSolutionAnalyzer.DistancesParameter.ActualName = DistancesParameter.Name;
        BestQAPSolutionAnalyzer.WeightsParameter.ActualName = WeightsParameter.Name;
        BestQAPSolutionAnalyzer.BestKnownQualityParameter.ActualName = BestKnownQualityParameter.Name;
        BestQAPSolutionAnalyzer.BestKnownSolutionsParameter.ActualName = BestKnownSolutionsParameter.Name;
        BestQAPSolutionAnalyzer.MaximizationParameter.ActualName = MaximizationParameter.Name;
      }
      if (QAPAlleleFrequencyAnalyzer != null) {
        operators.Add(QAPAlleleFrequencyAnalyzer);
        QAPAlleleFrequencyAnalyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        QAPAlleleFrequencyAnalyzer.BestKnownSolutionParameter.ActualName = BestKnownSolutionParameter.Name;
        QAPAlleleFrequencyAnalyzer.DistancesParameter.ActualName = DistancesParameter.Name;
        QAPAlleleFrequencyAnalyzer.MaximizationParameter.ActualName = MaximizationParameter.Name;
        QAPAlleleFrequencyAnalyzer.WeightsParameter.ActualName = WeightsParameter.Name;
      }
      foreach (var localOpt in Operators.OfType<IQAPLocalImprovementOperator>()) {
        operators.Add(localOpt);
        localOpt.DistancesParameter.ActualName = DistancesParameter.Name;
        localOpt.MaximizationParameter.ActualName = MaximizationParameter.Name;
        localOpt.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        localOpt.WeightsParameter.ActualName = WeightsParameter.Name;
      }

      foreach (var moveOp in Operators.OfType<IQAPMoveEvaluator>()) {
        operators.Add(moveOp);
        moveOp.DistancesParameter.ActualName = DistancesParameter.Name;
        moveOp.WeightsParameter.ActualName = WeightsParameter.Name;
        moveOp.QualityParameter.ActualName = Evaluator.QualityParameter.Name;

        var swaMoveOp = moveOp as QAPSwap2MoveEvaluator;
        if (swaMoveOp != null) {
          var moveQualityName = swaMoveOp.MoveQualityParameter.ActualName;
          foreach (var o in Encoding.Operators.OfType<IPermutationSwap2MoveQualityOperator>())
            o.MoveQualityParameter.ActualName = moveQualityName;
        }
        var invMoveOp = moveOp as QAPInversionMoveEvaluator;
        if (invMoveOp != null) {
          var moveQualityName = invMoveOp.MoveQualityParameter.ActualName;
          foreach (var o in Encoding.Operators.OfType<IPermutationInversionMoveQualityOperator>())
            o.MoveQualityParameter.ActualName = moveQualityName;
        }
        var traMoveOp = moveOp as QAPTranslocationMoveEvaluator;
        if (traMoveOp != null) {
          var moveQualityName = traMoveOp.MoveQualityParameter.ActualName;
          foreach (var o in Encoding.Operators.OfType<IPermutationTranslocationMoveQualityOperator>())
            o.MoveQualityParameter.ActualName = moveQualityName;
        }
        var scrMoveOp = moveOp as QAPScrambleMoveEvaluator;
        if (scrMoveOp != null) {
          var moveQualityName = scrMoveOp.MoveQualityParameter.ActualName;
          foreach (var o in Encoding.Operators.OfType<IPermutationScrambleMoveQualityOperator>())
            o.MoveQualityParameter.ActualName = moveQualityName;
        }
      }
      foreach (var similarityCalculator in Operators.OfType<ISolutionSimilarityCalculator>()) {
        operators.Add(similarityCalculator);
        similarityCalculator.SolutionVariableName = Encoding.Name;
        similarityCalculator.QualityVariableName = Evaluator.QualityParameter.ActualName;
        var qapsimcalc = similarityCalculator as QAPSimilarityCalculator;
        if (qapsimcalc != null) {
          qapsimcalc.Weights = Weights;
          qapsimcalc.Distances = Distances;
        }
      }

      if (operators.Count > 0) Encoding.ConfigureOperators(operators);
    }

    private void UpdateParameterValues() {
      Permutation lbSolution;
      // calculate the optimum of a LAP relaxation and use it as lower bound of our QAP
      LowerBound = new DoubleValue(GilmoreLawlerBoundCalculator.CalculateLowerBound(Weights, Distances, out lbSolution));
      // evaluate the LAP optimal solution as if it was a QAP solution
      var lbSolutionQuality = Evaluate(lbSolution, CancellationToken.None);
      // in case both qualities are the same it means that the LAP optimum is also a QAP optimum
      if (LowerBound.Value.IsAlmost(lbSolutionQuality)) {
        BestKnownSolution = lbSolution;
        BestKnownQuality = LowerBound.Value;
      }
      AverageQuality = new DoubleValue(ComputeAverageQuality());
    }

    private double ComputeAverageQuality() {
      double rt = 0, rd = 0, wt = 0, wd = 0;
      int n = Weights.Rows;
      for (int i = 0; i < n; i++)
        for (int j = 0; j < n; j++) {
          if (i == j) {
            rd += Distances[i, i];
            wd += Weights[i, i];
          } else {
            rt += Distances[i, j];
            wt += Weights[i, j];
          }
        }

      return rt * wt / (n * (n - 1)) + rd * wd / n;
    }
    #endregion

    public void Load(QAPData data) {
      if (data.Dimension > ProblemSizeLimit) throw new System.IO.InvalidDataException("The problem is limited to instance of size " + ProblemSizeLimit + ". You can change this limit by modifying " + nameof(QuadraticAssignmentProblem) + "." + nameof(ProblemSizeLimit) + "!");
      var weights = new DoubleMatrix(data.Weights, @readonly: true);
      var distances = new DoubleMatrix(data.Distances, @readonly: true);
      Name = data.Name;
      Description = data.Description;
      Load(weights, distances);
      if (data.BestKnownQuality.HasValue) BestKnownQuality = data.BestKnownQuality.Value;
      EvaluateAndLoadAssignment(data.BestKnownAssignment);
      OnReset();
    }

    public void Load(TSPData data) {
      if (data.Dimension > ProblemSizeLimit) throw new System.IO.InvalidDataException("The problem is limited to instance of size " + ProblemSizeLimit + ". You can change this limit by modifying " + nameof(QuadraticAssignmentProblem) + "." + nameof(ProblemSizeLimit) + "!");
      var w = new double[data.Dimension, data.Dimension];
      for (int i = 0; i < data.Dimension; i++)
        w[i, (i + 1) % data.Dimension] = 1;
      var weights = new DoubleMatrix(w, @readonly: true);
      var distances = new DoubleMatrix(data.GetDistanceMatrix(), @readonly: true);
      Name = data.Name;
      Description = data.Description;
      Load(weights, distances);
      if (data.BestKnownQuality.HasValue) BestKnownQuality = data.BestKnownQuality.Value;
      EvaluateAndLoadAssignment(data.BestKnownTour);
      OnReset();
    }

    public void Load(DoubleMatrix weights, DoubleMatrix distances) {
      if (weights == null || weights.Rows == 0)
        throw new System.IO.InvalidDataException("The given instance does not contain weights!");
      if (weights.Rows != weights.Columns)
        throw new System.IO.InvalidDataException("The weights matrix is not a square matrix!");
      if (distances == null || distances.Rows == 0)
        throw new System.IO.InvalidDataException("The given instance does not contain distances!");
      if (distances.Rows != distances.Columns)
        throw new System.IO.InvalidDataException("The distances matrix is not a square matrix!");
      if (weights.Rows != distances.Columns)
        throw new System.IO.InvalidDataException("The weights matrix and the distance matrix are not of equal size!");

      Weights = weights;
      Distances = distances;
      Dimension = weights.Rows;

      BestKnownQualityParameter.Value = null;
      BestKnownSolution = null;
      BestKnownSolutions = null;
      UpdateParameterValues();
    }

    public void EvaluateAndLoadAssignment(int[] assignment) {
      if (assignment == null || assignment.Length == 0) return;
      var vector = new Permutation(PermutationTypes.Absolute, assignment);
      var result = Evaluate(vector, CancellationToken.None);
      BestKnownQuality = result;
      BestKnownSolution = vector;
      BestKnownSolutions = new ItemSet<Permutation> { (Permutation)vector.Clone() };
    }

    public QAPData Export() {
      return new QAPData() {
        Name = Name,
        Description = Description,
        Dimension = Weights.Rows,
        Weights = Weights.CloneAsMatrix(),
        Distances = Distances.CloneAsMatrix(),
        BestKnownAssignment = BestKnownSolution?.ToArray(),
        BestKnownQuality = !double.IsNaN(BestKnownQuality) ? BestKnownQuality : (double?)null
      };
    }
  }
}
