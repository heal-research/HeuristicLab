#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Services.OKB.DataAccess;

namespace HeuristicLab.Services.OKB.DataAccess_33.Tests {
  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestClass]
  public class UnitTest {
    public UnitTest() {
      //
      // TODO: Add constructor logic here
      //
    }

    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext {
      get {
        return testContextInstance;
      }
      set {
        testContextInstance = value;
      }
    }

    #region Additional test attributes
    //
    // You can use the following additional attributes as you write your tests:
    //
    // Use ClassInitialize to run code before running the first test in the class
    // [ClassInitialize()]
    // public static void MyClassInitialize(TestContext testContext) { }
    //
    // Use ClassCleanup to run code after all tests in a class have run
    // [ClassCleanup()]
    // public static void MyClassCleanup() { }
    //
    // Use TestInitialize to run code before running each test 
    // [TestInitialize()]
    // public void MyTestInitialize() { }
    //
    // Use TestCleanup to run code after each test has run
    // [TestCleanup()]
    // public void MyTestCleanup() { }
    //
    #endregion

    [TestMethod]
    public void TestMethod1() {
      using (OKBDataContext okb = new OKBDataContext()) {
        okb.AlgorithmParameterIntValues.DeleteAllOnSubmit(okb.AlgorithmParameterIntValues);
        okb.AlgorithmParameters.DeleteAllOnSubmit(okb.AlgorithmParameters);
        okb.Experiments.DeleteAllOnSubmit(okb.Experiments);
        okb.Algorithms.DeleteAllOnSubmit(okb.Algorithms);
        okb.Problems.DeleteAllOnSubmit(okb.Problems);
        okb.SolutionRepresentations.DeleteAllOnSubmit(okb.SolutionRepresentations);
        okb.ProblemClasses.DeleteAllOnSubmit(okb.ProblemClasses);
        okb.AlgorithmClasses.DeleteAllOnSubmit(okb.AlgorithmClasses);
        okb.SubmitChanges();
        AlgorithmClass ac = new AlgorithmClass() { Name = "AlgorithmClass" };
        ProblemClass pc = new ProblemClass() { Name = "ProblemClass" };
        SolutionRepresentation sr = new SolutionRepresentation() { Name = "SolutionRepresentation" };
        Algorithm a = new Algorithm() { Name = "Alg", AlgorithmClass = ac, PlatformId = 1 };
        Problem p = new Problem() { Name = "Prb", ProblemClass = pc, SolutionRepresentation = sr, PlatformId = 1 };
        Experiment e = new Experiment() { Algorithm = a, Problem = p };
        AlgorithmParameter ap = new AlgorithmParameter() { Name = "Param", Algorithm = a, DataTypeId = 1 };
        ap.AlgorithmParameterIntValues.Add(new AlgorithmParameterIntValue() { AlgorithmParameterId = 0, Experiment = e, Value = 23 });
        okb.AlgorithmClasses.InsertOnSubmit(ac);
        okb.SubmitChanges();
      }
    }
  }
}
