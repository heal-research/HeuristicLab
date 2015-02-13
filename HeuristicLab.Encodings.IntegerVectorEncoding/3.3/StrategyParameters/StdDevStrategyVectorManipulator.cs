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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  /// <summary>
  /// Mutates the endogenous strategy parameters.
  /// </summary>
  [Item("StdDevStrategyVectorManipulator", "Mutates the endogenous strategy parameters.")]
  [StorableClass]
  public class StdDevStrategyVectorManipulator : SingleSuccessorOperator, IStochasticOperator, IIntegerVectorStdDevStrategyParameterManipulator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<DoubleArray> StrategyParameterParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["StrategyParameter"]; }
    }
    public IValueLookupParameter<DoubleValue> GeneralLearningRateParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["GeneralLearningRate"]; }
    }
    public IValueLookupParameter<DoubleValue> LearningRateParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["LearningRate"]; }
    }
    public IValueLookupParameter<DoubleMatrix> BoundsParameter {
      get { return (IValueLookupParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }

    [StorableConstructor]
    protected StdDevStrategyVectorManipulator(bool deserializing) : base(deserializing) { }
    protected StdDevStrategyVectorManipulator(StdDevStrategyVectorManipulator original, Cloner cloner) : base(original, cloner) { }
    public StdDevStrategyVectorManipulator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new LookupParameter<DoubleArray>("StrategyParameter", "The strategy parameter to manipulate."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("GeneralLearningRate", "The general learning rate (tau0)."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("LearningRate", "The learning rate (tau)."));
      Parameters.Add(new ValueLookupParameter<DoubleMatrix>("Bounds", "A 2 column matrix specifying the lower and upper bound for each dimension. If there are less rows than dimension the bounds vector is cycled.", new DoubleMatrix(new double[,] { { 0, 5 } })));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StdDevStrategyVectorManipulator(this, cloner);
    }

    /// <summary>
    /// Mutates the endogenous strategy parameters.
    /// </summary>
    /// <param name="random">The random number generator to use.</param>
    /// <param name="vector">The strategy vector to manipulate.</param>
    /// <param name="generalLearningRate">The general learning rate dampens the mutation over all dimensions.</param>
    /// <param name="learningRate">The learning rate dampens the mutation in each dimension.</param>
    /// <param name="bounds">The minimal and maximal value for each component, bounds are cycled if the length of bounds is smaller than the length of vector</param>
    public static void Apply(IRandom random, DoubleArray vector, double generalLearningRate, double learningRate, DoubleMatrix bounds) {
      NormalDistributedRandom N = new NormalDistributedRandom(random, 0.0, 1.0);
      double generalMultiplier = Math.Exp(generalLearningRate * N.NextDouble());
      for (int i = 0; i < vector.Length; i++) {
        double change = vector[i] * generalMultiplier * Math.Exp(learningRate * N.NextDouble());
        if (bounds != null) {
          double min = bounds[i % bounds.Rows, 0], max = bounds[i % bounds.Rows, 1];
          if (min == max) vector[i] = min;
          else {
            if (change < min || change > max) change = Math.Max(min, Math.Min(max, change));
            vector[i] = change;
          }
        }
      }
    }
    /// <summary>
    /// Mutates the endogenous strategy parameters.
    /// </summary>
    /// <remarks>Calls <see cref="OperatorBase.Apply"/> of base class <see cref="OperatorBase"/>.</remarks>
    /// <inheritdoc select="returns"/>
    public override IOperation Apply() {
      var strategyParams = StrategyParameterParameter.ActualValue;
      if (strategyParams != null) { // only apply if there is a strategy vector
        IRandom random = RandomParameter.ActualValue;
        double tau0 = GeneralLearningRateParameter.ActualValue.Value;
        double tau = LearningRateParameter.ActualValue.Value;
        Apply(random, strategyParams, tau0, tau, BoundsParameter.ActualValue);
      }
      return base.Apply();
    }
  }
}
