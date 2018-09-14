#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Web.Http;
using HeuristicLab.Services.Hive;
using HeuristicLab.Services.Hive.DataAccess;
using HeuristicLab.Services.Hive.DataAccess.Interfaces;
using DT = HeuristicLab.Services.WebApp.Maintenance.WebApi.DataTransfer;

namespace HeuristicLab.Services.WebApp.Maintenance.WebApi {
  public class FactClientInfoController : ApiController {
    private IPersistenceManager PersistenceManager {
      get { return ServiceLocator.Instance.PersistenceManager; }
    }

    public DT.FactClientInfo GetFactClientInfo(DateTime start, DateTime end) {
      var pm = PersistenceManager;
      var factClientInfo = pm.FactClientInfoDao;
      var query = factClientInfo.GetAll().Where(x => x.Time >= start && x.Time <= end);
      return new DT.FactClientInfo {
        Rows = query.Count(),
        Clients = query.Select(x => x.ClientId).Distinct().Count()
      };
    }

    public void Aggregate(DateTime start, DateTime end, int entries) {
      var pm = PersistenceManager;
      var factClientInfoDao = pm.FactClientInfoDao;
      var dimTimeDao = pm.DimTimeDao;
      var clientIds = pm.UseTransaction(() => factClientInfoDao.GetAll()
        .Where(x => x.Time >= start && x.Time <= end)
        .Select(x => x.ClientId)
        .Distinct()
      );
      foreach (var id in clientIds) {
        AggregateClient(pm, id, start, end, entries);
      }
      dimTimeDao.DeleteUnusedTimes();
    }

    private void AggregateClient(IPersistenceManager pm, Guid clientId, DateTime start, DateTime end, int entries) {
      var factClientInfoDao = pm.FactClientInfoDao;
      var clientInfos = pm.UseTransaction(() => factClientInfoDao.GetByClientId(clientId)
        .Where(x => x.Time >= start && x.Time <= end)
        .OrderBy(x => x.Time)
        .ToList()
      );
      var e = clientInfos.GetEnumerator();
      if (!e.MoveNext()) return;
      do {
        var infosToAggregate = new List<FactClientInfo> { e.Current };
        var prev = e.Current;
        while (e.MoveNext() && infosToAggregate.Count() != entries) {
          var cur = e.Current;
          if (prev.IsAllowedToCalculate != cur.IsAllowedToCalculate || prev.SlaveState != cur.SlaveState)
            break;
          infosToAggregate.Add(cur);
          prev = cur;
        }
        if (infosToAggregate.Count() >= 2) {
          AggregateClientInfos(pm, infosToAggregate);
        }
      } while (e.Current != null);
    }

    private void AggregateClientInfos(IPersistenceManager pm, List<FactClientInfo> clientInfos) {
      var factClientInfoDao = pm.FactClientInfoDao;
      var last = clientInfos.Last();
      var infos = clientInfos.GroupBy(x => x.ClientId).Select(x => new {
        NumUsedCores = (int)x.Average(y => y.NumUsedCores),
        NumTotalCores = (int)x.Average(y => y.NumTotalCores),
        UsedMemory = (int)x.Average(y => y.UsedMemory),
        TotalMemory = (int)x.Average(y => y.TotalMemory),
        CpuUtilization = Math.Round(x.Average(y => y.CpuUtilization), 2),
        IdleTime = x.Sum(y => y.IdleTime),
        OfflineTime = x.Sum(y => y.OfflineTime),
        UnavailableTime = x.Sum(y => y.UnavailableTime)
      }).SingleOrDefault();
      if (infos != null) {
        pm.UseTransaction(() => {
          last.NumUsedCores = infos.NumUsedCores;
          last.NumTotalCores = infos.NumTotalCores;
          last.UsedMemory = infos.UsedMemory;
          last.TotalMemory = infos.TotalMemory;
          last.CpuUtilization = infos.CpuUtilization;
          last.IdleTime = infos.IdleTime;
          last.OfflineTime = infos.OfflineTime;
          last.UnavailableTime = infos.UnavailableTime;
          clientInfos.Remove(last);
          factClientInfoDao.Delete(clientInfos);
          pm.SubmitChanges();
        });
      }
    }
  }
}
