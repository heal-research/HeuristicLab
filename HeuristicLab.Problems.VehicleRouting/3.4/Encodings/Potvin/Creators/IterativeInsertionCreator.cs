#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("IterativeInsertionCreator", "Creates a randomly initialized VRP solution.")]
  [StorableClass]
  public sealed class IterativeInsertionCreator : PotvinCreator, IStochasticOperator {
    #region IStochasticOperator Members
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    #endregion

    public IValueParameter<BoolValue> AdhereTimeWindowsParameter {
      get { return (IValueParameter<BoolValue>)Parameters["AdhereTimeWindows"]; }
    }

    [StorableConstructor]
    private IterativeInsertionCreator(bool deserializing) : base(deserializing) { }

    public IterativeInsertionCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator."));
      Parameters.Add(new ValueParameter<BoolValue>("AdhereTimeWindows", "Specifies if the time windows should be considered during construction.", new BoolValue(true)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new IterativeInsertionCreator(this, cloner);
    }

    private IterativeInsertionCreator(IterativeInsertionCreator original, Cloner cloner)
      : base(original, cloner) {
    }

    private static double CalculateAngleToDepot(IVRPProblemInstance instance, int city) {
      double dx = instance.GetCoordinates(0)[0];
      double dy = instance.GetCoordinates(0)[1];

      double cx = instance.GetCoordinates(city)[0];
      double cy = instance.GetCoordinates(city)[1];

      double alpha = Math.Atan((cx - dx) / (dy - cy)) * (180.0 / Math.PI);
      if (cx > dx && cy > dy)
        alpha = (90.0 + alpha) + 90.0;
      else if (cx < dx && cy > dy)
        alpha = alpha + 180.0;
      else if (cx < dx && cy < dy)
        alpha = (90.0 + alpha) + 270.0;

      return alpha;
    }

    private static PotvinEncoding CreateSolution(IVRPProblemInstance instance, IRandom random, bool adhereTimeWindows) {
      PotvinEncoding result = new PotvinEncoding(instance);

      IPickupAndDeliveryProblemInstance pdp = instance as IPickupAndDeliveryProblemInstance;

      List<int> customers = new List<int>();
      for (int i = 1; i <= instance.Cities.Value; i++)
        if (pdp == null || pdp.GetDemand(i) >= 0)
          customers.Add(i);

      customers.Sort(delegate(int city1, int city2) {
            double angle1 = CalculateAngleToDepot(instance, city1);
            double angle2 = CalculateAngleToDepot(instance, city2);

            return angle1.CompareTo(angle2);
          });

      Tour currentTour = new Tour();
      result.Tours.Add(currentTour);

      int j = random.Next(customers.Count);
      for (int i = 0; i < customers.Count; i++) {
        int index = (i + j) % customers.Count;

        int stopIdx = 0;
        if (currentTour.Stops.Count > 0)
          result.FindBestInsertionPlace(currentTour, customers[index]);
        currentTour.Stops.Insert(stopIdx, customers[index]);

        if (pdp != null) {
          stopIdx = result.FindBestInsertionPlace(currentTour, pdp.GetPickupDeliveryLocation(customers[index]));
          currentTour.Stops.Insert(stopIdx, pdp.GetPickupDeliveryLocation(customers[index]));
        }

        CVRPEvaluation evaluation = instance.EvaluateTour(currentTour, result) as CVRPEvaluation;
        if (result.Tours.Count < instance.Vehicles.Value &&
          ((adhereTimeWindows && !instance.Feasible(evaluation)) || ((!adhereTimeWindows) && evaluation.Overload > double.Epsilon))) {
          currentTour.Stops.Remove(customers[index]);
          if (pdp != null)
            currentTour.Stops.Remove(pdp.GetPickupDeliveryLocation(customers[index]));

          if (currentTour.Stops.Count == 0)
            result.Tours.Remove(currentTour);
          currentTour = new Tour();
          result.Tours.Add(currentTour);

          currentTour.Stops.Add(customers[index]);
          if (pdp != null) {
            currentTour.Stops.Add(pdp.GetPickupDeliveryLocation(customers[index]));
          }
        }
      }

      if (currentTour.Stops.Count == 0)
        result.Tours.Remove(currentTour);

      return result;
    }

    public override IOperation Apply() {
      VRPToursParameter.ActualValue = CreateSolution(ProblemInstance, RandomParameter.ActualValue, AdhereTimeWindowsParameter.Value.Value);

      return base.Apply();
    }
  }
}
