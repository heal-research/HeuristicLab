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
    private readonly string fileName;

    public DatabaseService(string fileName) {
      this.fileName = fileName;
      this.readOnly = false;

    }
    public DatabaseService(string fileName, bool readOnly)
      : this(fileName) {
      this.readOnly = readOnly;
    }

    private string ConnectionString {
      get {
        string connection = "Data Source =" + fileName + ";";
        if (this.readOnly)
          connection += "File Mode = Read Only; Temp Path =" + System.IO.Path.GetTempPath() + ";";
        else
          connection += "File Mode = Shared Read;";
        return connection;
      }
    }

    private bool readOnly;
    public bool ReadOnly {
      get { return this.readOnly; }
      set {
        if (ctx != null)
          throw new InvalidOperationException("Could not change readonly attribute of DatabaseService because connection is opened.");
        this.readOnly = value;
      }
    }

    public void EmptyDatabase() {
      ctx.Connection.Dispose();
      ctx.DeleteDatabase();
      Connect();
      ctx.CreateDatabase();
      Disconnect();
    }

    private ModelingDataContext ctx;
    public void Connect() {
      if (ctx != null)
        Disconnect();

      ctx = new ModelingDataContext(this.ConnectionString);
      DataLoadOptions dlo = new DataLoadOptions();
      dlo.LoadWith<ModelResult>(mr => mr.Result);
      dlo.LoadWith<ModelMetaData>(mmd => mmd.MetaData);
      dlo.LoadWith<InputVariableResult>(ir => ir.Variable);
      dlo.LoadWith<InputVariableResult>(ir => ir.Result);
      dlo.LoadWith<Model>(m => m.TargetVariable);
      dlo.LoadWith<Model>(m => m.Algorithm);
      ctx.LoadOptions = dlo;
      //ctx.Log = System.Console.Out;

      if (!ctx.DatabaseExists() && !this.ReadOnly)
        ctx.CreateDatabase();
      else
        ctx.Connection.Open();
    }

    public void Disconnect() {
      if (ctx == null)
        return;
      ctx.Connection.Close();
      ctx.Connection.Dispose();
      ctx.Dispose();
      ctx = null;
    }

    public void Commit() {
      if (ctx != null)
        ctx.SubmitChanges();
    }

    private void CheckConnection() {
      CheckConnection(false);
    }

    private void CheckConnection(bool writeEnabled) {
      if (ctx == null)
        throw new InvalidOperationException("Could not perform operation, when not connected to the database.");
      if (writeEnabled && this.ReadOnly)
        throw new InvalidOperationException("Could not perform update operation, when database is in readonly mode.");
    }

    public IEnumerable<IModel> GetAllModels() {
      this.CheckConnection();
      return ctx.Models.ToList().Cast<IModel>();
    }

    public IEnumerable<int> GetAllModelIds() {
      this.CheckConnection();
      return from m in ctx.Models
             select m.Id;
    }

    public IEnumerable<IVariable> GetAllVariables() {
      this.CheckConnection();
      return ctx.Variables.ToList().Cast<IVariable>();
    }

    public IEnumerable<IResult> GetAllResults() {
      this.CheckConnection();
      return ctx.Results.ToList().Cast<IResult>();
    }

    public IEnumerable<IResult> GetAllResultsForInputVariables() {
      this.CheckConnection();
      return (from ir in ctx.InputVariableResults select ir.Result).Distinct().ToList().Cast<IResult>();
    }

    public IEnumerable<IMetaData> GetAllMetaData() {
      this.CheckConnection();
      return ctx.MetaData.ToList().Cast<IMetaData>();
    }

    public IEnumerable<IAlgorithm> GetAllAlgorithms() {
      this.CheckConnection();
      return ctx.Algorithms.ToList().Cast<IAlgorithm>();
    }

    public IModel CreateModel(int id, string modelName, ModelType modelType, IAlgorithm algorithm, IVariable targetVariable,
        int trainingSamplesStart, int trainingSamplesEnd, int validationSamplesStart, int validationSamplesEnd, int testSamplesStart, int testSamplesEnd) {
      Model m = (Model)CreateModel(modelName, modelType, algorithm, targetVariable, trainingSamplesStart, trainingSamplesEnd, validationSamplesStart, validationSamplesEnd, testSamplesStart, testSamplesEnd);
      m.Id = id;
      return m;
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

    public IModel GetModel(int id) {
      this.CheckConnection();
      var model = ctx.Models.Where(m => m.Id == id);
      if (model.Count() == 1)
        return model.Single();
      return null;
    }

    public void PersistModel(IModel model) {
      this.CheckConnection(true);
      Model m = (Model)model;
      //check if model has to be updated or inserted
      if (ctx.Models.Any(x => x.Id == model.Id)) {
        Model orginal = ctx.Models.GetOriginalEntityState(m);
        if (orginal == null)
          ctx.Models.Attach(m);
        ctx.Refresh(RefreshMode.KeepCurrentValues, m);
      } else
        ctx.Models.InsertOnSubmit(m);
    }

    public void DeleteModel(IModel model) {
      this.CheckConnection(true);
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
      this.CheckConnection();
      if (ctx.Problems.Count() > 1)
        throw new InvalidOperationException("Could not get dataset. More than one problems are persisted in the database.");
      if (ctx.Problems.Count() == 1)
        return ctx.Problems.Single().Dataset;
      return null;
    }

    public void PersistProblem(Dataset dataset) {
      this.CheckConnection(true);
      Problem problem;
      if (ctx.Problems.Count() != 0)
        throw new InvalidOperationException("Could not persist dataset. A dataset is already saved in the database.");
      problem = new Problem(dataset);
      ctx.Problems.InsertOnSubmit(problem);
      foreach (string variable in dataset.VariableNames) {
        ctx.Variables.InsertOnSubmit(new Variable(variable));
      }
    }

    public IVariable GetVariable(string variableName) {
      this.CheckConnection();
      var variables = ctx.Variables.Where(v => v.Name == variableName);
      if (variables.Count() != 1)
        throw new ArgumentException("Zero or more than one variable with the name " + variableName + " are persisted in the database.");
      return variables.Single();
    }

    public IPredictor GetModelPredictor(IModel model) {
      this.CheckConnection();
      var data = (from md in ctx.ModelData
                  where md.Model == model
                  select md);
      if (data.Count() != 1)
        throw new ArgumentException("No predictor persisted for given model!");
      return (IPredictor)PersistenceManager.RestoreFromGZip(data.Single().Data);
    }

    public void PersistPredictor(IModel model, IPredictor predictor) {
      this.CheckConnection(true);
      Model m = (Model)model;
      ctx.ModelData.DeleteAllOnSubmit(ctx.ModelData.Where(x => x.Model == m));
      ctx.ModelResults.DeleteAllOnSubmit(ctx.ModelResults.Where(x => x.Model == m));
      ctx.InputVariableResults.DeleteAllOnSubmit(ctx.InputVariableResults.Where(x => x.Model == m));
      ctx.InputVariables.DeleteAllOnSubmit(ctx.InputVariables.Where(x => x.Model == m));

      ctx.ModelData.InsertOnSubmit(new ModelData(m, PersistenceManager.SaveToGZip(predictor)));
      foreach (string variableName in predictor.GetInputVariables())
        ctx.InputVariables.InsertOnSubmit(new InputVariable(m, (Variable)GetVariable(variableName)));
    }

    public IInputVariable GetInputVariable(IModel model, string inputVariableName) {
      this.CheckConnection();
      var inputVariables = ctx.InputVariables.Where(i => i.Model == model && i.Variable.Name == inputVariableName);
      if (inputVariables.Count() == 1)
        return inputVariables.Single();

      if (inputVariables.Count() > 1)
        throw new ArgumentException("More than one input variable with the same name are for the given model persisted.");

      return null;
    }

    public IAlgorithm GetOrPersistAlgorithm(string algorithmName) {
      this.CheckConnection();
      Algorithm algorithm;
      var algorithms = ctx.Algorithms.Where(algo => algo.Name == algorithmName);
      if (algorithms.Count() == 0) {
        algorithm = new Algorithm(algorithmName, "");
        this.CheckConnection(true);
        ctx.Algorithms.InsertOnSubmit(algorithm);
        ctx.SubmitChanges();
      } else if (algorithms.Count() == 1)
        algorithm = algorithms.Single();
      else
        throw new ArgumentException("Could not get Algorithm. More than one algorithm with the name " + algorithmName + " are saved in database.");
      return algorithm;
    }

    public IResult GetOrPersistResult(string resultName) {
      this.CheckConnection();
      Result result;
      var results = ctx.Results.Where(r => r.Name == resultName);
      if (results.Count() == 0) {
        this.CheckConnection(true);
        result = new Result(resultName);
        ctx.Results.InsertOnSubmit(result);
        ctx.SubmitChanges();
      } else if (results.Count() == 1)
        result = results.Single();
      else
        throw new ArgumentException("Could not get result. More than one result with the name " + resultName + " are saved in database.");
      return result;
    }

    public IMetaData GetOrPersistMetaData(string metaDataName) {
      this.CheckConnection();
      MetaData metadata;
      var md = ctx.MetaData.Where(r => r.Name == metaDataName);
      if (md.Count() == 0) {
        this.CheckConnection(true);
        metadata = new MetaData(metaDataName);
        ctx.MetaData.InsertOnSubmit(metadata);
        ctx.SubmitChanges();
      } else if (md.Count() == 1)
        metadata = md.Single();
      else
        throw new ArgumentException("Could not get metadata. More than one metadata with the name " + metaDataName + " are saved in database.");
      return metadata;
    }

    public IEnumerable<IModelResult> GetModelResults(IModel model) {
      this.CheckConnection();
      return ctx.ModelResults.Where(mr => mr.Model == model).Cast<IModelResult>();
    }
    public IEnumerable<IInputVariableResult> GetInputVariableResults(IModel model) {
      this.CheckConnection();
      return ctx.InputVariableResults.Where(ivr => ivr.Model == model).Cast<IInputVariableResult>();
    }
    public IEnumerable<IModelMetaData> GetModelMetaData(IModel model) {
      this.CheckConnection();
      return ctx.ModelMetaData.Where(md => md.Model == model).Cast<IModelMetaData>();
    }

    public IModelResult CreateModelResult(IModel model, IResult result, double value) {
      Model m = (Model)model;
      Result r = (Result)result;
      return new ModelResult(m, r, value);
    }

    public void PersistModelResults(IModel model, IEnumerable<IModelResult> modelResults) {
      this.CheckConnection(true);
      ctx.ModelResults.DeleteAllOnSubmit(GetModelResults(model).Cast<ModelResult>());
      ctx.ModelResults.InsertAllOnSubmit(modelResults.Cast<ModelResult>());
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

    public void PersistInputVariableResults(IModel model, IEnumerable<IInputVariableResult> inputVariableResults) {
      this.CheckConnection(true);
      ctx.InputVariableResults.DeleteAllOnSubmit(GetInputVariableResults(model).Cast<InputVariableResult>());
      ctx.InputVariableResults.InsertAllOnSubmit(inputVariableResults.Cast<InputVariableResult>());
    }

    public IModelMetaData CreateModelMetaData(IModel model, IMetaData metadata, double value) {
      Model m = (Model)model;
      MetaData md = (MetaData)metadata;
      return new ModelMetaData(m, md, value);
    }

    public void PersistModelMetaData(IModel model, IEnumerable<IModelMetaData> modelMetaData) {
      this.CheckConnection(true);
      ctx.ModelMetaData.DeleteAllOnSubmit(GetModelMetaData(model).Cast<ModelMetaData>());
      ctx.ModelMetaData.InsertAllOnSubmit(modelMetaData.Cast<ModelMetaData>());
    }

    public void Persist(HeuristicLab.Modeling.IAnalyzerModel model, string algorithmName, string algorithmDescription) {
      this.CheckConnection(true);
      Algorithm algorithm = (Algorithm)GetOrPersistAlgorithm(algorithmName);
      Variable targetVariable = (Variable)GetVariable(model.TargetVariable);
      Model m = (Model)CreateModel(null, model.Type, algorithm, targetVariable, model.TrainingSamplesStart, model.TrainingSamplesEnd,
        model.ValidationSamplesStart, model.ValidationSamplesEnd, model.TestSamplesStart, model.TestSamplesEnd);
      ctx.Models.InsertOnSubmit(m);
      ctx.SubmitChanges();
      ctx.ModelData.InsertOnSubmit(new ModelData(m, PersistenceManager.SaveToGZip(model.Predictor)));

      foreach (string variableName in model.Predictor.GetInputVariables())
        ctx.InputVariables.InsertOnSubmit(new InputVariable(m, (Variable)GetVariable(variableName)));
      ctx.SubmitChanges();

      foreach (KeyValuePair<string, double> pair in model.MetaData) {
        MetaData metaData = (MetaData)GetOrPersistMetaData(pair.Key);
        ctx.ModelMetaData.InsertOnSubmit(new ModelMetaData(m, metaData, pair.Value));
      }

      foreach (KeyValuePair<ModelingResult, double> pair in model.Results) {
        Result result = (Result)GetOrPersistResult(pair.Key.ToString());
        ctx.ModelResults.InsertOnSubmit(new ModelResult(m, result, pair.Value));
      }

      foreach (InputVariable variable in ctx.InputVariables.Where(iv => iv.Model == m)) {
        foreach (KeyValuePair<ModelingResult, double> variableResult in model.GetVariableResults(variable.Variable.Name)) {
          Result result = (Result)GetOrPersistResult(variableResult.Key.ToString());
          ctx.InputVariableResults.InsertOnSubmit(new InputVariableResult(variable, result, variableResult.Value));
        }
      }
      ctx.SubmitChanges();
    }
  }
}
