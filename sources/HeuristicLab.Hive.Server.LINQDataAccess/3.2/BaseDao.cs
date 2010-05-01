using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using HeuristicLab.Tracing;

namespace HeuristicLab.Hive.Server.LINQDataAccess {
  public abstract class BaseDao<TBusiness, TDatabaseEntity> {
    public static HiveDataContext Context {
      get {
        return ContextFactory.Context;
      }
    }

    protected void CommitChanges() {
      try {
        Context.SubmitChanges(ConflictMode.ContinueOnConflict);
      } catch (ChangeConflictException e) {
        Logger.Warn("Concurrency Exception! " + e.Message);
        foreach (ObjectChangeConflict conflict in Context.ChangeConflicts) {          
          conflict.Resolve(RefreshMode.KeepChanges);
        }
      }
    }

    public abstract TDatabaseEntity DtoToEntity(TBusiness source, TDatabaseEntity target);
    public abstract TBusiness EntityToDto(TDatabaseEntity source, TBusiness target);

  }
}
