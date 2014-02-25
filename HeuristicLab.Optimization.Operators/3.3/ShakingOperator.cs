#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// A shaking operator for VNS.
  /// </summary>
  [Item("ShakingOperator", "A shaking operator for VNS that can be filled with arbitrary manipulation operators.")]
  [StorableClass]
  public class ShakingOperator<T> : CheckedMultiOperator<T>, IMultiNeighborhoodShakingOperator where T : class, IManipulator {

    public IValueLookupParameter<IntValue> CurrentNeighborhoodIndexParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["CurrentNeighborhoodIndex"]; }
    }

    public ILookupParameter<IntValue> NeighborhoodCountParameter {
      get { return (ILookupParameter<IntValue>)Parameters["NeighborhoodCount"]; }
    }

    [StorableConstructor]
    protected ShakingOperator(bool deserializing) : base(deserializing) { }
    protected ShakingOperator(ShakingOperator<T> original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ShakingOperator<T>(this, cloner);
    }
    public ShakingOperator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>("CurrentNeighborhoodIndex", "The index of the operator that should be applied (k)."));
      Parameters.Add(new LookupParameter<IntValue>("NeighborhoodCount", "The number of operators that are available."));
    }

    public override IOperation InstrumentedApply() {
      if (NeighborhoodCountParameter.ActualValue == null)
        NeighborhoodCountParameter.ActualValue = new IntValue(Operators.CheckedItems.Count());
      else NeighborhoodCountParameter.ActualValue.Value = Operators.CheckedItems.Count();

      int index = CurrentNeighborhoodIndexParameter.ActualValue.Value;
      var shaker = base.Operators.CheckedItems.SingleOrDefault(x => x.Index == index);

      OperationCollection next = new OperationCollection(base.InstrumentedApply());
      if (shaker.Value != null)
        next.Insert(0, ExecutionContext.CreateChildOperation(shaker.Value));

      return next;
    }
  }
}
