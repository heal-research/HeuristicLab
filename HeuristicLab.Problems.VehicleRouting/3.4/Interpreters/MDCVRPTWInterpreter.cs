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

using HeuristicLab.Data;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using System;

namespace HeuristicLab.Problems.VehicleRouting.Interpreters {
  public class MDCVRPTWInterpreter : MDCVRPInterpreter, IVRPDataInterpreter<MDCVRPTWData> {    
    protected override IVRPProblemInstance CreateProblemInstance() {
      return new MDCVRPTWProblemInstance();
    }

    protected override void Interpret(IVRPData data, IVRPProblemInstance problemInstance) {
      base.Interpret(data, problemInstance);

      MDCVRPTWData cvrptwData = (MDCVRPTWData)data;
      MDCVRPTWProblemInstance problem = (MDCVRPTWProblemInstance)problemInstance;

      problem.ReadyTime = new DoubleArray(cvrptwData.ReadyTimes);
      problem.ServiceTime = new DoubleArray(cvrptwData.ServiceTimes);
      problem.DueTime = new DoubleArray(cvrptwData.DueTimes);
    }
  }
}
