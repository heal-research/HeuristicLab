#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using HeuristicLab.Core;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Data;
using System.Data.Linq;

namespace HeuristicLab.Modeling.Database.SQLServerCompact {
  public class DatabaseService : IModelingDatabase {

    private readonly string connection;
    public DatabaseService(string connection) {
      this.connection = connection;
      Connect();
      if (!ctx.DatabaseExists())
        ctx.CreateDatabase();
    }

    private void EmptyDatabase() {
      ctx.DeleteDatabase();
      ctx.CreateDatabase();
    }

    private ModelingDataContext ctx;
    public void Connect() {
      if (ctx != null)
        return;

      ctx = new ModelingDataContext(connection);
      DataLoadOptions dlo = new DataLoadOptions();
      dlo.LoadWith<ModelResult>(mr => mr.Result);
      dlo.LoadWith<InputVariableResult>(ir => ir.Variable);
      dlo.LoadWith<InputVariableResult>(ir => ir.Result);
      dlo.LoadWith<Model>(m => m.TargetVariable);
      ctx.LoadOptions = dlo;
    }

    public void Disconnect() {
      if (ctx == null)
        return;
      ctx.Dispose();
      ctx = null;
    }

    public void Persist(HeuristicLab.Modeling.IAlgorithm algorithm) {
      GetOrCreateProblem(algorithm.Dataset);
      Persist(algorithm.Model, algorithm.Name, algorithm.Description);
    }

    public void Persist(HeuristicLab.Modeling.IAnalyzerModel model, string algorithmName, string algorithmDescription) {
      Dictionary<string, Variable> variables = GetAllVariables();
      Algorithm algo = GetOrCreateAlgorithm(algorithmName, algorithmDescription);
      Variable target = variables[model.TargetVariable];
      Model m;

      using (ModelingDataContext ctx = new ModelingDataContext(connection)) {
        m = new Model(target, algo);
        m.TrainingSamplesStart = model.TrainingSamplesStart;
        m.TrainingSamplesEnd = model.TrainingSamplesEnd;
        m.ValidationSamplesStart = model.ValidationSamplesStart;
        m.ValidationSamplesEnd = model.ValidationSamplesEnd;
        m.TestSamplesStart = model.TestSamplesStart;
        m.TestSamplesEnd = model.TestSamplesEnd;

        ctx.Models.InsertOnSubmit(m);

        ctx.SubmitChanges();
      }

      using (ModelingDataContext ctx = new ModelingDataContext(connection)) {
        ctx.ModelData.InsertOnSubmit(new ModelData(m, PersistenceManager.SaveToGZip(model.Predictor)));
        ctx.SubmitChanges();
      }

      using (ModelingDataContext ctx = new ModelingDataContext(connection)) {
        foreach (string inputVariable in model.InputVariables) {
          ctx.InputVariables.InsertOnSubmit(new InputVariable(m, variables[inputVariable]));
        }
        ctx.SubmitChanges();
      }

      using (ModelingDataContext ctx = new ModelingDataContext(connection)) {
        //get all double properties to save as modelResult
        IEnumerable<PropertyInfo> modelResultInfos = model.GetType().GetProperties().Where(
          info => info.PropertyType == typeof(double));
        foreach (PropertyInfo modelResultInfo in modelResultInfos) {
          Result result = GetOrCreateResult(modelResultInfo.Name);
          double value = (double)modelResultInfo.GetValue(model, null);
          ctx.ModelResults.InsertOnSubmit(new ModelResult(m, result, value));
        }
        ctx.SubmitChanges();
      }

      using (ModelingDataContext ctx = new ModelingDataContext(connection)) {
        IEnumerable<MethodInfo> inputVariableResultInfos = model.GetType().GetMethods().Where(
          info => info.GetParameters().Count() == 1 &&
             info.GetParameters()[0].ParameterType == typeof(string) &&
             info.GetParameters()[0].Name == "variableName" &&
             info.ReturnParameter.ParameterType == typeof(double) &&
             info.Name.StartsWith("Get"));
        foreach (MethodInfo inputVariableResultInfo in inputVariableResultInfos) {
          Result result = GetOrCreateResult(inputVariableResultInfo.Name.Substring(3));
          foreach (InputVariable variable in ctx.InputVariables.Where(iv => iv.Model == m)) {
            double value = (double)inputVariableResultInfo.Invoke(model, new object[] { variable.Variable.Name });
            ctx.InputVariableResults.InsertOnSubmit(new InputVariableResult(variable, result, value));
          }
        }
        ctx.SubmitChanges();
      }
    }

