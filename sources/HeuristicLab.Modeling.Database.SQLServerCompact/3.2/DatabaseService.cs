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

    public void EmptyDatabase() {
      ctx.Connection.Dispose();
      ctx.DeleteDatabase();
      Connect();
      ctx.CreateDatabase();
    }

    private ModelingDataContext ctx;
    public void Connect() {
      if (ctx != null)
        Disconnect();

      ctx = new ModelingDataContext(connection);
      DataLoadOptions dlo = new DataLoadOptions();
      dlo.LoadWith<ModelResult>(mr => mr.Result);
      dlo.LoadWith<ModelMetaData>(mmd => mmd.MetaData);
      dlo.LoadWith<InputVariableResult>(ir => ir.Variable);
      dlo.LoadWith<InputVariableResult>(ir => ir.Result);
      dlo.LoadWith<Model>(m => m.TargetVariable);
      dlo.LoadWith<Model>(m => m.Algorithm);
      ctx.LoadOptions = dlo;
    }

    public void Disconnect() {
      if (ctx == null)
        return;
      ctx.Connection.Dispose();
      ctx.Dispose();
      ctx = null;
    }

    public IEnumerable<IModel> GetAllModels() {
      return ctx.Models.ToList().Cast<IModel>();
    }

    public IEnumerable<IVariable> GetAllVariables() {
      return ctx.Variables.ToList().Cast<IVariable>();
    }

    public IEnumerable<IResult> GetAllResults() {
      return ctx.Results.ToList().Cast<IResult>();
    }

    public IEnumerable<IResult> GetAllResultsForInputVariables() {
      return (from ir in ctx.InputVariableResults select ir.Result).Distinct().ToList().Cast<IResult>();
    }

    public IEnumerable<IMetaData> GetAllMetaData() {
      return ctx.MetaData.ToList().Cast<IMetaData>();
    }

    public IEnumerable<IAlgorithm> GetAllAlgorithms() {
      return ctx.Algorithms.ToList().Cast<IAlgorithm>();
    }

    public IModel CreateModel(ModelType modelType, IAlgorithm algorithm, IVariable targetVariable,
int trainingSamplesStart, int trainingSamplesEnd, int validationSamplesStart, int validationSamplesEnd, int testSamplesStart, int testSamplesEnd) {
      return CreateModel(null, modelType, algorithm, targetVariable, trainingSamplesStart, trainingSamplesEnd, validationSamplesStart, validationSamplesEnd, testSamplesStart, testSamplesEnd);
    }

    public IModel CreateModel(string modelName, ModelType modelType, IAlgorithm algorithm, IVariable targetVariable,
     int trainingSamplesStart, int trainingSamplesEnd, int validationSamplesStart, int validationSamplesEnd, int testSamplesStart, int testSamplesEnd) {
      Variable target = (Variable)targetVariable;
      Algorithm algo = (Algorithm)algorithm;
      Model model = new Model(target, algo, modelType);
      model.Name = modelName;
      model.TrainingSamplesStart = trainingSamplesStart;
      model.TrainingSamplesEnd = trainingSamplesEnd;
      model.ValidationSamplesStart = validationSamplesStart;
      model.ValidationSamplesEnd = validationSamplesEnd;
      model.TestSamplesStart = testSamplesStart;
      model.TestSamplesEnd = testSamplesEnd;

      return model;
    }

    public void PersistModel(IModel model) {
      using (ModelingDataContext ctx = new ModelingDataContext(connection)) {
        Model m = (Model)model;
        Model orginal = ctx.Models.GetOriginalEntityState(m);
        if (orginal == null)
          ctx.Models.Attach(m);
        ctx.Refresh(RefreshMode.KeepCurrentValues, m);
        ctx.SubmitChanges();
      }
    }

    public void DeleteModel(IModel model) {
      Model m = (Model)model;
      ctx.ModelData.DeleteAllOnSubmit(ctx.ModelData.Where(x => x.Model == m));
      ctx.ModelMetaData.DeleteAllOnSubmit(ctx.ModelMetaData.Where(x => x.Model == m));
      ctx.ModelResults.DeleteAllOnSubmit(ctx.ModelResults.Where(x => x.Model == m));
      ctx.InputVariableResults.DeleteAllOnSubmit(ctx.InputVariableResults.Where(x => x.Model == m));
      ctx.InputVariables.DeleteAllOnSubmit(ctx.InputVariables.Where(x => x.Model == m));
      Model orginal = ctx.Models.GetOriginalEntityState(m);
      if (orginal == null)
        ctx.Models.Attach(m);
      ctx.Models.DeleteOnSubmit(m);
      ctx.SubmitChanges();
    }

    public Dataset GetDataset() {
      if (ctx.Problems.Count() != 1)
        throw new InvalidOperationException("Could not get dataset. No or more than one problems are persisted in the database.");
      Problem problem = ctx.Problems.Single();
      return problem.Dataset;
    }

    public void PersistProblem(Dataset dataset) {
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
    }

    public IVariable GetVariable(string variableName) {
      var variables = ctx.Variables.Where(v => v.Name == variableName);
      if (variables.Count() != 1)
        throw new ArgumentException("Zero or more than one variable with the name " + variableName + " are persisted in the database.");
      return variables.Single();
    }

    public IPredictor GetModelPredictor(IModel model) {
      var data = (from md in ctx.ModelData
                  where md.Model == model
                  select md);
      if (data.Count() != 1)
        throw new ArgumentException("No predictor persisted for given model!");
      return (IPredictor)PersistenceManager.RestoreFromGZip(data.Single().Data);
    }

    public void PersistPredictor(IModel model, IPredictor predictor) {
      Model m = (Model)model;
      using (ModelingDataContext ctx = new ModelingDataContext(connection)) {
        ctx.ModelData.DeleteAllOnSubmit(ctx.ModelData.Where(x => x.Model == m));
        ctx.ModelResults.DeleteAllOnSubmit(ctx.ModelResults.Where(x => x.Model == m));
        ctx.InputVariableResults.DeleteAllOnSubmit(ctx.InputVariableResults.Where(x => x.Model == m));
        ctx.InputVariables.DeleteAllOnSubmit(ctx.InputVariables.Where(x => x.Model == m));

        ctx.ModelData.InsertOnSubmit(new ModelData(m, PersistenceManager.SaveToGZip(predictor)));
        foreach (string variableName in predictor.GetInputVariables())
          ctx.InputVariables.InsertOnSubmit(new InputVariable(m, (Variable)GetVariable(variableName)));

        ctx.SubmitChanges();
      }
    }

    public IAlgorithm GetOrPersistAlgorithm(string algorithmName) {
      Algorithm algorithm;
      using (ModelingDataContext ctx = new ModelingDataContext(connection)) {
        var algorithms = ctx.Algorithms.Where(algo => algo.Name == algorithmName);
        if (algorithms.Count() == 0) {
          algorithm = new Algorithm(algorithmName, "");
          ctx.Algorithms.InsertOnSubmit(algorithm);
          ctx.SubmitChanges();
        } else if (algorithms.Count() == 1)
          algorithm = algorithms.Single();
        else
          throw new ArgumentException("Could not get Algorithm. More than one algorithm with the name " + algorithmName + " are saved in database.");
      }
      return algorithm;
    }

    public IResult GetOrPersistResult(string resultName) {
      Result result;
      using (ModelingDataContext ctx = new ModelingDataContext(connection)) {
        var results = ctx.Results.Where(r => r.Name == resultName);
        if (results.Count() == 0) {
          result = new Result(resultName);
          ctx.Results.InsertOnSubmit(result);
          ctx.SubmitChanges();
        } else if (results.Count() == 1)
          result = results.Single();
        else
          throw new ArgumentException("Could not get result. More than one result with the name " + resultName + " are saved in database.");
      }
      return result;
    }

    public IMetaData GetOrPersistMetaData(string metaDataName) {
      MetaData metadata;
      using (ModelingDataContext ctx = new ModelingDataContext(connection)) {
        var md = ctx.MetaData.Where(r => r.Name == metaDataName);
        if (md.Count() == 0) {
          metadata = new MetaData(metaDataName);
          ctx.MetaData.InsertOnSubmit(metadata);
          ctx.SubmitChanges();
        } else if (md.Count() == 1)
          metadata = md.Single();
        else
          throw new ArgumentException("Could not get metadata. More than one metadata with the name " + metaDataName + " are saved in database.");
      }
      return metadata;
    }

    public IEnumerable<IModelResult> GetModelResults(IModel model) {
      return ctx.ModelResults.Where(mr => mr.Model == model).Cast<IModelResult>();
    }
    public IEnumerable<IInputVariableResult> GetInputVariableResults(IModel model) {
      return ctx.InputVariableResults.Where(ivr => ivr.Model == model).Cast<IInputVariableResult>();
    }
    public IEnumerable<IModelMetaData> GetModelMetaData(IModel model) {
      return ctx.ModelMetaData.Where(md => md.Model == model).Cast<IModelMetaData>();
    }

    public IModelResult CreateModelResult(IModel model, IResult result, double value) {
      Model m = (Model)model;
      Result r = (Result)result;
      return new ModelResult(m, r, value);
    }

    public void PersistModelResults(IModel model,IEnumerable<IModelResult> modelResults) {
      using (ModelingDataContext ctx = new ModelingDataContext(connection)) {
        ctx.ModelResults.DeleteAllOnSubmit(GetModelResults(model).Cast<ModelResult>());
        ctx.ModelResults.InsertAllOnSubmit(modelResults.Cast<ModelResult>());
        ctx.SubmitChanges();
      }
    }

    public IInputVariable CreateInputVariable(IModel model, IVariable variable) {
      InputVariable inputVariable = new InputVariable((Model)model, (Variable)variable);
      return inputVariable;
    }

    public IInputVariableResult CreateInputVariableResult(IInputVariable inputVariable, IResult result, double value) {
      InputVariable i = (InputVariable)inputVariable;
      Result r = (Result)result;
      return new InputVariableResult(i, r, value);
    }

    public void PersistInputVariableResults(IModel model,IEnumerable<IInputVariableResult> inputVariableResults) {
      using (ModelingDataContext ctx = new ModelingDataContext(connection)) {
        ctx.InputVariableResults.DeleteAllOnSubmit(GetInputVariableResults(model).Cast<InputVariableResult>());
        ctx.InputVariableResults.InsertAllOnSubmit(inputVariableResults.Cast<InputVariableResult>());
        ctx.SubmitChanges();
      }
    }

    public IModelMetaData CreateModelMetaData(IModel model, IMetaData metadata, double value) {
      Model m = (Model)model;
      MetaData md = (MetaData)metadata;
      return new ModelMetaData(m, md, value);
    }

    public void PersistModelMetaData(IModel model, IEnumerable<IModelMetaData> modelMetaData) {
      using (ModelingDataContext ctx = new ModelingDataContext(connection)) {
        ctx.ModelMetaData.DeleteAllOnSubmit(GetModelMetaData(model).Cast<ModelMetaData>());
        ctx.ModelMetaData.InsertAllOnSubmit(modelMetaData.Cast<ModelMetaData>());
        ctx.SubmitChanges();
      }
    }

    public IModel Persist(HeuristicLab.Modeling.IAlgorithm algorithm) {
      if (ctx.Problems.Count() == 0)
        PersistProblem(algorithm.Dataset);
      return Persist(algorithm.Model, algorithm.Name, algorithm.Description);
    }

    public IModel Persist(HeuristicLab.Modeling.IAnalyzerModel model, string algorithmName, string algorithmDescription) {
      Algorithm algorithm = (Algorithm) GetOrPersistAlgorithm(algorithmName);
      Variable targetVariable  = (Variable)GetVariable (model.TargetVariable);

      Model m = (Model)CreateModel(model.Type, algorithm, targetVariable, model.TrainingSamplesStart, model.TrainingSamplesEnd,
        model.ValidationSamplesStart, model.ValidationSamplesEnd, model.TestSamplesStart, model.TestSamplesEnd);
      PersistModel(m);
      PersistPredictor(m, model.Predictor);

      using (ModelingDataContext ctx = new ModelingDataContext(connection)) {
        foreach (KeyValuePair<string, double> pair in model.MetaData) {
          MetaData metaData = (MetaData)GetOrPersistMetaData(pair.Key);
          ctx.ModelMetaData.InsertOnSubmit(new ModelMetaData(m, metaData, pair.Value));
        }
        ctx.SubmitChanges();
      }

      using (ModelingDataContext ctx = new ModelingDataContext(connection)) {
        foreach (KeyValuePair<ModelingResult, double> pair in model.Results) {
          Result result = (Result)GetOrPersistResult(pair.Key.ToString());
          ctx.ModelResults.InsertOnSubmit(new ModelResult(m, result, pair.Value));
        }
        ctx.SubmitChanges();
      }

      using (ModelingDataContext ctx = new ModelingDataContext(connection)) {
        foreach (InputVariable variable in ctx.InputVariables.Where(iv => iv.Model == m)) {
          foreach (KeyValuePair<ModelingResult, double> variableResult in model.GetVariableResults(variable.Variable.Name)) {
            Result result = (Result)GetOrPersistResult(variableResult.Key.ToString());
            ctx.InputVariableResults.InsertOnSubmit(new InputVariableResult(variable, result, variableResult.Value));
          }
        }
        ctx.SubmitChanges();
      }

      //if connected to database return inserted model
      if (this.ctx != null)
        return this.ctx.Models.Where(x => x.Id == m.Id).Single();
      return null;
    }
  }
}
