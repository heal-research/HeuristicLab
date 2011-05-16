#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab_33.Tests {
  [TestClass]
  public class CollectObjectGraphTest {

    private TestContext testContextInstance;
    public TestContext TestContext {
      get { return testContextInstance; }
      set { testContextInstance = value; }
    }

    [TestMethod]
    [DeploymentItem(@"GA_SymbReg.hl")]
    public void CollectGASample() {
      GeneticAlgorithm ga = (GeneticAlgorithm)XmlParser.Deserialize("GA_SymbReg.hl");

      Stopwatch watch = new Stopwatch();
      watch.Start();
      for (int i = 0; i < 1; i++)
        ga.GetObjectGraphObjects().Count();
      watch.Stop();

      var objects = ga.GetObjectGraphObjects().ToList();

      TestContext.WriteLine("Time elapsed {0}", watch.Elapsed);
      TestContext.WriteLine("Objects discovered: {0}", objects.Count());
      TestContext.WriteLine("HL objects discovered: {0}", objects.Where(o => o.GetType().Namespace.StartsWith("HeuristicLab")).Count());
      TestContext.WriteLine("");

      Dictionary<Type, List<object>> objs = new Dictionary<Type, List<object>>();
      foreach (object o in objects) {
        if (!objs.ContainsKey(o.GetType()))
          objs.Add(o.GetType(), new List<object>());
        objs[o.GetType()].Add(o);
      }

      foreach (string s in objects.Select(o => o.GetType().Namespace).Distinct().OrderBy(s => s)) {
        TestContext.WriteLine("{0}: {1}", s, objects.Where(o => o.GetType().Namespace == s).Count());
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
  }
}
