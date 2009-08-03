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
using System.ServiceModel.Description;
using System.Linq;
using HeuristicLab.CEDMA.Core;
using HeuristicLab.Data;
using HeuristicLab.Core;
using HeuristicLab.Modeling;
using HeuristicLab.Modeling.Database;

namespace HeuristicLab.CEDMA.Server {
  public abstract class DispatcherBase : IDispatcher {
    private IModelingDatabase database;
    private List<int> allowedTargetVariables;
    private Dictionary<int, List<int>> activeInputVariables;
    private Problem problem;
    internal event EventHandler Changed;
    private object locker = new object();

    public IEnumerable<string> TargetVariables {
      get {
        if (problem != null) {
          return Enumerable.Range(0, problem.Dataset.Columns).Select(x => problem.Dataset.GetVariableName(x));
        } else return new string[0];
      }
    }

    public IEnumerable<string> InputVariables {
      get {
        if (problem != null) {
          return TargetVariables;
        } else return new string[0];
      }
    }

    public DispatcherBase(IModelingDatabase database, Problem problem) {
      this.problem = problem;
      allowedTargetVariables = new List<int>();
      activeInputVariables = new Dictionary<int, List<int>>();
      problem.Changed += (sender, args) => {
        lock (locker) {
          allowedTargetVariables.Clear();
          activeInputVariables.Clear();
        }
        OnChanged();
      };
      this.database = database;
    }

    public HeuristicLab.Modeling.IAlgorithm GetNextJob() {
      if (allowedTargetVariables.Count > 0) {
        int[] targetVariables, inputVariables;
        lock (locker) {
          targetVariables = allowedTargetVariables.ToArray();
        }

        int targetVariable = SelectTargetVariable(targetVariables);

        lock (locker) {
          inputVariables = activeInputVariables[targetVariable].ToArray();
        }

        HeuristicLab.Modeling.IAlgorithm selectedAlgorithm = SelectAndConfigureAlgorithm(targetVariable, inputVariables, problem);

        return selectedAlgorithm;
      } else return null;
    }

    public virtual int SelectTargetVariable(int[] targetVariables) {
      Random rand = new Random();
      return targetVariables[rand.Next(targetVariables.Length)];
    }
    public abstract HeuristicLab.Modeling.IAlgorithm SelectAndConfigureAlgorithm(int targetVariable, int[] inputVariables, Problem problem);

    #region IViewable Members

    public IView CreateView() {
      return new DispatcherView(this);
    }

    #endregion

    internal void EnableTargetVariable(string name) {
      lock (locker)
        allowedTargetVariables.Add(problem.Dataset.GetVariableIndex(name));
    }

    internal void DisableTargetVariable(string name) {
      lock (locker)
        allowedTargetVariables.Remove(problem.Dataset.GetVariableIndex(name));
    }

    internal void EnableInputVariable(string target, string name) {
      lock (locker) {
        int targetIndex = problem.Dataset.GetVariableIndex(target);
        int inputIndex = problem.Dataset.GetVariableIndex(name);
        if (!activeInputVariables.ContainsKey(targetIndex)) activeInputVariables[targetIndex] = new List<int>();
        if (!activeInputVariables[targetIndex].Contains(inputIndex)) {
          activeInputVariables[targetIndex].Add(inputIndex);
        }
      }
    }

    internal void DisableInputVariable(string target, string name) {
      lock (locker) {
        int targetIndex = problem.Dataset.GetVariableIndex(target);
        int inputIndex = problem.Dataset.GetVariableIndex(name);
        if (!activeInputVariables.ContainsKey(targetIndex)) activeInputVariables[targetIndex] = new List<int>();
        while (activeInputVariables[targetIndex].Remove(inputIndex)) { }
      }
    }

    private void OnChanged() {
      if (Changed != null) Changed(this, new EventArgs());
    }

    internal IEnumerable<string> GetInputVariables(string target) {
      int targetIndex = problem.Dataset.GetVariableIndex(target);
      if (!activeInputVariables.ContainsKey(targetIndex)) activeInputVariables[targetIndex] = new List<int>();
      return activeInputVariables[targetIndex]
        .Select(i => problem.Dataset.GetVariableName(i));
    }
  }
}
