using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Server.DataAccess;
using HeuristicLab.Hive.Server.ADODataAccess.TableAdapterWrapper;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class ClientConfigAdapter :
    DataAdapterBase<
      dsHiveServerTableAdapters.ClientConfigTableAdapter,
      ClientConfigDto,
      dsHiveServer.ClientConfigRow>,
    IClientConfigAdapter {
    public ClientConfigAdapter(): 
      base(new ClientConfigAdapterWrapper()) {
    }

    protected override dsHiveServer.ClientConfigRow 
      ConvertObj(ClientConfigDto config, dsHiveServer.ClientConfigRow row) {
      if (row != null && config != null) {
        row.ClientConfigId = config.Id;
        //Todo: commOut
        //row.HeartBeatIntervall = config.HeartBeatIntervall;
        //row.UpDownTimeCalendar = config.UpDownTimeCalendar;

        return row;
      } else
        return null;
    }

    protected override ClientConfigDto ConvertRow(dsHiveServer.ClientConfigRow row, ClientConfigDto config) {
      if (config != null && row != null) {
        config.Id = row.ClientConfigId;

        if (!row.IsHeartBeatIntervallNull())
          config.HeartBeatIntervall = row.HeartBeatIntervall;
        else
          config.HeartBeatIntervall = 0;

        if (!row.IsUpDownTimeCalendarNull())
          config.UpDownTimeCalendar = row.UpDownTimeCalendar;
        else
          config.UpDownTimeCalendar = null;

        return config;
      } else
        return null;
    }
  }
}
