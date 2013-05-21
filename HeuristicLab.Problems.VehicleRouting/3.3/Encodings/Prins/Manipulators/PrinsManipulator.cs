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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Prins {
  [Item("PrinsManipulator", "An operator which manipulates a VRP representation.")]
  [StorableClass]
  public abstract class PrinsManipulator : VRPManipulator, IStochasticOperator, IPrinsOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    public ILookupParameter<DoubleValue> FleetUsageFactor {
      get { return (ILookupParameter<DoubleValue>)Parameters["EvalFleetUsageFactor"]; }
    }
    public ILookupParameter<DoubleValue> TimeFactor {
      get { return (ILookupParameter<DoubleValue>)Parameters["EvalTimeFactor"]; }
    }
    public ILookupParameter<DoubleValue> DistanceFactor {
      get { return (ILookupParameter<DoubleValue>)Parameters["EvalDistanceFactor"]; }
    }
    public ILookupParameter<DoubleValue> OverloadPenalty {
      get { return (ILookupParameter<DoubleValue>)Parameters["EvalOverloadPenalty"]; }
    }
    public ILookupParameter<DoubleValue> TardinessPenalty {
      get { return (ILookupParameter<DoubleValue>)Parameters["EvalTardinessPenalty"]; }
    }

    [StorableConstructor]
    protected PrinsManipulator(bool deserializing) : base(deserializing) { }
    protected PrinsManipulator(PrinsManipulator original, Cloner cloner) : base(original, cloner) { }
    public PrinsManipulator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("EvalFleetUsageFactor", "The fleet usage factor considered in the evaluation."));
      Parameters.Add(new LookupParameter<DoubleValue>("EvalTimeFactor", "The time factor considered in the evaluation."));
      Parameters.Add(new LookupParameter<DoubleValue>("EvalDistanceFactor", "The distance factor considered in the evaluation."));
      Parameters.Add(new LookupParameter<DoubleValue>("EvalOverloadPenalty", "The overload penalty considered in the evaluation."));
      Parameters.Add(new LookupParameter<DoubleValue>("EvalTardinessPenalty", "The tardiness penalty considered in the evaluation."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
    }

    protected abstract void Manipulate(IRandom random, PrinsEncoding individual);

    public override IOperation Apply() {
      IVRPEncoding solution = VRPToursParameter.ActualValue;
      if (!(solution is PrinsEncoding)) {
        VRPToursParameter.ActualValue = PrinsEncoding.ConvertFrom(solution,
            Cities, 
            DueTimeParameter.ActualValue,
            ServiceTimeParameter.ActualValue,
            ReadyTimeParameter.ActualValue,
            DemandParameter.ActualValue,
            CapacityParameter.ActualValue,
            FleetUsageFactor.ActualValue, 
            TimeFactor.ActualValue, 
            DistanceFactor.ActualValue, 
            OverloadPenalty.ActualValue, 
            TardinessPenalty.ActualValue,
            CoordinatesParameter.ActualValue,
            DistanceMatrixParameter,
            UseDistanceMatrixParameter.ActualValue);
      }

      Manipulate(RandomParameter.ActualValue, VRPToursParameter.ActualValue as PrinsEncoding);

      return base.Apply();
    }
  }
}
