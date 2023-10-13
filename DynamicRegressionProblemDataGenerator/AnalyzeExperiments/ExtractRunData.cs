using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.PluginInfrastructure.Manager;
using Xunit;

namespace DynamicRegressionProblemDataGenerator.AnalyzeExperiments {
  public class ExtractRunData {
    static ExtractRunData() {
      var pm = new PluginManager(AppDomain.CurrentDomain.BaseDirectory);
      pm.DiscoverAndCheckPlugins();
      foreach (var plugin in pm.Plugins) {
        foreach (string path in plugin.AssemblyLocations) {
          Assembly.LoadFile(path);
        }
      }
      ContentManager.Initialize(new PersistenceContentManager());
    }
    
    [Theory]
    [Trait("Analyze", "Extract")]
    [InlineData(@"C:\Users\P41107\Desktop\F1_GA_runs.hl")]
    void AnalyzeExperiment_GA_F1(string path) {
      string filename = Path.GetFileName(path);
      var runCollection = (RunCollection)ContentManager.Load(path);

      CreateCsv(path.Replace(filename, filename + "_Qualities.csv"), runCollection, 
        resultName: "Qualities", parameterName: null, 
        "CurrentBestQuality", "CurrentAverageQuality", "CurrentWorstQuality", "BestQuality"
      );
      
      CreateCsv(path.Replace(filename, filename + "_IndividualQualities.csv"), runCollection, 
        resultName: "IndividualQualities", parameterName: null
      );
      
      CreateCsv(path.Replace(filename, filename + "_SymbolFrequencies.csv"), runCollection,
        resultName: "Symbol frequencies", parameterName: null
      );
      
      CreateCsv(path.Replace(filename, filename + "_VariableFrequencies.csv"), runCollection,
        resultName: "Variable frequencies", parameterName: null
      );
      
      CreateCsv(path.Replace(filename, filename + "_BestSolutionVariableImpacts.csv"), runCollection,
        resultName: "BestSolutionVariableImpacts", parameterName: null
      );
      
      CreateCsv(path.Replace(filename, filename + "_PopulationVariableImpacts.csv"), runCollection,
        resultName: "PopulationVariableImpacts", parameterName: null
      );
      
      CreateCsv(path.Replace(filename, filename + "_PopulationVariableImpactsIndividuals.csv"), runCollection,
        resultName: "PopulationVariableImpactsIndividuals", parameterName: null
      );
    }

    private void CreateCsv(string path, RunCollection runCollection, string resultName = null, string parameterName = null, params string[] columnNames) {
      var tables = ExtractDataTables(runCollection, resultName, parameterName);
      var combined = CombineDataTables(tables, columnNames);
      WriteCsv(combined, path);
    }

    private static void WriteCsv(List<dynamic> data, string path) {
      if (!Directory.Exists(Path.GetDirectoryName(path))) {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
      }
      using var file = new FileStream(path, FileMode.Create);
      using var writer = new StreamWriter(file, Encoding.UTF8);
      using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" });
      
      csv.WriteRecords(data);
    }

    private static List<dynamic> CombineDataTables(List<(RunMetadata, DataTable)> dataTables, params string[] columnNames) {
      var allRows = new List<dynamic>();

      if (columnNames.Length == 0) {
        columnNames = dataTables.SelectMany(x => x.Item2.Rows.Select(r => r.Name)).Distinct().ToArray();
      }

      foreach (var (metadata, dataTable) in dataTables) {
        for (int i = 0; i < dataTable.Rows.First().Values.Count; i++) {
          dynamic row = new ExpandoObject();
          row.AlgorithmName = metadata.AlgorithmName;
          row.AlgorithmType = metadata.AlgorithmType;
          row.RunName = metadata.RunName;
          row.RunIndex = metadata.RunIndex;
          row.Iteration = i;
          foreach (string column in columnNames) {
            ((IDictionary<string, Object>)row).Add(column, dataTable.Rows[column].Values[i]);
          }
          allRows.Add(row);  
        }
      }

      return allRows;
    }

    private static List<(RunMetadata, DataTable)> ExtractDataTables(RunCollection runCollection, string resultName = null, string parameterName = null) {
      var dataTables = new List<(RunMetadata, DataTable)>();
      int index = 0;
      foreach (var run in runCollection) {
        string algorithmName = ((StringValue)run.Parameters["Algorithm Name"]).Value;
        string algorithmType = ((StringValue)run.Parameters["Algorithm Type"]).Value;
        string runName = run.Name;

        var metadata = new RunMetadata(algorithmName, algorithmType, runName, index);
        DataTable dataTable;
        
        if (resultName != null) {
          dataTable = (DataTable)run.Results[resultName];
        } else if (parameterName != null) {
          dataTable = (DataTable)run.Parameters[parameterName];
        } else {
          throw new ArgumentException("Either resultName or parameterName must be specified.");
        }
        
        dataTables.Add((metadata, dataTable));
        index++;
      }

      return dataTables;
    }
  }
  
  public record RunMetadata(string AlgorithmName, string AlgorithmType, string RunName, int RunIndex);
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
