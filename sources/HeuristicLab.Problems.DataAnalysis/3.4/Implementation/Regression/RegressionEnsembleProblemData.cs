#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  [Item("RegressionEnsembleProblemData", "Represents an item containing all data defining a regression problem.")]
  public sealed class RegressionEnsembleProblemData : RegressionProblemData {

    public override IEnumerable<int> TrainingIndizes {
      get {
        return Enumerable.Range(TrainingPartition.Start, TrainingPartition.End - TrainingPartition.Start);
      }
    }

    public override IEnumerable<int> TestIndizes {
      get {
        return Enumerable.Range(TestPartition.Start, TestPartition.End - TestPartition.Start);
      }
    }

    [StorableConstructor]
    private RegressionEnsembleProblemData(bool deserializing) : base(deserializing) { }

    private RegressionEnsembleProblemData(RegressionEnsembleProblemData original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new RegressionEnsembleProblemData(this, cloner); }

    public RegressionEnsembleProblemData(IRegressionProblemData regressionProblemData)
      : base(regressionProblemData.Dataset, regressionProblemData.AllowedInputVariables, regressionProblemData.TargetVariable) {
      TrainingPartition.Start = regressionProblemData.TrainingPartition.Start;
      TrainingPartition.End = regressionProblemData.TrainingPartition.End;
      TestPartition.Start = regressionProblemData.TestPartition.Start;
      TestPartition.End = regressionProblemData.TestPartition.End;
    }
  }
}
