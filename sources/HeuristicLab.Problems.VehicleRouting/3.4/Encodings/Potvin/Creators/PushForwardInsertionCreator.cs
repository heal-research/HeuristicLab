#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("PushForwardInsertionCreator", "The push forward insertion heuristic. It is implemented as described in Sam, and Thangiah, R. (1999). A Hybrid Genetic Algorithms, Simulated Annealing and Tabu Search Heuristic for Vehicle Routing Problems with Time Windows. Practical Handbook of Genetic Algorithms, Volume III, pp 347–381.")]
  [StorableClass]
  public sealed class PushForwardInsertionCreator : PotvinCreator, IStochasticOperator {
    #region IStochasticOperator Members
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    #endregion

    public IValueParameter<DoubleValue> Alpha {
      get { return (IValueParameter<DoubleValue>)Parameters["Alpha"]; }
    }
    public IValueParameter<DoubleValue> AlphaVariance {
      get { return (IValueParameter<DoubleValue>)Parameters["AlphaVariance"]; }
    }
    public IValueParameter<DoubleValue> Beta {
      get { return (IValueParameter<DoubleValue>)Parameters["Beta"]; }
    }
    public IValueParameter<DoubleValue> BetaVariance {
      get { return (IValueParameter<DoubleValue>)Parameters["BetaVariance"]; }
    }
    public IValueParameter<DoubleValue> Gamma {
      get { return (IValueParameter<DoubleValue>)Parameters["Gamma"]; }
    }
    public IValueParameter<DoubleValue> GammaVariance {
      get { return (IValueParameter<DoubleValue>)Parameters["GammaVariance"]; }
    }

    [StorableConstructor]
    private PushForwardInsertionCreator(bool deserializing) : base(deserializing) { }

    public PushForwardInsertionCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator."));
      Parameters.Add(new ValueParameter<DoubleValue>("Alpha", "The alpha value.", new DoubleValue(0.7)));
      Parameters.Add(new ValueParameter<DoubleValue>("AlphaVariance", "The alpha variance.", new DoubleValue(0.5)));
      Parameters.Add(new ValueParameter<DoubleValue>("Beta", "The beta value.", new DoubleValue(0.1)));
      Parameters.Add(new ValueParameter<DoubleValue>("BetaVariance", "The beta variance.", new DoubleValue(0.07)));
      Parameters.Add(new ValueParameter<DoubleValue>("Gamma", "The gamma value.", new DoubleValue(0.2)));
      Parameters.Add(new ValueParameter<DoubleValue>("GammaVariance", "The gamma variance.", new DoubleValue(0.14)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PushForwardInsertionCreator(this, cloner);
    }

    private PushForwardInsertionCreator(PushForwardInsertionCreator original, Cloner cloner)
      : base(original, cloner) {
    }

    // use the Box-Mueller transform in the polar form to generate a N(0,1) random variable out of two uniformly distributed random variables
    private static double Gauss(IRandom random) {
      double u = 0.0, v = 0.0, s = 0.0;
      do {
        u = (random.NextDouble() * 2) - 1;
        v = (random.NextDouble() * 2) - 1;
        s = Math.Sqrt(u * u + v * v);
      } while (s < Double.Epsilon || s > 1);
      return u * Math.Sqrt((-2.0 * Math.Log(s)) / s);
    }

    private static double N(double mu, double sigma, IRandom random) {
      return mu + (sigma * Gauss(random)); // transform the random variable sampled from N(0,1) to N(mu,sigma)
    }

    private static double GetDistance(int start, int end, IVRPProblemInstance problemInstance) {
      double distance = 0.0;

      double startX = problemInstance.Coordinates[start, 0];
      double startY = problemInstance.Coordinates[start, 1];

      double endX = problemInstance.Coordinates[end, 0];
      double endY = problemInstance.Coordinates[end, 1];

      distance =
          Math.Sqrt(
            Math.Pow(startX - endX, 2) +
            Math.Pow(startY - endY, 2));

      return distance;
    }

    private static int GetNearestDepot(IVRPProblemInstance problemInstance, List<int> depots, int customer,
      double alpha, double beta, double gamma, out double minCost) {
      int nearest = -1;
      minCost = double.MaxValue;

      int depotCount = 1;
      IMultiDepotProblemInstance mdp = problemInstance as IMultiDepotProblemInstance;
      if (mdp != null) {
        depotCount = mdp.Depots.Value;
      }

      foreach (int depot in depots) {
        double x0 = problemInstance.Coordinates[depot, 0];
        double y0 = problemInstance.Coordinates[depot, 1];

        double distance = GetDistance(customer + depotCount - 1, depot, problemInstance);

        double dueTime = 0;
        if (problemInstance is ITimeWindowedProblemInstance)
          dueTime = (problemInstance as ITimeWindowedProblemInstance).DueTime[customer + depotCount - 1];
        if (dueTime == double.MaxValue)
          dueTime = 0;

        double x = problemInstance.Coordinates[customer + depotCount - 1, 0];
        double y = problemInstance.Coordinates[customer + depotCount - 1, 1];

        double cost = alpha * distance + // distance 0 <-> City[i]
                      -beta * dueTime + // latest arrival time
                      -gamma * ((Math.Atan2(y - y0, x - x0) + Math.PI) / (2.0 * Math.PI) * distance); // polar angle

        if (cost < minCost) {
          minCost = cost;
          nearest = depot;
        }
      }

      return nearest;
    }

    private static List<int> SortCustomers(IVRPProblemInstance problemInstance, List<int> customers, List<int> depots,
      Dictionary<int, int> depotAssignment,
      double alpha, double beta, double gamma) {
      List<int> sortedCustomers = new List<int>();
      depotAssignment.Clear();

      List<double> costList = new List<double>();

      for (int i = 0; i < customers.Count; i++) {
        double cost;
        int depot = GetNearestDepot(problemInstance, depots, customers[i],
          alpha, beta, gamma,
          out cost);
        depotAssignment[customers[i]] = depot;

        int index = 0;
        while (index < costList.Count && costList[index] < cost) index++;
        costList.Insert(index, cost);
        sortedCustomers.Insert(index, customers[i]);
      }

      return sortedCustomers;
    }

    private static bool RemoveUnusedDepots(List<int> depots, Dictionary<int, List<int>> vehicles) {
      List<int> toBeRemoved = new List<int>();

      foreach (int depot in depots) {
        if (vehicles[depot].Count == 0)
          toBeRemoved.Add(depot);
      }

      foreach (int depot in toBeRemoved) {
        depots.Remove(depot);
        vehicles.Remove(depot);
      }

      return toBeRemoved.Count > 0;
    }

    public static PotvinEncoding CreateSolution(IVRPProblemInstance problemInstance, IRandom random,
      double alphaValue = 0.7, double betaValue = 0.1, double gammaValue = 0.2,
      double alphaVariance = 0.5, double betaVariance = 0.07, double gammaVariance = 0.14) {
      PotvinEncoding result = new PotvinEncoding(problemInstance);

      IPickupAndDeliveryProblemInstance pdp = problemInstance as IPickupAndDeliveryProblemInstance;
      IMultiDepotProblemInstance mdp = problemInstance as IMultiDepotProblemInstance;

      double alpha, beta, gamma;
      alpha = N(alphaValue, Math.Sqrt(alphaVariance), random);
      beta = N(betaValue, Math.Sqrt(betaVariance), random);
      gamma = N(gammaValue, Math.Sqrt(gammaVariance), random);

      List<int> unroutedCustomers = new List<int>();
      for (int i = 1; i <= problemInstance.Cities.Value; i++) {
        if (pdp == null || (problemInstance.GetDemand(i) >= 0))
          unroutedCustomers.Add(i);
      }

      List<int> depots = new List<int>();
      if (mdp != null) {
        for (int i = 0; i < mdp.Depots.Value; i++) {
          depots.Add(i);
        }
      } else {
        depots.Add(0);
      }

      Dictionary<int, List<int>> vehicles = new Dictionary<int, List<int>>();
      foreach (int depot in depots) {
        vehicles[depot] = new List<int>();

        int vehicleCount = problemInstance.Vehicles.Value;
        if (mdp != null) {
          for (int vehicle = 0; vehicle < mdp.VehicleDepotAssignment.Length; vehicle++) {
            if (mdp.VehicleDepotAssignment[vehicle] == depot) {
              vehicles[depot].Add(vehicle);
            }
          }
        } else {
          for (int vehicle = 0; vehicle < vehicleCount; vehicle++) {
            vehicles[depot].Add(vehicle);
          }
        }
      }

      RemoveUnusedDepots(depots, vehicles);
      Dictionary<int, int> depotAssignment = new Dictionary<int, int>();

      unroutedCustomers = SortCustomers(
        problemInstance, unroutedCustomers, depots, depotAssignment,
        alpha, beta, gamma);

      /////////
      Tour tour = new Tour();
      result.Tours.Add(tour);
      int currentCustomer = unroutedCustomers[0];
      unroutedCustomers.RemoveAt(0);

      int currentDepot = depotAssignment[currentCustomer];
      int currentVehicle = vehicles[currentDepot][0];
      vehicles[currentDepot].RemoveAt(0);
      if (RemoveUnusedDepots(depots, vehicles)) {
        unroutedCustomers = SortCustomers(
        problemInstance, unroutedCustomers, depots, depotAssignment,
        alpha, beta, gamma);
      }

      result.VehicleAssignment[result.Tours.Count - 1] = currentVehicle;

      tour.Stops.Add(currentCustomer);
      if (pdp != null) {
        tour.Stops.Add(pdp.GetPickupDeliveryLocation(currentCustomer));
      }
      ////////

      while (unroutedCustomers.Count > 0) {
        double minimumCost = double.MaxValue;
        int customer = -1;
        int indexOfMinimumCost = -1;
        int indexOfMinimumCost2 = -1;

        foreach (int unrouted in unroutedCustomers) {
          VRPEvaluation eval = problemInstance.EvaluateTour(tour, result);
          double originalCosts = eval.Quality;

          for (int i = 0; i <= tour.Stops.Count; i++) {
            tour.Stops.Insert(i, unrouted);
            eval = problemInstance.EvaluateTour(tour, result);
            double tourCost = eval.Quality - originalCosts;

            if (pdp != null) {
              for (int j = i + 1; j <= tour.Stops.Count; j++) {
                bool feasible;
                double cost = tourCost +
                  problemInstance.GetInsertionCosts(eval, result, pdp.GetPickupDeliveryLocation(unrouted), 0, j, out feasible);
                if (cost < minimumCost && feasible) {
                  customer = unrouted;
                  minimumCost = cost;
                  indexOfMinimumCost = i;
                  indexOfMinimumCost2 = j;
                }
              }
            } else {
              double cost = tourCost;
              bool feasible = problemInstance.Feasible(eval);
              if (cost < minimumCost && feasible) {
                customer = unrouted;
                minimumCost = cost;
                indexOfMinimumCost = i;
              }
            }

            tour.Stops.RemoveAt(i);
          }
        }

        if (indexOfMinimumCost == -1 && vehicles.Count == 0) {
          indexOfMinimumCost = tour.Stops.Count;
          indexOfMinimumCost2 = tour.Stops.Count + 1;
          customer = unroutedCustomers[0];
        }

        // insert customer if found
        if (indexOfMinimumCost != -1) {
          tour.Stops.Insert(indexOfMinimumCost, customer);
          if (pdp != null) {
            tour.Stops.Insert(indexOfMinimumCost2, pdp.GetPickupDeliveryLocation(customer));
          }

          unroutedCustomers.Remove(customer);
        } else { // no feasible customer found
          tour = new Tour();
          result.Tours.Add(tour);
          currentCustomer = unroutedCustomers[0];
          unroutedCustomers.RemoveAt(0);

          currentDepot = depotAssignment[currentCustomer];
          currentVehicle = vehicles[currentDepot][0];
          vehicles[currentDepot].RemoveAt(0);
          if (RemoveUnusedDepots(depots, vehicles)) {
            unroutedCustomers = SortCustomers(
            problemInstance, unroutedCustomers, depots, depotAssignment,
            alpha, beta, gamma);
          }

          result.VehicleAssignment[result.Tours.Count - 1] = currentVehicle;

          tour.Stops.Add(currentCustomer);
          if (pdp != null) {
            tour.Stops.Add(pdp.GetPickupDeliveryLocation(currentCustomer));
          }
        }
      }

      if (mdp != null) {
        List<int> availableVehicles = new List<int>();
        for (int i = 0; i < mdp.Vehicles.Value; i++)
          availableVehicles.Add(i);

        for (int i = 0; i < result.VehicleAssignment.Length; i++) {
          if (result.VehicleAssignment[i] != -1)
            availableVehicles.Remove(result.VehicleAssignment[i]);
        }

        for (int i = 0; i < result.VehicleAssignment.Length; i++) {
          if (result.VehicleAssignment[i] == -1) {
            result.VehicleAssignment[i] = availableVehicles[0];
            availableVehicles.RemoveAt(0);
          }
        }
      }

      return result;
    }

    public override IOperation InstrumentedApply() {
      VRPToursParameter.ActualValue = CreateSolution(ProblemInstance, RandomParameter.ActualValue,
        Alpha.Value.Value, Beta.Value.Value, Gamma.Value.Value,
        AlphaVariance.Value.Value, BetaVariance.Value.Value, GammaVariance.Value.Value);

      return base.InstrumentedApply();
    }
  }
}
