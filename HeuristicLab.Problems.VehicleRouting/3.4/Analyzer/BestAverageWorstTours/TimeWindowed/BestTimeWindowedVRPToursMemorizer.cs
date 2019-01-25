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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.VehicleRouting {
  [Item("BestTimeWindowedVRPToursMemorizer", "An operator that updates the best VRP tour found so far in the scope three.")]
  [StorableType("74E88AF0-67A4-41FC-8235-4B72253C95BE")]
  public class BestTimeWindowedVRPToursMemorizer : SingleSuccessorOperator {
    public ScopeTreeLookupParameter<DoubleValue> TardinessParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Tardiness"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> TravelTimeParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["TravelTime"]; }
    }

    public ValueLookupParameter<DoubleValue> BestTardinessParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestTardiness"]; }
    }
    public ValueLookupParameter<DoubleValue> BestTravelTimeParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestTravelTime"]; }
    }

    public BestTimeWindowedVRPToursMemorizer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Tardiness", "The tardiness of the VRP solutions which should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("TravelTime", "The travel times of the VRP solutions which should be analyzed."));

      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestTardiness", "The best tardiness found so far."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestTravelTime", "The best travel time found so far."));
    }

    public override IOperation Apply() {
      int i = TardinessParameter.ActualValue.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      if (BestTardinessParameter.ActualValue == null)
        BestTardinessParameter.ActualValue = new DoubleValue(TardinessParameter.ActualValue[i].Value);
      else if (TardinessParameter.ActualValue[i].Value <= BestTardinessParameter.ActualValue.Value)
        BestTardinessParameter.ActualValue.Value = TardinessParameter.ActualValue[i].Value;

      i = TravelTimeParameter.ActualValue.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      if (BestTravelTimeParameter.ActualValue == null)
        BestTravelTimeParameter.ActualValue = new DoubleValue(TravelTimeParameter.ActualValue[i].Value);
      else if (TravelTimeParameter.ActualValue[i].Value <= BestTravelTimeParameter.ActualValue.Value)
        BestTravelTimeParameter.ActualValue.Value = TravelTimeParameter.ActualValue[i].Value;

      return base.Apply();
    }

    [StorableConstructor]
    protected BestTimeWindowedVRPToursMemorizer(StorableConstructorFlag _) : base(_) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestTimeWindowedVRPToursMemorizer(this, cloner);
    }

    protected BestTimeWindowedVRPToursMemorizer(BestTimeWindowedVRPToursMemorizer original, Cloner cloner)
      : base(original, cloner) {
    }
  }
}
