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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting {
  [Item("BestCapacitatedVRPToursMemorizer", "An operator that updates the best VRP tour found so far in the scope three.")]
  [StorableClass]
  public class BestCapacitatedVRPToursMemorizer : SingleSuccessorOperator {
    public ScopeTreeLookupParameter<DoubleValue> OverloadParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Overload"]; }
    }

    public ValueLookupParameter<DoubleValue> BestOverloadParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestOverload"]; }
    }

    public BestCapacitatedVRPToursMemorizer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Overload", "The overloads of the VRP solutions which should be analyzed."));

      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestOverload", "The best overload found so far."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestCapacitatedVRPToursMemorizer(this, cloner);
    }

    protected BestCapacitatedVRPToursMemorizer(BestCapacitatedVRPToursMemorizer original, Cloner cloner)
      : base(original, cloner) {
    }

    [StorableConstructor]
    protected BestCapacitatedVRPToursMemorizer(bool deserializing) : base(deserializing) { }

    public override IOperation Apply() {
      int i = OverloadParameter.ActualValue.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      if (BestOverloadParameter.ActualValue == null)
        BestOverloadParameter.ActualValue = new DoubleValue(OverloadParameter.ActualValue[i].Value);
      else if (OverloadParameter.ActualValue[i].Value <= BestOverloadParameter.ActualValue.Value)
        BestOverloadParameter.ActualValue.Value = OverloadParameter.ActualValue[i].Value;

      return base.Apply();
    }
  }
}
