using System;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.PluginInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.MainForm.WindowsForms_3._3.Tests {
  [TestClass]
  public class ContentViewTests {
    public ContentViewTests() {
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
    [ClassInitialize()]
    public static void MyClassInitialize(TestContext testContext) {
      //needed to load all assemblies in the current appdomain
      new HeuristicLab.Analysis.Views.HeuristicLabAnalysisViewsPlugin();
      new HeuristicLab.Core.Views.HeuristicLabCoreViewsPlugin();
      new HeuristicLab.Data.Views.HeuristicLabDataViewsPlugin();
      new HeuristicLab.Encodings.PermutationEncoding.Views.HeuristicLabEncodingsPermutationEncodingViewsPlugin();
      new HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views.HeuristicLabEncodingsSymbolicExpressionTreeEncodingViewsPlugin();
      new HeuristicLab.MainForm.HeuristicLabMainFormPlugin();
      new HeuristicLab.MainForm.WindowsForms.HeuristicLabMainFormPlugin();
      new HeuristicLab.Operators.Views.HeuristicLabOperatorsViewsPlugin();
      new HeuristicLab.Operators.Views.GraphVisualization.HeuristicLabOperatorsViewsGraphVisualizationPlugin();
      new HeuristicLab.Parameters.Views.HeuristicLabParametersViewsPlugin();
      new HeuristicLab.Problems.ArtificialAnt.Views.HeuristicLabProblemsArtificialAntViewsPlugin();
      new HeuristicLab.Problems.DataAnalysis.Views.HeuristicLabProblemsDataAnalysisViewsPlugin();
      new HeuristicLab.Problems.ExternalEvaluation.Views.HeuristicLabProblemsExternalEvaluationViewsPlugin();
      new HeuristicLab.Problems.Knapsack.Views.HeuristicLabProblemsKnapsackViewsPlugin();
      new HeuristicLab.Problems.OneMax.Views.HeuristicLabProblemsKnapsackViewsPlugin();
      new HeuristicLab.Problems.TestFunctions.Views.HeuristicLabProblemsTestFunctionsViewsPlugin();
      new HeuristicLab.Problems.TravelingSalesman.Views.HeuristicLabProblemsTravelingSalesmanViewsPlugin();
      new HeuristicLab.Problems.VehicleRouting.Views.HeuristicLabProblemsVehicleRoutingViewsPlugin();
    }

    // Use ClassCleanup to run code after all tests in a class have run
    //[ClassCleanup()]
    //public static void MyClassCleanup() {
    //}
    //
    // Use TestInitialize to run code before running each test 
    //[TestInitialize()]
    //public void MyTestInitialize() {      }
    //
    // Use TestCleanup to run code after each test has run
    // [TestCleanup()]
    // public void MyTestCleanup() { }
    //
    #endregion

    [TestMethod]
    public void ContentViewAttributeTest() {
      //get all non-generic and instantiable classes which implement IContentView
      foreach (Type viewType in ApplicationManager.Manager.GetTypes(typeof(IContentView), true).Where(t => !t.ContainsGenericParameters)) {
        //get all ContentAttributes on the instantiable view
        foreach (ContentAttribute attribute in viewType.GetCustomAttributes(typeof(ContentAttribute), false).Cast<ContentAttribute>()) {
          Assert.IsTrue(attribute.ContentType == typeof(IContent) || attribute.ContentType.GetInterfaces().Contains(typeof(IContent)),
            "The type specified in the ContentAttribute of {0} must implement IContent.", viewType);
        }
        //check if view can handle null as content by calling OnContentChanged
        IContentView view = (IContentView)Activator.CreateInstance(viewType);
        ContentView_Accessor accessor = new ContentView_Accessor(new PrivateObject(view));
        accessor.OnContentChanged();
      }
    }
  }
}
