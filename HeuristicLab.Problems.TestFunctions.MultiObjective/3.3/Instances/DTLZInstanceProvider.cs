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
using System.Linq;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  public class DTLZInstanceProvider : ProblemInstanceProvider<MOTFData> {

    public override string Name {
      get { return "DTLZ"; }
    }

    public override string Description {
      get { return "DTLZ Problems"; }
    }

    public override Uri WebLink {
      get { return new Uri("http://dev.heuristiclab.com/trac.fcgi/wiki/Documentation/Reference/Test%20Functions"); }
    }

    public override string ReferencePublication {
      get { return "http://repository.ias.ac.in/81671/ [30.11.15]"; }
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      var evaluators = ApplicationManager.Manager.GetInstances<DTLZ>().OrderBy(x => x.Name);
      return evaluators.Select(x => new MOTFDataDescriptor(x));
    }

    public override MOTFData LoadData(IDataDescriptor id) {
      var descriptor = (MOTFDataDescriptor)id;
      return new MOTFData(descriptor);
    }
  }
}
