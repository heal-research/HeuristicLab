#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// Compares two qualities and creates a boolean flag that indicates if the left side is better than the right side or if the difference in quality is acceptable given a certain probability.
  /// </summary>
  /// <remarks>
  /// The probabilty is that of simulated annealing: A randomly drawn (uniformly) real value in
  /// the interval [0;1) has to be smaller than e^(-diff/T) where diff is the absolute quality difference
  /// and T is the temperature (here called dampening).
  /// </remarks>
  [Item("ProbabilisticQualityComparator", "Compares two qualities and creates a boolean flag that indicates if the left side is better than the right side or if the difference in quality is acceptable given a certain probability.")]
  [StorableType("E8467C96-D88F-4EFA-B3BF-78DBFA63B36A")]
  public class ProbabilisticQualityComparator : QualityComparator, IStochasticOperator {
    public ILookupParameter<DoubleValue> DampeningParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Dampening"]; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected ProbabilisticQualityComparator(StorableConstructorFlag _) : base(_) { }
    protected ProbabilisticQualityComparator(ProbabilisticQualityComparator original, Cloner cloner) : base(original, cloner) { }
    public ProbabilisticQualityComparator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Dampening", "The dampening factor that influences the probability."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ProbabilisticQualityComparator(this, cloner);
    }

    protected override bool Compare(bool maximization, double left, double right) {
      if (base.Compare(maximization, left, right)) return true;
      double t = DampeningParameter.ActualValue.Value;
      double probability = Math.Exp(-Math.Abs(left - right) / t);
      return RandomParameter.ActualValue.NextDouble() < probability;
    }
  }
}
