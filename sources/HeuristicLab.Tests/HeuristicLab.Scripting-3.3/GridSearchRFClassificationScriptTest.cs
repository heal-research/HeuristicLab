#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class GridSearchRFClassificationScriptTest {
    private const string ScriptFileName = "GridSearch_RF_Classification_Script";
    private const string ScriptItemName = "Grid Search Random Forest Script - Classification";
    private const string ScriptItemDescription = "A script that runs a grid search for random forest parameters for solving symbolic classification problems";
    private const string ProblemInstanceName = "Mammography, M. Elter, 2007";
    private const string ProblemInstanceDataVaribleName = "problem";
    private const string BestSolutionVariableName = "bestSolution";

    [TestMethod]
    [TestCategory("Scripts.Create")]
    [TestProperty("Time", "short")]
    public void CreateGridSearchRFClassificationScriptTest() {
      var script = CreateGridSearchRFClassificationScript();
      string path = Path.Combine(ScriptingUtils.ScriptsDirectory, ScriptFileName + ScriptingUtils.ScriptFileExtension);
      XmlGenerator.Serialize(script, path);
    }

    [TestMethod]
    [TestCategory("Scripts.Execute")]
    [TestProperty("Time", "long")]
    public void RunGridSearchRFClassificationScriptTest() {
      var script = CreateGridSearchRFClassificationScript();

      script.Compile();
      ScriptingUtils.RunScript(script);

      var bestSolution = ScriptingUtils.GetVariable<IClassificationSolution>(script, BestSolutionVariableName);
      Assert.AreEqual(0.85179407176287, bestSolution.TrainingAccuracy, 1E-8);
      Assert.AreEqual(0.81875, bestSolution.TestAccuracy, 1E-8);
    }

    private CSharpScript CreateGridSearchRFClassificationScript() {
      var script = new CSharpScript {
        Name = ScriptItemName,
        Description = ScriptItemDescription
      };
      #region Variables
      var provider = new UCIInstanceProvider();
      var instance = (UCIDataDescriptor)provider.GetDataDescriptors().Single(x => x.Name == ProblemInstanceName);
      var data = provider.LoadData(instance);
      script.VariableStore.Add(ProblemInstanceDataVaribleName, data);
      #endregion
      #region Code
      script.Code = ScriptSources.GridSearchRFClassificationScriptSource;
      #endregion
      return script;
    }
  }
}
