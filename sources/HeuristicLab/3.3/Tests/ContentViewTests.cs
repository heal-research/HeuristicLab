using System;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.PluginInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab_3._3.Tests {
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

    // Use ClassInitialize to run code before running the first test in the class
    [ClassInitialize()]
    public static void MyClassInitialize(TestContext testContext) {
      PluginLoader.LoadPluginsIntoAppDomain();
    }


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
        try {
          accessor.OnContentChanged();
        }
        catch (Exception ex) {
          Assert.Fail(viewType.ToString() + Environment.NewLine + ex.Message);
        }
      }
    }
  }
}
