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
using HeuristicLab.Data;
using HeuristicLab.Core;
using HeuristicLab.Modeling;

namespace HeuristicLab.CEDMA.Server {
  public abstract class DispatcherBase : IDispatcher {
    private IStore store;
    private DataSet dataset;
    private List<int> allowedTargetVariables;
    private Dictionary<int, List<int>> activeInputVariables;

    internal event EventHandler Changed;
    private object locker = new object();

    public IEnumerable<string> TargetVariables {
      get {
        if (dataset != null) {
          return dataset.Problem.AllowedTargetVariables.Select(x => dataset.Problem.Dataset.GetVariableName(x));
        } else return new string[0];
      }
    }

    public IEnumerable<string> InputVariables {
      get {
        if (dataset != null) {
          return dataset.Problem.AllowedInputVariables.Select(x => dataset.Problem.Dataset.GetVariableName(x));
        } else return new string[0];
      }
    }

    public DispatcherBase(IStore store) {
      this.store = store;
      allowedTargetVariables = new List<int>();
      activeInputVariables = new Dictionary<int, List<int>>();
    }

    public IAlgorithm GetNextJob() {
      if (dataset == null) {
        var datasetEntities = store.Query("?DataSet <" + Ontology.InstanceOf.Uri + "> <" + Ontology.TypeDataSet.Uri + "> .", 0, 1)
          .Select(x => (Entity)x.Get("DataSet"));
        if (datasetEntities.Count() == 0) return null;
        dataset = new DataSet(store, datasetEntities.ElementAt(0));
        foreach (int targetVar in dataset.Problem.AllowedTargetVariables) {
          activeInputVariables.Add(targetVar, new List<int>());
          activeInputVariables[targetVar].AddRange(dataset.Problem.AllowedInputVariables);
        }
        OnChanged();
      }
      if (allowedTargetVariables.Count > 0) {
        int[] targetVariables, inputVariables;
        lock (locker) {
          targetVariables = allowedTargetVariables.ToArray();
        }

        int targetVariable = SelectTargetVariable(targetVariables);

        lock (locker) {
          inputVariables = activeInputVariables[targetVariable].ToArray();
        }

        IAlgorithm selectedAlgorithm = SelectAndConfigureAlgorithm(targetVariable, inputVariables, dataset.Problem);

        return selectedAlgorithm;
      } else return null;
    }

    public virtual int SelectTargetVariable(int[] targetVariables) {
      Random rand = new Random();
      return targetVariables[rand.Next(targetVariables.Length)];
    }
    public abstract IAlgorithm SelectAndConfigureAlgorithm(int targetVariable, int[] inputVariables, Problem problem);

    #region IViewable Members

    public IView CreateView() {
      return new DispatcherView(this);
    }

    #endregion

    internal void EnableTargetVariable(string name) {
      lock (locker)
        allowedTargetVariables.Add(dataset.Problem.Dataset.GetVariableIndex(name));
    }

    internal void DisableTargetVariable(string name) {
      lock (locker)
        allowedTargetVariables.Remove(dataset.Problem.Dataset.GetVariableIndex(name));
    }

    internal void EnableInputVariable(string target, string name) {
      lock (locker) {
        int targetIndex = dataset.Problem.Dataset.GetVariableIndex(target);
        int inputIndex = dataset.Problem.Dataset.GetVariableIndex(name);
        if (!activeInputVariables[targetIndex].Contains(inputIndex)) {
          activeInputVariables[targetIndex].Add(inputIndex);
        }
      }
    }

    internal void DisableInputVariable(string target, string name) {
      lock (locker) {
        int targetIndex = dataset.Problem.Dataset.GetVariableIndex(target);
        int inputIndex = dataset.Problem.Dataset.GetVariableIndex(name);
        while (activeInputVariables[targetIndex].Remove(inputIndex)) { }
      }
    }

    private void OnChanged() {
      if (Changed != null) Changed(this, new EventArgs());
    }

    internal IEnumerable<string> GetInputVariables(string target) {
      return activeInputVariables[dataset.Problem.Dataset.GetVariableIndex(target)]
        .Select(i => dataset.Problem.Dataset.GetVariableName(i));
    }
  }
}
