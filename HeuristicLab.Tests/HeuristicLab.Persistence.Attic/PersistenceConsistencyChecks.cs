using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests.Persistence.Attic {
  [TestClass]
  public class PersistenceConsistencyChecks {
    [TestCategory("Persistence.Attic")]
    [TestCategory("Essential")]
    [TestProperty("Time", "short")]
    [TestMethod]
    public void CheckDuplicateGUIDs() {
      // easy to produce duplicate GUIDs with copy&paste
      var dict = new Dictionary<Guid, string>();
      var duplicates = new Dictionary<string, string>();
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

      foreach (var kvp in duplicates) {
        Console.WriteLine($"{kvp.Key} has same GUID as {kvp.Value}");
      }

      if (duplicates.Any()) Assert.Fail("Duplicate GUIDs found.");
    }
  }
}
