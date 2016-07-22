#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.ParameterOptimization;
using ScilabConnector = DotNetScilab.Scilab;

namespace HeuristicLab.Problems.ExternalEvaluation.Scilab {
  [Item("SciLabParameterVectorEvaluator", "An evaluator which takes a parameter vector and returns a quality value, calculated by a Scilab script.")]
  [StorableClass]
  public sealed class ScilabParameterVectorEvaluator : ParameterVectorEvaluator {
    private const string MaximizationParameterName = "Maximization";
    private const string QualityVariableParameterName = "QualityVariableName";
    private const string ScilabEvaluationScriptParameterName = "ScilabEvaluationScript";
    private const string ScilabInitializationScriptParameterName = "ScilabInitializationScript";

    #region parameters
    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[MaximizationParameterName]; }
    }
    public ILookupParameter<StringValue> QualityVariableParameter {
      get { return (ILookupParameter<StringValue>)Parameters[QualityVariableParameterName]; }
    }
    public ILookupParameter<TextFileValue> ScilabEvaluationScriptParameter {
      get { return (ILookupParameter<TextFileValue>)Parameters[ScilabEvaluationScriptParameterName]; }
    }
    public ILookupParameter<TextFileValue> ScilabInitializationScriptParameter {
      get { return (ILookupParameter<TextFileValue>)Parameters[ScilabInitializationScriptParameterName]; }
    }
    #endregion

    [StorableConstructor]
    private ScilabParameterVectorEvaluator(bool deserializing) : base(deserializing) { }
    private ScilabParameterVectorEvaluator(ScilabParameterVectorEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScilabParameterVectorEvaluator(this, cloner);
    }

    public ScilabParameterVectorEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>(MaximizationParameterName, "The flag which determines if this is a maximization problem."));
      Parameters.Add(new LookupParameter<StringValue>(QualityVariableParameterName, "The name of the quality variable of the Scilab script."));
      Parameters.Add(new LookupParameter<TextFileValue>(ScilabEvaluationScriptParameterName, "The path to the Scilab evaluation script."));
      Parameters.Add(new LookupParameter<TextFileValue>(ScilabInitializationScriptParameterName, "The path to a Scilab script that should be executed once when the algorithm starts."));
    }

    private static readonly object locker = new object();
    private static ScilabConnector scilab = null;
    private bool startedScilab = false;

    public override IOperation Apply() {
      var evaluationScript = ScilabEvaluationScriptParameter.ActualValue;
      if (string.IsNullOrEmpty(evaluationScript.Value)) throw new FileNotFoundException("The evaluation script in the problem is not set.");
      if (!evaluationScript.Exists()) throw new FileNotFoundException(string.Format("The evaluation script \"{0}\" cannot be found.", evaluationScript.Value));

      var initializationScript = ScilabInitializationScriptParameter.ActualValue;
      if (!string.IsNullOrEmpty(initializationScript.Value) && !initializationScript.Exists()) throw new FileNotFoundException(string.Format("The initialization script \"{0}\" cannot be found.", initializationScript.Value));

      int result;
      // Scilab is used via a c++ wrapper that calls static methods. Hence it is not possible to parallelize the evaluation.
      // It is also not possible to run multiple algorithms solving separate Scilab optimization problems at the same time.
      lock (locker) {
        //initialize scilab and execute initialization script
        if (scilab == null) {
          startedScilab = true;
          scilab = new ScilabConnector(false);
          if (!string.IsNullOrEmpty(initializationScript.Value)) {
            result = scilab.execScilabScript(initializationScript.Value);
            if (result != 0) ThrowSciLabException(initializationScript.Value, result);
          }
        } else if (!startedScilab) {
          throw new InvalidOperationException("Could not run multiple optimization algorithms in parallel.");
        }

        var parameterVector = ParameterVectorParameter.ActualValue;
        var parameterNames = ParameterNamesParameter.ActualValue;
        if (parameterNames.Any(string.IsNullOrEmpty)) throw new ArgumentException("Not all parameter names are provided. Change the 'ParameterNames' parameter in the 'Problem' tab.");
        if (ProblemSizeParameter.ActualValue.Value != parameterVector.Length || ProblemSizeParameter.ActualValue.Value != parameterNames.Length)
          throw new ArgumentException("The size of the parameter vector or the parameter names vector does not match the problem size.");

        for (int i = 0; i < ProblemSizeParameter.ActualValue.Value; i++) {
          result = scilab.createNamedMatrixOfDouble(parameterNames[i], 1, 1, new double[] { parameterVector[i] });
          if (result != 0) ThrowSciLabException("setting parameter " + parameterNames[i], result);
        }

        string script = ScilabEvaluationScriptParameter.ActualValue.Value;
        result = scilab.execScilabScript(script);
        if (result != 0) ThrowSciLabException(script, result);

        string qualityVariableName = QualityVariableParameter.ActualValue.Value;
        double[] values = scilab.readNamedMatrixOfDouble(qualityVariableName);
        if (values == null) throw new InvalidOperationException(string.Format("Could not find the variable \"{0}\" in the Scilab workspace, that should hold the quality value.", qualityVariableName));
        double quality = values[0];

        var worstQualityValue = MaximizationParameter.ActualValue.Value ? double.MinValue : double.MaxValue;
        if (double.IsNaN(quality)) quality = worstQualityValue;
        if (double.IsInfinity(quality)) quality = worstQualityValue;

        QualityParameter.ActualValue = new DoubleValue(quality);
        return base.Apply();
      }
    }

    public override void ClearState() {
      base.ClearState();
      if (startedScilab)
        scilab = null;
      startedScilab = false;
    }

    private void ThrowSciLabException(string fileName, int errorCode) {
      const string code = "errorMsg = lasterror();";
      int result = scilab.SendScilabJob(code);
      if (result != 0) throw new InvalidOperationException(string.Format("An error occured during the execution of the Scilab script {0}.", fileName));

      string errorMessage = scilab.readNamedMatrixOfString("errorMsg")[0];

      string message = string.Format("The error {1} occured during the execution of the Scilab script {0}. "
        + Environment.NewLine + Environment.NewLine + " {2}", fileName, errorCode, errorMessage);
      throw new InvalidOperationException(message);
    }
  }
}
