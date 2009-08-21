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
    void Persist(HeuristicLab.Modeling.IAnalyzerModel model, string algorithmName, string algorithmDescription);
    void Persist(HeuristicLab.Modeling.IAlgorithm algorithm);
    IProblem PersistProblem(Dataset dataset);

    IEnumerable<IModel> GetAllModels();
    IEnumerable<IResult> GetAllResults();
    IEnumerable<IResult> GetAllResultsForInputVariables();
    IEnumerable<IAlgorithm> GetAllAlgorithms();
       
    Dataset GetDataset();
    byte[] GetModelData(IModel model);
    IPredictor GetModelPredictor(IModel model);
    IEnumerable<IModelResult> GetModelResults(IModel model);
    IEnumerable<IInputVariableResult> GetInputVariableResults(IModel model);

    void Connect();
    void Disconnect();
  }
}
