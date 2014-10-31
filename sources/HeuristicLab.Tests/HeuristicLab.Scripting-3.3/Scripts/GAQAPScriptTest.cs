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
using HeuristicLab.Problems.Instances.QAPLIB;
using HeuristicLab.Problems.QuadraticAssignment;
using HeuristicLab.Scripting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class GAQAPScriptTest {
    private const string ScriptFileName = "GA_QAP_Script";
    private const string ScriptItemName = "Genetic Algorithm Script - QAP";
    private const string ScriptItemDescription = "A scripted genetic algorithm which solves the \"" + ProblemInstanceName + "\" quadratic assignment problem (imported from Drezner)";
    private const string ProblemInstanceName = "dre56";
    private const string BestQualityVariableName = "bestQuality";

    [TestMethod]
    [TestCategory("Scripts.Create")]
    [TestProperty("Time", "short")]
    public void CreateGAQAPScriptScriptTest() {
      var script = CreateGAQAPScript();
      string path = Path.Combine(ScriptingUtils.ScriptsDirectory, ScriptFileName + ScriptingUtils.ScriptFileExtension);
      XmlGenerator.Serialize(script, path);
    }

    [TestMethod]
    [TestCategory("Scripts.Execute")]
    [TestProperty("Time", "long")]
    public void RunGAQAPScriptTest() {
      var script = CreateGAQAPScript();

      script.Compile();
      ScriptingUtils.RunScript(script);

      var bestQuality = ScriptingUtils.GetVariable<double>(script, BestQualityVariableName);
      Assert.AreEqual(2410.0, bestQuality, 1E-8);
    }

    private CSharpScript CreateGAQAPScript() {
      var script = new CSharpScript {
        Name = ScriptItemName,
        Description = ScriptItemDescription
      };
      #region Variables
      var provider = new DreznerQAPInstanceProvider();
      var instance = provider.GetDataDescriptors().Single(x => x.Name == ProblemInstanceName);
      var data = provider.LoadData(instance);
      var problem = new QuadraticAssignmentProblem();
      problem.Load(data);
      script.VariableStore.Add(ProblemInstanceName, problem);
      #endregion
      #region Code
      script.Code = ScriptingUtils.LoadScriptCodeFromFile(ScriptFileName);
      #endregion
      return script;
    }
  }
}
