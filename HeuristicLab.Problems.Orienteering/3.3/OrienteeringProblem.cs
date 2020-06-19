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
using System.IO;
using System.Linq;
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.Instances.Types;
using HeuristicLab.Problems.TravelingSalesman;

namespace HeuristicLab.Problems.Orienteering {
  [Item("Orienteering Problem (OP)", "Represents a single-objective Orienteering Problem.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 115)]
  [StorableType("0B8DB4A4-F183-4368-86C6-C51289B183D2")]
  public sealed class OrienteeringProblem : IntegerVectorProblem,
      IProblemInstanceConsumer<OPData>, IProblemInstanceConsumer<TSPData>, IProblemInstanceConsumer<CVRPData> {

    [Storable] public ValueParameter<IOrienteeringProblemData> OrienteeringProblemDataParameter { get; private set; }
    [Storable] public OptionalValueParameter<OrienteeringSolution> BestKnownSolutionParameter { get; private set; }
    [Storable] private IResultParameter<OrienteeringSolution> BestOrienteeringSolutionParameter { get; set; }

    public IOrienteeringProblemData OrienteeringProblemData {
      get { return OrienteeringProblemDataParameter.Value; }
      set { OrienteeringProblemDataParameter.Value = value; }
    }
    public OrienteeringSolution BestKnownSolution {
      get { return BestKnownSolutionParameter.Value; }
      set { BestKnownSolutionParameter.Value = value; }
    }

    [StorableConstructor]
    private OrienteeringProblem(StorableConstructorFlag _) : base(_) {
    }
    private OrienteeringProblem(OrienteeringProblem original, Cloner cloner)
      : base(original, cloner) {
      OrienteeringProblemDataParameter = cloner.Clone(original.OrienteeringProblemDataParameter);
      BestKnownSolutionParameter = cloner.Clone(original.BestKnownSolutionParameter);
      BestOrienteeringSolutionParameter = cloner.Clone(original.BestOrienteeringSolutionParameter);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new OrienteeringProblem(this, cloner);
    }
    public OrienteeringProblem()
      : base(new IntegerVectorEncoding("Route")) {
      Parameters.Add(OrienteeringProblemDataParameter = new ValueParameter<IOrienteeringProblemData>("OP Data", "The main parameters for the orienteering problem.", new OrienteeringProblemData()));
      Parameters.Add(BestKnownSolutionParameter = new OptionalValueParameter<OrienteeringSolution>("BestKnownSolution", "The best known solution of this Orienteering instance."));
      Parameters.Add(BestOrienteeringSolutionParameter = new ResultParameter<OrienteeringSolution>("Best Orienteering Solution", "The best so far solution found."));
      Maximization = true;
      Dimension = OrienteeringProblemData.Cities;

      InitializeOperators();
    }

    public override ISingleObjectiveEvaluationResult Evaluate(IntegerVector solution, IRandom random, CancellationToken cancellationToken) {
      var data = OrienteeringProblemData;
      var score = CalculateScore(data, solution);
      var travelCosts = CalculateTravelCosts(data, solution);
      var quality = CalculateQuality(data, score, travelCosts);

      return new SingleObjectiveEvaluationResult(quality);
    }

    public static double CalculateQuality(IOrienteeringProblemData data, double score, double travelCosts) {
      if (travelCosts > data.MaximumTravelCosts) return data.MaximumTravelCosts - travelCosts; // negative excessive distance
      return score;
    }
    public static double CalculateScore(IOrienteeringProblemData data, IEnumerable<int> solution) {
      return solution.Sum(t => data.GetScore(t));
    }
    public static double CalculateTravelCosts(IOrienteeringProblemData data, IntegerVector solution) {
      var distance = data.GetPathDistance(solution, closed: false);
      distance += (solution.Length - 2) * data.PointVisitingCosts;
      return distance;
    }
    public static double CalculateTravelCosts(IOrienteeringProblemData data, IList<int> solution) {
      var distance = data.GetPathDistance(solution, closed: false);
      distance += (solution.Count - 2) * data.PointVisitingCosts;
      return distance;
    }

    public override void Analyze(IntegerVector[] vectors, double[] qualities, ResultCollection results, IRandom random) {
      base.Analyze(vectors, qualities, results, random);
      var data = OrienteeringProblemData;

      var best = GetBestSolution(vectors, qualities).Item1;
      var score = CalculateScore(OrienteeringProblemData, best);
      var travelCosts = CalculateTravelCosts(OrienteeringProblemData, best);
      var quality = CalculateQuality(OrienteeringProblemData, score, travelCosts);

      if (double.IsNaN(BestKnownQuality) || IsBetter(quality, BestKnownQuality)) {
        BestKnownQuality = quality;
        BestKnownSolutionParameter.ActualValue = data.GetSolution((IntegerVector)best.Clone(), quality, score, travelCosts);
      }
      var bestSoFar = BestOrienteeringSolutionParameter.ActualValue;

      if (bestSoFar == null || IsBetter(quality, bestSoFar.Quality.Value)) {
        bestSoFar = data.GetSolution((IntegerVector)best.Clone(), quality, score, travelCosts);
        BestOrienteeringSolutionParameter.ActualValue = bestSoFar;
      }
    }
    public static double CalculateInsertionCosts(IOrienteeringProblemData data, IList<int> path, int insertPosition, int point) {
      double detour = data.GetDistance(path[insertPosition - 1], point) + data.GetDistance(point, path[insertPosition]);
      detour += data.PointVisitingCosts;
      detour -= data.GetDistance(path[insertPosition - 1], path[insertPosition]);
      return detour;
    }
    public static double CalculateReplacementCosts(IOrienteeringProblemData data, IList<int> path, int replacePosition, int point) {
      double detour = data.GetDistance(path[replacePosition - 1], point) + data.GetDistance(point, path[replacePosition + 1]);
      detour -= data.GetDistance(path[replacePosition - 1], path[replacePosition]) + data.GetDistance(path[replacePosition], path[replacePosition + 1]);
      return detour;
    }
    public static double CalculateRemovementSaving(IOrienteeringProblemData data, IList<int> path, int removePosition) {
      double saving = data.GetDistance(path[removePosition - 1], path[removePosition]);
      saving += data.GetDistance(path[removePosition], path[removePosition + 1]);
      saving -= data.GetDistance(path[removePosition - 1], path[removePosition + 1]);
      saving += data.PointVisitingCosts;
      return saving;
    }

    private void RegisterEventHandlers() {
      OrienteeringProblemDataParameter.ValueChanged += OrienteeringProblemDataParameterOnValueChanged;
    }

    private void OrienteeringProblemDataParameterOnValueChanged(object sender, EventArgs e) {
      Dimension = OrienteeringProblemData.Cities;
    }

    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      ParameterizeOperators();
    }
    protected override void DimensionOnChanged() {
      base.DimensionOnChanged();
      if (Dimension != OrienteeringProblemData.Cities)
        Dimension = OrienteeringProblemData.Cities;
    }

    private void InitializeOperators() {
      ISolutionCreator creator;
      SolutionCreatorParameter.ValidValues.Add(creator = new GreedyOrienteeringTourCreator() {
        OrienteeringProblemDataParameter = { ActualName = OrienteeringProblemDataParameter.Name }
      });
      SolutionCreatorParameter.Value = creator;

      Operators.Add(new OrienteeringLocalImprovementOperator() {
        OrienteeringProblemDataParameter = { ActualName = OrienteeringProblemDataParameter.Name }
      });
      Operators.Add(new OrienteeringShakingOperator() {
        OrienteeringProblemDataParameter = { ActualName = OrienteeringProblemDataParameter.Name }
      });
      Operators.Add(new QualitySimilarityCalculator());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));

      ParameterizeOperators();
    }

