using System;
using System.Linq;
using System.Threading;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [TestClass()]
  public class GradientBoostingTest {
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void DecisionTreeTest() {
      {
        var xy = new double[,]
        {
          {-1, 20, 0},
          {-1, 20, 0},
          { 1, 10, 0},
          { 1, 10, 0},
        };
        var allVariables = new string[] { "y", "x1", "x2" };

        // x1 <= 15 -> 1 
        // x1 >  15 -> -1
        BuildTree(xy, allVariables, 10);
      }


      {
        var xy = new double[,]
        {
          {-1, 20,  1},
          {-1, 20, -1},
          { 1, 10, -1},
          { 1, 10, 1},
        };
        var allVariables = new string[] { "y", "x1", "x2" };

        // ignore irrelevant variables
        // x1 <= 15 -> 1 
        // x1 >  15 -> -1
        BuildTree(xy, allVariables, 10);
      }

      {
        // split must be by x1 first 
        var xy = new double[,]
        {
          {-2, 20,  1},
          {-1, 20, -1},
          { 1, 10, -1},
          { 2, 10, 1},
        };

        var allVariables = new string[] { "y", "x1", "x2" };

        // x1 <= 15 AND x2 <= 0 -> 1 
        // x1 <= 15 AND x2 >  0 -> 2 
        // x1 >  15 AND x2 <= 0 -> -1
        // x1 >  15 AND x2 >  0 -> -2
        BuildTree(xy, allVariables, 10);
      }

      {
        // averaging ys
        var xy = new double[,]
        {
          {-2.5, 20,  1},
          {-1.5, 20,  1},
          {-1.5, 20, -1},
          {-0.5, 20, -1},
          {0.5, 10, -1},
          {1.5, 10, -1},
          {1.5, 10, 1},
          {2.5, 10, 1},
        };

        var allVariables = new string[] { "y", "x1", "x2" };

        // x1 <= 15 AND x2 <= 0 -> 1 
        // x1 <= 15 AND x2 >  0 -> 2 
        // x1 >  15 AND x2 <= 0 -> -1
        // x1 >  15 AND x2 >  0 -> -2
        BuildTree(xy, allVariables, 10);
      }


      {
        // diagonal split (no split possible)
        var xy = new double[,]
        {
          { 1, 1, 1},
          {-1, 1, 2},
          {-1, 2, 1},
          { 1, 2, 2},
        };

        var allVariables = new string[] { "y", "x1", "x2" };

        // split cannot be found
        // -> 0.0
        BuildTree(xy, allVariables, 3);
      }
      {
        // almost diagonal split 
        var xy = new double[,]
        {
          { 1, 1, 1},
          {-1, 1, 2},
          {-1, 2, 1},
          { 1.0001, 2, 2},
        };

        var allVariables = new string[] { "y", "x1", "x2" };
        // (two possible solutions)
        // x2 <= 1.5 -> 0
        // x2 >  1.5 -> 0 (not quite)
        BuildTree(xy, allVariables, 3);

        // x1 <= 1.5 AND x2 <= 1.5 -> 1 
        // x1 <= 1.5 AND x2 >  1.5 -> -1 
        // x1 >  1.5 AND x2 <= 1.5 -> -1
        // x1 >  1.5 AND x2 >  1.5 -> 1 (not quite)
        BuildTree(xy, allVariables, 7);
      }
      {
        // unbalanced split
        var xy = new double[,]
        {
          {-1, 1, 1},
          {-1, 1, 2},
          {0.9, 2, 1},
          {1.1, 2, 2},
        };

        var allVariables = new string[] { "y", "x1", "x2" };
        // x1 <= 1.5 -> -1.0 
        // x1 >  1.5 AND x2 <= 1.5 -> 0.9
        // x1 >  1.5 AND x2 >  1.5 -> 1.1
        BuildTree(xy, allVariables, 10);
      }

      {
        // unbalanced split
        var xy = new double[,]
        {
          {-1, 1, 1},
          {-1, 1, 2},
          {-1, 2, 1},
          { 3, 2, 2},
        };

        var allVariables = new string[] { "y", "x1", "x2" };
        // (two possible solutions)
        // x2 <= 1.5 -> -1.0 
        // x2 >  1.5 AND x1 <= 1.5 -> -1.0
        // x2 >  1.5 AND x1 >  1.5 ->  3.0
        BuildTree(xy, allVariables, 10);
      }
    }

    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "long")]
    public void GradientBoostingTestTowerSquaredError() {
      var gbt = new GradientBoostedTreesAlgorithm();
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.RegressionRealWorldInstanceProvider();
      var instance = provider.GetDataDescriptors().Single(x => x.Name.Contains("Tower"));
      var regProblem = new RegressionProblem();
      regProblem.Load(provider.LoadData(instance));

      #region Algorithm Configuration
      gbt.Problem = regProblem;
      gbt.Seed = 0;
      gbt.SetSeedRandomly = false;
      gbt.Iterations = 5000;
      gbt.MaxSize = 20;
      gbt.CreateSolution = false;
      #endregion

      RunAlgorithm(gbt);

      Console.WriteLine(gbt.ExecutionTime);
      Assert.AreEqual(267.68704241153921, ((DoubleValue)gbt.Results["Loss (train)"].Value).Value, 1E-6);
      Assert.AreEqual(393.84704062205469, ((DoubleValue)gbt.Results["Loss (test)"].Value).Value, 1E-6);
    }

    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "long")]
    public void GradientBoostingTestTowerAbsoluteError() {
      var gbt = new GradientBoostedTreesAlgorithm();
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.RegressionRealWorldInstanceProvider();
      var instance = provider.GetDataDescriptors().Single(x => x.Name.Contains("Tower"));
      var regProblem = new RegressionProblem();
      regProblem.Load(provider.LoadData(instance));

      #region Algorithm Configuration
      gbt.Problem = regProblem;
      gbt.Seed = 0;
      gbt.SetSeedRandomly = false;
      gbt.Iterations = 1000;
      gbt.MaxSize = 20;
      gbt.Nu = 0.02;
      gbt.LossFunctionParameter.Value = gbt.LossFunctionParameter.ValidValues.First(l => l.ToString().Contains("Absolute"));
      gbt.CreateSolution = false;
      #endregion

      RunAlgorithm(gbt);

      Console.WriteLine(gbt.ExecutionTime);
      Assert.AreEqual(10.551385044666661, ((DoubleValue)gbt.Results["Loss (train)"].Value).Value, 1E-6);
      Assert.AreEqual(12.918001745581172, ((DoubleValue)gbt.Results["Loss (test)"].Value).Value, 1E-6);
    }

    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "long")]
    public void GradientBoostingTestTowerRelativeError() {
      var gbt = new GradientBoostedTreesAlgorithm();
      var provider = new HeuristicLab.Problems.Instances.DataAnalysis.RegressionRealWorldInstanceProvider();
      var instance = provider.GetDataDescriptors().Single(x => x.Name.Contains("Tower"));
      var regProblem = new RegressionProblem();
      regProblem.Load(provider.LoadData(instance));

      #region Algorithm Configuration
      gbt.Problem = regProblem;
      gbt.Seed = 0;
      gbt.SetSeedRandomly = false;
      gbt.Iterations = 3000;
      gbt.MaxSize = 20;
      gbt.Nu = 0.005;
      gbt.LossFunctionParameter.Value = gbt.LossFunctionParameter.ValidValues.First(l => l.ToString().Contains("Relative"));
      gbt.CreateSolution = false;
      #endregion

      RunAlgorithm(gbt);

      Console.WriteLine(gbt.ExecutionTime);
      Assert.AreEqual(0.061954221604374943, ((DoubleValue)gbt.Results["Loss (train)"].Value).Value, 1E-6);
      Assert.AreEqual(0.06316303473499961, ((DoubleValue)gbt.Results["Loss (test)"].Value).Value, 1E-6);
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

    #region helper
    private void BuildTree(double[,] xy, string[] allVariables, int maxSize) {
      int nRows = xy.GetLength(0);
      var allowedInputs = allVariables.Skip(1);
      var dataset = new Dataset(allVariables, xy);
      var problemData = new RegressionProblemData(dataset, allowedInputs, allVariables.First());
      problemData.TrainingPartition.Start = 0;
      problemData.TrainingPartition.End = nRows;
      problemData.TestPartition.Start = nRows;
      problemData.TestPartition.End = nRows;
      var solution = GradientBoostedTreesAlgorithmStatic.TrainGbm(problemData, new SquaredErrorLoss(), maxSize, nu: 1, r: 1, m: 1, maxIterations: 1, randSeed: 31415);
      var model = solution.Model;
      var treeM = model.Models.Skip(1).First() as RegressionTreeModel;

      Console.WriteLine(treeM.ToString());
      Console.WriteLine();
    }
    #endregion
  }
}
