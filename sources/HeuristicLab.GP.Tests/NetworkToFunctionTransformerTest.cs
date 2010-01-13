using HeuristicLab.GP.StructureIdentification.Networks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.Core;
using HeuristicLab.GP.StructureIdentification;
using System.Linq;
using System.Collections.Generic;

namespace HeuristicLab.GP.Test {


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
      actual = NetworkToFunctionTransformer_Accessor.InvertChain(tree);
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
      SymbolicExpressionImporter importer = new SymbolicExpressionImporter();

      {
        IFunctionTree tree = importer.Import("(open-+ (open-param - 0) (open-param - 0) (open-param - 0))");

        IEnumerable<IFunctionTree> actualTrees = NetworkToFunctionTransformer_Accessor.Transform(tree, new List<string>() { "a", "b", "c" });

        IFunctionTree t0 = importer.Import("(+ (variable 1.0 b 0) (variable 1.0 c 0))");
        IFunctionTree t1 = importer.Import("(- (variable 1.0 a 0) (variable 1.0 c 0))");
        IFunctionTree t2 = importer.Import("(- (variable 1.0 a 0) (variable 1.0 b 0))");

        CompareTrees(actualTrees, new List<IFunctionTree>() {
        t0, t1, t2
      });
      }

      {
        IFunctionTree tree = importer.Import("(open-- (open-param - 0) (f1-+ (open-param - 0) 1.0) (f1-* (open-param - 0) 1.0))");

        IEnumerable<IFunctionTree> actualTrees = NetworkToFunctionTransformer_Accessor.Transform(tree, new List<string>() { "a", "b", "c" });

        IFunctionTree t0 = importer.Import("(- (+ (variable 1.0 b 0) 1.0) (* (variable 1.0 c 0) 1.0 ))");
        IFunctionTree t1 = importer.Import("(- (+ (variable 1.0 a 0) (* (variable 1.0 c 0) 1.0)) 1.0 )");
        IFunctionTree t2 = importer.Import("(/ (+ (variable 1.0 a 0) (+ (variable 1.0 b 0) 1.0)) 1.0 )");

        CompareTrees(actualTrees, new List<IFunctionTree>() {
        t0, t1, t2
      });
      }

      {
        IFunctionTree tree = importer.Import("(cycle (open-* (open-param - 0) (open-param - 0) (open-param - 0)))");

        IEnumerable<IFunctionTree> actualTrees = NetworkToFunctionTransformer_Accessor.Transform(tree, new List<string>() { "a", "b", "c" });

        IFunctionTree t0 = importer.Import("(* (variable 1.0 b 0) (variable 1.0 c 0))");
        IFunctionTree t1 = importer.Import("(/ (variable 1.0 a 0) (variable 1.0 c 0))");
        IFunctionTree t2 = importer.Import("(/ (variable 1.0 a 0) (variable 1.0 b 0))");


        CompareTrees(actualTrees, new List<IFunctionTree>() {
        t0, t1, t2
      });
      }


      {
        IFunctionTree tree = importer.Import("(open-- (open-log (open-param - 0)) (open-exp (open-param - 0)) (open-param - 0))");

        IEnumerable<IFunctionTree> actualTrees = NetworkToFunctionTransformer_Accessor.Transform(tree, new List<string>() { "a", "b", "c" });

        IFunctionTree t0 = importer.Import(@"(exp (- (exp (variable 1.0 b 0))
                                                     (variable 1.0 c 0))))");
        IFunctionTree t1 = importer.Import(@"(log (+ (log (variable 1.0 a 0))
                                                     (variable 1.0 c 0))))");
        IFunctionTree t2 = importer.Import(@"(+ (log (variable 1.0 a 0))
                                                (exp (variable 1.0 b 0)))");

        CompareTrees(actualTrees, new List<IFunctionTree>() {
        t0, t1, t2
      });
      }



    }

    private void CompareTrees(IEnumerable<IFunctionTree> actual, IEnumerable<IFunctionTree> expected) {
      var expectedEnumerator = expected.GetEnumerator();
      foreach (var actualTree in actual) {
        if (!expectedEnumerator.MoveNext()) Assert.Fail();
        IFunctionTree expectedTree = expectedEnumerator.Current;
        var e = (from x in FunctionTreeIterator.IteratePostfix(expectedTree)
                 select x).GetEnumerator();
        var a = (from x in FunctionTreeIterator.IteratePostfix(actualTree)
                 select x).GetEnumerator();
        Assert.AreEqual(expectedTree.GetSize(), actualTree.GetSize());
        while (e.MoveNext() && a.MoveNext()) {
          Assert.AreEqual(e.Current.GetType(), a.Current.GetType());
          if (e.Current.Function is HeuristicLab.GP.StructureIdentification.Variable) {
            var expectedVar = (VariableFunctionTree)e.Current;
            var actualVar = (VariableFunctionTree)a.Current;
            Assert.AreEqual(expectedVar.VariableName, actualVar.VariableName);
            Assert.AreEqual(expectedVar.Weight, actualVar.Weight);
            Assert.AreEqual(expectedVar.SampleOffset, actualVar.SampleOffset);
          } else if (e.Current.Function is Constant) {
            var expectedConst = (ConstantFunctionTree)e.Current;
            var actualConst = (ConstantFunctionTree)a.Current;
            Assert.AreEqual(expectedConst.Value, actualConst.Value);
          }
        }
      }
    }
  }
}