    private void ParameterizeOperators() {
      foreach (var op in Operators.OfType<OrienteeringLocalImprovementOperator>()) {
        op.IntegerVectorParameter.ActualName = Encoding.Name;
        op.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
      }
      foreach (var op in Operators.OfType<OrienteeringShakingOperator>()) {
        op.IntegerVectorParameter.ActualName = Encoding.Name;
      }
      foreach (var similarityCalculator in Operators.OfType<ISolutionSimilarityCalculator>()) {
        similarityCalculator.SolutionVariableName = Encoding.Name;
        similarityCalculator.QualityVariableName = Evaluator.QualityParameter.ActualName;
      }
    }

    #region Instance consuming
    public void Load(OPData data) {
      if (data.Coordinates == null && data.Distances == null)
        throw new InvalidDataException("The given instance specifies no coordinates or distance matrix!");
      if (data.Coordinates != null && data.Coordinates.GetLength(1) != 2)
        throw new InvalidDataException("The coordinates of the given instance are not in the right format, there need to be one row for each customer and two columns for the x and y coordinates.");

      // Clear old solutions
      BestKnownSolution = null;

      Name = data.Name;
      Description = data.Description;

      var tsp = TSP.GetDataFromInstance(data);
      OrienteeringProblemData = new OrienteeringProblemData(tsp, data.StartingPoint, data.TerminalPoint, data.Scores, data.MaximumDistance, data.PointVisitingCosts);
    }

    public void Load(TSPData data) {
      if (data.Coordinates == null && data.Distances == null)
        throw new InvalidDataException("The given instance specifies no coordinates or distance matrix!");
      if (data.Coordinates != null && data.Coordinates.GetLength(1) != 2)
        throw new InvalidDataException("The coordinates of the given instance are not in the right format, there need to be one row for each customer and two columns for the x and y coordinates.");

      // Clear old solutions
      BestKnownSolution = null;

      Name = data.Name;
      Description = data.Description;


      var tsp = TSP.GetDataFromInstance(data);
      var avgDist = 0.0;
      for (var i = 0; i < data.Dimension - 1; i++)
        for (var j = i + 1; i < data.Dimension; j++)
          avgDist += tsp.GetDistance(i, j);
      avgDist /= (data.Dimension - 1) * data.Dimension / 2.0;

      OrienteeringProblemData = new OrienteeringProblemData(tsp, 0, data.Dimension - 1,
        Enumerable.Repeat(1.0, data.Dimension).ToArray(), 5 * avgDist, 0);
    }

    public void Load(CVRPData data) {
      if (data.Coordinates == null && data.Distances == null)
        throw new InvalidDataException("The given instance specifies no coordinates or distance matrix!");
      if (data.Coordinates != null && data.Coordinates.GetLength(1) != 2)
        throw new InvalidDataException("The coordinates of the given instance are not in the right format, there need to be one row for each customer and two columns for the x and y coordinates.");

      // Clear old solutions
      BestKnownSolution = null;

      Name = data.Name;
      Description = data.Description;

      var tsp = TSP.GetDataFromInstance(data);
      OrienteeringProblemData = new OrienteeringProblemData(tsp, 0, 0, data.Demands, data.Capacity * 2, 0);
    }
    #endregion
  }
}