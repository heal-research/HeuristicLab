using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.PluginInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class NamedItemTest {
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
