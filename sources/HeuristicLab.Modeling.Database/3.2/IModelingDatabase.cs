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
using System.Text;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Modeling.Database {
  public interface IModelingDatabase {
    void Connect();
    void EmptyDatabase();
    void Disconnect();

    IEnumerable<IModel> GetAllModels();
    IEnumerable<IVariable> GetAllVariables();
    IEnumerable<IResult> GetAllResults();
    IEnumerable<IResult> GetAllResultsForInputVariables();
    IEnumerable<IMetaData> GetAllMetaData();
    IEnumerable<IAlgorithm> GetAllAlgorithms();

    IModel Persist(HeuristicLab.Modeling.IAlgorithm algorithm);
    IModel Persist(HeuristicLab.Modeling.IAnalyzerModel model, string algorithmName, string algorithmDescription);

    IModel CreateModel(ModelType modelType, IAlgorithm algorithm, IVariable targetVariable,
     int trainingSamplesStart, int trainingSamplesEnd, int validationSamplesStart, int validationSamplesEnd, int testSamplesStart, int testSamplesEnd);
    IModel CreateModel(string modelName, ModelType modelType, IAlgorithm algorithm, IVariable targetVariable,
      int trainingSamplesStart, int trainingSamplesEnd, int validationSamplesStart, int validationSamplesEnd, int testSamplesStart, int testSamplesEnd);
    void PersistModel(IModel model);
    void DeleteModel(IModel model);

    Dataset GetDataset();
    void PersistProblem(Dataset dataset);
    IVariable GetVariable(string variableName);

    IPredictor GetModelPredictor(IModel model);
    void PersistPredictor(IModel model, IPredictor predictor);

    IAlgorithm GetOrPersistAlgorithm(string algorithmName);

    IResult GetOrPersistResult(string resultName);
    IMetaData GetOrPersistMetaData(string metaDataName);

    IEnumerable<IModelResult> GetModelResults(IModel model);
    IEnumerable<IInputVariableResult> GetInputVariableResults(IModel model);
    IEnumerable<IModelMetaData> GetModelMetaData(IModel model);

    IModelResult CreateModelResult(IModel model, IResult result, double value);
    void PersistModelResults(IModel model, IEnumerable<IModelResult> modelResults);
    IInputVariableResult CreateInputVariableResult(IInputVariable inputVariable, IResult result, double value);
    void PersistInputVariableResults(IModel model, IEnumerable<IInputVariableResult> inputVariableResults);
    IModelMetaData CreateModelMetaData(IModel model, IMetaData metadata, double value);
    void PersistModelMetaData(IModel model, IEnumerable<IModelMetaData> modelMetaData);
  }
}
