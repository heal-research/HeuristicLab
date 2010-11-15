using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab_33.Tests {
  [TestClass]
  public class StorableConstructorTest {

    // Use ClassInitialize to run code before running the first test in the class
    [ClassInitialize]
    public static void MyClassInitialize(TestContext testContext) {
      PluginLoader.pluginAssemblies.Any();
    }

    [TestMethod]
    public void TestStorableConstructor() {
      StringBuilder errorMessage = new StringBuilder();

      foreach (Type storableType in ApplicationManager.Manager.GetTypes(typeof(object))
        .Where(t => StorableClassAttribute.IsStorableClass(t))) {
        IEnumerable<ConstructorInfo> ctors = storableType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        ConstructorInfo storableConstructor = ctors.Where(c => c.GetParameters().Count() == 1 && c.GetParameters().First().ParameterType == typeof(bool)).FirstOrDefault();
        if (storableConstructor == null) errorMessage.Append(Environment.NewLine + storableType.ToString() + ": No storable constructor is defined.");
        else {
          if (storableType.IsSealed && !storableConstructor.IsPrivate)
            errorMessage.Append(Environment.NewLine + storableType.ToString() + ": Storable constructor must be private in sealed classes.");
          else if (!storableType.IsSealed && !(storableConstructor.IsFamily || storableConstructor.IsPublic))
            errorMessage.Append(Environment.NewLine + storableType.ToString() + ": Storable constructor must be protected (can be public in rare cases).");
        }
      }
      Assert.IsTrue(errorMessage.Length == 0, errorMessage.ToString());
    }
  }
}
