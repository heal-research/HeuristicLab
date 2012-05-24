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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Optimization;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;

namespace HeuristicLab.Problems.VehicleRouting.Interfaces {
  public interface IVRPProblemInstance: IItem {
    IVRPEvaluator SolutionEvaluator { get; }
    IVRPEvaluator MoveEvaluator { get; }
    IEnumerable<IOperator> Operators { get; }

    event EventHandler EvaluationChanged;

    DoubleMatrix Coordinates { get; }
    DoubleMatrix DistanceMatrix { get; }
    BoolValue UseDistanceMatrix { get; }
    IntValue Vehicles { get; }
    DoubleArray Demand { get; }
    IntValue Cities { get; }

    DoubleValue FleetUsageFactor { get; }
    DoubleValue DistanceFactor { get; }

    double[] GetCoordinates(int city);
    double GetDemand(int city);
    double GetDistance(int start, int end, IVRPEncoding solution);
    double GetInsertionDistance(int start, int customer, int end, IVRPEncoding solution, out double startDistance, out double endDistance);
    bool Feasible(IVRPEncoding solution);
    bool TourFeasible(Tour tour, IVRPEncoding solution);
    VRPEvaluation Evaluate(IVRPEncoding solution);
    VRPEvaluation EvaluateTour(Tour tour, IVRPEncoding solution);
    bool Feasible(VRPEvaluation eval);
    double GetInsertionCosts(VRPEvaluation eval, IVRPEncoding solution, int customer, int tour, int index, out bool feasible);
  }
}
