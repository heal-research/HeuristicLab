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
using System.Text;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;
using System.Net;
using System.ServiceModel;
using HeuristicLab.CEDMA.DB.Interfaces;
using HeuristicLab.CEDMA.DB;
using System.ServiceModel.Description;
using System.Linq;
using HeuristicLab.CEDMA.Core;
using HeuristicLab.GP.StructureIdentification;
using HeuristicLab.Data;
using HeuristicLab.Core;
using HeuristicLab.Modeling;

namespace HeuristicLab.CEDMA.Server {
  public abstract class DispatcherBase : IDispatcher {
    private IStore store;
    private DataSet dataset;
    internal event EventHandler Changed;
    public IEnumerable<string> AllowedTargetVariables {
      get {
        if (dataset != null) {
          return dataset.Problem.AllowedTargetVariables.Select(x => dataset.Problem.Dataset.GetVariableName(x));
        } else return new string[0];
      }
    }

    public IEnumerable<string> AllowedInputVariables {
      get {
        if (dataset != null) {
          return dataset.Problem.AllowedInputVariables.Select(x => dataset.Problem.Dataset.GetVariableName(x));
        } else return new string[0];
      }
    }

    public DispatcherBase(IStore store) {
      this.store = store;
    }

    public IAlgorithm GetNextJob() {
      if (dataset == null) {
        var datasetEntities = store.Query("?DataSet <" + Ontology.InstanceOf.Uri + "> <" + Ontology.TypeDataSet.Uri + "> .", 0, 1)
          .Select(x => (Entity)x.Get("DataSet"));
        if (datasetEntities.Count() == 0) return null;
        dataset = new DataSet(store, datasetEntities.ElementAt(0));
        OnChanged();
      }
      int targetVariable = SelectTargetVariable(dataset.Problem.AllowedTargetVariables.ToArray());
      IAlgorithm selectedAlgorithm = SelectAlgorithm(targetVariable, dataset.Problem.LearningTask);
      string targetVariableName = dataset.Problem.GetVariableName(targetVariable);

      if (selectedAlgorithm != null) {
        SetProblemParameters(selectedAlgorithm, dataset.Problem, targetVariable);
      }
      return selectedAlgorithm;
    }

    public abstract int SelectTargetVariable(int[] targetVariables);
    public abstract IAlgorithm SelectAlgorithm(int targetVariable, LearningTask learningTask);

    private void SetProblemParameters(IAlgorithm algo, Problem problem, int targetVariable) {
      algo.Dataset = problem.Dataset;
      algo.TargetVariable = targetVariable;
      algo.ProblemInjector.GetVariable("TrainingSamplesStart").GetValue<IntData>().Data = problem.TrainingSamplesStart;
      algo.ProblemInjector.GetVariable("TrainingSamplesEnd").GetValue<IntData>().Data = problem.TrainingSamplesEnd;
      algo.ProblemInjector.GetVariable("ValidationSamplesStart").GetValue<IntData>().Data = problem.ValidationSamplesStart;
      algo.ProblemInjector.GetVariable("ValidationSamplesEnd").GetValue<IntData>().Data = problem.ValidationSamplesEnd;
      algo.ProblemInjector.GetVariable("TestSamplesStart").GetValue<IntData>().Data = problem.TestSamplesStart;
      algo.ProblemInjector.GetVariable("TestSamplesEnd").GetValue<IntData>().Data = problem.TestSamplesEnd;
      ItemList<IntData> allowedFeatures = algo.ProblemInjector.GetVariable("AllowedFeatures").GetValue<ItemList<IntData>>();
      foreach (int allowedFeature in problem.AllowedInputVariables) allowedFeatures.Add(new IntData(allowedFeature));

      if (problem.LearningTask == LearningTask.TimeSeries) {
        algo.ProblemInjector.GetVariable("Autoregressive").GetValue<BoolData>().Data = problem.AutoRegressive;
        algo.ProblemInjector.GetVariable("MinTimeOffset").GetValue<IntData>().Data = problem.MinTimeOffset;
        algo.ProblemInjector.GetVariable("MaxTimeOffset").GetValue<IntData>().Data = problem.MaxTimeOffset;
      } else if (problem.LearningTask == LearningTask.Classification) {
        ItemList<DoubleData> classValues = algo.ProblemInjector.GetVariable("TargetClassValues").GetValue<ItemList<DoubleData>>();
        foreach (double classValue in GetDifferentClassValues(problem.Dataset, targetVariable)) classValues.Add(new DoubleData(classValue));
      }
    }

    private IEnumerable<double> GetDifferentClassValues(HeuristicLab.DataAnalysis.Dataset dataset, int targetVariable) {
      return Enumerable.Range(0, dataset.Rows).Select(x => dataset.GetValue(x, targetVariable)).Distinct();
    }

    private void OnChanged() {
      if (Changed != null) Changed(this, new EventArgs());
    }

    #region IViewable Members

    public IView CreateView() {
      return new DispatcherView(this);
    }

    #endregion
  }
}
