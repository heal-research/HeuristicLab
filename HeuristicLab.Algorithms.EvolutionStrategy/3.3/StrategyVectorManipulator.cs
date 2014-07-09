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

using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Data;
using HeuristicLab.Random;
using System;

namespace HeuristicLab.Algorithms.EvolutionStrategy {
  /// <summary>
  /// Mutates the endogenous strategy parameters.
  /// </summary>
  public class StrategyVectorManipulator : SingleSuccessorOperator, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<RealVector> StrategyVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters["StrategyVector"]; }
    }
    public IValueLookupParameter<DoubleValue> GeneralLearningRateParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["GeneralLearningRate"]; }
    }
    public IValueLookupParameter<DoubleValue> LearningRateParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["LearningRate"]; }
    }
    /// <summary>
    /// Initializes a new instance of <see cref="StrategyVectorManipulator"/> with four 
    /// parameters (<c>Random</c>, <c>StrategyVector</c>, <c>GeneralLearningRate</c> and
    /// <c>LearningRate</c>).
    /// </summary>
    public StrategyVectorManipulator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new LookupParameter<RealVector>("StrategyVector", "The strategy vector to manipulate."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("GeneralLearningRate", "The general learning rate (tau0)."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("LearningRate", "The learning rate (tau)."));
    }

    /// <summary>
    /// Mutates the endogenous strategy parameters.
    /// </summary>
    /// <param name="random">The random number generator to use.</param>
    /// <param name="vector">The strategy vector to manipulate.</param>
    /// <param name="generalLearningRate">The general learning rate dampens the mutation over all dimensions.</param>
    /// <param name="learningRate">The learning rate dampens the mutation in each dimension.</param>
    public static void Apply(IRandom random, RealVector vector, double generalLearningRate, double learningRate) {
      NormalDistributedRandom N = new NormalDistributedRandom(random, 0.0, 1.0);
      double generalMultiplier = Math.Exp(generalLearningRate * N.NextDouble());
      for (int i = 0; i < vector.Length; i++) {
        vector[i] *= generalMultiplier * Math.Exp(learningRate * N.NextDouble());
      }
    }
    /// <summary>
    /// Mutates the endogenous strategy parameters.
    /// </summary>
    /// <remarks>Calls <see cref="OperatorBase.Apply"/> of base class <see cref="OperatorBase"/>.</remarks>
    /// <inheritdoc select="returns"/>
    public override IOperation Apply() {
      RealVector strategyParams = StrategyVectorParameter.ActualValue;
      if (strategyParams != null) { // only apply if there is a strategy vector
        IRandom random = RandomParameter.ActualValue;
        double tau0 = GeneralLearningRateParameter.ActualValue.Value;
        double tau = LearningRateParameter.ActualValue.Value;
        Apply(random, strategyParams, tau0, tau);
      }
      return base.Apply();
    }
  }
}
