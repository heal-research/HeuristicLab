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

namespace HeuristicLab.Problems.VehicleRouting {
  [Item("BestVRPToursMemorizer", "An operator that updates the best VRP tour found so far in the scope three.")]
  [StorableClass]
  public class BestVRPToursMemorizer : SingleSuccessorOperator {
    public ScopeTreeLookupParameter<DoubleValue> DistanceParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Distance"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> VehiclesUtilizedParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["VehiclesUtilized"]; }
    }

    public ValueLookupParameter<DoubleValue> BestDistanceParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestDistance"]; }
    }
    public ValueLookupParameter<DoubleValue> BestVehiclesUtilizedParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestVehiclesUtilized"]; }
    }

    public BestVRPToursMemorizer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Distance", "The distances of the VRP solutions which should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("VehiclesUtilized", "The utilized vehicles of the VRP solutions which should be analyzed."));

      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestDistance", "The best distance found so far."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestVehiclesUtilized", "The best vehicles utilized found so far."));

    }

    [StorableConstructor]
    protected BestVRPToursMemorizer(bool deserializing) : base(deserializing) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestVRPToursMemorizer(this, cloner);
    }

    protected BestVRPToursMemorizer(BestVRPToursMemorizer original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IOperation Apply() {
      int i = DistanceParameter.ActualValue.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      if (BestDistanceParameter.ActualValue == null)
        BestDistanceParameter.ActualValue = new DoubleValue(DistanceParameter.ActualValue[i].Value);
      else if (DistanceParameter.ActualValue[i].Value <= BestDistanceParameter.ActualValue.Value)
        BestDistanceParameter.ActualValue.Value = DistanceParameter.ActualValue[i].Value;

      i = VehiclesUtilizedParameter.ActualValue.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      if (BestVehiclesUtilizedParameter.ActualValue == null)
        BestVehiclesUtilizedParameter.ActualValue = new DoubleValue(VehiclesUtilizedParameter.ActualValue[i].Value);
      else if (VehiclesUtilizedParameter.ActualValue[i].Value <= BestVehiclesUtilizedParameter.ActualValue.Value)
        BestVehiclesUtilizedParameter.ActualValue.Value = VehiclesUtilizedParameter.ActualValue[i].Value;

      return base.Apply();
    }
  }
}
