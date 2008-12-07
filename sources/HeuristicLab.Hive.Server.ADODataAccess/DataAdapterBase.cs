using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  abstract class DataAdapterBase {
    protected abstract void Update();

    protected DataAdapterBase() {
      ServiceLocator.GetTransactionManager().OnUpdate += 
        new EventHandler(DataAdapterBase_OnUpdate);
    }

    void DataAdapterBase_OnUpdate(object sender, EventArgs e) {
      this.Update();
    }
  }
}
