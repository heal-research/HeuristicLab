#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("1B593917-9CC6-49B2-A446-02A0CFB273FE")]
  public abstract class SupervisedDataAnalysisProblemData : DataAnalysisProblemData {
    protected const string TargetVariableParameterName = "TargetVariable";

    public IConstrainedValueParameter<StringValue> TargetVariableParameter => (IConstrainedValueParameter<StringValue>)Parameters[TargetVariableParameterName];

    public string TargetVariable {
      get { return TargetVariableParameter.Value.Value; }
      set {
        if (value == null) throw new ArgumentNullException("targetVariable", "The provided value for the targetVariable is null.");
        if (value == TargetVariable) return;

        var matchingParameterValue = TargetVariableParameter.ValidValues.FirstOrDefault(v => v.Value == value);
        if (matchingParameterValue == null) throw new ArgumentException("The provided value is not valid as the targetVariable.", "targetVariable");
        TargetVariableParameter.Value = matchingParameterValue;
      }
    }
    public IEnumerable<double> TargetVariableValues => Dataset.GetDoubleValues(TargetVariable);
    public IEnumerable<double> TargetVariableTrainingValues => Dataset.GetDoubleValues(TargetVariable, TrainingIndices);
    public IEnumerable<double> TargetVariableTestValues => Dataset.GetDoubleValues(TargetVariable, TestIndices);


    public SupervisedDataAnalysisProblemData(IDataset dataset, IEnumerable<string> allowedInputVariables, IEnumerable<ITransformation> transformations = null, IntervalCollection variableRanges = null) : base(dataset, allowedInputVariables, transformations, variableRanges) { }
    protected SupervisedDataAnalysisProblemData(SupervisedDataAnalysisProblemData original, Cloner cloner) : base(original, cloner) { }

    [StorableConstructor]
    protected SupervisedDataAnalysisProblemData(StorableConstructorFlag _) : base(_) { }

  }
}
