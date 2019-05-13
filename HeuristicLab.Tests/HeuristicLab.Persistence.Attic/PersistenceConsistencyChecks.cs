using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.PluginInfrastructure;
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
      //get all non-generic and instantiable classes which implement IContentView
      foreach (Type type in ApplicationManager.Manager.GetTypes(typeof(object))) {
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
