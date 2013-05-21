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
  [Item("PrinsCrossover", "An operator which crosses two VRP representations.")]
  [StorableClass]
  public abstract class PrinsCrossover : VRPCrossover, IStochasticOperator {
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
    
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    
    [StorableConstructor]
    protected PrinsCrossover(bool deserializing) : base(deserializing) { }
    protected PrinsCrossover(PrinsCrossover original, Cloner cloner) : base(original, cloner) { }
    public PrinsCrossover()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("EvalFleetUsageFactor", "The fleet usage factor considered in the evaluation."));
      Parameters.Add(new LookupParameter<DoubleValue>("EvalTimeFactor", "The time factor considered in the evaluation."));
      Parameters.Add(new LookupParameter<DoubleValue>("EvalDistanceFactor", "The distance factor considered in the evaluation."));
      Parameters.Add(new LookupParameter<DoubleValue>("EvalOverloadPenalty", "The overload penalty considered in the evaluation."));
      Parameters.Add(new LookupParameter<DoubleValue>("EvalTardinessPenalty", "The tardiness penalty considered in the evaluation."));

      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
    }

    protected abstract PrinsEncoding Crossover(IRandom random, PrinsEncoding parent1, PrinsEncoding parent2);

    public override IOperation Apply() {
      ItemArray<IVRPEncoding> parents = new ItemArray<IVRPEncoding>(ParentsParameter.ActualValue.Length);
      for (int i = 0; i < ParentsParameter.ActualValue.Length; i++) {
        IVRPEncoding solution = ParentsParameter.ActualValue[i];

        if (!(solution is PrinsEncoding)) {
          parents[i] = PrinsEncoding.ConvertFrom(solution,
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
        } else {
          parents[i] = solution;
        }
      }
      ParentsParameter.ActualValue = parents;

      ChildParameter.ActualValue =
        Crossover(RandomParameter.ActualValue, parents[0] as PrinsEncoding, parents[1] as PrinsEncoding);

      return base.Apply();
    }
  }
}
