#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  class PhysicsInstanceProvider : ArtificialRegressionInstanceProvider {
    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      return new List<IDataDescriptor>()
      {
         new RocketFuelFlow(123),
         new AircraftLift(456),
         new FluidDynamics(789),
         new AircraftMaximumLift(321)
      };
    }

    public override string Name { get { return "Physics Benchmark Problems"; } }
    public override string Description { get { return ""; } }
    public override Uri WebLink { get { return new Uri(@"https://doi.org/10.1016/j.eswa.2018.05.021"); } }
    public override string ReferencePublication {
      get {
        return "Chen Chen, Changtong Luo, Zonglin Jiang, \"A multilevel block building algorithm for fast modeling generalized separable systems\", Expert Systems with Applications, Volume 109, 2018, Pages 25-34 https://doi.org/10.1016/j.eswa.2018.05.021 as well as the (slightly different) pre-print on arXiv: https://arxiv.org/abs/1706.02281";
      }
    }
  }
}
