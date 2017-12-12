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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Common;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.TestFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class CollectObjectGraphTest {

    private TestContext testContextInstance;
    public TestContext TestContext {
      get { return testContextInstance; }
      set { testContextInstance = value; }
    }

    [TestMethod]
    [Description("Verify that the object graph traversal is working by checking the number of objects after traversal.")]
    [TestCategory("General")]
    [TestCategory("Essential")]
    [TestProperty("Time", "medium")]
    public void TestObjectGraphTraversal() {
      GeneticAlgorithm ga = (GeneticAlgorithm)XmlParser.Deserialize(@"Test Resources\GA_SymbReg.hl");
      var objects = ga.GetObjectGraphObjects().ToList();

      // Should be 3982, but count may change slightly as members are added or removed
      Assert.IsTrue(objects.Count > 1, "Number of objects in the object graph seems to small.");
    }

    [TestMethod]
    [TestCategory("General")]
    [TestCategory("Essential")]
    [TestProperty("Time", "medium")]
    public void CollectGASample() {
      GeneticAlgorithm ga = (GeneticAlgorithm)XmlParser.Deserialize(@"Test Resources\GA_SymbReg.hl");

      Stopwatch watch = new Stopwatch();
      watch.Start();
      for (int i = 0; i < 1; i++)
        ga.GetObjectGraphObjects().Count();
      watch.Stop();

      var objects = ga.GetObjectGraphObjects().ToList();

      TestContext.WriteLine("Time elapsed {0}", watch.Elapsed);
      TestContext.WriteLine("Objects discovered: {0}", objects.Count());
      TestContext.WriteLine("HL objects discovered: {0}", objects.Count(o => o.GetType().Namespace.StartsWith("HeuristicLab")));
      TestContext.WriteLine("");

      Dictionary<Type, List<object>> objs = new Dictionary<Type, List<object>>();
      foreach (object o in objects) {
        if (!objs.ContainsKey(o.GetType()))
          objs.Add(o.GetType(), new List<object>());
        objs[o.GetType()].Add(o);
      }

      foreach (string s in objects.Select(o => o.GetType().Namespace).Distinct().OrderBy(s => s)) {
        TestContext.WriteLine("{0}: {1}", s, objects.Count(o => o.GetType().Namespace == s));
      }
      TestContext.WriteLine("");


      TestContext.WriteLine("Analysis of contained objects per name");
      foreach (var pair in objs.OrderBy(x => x.Key.ToString())) {
        TestContext.WriteLine("{0}: {1}", pair.Key, pair.Value.Count);
      }
      TestContext.WriteLine("");

      TestContext.WriteLine("Analysis of contained objects");
      foreach (var pair in from o in objs orderby o.Value.Count descending select o) {
        TestContext.WriteLine("{0}: {1}", pair.Key, pair.Value.Count);
      }
      TestContext.WriteLine("");
    }

    /// <summary>
    /// Tests if performance of multiple executions of a GA stays constant (as discussed in #1424)
    /// Tests if object collection works after multiple executions of a GA 
    /// (for example the traversal of `ThreadLocal` objects in CollectObjectGraphObjects 
    /// causes a StackOverflow occurs after some executions)
    /// </summary>
    [TestMethod]
    [TestCategory("General")]
    [TestProperty("Time", "long")]
    public void AlgorithmExecutions() {
      var algs = new List<IAlgorithm>();

      Stopwatch sw = new Stopwatch();
      for (int i = 0; i < 100; i++) {
        GeneticAlgorithm ga = new GeneticAlgorithm();
        ga.PopulationSize.Value = 5;
        ga.MaximumGenerations.Value = 5;
        ga.Engine = new SequentialEngine.SequentialEngine();
        ga.Problem = new SingleObjectiveTestFunctionProblem();

        sw.Start();
        algs.Add(ga);

        ga.Start();
        sw.Stop();
        TestContext.WriteLine("{0}: {1} ", i, sw.Elapsed);
        sw.Reset();
      }
    }

    /// <summary>
    /// Test the execution of many algorithms in parallel
    /// </summary>
    [TestMethod]
    [TestCategory("General")]
    [TestProperty("Time", "medium")]
    public void ParallelAlgorithmExecutions() {
      int n = 60;
      var tasks = new Task[n];

      TestContext.WriteLine("creating tasks...");
      for (int i = 0; i < n; i++) {
        tasks[i] = new Task((iobj) => {
          int locali = (int)iobj;
          GeneticAlgorithm ga = new GeneticAlgorithm();
          ga.Name = "Alg " + locali;
          ga.PopulationSize.Value = 5;
          ga.MaximumGenerations.Value = 5;
          ga.Engine = new SequentialEngine.SequentialEngine();
          ga.Problem = new SingleObjectiveTestFunctionProblem();
          ga.Prepare(true);
          Console.WriteLine("{0}; Objects before execution: {1}", ga.Name, ga.GetObjectGraphObjects().Count());
          var sw = new Stopwatch();
          sw.Start();
          ga.Start();
          sw.Stop();
          Console.WriteLine("{0}; Objects after execution: {1}", ga.Name, ga.GetObjectGraphObjects().Count());
          Console.WriteLine("{0}; ExecutionTime: {1} ", ga.Name, sw.Elapsed);
        }, i);
      }
      TestContext.WriteLine("starting tasks...");
      for (int i = 0; i < n; i++) {
        tasks[i].Start();
      }
      TestContext.WriteLine("waiting for tasks to finish...");
      Task.WaitAll(tasks);
    }
  }
}
