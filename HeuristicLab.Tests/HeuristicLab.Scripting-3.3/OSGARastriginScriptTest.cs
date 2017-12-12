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
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Scripting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class OSGARastriginScriptTest {
    private const string ScriptFileName = "OSGA_Rastrigin_Script";
    private const string ScriptItemName = "Offspring Selection Genetic Algorithm Script - Rastrigin";
    private const string ScriptItemDescription = "A scripted offspring selection genetic algorithm that solves the 100-dimensional Rastrigin test function";
    private const string LowerBoundVariableName = "minX";
    private const string UpperBoundVariableName = "maxX";
    private const string DimensionsVariableName = "N";
    private const string SeedVariableName = "seed";
    private const string BestQualityVariableName = "bestFitness";

    [TestMethod]
    [TestCategory("Scripts.Create")]
    [TestProperty("Time", "short")]
    public void CreateOSGARastriginScriptTest() {
      var script = CreateOSGARastriginScript();
      string path = Path.Combine(ScriptingUtils.ScriptsDirectory, ScriptFileName + ScriptingUtils.ScriptFileExtension);
      XmlGenerator.Serialize(script, path);
    }

    [TestMethod]
    [TestCategory("Scripts.Execute")]
    [TestProperty("Time", "long")]
    public void RunOSGARastriginScriptTest() {
      var script = CreateOSGARastriginScript();

      script.Compile();
      ScriptingUtils.RunScript(script);

      var bestQuality = ScriptingUtils.GetVariable<double>(script, BestQualityVariableName);
      Assert.AreEqual(0.176350329149955, bestQuality, 1E-8);
    }

    private CSharpScript CreateOSGARastriginScript() {
      var script = new CSharpScript {
        Name = ScriptItemName,
        Description = ScriptItemDescription
      };
      #region Variables
      script.VariableStore.Add(LowerBoundVariableName, new DoubleValue(-5.12));
      script.VariableStore.Add(UpperBoundVariableName, new DoubleValue(5.12));
      script.VariableStore.Add(DimensionsVariableName, new IntValue(100));
      script.VariableStore.Add(SeedVariableName, new IntValue(0));
      #endregion
      #region Code
      script.Code = ScriptSources.OSGARastriginScriptSource;
      #endregion
      return script;
    }
  }
}
