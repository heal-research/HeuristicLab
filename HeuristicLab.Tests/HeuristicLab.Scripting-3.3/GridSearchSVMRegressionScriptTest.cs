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

using System.IO;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.Instances.DataAnalysis;
using HeuristicLab.Scripting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class GridSearchSVMRegressionScriptTest {
    private const string ScriptFileName = "GridSearch_SVM_Regression_Script";
    private const string ScriptItemName = "Grid Search SVM Script - Regression";
    private const string ScriptItemDescription = "A script that runs a grid search for SVM parameters for solving symbolic regression problems";
    private const string ProblemInstanceName = "Keijzer 9 f(x) = arcsinh(x)  i.e. ln(x + sqrt(x² + 1))";
    private const string ProblemInstanceDataVaribleName = "problem";
    private const string BestSolutionVariableName = "bestSolution";

    private static readonly ProtoBufSerializer serializer = new ProtoBufSerializer();

    [TestMethod]
    [TestCategory("Scripts.Create")]
    [TestProperty("Time", "short")]
    public void CreateGridSearchSVMRegressionScriptTest() {
      var script = CreateGridSearchSVMRegressionScript();
      string path = Path.Combine(ScriptingUtils.ScriptsDirectory, ScriptFileName + ScriptingUtils.ScriptFileExtension);
      serializer.Serialize(script, path);
    }

    [TestMethod]
    [TestCategory("Scripts.Execute")]
    [TestProperty("Time", "medium")]
    public void RunGridSearchSVMRegressionScriptTest() {
      var script = CreateGridSearchSVMRegressionScript();

      script.Compile();
      ScriptingUtils.RunScript(script);

      var bestSolution = ScriptingUtils.GetVariable<IRegressionSolution>(script, BestSolutionVariableName);
      Assert.AreEqual(0.982485852864274, bestSolution.TrainingRSquared, 1E-8);
      Assert.AreEqual(0.98817480950295, bestSolution.TestRSquared, 1E-8);
    }

    private CSharpScript CreateGridSearchSVMRegressionScript() {
      var script = new CSharpScript {
        Name = ScriptItemName,
        Description = ScriptItemDescription
      };
      #region Variables
      var provider = new KeijzerInstanceProvider();
      var instance = (ArtificialRegressionDataDescriptor)provider.GetDataDescriptors().Single(x => x.Name == ProblemInstanceName);
      var data = instance.GenerateRegressionData();
      script.VariableStore.Add(ProblemInstanceDataVaribleName, data);
      #endregion
      #region Code
      script.Code = ScriptSources.GridSearchSVMRegressionScriptSource;
      #endregion
      return script;
    }
  }
}
