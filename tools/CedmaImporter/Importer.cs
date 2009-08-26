using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HeuristicLab.Modeling.Database;
using HeuristicLab.GP;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.GP.StructureIdentification;
using System.Diagnostics;
using HeuristicLab.Modeling;

namespace CedmaImporter {
  public class Importer {

    private const int ID_COLUMN = 0;
    private const int FILENAME_COLUMN = 1;
    private const int TARGETVARIABLE_COLUMN = 2;
    private const int ALGORITHM_COLUMN = 3;
    private const int RESULTS_IDX = 4;
    private const int TRAINING_MSE = 4;
    private const int VALIDATION_MSE = TRAINING_MSE + 1;
    private const int TEST_MSE = TRAINING_MSE + 2;

    private const int TRAINING_R2 = 7;
    private const int VALIDATION_R2 = TRAINING_R2 + 1;
    private const int TEST_R2 = TRAINING_R2 + 2;

    private const int TRAINING_MAPE = 10;
    private const int VALIDATION_MAPE = TRAINING_MAPE + 1;
    private const int TEST_MAPE = TRAINING_MAPE + 2;

    private const int TRAINING_MAPRE = 13;
    private const int VALIDATION_MAPRE = TRAINING_MAPRE + 1;
    private const int TEST_MAPRE = TRAINING_MAPRE + 2;

    private const int TRAINING_VAF = 16;
    private const int VALIDATION_VAF = TRAINING_VAF + 1;
    private const int TEST_VAF = TRAINING_VAF + 2;

    private const int VARIABLE_IMPACTS = 19;
    private const string EVALUATION_IMPACT = "EvaluationImpact";
    private const string QUALITY_IMPACT = "QualityImpact";

    private string[] results;
    private string[] inputVariables;
    private HeuristicLab.CEDMA.Server.Problem problem;


    public Importer(HeuristicLab.CEDMA.Server.Problem problem) {
      this.problem = problem;
    }

    public void Import(string fileName, string dirName) {
      string outputFileName = Path.Combine(dirName, Path.GetFileNameWithoutExtension(fileName) + ".sdf");
      string connectionString = @"Data Source=" + outputFileName;

      var database = new HeuristicLab.Modeling.Database.SQLServerCompact.DatabaseService(connectionString);
      IProblem p = database.GetOrCreateProblem(problem.Dataset);
      using (StreamReader reader = File.OpenText(fileName)) {
        ReadResultsAndInputVariables(reader, database);
        reader.ReadLine();
        ImportAllModels(dirName, reader, database);
      }
      database.Disconnect();
    }

    private void ReadResultsAndInputVariables(StreamReader reader, IModelingDatabase database) {
      string[] columns = reader.ReadLine().Split(';').Select(x=>x.Trim()).ToArray();
      results = new string[columns.Length];
      inputVariables = new string[columns.Length];
      for (int i = RESULTS_IDX; i < columns.Length; i++) {
        string resultColumn = columns[i].Trim();
        if (resultColumn.Contains(" ")) {
          string[] tokens = resultColumn.Split(' ');
          string variableName = tokens[1].Trim(' ','(',')');
          string variableResultName = tokens[0].Trim();
          inputVariables[i] = variableName;
          results[i] = variableResultName;
        } else {
          // normal result value
          results[i] = resultColumn;
        }
      }
    }

    private void ImportAllModels(string dirName, StreamReader reader, IModelingDatabase database) {
      while (!reader.EndOfStream) {
        string[] modelData = reader.ReadLine().Split(';','\t').Select(x => x.Trim()).ToArray();
        int id = int.Parse(modelData[ID_COLUMN]);
        string targetVariableName = modelData[TARGETVARIABLE_COLUMN].Trim();
        string algoName = modelData[ALGORITHM_COLUMN].Trim();
        try {
          HeuristicLab.Modeling.IAnalyzerModel model = new AnalyzerModel();
          model.TargetVariable = targetVariableName;
          model.Dataset = problem.Dataset;
          model.TrainingSamplesStart = problem.TrainingSamplesStart;
          model.TrainingSamplesEnd = problem.TrainingSamplesEnd;
          model.ValidationSamplesStart = problem.ValidationSamplesStart;
          model.ValidationSamplesEnd = problem.ValidationSamplesEnd;
          model.TestSamplesStart = problem.TestSamplesStart;
          model.TestSamplesEnd = problem.TestSamplesEnd;

          SetModelResults(model, modelData);
          SetInputVariableResults(model, modelData);

          model.Predictor = CreatePredictor(targetVariableName, dirName, modelData[FILENAME_COLUMN].Trim(), algoName);
          database.Persist(model, algoName, null);
        }
        catch (Exception ex) {
        }
      }
    }

