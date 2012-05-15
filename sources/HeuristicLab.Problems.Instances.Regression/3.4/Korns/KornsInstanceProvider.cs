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
  public class KornsInstanceProvider : ArtificialRegressionInstanceProvider {
    public override string Name {
      get { return "Korns Benchmark Problems"; }
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
      descriptorList.Add(new KornsFunctionOne());
      descriptorList.Add(new KornsFunctionTwo());
      descriptorList.Add(new KornsFunctionThree());
      descriptorList.Add(new KornsFunctionFour());
      descriptorList.Add(new KornsFunctionFive());
      descriptorList.Add(new KornsFunctionSix());
      descriptorList.Add(new KornsFunctionSeven());
      descriptorList.Add(new KornsFunctionEight());
      descriptorList.Add(new KornsFunctionNine());
      descriptorList.Add(new KornsFunctionTen());
      descriptorList.Add(new KornsFunctionEleven());
      descriptorList.Add(new KornsFunctionTwelve());
      descriptorList.Add(new KornsFunctionThirteen());
      descriptorList.Add(new KornsFunctionFourteen());
      descriptorList.Add(new KornsFunctionFiveteen());
      return descriptorList;
    }
  }
}
