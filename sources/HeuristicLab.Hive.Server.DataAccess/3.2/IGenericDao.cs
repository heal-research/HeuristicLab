using System;
using System.Collections.Generic;
using System.Text;

namespace HeuristicLab.Hive.Server.DataAccess {
  public interface IGenericDao<TBusiness> {
    TBusiness FindById(Guid id);
    IEnumerable<TBusiness> FindAll();
    TBusiness Insert(TBusiness bObj);
    
    void Delete(TBusiness bObj);

    void Update(TBusiness bObj);
  }
}
