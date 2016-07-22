#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Clients.Common;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Clients.OKB.Administration {
  [Item("AdministrationClient", "OKB administration client.")]
  public sealed class AdministrationClient : IContent {
    private static AdministrationClient instance;
    public static AdministrationClient Instance {
      get {
        if (instance == null) instance = new AdministrationClient();
        return instance;
      }
    }

    #region Properties
    private ItemCollection<Platform> platforms;
    public ItemCollection<Platform> Platforms {
      get { return platforms; }
    }
    private ItemCollection<AlgorithmClass> algorithmClasses;
    public ItemCollection<AlgorithmClass> AlgorithmClasses {
      get { return algorithmClasses; }
    }
    private ItemCollection<Algorithm> algorithms;
    public ItemCollection<Algorithm> Algorithms {
      get { return algorithms; }
    }
    private ItemCollection<ProblemClass> problemClasses;
    public ItemCollection<ProblemClass> ProblemClasses {
      get { return problemClasses; }
    }
    private ItemCollection<Problem> problems;
    public ItemCollection<Problem> Problems {
      get { return problems; }
    }
    #endregion

    public AdministrationClient() { }

    #region Refresh
    public void Refresh() {
      OnRefreshing();

      platforms = new OKBItemCollection<Platform>();
      algorithmClasses = new OKBItemCollection<AlgorithmClass>();
      algorithms = new OKBItemCollection<Algorithm>();
      problemClasses = new OKBItemCollection<ProblemClass>();
      problems = new OKBItemCollection<Problem>();

      try {
        platforms.AddRange(CallAdministrationService<List<Platform>>(s => s.GetPlatforms()).OrderBy(x => x.Name));
        algorithmClasses.AddRange(CallAdministrationService<List<AlgorithmClass>>(s => s.GetAlgorithmClasses()).OrderBy(x => x.Name));
        algorithms.AddRange(CallAdministrationService<List<Algorithm>>(s => s.GetAlgorithms()).OrderBy(x => x.Name));
        problemClasses.AddRange(CallAdministrationService<List<ProblemClass>>(s => s.GetProblemClasses()).OrderBy(x => x.Name));
        problems.AddRange(CallAdministrationService<List<Problem>>(s => s.GetProblems()).OrderBy(x => x.Name));
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

    #region Store
    public static void Store(IOKBItem item) {
      if (item.Id == 0) {
        if (item is Platform)
          item.Id = CallAdministrationService<long>(s => s.AddPlatform((Platform)item));
        else if (item is AlgorithmClass)
          item.Id = CallAdministrationService<long>(s => s.AddAlgorithmClass((AlgorithmClass)item));
        else if (item is Algorithm)
          item.Id = CallAdministrationService<long>(s => s.AddAlgorithm((Algorithm)item));
        else if (item is ProblemClass)
          item.Id = CallAdministrationService<long>(s => s.AddProblemClass((ProblemClass)item));
        else if (item is Problem)
          item.Id = CallAdministrationService<long>(s => s.AddProblem((Problem)item));
      } else {
        if (item is Platform)
          CallAdministrationService(s => s.UpdatePlatform((Platform)item));
        else if (item is AlgorithmClass)
          CallAdministrationService(s => s.UpdateAlgorithmClass((AlgorithmClass)item));
        else if (item is Algorithm)
          CallAdministrationService(s => s.UpdateAlgorithm((Algorithm)item));
        else if (item is ProblemClass)
          CallAdministrationService(s => s.UpdateProblemClass((ProblemClass)item));
        else if (item is Problem)
          CallAdministrationService(s => s.UpdateProblem((Problem)item));
      }
    }
    #endregion

    #region Delete
    public static void Delete(IOKBItem item) {
      if (item is Platform)
        CallAdministrationService(s => s.DeletePlatform(item.Id));
      else if (item is AlgorithmClass)
        CallAdministrationService(s => s.DeleteAlgorithmClass(item.Id));
      else if (item is Algorithm)
        CallAdministrationService(s => s.DeleteAlgorithm(item.Id));
      else if (item is ProblemClass)
        CallAdministrationService(s => s.DeleteProblemClass(item.Id));
      else if (item is Problem)
        CallAdministrationService(s => s.DeleteProblem(item.Id));
      item.Id = 0;
    }
    #endregion

    #region Algorithm Methods
    public static List<Guid> GetAlgorithmUsers(long algorithmId) {
      return CallAdministrationService<List<Guid>>(s => s.GetAlgorithmUsers(algorithmId));
    }
    public static void UpdateAlgorithmUsers(long algorithmId, List<Guid> users) {
      CallAdministrationService(s => s.UpdateAlgorithmUsers(algorithmId, users));
    }
    public static byte[] GetAlgorithmData(long algorithmId) {
      return CallAdministrationService<byte[]>(s => s.GetAlgorithmData(algorithmId));
    }
    public static void UpdateAlgorithmData(long algorithmId, byte[] algorithmData) {
      CallAdministrationService(s => s.UpdateAlgorithmData(algorithmId, algorithmData));
    }
    #endregion

    #region Problem Methods
    public static List<Guid> GetProblemUsers(long problemId) {
      return CallAdministrationService<List<Guid>>(s => s.GetProblemUsers(problemId));
    }
    public static void UpdateProblemUsers(long problemId, List<Guid> users) {
      CallAdministrationService(s => s.UpdateProblemUsers(problemId, users));
    }
    public static byte[] GetProblemData(long problemId) {
      return CallAdministrationService<byte[]>(s => s.GetProblemData(problemId));
    }
    public static void UpdateProblemData(long problemId, byte[] problemData) {
      CallAdministrationService(s => s.UpdateProblemData(problemId, problemData));
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
      var handler = Refreshed;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion

    #region Helpers
    private static void CallAdministrationService(Action<IAdministrationService> call) {
      AdministrationServiceClient client = ClientFactory.CreateClient<AdministrationServiceClient, IAdministrationService>();
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
    private static T CallAdministrationService<T>(Func<IAdministrationService, T> call) {
      AdministrationServiceClient client = ClientFactory.CreateClient<AdministrationServiceClient, IAdministrationService>();
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
