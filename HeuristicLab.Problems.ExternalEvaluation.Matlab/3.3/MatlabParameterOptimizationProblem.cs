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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Problems.ParameterOptimization;

namespace HeuristicLab.Problems.ExternalEvaluation.Matlab {
  [Item("MATLAB Parameter Optimization Problem", "Optimization of a parameter vector which is evaluated in MATLAB.")]
  [StorableType("CC937102-7280-484C-BB85-88C357E31C6B")]
  [Creatable(CreatableAttribute.Categories.ExternalEvaluationProblems, Priority = 110)]
  public class MatlabParameterOptimizationProblem : ParameterOptimizationProblem {
    private const string QualityVariableParameterName = "QualityVariableName";
    private const string MatlabEvaluationScriptParameterName = "MATLABEvaluationScript";
    private const string MatlabInitializationScriptParameterName = "MATLABInitializationScript";

    #region parameters
    public IFixedValueParameter<StringValue> QualityVariableParameter {
      get { return (IFixedValueParameter<StringValue>)Parameters[QualityVariableParameterName]; }
    }
    public IFixedValueParameter<TextFileValue> MatlabEvaluationScriptParameter {
      get { return (IFixedValueParameter<TextFileValue>)Parameters[MatlabEvaluationScriptParameterName]; }
    }
    public IFixedValueParameter<TextFileValue> MatlabInitializationScriptParameter {
      get { return (IFixedValueParameter<TextFileValue>)Parameters[MatlabInitializationScriptParameterName]; }
    }
    #endregion

    #region properties
    public string QualityVariable {
      get { return QualityVariableParameter.Value.Value; }
      set { QualityVariableParameter.Value.Value = value; }
    }
    public TextFileValue MatlabEvaluationScript {
      get { return MatlabEvaluationScriptParameter.Value; }
    }
    public TextFileValue MatlabInitializationScript {
      get { return MatlabInitializationScriptParameter.Value; }
    }
    #endregion

    [StorableConstructor]
    protected MatlabParameterOptimizationProblem(StorableConstructorFlag _) : base(_) { }
    protected MatlabParameterOptimizationProblem(MatlabParameterOptimizationProblem original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MatlabParameterOptimizationProblem(this, cloner);
    }

    public MatlabParameterOptimizationProblem()
      : base(new MatlabParameterVectorEvaluator()) {
      Parameters.Add(new FixedValueParameter<StringValue>(QualityVariableParameterName, "The name of the quality variable of the MATLAB script.", new StringValue("quality")));
      Parameters.Add(new FixedValueParameter<TextFileValue>(MatlabEvaluationScriptParameterName, "The path to the MATLAB evaluation script.", new TextFileValue()));
      Parameters.Add(new FixedValueParameter<TextFileValue>(MatlabInitializationScriptParameterName, "The path to a MATLAB script for initialization that should be executed once before the algorithm starts.", new TextFileValue()));

      MatlabEvaluationScript.FileDialogFilter = @"MATLAB Scripts|*.m|All files|*.*";
    }
  }
}
