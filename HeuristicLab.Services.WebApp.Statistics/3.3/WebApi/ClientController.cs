#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using DT = HeuristicLab.Services.WebApp.Statistics.WebApi.DataTransfer;

namespace HeuristicLab.Services.WebApp.Statistics.WebApi {

  [Authorize]
  public class ClientController : ApiController {
    private IPersistenceManager PersistenceManager {
      get { return ServiceLocator.Instance.PersistenceManager; }
    }

    public DT.ClientDetails GetClientDetails(Guid id) {
      var pm = PersistenceManager;
      var dimClientDao = pm.DimClientDao;
      var factClientInfoDao = pm.FactClientInfoDao;
      var factTaskDao = pm.FactTaskDao;
      return pm.UseTransaction(() => {
        var timeData = factClientInfoDao.GetByClientId(id)
          .GroupBy(x => x.ClientId)
          .Select(x => new {
            TotalIdleTime = x.Sum(y => y.IdleTime),
            TotalOfflineTime = x.Sum(y => y.OfflineTime),
            TotalUnavailableTime = x.Sum(y => y.UnavailableTime)
          }).FirstOrDefault();
        var taskTimeData = factTaskDao.GetByClientId(id)
          .GroupBy(x => x.LastClientId)
          .Select(x => new {
            TotalCalculatingTime = x.Sum(y => y.CalculatingTime),
            TotalTransferringTime = x.Sum(y => y.TransferTime)
          }).FirstOrDefault();
        var startDate = factClientInfoDao.GetByClientId(id)
          .Where(x => x.SlaveState == SlaveState.Offline)
          .OrderByDescending(x => x.Time)
          .Select(x => x.Time).FirstOrDefault();
        long upTime = 0;
        if (startDate == default(DateTime)) {
          startDate = factClientInfoDao.GetByClientId(id)
            .OrderByDescending(x => x.Time)
            .Select(x => x.Time)
            .FirstOrDefault();
        }
        if (startDate != default(DateTime)) {
          upTime = (long)(DateTime.Now - startDate).TotalSeconds;
        }
        return (from client in dimClientDao.GetAll().Where(x => x.Id == id)
                join info in factClientInfoDao.GetAll()
                  on client.Id equals info.ClientId into clientInfoJoin
                from clientInfo in clientInfoJoin.OrderByDescending(x => x.Time).Take(1)
                let offline = (client.DateExpired != null || clientInfo.SlaveState == SlaveState.Offline)
                let parent = client.ParentResourceId.HasValue ? dimClientDao.GetById(client.ParentResourceId.Value) : null
                select new DT.ClientDetails {
                  Id = client.Id,
                  Name = client.Name,
                  TotalCores = clientInfo.NumTotalCores,
                  UsedCores = offline ? 0 : clientInfo.NumUsedCores,
                  TotalMemory = clientInfo.TotalMemory,
                  UsedMemory = offline ? 0 : clientInfo.UsedMemory,
                  CpuUtilization = offline ? 0 : clientInfo.CpuUtilization,
                  State = offline ? SlaveState.Offline.ToString() : clientInfo.SlaveState.ToString(),
                  LastUpdate = clientInfo.Time,
                  GroupId = client.ParentResourceId,
                  GroupName = parent != null ? parent.Name : null,
                  UpTime = offline ? 0 : upTime,
                  TotalUnavailableTime = timeData != null ? timeData.TotalUnavailableTime : 0,
                  TotalCalculatingTime = taskTimeData != null ? taskTimeData.TotalCalculatingTime : 0,
                  TotalIdleTime = timeData != null ? timeData.TotalIdleTime : 0,
                  TotalOfflineTime = timeData != null ? timeData.TotalOfflineTime : 0,
                  TotalTransferringTime = taskTimeData != null ? taskTimeData.TotalTransferringTime : 0,
                  TasksStates = factTaskDao.GetByClientId(id)
                                  .GroupBy(x => x.TaskState)
                                  .Select(x => new DT.TaskStateCount {
                                    State = x.Key.ToString(),
                                    Count = x.Count()
                                  }).ToList(),
                  Users = factTaskDao.GetAll()
                            .Where(x => x.LastClientId == id)
                            .Select(x => new DT.User {
                              Id = x.DimJob.UserId,
                              Name = x.DimJob.UserName
                            })
                            .Distinct()
                            .ToList()
                })
                .FirstOrDefault();
      });
    }

