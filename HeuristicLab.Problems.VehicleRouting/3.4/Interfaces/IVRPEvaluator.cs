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

using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;
using HEAL.Attic;

namespace HeuristicLab.Problems.VehicleRouting.Interfaces {
  [StorableType("C3423687-3CBE-41E4-9227-FF4F1D6912E1")]
  public interface IVRPEvaluator : IVRPOperator, ISingleObjectiveEvaluator {
    ILookupParameter<IVRPEncoding> VRPToursParameter { get; }

    VRPEvaluation Evaluate(IVRPProblemInstance instance, IVRPEncoding solution);
    VRPEvaluation EvaluateTour(IVRPProblemInstance instance, Tour tour, IVRPEncoding solution);
    bool Feasible(VRPEvaluation evaluation);
    double GetInsertionCosts(IVRPProblemInstance instance, IVRPEncoding solution, VRPEvaluation eval, int customer, int tour, int index, out bool feasible);
  }
}
