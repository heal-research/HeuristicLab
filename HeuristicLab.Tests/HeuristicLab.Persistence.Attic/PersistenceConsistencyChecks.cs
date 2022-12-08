using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HEAL.Attic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests.Persistence.Attic {
  [TestClass]
  public class PersistenceConsistencyChecks {
    [TestCategory("Persistence.Attic")]
    [TestCategory("Essential")]
    [TestMethod]
    public void CheckDuplicateGUIDs() {
      // easy to produce duplicate GUIDs with copy&paste
      var dict = new Dictionary<Guid, string>();
      var duplicates = new Dictionary<string, string>();

      try {
        // using AppDomain instead of ApplicationManager so that NonDiscoverableTypes are also checked
        foreach (Type type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())) {
          var attr = StorableTypeAttribute.GetStorableTypeAttribute(type);
          if (attr == null)
            continue;

          foreach (var guid in attr.Guids) {
            if (!dict.ContainsKey(guid)) {
              dict.Add(guid, type.FullName);
            } else {
              duplicates.Add(type.FullName, dict[guid]);
            }
          }
        }
      } catch (ReflectionTypeLoadException e) {
        var loaderExeptions = string.Join("-----" + Environment.NewLine, e.LoaderExceptions.Select(x => x.ToString()));
        var message = string.Join(Environment.NewLine, "Could not load all types. Check if test process architecture is set to x64.",
          string.Empty, "Exception:", e, string.Empty, "LoaderExceptions:", loaderExeptions);
        Assert.Fail(message);
      }

      foreach (var kvp in duplicates) {
        Console.WriteLine($"{kvp.Key} has same GUID as {kvp.Value}");
      }

      if (duplicates.Any()) Assert.Fail("Duplicate GUIDs found.");
    }
  }
}
