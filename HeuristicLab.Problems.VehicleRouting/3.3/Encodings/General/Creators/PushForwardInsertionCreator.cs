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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  [Item("PushForwardCreator", "The push forward insertion heuristic.  It is implemented as described in Sam, and Thangiah, R. (1999). A Hybrid Genetic Algorithms, Simulated Annealing and Tabu Search Heuristic for Vehicle Routing Problems with Time Windows. Practical Handbook of Genetic Algorithms, Volume III, pp 347–381.")]
  [StorableClass]
  public sealed class PushForwardCreator : DefaultRepresentationCreator, IStochasticOperator {
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
    private PushForwardCreator(bool deserializing) : base(deserializing) { }
    private PushForwardCreator(PushForwardCreator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PushForwardCreator(this, cloner);
    }

    public PushForwardCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator."));
      Parameters.Add(new ValueParameter<DoubleValue>("Alpha", "The alpha value.", new DoubleValue(0.7)));
      Parameters.Add(new ValueParameter<DoubleValue>("AlphaVariance", "The alpha variance.", new DoubleValue(0.5)));
      Parameters.Add(new ValueParameter<DoubleValue>("Beta", "The beta value.", new DoubleValue(0.1)));
      Parameters.Add(new ValueParameter<DoubleValue>("BetaVariance", "The beta variance.", new DoubleValue(0.07)));
      Parameters.Add(new ValueParameter<DoubleValue>("Gamma", "The gamma value.", new DoubleValue(0.2)));
      Parameters.Add(new ValueParameter<DoubleValue>("GammaVariance", "The gamma variance.", new DoubleValue(0.14)));
    }

    // use the Box-Mueller transform in the polar form to generate a N(0,1) random variable out of two uniformly distributed random variables
    private double Gauss(IRandom random) {
      double u = 0.0, v = 0.0, s = 0.0;
      do {
        u = (random.NextDouble() * 2) - 1;
        v = (random.NextDouble() * 2) - 1;
        s = Math.Sqrt(u * u + v * v);
      } while (s < Double.Epsilon || s > 1);
      return u * Math.Sqrt((-2.0 * Math.Log(s)) / s);
    }

    private double N(double mu, double sigma, IRandom random) {
      return mu + (sigma * Gauss(random)); // transform the random variable sampled from N(0,1) to N(mu,sigma)
    }

    private double CalculateDistance(int start, int end, DoubleMatrix coordinates) {
      double distance = 0.0;

      distance =
          Math.Sqrt(
            Math.Pow(coordinates[start, 0] - coordinates[end, 0], 2) +
            Math.Pow(coordinates[start, 1] - coordinates[end, 1], 2));

      return distance;
    }

    private DoubleMatrix CreateDistanceMatrix(DoubleMatrix coordinates) {
      DoubleMatrix distanceMatrix = new DoubleMatrix(coordinates.Rows, coordinates.Rows);

      for (int i = 0; i < distanceMatrix.Rows; i++) {
        for (int j = i; j < distanceMatrix.Columns; j++) {
          double distance = CalculateDistance(i, j, coordinates);

          distanceMatrix[i, j] = distance;
          distanceMatrix[j, i] = distance;
        }
      }

      return distanceMatrix;
    }

    private double Distance(int start, int end, DoubleMatrix coordinates, bool useDistanceMatrix) {
      double distance = 0.0;

      if (useDistanceMatrix) {
        distance = coordinates[start, end];
      } else {  
        distance = CalculateDistance(start, end, coordinates);
      }

      return distance;
    }

    private double TravelDistance(List<int> route, int begin, DoubleMatrix coordinates, bool useDistanceMatrix) {
      double distance = 0;
      for (int i = begin; i < route.Count - 1 && (i == begin || route[i] != 0); i++) {
        distance += Distance(route[i], route[i + 1], coordinates, useDistanceMatrix);
      }
      return distance;
    }

    private bool SubrouteConstraintsOK(List<int> route, int begin, DoubleMatrix coordinates, bool useDistanceMatrix, 
      DoubleArray dueTime, DoubleArray readyTime, DoubleArray serviceTime, DoubleArray demand, DoubleValue capacity) {
      double t = 0.0, o = 0.0;
      for (int i = begin + 1; i < route.Count; i++) {
        t += Distance(route[i - 1], route[i], coordinates, useDistanceMatrix);
        if (route[i] == 0) return (t < DueTimeParameter.ActualValue[0]); // violation on capacity constraint is handled below
        else {
          if (t > dueTime[route[i]]) return false;
          t = Math.Max(readyTime[route[i]], t);
          t += serviceTime[route[i]];
          o += demand[route[i]];
          if (o > capacity.Value) return false; // premature exit on capacity constraint violation
        }
      }
      return true;
    }

    private bool SubrouteTardinessOK(List<int> route, int begin, DoubleMatrix coordinates, bool useDistanceMatrix, 
      DoubleArray dueTime, DoubleArray readyTime, DoubleArray serviceTime) {
      double t = 0.0;
      for (int i = begin + 1; i < route.Count; i++) {
        t += Distance(route[i - 1], route[i], coordinates, useDistanceMatrix);
        if (route[i] == 0) {
          if (t < dueTime[0]) return true;
          else return false;
        } else {
          if (t > dueTime[route[i]]) return false;
          t = Math.Max(readyTime[route[i]], t);
          t += serviceTime[route[i]];
        }
      }
      return true;
    }

    private bool SubrouteLoadOK(List<int> route, int begin, DoubleValue capacity, DoubleArray demand) {
      double o = 0.0;
      for (int i = begin + 1; i < route.Count; i++) {
        if (route[i] == 0) return (o < capacity.Value);
        else {
          o += demand[route[i]];
        }
      }
      return (o < capacity.Value);
    }

    protected override List<int> CreateSolution() {
      //double alpha, beta, gamma;
      double alpha = N(Alpha.Value.Value, Math.Sqrt(AlphaVariance.Value.Value), RandomParameter.ActualValue);
      double beta = N(Beta.Value.Value, Math.Sqrt(BetaVariance.Value.Value), RandomParameter.ActualValue);
      double gamma = N(Gamma.Value.Value, Math.Sqrt(GammaVariance.Value.Value), RandomParameter.ActualValue);

      double x0 = CoordinatesParameter.ActualValue[0, 0];
      double y0 = CoordinatesParameter.ActualValue[0, 1];
      double distance = 0;
      double cost = 0;
      double minimumCost = double.MaxValue;
      List<int> unroutedList = new List<int>();
      List<double> costList = new List<double>();
      int index;
      int indexOfMinimumCost = -1;
      int indexOfCustomer = -1;

      int vehicles = VehiclesParameter.ActualValue.Value;
      DoubleMatrix coordinates = CoordinatesParameter.ActualValue;
      DoubleArray dueTime = DueTimeParameter.ActualValue;
      DoubleArray serviceTime = ServiceTimeParameter.ActualValue;
      DoubleArray readyTime = ReadyTimeParameter.ActualValue;
      DoubleArray demand = DemandParameter.ActualValue;
      DoubleValue capacity = CapacityParameter.ActualValue;

      bool useDistanceMatrix = UseDistanceMatrixParameter.ActualValue.Value;
      if (useDistanceMatrix) {
        if (DistanceMatrixParameter.ActualValue == null) {
          DistanceMatrixParameter.ActualValue = CreateDistanceMatrix(coordinates);
        }

        coordinates = DistanceMatrixParameter.ActualValue;
      }

      /*-----------------------------------------------------------------------------
       * generate cost list
       *-----------------------------------------------------------------------------
       */
      for (int i = 1; i <= Cities; i++) {
        distance = Distance(i, 0, coordinates, useDistanceMatrix);
        if (coordinates[i, 0] < x0) distance = -distance;
        cost = -alpha * distance + // distance 0 <-> City[i]
                 beta * (DueTimeParameter.ActualValue[i]) + // latest arrival time
                 gamma * (Math.Asin((coordinates[i, 1] - y0) / distance) / 360 * distance); // polar angle

        index = 0;
        while (index < costList.Count && costList[index] < cost) index++;
        costList.Insert(index, cost);
        unroutedList.Insert(index, i);
      }

      /*------------------------------------------------------------------------------
       * route customers according to cost list
       *------------------------------------------------------------------------------
       */
      int routeIndex = 0;
      int currentRoute = 0;
      int c;
      int customer = -1;
      int subTourCount = 1;

      List<int> route = new List<int>(Cities + vehicles - 1);
      minimumCost = double.MaxValue;
      indexOfMinimumCost = -1;
      route.Add(0);
      route.Add(0);
      route.Insert(1, unroutedList[0]);
      unroutedList.RemoveAt(0);
      currentRoute = routeIndex;
      routeIndex++;

      do {
        for (c = 0; c < unroutedList.Count; c++) {
          for (int i = currentRoute + 1; i < route.Count; i++) {
            route.Insert(i, (int)unroutedList[c]);
            if (route[currentRoute] != 0) { throw new Exception("currentRoute not depot"); }
            cost = TravelDistance(route, currentRoute, coordinates, useDistanceMatrix);
            if (cost < minimumCost && SubrouteConstraintsOK(route, currentRoute, coordinates, useDistanceMatrix, dueTime, readyTime, serviceTime, demand, capacity)) {
              minimumCost = cost;
              indexOfMinimumCost = i;
              customer = (int)unroutedList[c];
              indexOfCustomer = c;
            }
            route.RemoveAt(i);
          }
        }
        // insert customer if found
        if (indexOfMinimumCost != -1) {
          route.Insert(indexOfMinimumCost, customer);
          routeIndex++;
          unroutedList.RemoveAt(indexOfCustomer);
          costList.RemoveAt(indexOfCustomer);
        } else { // no feasible customer found
          routeIndex++;
          route.Insert(routeIndex, 0);
          currentRoute = routeIndex;
          route.Insert(route.Count - 1, (int)unroutedList[0]);
          unroutedList.RemoveAt(0);
          routeIndex++;
          subTourCount++;
        }
        // reset minimum		
        minimumCost = double.MaxValue;
        indexOfMinimumCost = -1;
        indexOfCustomer = -1;
        customer = -1;
      } while (unroutedList.Count > 0);
      while (route.Count < Cities + vehicles)
        route.Add(0);

      return route;
    }
  }
}
