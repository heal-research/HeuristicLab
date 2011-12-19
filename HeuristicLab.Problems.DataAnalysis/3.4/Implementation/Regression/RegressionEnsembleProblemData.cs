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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  [Item("RegressionEnsembleProblemData", "Represents an item containing all data defining a regression problem.")]
  public sealed class RegressionEnsembleProblemData : RegressionProblemData {

    public override bool IsTrainingSample(int index) {
      return index >= 0 && index < Dataset.Rows &&
             TrainingPartition.Start <= index && index < TrainingPartition.End;
    }

    public override bool IsTestSample(int index) {
      return index >= 0 && index < Dataset.Rows &&
             TestPartition.Start <= index && index < TestPartition.End;
    }

    private static readonly RegressionEnsembleProblemData emptyProblemData;
    public new static RegressionEnsembleProblemData EmptyProblemData {
      get { return emptyProblemData; }
    }
    static RegressionEnsembleProblemData() {
      var problemData = new RegressionEnsembleProblemData();
      problemData.Parameters.Clear();
      problemData.Name = "Empty Regression ProblemData";
      problemData.Description = "This ProblemData acts as place holder before the correct problem data is loaded.";
      problemData.isEmpty = true;

      problemData.Parameters.Add(new FixedValueParameter<Dataset>(DatasetParameterName, "", new Dataset()));
      problemData.Parameters.Add(new FixedValueParameter<ReadOnlyCheckedItemList<StringValue>>(InputVariablesParameterName, ""));
      problemData.Parameters.Add(new FixedValueParameter<IntRange>(TrainingPartitionParameterName, "", (IntRange)new IntRange(0, 0).AsReadOnly()));
      problemData.Parameters.Add(new FixedValueParameter<IntRange>(TestPartitionParameterName, "", (IntRange)new IntRange(0, 0).AsReadOnly()));
      problemData.Parameters.Add(new ConstrainedValueParameter<StringValue>(TargetVariableParameterName, new ItemSet<StringValue>()));
      emptyProblemData = problemData;
    }

    [StorableConstructor]
    private RegressionEnsembleProblemData(bool deserializing) : base(deserializing) { }
    private RegressionEnsembleProblemData(RegressionEnsembleProblemData original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      if (this == emptyProblemData) return emptyProblemData;
      return new RegressionEnsembleProblemData(this, cloner);
    }

    public RegressionEnsembleProblemData() : base() { }
    public RegressionEnsembleProblemData(IRegressionProblemData regressionProblemData)
      : base(regressionProblemData.Dataset, regressionProblemData.AllowedInputVariables, regressionProblemData.TargetVariable) {
      TrainingPartition.Start = regressionProblemData.TrainingPartition.Start;
      TrainingPartition.End = regressionProblemData.TrainingPartition.End;
      TestPartition.Start = regressionProblemData.TestPartition.Start;
      TestPartition.End = regressionProblemData.TestPartition.End;
    }
    public RegressionEnsembleProblemData(Dataset dataset, IEnumerable<string> allowedInputVariables, string targetVariable)
      : base(dataset, allowedInputVariables, targetVariable) {
    }
  }
}
