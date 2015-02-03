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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.PluginInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  /// <summary>
  /// Summary description for DeepCloneableCloningTest
  /// </summary>
  [TestClass]
  public class DeepCloneableCloningTest {

    [ClassInitialize]
    public static void MyClassInitialize(TestContext testContext) {
      PluginLoader.Assemblies.Any();
    }

    private TestContext testContextInstance;
    public TestContext TestContext {
      get { return testContextInstance; }
      set { testContextInstance = value; }
    }

    public DeepCloneableCloningTest() {
      excludedTypes = new HashSet<Type>();
      excludedTypes.Add(typeof(HeuristicLab.Problems.DataAnalysis.Dataset));
      excludedTypes.Add(typeof(HeuristicLab.Problems.TravelingSalesman.DistanceMatrix));
      excludedTypes.Add(typeof(HeuristicLab.Problems.DataAnalysis.ClassificationEnsembleSolution));
      excludedTypes.Add(typeof(HeuristicLab.Problems.DataAnalysis.RegressionEnsembleSolution));

      foreach (var symbolType in ApplicationManager.Manager.GetTypes(typeof(HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbol)))
        excludedTypes.Add(symbolType);
      foreach (var grammarType in ApplicationManager.Manager.GetTypes(typeof(HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.SymbolicExpressionGrammarBase)))
        excludedTypes.Add(grammarType);
    }

    private readonly HashSet<Type> excludedTypes;

    [TestMethod]
    [TestCategory("General")]
    [TestCategory("Essential")]
    [TestProperty("Time", "long")]
    public void TestCloningFinishedExperiment() {
      Experiment experiment = (Experiment)XmlParser.Deserialize(@"Test Resources\SamplesExperimentFinished.hl");

      Experiment clone = (Experiment)experiment.Clone(new Cloner());
      var intersections = CheckTotalInequality(experiment, clone).Where(x => x.GetType().FullName.StartsWith("HeuristicLab"));

      Assert.IsTrue(ProcessEqualObjects(experiment, intersections));
    }

    [TestMethod]
    [TestCategory("General")]
    [TestCategory("Essential")]
    [TestProperty("Time", "long")]
    public void TestCloningAllDeepCloneables() {
      PluginLoader.Assemblies.ToArray();
      bool success = true;
      foreach (Type deepCloneableType in ApplicationManager.Manager.GetTypes(typeof(IDeepCloneable))) {
        // skip types that explicitely choose not to deep-clone every member
        if (excludedTypes.Contains(deepCloneableType)) continue;
        // test only types contained in HL plugin assemblies
        if (!PluginLoader.Assemblies.Contains(deepCloneableType.Assembly)) continue;
        // test only instantiable types
        if (deepCloneableType.IsAbstract || !deepCloneableType.IsClass) continue;

        IDeepCloneable item = null;
        try {
          item = (IDeepCloneable)Activator.CreateInstance(deepCloneableType, nonPublic: false);
        }
        catch { continue; } // no default constructor

        IDeepCloneable clone = null;
        try {
          clone = (IDeepCloneable)item.Clone(new Cloner());
        }
        catch (Exception e) {
          TestContext.WriteLine(Environment.NewLine + deepCloneableType.FullName + ":");
          TestContext.WriteLine("ERROR! " + e.GetType().Name + @" was thrown during cloning.
All IDeepCloneable items with a default constructor should be cloneable when using that constructor!");
          success = false;
          continue;
        }
        var intersections = CheckTotalInequality(item, clone).Where(x => x.GetType().FullName.StartsWith("HeuristicLab"));
        if (!intersections.Any()) continue;

        if (!ProcessEqualObjects(item, intersections))
          success = false;
      }
      Assert.IsTrue(success, "There are potential errors in deep cloning objects.");
    }

    private IEnumerable<object> CheckTotalInequality(object original, object clone) {
      var originalObjects = new HashSet<object>(original.GetObjectGraphObjects(excludeStaticMembers: true).Where(x => !x.GetType().IsValueType), new ReferenceEqualityComparer());
      var clonedObjects = new HashSet<object>(clone.GetObjectGraphObjects(excludeStaticMembers: true).Where(x => !x.GetType().IsValueType), new ReferenceEqualityComparer());

      return originalObjects.Intersect(clonedObjects, new ReferenceEqualityComparer());
    }

    private bool ProcessEqualObjects(IDeepCloneable item, IEnumerable<object> intersections) {
      bool success = true;
      bool headerWritten = false;

      foreach (object o in intersections) {
        string typeName = o.GetType().FullName;
        if (excludedTypes.Contains(o.GetType())) {
          //TestContext.WriteLine("Skipping excluded type " + typeName);
        } else if (o is IDeepCloneable) {
          string info = (o is IItem) ? ((IItem)o).ItemName + ((o is INamedItem) ? ", " + ((INamedItem)o).Name : String.Empty) : String.Empty;
          if (!headerWritten) {
            TestContext.WriteLine(Environment.NewLine + item.GetType().FullName + ":");
            headerWritten = true;
          }
          TestContext.WriteLine("POTENTIAL ERROR! A DEEPCLONEABLE WAS NOT DEEP CLONED (" + info + "): " + typeName);
          success = false;
        } else {
          Array array = o as Array;
          if (array != null && array.Length == 0) continue; //arrays of length 0 are used inside empty collections
          if (!headerWritten) {
            TestContext.WriteLine(Environment.NewLine + item.GetType().FullName + ":");
            headerWritten = true;
          }
          TestContext.WriteLine("WARNING: An object of type " + typeName + " is referenced in the original and in the clone.");
        }
      }
      return success;
    }
  }
}
