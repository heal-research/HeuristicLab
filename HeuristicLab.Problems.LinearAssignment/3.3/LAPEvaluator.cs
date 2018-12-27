using System;
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

using HeuristicLab.Operators;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Core;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Encodings.PermutationEncoding;

namespace HeuristicLab.Problems.LinearAssignment {
  [Item("LAPEvaluator", "Evaluates a solution to the linear assignment problem.")]
  [StorableClass]
  public class LAPEvaluator : InstrumentedOperator, ILAPEvaluator {

    public ILookupParameter<DoubleMatrix> CostsParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Costs"]; }
    }
    public ILookupParameter<Permutation> AssignmentParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Assignment"]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    [StorableConstructor]
    protected LAPEvaluator(bool deserializing) : base(deserializing) { }
    protected LAPEvaluator(LAPEvaluator original, Cloner cloner) : base(original, cloner) { }
    public LAPEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleMatrix>("Costs", LinearAssignmentProblem.CostsDescription));
      Parameters.Add(new LookupParameter<Permutation>("Assignment", "The assignment solution to evaluate."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality value of the solution."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LAPEvaluator(this, cloner);
    }

    public override IOperation InstrumentedApply() {
      var costs = CostsParameter.ActualValue;
      var assignment = AssignmentParameter.ActualValue;
      if (costs == null || assignment == null) throw new InvalidOperationException(Name + ": Cannot find Costs or Assignment.");

      int len = assignment.Length;
      double quality = 0;
      for (int i = 0; i < len; i++) {
        quality += costs[i, assignment[i]];
      }

      QualityParameter.ActualValue = new DoubleValue(quality);
      return base.InstrumentedApply();
    }
  }
}
