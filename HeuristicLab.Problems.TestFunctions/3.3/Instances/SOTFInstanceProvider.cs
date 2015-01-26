#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.TestFunctions {
  public class SOTFInstanceProvider : ProblemInstanceProvider<SOTFData> {
    public override string Name {
      get { return "General"; }
    }

    public override string Description {
      get { return "Single Objective Test Functions"; }
    }

    public override Uri WebLink {
      get { return new Uri("http://dev.heuristiclab.com/trac.fcgi/wiki/Documentation/Reference/Test%20Functions"); }
    }

    public override string ReferencePublication {
      get { return string.Empty; }
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      var evaluators = ApplicationManager.Manager.GetInstances<ISingleObjectiveTestFunctionProblemEvaluator>()
                                                 .OrderBy(x => x.Name);
      return evaluators.Select(x => new SOTFDataDescriptor(x));
    }

    public override SOTFData LoadData(IDataDescriptor id) {
      var descriptor = (SOTFDataDescriptor)id;
      return new SOTFData {
        Name = descriptor.Name,
        Description = descriptor.Description,
        Evaluator = descriptor.Evaluator
      };
    }
  }
}
