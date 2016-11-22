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

using System.IO;
using System.Linq;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.Instances.DataAnalysis;
using HeuristicLab.Scripting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class GridSearchRFRegressionScriptTest {
    private const string ScriptFileName = "GridSearch_RF_Regression_Script";
    private const string ScriptItemName = "Grid Search Random Forest Script - Regression";
    private const string ScriptItemDescription = "A script that runs a grid search for random forest parameters for solving symbolic regression problems";
    private const string ProblemInstanceName = "Keijzer 3 f(x) = 0.3 * x *sin(2 * PI * x); Interval [-3, 3]";
    private const string ProblemInstanceDataVaribleName = "problem";
    private const string BestSolutionVariableName = "bestSolution";

    [TestMethod]
    [TestCategory("Scripts.Create")]
    [TestProperty("Time", "short")]
    public void CreateGridSearchRFRegressionScriptTest() {
      var script = CreateGridSearchRFRegressionScript();
      string path = Path.Combine(ScriptingUtils.ScriptsDirectory, ScriptFileName + ScriptingUtils.ScriptFileExtension);
      XmlGenerator.Serialize(script, path);
    }

    [TestMethod]
    [TestCategory("Scripts.Execute")]
    [TestProperty("Time", "long")]
    public void RunGridSearchRFRegressionScriptTest() {
      var script = CreateGridSearchRFRegressionScript();

      script.Compile();
      ScriptingUtils.RunScript(script);

      var bestSolution = ScriptingUtils.GetVariable<IRegressionSolution>(script, BestSolutionVariableName);
      Assert.AreEqual(0.968329534139836, bestSolution.TrainingRSquared, 1E-8);
      Assert.AreEqual(0.982380790563445, bestSolution.TestRSquared, 1E-8);
    }

    private CSharpScript CreateGridSearchRFRegressionScript() {
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
      script.Code = ScriptSources.GridSearchRFRegressionScriptSource;
      #endregion
      return script;
    }
  }
}
