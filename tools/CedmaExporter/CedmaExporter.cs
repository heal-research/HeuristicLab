using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.GP;
using HeuristicLab.Core;
using HeuristicLab.GP.StructureIdentification;
using System.IO;
using HeuristicLab.Data;
using SVM;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Modeling.Database;

namespace CedmaExporter {
  class CedmaExporter {

    public static List<string> WriteVariableImpactHeaders(HeuristicLab.Modeling.Database.SQLServerCompact.DatabaseService database, StreamWriter writer) {
      List<string> inputVarNames = new List<string>();
      var variables = database.GetAllVariables();
      foreach (var r in database.GetAllResultsForInputVariables()) {
        foreach (string varName in variables.Keys) {
          writer.Write(r.Name); writer.Write(": "); writer.Write(varName); writer.Write("; ");
        }
      }
      writer.WriteLine();
      return new List<string>(variables.Keys);
    }

    public static void WriteModel(IModel model, int id, HeuristicLab.Modeling.Database.IModelingDatabase database, StreamWriter writer, List<string> inputVariables, ModelExporter exporter) {
      try {
        writer.Write(id); writer.Write("; ");
        string targetVariable = model.TargetVariable.Name;
        string algoName = model.Algorithm.Name;
        string modelFileName = "model_" + targetVariable + "_" + id.ToString("000");
        writer.Write(modelFileName); writer.Write("; ");
        writer.Write(targetVariable); writer.Write("; ");
        writer.Write(algoName); writer.Write("; ");
        var modelResults = database.GetModelResults(model);
        writer.Write(FindResult("TrainingMeanSquaredError", modelResults)); writer.Write("; ");
        writer.Write(FindResult("ValidationMeanSquaredError", modelResults)); writer.Write("; ");
        writer.Write(FindResult("TestMeanSquaredError", modelResults)); writer.Write("; ");
        writer.Write(FindResult("TrainingCoefficientOfDetermination", modelResults)); writer.Write("; ");
        writer.Write(FindResult("ValidationCoefficientOfDetermination", modelResults)); writer.Write("; ");
        writer.Write(FindResult("TestCoefficientOfDetermination", modelResults)); writer.Write("; ");
        writer.Write(FindResult("TrainingMeanAbsolutePercentageError", modelResults)); writer.Write("; ");
        writer.Write(FindResult("ValidationMeanAbsolutePercentageError", modelResults)); writer.Write("; ");
        writer.Write(FindResult("TestMeanAbsolutePercentageError", modelResults)); writer.Write("; ");
        writer.Write(FindResult("TrainingMeanAbsolutePercentageOfRangeError", modelResults)); writer.Write("; ");
        writer.Write(FindResult("ValidationMeanAbsolutePercentageOfRangeError", modelResults)); writer.Write("; ");
        writer.Write(FindResult("TestMeanAbsolutePercentageOfRangeError", modelResults)); writer.Write("; ");
        writer.Write(FindResult("TrainingVarianceAccountedFor", modelResults)); writer.Write("; ");
        writer.Write(FindResult("ValidationVarianceAccountedFor", modelResults)); writer.Write("; ");
        writer.Write(FindResult("TestVarianceAccountedFor", modelResults)); writer.Write("; ");
        WriteVariableImpacts(writer, database, model, inputVariables);
        // exporter.Export(modelFileName, data);
      }
      catch (FormatException ex) {
        // ignore
      }
      finally {
        writer.WriteLine();
      }
    }

    private static void WriteVariableImpacts(StreamWriter writer, IModelingDatabase database, IModel model, List<string> inputVariables) {
      Dictionary<string, List<IInputVariableResult>> impacts = new Dictionary<string, List<IInputVariableResult>>();
      foreach (var inputVariableResult in database.GetInputVariableResults(model)) {
        if (!impacts.ContainsKey(inputVariableResult.Variable.Name))
          impacts[inputVariableResult.Variable.Name] = new List<IInputVariableResult>();
        impacts[inputVariableResult.Variable.Name].Add(inputVariableResult);
      }

      foreach (string varName in inputVariables) {
        if (impacts.ContainsKey(varName)) {
          writer.Write(FindResult("VariableEvaluationImpact", impacts[varName])); writer.Write("; ");
        } else {
          writer.Write(" ; ");
        }
      }

      foreach (string varName in inputVariables) {
        if (impacts.ContainsKey(varName)) {
          writer.Write(FindResult("VariableQualityImpact", impacts[varName])); writer.Write("; ");
        } else {
          writer.Write(" ; ");
        }
      }
    }

    private static double FindResult(string p, List<IInputVariableResult> rs) {
      return rs.First(e => e.Result.Name == p).Value;
    }

    private static double FindResult(string p, IEnumerable<IModelResult> rs) {
      return rs.First(e => e.Result.Name == p).Value;
    }

    public static void WriteColumnHeaders(StreamWriter writer) {
      writer.Write("Id; Filename; TargetVariable; Algorithm;" +
        "TrainingMSE; ValidationMSE; TestMSE; " +
        "TrainingR2; ValidationR2; TestR2; " +
        "TrainingMAPE; ValidationMAPE; TestMAPE; " +
        "TrainingMAPRE; ValidationMAPRE; TestMAPRE; " +
        "TrainingVAF; ValidationVAF; TestVAF; ");
    }
  }
  class ModelExporter {
    private string outputDir;
    private bool debugging;
    private Dataset dataset;

    public ModelExporter(Dataset ds, string outputDir, bool debugging) {
      this.dataset = ds;
      this.outputDir = outputDir;
      this.debugging = debugging;
    }

    public void Export(string modelFileName, IStorable model) {
      //if (debugging) return;
      //foreach (char c in Path.GetInvalidFileNameChars()) {
      //  modelFileName = modelFileName.Replace(c, '_');
      //}
      //if (model is Predictor) {
      //  using (StreamWriter writer = File.CreateText(Path.Combine(outputDir, modelFileName + ".gp.txt"))) {
      //    writer.Write(treeExporter.Export(((Predictor)model).FunctionTree);
      //  }
      //} else if (model is HeuristicLab.SupportVectorMachines.Predictor) {
      //  SVMModel svmModel = (SVMModel)model;
      //  RangeTransform.Write(Path.Combine(outputDir, modelFileName + ".svm.transform.txt"), svmModel.RangeTransform);
      //  SVM.Model.Write(Path.Combine(outputDir, modelFileName + ".svm.model.txt"), svmModel.Model);
      //} else throw new NotSupportedException("This type of model is not supported by the CedmaExporter: " + model);
    }
  }
}
