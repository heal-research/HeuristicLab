using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.Instances.DataAnalysis;

namespace HeuristicLab.Problems.DataAnalysis.Dynamic.ProblemInstances; 

public class DynSymRegCsvInstanceProvider : DynamicRegressionInstanceProvider {
  public override string Name => " CSV File (with extra Indices)";
  public override string Description => "";
  public override Uri WebLink => new("http://dev.heuristiclab.com/trac.fcgi/wiki/Documentation/FAQ#DataAnalysisImportFileFormat");
  public override string ReferencePublication => "";
  public override bool CanImportData => true;

  public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
    return Array.Empty<IDataDescriptor>();
  }

  public override DynamicRegressionProblemData ImportData(string path) {
    var csvFileParser = new TableFileParser();
    csvFileParser.Parse(path, csvFileParser.AreColumnNamesInFirstLine(path));

    var dataset = new Dataset(csvFileParser.VariableNames, csvFileParser.Values);
    string targetVar = dataset.DoubleVariables.Last();
    
    var indicesFileParser = new TableFileParser();
    indicesFileParser.Parse(path + ".indices", columnNamesInFirstLine: true);
    var trainingStartIndices = indicesFileParser.Values[0].Cast<double>().Select(d => (int)d).ToList();
    var trainingEndIndices = indicesFileParser.Values[1].Cast<double>().Select(d => (int)d).ToList();
    var testStartIndices = indicesFileParser.Values[2].Cast<double>().Select(d => (int)d).ToList();
    var testEndIndices = indicesFileParser.Values[3].Cast<double>().Select(d => (int)d).ToList();

    var trainingIndices = new int[indicesFileParser.Rows, 2];
    var testIndices = new int[indicesFileParser.Rows, 2];
    for (int i = 0; i < indicesFileParser.Rows; i++) {
      trainingIndices[i, 0] = trainingStartIndices[i];
      trainingIndices[i, 1] = trainingEndIndices[i];
      testIndices[i, 0] = testStartIndices[i];
      testIndices[i, 1] = testEndIndices[i];
    }
    
    // turn off input variables that are constant in the training partition
    var allowedInputVars = new List<string>();
    var allTrainingIndices = Enumerable
      .Range(0, trainingIndices.GetLength(0))
      .SelectMany(epoch => Enumerable.Range(trainingIndices[epoch, 0], trainingIndices[epoch, 1] - trainingIndices[epoch, 0]))
      .ToList(); 
    if (allTrainingIndices.Count >= 2) {
      foreach (var variableName in dataset.DoubleVariables) {
        if (dataset.GetDoubleValues(variableName, allTrainingIndices).Range() > 0 && variableName != targetVar)
          allowedInputVars.Add(variableName);
      }
    } else {
      allowedInputVars.AddRange(dataset.DoubleVariables.Where(x => !x.Equals(targetVar)));
    }

    var regressionData = new DynamicRegressionProblemData(dataset, allowedInputVars, targetVar);

    var trainingPartEnd = allTrainingIndices.Last();
    regressionData.TrainingPartition.Start = allTrainingIndices.First();
    regressionData.TrainingPartition.End = trainingPartEnd;
    regressionData.TestPartition.Start = trainingPartEnd;
    regressionData.TestPartition.End = csvFileParser.Rows;
    regressionData.TrainingPartitions = new IntMatrix(trainingIndices);
    regressionData.TestPartitions = new IntMatrix(testIndices);

    regressionData.Name = Path.GetFileName(path);

    return regressionData;
  }
  
  
  public override DynamicRegressionProblemData LoadData(IDataDescriptor descriptor) {
    throw new NotImplementedException();
  }
}
