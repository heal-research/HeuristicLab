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

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class RegressionRealWorldInstanceProvider : ResourceRegressionInstanceProvider {
    public override string Name {
      get { return "Real World Benchmark Problems"; }
    }
    public override string Description {
      get {
        return "";
      }
    }
    public override Uri WebLink {
      get { return null; }
    }
    public override string ReferencePublication {
      get { return ""; }
    }

    protected override string FileName { get { return "RegressionRealWorld"; } }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      List<ResourceRegressionDataDescriptor> descriptorList = new List<ResourceRegressionDataDescriptor>();
      descriptorList.Add(new ChemicalOne());
      descriptorList.Add(new Housing());
      descriptorList.Add(new Tower());
      descriptorList.Add(new Powermeter());
      descriptorList.Add(new SARCOS());
      return descriptorList;
    }
  }
}
