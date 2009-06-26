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

    private List<int> allowedInputVariables;
    private List<int> allowedTargetVariables;

    public DispatcherBase(IStore store) {
      this.store = store;
      allowedInputVariables = new List<int>();
      allowedTargetVariables = new List<int>();
    }

    public IAlgorithm GetNextJob() {
      if (dataset == null) {
        var datasetEntities = store.Query("?DataSet <" + Ontology.InstanceOf.Uri + "> <" + Ontology.TypeDataSet.Uri + "> .", 0, 1)
          .Select(x => (Entity)x.Get("DataSet"));
        if (datasetEntities.Count() == 0) return null;
        dataset = new DataSet(store, datasetEntities.ElementAt(0));
        OnChanged();
      }
      if (allowedTargetVariables.Count > 0 && allowedInputVariables.Count > 0) {
        int[] targetVariables, inputVariables;
        lock (locker) {
          targetVariables = allowedTargetVariables.ToArray();
          inputVariables = allowedInputVariables.ToArray();
        }

        IAlgorithm selectedAlgorithm = SelectAndConfigureAlgorithm(targetVariables, inputVariables, dataset.Problem);
        
        return selectedAlgorithm;
      } else return null;
    }

    public abstract IAlgorithm SelectAndConfigureAlgorithm(int[] targetVariables, int[] inputVariables, Problem problem);

    #region IViewable Members

    public IView CreateView() {
      return new DispatcherView(this);
    }

    #endregion

    internal void EnableInputVariable(string name) {
      lock (locker)
        allowedInputVariables.Add(dataset.Problem.Dataset.GetVariableIndex(name));
    }

    internal void EnableTargetVariable(string name) {
      lock (locker)
        allowedTargetVariables.Add(dataset.Problem.Dataset.GetVariableIndex(name));
    }

    internal void DisableTargetVariable(string name) {
      lock (locker)
        allowedTargetVariables.Remove(dataset.Problem.Dataset.GetVariableIndex(name));
    }

    internal void DisableInputVariable(string name) {
      lock (locker)
        allowedInputVariables.Remove(dataset.Problem.Dataset.GetVariableIndex(name));
    }

    private void OnChanged() {
      if (Changed != null) Changed(this, new EventArgs());
    }
  }
}
