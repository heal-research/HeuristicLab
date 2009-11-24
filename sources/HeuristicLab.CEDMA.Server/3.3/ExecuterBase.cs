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
using HeuristicLab.Data;
using HeuristicLab.Grid;
using System.Diagnostics;
using HeuristicLab.Core;
using System.Threading;
using HeuristicLab.Modeling;
using HeuristicLab.Modeling.Database;

namespace HeuristicLab.CEDMA.Server {
  public abstract class ExecuterBase : IExecuter {
    internal event EventHandler Changed;

    private IDispatcher dispatcher;
    protected IDispatcher Dispatcher {
      get { return dispatcher; }
    }
    private IModelingDatabase databaseService;

    private int maxActiveJobs;
    public int MaxActiveJobs {
      get { return maxActiveJobs; }
      set {
        if (value < 0) throw new ArgumentException("Only positive values are allowed for MaxActiveJobs");
        maxActiveJobs = value;
        OnChanged();
      }
    }

    public int CalculatedJobs {
      get;
      set;
    }
    public int StoredJobs {
      get;
      set;
    }

    public ExecuterBase(IDispatcher dispatcher, IModelingDatabase databaseService) {
      CalculatedJobs = 0;
      maxActiveJobs = 10;
      this.dispatcher = dispatcher;
      this.databaseService = databaseService;
      this.databaseService.Connect();
      StoredJobs = databaseService.GetAllModels().Count();
      this.databaseService.Disconnect();
    }

    public void Start() {
      new Thread(StartJobs).Start();
    }

    protected abstract void StartJobs();

    protected void SetResults(IScope src, IScope target) {
      foreach (HeuristicLab.Core.IVariable v in src.Variables) {
        target.AddVariable(v);
      }
      foreach (IScope subScope in src.SubScopes) {
        target.AddSubScope(subScope);
      }
      foreach (KeyValuePair<string, string> alias in src.Aliases) {
        target.AddAlias(alias.Key, alias.Value);
      }
    }

    protected void StoreResults(HeuristicLab.Modeling.IAlgorithm finishedAlgorithm) {
      CalculatedJobs++;
      databaseService.Connect();
      if (databaseService.GetDataset() == null) {
        databaseService.PersistProblem(finishedAlgorithm.Dataset);
        databaseService.Commit();
        databaseService.Disconnect();
        databaseService.Connect();
      }
      databaseService.Persist(finishedAlgorithm.Model,finishedAlgorithm.Name, finishedAlgorithm.Description);
      databaseService.Commit();
      databaseService.Disconnect();
      StoredJobs++;
      OnChanged();
    }

    public abstract string[] GetJobs();

    protected internal void OnChanged() {
      if (Changed != null) Changed(this, new EventArgs());
    }

    #region IViewable Members

    public IView CreateView() {
      return new ExecuterView(this);
    }

    #endregion
  }
}
