using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.PluginInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests.HeuristicLab_3._3 {
  [TestClass]
  public class NamedItemTest {
    [ClassInitialize]
    public static void MyClassInitialize(TestContext testContext) {
      PluginLoader.Assemblies.Any();
    }

    [TestMethod]
    [TestCategory("General")]
    [TestCategory("Essential")]
    [TestProperty("Time", "long")]
    public void NamedItemTestNameNotEmpty() {
      StringBuilder sb = new StringBuilder();
      foreach (var namedItem in ApplicationManager.Manager.GetInstances<INamedItem>()) {
        if (string.IsNullOrEmpty(namedItem.Name)) {
          sb.AppendLine(string.Format("{0} does not have a name specified when created by the default ctor.", namedItem.GetType().GetPrettyName()));
        }
      }
      Assert.IsTrue(sb.Length == 0, sb.ToString());
    }
  }
}
