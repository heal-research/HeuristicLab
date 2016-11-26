#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.LocalSearch;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.Binary.LocalSearch {
  [Item("Exhaustive Bitflip Local Search Operator (binary)", "")]
  [StorableClass]
  public class ExhaustiveBitflipOperator : SingleSuccessorOperator, ILocalSearch<BinaryVector>, ISingleObjectiveOperator, IStochasticOperator {

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    
    public ILookupParameter<BinaryVector> SolutionParameter {
      get { return (ILookupParameter<BinaryVector>)Parameters["Solution"]; }
    }

    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["Maximization"]; }
    }

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public ILookupParameter<BoolArray> SubspaceParameter {
      get { return (ILookupParameter<BoolArray>)Parameters["Subspace"]; }
    }

    public Func<BinaryVector, double> EvaluateFunc { get; set; }

    [StorableConstructor]
    protected ExhaustiveBitflipOperator(bool deserializing) : base(deserializing) { }
    protected ExhaustiveBitflipOperator(ExhaustiveBitflipOperator original, Cloner cloner) : base(original, cloner) { }
    public ExhaustiveBitflipOperator() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new LookupParameter<BinaryVector>("Solution", "The solution that is to be optimized"));
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "Whether the solution's quality should be maximized or minimized."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the solution."));
      Parameters.Add(new LookupParameter<BoolArray>("Subspace", "Whether the local search should be confined to a sub-space of the solution space (indicated by true in this vector)."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExhaustiveBitflipOperator(this, cloner);
    }

    public override IOperation Apply() {
      var quality = QualityParameter.ActualValue.Value;
      var subspace = SubspaceParameter.ActualValue != null ? SubspaceParameter.ActualValue.ToArray() : null;
      try {
        Heuristic.ExhaustiveBitFlipSearch(RandomParameter.ActualValue,
          SolutionParameter.ActualValue, ref quality, MaximizationParameter.ActualValue.Value,
          EvaluateFunc, CancellationToken, subspace);
      } finally {
        QualityParameter.ActualValue.Value = quality;
      }
      return base.Apply();
    }
  }
}
