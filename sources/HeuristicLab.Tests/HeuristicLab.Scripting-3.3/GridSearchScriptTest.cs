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

using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimizer;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Scripting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class GridSearchScriptTest {
    private const string PathPrefix = "HeuristicLab.Optimizer.Documents.";
    private const string PathSuffix = ".hl";
    private const string SvmClassificationScriptName = "GridSearch_SVM_Classification";
    private const string SvmRegressionScriptName = "GridSearch_SVM_Regression";
    private const string RandomForestRegressionScriptName = "GridSearch_RF_Regression";
    private const string RandomForestClassificationScriptName = "GridSearch_RF_Classification";
    private const string SamplesDirectory = SamplesUtils.Directory;

    private readonly ManualResetEvent manualResetEvent = new ManualResetEvent(false);

    [ClassInitialize]
    public static void MyClassInitialize(TestContext testContext) {
      PluginLoader.Assemblies.Any();
      if (!Directory.Exists(SamplesDirectory))
        Directory.CreateDirectory(SamplesDirectory);
    }

    [TestMethod]
    [TestCategory("Scripting")]
    public void RunRandomForestRegressionScriptTest() {
      var assembly = new StartPage().GetType().Assembly;
      const string name = PathPrefix + RandomForestRegressionScriptName + PathSuffix;
      var script = (CSharpScript)LoadSample(name, assembly);

      try {
        script.Compile();
      }
      catch {
        if (script.CompileErrors.HasErrors) {
          ShowCompilationResults(script);
          throw new Exception("Compilation failed.");
        } else {
          Console.WriteLine("Compilation succeeded.");
        }
      }
      finally {
        script.ScriptExecutionFinished += script_ExecutionFinished;
        script.Execute();
        var vs = script.VariableStore;
        var solution = (IRegressionSolution)vs["demo_bestSolution"];
        Assert.IsTrue(solution.TrainingRSquared.IsAlmost(1));
      }
    }

    [TestMethod]
    [TestCategory("Scripting")]
    public void RunRandomForestClassificationScriptTest() {
      var assembly = new StartPage().GetType().Assembly;
      const string name = PathPrefix + RandomForestClassificationScriptName + PathSuffix;
      var script = (CSharpScript)LoadSample(name, assembly);

      try {
        script.Compile();
      }
      catch {
        if (script.CompileErrors.HasErrors) {
          ShowCompilationResults(script);
          throw new Exception("Compilation failed.");
        } else {
          Console.WriteLine("Compilation succeeded.");
        }
      }
      finally {
        script.ScriptExecutionFinished += script_ExecutionFinished;
        script.Execute();
        var vs = script.VariableStore;
        var solution = (IClassificationSolution)vs["demo_bestSolution"];
        Assert.IsTrue(solution.TrainingAccuracy.IsAlmost(1) && solution.TestAccuracy.IsAlmost(0.953125));
      }
    }

    [TestMethod]
    [TestCategory("Scripting")]
    public void RunSvmRegressionScriptTest() {
      var assembly = new StartPage().GetType().Assembly;
      const string name = PathPrefix + SvmRegressionScriptName + PathSuffix;
      var script = (CSharpScript)LoadSample(name, assembly);

      try {
        script.Compile();
      }
      catch {
        if (script.CompileErrors.HasErrors) {
          ShowCompilationResults(script);
          throw new Exception("Compilation failed.");
        } else {
          Console.WriteLine("Compilation succeeded.");
        }
      }
      finally {
        script.ScriptExecutionFinished += script_ExecutionFinished;
        script.Execute();
        var vs = script.VariableStore;
        var solution = (IRegressionSolution)vs["demo_bestSolution"];
        Assert.IsTrue(solution.TrainingRSquared.IsAlmost(0.066221959224331) && solution.TestRSquared.IsAlmost(0.0794407638195883));
      }
    }

    [TestMethod]
    [TestCategory("Scripting")]
    public void RunSvmClassificationScriptTest() {
      var assembly = new StartPage().GetType().Assembly;
      const string name = PathPrefix + SvmClassificationScriptName + PathSuffix;
      var script = (CSharpScript)LoadSample(name, assembly);

      try {
        script.Compile();
      }
      catch {
        if (script.CompileErrors.HasErrors) {
          ShowCompilationResults(script);
          throw new Exception("Compilation failed.");
        } else {
          Console.WriteLine("Compilation succeeded.");
        }
      }
      finally {
        script.ScriptExecutionFinished += script_ExecutionFinished;
        script.Execute();
        manualResetEvent.WaitOne();
        var vs = script.VariableStore;
        var solution = (IClassificationSolution)vs["demo_bestSolution"];
        Assert.IsTrue(solution.TrainingAccuracy.IsAlmost(0.817472698907956) && solution.TestAccuracy.IsAlmost(0.809375));
      }
    }

    #region Helpers
    private static void ShowCompilationResults(Script script) {
      if (script.CompileErrors.Count == 0) return;
      var msgs = script.CompileErrors.OfType<CompilerError>()
                                      .OrderBy(x => x.IsWarning)
                                      .ThenBy(x => x.Line)
                                      .ThenBy(x => x.Column);
      foreach (var m in msgs) {
        Console.WriteLine(m);
      }
    }

    private INamedItem LoadSample(string name, Assembly assembly) {
      string path = Path.GetTempFileName();
      INamedItem item = null;
      try {
        using (var stream = assembly.GetManifestResourceStream(name)) {
          WriteStreamToTempFile(stream, path); // create a file in a temporary folder (persistence cannot load these files directly from the stream)
          item = XmlParser.Deserialize<INamedItem>(path);
        }
      }
      catch (Exception) {
      }
      finally {
        if (File.Exists(path)) {
          File.Delete(path); // make sure we remove the temporary file
        }
      }
      return item;
    }

    private void WriteStreamToTempFile(Stream stream, string path) {
      using (FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write)) {
        stream.CopyTo(output);
      }
    }

    private void script_ExecutionFinished(object sender, EventArgs a) {
      manualResetEvent.Set();
    }
    #endregion
  }
}
