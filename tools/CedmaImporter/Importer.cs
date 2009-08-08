using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HeuristicLab.Modeling.Database;
using HeuristicLab.Modeling.Database.SQLServerCompact;
using HeuristicLab.GP;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.GP.StructureIdentification;
using System.Diagnostics;

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
        ImportAllModels(dirName, reader, database);
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

    private void ImportAllModels(string dirName, StreamReader reader, DatabaseService database) {
      while (!reader.EndOfStream) {
        string modelLine = reader.ReadLine();
        string[] modelData = modelLine.Split(';');
        int id = int.Parse(modelData[ID_COLUMN]);
        string targetVariableName = modelData[TARGETVARIABLE_COLUMN].Trim();
        string algoName = modelData[ALGORITHM_COLUMN].Trim();
        HeuristicLab.Core.IItem modelItem = ParseModel(dirName, modelData[FILENAME_COLUMN].Trim(), algoName);
        HeuristicLab.Modeling.Database.SQLServerCompact.Variable targetVariable = new HeuristicLab.Modeling.Database.SQLServerCompact.Variable(targetVariableName);
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
             select new InputVariableResult(new InputVariable(model, new HeuristicLab.Modeling.Database.SQLServerCompact.Variable(inputVariables[i])), results[i], double.Parse(modelData[i]));
    }

    private IEnumerable<ModelResult> GetModelResults(Model model, string[] modelData) {
      return from i in Enumerable.Range(0, results.Count())
             where results[i] != null
             select new ModelResult(model, results[i], double.Parse(modelData[i]));
    }

    Dictionary<string, IFunction> knownFunctions = new Dictionary<string, IFunction>() {
    {"+", new Addition()},
    {"and", new And()},
    {"mean", new Average()},
    {"cos", new Cosinus()},
    {"/", new Division()},
    {"equ", new Equal()},
    {"exp", new Exponential()},
    {">", new GreaterThan()},
    {"if", new IfThenElse()},
    {"<", new LessThan()},
    {"log", new Logarithm()},
    {"*", new Multiplication()},
    {"not", new Not()},
    {"or", new Or()},
    {"expt", new Power()},
    {"sign", new Signum()},
    {"sin",new Sinus()},
    {"sqrt", new Sqrt()},
    {"-", new Subtraction()},
    {"tan", new Tangens()},
    {"xor", new Xor()}
    };
    Constant constant = new Constant();
    HeuristicLab.GP.StructureIdentification.Variable variable = new HeuristicLab.GP.StructureIdentification.Variable();
    Differential differential = new Differential();

    private HeuristicLab.Core.IItem ParseModel(string dirName, string modelFileName, string algoName) {
      if (algoName == "SupportVectorRegression") {
        HeuristicLab.Data.SVMModel model = new HeuristicLab.Data.SVMModel();
        model.Model = SVM.Model.Read(Path.Combine(dirName, modelFileName) + ".svm.model.txt");
        model.RangeTransform = SVM.RangeTransform.Read(Path.Combine(dirName, modelFileName) + ".svm.transform.txt");
        return model;
      } else {
        GeneticProgrammingModel model = new GeneticProgrammingModel();
        IEnumerable<Token> tokens = GetTokenStream(File.OpenText(Path.Combine(dirName, modelFileName) + ".gp.txt"));
        model.FunctionTree = ParseSexp(new Queue<Token>(tokens));
        return model;
      }
    }

    private IEnumerable<Token> GetTokenStream(StreamReader reader) {
      return from line in GetLineStream(reader)
             let strTokens = line.Split(new string[] { " ", "\t", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).AsEnumerable()
             from strToken in strTokens
             let t = Token.Parse(strToken)
             where t != null
             select t;
    }

    private IEnumerable<string> GetLineStream(StreamReader reader) {
      while (!reader.EndOfStream) yield return reader.ReadLine().Replace("(", " ( ").Replace(")", " ) ");
      yield break;
    }

    private HeuristicLab.GP.Interfaces.IFunctionTree ParseSexp(Queue<Token> tokens) {
      Expect(Token.LPAR, tokens);

      if (tokens.Peek().Symbol == TokenSymbol.SYMB) {
        if (tokens.Peek().StringValue.Equals("variable")) {
          return ParseVariable(tokens);
        } else if (tokens.Peek().StringValue.Equals("differential")) {
          return ParseDifferential(tokens);
        } else {
          Token curToken = tokens.Dequeue();
          IFunctionTree tree = CreateTree(curToken);
          while (!tokens.Peek().Equals(Token.RPAR)) {
            tree.AddSubTree(ParseSexp(tokens));
          }
          Expect(Token.RPAR, tokens);
          return tree;
        }
      } else if (tokens.Peek().Symbol == TokenSymbol.NUMBER) {
        ConstantFunctionTree t = (ConstantFunctionTree)constant.GetTreeNode();
        t.Value = tokens.Dequeue().DoubleValue;
        return t;
      } else {
        throw new FormatException("Expected function or constant symbol");
      }
    }

    private IFunctionTree ParseDifferential(Queue<Token> tokens) {
      Debug.Assert(tokens.Dequeue().StringValue == "differential");
      VariableFunctionTree t = (VariableFunctionTree)differential.GetTreeNode();
      t.Weight = tokens.Dequeue().DoubleValue;
      t.VariableName = tokens.Dequeue().StringValue;
      t.SampleOffset = (int)tokens.Dequeue().DoubleValue;
      Expect(Token.RPAR, tokens);
      return t;
    }

    private IFunctionTree ParseVariable(Queue<Token> tokens) {
      Debug.Assert(tokens.Dequeue().StringValue == "variable");
      VariableFunctionTree t = (VariableFunctionTree)variable.GetTreeNode();
      t.Weight = tokens.Dequeue().DoubleValue;
      t.VariableName = tokens.Dequeue().StringValue;
      t.SampleOffset = (int)tokens.Dequeue().DoubleValue;
      Expect(Token.RPAR, tokens);
      return t;
    }

    private IFunctionTree CreateTree(Token token) {
      if (token.Symbol != TokenSymbol.SYMB) throw new FormatException("Expected function symbol, but got: " + token.StringValue);
      return knownFunctions[token.StringValue].GetTreeNode();
    }

    private void Expect(Token token, Queue<Token> tokens) {
      Token cur = tokens.Dequeue();
      if (!token.Equals(cur)) throw new FormatException("Expected: " + token.StringValue + ", but got found: " + cur.StringValue);
    }

    private enum TokenSymbol { LPAR, RPAR, SYMB, NUMBER };
    private class Token {
      public static readonly Token LPAR = Token.Parse("(");
      public static readonly Token RPAR = Token.Parse(")");

      public TokenSymbol Symbol { get; set; }
      public string StringValue { get; set; }
      public double DoubleValue { get; set; }
      public Token() { }

      public override bool Equals(object obj) {
        Token other = (obj as Token);
        if (other == null) return false;
        if (other.Symbol != Symbol) return false;
        return other.StringValue == this.StringValue;
      }

      public static Token Parse(string strToken) {
        strToken = strToken.Trim();
        Token t = new Token();
        t.StringValue = strToken.Trim();
        double temp;
        if (strToken == "") {
          t = null;
        } else if (strToken == "(") {
          t.Symbol = TokenSymbol.LPAR;
        } else if (strToken == ")") {
          t.Symbol = TokenSymbol.RPAR;
        } else if (double.TryParse(strToken, out temp)) {
          t.Symbol = TokenSymbol.NUMBER;
          t.DoubleValue = double.Parse(strToken);
        } else {
          t.Symbol = TokenSymbol.SYMB;
        }
        return t;
      }
    }
  }
}
