using System;
using System.Linq;
using System.Reflection;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.PluginInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab_33.Tests {
  [TestClass]
  public class CloningConstructorTest {
    // Use ClassInitialize to run code before running the first test in the class
    [ClassInitialize]
    public static void MyClassInitialize(TestContext testContext) {
      PluginLoader.pluginAssemblies.Any();
    }

    [TestMethod]
    public void TestCloningConstructor() {
      StringBuilder errorMessage = new StringBuilder();

      foreach (Type deepCloneableType in ApplicationManager.Manager.GetTypes(typeof(IDeepCloneable))) {
        bool found = false;
        foreach (ConstructorInfo constructor in deepCloneableType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)) {
          ParameterInfo[] parameters = constructor.GetParameters();
          if (parameters.Length == 2 && parameters[0].ParameterType == deepCloneableType && parameters[1].ParameterType == typeof(Cloner)) {
            found = true;
            if (deepCloneableType.IsSealed && !constructor.IsPrivate)
              errorMessage.Append(Environment.NewLine + deepCloneableType.ToString() + ": Cloning constructor must be private in sealed classes.");
            else if (!deepCloneableType.IsSealed && !constructor.IsFamily)
              errorMessage.Append(Environment.NewLine + deepCloneableType.ToString() + ": Cloning constructor must be protected.");
            break;
          }
        }
        if (!found)
          errorMessage.Append(Environment.NewLine + deepCloneableType.ToString() + ": No cloning constructor is defined.");

        if (!deepCloneableType.IsAbstract) {
          MethodInfo cloneMethod = deepCloneableType.GetMethod("Clone", new Type[] { typeof(Cloner) });
          if (cloneMethod == null)
            errorMessage.Append(Environment.NewLine + deepCloneableType.ToString() + ": No virtual cloning method is defined.");
        }
      }
      Assert.IsTrue(errorMessage.Length == 0, errorMessage.ToString());
    }
  }
}
