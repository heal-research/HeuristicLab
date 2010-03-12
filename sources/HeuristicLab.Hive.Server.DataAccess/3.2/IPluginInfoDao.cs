using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Server.DataAccess {
  public interface IPluginInfoDao: IGenericDao<HivePluginInfoDto> {    
    void InsertPluginDependenciesForJob(JobDto jobDto);
  }
}
