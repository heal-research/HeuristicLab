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
  public class GridSearchSVMClassificationScriptTest {
    private const string ScriptFileName = "GridSearch_SVM_Classification_Script";
    private const string ScriptItemName = "Grid Search SVM Script - Classification";
    private const string ScriptItemDescription = "A script that runs a grid search for SVM parameters for solving symbolic classification problems";
    private const string ProblemInstanceName = "Mammography, M. Elter, 2007";
    private const string ProblemInstanceDataVaribleName = "problem";
    private const string BestSolutionVariableName = "bestSolution";

    private static readonly ProtoBufSerializer serializer = new ProtoBufSerializer();

    [TestMethod]
    [TestCategory("Scripts.Create")]
    [TestProperty("Time", "short")]
    public void CreateGridSearchSVMClassificationScriptTest() {
      var script = CreateGridSearchSVMClassificationScript();
      string path = Path.Combine(ScriptingUtils.ScriptsDirectory, ScriptFileName + ScriptingUtils.ScriptFileExtension);
      serializer.Serialize(script, path);
    }

    [TestMethod]
    [TestCategory("Scripts.Execute")]
    [TestProperty("Time", "medium")]
    public void RunGridSearchSVMClassificationScriptTest() {
      var script = CreateGridSearchSVMClassificationScript();

      script.Compile();
      ScriptingUtils.RunScript(script);

      var bestSolution = ScriptingUtils.GetVariable<IClassificationSolution>(script, BestSolutionVariableName);
      Assert.AreEqual(0.819032761310452, bestSolution.TrainingAccuracy, 1E-8);
      Assert.AreEqual(0.721875, bestSolution.TestAccuracy, 1E-8);
    }

    private CSharpScript CreateGridSearchSVMClassificationScript() {
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
      script.Code = ScriptSources.GridSearchSVMClassificationScriptSource;
      #endregion
      return script;
    }
  }
}
