using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Server.DataAccess;

namespace HeuristicLab.Hive.Server.LINQDataAccess {
  public class PluginInfoDao: BaseDao<HivePluginInfoDto, PluginInfo>, IPluginInfoDao {
    #region IGenericDao<HivePluginInfoDto,PluginInfo> Members

    public HivePluginInfoDto FindById(Guid id) {
      return (from pi in Context.PluginInfos
              where pi.PluginId.Equals(id)
              select EntityToDto(pi, null)
             ).SingleOrDefault();  
    }

    public IEnumerable<HivePluginInfoDto> FindAll() {
      return (from pi in Context.PluginInfos              
              select EntityToDto(pi, null)
       ).ToList();  
    }

    public HivePluginInfoDto Insert(HivePluginInfoDto bObj) {
      PluginInfo pi = DtoToEntity(bObj, null);
      Context.PluginInfos.InsertOnSubmit(pi);
      Context.SubmitChanges();
      bObj.Id = pi.PluginId;
      return bObj;
    }

    public void Delete(HivePluginInfoDto bObj) {
      PluginInfo pi = Context.PluginInfos.SingleOrDefault(p => p.PluginId.Equals(bObj.Id));
      Context.PluginInfos.DeleteOnSubmit(pi);      
    }

    public void Update(HivePluginInfoDto bObj) {
      PluginInfo pi = Context.PluginInfos.SingleOrDefault(p => p.PluginId.Equals(bObj.Id));
      DtoToEntity(bObj, pi);
      Context.SubmitChanges();
    }

    public void InsertPluginDependenciesForJob(JobDto jobDto) {
      foreach (HivePluginInfoDto info in jobDto.PluginsNeeded) {        
        PluginInfo dbpi =
          Context.PluginInfos.Where(pi => pi.Name.Equals(info.Name) && pi.Version == info.Version).SingleOrDefault();
        if (dbpi == null) {
          dbpi = new PluginInfo {
                                  BuildDate = info.BuildDate.ToString(),
                                  Name = info.Name,
                                  Version = info.Version
                                };
          Context.PluginInfos.InsertOnSubmit(dbpi);
          Context.SubmitChanges();
        }

        RequiredPlugin rq = new RequiredPlugin();
        rq.JobId = jobDto.Id;
        rq.PluginInfo = dbpi;
        Context.RequiredPlugins.InsertOnSubmit(rq);
        Context.SubmitChanges();
      }
    }

    #endregion

    // Build Date not mapped - won't matter anyway, it's a obsolete field.
    public override PluginInfo DtoToEntity(HivePluginInfoDto source, PluginInfo target) {
      if (source == null)
        return null;
      if (target == null)
        target = new PluginInfo();

      target.Name = source.Name;
      target.PluginId = source.Id;
      target.Version = source.Version;      

      return target;

    }

    public override HivePluginInfoDto EntityToDto(PluginInfo source, HivePluginInfoDto target) {
      if (source == null)
        return null;
      if (target == null)
        target = new HivePluginInfoDto();

      target.Name = source.Name;
      target.Id = source.PluginId;
      target.Version = source.Version;

      return target;
    }
  }
}
