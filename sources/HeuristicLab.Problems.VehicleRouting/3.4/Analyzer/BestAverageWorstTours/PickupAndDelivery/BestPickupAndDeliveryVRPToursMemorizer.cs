#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.VehicleRouting {
  [Item("BestPickupAndDeliveryVRPToursMemorizer", "An operator that updates the best VRP tour found so far in the scope three.")]
  [StorableClass]
  public class BestPickupAndDeliveryVRPToursMemorizer : SingleSuccessorOperator {
    public ScopeTreeLookupParameter<IntValue> PickupViolationsParameter {
      get { return (ScopeTreeLookupParameter<IntValue>)Parameters["PickupViolations"]; }
    }

    public ValueLookupParameter<IntValue> BestPickupViolationsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["BestPickupViolations"]; }
    }

    public BestPickupAndDeliveryVRPToursMemorizer()
      : base() {
        Parameters.Add(new ScopeTreeLookupParameter<IntValue>("PickupViolations", "The pickup violations of the VRP solutions which should be analyzed."));

        Parameters.Add(new ValueLookupParameter<IntValue>("BestPickupViolations", "The best pickup violations found so far."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestPickupAndDeliveryVRPToursMemorizer(this, cloner);
    }

    protected BestPickupAndDeliveryVRPToursMemorizer(BestPickupAndDeliveryVRPToursMemorizer original, Cloner cloner)
      : base(original, cloner) {
    }

    [StorableConstructor]
    protected BestPickupAndDeliveryVRPToursMemorizer(bool deserializing) : base(deserializing) { }

    public override IOperation Apply() {
      int i = PickupViolationsParameter.ActualValue.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      if (BestPickupViolationsParameter.ActualValue == null)
        BestPickupViolationsParameter.ActualValue = new IntValue(PickupViolationsParameter.ActualValue[i].Value);
      else if (PickupViolationsParameter.ActualValue[i].Value <= BestPickupViolationsParameter.ActualValue.Value)
        BestPickupViolationsParameter.ActualValue.Value = PickupViolationsParameter.ActualValue[i].Value;

      return base.Apply();
    }
  }
}
