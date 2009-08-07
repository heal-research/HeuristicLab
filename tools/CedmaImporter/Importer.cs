using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HeuristicLab.Modeling.Database;
using HeuristicLab.Modeling.Database.SQLServerCompact;

namespace CedmaImporter {
  public class Importer {

    private const int ID_COLUMN = 0;
    private const int FILENAME_COLUMN = 1;
    private const int TARGETVARIABLE_COLUMN = 2;
    private const int ALGORITHM_COLUMN = 3;
    private const int RESULTS_IDX = 4;
    private Result[] results;
    private string[] inputVariables;

    private HeuristicLab.CEDMA.Server.Problem problem;


    public Importer(HeuristicLab.CEDMA.Server.Problem problem) {
      this.problem = problem;
    }

    public void Import(string fileName, string dirName) {
      string outputFileName = Path.GetFileNameWithoutExtension(fileName) + ".sdf";
      string connectionString = @"Data Source=" + outputFileName;

      DatabaseService database = new DatabaseService(connectionString);
      Problem p = database.GetOrCreateProblem(problem.Dataset);
      using (StreamReader reader = File.OpenText(fileName)) {
        ReadResultsAndInputVariables(reader);
        ImportAllModels(reader, database);
      }
    }

    private void ReadResultsAndInputVariables(StreamReader reader) {
      string[] columns = reader.ReadLine().Split(';');
      results = Enumerable.Repeat<Result>(null, columns.Length).ToArray();
      inputVariables = Enumerable.Repeat<string>(null, columns.Length).ToArray();
      for (int i = RESULTS_IDX; i < columns.Length; i++) {
        string resultColumn = columns[i].Trim();
        if (resultColumn.Contains(":")) {
          string[] tokens = resultColumn.Split(':');
          string variableName = tokens[1].Trim();
          string variableResultName = tokens[0].Trim();
          inputVariables[i] = variableName;
          results[i] = new Result(variableResultName);
        } else {
          // normal result value
          results[i] = new Result(resultColumn);
        }
      }
    }

    private void ImportAllModels(StreamReader reader, DatabaseService database) {
      while (!reader.EndOfStream) {
        string modelLine = reader.ReadLine();
        string[] modelData = modelLine.Split(';');
        int id = int.Parse(modelData[ID_COLUMN]);
        string fileName = modelData[FILENAME_COLUMN];
        string targetVariableName = modelData[TARGETVARIABLE_COLUMN];
        string algoName = modelData[ALGORITHM_COLUMN];
        Variable targetVariable = new Variable(targetVariableName);
        Algorithm algorithm = new Algorithm(algoName);
        Model model = new Model(targetVariable, algorithm);
        model.TrainingSamplesStart = problem.TrainingSamplesStart;
        model.TrainingSamplesEnd = problem.TrainingSamplesEnd;
        model.ValidationSamplesStart = problem.ValidationSamplesStart;
        model.ValidationSamplesEnd = problem.ValidationSamplesEnd;
        model.TestSamplesStart = problem.TestSamplesStart;
        model.TestSamplesEnd = problem.TestSamplesEnd;

        IEnumerable<ModelResult> qualityModelResults = GetModelResults(model, modelData);
        IEnumerable<InputVariableResult> inputVariableResults = GetInputVariableResults(model, modelData);

        // TODO
        //database.Persist(model);
        //foreach (ModelResult modelResult in qualityModelResults)
        //  database.Persist(modelResult);
        //foreach (InputVariableResult inputVariableResult in inputVariableResults)
        //  database.Persist(inputVariableResult);
        
      }
    }

    private IEnumerable<InputVariableResult> GetInputVariableResults(Model model, string[] modelData) {
      double temp;
      return from i in Enumerable.Range(0, inputVariables.Count())
             where inputVariables[i] != null && results[i] != null && double.TryParse(modelData[i], out temp)
             select new InputVariableResult(new InputVariable(model, new Variable(inputVariables[i])), results[i], double.Parse(modelData[i]));
    }

    private IEnumerable<ModelResult> GetModelResults(Model model, string[] modelData) {
      return from i in Enumerable.Range(0, results.Count())
             where results[i] != null
             select new ModelResult(model, results[i], double.Parse(modelData[i]));
    }
  }
}
