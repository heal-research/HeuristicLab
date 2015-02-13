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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [Item("Single-objective MoveMaker", "Applies a move.")]
  [StorableClass]
  public class SingleObjectiveMoveMaker : InstrumentedOperator, IMoveMaker, ISingleObjectiveMoveOperator {
    public ILookupParameter<IEncoding> EncodingParameter {
      get { return (ILookupParameter<IEncoding>)Parameters["Encoding"]; }
    }

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }

    [StorableConstructor]
    protected SingleObjectiveMoveMaker(bool deserializing) : base(deserializing) { }
    protected SingleObjectiveMoveMaker(SingleObjectiveMoveMaker original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectiveMoveMaker() {
      Parameters.Add(new LookupParameter<IEncoding>("Encoding", "An item that holds the problem's encoding."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the parameter vector."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The quality of the move."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveMoveMaker(this, cloner);
    }

    public override IOperation InstrumentedApply() {
      if (MoveQualityParameter.ActualValue == null) throw new InvalidOperationException("Move has not been evaluated!");

      var encoding = EncodingParameter.ActualValue;
      var individual = encoding.GetIndividual(ExecutionContext.Scope);
      individual.CopyToScope(ExecutionContext.Scope.Parent.Parent);

      if (QualityParameter.ActualValue == null) QualityParameter.ActualValue = new DoubleValue(MoveQualityParameter.ActualValue.Value);
      else QualityParameter.ActualValue.Value = MoveQualityParameter.ActualValue.Value;

      return base.InstrumentedApply();
    }
  }
}