    #region Problem

    public Dataset GetDataset() {
      if (ctx.Problems.Count() != 1)
        throw new InvalidOperationException("Could not get dataset. No or more than one problems are persisted in the database.");

      Problem problem = ctx.Problems.Single();
      return problem.Dataset;

    }

    public Problem GetOrCreateProblem(Dataset dataset) {
      Problem problem;
      if (ctx.Problems.Count() == 0)
        problem = PersistProblem(dataset);
      else
        problem = ctx.Problems.Single();
      if (problem.Dataset.ToString() != dataset.ToString())
        throw new InvalidOperationException("Could not persist dataset. The database already contains a different dataset.");
      return problem;
    }

    private Problem PersistProblem(Dataset dataset) {
      Problem problem;
      using (ModelingDataContext ctx = new ModelingDataContext(connection)) {
        if (ctx.Problems.Count() != 0)
          throw new InvalidOperationException("Could not persist dataset. A dataset is already saved in the database.");
        problem = new Problem(dataset);
        ctx.Problems.InsertOnSubmit(problem);
        foreach (string variable in dataset.VariableNames) {
          ctx.Variables.InsertOnSubmit(new Variable(variable));
        }
        ctx.SubmitChanges();
      }
      return problem;
    }

    #endregion

    #region Algorithm
    public Algorithm GetOrCreateAlgorithm(string name, string description) {
      Algorithm algorithm;
      using (ModelingDataContext ctx = new ModelingDataContext(connection)) {
        var algorithms = from algo in ctx.Algorithms
                         where algo.Name == name && algo.Description == description
                         select algo;
        if (algorithms.Count() == 0) {
          algorithm = new Algorithm(name, description);
          ctx.Algorithms.InsertOnSubmit(algorithm);
          ctx.SubmitChanges();
        } else if (algorithms.Count() == 1)
          algorithm = algorithms.Single();
        else
          throw new ArgumentException("Could not get Algorithm. More than one algorithm with the name " + name + " are saved in database.");
      }
      return algorithm;
    }

    public IEnumerable<IAlgorithm> GetAllAlgorithms() {
      return ctx.Algorithms.ToList().Cast<IAlgorithm>();
    }
    #endregion

    #region Variables
    public Dictionary<string, Variable> GetAllVariables() {
      Dictionary<string, Variable> dict = new Dictionary<string, Variable>();
      dict = ctx.Variables.ToDictionary<Variable, string>(delegate(Variable v) { return v.Name; });
      return dict;
    }
    #endregion

    #region Result
    public Result GetOrCreateResult(string name) {
      Result result;
      using (ModelingDataContext ctx = new ModelingDataContext(connection)) {
        var results = from r in ctx.Results
                      where r.Name == name
                      select r;
        if (results.Count() == 0) {
          result = new Result(name);
          ctx.Results.InsertOnSubmit(result);
          ctx.SubmitChanges();
        } else if (results.Count() == 1)
          result = results.Single();
        else
          throw new ArgumentException("Could not get result. More than one result with the name " + name + " are saved in database.");
      }
      return result;
    }

    public IEnumerable<IResult> GetAllResults() {
      return ctx.Results.ToList().Cast<IResult>();
    }

    public IEnumerable<IResult> GetAllResultsForInputVariables() {
      return (from ir in ctx.InputVariableResults select ir.Result).Distinct().ToList().Cast<IResult>();
    }

    #endregion

    #region ModelResult
    public IEnumerable<IModelResult> GetModelResults(IModel model) {
      var results = from result in ctx.ModelResults
                    where result.Model == model
                    select result;
      return results.ToList().Cast<IModelResult>();
    }
    #endregion

    #region InputVariableResults
    public IEnumerable<IInputVariableResult> GetInputVariableResults(IModel model) {
      var inputResults = from ir in ctx.InputVariableResults
                         where ir.Model == model
                         select ir;
      return inputResults.ToList().Cast<IInputVariableResult>();
    }

    #endregion

    #region Model
    public IEnumerable<IModel> GetAllModels() {
      return ctx.Models.ToList().Cast<IModel>();
    }

    public byte[] GetModelData(IModel model) {
      var data = (from md in ctx.ModelData
                  where md.Model == model
                  select md);
      if (data.Count() == 0)
        return null;
      return data.Single().Data;
    }
    #endregion

  }
}
