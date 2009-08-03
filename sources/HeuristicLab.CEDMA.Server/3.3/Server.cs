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
using HeuristicLab.Grid;
using HeuristicLab.Grid.HiveBridge;
using HeuristicLab.Core;
using HeuristicLab.Modeling.Database;
using HeuristicLab.Modeling.Database.SQLServerCompact;

namespace HeuristicLab.CEDMA.Server {
  public class Server : IViewable {
    private static readonly string sqlServerCompactFile = AppDomain.CurrentDomain.BaseDirectory + "HeuristicLab.Modeling.database.sdf";
    private static readonly string sqlServerCompactConnectionString = @"Data Source=" + sqlServerCompactFile;

    private DatabaseService database;
    private IDispatcher dispatcher;
    public IDispatcher Dispatcher { get { return dispatcher; } }
    private IExecuter executer;
    public IExecuter Executer { get { return executer; } }
    private Problem problem;
    public Problem Problem { get { return problem; } }

    private string gridServiceUrl;
    public string GridServiceUrl {
      get { return gridServiceUrl; }
      set { gridServiceUrl = value; }
    }

    public Server() {
      database = new DatabaseService(sqlServerCompactConnectionString);
      problem = new Problem();
      try {
        problem.Dataset = database.GetDataset();
      }
      catch (InvalidOperationException ex) {
      }
    }

    internal void Connect(string serverUrl) {
      dispatcher = new SimpleDispatcher(database, problem);
      IGridServer gridServer = null;
      if (serverUrl.Contains("ExecutionEngine")) {
        gridServer = new HiveGridServerWrapper(serverUrl);
      } else {
        // default is grid backend
        gridServer = new GridServerProxy(serverUrl);
      }
      executer = new GridExecuter(dispatcher, gridServer, database);
      executer.Start();
    }
    #region IViewable Members

    public IView CreateView() {
      return new ServerView(this);
    }
    #endregion
  }
}
