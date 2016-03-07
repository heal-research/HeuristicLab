using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using HeuristicLab.Algorithms.DataAnalysis.MctsSymbolicRegression;
using HeuristicLab.Algorithms.DataAnalysis.MctsSymbolicRegression.Policies;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [TestClass()]
  public class MctsSymbolicRegressionTest {
    #region number of solutions
    // the algorithm should visits each solution only once
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegNumberOfSolutionsOneVariable() {
      // this problem has only one variable
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.NguyenInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("F1 ")));
      {
        // possible solutions with max one variable reference:
        // x
        // log(x)
        // exp(x)
        // 1/x
        TestMctsNumberOfSolutions(regProblem, 1, 4);
      }
      {
        // possible solutions with max two variable references:
        // TODO: equal terms should not be allowed (see ConstraintHandler)
        // x
        // log(x)
        // exp(x)
        // 1/x
        //              -- 4
        // x * x
        // x * log(x)
        // x * exp(x)
        // x * 1/x
        // x + x                                        ?
        // x + log(x)
        // x + exp(x)
        // x + 1/x
        //              -- 8
        // log(x) * log(x)
        // log(x) * exp(x)
        // log(x) * 1/x
        // log(x) + log(x)                              ?
        // log(x) + exp(x)                              ?
        // log(x) + 1/x
        //              -- 6
        // exp(x) * exp(x)
        // exp(x) * 1/x
        // exp(x) + exp(x)                              ?
        // exp(x) + 1/x
        //              -- 4
        // 1/x * 1/x
        // 1/x + 1/x                                    ?
        //              -- 2
        // log(x+x)                                     ?
        // log(x*x)
        // exp(x*x)
        // 1/(x+x)                                      ?
        // 1/(x*x)
        //              -- 5


        TestMctsNumberOfSolutions(regProblem, 2, 29);
      }
      {
        // possible solutions with max three variable references:
        // without log and inv
        // x
        // exp(x)
        //              -- 2
        // x * x
        // x + x                                            ?
        // x * exp(x)
        // x + exp(x)
        // exp(x) * exp(x)
        // exp(x) + exp(x)                                  ?
        // exp(x*x)
        //              -- 7
        // x * x * x
        // x + x * x                                       
        // x + x + x                                        ?
        // x * x * exp(x)
        // x + x * exp(x)                                   
        // x + x + exp(x)                                   ?
        // exp(x) + x*x
        // exp(x) + x*exp(x)                                
        // x + exp(x) * exp(x)                              
        // x + exp(x) + exp(x)                              ?
        // x * exp(x) * exp(x)
        // x * exp(x*x)
        // x + exp(x*x)
        //              -- 13

        // exp(x) * exp(x) * exp(x)
        // exp(x) + exp(x) * exp(x)                         
        // exp(x) + exp(x) + exp(x)                         ?
        //              -- 3

        // exp(x)   * exp(x*x)
        // exp(x)   + exp(x*x)
        //              -- 2
        // exp(x*x*x)
        //              -- 1
        TestMctsNumberOfSolutions(regProblem, 3, 2 + 7 + 13 + 3 + 2 + 1, allowLog: false, allowInv: false);
      }
      {
        // possible solutions with max 4 variable references:
        // without exp, log and inv
        // x       
        // x*x
        // x+x                                             ?
        // x*x*x
        // x+x*x
        // x+x+x                                           ?
        // x*x*x*x
        // x+x*x*x
        // x*x+x*x                                         ?
        // x+x+x*x                                         ?
        // x+x+x+x                                         ?

        TestMctsNumberOfSolutions(regProblem, 4, 11, allowLog: false, allowInv: false, allowExp: false);
      }
      {
        // possible solutions with max 5 variable references:
        // without exp, log and inv
        // x       
        // xx
        // x+x                                             ?
        // xxx
        // x+xx
        // x+x+x                                           ?
        // xxxx
        // x+xxx
        // xx+xx                                           ?
        // x+x+xx                                          ?
        // x+x+x+x                                         ?
        // xxxxx 
        // x+xxxx
        // xx+xxx
        // x+x+xxx                                         ?
        // x+xx+xx                                         ?
        // x+x+x+xx                                        ?
        // x+x+x+x+x                                       ?
        TestMctsNumberOfSolutions(regProblem, 5, 18, allowLog: false, allowInv: false, allowExp: false);
      }
    }

    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegNumberOfSolutionsTwoVariables() {
      // this problem has only two input variables
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.NguyenInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("F9 ")));
      {
        // possible solutions with max one variable reference:
        // x
        // log(x)
        // exp(x)
        // 1/x
        // y
        // log(y)
        // exp(y)
        // 1/y
        TestMctsNumberOfSolutions(regProblem, 1, 8);
      }
      {
        // possible solutions with max one variable reference:
        // without log and inv

        // x
        // exp(x)
        // y
        // exp(y)
        TestMctsNumberOfSolutions(regProblem, 1, 4, allowLog: false, allowInv: false);
      }
      {
        // possible solutions with max two variable references:
        // without log and inv

        // x
        // y
        // exp(x)
        // exp(y)
        //                  -- 4
        // x (*|+) x
        // x (*|+) exp(x)
        // x (*|+) y
        // x (*|+) exp(y)
        //                  -- 8
        // exp(x) (*|+) exp(x)
        // exp(x) (*|+) exp(y)
        //                  -- 4
        // y (*|+) y
        // y (*|+) exp(x)
        // y (*|+) exp(y)
        //                  -- 6
        // exp(y) (*|+) exp(y)
        //                  -- 2
        //
        // exp(x*x)
        // exp(x*y)
        // exp(y*y)
        //                  -- 3

        TestMctsNumberOfSolutions(regProblem, 2, 4 + 8 + 4 + 6 + 2 + 3, allowLog: false, allowInv: false);
      }

      {
        // possible solutions with max two variable references:
        // without exp and sum
        // x
        // y
        // log(x)
        // log(y)
        // inv(x)
        // inv(y)
        //              -- 6
        // x * x
        // x * y
        // x * log(x)
        // x * log(y)
        // x * inv(x)
        // x * inv(y)
        //              -- 6
        // log(x) * log(x)
        // log(x) * log(y)
        // log(x) * inv(x)
        // log(x) * inv(y)
        //              -- 4
        // inv(x) * inv(x)
        // inv(x) * inv(y)
        //              -- 2
        // y * y
        // y * log(x)
        // y * log(y)
        // y * inv(x)
        // y * inv(y)
        //              -- 5
        // log(y) * log(y)
        // log(y) * inv(x)
        // log(y) * inv(y)
        //              -- 3
        // inv(y) * inv(y)
        //              -- 1
        // log(x*x)
        // log(x*y)
        // log(y*y)

        // inv(x*x)
        // inv(x*y)
        // inv(y*y)
        //             -- 6
        // log(x+x)
        // log(x+y)
        // log(y+y)

        // inv(x+x)
        // inv(x+y)
        // inv(y+y)
        //             -- 6
        TestMctsNumberOfSolutions(regProblem, 2, 6 + 6 + 4 + 2 + 5 + 3 + 1 + 6 + 6, allowExp: false, allowSum: false);
      }
    }
    #endregion


    #region Nguyen
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkNguyen1() {
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.NguyenInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("F1 ")));
      TestMcts(regProblem);
    }
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkNguyen2() {
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.NguyenInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("F2 ")));
      TestMcts(regProblem);
    }
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkNguyen3() {
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.NguyenInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("F3 ")));
      TestMcts(regProblem, successThreshold: 0.99);
    }
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkNguyen4() {
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.NguyenInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("F4 ")));
      TestMcts(regProblem);
    }
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkNguyen5() {
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.NguyenInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("F5 ")));
      TestMcts(regProblem);
    }
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkNguyen6() {
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.NguyenInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("F6 ")));
      TestMcts(regProblem);
    }
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkNguyen7() {
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.NguyenInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("F7 ")));
      TestMcts(regProblem);
    }
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkNguyen8() {
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.NguyenInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("F8 ")));
      TestMcts(regProblem, successThreshold: 0.99);
    }
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkNguyen9() {
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.NguyenInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("F9 ")));
      TestMcts(regProblem, iterations: 10000);
    }
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkNguyen10() {
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.NguyenInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("F10 ")));
      TestMcts(regProblem);
    }
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkNguyen11() {
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.NguyenInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("F11 ")));
      TestMcts(regProblem, 10000, 0.95); // cannot solve exactly in 10000 iterations
    }
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkNguyen12() {
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.NguyenInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("F12 ")));
      TestMcts(regProblem, iterations: 10000, successThreshold: 0.99, allowLog: false, allowExp: false, allowInv: false);
    }

    #endregion

    #region keijzer
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "long")]
    public void MctsSymbRegBenchmarkKeijzer5() {
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.KeijzerInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("Keijzer 5 f(")));
      // some Keijzer problem instances have very large test partitions (here we are not concerened about test performance)
      if (regProblem.TestPartition.End - regProblem.TestPartition.Start > 1000) regProblem.TestPartition.End = regProblem.TestPartition.Start + 1000;
      TestMcts(regProblem, iterations: 10000, allowExp: false, allowLog: false, allowSum: false, successThreshold: 0.99);
    }

    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkKeijzer6() {
      // Keijzer 6 f(x) = Sum(1 / i) From 1 to X
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.KeijzerInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("Keijzer 6 f(")));
      // some Keijzer problem instances have very large test partitions (here we are not concerened about test performance)
      if (regProblem.TestPartition.End - regProblem.TestPartition.Start > 1000) regProblem.TestPartition.End = regProblem.TestPartition.Start + 1000;
      TestMcts(regProblem, allowLog: false, allowExp: false, successThreshold: 0.995); // cannot solve
    }

    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkKeijzer7() {
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.KeijzerInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("Keijzer 7 f(")));
      // some Keijzer problem instances have very large test partitions (here we are not concerened about test performance)
      if (regProblem.TestPartition.End - regProblem.TestPartition.Start > 1000) regProblem.TestPartition.End = regProblem.TestPartition.Start + 1000;
      TestMcts(regProblem);
    }

    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkKeijzer8() {
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.KeijzerInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("Keijzer 8 f(")));
      // some Keijzer problem instances have very large test partitions (here we are not concerened about test performance)
      if (regProblem.TestPartition.End - regProblem.TestPartition.Start > 1000) regProblem.TestPartition.End = regProblem.TestPartition.Start + 1000;
      TestMcts(regProblem);
    }

    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkKeijzer9() {
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.KeijzerInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("Keijzer 9 f(")));
      // some Keijzer problem instances have very large test partitions (here we are not concerened about test performance)
      if (regProblem.TestPartition.End - regProblem.TestPartition.Start > 1000) regProblem.TestPartition.End = regProblem.TestPartition.Start + 1000;
      TestMcts(regProblem);
    }

    /*
     * cannot solve this yet x^y
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkKeijzer10() {
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.KeijzerInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("Keijzer 10 f(")));
      // some Keijzer problem instances have very large test partitions (here we are not concerened about test performance)
      if (regProblem.TestPartition.End - regProblem.TestPartition.Start > 1000) regProblem.TestPartition.End = regProblem.TestPartition.Start + 1000;
      TestMcts(regProblem, iterations: 10000, successThreshold: 0.99);
    }
     */

    /* cannot solve this yet
     * xy + sin( (x-1) (y-1) )
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkKeijzer11() {
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.KeijzerInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("Keijzer 11 f(")));
      // some Keijzer problem instances have very large test partitions (here we are not concerened about test performance)
      if (regProblem.TestPartition.End - regProblem.TestPartition.Start > 1000) regProblem.TestPartition.End = regProblem.TestPartition.Start + 1000;
      TestMcts(regProblem, successThreshold: 0.99); // cannot solve this yet
    }
     */
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkKeijzer12() {
      // sames a Nguyen 12
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.KeijzerInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("Keijzer 12 f(")));
      // some Keijzer problem instances have very large test partitions (here we are not concerened about test performance)
      if (regProblem.TestPartition.End - regProblem.TestPartition.Start > 1000) regProblem.TestPartition.End = regProblem.TestPartition.Start + 1000;
      TestMcts(regProblem, iterations: 10000, allowLog: false, allowExp: false, allowInv: false, successThreshold: 0.99); // cannot solve exactly in 10000 iterations
    }
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkKeijzer14() {
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.KeijzerInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("Keijzer 14 f(")));
      // some Keijzer problem instances have very large test partitions (here we are not concerened about test performance)
      if (regProblem.TestPartition.End - regProblem.TestPartition.Start > 1000) regProblem.TestPartition.End = regProblem.TestPartition.Start + 1000;
      TestMcts(regProblem);
    }
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void MctsSymbRegBenchmarkKeijzer15() {
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.KeijzerInstanceProvider();
      var regProblem = provider.LoadData(provider.GetDataDescriptors().Single(x => x.Name.Contains("Keijzer 15 f(")));
      // some Keijzer problem instances have very large test partitions (here we are not concerened about test performance)
      if (regProblem.TestPartition.End - regProblem.TestPartition.Start > 1000) regProblem.TestPartition.End = regProblem.TestPartition.Start + 1000;
      TestMcts(regProblem, iterations: 10000, allowLog: false, allowExp: false, allowInv: false);
    }
    #endregion

    private void TestMcts(IRegressionProblemData problemData, int iterations = 2000, double successThreshold = 0.999,
      bool allowExp = true,
      bool allowLog = true,
      bool allowInv = true,
      bool allowSum = true
      ) {
      var mctsSymbReg = new MctsSymbolicRegressionAlgorithm();
      var regProblem = new RegressionProblem();
      regProblem.ProblemDataParameter.Value = problemData;
      #region Algorithm Configuration
      mctsSymbReg.Problem = regProblem;
      mctsSymbReg.Iterations = iterations;
      mctsSymbReg.MaxVariableReferences = 10;
      var ucbPolicy = new Ucb();
      ucbPolicy.C = 2;
      mctsSymbReg.Policy = ucbPolicy;
      mctsSymbReg.SetSeedRandomly = false;
      mctsSymbReg.Seed = 1234;
      mctsSymbReg.AllowedFactors.SetItemCheckedState(mctsSymbReg.AllowedFactors.Single(s => s.Value.Contains("exp")), allowExp);
      mctsSymbReg.AllowedFactors.SetItemCheckedState(mctsSymbReg.AllowedFactors.Single(s => s.Value.Contains("log")), allowLog);
      mctsSymbReg.AllowedFactors.SetItemCheckedState(mctsSymbReg.AllowedFactors.Single(s => s.Value.Contains("1 /")), allowInv);
      mctsSymbReg.AllowedFactors.SetItemCheckedState(mctsSymbReg.AllowedFactors.Single(s => s.Value.Contains("sum of multiple")), allowSum);
      #endregion
      RunAlgorithm(mctsSymbReg);

      Console.WriteLine(mctsSymbReg.ExecutionTime);
      // R² >= 0.999
      // using arequal to get output of the actual value
      var eps = 1.0 - successThreshold;
      Assert.AreEqual(1.0, ((DoubleValue)mctsSymbReg.Results["Best solution quality (train)"].Value).Value, eps);
      Assert.AreEqual(1.0, ((DoubleValue)mctsSymbReg.Results["Best solution quality (test)"].Value).Value, eps);
    }

    private void TestMctsNumberOfSolutions(IRegressionProblemData problemData, int maxNumberOfVariables, int expectedNumberOfSolutions,
      bool allowProd = true,
      bool allowExp = true,
      bool allowLog = true,
      bool allowInv = true,
      bool allowSum = true
  ) {
      var mctsSymbReg = new MctsSymbolicRegressionAlgorithm();
      var regProblem = new RegressionProblem();
      regProblem.ProblemDataParameter.Value = problemData;
      #region Algorithm Configuration

      mctsSymbReg.SetSeedRandomly = false;
      mctsSymbReg.Seed = 1234;
      mctsSymbReg.Problem = regProblem;
      mctsSymbReg.Iterations = int.MaxValue; // stopping when all solutions have been enumerated
      mctsSymbReg.MaxVariableReferences = maxNumberOfVariables;
      var ucbPolicy = new Ucb();
      ucbPolicy.C = 1000; // essentially breadth first search
      mctsSymbReg.Policy = ucbPolicy;
      mctsSymbReg.AllowedFactors.SetItemCheckedState(mctsSymbReg.AllowedFactors.Single(s => s.Value.StartsWith("prod")), allowProd);
      mctsSymbReg.AllowedFactors.SetItemCheckedState(mctsSymbReg.AllowedFactors.Single(s => s.Value.Contains("exp")), allowExp);
      mctsSymbReg.AllowedFactors.SetItemCheckedState(mctsSymbReg.AllowedFactors.Single(s => s.Value.Contains("log")), allowLog);
      mctsSymbReg.AllowedFactors.SetItemCheckedState(mctsSymbReg.AllowedFactors.Single(s => s.Value.Contains("1 /")), allowInv);
      mctsSymbReg.AllowedFactors.SetItemCheckedState(mctsSymbReg.AllowedFactors.Single(s => s.Value.Contains("sum of multiple")), allowSum);
      #endregion
      RunAlgorithm(mctsSymbReg);

      Console.WriteLine(mctsSymbReg.ExecutionTime);
      Assert.AreEqual(expectedNumberOfSolutions, ((IntValue)mctsSymbReg.Results["Iterations"].Value).Value);
    }


    // same as in SamplesUtil
    private void RunAlgorithm(IAlgorithm a) {
      var trigger = new EventWaitHandle(false, EventResetMode.ManualReset);
      Exception ex = null;
      a.Stopped += (src, e) => { trigger.Set(); };
      a.ExceptionOccurred += (src, e) => { ex = e.Value; trigger.Set(); };
      a.Prepare();
      a.Start();
      trigger.WaitOne();

      Assert.AreEqual(ex, null);
    }

  }
}