    public DT.ClientPage GetClients(int page, int size, bool expired = false) {
      var pm = PersistenceManager;
      var dimClientDao = pm.DimClientDao;
      var factClientInfoDao = pm.FactClientInfoDao;
      return pm.UseTransaction(() => {
        var clients = expired ? dimClientDao.GetAllExpiredSlaves() : dimClientDao.GetAllOnlineSlaves();
        var query = (from client in clients
                     join info in factClientInfoDao.GetAll()
                       on client.Id equals info.ClientId into clientInfoJoin
                     from clientInfo in clientInfoJoin.OrderByDescending(x => x.Time).Take(1)
                     let offline = (expired || clientInfo.SlaveState == SlaveState.Offline)
                     let parent = client.ParentResourceId.HasValue ? dimClientDao.GetById(client.ParentResourceId.Value) : null
                     select new DT.Client {
                       Id = client.Id,
                       Name = client.Name,
                       TotalCores = clientInfo.NumTotalCores,
                       UsedCores = offline ? 0 : clientInfo.NumUsedCores,
                       TotalMemory = clientInfo.TotalMemory,
                       UsedMemory = offline ? 0 : clientInfo.UsedMemory,
                       CpuUtilization = offline ? 0 : clientInfo.CpuUtilization,
                       State = offline ? SlaveState.Offline.ToString() : clientInfo.SlaveState.ToString(),
                       GroupId = client.ParentResourceId,
                       GroupName = parent != null ? parent.Name : null,
                       IsAllowedToCalculate = clientInfo.IsAllowedToCalculate
                     });
        return new DT.ClientPage {
          TotalClients = query.Count(),
          Clients = query.Skip((page - 1) * size)
            .Take(size)
            .ToList()
        };
      });
    }

    public IEnumerable<DT.ClientStatus> GetClientHistory(Guid id, DateTime start, DateTime end) {
      TimeSpan ts = end - start;
      int increment = 1;
      double totalMinutes = ts.TotalMinutes;
      while (totalMinutes > 5761) {
        totalMinutes -= 5761;
        increment += 5;
      }
      var pm = PersistenceManager;
      var factClientInfo = pm.FactClientInfoDao;
      var clientInfos = factClientInfo.GetByClientId(id)
        .Where(x => x.Time >= start && x.Time <= end)
        .OrderBy(x => x.Time)
        .ToList();
      var clientStatus = new DT.ClientStatus {
        CpuUtilization = 0,
        TotalCores = 0,
        UsedCores = 0,
        TotalMemory = 0,
        UsedMemory = 0
      };
      int i = 1;
      foreach (var clientInfo in clientInfos) {
        clientStatus.CpuUtilization += clientInfo.CpuUtilization;
        clientStatus.TotalCores += clientInfo.NumTotalCores;
        clientStatus.UsedCores += clientInfo.NumUsedCores;
        clientStatus.TotalMemory += clientInfo.TotalMemory;
        clientStatus.UsedMemory += clientInfo.UsedMemory;
        if (i >= increment) {
          clientStatus.Timestamp = JavascriptUtils.ToTimestamp(clientInfo.Time);
          clientStatus.CpuUtilization /= i;
          clientStatus.TotalCores /= i;
          clientStatus.UsedCores /= i;
          clientStatus.TotalMemory /= i;
          clientStatus.UsedMemory /= i;
          yield return clientStatus;
          clientStatus = new DT.ClientStatus {
            CpuUtilization = 0,
            TotalCores = 0,
            UsedCores = 0,
            TotalMemory = 0,
            UsedMemory = 0
          };
          i = 1;
        } else {
          ++i;
        }
      }
    }

