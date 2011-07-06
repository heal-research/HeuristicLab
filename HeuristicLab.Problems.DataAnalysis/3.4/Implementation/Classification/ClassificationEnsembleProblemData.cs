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
  [Item("ClassificationEnsembleProblemData", "Represents an item containing all data defining a classification problem.")]
  public class ClassificationEnsembleProblemData : ClassificationProblemData {

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
    protected ClassificationEnsembleProblemData(bool deserializing) : base(deserializing) { }

    protected ClassificationEnsembleProblemData(ClassificationEnsembleProblemData original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new ClassificationEnsembleProblemData(this, cloner); }

    public ClassificationEnsembleProblemData(IClassificationProblemData classificationProblemData)
      : base(classificationProblemData.Dataset, classificationProblemData.AllowedInputVariables, classificationProblemData.TargetVariable) {
      this.TrainingPartition.Start = classificationProblemData.TrainingPartition.Start;
      this.TrainingPartition.End = classificationProblemData.TrainingPartition.End;
      this.TestPartition.Start = classificationProblemData.TestPartition.Start;
      this.TestPartition.End = classificationProblemData.TestPartition.End;
    }
  }
}
