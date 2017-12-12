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
using System.Runtime.InteropServices;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.ParameterOptimization;

namespace HeuristicLab.Problems.ExternalEvaluation.Matlab {
  [Item("MATLABParameterVectorEvaluator", "An evaluator which takes a parameter vector and returns a quality value, calculated by a MATLAB script.")]
  [StorableClass]
  public sealed class MatlabParameterVectorEvaluator : ParameterVectorEvaluator {
    private const string QualityVariableParameterName = "QualityVariableName";
    private const string MatlabEvaluationScriptParameterName = "MATLABEvaluationScript";
    private const string MatlabInitializationScriptParameterName = "MATLABInitializationScript";

    #region parameters
    public ILookupParameter<StringValue> QualityVariableParameter {
      get { return (ILookupParameter<StringValue>)Parameters[QualityVariableParameterName]; }
    }
    public ILookupParameter<TextFileValue> MatlabEvaluationScriptParameter {
      get { return (ILookupParameter<TextFileValue>)Parameters[MatlabEvaluationScriptParameterName]; }
    }
    public ILookupParameter<TextFileValue> MatlabInitializationScriptParameter {
      get { return (ILookupParameter<TextFileValue>)Parameters[MatlabInitializationScriptParameterName]; }
    }
    #endregion

    [StorableConstructor]
    private MatlabParameterVectorEvaluator(bool deserializing) : base(deserializing) { }
    private MatlabParameterVectorEvaluator(MatlabParameterVectorEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MatlabParameterVectorEvaluator(this, cloner);
    }

    public MatlabParameterVectorEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<StringValue>(QualityVariableParameterName, "The name of the quality variable calculated in the MATLAB script."));
      Parameters.Add(new LookupParameter<TextFileValue>(MatlabEvaluationScriptParameterName, "The path to the MATLAB evaluation script."));
      Parameters.Add(new LookupParameter<TextFileValue>(MatlabInitializationScriptParameterName, "The path to a MATLAB script for initialization that should be executed once when the algorithm starts."));
    }


    public override void ClearState() {
      //mkommend: necessary because matlabConnector.Quit() does not work
      try {
        if (matLabConnector != null)
          matLabConnector.Execute("exit");
      } catch (COMException) { } finally {
        matLabConnector = null;
      }

      changedDirectory = false;
      base.ClearState();
    }

    private MLApp.MLApp matLabConnector;
    private bool changedDirectory = false;
    private readonly object locker = new object();

    public override IOperation Apply() {
      lock (locker) {
        var initializationScript = MatlabInitializationScriptParameter.ActualValue;
        if (!string.IsNullOrEmpty(initializationScript.Value) && !initializationScript.Exists())
          throw new FileNotFoundException(string.Format("The initialization script \"{0}\" cannot be found.", initializationScript.Value));

        var evaluationScript = MatlabEvaluationScriptParameter.ActualValue;
        if (string.IsNullOrEmpty(evaluationScript.Value))
          throw new FileNotFoundException("The evaluation script in the problem is not set.");
        if (!evaluationScript.Exists())
          throw new FileNotFoundException(string.Format("The evaluation script \"{0}\" cannot be found.", evaluationScript.Value));

        string result;
        if (matLabConnector == null) {
          Type matlabtype = Type.GetTypeFromProgID("matlab.application.single");
          matLabConnector = (MLApp.MLApp)Activator.CreateInstance(matlabtype);
          matLabConnector.Visible = 0;

          if (!string.IsNullOrEmpty(initializationScript.Value)) {
            var directoryName = Path.GetDirectoryName(initializationScript.Value);
            if (string.IsNullOrEmpty(directoryName)) directoryName = Environment.CurrentDirectory;
            result = matLabConnector.Execute(string.Format("cd '{0}'", directoryName));
            if (!string.IsNullOrEmpty(result)) throw new InvalidOperationException(result);

            result = matLabConnector.Execute(Path.GetFileNameWithoutExtension(initializationScript.Value));
            if (!string.IsNullOrEmpty(result)) throw new InvalidOperationException(result);
          }
        }

        if (!changedDirectory) {
          var directoryName = Path.GetDirectoryName(evaluationScript.Value);
          if (string.IsNullOrEmpty(directoryName)) directoryName = Environment.CurrentDirectory;
          result = matLabConnector.Execute(string.Format("cd '{0}'", directoryName));
          if (!string.IsNullOrEmpty(result)) throw new InvalidOperationException(result);
          changedDirectory = true;
        }

        var parameterVector = ParameterVectorParameter.ActualValue;
        var parameterNames = ParameterNamesParameter.ActualValue;

        for (int i = 0; i < ProblemSizeParameter.ActualValue.Value; i++) {
          matLabConnector.PutWorkspaceData(parameterNames[i], "base", parameterVector[i]);
        }

        result = matLabConnector.Execute(Path.GetFileNameWithoutExtension(evaluationScript.Value));
        if (!string.IsNullOrEmpty(result)) throw new InvalidOperationException(result);

        string qualityVariableName = QualityVariableParameter.ActualValue.Value;
        var quality = matLabConnector.GetVariable(qualityVariableName, "base");

        if (double.IsNaN(quality)) quality = double.MaxValue;
        if (double.IsInfinity(quality)) quality = double.MaxValue;

        QualityParameter.ActualValue = new DoubleValue(quality);
        return base.Apply();
      }
    }
  }
}

