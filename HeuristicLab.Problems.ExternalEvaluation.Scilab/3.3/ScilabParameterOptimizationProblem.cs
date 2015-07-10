#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.ParameterOptimization;

namespace HeuristicLab.Problems.ExternalEvaluation.Scilab {
  [Item("Scilab Parameter Optimization Problem", "Optimization of a parameter vector which is evaluated in Scilab.")]
  [StorableClass]
  [Creatable(CreatableAttribute.Categories.ExternalEvaluationProblems, Priority = 120)]
  public class ScilabParameterOptimizationProblem : ParameterOptimizationProblem {
    private const string QualityVariableParameterName = "QualityVariableName";
    private const string ScilabEvaluationScriptParameterName = "ScilabEvaluationScript";
    private const string ScilabInitializationScriptParameterName = "ScilabInitializationScript";

    #region parameters
    public IFixedValueParameter<StringValue> QualityVariableParameter {
      get { return (IFixedValueParameter<StringValue>)Parameters[QualityVariableParameterName]; }
    }
    public IFixedValueParameter<TextFileValue> ScilabEvaluationScriptParameter {
      get { return (IFixedValueParameter<TextFileValue>)Parameters[ScilabEvaluationScriptParameterName]; }
    }
    public IFixedValueParameter<TextFileValue> ScilabInitializationScriptParameter {
      get { return (IFixedValueParameter<TextFileValue>)Parameters[ScilabInitializationScriptParameterName]; }
    }
    #endregion

    #region properties
    public string QualityVariable {
      get { return QualityVariableParameter.Value.Value; }
      set { QualityVariableParameter.Value.Value = value; }
    }
    public TextFileValue ScilabEvaluationScript {
      get { return ScilabEvaluationScriptParameter.Value; }
    }
    public TextFileValue ScilabInitializationScript {
      get { return ScilabInitializationScriptParameter.Value; }
    }

    #endregion

    [StorableConstructor]
    protected ScilabParameterOptimizationProblem(bool deserializing) : base(deserializing) { }
    protected ScilabParameterOptimizationProblem(ScilabParameterOptimizationProblem original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScilabParameterOptimizationProblem(this, cloner);
    }

    public ScilabParameterOptimizationProblem()
      : base(new ScilabParameterVectorEvaluator()) {
      Parameters.Add(new FixedValueParameter<StringValue>(QualityVariableParameterName, "The name of the quality variable of the Scilab script.", new StringValue("quality")));
      Parameters.Add(new FixedValueParameter<TextFileValue>(ScilabEvaluationScriptParameterName, "The path to the Scilab evaluation script.", new TextFileValue()));
      Parameters.Add(new FixedValueParameter<TextFileValue>(ScilabInitializationScriptParameterName, "The path to a Scilab script that should be executed once when the algorithm starts.", new TextFileValue()));

      ScilabEvaluationScript.FileDialogFilter = @"Scilab Scripts|*.sce|All files|*.*";
      ScilabInitializationScript.FileDialogFilter = @"Scilab Scripts|*.sce|All files|*.*";
    }
  }
}
