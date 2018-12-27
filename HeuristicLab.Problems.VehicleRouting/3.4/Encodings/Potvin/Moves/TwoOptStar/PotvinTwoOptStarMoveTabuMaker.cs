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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinTwoOptStarMoveTabuMaker", "Declares a given two opt star move as tabu.")]
  [StorableClass]
  public class PotvinTwoOptStarMoveTabuMaker : SingleSuccessorOperator, ITabuMaker, IPotvinTwoOptStarMoveOperator, IPotvinOperator, ISingleObjectiveOperator {
    public LookupParameter<ItemList<IItem>> TabuListParameter {
      get { return (LookupParameter<ItemList<IItem>>)Parameters["TabuList"]; }
    }
    public ValueLookupParameter<IntValue> TabuTenureParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["TabuTenure"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }


    public ILookupParameter<PotvinTwoOptStarMove> TwoOptStarMoveParameter {
      get { return (ILookupParameter<PotvinTwoOptStarMove>)Parameters["PotvinTwoOptStarMove"]; }
    }
    public ILookupParameter VRPMoveParameter {
      get { return TwoOptStarMoveParameter; }
    }
    public ILookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }
    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (LookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }

    public ILookupParameter<DoubleValue> DistanceParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Distance"]; }
    }
    public ILookupParameter<DoubleValue> OverloadParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Overload"]; }
    }
    public ILookupParameter<DoubleValue> TardinessParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Tardiness"]; }
    }

    [StorableConstructor]
    protected PotvinTwoOptStarMoveTabuMaker(bool deserializing) : base(deserializing) { }
    protected PotvinTwoOptStarMoveTabuMaker(PotvinTwoOptStarMoveTabuMaker original, Cloner cloner) : base(original, cloner) { }
    public PotvinTwoOptStarMoveTabuMaker()
      : base() {
      Parameters.Add(new LookupParameter<ItemList<IItem>>("TabuList", "The tabu list where move attributes are stored."));
      Parameters.Add(new ValueLookupParameter<IntValue>("TabuTenure", "The tenure of the tabu list."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The quality of the move."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the solution."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, else if it is a minimization problem."));

      Parameters.Add(new LookupParameter<PotvinTwoOptStarMove>("PotvinTwoOptStarMove", "The moves that should be made."));
      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPTours", "The VRP tours considered in the move."));
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The VRP problem instance"));

      Parameters.Add(new LookupParameter<DoubleValue>("Distance", "The distance of the individual"));
      Parameters.Add(new LookupParameter<DoubleValue>("Overload", "The overload of the individual"));
      Parameters.Add(new LookupParameter<DoubleValue>("Tardiness", "The tardiness of the individual"));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinTwoOptStarMoveTabuMaker(this, cloner);
    }

    public override IOperation Apply() {
      ItemList<IItem> tabuList = TabuListParameter.ActualValue;
      int tabuTenure = TabuTenureParameter.ActualValue.Value;

      int overlength = tabuList.Count - tabuTenure;
      if (overlength >= 0) {
        for (int i = 0; i < tabuTenure - 1; i++)
          tabuList[i] = tabuList[i + overlength + 1];
        while (tabuList.Count >= tabuTenure)
          tabuList.RemoveAt(tabuList.Count - 1);
      }

      double distance = 0;
      if (DistanceParameter.ActualValue != null)
        distance = DistanceParameter.ActualValue.Value;

      double overload = 0;
      if (OverloadParameter.ActualValue != null)
        overload = OverloadParameter.ActualValue.Value;

      double tardiness = 0;
      if (TardinessParameter.ActualValue != null)
        tardiness = TardinessParameter.ActualValue.Value;

      PotvinTwoOptStarMove move = TwoOptStarMoveParameter.ActualValue;
      double moveQuality = MoveQualityParameter.ActualValue.Value;
      double quality = QualityParameter.ActualValue.Value;
      double baseQuality = moveQuality;
      if (quality < moveQuality) baseQuality = quality; // we make an uphill move, the lower bound is the solution quality

      List<int> segmentX1;
      List<int> segmentX2;
      PotvinTwoOptStarMoveMaker.GetSegments(move, out segmentX1, out segmentX2);

      foreach (int city in segmentX1) {
        tabuList.Add(new PotvinTwoOptStarMoveAttribute(baseQuality, move.Tour1, city, distance, overload, tardiness));
      }

      foreach (int city in segmentX2) {
        tabuList.Add(new PotvinTwoOptStarMoveAttribute(baseQuality, move.Tour2, city, distance, overload, tardiness));
      }

      return base.Apply();
    }
  }
}
