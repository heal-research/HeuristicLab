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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class PennMLRegressionDataDescriptor : ResourceRegressionDataDescriptor {
    private readonly string[] variableNames;
    private readonly string[] allowedInputVariables;
    private readonly string target;
    private readonly int trainStart;
    private readonly int trainEnd;
    private readonly int testStart;
    private readonly int testEnd;

    public PennMLRegressionDataDescriptor(string resourceName) : base(resourceName) { }

    public PennMLRegressionDataDescriptor(string resourceName, IEnumerable<string> variableNames, IEnumerable<string> allowedInputVariables, string target,
      IntRange trainRange, IntRange testRange) : this(resourceName) {
      this.variableNames = variableNames.ToArray();
      this.allowedInputVariables = allowedInputVariables.ToArray();
      this.target = target;

      this.trainStart = trainRange.Start;
      this.trainEnd = trainRange.End;
      this.testStart = testRange.Start;
      this.testEnd = testRange.End;
    }

    public override string Name {
      get { return ResourceName.Replace(".csv", ""); }
    }

    public override string Description {
      get { return "No description available."; }
    }

    protected override string TargetVariable {
      get { return target; }
    }

    protected override string[] VariableNames {
      get { return variableNames; }
    }

    protected override string[] AllowedInputVariables {
      get { return allowedInputVariables; }
    }

    protected override int TrainingPartitionStart {
      get { return trainStart; }
    }

    protected override int TrainingPartitionEnd {
      get { return trainEnd; }
    }

    protected override int TestPartitionStart {
      get { return testStart; }
    }

    protected override int TestPartitionEnd {
      get { return testEnd; }
    }
  }
}
