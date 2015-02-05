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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Optimization;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;
using HeuristicLab.Common;
using HeuristicLab.Parameters;
using HeuristicLab.Operators;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaLambdaInterchangeLocalImprovementOperator", "Takes a solution and finds the local optimum with respect to the lambda interchange neighborhood by decending along the steepest gradient.")]
  [StorableClass]
  public class AlbaLambdaInterchangeLocalImprovementOperator : VRPOperator, IStochasticOperator, ILocalImprovementOperator {

    public IValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaximumIterations"]; }
    }

    public ILookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }

    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public ILookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public IValueParameter<IntValue> LambdaParameter {
      get { return (IValueParameter<IntValue>)Parameters["Lambda"]; }
    }

    public IValueParameter<IntValue> SampleSizeParameter {
      get { return (IValueParameter<IntValue>)Parameters["SampleSize"]; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected AlbaLambdaInterchangeLocalImprovementOperator(bool deserializing) : base(deserializing) { }
    protected AlbaLambdaInterchangeLocalImprovementOperator(AlbaLambdaInterchangeLocalImprovementOperator original, Cloner cloner)
      : base(original, cloner) {
    }
    public AlbaLambdaInterchangeLocalImprovementOperator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum amount of iterations that should be performed (note that this operator will abort earlier when a local optimum is reached.", new IntValue(10000)));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The amount of evaluated solutions (here a move is counted only as 4/n evaluated solutions with n being the length of the permutation)."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "The collection where to store results."));
      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPTours", "The VRP tours to be manipulated."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality value of the assignment."));
      Parameters.Add(new ValueParameter<IntValue>("Lambda", "The lambda value.", new IntValue(1)));
      Parameters.Add(new ValueParameter<IntValue>("SampleSize", "The number of moves to generate.", new IntValue(2000)));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaLambdaInterchangeLocalImprovementOperator(this, cloner);
    }

    public static void Apply(AlbaEncoding solution, int maxIterations,
      int lambda, int samples, IRandom random, IVRPProblemInstance problemInstance, ref double quality, out int evaluatedSolutions) {
      evaluatedSolutions = 0;

      for (int i = 0; i < maxIterations; i++) {
        AlbaLambdaInterchangeMove bestMove = null;
        foreach (AlbaLambdaInterchangeMove move in AlbaStochasticLambdaInterchangeMultiMoveGenerator.GenerateAllMoves(solution, problemInstance, lambda, samples, random)) {
          AlbaEncoding newSolution = solution.Clone() as AlbaEncoding;
          AlbaLambdaInterchangeMoveMaker.Apply(newSolution, move);
          double moveQuality =
            problemInstance.Evaluate(newSolution).Quality;

          evaluatedSolutions++;
          if (moveQuality < quality || quality == -1) {
            quality = moveQuality;
            bestMove = move;
          }
        }
        if (bestMove != null)
          AlbaLambdaInterchangeMoveMaker.Apply(solution, bestMove);
      }
    }

    public override IOperation InstrumentedApply() {
      int maxIterations = MaximumIterationsParameter.ActualValue.Value;
      AlbaEncoding solution = null;

      if (VRPToursParameter.ActualValue is AlbaEncoding)
        solution = VRPToursParameter.ActualValue as AlbaEncoding;
      else
        VRPToursParameter.ActualValue = solution = AlbaEncoding.ConvertFrom(VRPToursParameter.ActualValue, ProblemInstance);

      int lambda = LambdaParameter.Value.Value;
      int samples = SampleSizeParameter.Value.Value;
      IRandom random = RandomParameter.ActualValue;

      double quality = QualityParameter.ActualValue.Value;
      int evaluatedSolutions;

      Apply(solution, maxIterations, lambda, samples, random, ProblemInstance, ref quality, out evaluatedSolutions);

      EvaluatedSolutionsParameter.ActualValue.Value += evaluatedSolutions;
      QualityParameter.ActualValue.Value = quality;

      return base.InstrumentedApply();
    }
  }
}