    public DT.ClientPage GetClientsByGroupId(Guid id, int page, int size, bool expired = false) {
      var pm = PersistenceManager;
      var dimClientDao = pm.DimClientDao;
      var factClientInfoDao = pm.FactClientInfoDao;
      return pm.UseTransaction(() => {
        var clients = expired ? dimClientDao.GetAllExpiredClients() : dimClientDao.GetAllOnlineClients();
        clients = clients.Where(x => x.ParentResourceId == id);
        var query = (from client in clients
                     join info in factClientInfoDao.GetAll()
                       on client.Id equals info.ClientId into clientInfoJoin
                     from clientInfo in clientInfoJoin.OrderByDescending(x => x.Time).Take(1)
                     let offline = (expired || clientInfo.SlaveState == SlaveState.Offline)
                     let parent = client.ParentResourceId.HasValue ? dimClientDao.GetById(client.ParentResourceId.Value) : null
                     select new DT.Client {
                       Id = client.Id,
                       Name = client.Name,
                       TotalCores = clientInfo.NumTotalCores,
                       UsedCores = offline ? 0 : clientInfo.NumUsedCores,
                       TotalMemory = clientInfo.TotalMemory,
                       UsedMemory = offline ? 0 : clientInfo.UsedMemory,
                       CpuUtilization = offline ? 0 : clientInfo.CpuUtilization,
                       State = offline ? SlaveState.Offline.ToString() : clientInfo.SlaveState.ToString(),
                       GroupId = client.ParentResourceId,
                       GroupName = parent != null ? parent.Name : null,
                       IsAllowedToCalculate = clientInfo.IsAllowedToCalculate
                     });
        return new DT.ClientPage {
          TotalClients = query.Count(),
          Clients = query.Skip((page - 1) * size)
            .Take(size)
            .ToList()
        };
      });
    }

    public IEnumerable<DT.ClientStatus> GetClientHistoryByGroupId(Guid id, DateTime start, DateTime end) {
      TimeSpan ts = end - start;
      int increment = 1;
      double totalMinutes = ts.TotalMinutes;
      while (totalMinutes > 5761) {
        totalMinutes -= 5761;
        increment += 5;
      }
      var pm = PersistenceManager;
      var factClientInfo = pm.FactClientInfoDao;
      var dimClientDao = pm.DimClientDao;
      var clientInfos = factClientInfo.GetAll()
        .Where(x => x.Time >= start && x.Time <= end)
        .OrderBy(x => x.Time)
        .Join(dimClientDao.GetAll(),
          fact => fact.ClientId,
          client => client.Id,
          (fact, client) => new {
            client.ParentResourceId,
            fact.Time,
            fact.CpuUtilization,
            fact.NumTotalCores,
            fact.NumUsedCores,
            fact.TotalMemory,
            fact.UsedMemory
          })
        .Where(x => x.ParentResourceId == id)
        .ToList();
      var clientStatus = new DT.ClientStatus {
        CpuUtilization = 0,
        TotalCores = 0,
        UsedCores = 0,
        TotalMemory = 0,
        UsedMemory = 0
      };
      int i = 1;
      foreach (var clientInfo in clientInfos) {
        clientStatus.CpuUtilization += clientInfo.CpuUtilization;
        clientStatus.TotalCores += clientInfo.NumTotalCores;
        clientStatus.UsedCores += clientInfo.NumUsedCores;
        clientStatus.TotalMemory += clientInfo.TotalMemory;
        clientStatus.UsedMemory += clientInfo.UsedMemory;
        if (i >= increment) {
          clientStatus.Timestamp = JavascriptUtils.ToTimestamp(clientInfo.Time);
          clientStatus.CpuUtilization /= i;
          clientStatus.TotalCores /= i;
          clientStatus.UsedCores /= i;
          clientStatus.TotalMemory /= i;
          clientStatus.UsedMemory /= i;
          yield return clientStatus;
          clientStatus = new DT.ClientStatus {
            CpuUtilization = 0,
            TotalCores = 0,
            UsedCores = 0,
            TotalMemory = 0,
            UsedMemory = 0
          };
          i = 1;
        } else {
          ++i;
        }
      }
    }
  }
}