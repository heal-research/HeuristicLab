#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Clients.Common;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [Item("RunCreationClient", "OKB run creation client.")]
  public sealed class RunCreationClient : IContent {
    private static RunCreationClient instance;
    public static RunCreationClient Instance {
      get {
        if (instance == null) instance = new RunCreationClient();
        return instance;
      }
    }

    #region Properties
    private List<Algorithm> algorithms;
    public IEnumerable<Algorithm> Algorithms {
      get { return algorithms; }
    }
    private List<Problem> problems;
    public IEnumerable<Problem> Problems {
      get { return problems; }
    }
    #endregion

    private RunCreationClient() {
      algorithms = new List<Algorithm>();
      problems = new List<Problem>();
    }

    #region Refresh
    public void Refresh() {
      OnRefreshing();
      algorithms = new List<Algorithm>();
      problems = new List<Problem>();
      try {
        algorithms.AddRange(CallRunCreationService<List<Algorithm>>(s => s.GetAlgorithms("HeuristicLab 3.3")));
        problems.AddRange(CallRunCreationService<List<Problem>>(s => s.GetProblems("HeuristicLab 3.3")));
      }
      finally {
        OnRefreshed();
      }
    }
    public void RefreshAsync(Action<Exception> exceptionCallback) {
      var call = new Func<Exception>(delegate() {
        try {
          Refresh();
        }
        catch (Exception ex) {
          return ex;
        }
        return null;
      });
      call.BeginInvoke(delegate(IAsyncResult result) {
        Exception ex = call.EndInvoke(result);
        if (ex != null) exceptionCallback(ex);
      }, null);
    }
    #endregion

    #region Algorithm Methods
    public static byte[] GetAlgorithmData(long algorithmId) {
      return CallRunCreationService<byte[]>(s => s.GetAlgorithmData(algorithmId));
    }
    #endregion

    #region Problem Methods
    public static byte[] GetProblemData(long problemId) {
      return CallRunCreationService<byte[]>(s => s.GetProblemData(problemId));
    }
    #endregion

    #region Run Methods
    public void AddRun(Run run) {
      CallRunCreationService(s => s.AddRun(run));
    }
    #endregion

    #region Events
    public event EventHandler Refreshing;
    private void OnRefreshing() {
      EventHandler handler = Refreshing;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Refreshed;
    private void OnRefreshed() {
      EventHandler handler = Refreshed;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion

    #region Helpers
    private static void CallRunCreationService(Action<IRunCreationService> call) {
      RunCreationServiceClient client = ClientFactory.CreateClient<RunCreationServiceClient, IRunCreationService>();
      try {
        call(client);
      }
      finally {
        try {
          client.Close();
        }
        catch (Exception) {
          client.Abort();
        }
      }
    }
    private static T CallRunCreationService<T>(Func<IRunCreationService, T> call) {
      RunCreationServiceClient client = ClientFactory.CreateClient<RunCreationServiceClient, IRunCreationService>();
      try {
        return call(client);
      }
      finally {
        try {
          client.Close();
        }
        catch (Exception) {
          client.Abort();
        }
      }
    }
    #endregion
  }
}