    private void SetInputVariableResults(HeuristicLab.Modeling.IAnalyzerModel model, string[] modelData) {
      for (int i = VARIABLE_IMPACTS; i < modelData.Length; i++) {
        if (!string.IsNullOrEmpty(modelData[i])) {
          model.AddInputVariable(inputVariables[i]);
          if (results[i] == EVALUATION_IMPACT) {
            model.SetVariableEvaluationImpact(inputVariables[i], double.Parse(modelData[i]));
          } else if (results[i] == QUALITY_IMPACT) {
            model.SetVariableQualityImpact(inputVariables[i], double.Parse(modelData[i]));
          } else throw new FormatException();
        }
      }
    }

    private void SetModelResults(HeuristicLab.Modeling.IAnalyzerModel model, string[] modelData) {
      model.TrainingMeanSquaredError = double.Parse(modelData[TRAINING_MSE]);
      model.ValidationMeanSquaredError = double.Parse(modelData[VALIDATION_MSE]);
      model.TestMeanSquaredError = double.Parse(modelData[TEST_MSE]);

      model.TrainingCoefficientOfDetermination = double.Parse(modelData[TRAINING_R2]);
      model.ValidationCoefficientOfDetermination = double.Parse(modelData[VALIDATION_R2]);
      model.TestCoefficientOfDetermination = double.Parse(modelData[TEST_R2]);

      model.TrainingMeanAbsolutePercentageError = double.Parse(modelData[TRAINING_MAPE]);
      model.ValidationMeanAbsolutePercentageError = double.Parse(modelData[VALIDATION_MAPE]);
      model.TestMeanAbsolutePercentageError = double.Parse(modelData[TEST_MAPE]);

      model.TrainingMeanAbsolutePercentageOfRangeError = double.Parse(modelData[TRAINING_MAPRE]);
      model.ValidationMeanAbsolutePercentageOfRangeError = double.Parse(modelData[VALIDATION_MAPRE]);
      model.TestMeanAbsolutePercentageOfRangeError = double.Parse(modelData[TEST_MAPRE]);

      model.TrainingVarianceAccountedFor = double.Parse(modelData[TRAINING_VAF]);
      model.ValidationVarianceAccountedFor = double.Parse(modelData[VALIDATION_VAF]);
      model.TestVarianceAccountedFor = double.Parse(modelData[TEST_VAF]);
    }

    private HeuristicLab.Modeling.IPredictor CreatePredictor(string targetVariable, string dirName, string modelFileName, string algoName) {
      foreach (char c in Path.GetInvalidFileNameChars()) {
        modelFileName = modelFileName.Replace(c, '_');
      }
      if (algoName == "SupportVectorRegression") {
        //HeuristicLab.SupportVectorMachines.SVMModel model = new HeuristicLab.SupportVectorMachines.SVMModel();
        //model.Model = SVM.Model.Read(Path.Combine(dirName, modelFileName) + ".svm.model.txt");
        //model.RangeTransform = SVM.RangeTransform.Read(Path.Combine(dirName, modelFileName) + ".svm.transform.txt");
        //return new HeuristicLab.SupportVectorMachines.Predictor(model, targetVariable);
        throw new FormatException();
      } else {
        SymbolicExpressionImporter sexpImporter = new SymbolicExpressionImporter();
        GeneticProgrammingModel model = new GeneticProgrammingModel();
        using (StreamReader reader = File.OpenText(Path.Combine(dirName, modelFileName) + ".gp.txt")) {
          model.FunctionTree = sexpImporter.Import(reader);
        }
        return new HeuristicLab.GP.StructureIdentification.Predictor(new HL2TreeEvaluator(), model);
      }
    }
  }
}
