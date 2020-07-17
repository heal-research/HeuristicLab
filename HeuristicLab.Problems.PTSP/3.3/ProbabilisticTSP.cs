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
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.TravelingSalesman;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.PTSP {
  [Item("Probabilistic TSP (pTSP)", "Represents a Probabilistic Traveling Salesman Problem.")]
  [StorableType("86041a8c-14e6-46e1-b20f-566892c871f6")]
  public abstract class ProbabilisticTSP : PermutationProblem,
      IProblemInstanceConsumer<PTSPData> {
    protected bool SuppressEvents { get; set; }

    public static int DistanceMatrixSizeLimit = 1000;

    #region Parameter Properties
    [Storable] public ValueParameter<IProbabilisticTSPData> PTSPDataParameter { get; }
    [Storable] public OptionalValueParameter<IProbabilisticTSPSolution> BestKnownSolutionParameter { get; }
    #endregion

    #region Properties
    public IProbabilisticTSPData ProbabilisticTSPData {
      get { return PTSPDataParameter.Value; }
      set { PTSPDataParameter.Value = value; }
    }
    public IProbabilisticTSPSolution BestKnownSolution {
      get { return BestKnownSolutionParameter.Value; }
      set { BestKnownSolutionParameter.Value = value; }
    }
    #endregion

    [StorableConstructor]
    protected ProbabilisticTSP(StorableConstructorFlag _) : base(_) { }
    protected ProbabilisticTSP(ProbabilisticTSP original, Cloner cloner)
      : base(original, cloner) {
      PTSPDataParameter = cloner.Clone(original.PTSPDataParameter);
      BestKnownSolutionParameter = cloner.Clone(original.BestKnownSolutionParameter);
    }
    protected ProbabilisticTSP() : base(new PermutationEncoding("Tour")) {
      Maximization = false;
      Encoding.LengthParameter.ReadOnly = DimensionRefParameter.ReadOnly = true;
      Encoding.PermutationTypeParameter.ReadOnly = PermutationTypeRefParameter.ReadOnly = true;
      PermutationTypeRefParameter.Hidden = true;

      Parameters.Add(PTSPDataParameter = new ValueParameter<IProbabilisticTSPData>("PTSP Data", "The main parameters for the pTSP."));
      Parameters.Add(BestKnownSolutionParameter = new OptionalValueParameter<IProbabilisticTSPSolution>("BestKnownSolution", "The best known solution of this pTSP instance."));

      ProbabilisticTSPData = new ProbabilisticTSPData();
      Dimension = ProbabilisticTSPData.Cities;
    }

    public override void Analyze(Permutation[] solutions, double[] qualities, ResultCollection results, IRandom random) {
      base.Analyze(solutions, qualities, results, random);
      var max = Maximization;

      var i = !max ? qualities.Select((x, index) => new { index, Quality = x }).OrderBy(x => x.Quality).First().index
                   : qualities.Select((x, index) => new { index, Quality = x }).OrderByDescending(x => x.Quality).First().index;

      if (double.IsNaN(BestKnownQuality) ||
          max && qualities[i] > BestKnownQuality ||
          !max && qualities[i] < BestKnownQuality) {
        BestKnownQuality = qualities[i];
        BestKnownSolution = ProbabilisticTSPData.GetSolution((Permutation)solutions[i].Clone(), qualities[i]);
      }

      IResult bestSolutionResult;
      if (results.TryGetValue("Best pTSP Solution", out bestSolutionResult)) {
        var bestSolution = bestSolutionResult.Value as ITSPSolution;
        if (bestSolution == null || Maximization && bestSolution.TourLength.Value < qualities[i]
          || !Maximization && bestSolution.TourLength.Value > qualities[i]) {
          bestSolutionResult.Value = ProbabilisticTSPData.GetSolution(solutions[i], qualities[i]);
        }
      } else results.Add(new Result("Best pTSP Solution", ProbabilisticTSPData.GetSolution(solutions[i], qualities[i])));
    }

    public virtual void Load(PTSPData data) {
      if (data.Coordinates == null && data.Distances == null)
        throw new System.IO.InvalidDataException("The given instance specifies neither coordinates nor distances!");
      if (data.Dimension > DistanceMatrixSizeLimit && (data.DistanceMeasure == DistanceMeasure.Att
        || data.DistanceMeasure == DistanceMeasure.Manhattan
        || data.DistanceMeasure == DistanceMeasure.Maximum))
        throw new System.IO.InvalidDataException("The given instance uses an unsupported distance measure and is too large for using a distance matrix.");
      if (data.Coordinates != null && data.Coordinates.GetLength(1) != 2)
        throw new System.IO.InvalidDataException("The coordinates of the given instance are not in the right format, there need to be one row for each customer and two columns for the x and y coordinates.");

      Dimension = data.Dimension;
      Name = data.Name;
      Description = data.Description;

      var tspData = TSP.GetDataFromInstance(data);
      ProbabilisticTSPData = new ProbabilisticTSPData(tspData, data.Probabilities);
      BestKnownSolution = null;
      BestKnownQuality = double.NaN;

      if (data.BestKnownTour != null) {
        try {
          var tour = new Permutation(PermutationTypes.RelativeUndirected, data.BestKnownTour);
          var tourLength = Evaluate(tour, new MersenneTwister(1)).Quality;
          BestKnownSolution = ProbabilisticTSPData.GetSolution(tour, tourLength);
          BestKnownQuality = tourLength;
        } catch (InvalidOperationException) {
          if (data.BestKnownQuality.HasValue)
            BestKnownQuality = data.BestKnownQuality.Value;
        }
      } else if (data.BestKnownQuality.HasValue) {
        BestKnownQuality = data.BestKnownQuality.Value;
      }
      OnReset();
    }
  }
}
