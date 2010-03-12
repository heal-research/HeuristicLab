using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Server.DataAccess;

namespace HeuristicLab.Hive.Server.LINQDataAccess {
  public class ClientConfigDao: BaseDao<ClientConfigDto, ClientConfig>, IClientConfigDao {


    #region IGenericDao<ClientConfigDto,ClientConfig> Members

    public ClientConfigDto FindById(Guid id) {
      return (from cc in Context.ClientConfigs
              where cc.ClientConfigId.Equals(id)
              select EntityToDto(cc, null)).SingleOrDefault();
    }

    public IEnumerable<ClientConfigDto> FindAll() {
      return (from cc in Context.ClientConfigs              
              select EntityToDto(cc, null)).ToList();
    }

    public ClientConfigDto Insert(ClientConfigDto bObj) {
      ClientConfig c = DtoToEntity(bObj, null);
      Context.ClientConfigs.InsertOnSubmit(c);
      Context.SubmitChanges();
      bObj.Id = c.ClientConfigId;
      return bObj;  
    }

    public void Delete(ClientConfigDto bObj) {
      Context.ClientConfigs.DeleteOnSubmit(Context.ClientConfigs.SingleOrDefault(c => c.ClientConfigId.Equals(bObj.Id)));
      Context.SubmitChanges();
    }

    public void Update(ClientConfigDto bObj) {
      ClientConfig cc = Context.ClientConfigs.SingleOrDefault(c => c.ClientConfigId.Equals(bObj.Id));
      DtoToEntity(bObj, cc);
      Context.SubmitChanges();
    }

    #endregion

    public override ClientConfig DtoToEntity(ClientConfigDto source, ClientConfig target) {
      if (source == null)
        return null;
      if (target == null)
        target = new ClientConfig();

      target.ClientConfigId = source.Id;
      target.HeartBeatIntervall = source.HeartBeatIntervall;
      
      return target;
    }

    public override ClientConfigDto EntityToDto(ClientConfig source, ClientConfigDto target) {
      if (source == null)
        return null;
      if (target == null)
        target = new ClientConfigDto();

      target.Id = source.ClientConfigId;
      target.HeartBeatIntervall = source.HeartBeatIntervall;

      return target;

    }
  }
}
