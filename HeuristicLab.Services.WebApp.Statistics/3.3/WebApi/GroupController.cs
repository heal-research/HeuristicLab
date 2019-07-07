#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Web.Http;
using HeuristicLab.Services.Hive;
using HeuristicLab.Services.Hive.DataAccess.Interfaces;
using DA = HeuristicLab.Services.Hive.DataAccess;
using DT = HeuristicLab.Services.WebApp.Statistics.WebApi.DataTransfer;

namespace HeuristicLab.Services.WebApp.Statistics.WebApi {
  public class GroupController : ApiController {
    private IPersistenceManager PersistenceManager {
      get { return ServiceLocator.Instance.PersistenceManager; }
    }

    public DT.GroupDetails GetGroupDetails(Guid id) {
      var pm = PersistenceManager;
      var dimClientDao = pm.DimClientDao;
      var factClientInfoDao = pm.FactClientInfoDao;
      var factTaskDao = pm.FactTaskDao;
      return pm.UseTransaction(() => {
        var clientTimeData = factClientInfoDao.GetAll()
          .Join(dimClientDao.GetAll(), x => x.ClientId, y => y.Id, (x, y) => new {
            y.ParentResourceId,
            x.IdleTime,
            x.OfflineTime,
            x.UnavailableTime
          })
          .Where(x => x.ParentResourceId == id)
          .GroupBy(x => x.ParentResourceId)
          .Select(x => new {
            TotalIdleTime = x.Sum(y => y.IdleTime),
            TotalOfflineTime = x.Sum(y => y.OfflineTime),
            TotalUnavailableTime = x.Sum(y => y.UnavailableTime)
          })
          .FirstOrDefault();

        var taskTimeData = factTaskDao.GetByGroupId(id)
          .Select(x => new {
            id,
            x.CalculatingTime,
            x.TransferTime
          })
          .GroupBy(x => x.id)
          .Select(x => new {
            CalculatingTime = x.Sum(y => y.CalculatingTime),
            TransferTime = x.Sum(y => y.TransferTime)
          })
          .FirstOrDefault();
        return (from client in dimClientDao.GetAllOnlineSlaves().Where(x => x.ParentResourceId == id)
                join info in factClientInfoDao.GetAll()
                  on client.Id equals info.ClientId into clientInfoJoin
                from clientInfo in clientInfoJoin.OrderByDescending(x => x.Time).Take(1)
                let offline = (clientInfo.SlaveState == DA.SlaveState.Offline)
                let parent = client.ParentResourceId.HasValue ? dimClientDao.GetById(client.ParentResourceId.Value) : null
                select new {
                  ResourceGroupId = client.ParentResourceId,
                  GroupName = parent != null ? parent.Name : null,
                  TotalCores = clientInfo.NumTotalCores,
                  UsedCores = offline ? 0 : clientInfo.NumUsedCores,
                  TotalMemory = clientInfo.TotalMemory,
                  UsedMemory = offline ? 0 : clientInfo.UsedMemory,
                  CpuUtilization = offline ? 0 : clientInfo.CpuUtilization,
                  SlaveState = clientInfo.SlaveState
                })
          .GroupBy(x => new { x.ResourceGroupId, x.GroupName })
          .Select(x => new DT.GroupDetails {
            Id = id,
            Name = x.Key.GroupName,
            TotalClients = x.Count(),
            OnlineClients = x.Count(y => y.SlaveState != DA.SlaveState.Offline),
            TotalCores = x.Sum(y => y.TotalCores),
            UsedCores = x.Sum(y => y.UsedCores),
            TotalMemory = x.Sum(y => y.TotalMemory),
            UsedMemory = x.Sum(y => y.UsedMemory),
            TotalCpuUtilization = x.Average(y => (double?)y.CpuUtilization) ?? 0.0,
            ActiveCpuUtilization = x.Where(y => y.SlaveState != DA.SlaveState.Offline).Average(y => (double?)y.CpuUtilization) ?? 0.0,
            TotalUnavailableTime = clientTimeData != null ? clientTimeData.TotalUnavailableTime : 0,
            TotalCalculatingTime = taskTimeData != null ? taskTimeData.CalculatingTime : 0,
            TotalIdleTime = clientTimeData != null ? clientTimeData.TotalIdleTime : 0,
            TotalOfflineTime = clientTimeData != null ? clientTimeData.TotalOfflineTime : 0,
            TotalTransferringTime = taskTimeData != null ? taskTimeData.TransferTime : 0,
            TasksStates = factTaskDao.GetByGroupId(id)
                                  .GroupBy(y => y.TaskState)
                                  .Select(y => new DT.TaskStateCount {
                                    State = y.Key.ToString(),
                                    Count = y.Count()
                                  }).ToList()
          }).FirstOrDefault();
      });
    }

    public DT.GroupPage GetGroups(int page, int size) {
      var pm = PersistenceManager;
      var dimClientDao = pm.DimClientDao;
      var factClientInfoDao = pm.FactClientInfoDao;
      var data = (from client in dimClientDao.GetAllOnlineSlaves()
                  join info in factClientInfoDao.GetAll()
                    on client.Id equals info.ClientId into clientInfoJoin
                  from clientInfo in clientInfoJoin.OrderByDescending(x => x.Time).Take(1)
                  let offline = (clientInfo.SlaveState == DA.SlaveState.Offline)
                  select new {
                    ResourceGroupId = client.ParentResourceId,
                    TotalCores = clientInfo.NumTotalCores,
                    UsedCores = offline ? 0 : clientInfo.NumUsedCores,
                    TotalMemory = clientInfo.TotalMemory,
                    UsedMemory = offline ? 0 : clientInfo.UsedMemory,
                    CpuUtilization = offline ? 0 : clientInfo.CpuUtilization,
                    SlaveState = clientInfo.SlaveState
                  }).GroupBy(x => x.ResourceGroupId).Select(x => new {
                    GroupId = x.Key,
                    TotalClients = x.Count(),
                    OnlineClients = x.Count(y => y.SlaveState != DA.SlaveState.Offline),
                    TotalCores = x.Sum(y => y.TotalCores),
                    UsedCores = x.Sum(y => y.UsedCores),
                    TotalMemory = x.Sum(y => y.TotalMemory),
                    UsedMemory = x.Sum(y => y.UsedMemory),
                    CpuUtilization = x.Where(y => y.SlaveState != DA.SlaveState.Offline).Average(y => (double?)y.CpuUtilization) ?? 0.0
                  });

      var query = dimClientDao.GetAllOnlineSlaveGroups().Select(x => new {
        Id = x.ResourceId,
        Name = x.Name
      });
      return pm.UseTransaction(() => new DT.GroupPage {
        TotalGroups = query.Count(),
        Groups = query
        .Skip((page - 1) * size)
        .Take(size)
        .Join(data, x => x.Id, y => y.GroupId,
          (group, info) => new DT.Group {
            Id = group.Id,
            Name = group.Name,
            TotalClients = info.TotalClients,
            OnlineClients = info.OnlineClients,
            TotalCores = info.TotalCores,
            UsedCores = info.UsedCores,
            TotalMemory = info.TotalMemory,
            UsedMemory = info.UsedMemory,
            CpuUtilization = info.CpuUtilization
          }).ToList()
      });
    }
  }
}