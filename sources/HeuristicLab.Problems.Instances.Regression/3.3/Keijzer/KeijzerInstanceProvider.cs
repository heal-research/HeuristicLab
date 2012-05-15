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

namespace HeuristicLab.Problems.Instances.Regression {
  public class KeijzerInstanceProvider : ArtificialRegressionInstanceProvider {
    public override string Name {
      get { return "Keijzer Benchmark Problems"; }
    }
    public override string Description {
      get { return ""; }
    }
    public override Uri WebLink {
      get { return new Uri("http://groups.csail.mit.edu/EVO-DesignOpt/GPBenchmarks/"); }
    }
    public override string ReferencePublication {
      get { return ""; }
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      List<IDataDescriptor> descriptorList = new List<IDataDescriptor>();
      descriptorList.Add(new KeijzerFunctionFour());
      descriptorList.Add(new KeijzerFunctionSix());
      descriptorList.Add(new KeijzerFunctionSeven());
      descriptorList.Add(new KeijzerFunctionEight());
      descriptorList.Add(new KeijzerFunctionTwelve());
      descriptorList.Add(new KeijzerFunctionThirteen());
      descriptorList.Add(new KeijzerFunctionFifteen());
      descriptorList.Add(new KeijzerFunctionSixteen());
      return descriptorList;
    }
  }
}
