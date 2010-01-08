using HeuristicLab.GP.StructureIdentification.Networks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.Core;
using HeuristicLab.GP.StructureIdentification;
using System.Linq;
using System.Collections.Generic;

namespace HeuristicLab.GP.Tests {


  /// <summary>
  ///This is a test class for NetworkToFunctionTransformerTest and is intended
  ///to contain all NetworkToFunctionTransformerTest Unit Tests
  ///</summary>
  [TestClass()]
  public class NetworkToFunctionTransformerTest {


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
    //You can use the following additional attributes as you write your tests:
    //
    //Use ClassInitialize to run code before running the first test in the class
    //[ClassInitialize()]
    //public static void MyClassInitialize(TestContext testContext)
    //{
    //}
    //
    //Use ClassCleanup to run code after all tests in a class have run
    //[ClassCleanup()]
    //public static void MyClassCleanup()
    //{
    //}
    //
    //Use TestInitialize to run code before running each test
    //[TestInitialize()]
    //public void MyTestInitialize()
    //{
    //}
    //
    //Use TestCleanup to run code after each test has run
    //[TestCleanup()]
    //public void MyTestCleanup()
    //{
    //}
    //
    #endregion


    /// <summary>
    ///A test for TransformExpression
    ///</summary>
    [TestMethod()]
    [DeploymentItem("HeuristicLab.GP.StructureIdentification.Networks-3.2.dll")]
    public void TransformExpressionTest() {
      IFunctionTree tree = null; // TODO: Initialize to an appropriate value
      string targetVariable = string.Empty; // TODO: Initialize to an appropriate value
      IFunctionTree expected = null; // TODO: Initialize to an appropriate value
      IFunctionTree actual;
      actual = NetworkToFunctionTransformer_Accessor.TransformExpression(tree, targetVariable);
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for InvertFunction
    ///</summary>
    [TestMethod()]
    [DeploymentItem("HeuristicLab.GP.StructureIdentification.Networks-3.2.dll")]
    public void InvertFunctionTest() {
      var log = new OpenLog();
      var exp = new OpenExp();
      var openAdd = new AdditionF1();
      var openSub = new SubtractionF1();
      var openMul = new MultiplicationF1();
      var openDiv = new DivisionF1();
      var param = new OpenParameter();
      var rootAdd = new OpenAddition();
      var rootSub = new OpenSubtraction();
      var rootMul = new OpenMultiplication();
      var rootDiv = new OpenDivision();

      IFunctionTree tree = exp.GetTreeNode(); tree.AddSubTree(param.GetTreeNode());
      IFunctionTree expected = log.GetTreeNode(); expected.AddSubTree(param.GetTreeNode());
      IFunctionTree actual;
      actual = NetworkToFunctionTransformer_Accessor.InvertFunction(tree);
      var e = (from x in FunctionTreeIterator.IteratePostfix(expected)
               select x.Function.GetType()).GetEnumerator();
      var a = (from x in FunctionTreeIterator.IteratePostfix(actual)
               select x.Function.GetType()).GetEnumerator();

      Assert.AreEqual(expected.GetSize(), actual.GetSize());
      while (e.MoveNext() && a.MoveNext()) {
        Assert.AreEqual(e.Current, a.Current);
      }
    }

    [TestMethod()]
    [DeploymentItem("HeuristicLab.GP.StructureIdentification.Networks-3.2.dll")]
    public void TransformTest() {
      var log = new OpenLog();
      var exp = new OpenExp();
      var openAdd = new AdditionF1();
      var openSub = new SubtractionF1();
      var openMul = new MultiplicationF1();
      var openDiv = new DivisionF1();
      var param = new OpenParameter();
      var rootAdd = new OpenAddition();
      var rootSub = new OpenSubtraction();
      var rootMul = new OpenMultiplication();
      var rootDiv = new OpenDivision();
      var closedAdd = new Addition();
      var closedSub = new Subtraction();
      var closedMul = new Multiplication();
      var closedDiv = new Division();
      var closedVar = new HeuristicLab.GP.StructureIdentification.Variable();

      IFunctionTree tree = rootAdd.GetTreeNode();
      tree.AddSubTree(param.GetTreeNode());
      tree.AddSubTree(param.GetTreeNode());
      tree.AddSubTree(param.GetTreeNode());

      IEnumerable<IFunctionTree> actualTrees = NetworkToFunctionTransformer_Accessor.Transform(tree, new List<string>() { "1", "2", "3" });
      IFunctionTree t0 = closedAdd.GetTreeNode(); t0.AddSubTree(param.GetTreeNode()); t0.AddSubTree(param.GetTreeNode());
      IFunctionTree t1 = closedSub.GetTreeNode(); t1.AddSubTree(param.GetTreeNode()); t1.AddSubTree(param.GetTreeNode());
      IFunctionTree t2 = closedSub.GetTreeNode(); t2.AddSubTree(param.GetTreeNode()); t2.AddSubTree(param.GetTreeNode());

      var expectedTrees = (new List<IFunctionTree>() {
        t0, t1, t2
      }).GetEnumerator();
      foreach (var actualTree in actualTrees) {
        if (!expectedTrees.MoveNext()) Assert.Fail();
        IFunctionTree expectedTree = expectedTrees.Current;
        var e = (from x in FunctionTreeIterator.IteratePostfix(expectedTree)
                 select x.Function.GetType()).GetEnumerator();
        var a = (from x in FunctionTreeIterator.IteratePostfix(actualTree)
                 select x.Function.GetType()).GetEnumerator();
        Assert.AreEqual(expectedTree.GetSize(), actualTree.GetSize());
        while (e.MoveNext() && a.MoveNext()) {
          Assert.AreEqual(e.Current, a.Current);
        }
      }
    }

  }
}
