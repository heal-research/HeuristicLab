using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.FileShrinker {
  class Program {
    private static void Main(string[] args) {
      string directoryName = ".";

      if (args.Length == 1) directoryName = args[0];
      var variableValuesField = typeof(Dataset).GetField("variableValues", BindingFlags.NonPublic | BindingFlags.Instance);

      foreach (var fileName in Directory.GetFiles(directoryName, "*.hl")) {
        IStorableContent content;
        try {
          content = XmlParser.Deserialize<IStorableContent>(fileName);
        }
        catch (PersistenceException) {
          continue;
        }

        var datasetCache = new Dictionary<string, List<Dataset>>();
        foreach (var problemData in content.GetObjectGraphObjects().OfType<DataAnalysisProblemData>()) {
          Dataset uniqueData = GetEqualDataset(problemData, datasetCache);
          if (uniqueData == null) continue;

          var uniqueValues = (Dictionary<string, IList>)variableValuesField.GetValue(uniqueData);
          variableValuesField.SetValue(problemData.Dataset, new Dictionary<string, IList>(uniqueValues));
        }

        foreach (var run in content.GetObjectGraphObjects().OfType<IRun>()) {
          var results = (Dictionary<string, IItem>)run.Results;
          if (results.ContainsKey("ProblemData.Dataset"))
            results.Remove("ProblemData.Dataset");
        }

        string directory = Path.GetDirectoryName(fileName);
        string file = Path.GetFileName(fileName);
        XmlGenerator.Serialize(content, directory + Path.DirectorySeparatorChar + "Shrinked " + file, 9);
      }
    }

    private static Dataset GetEqualDataset(DataAnalysisProblemData problemData, Dictionary<string, List<Dataset>> datasetCache) {
      if (!datasetCache.ContainsKey(problemData.Name)) {
        datasetCache.Add(problemData.Name, new List<Dataset>() { problemData.Dataset });
        return null;
      }

      foreach (var dataset in datasetCache[problemData.Name]) {
        if (EqualDatasets(problemData.Dataset, dataset)) return dataset;
      }

      datasetCache[problemData.Name].Add(problemData.Dataset);
      return null;
    }

    private static bool EqualDatasets(Dataset ds1, Dataset ds2) {
      if (ds1.Rows != ds2.Rows) return false;
      if (!ds1.VariableNames.SequenceEqual(ds2.VariableNames)) return false;

      foreach (string variable in ds1.DoubleVariables) {
        var values1 = ds1.GetDoubleValues(variable);
        var values2 = ds2.GetDoubleValues(variable);
        if (!values1.SequenceEqual(values2)) return false;
      }

      return true;
    }
  }

}
