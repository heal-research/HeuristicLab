using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Server.LINQDataAccess {
  public interface IGenericDao<TBusiness, TDatabase> {
    TBusiness FindById(Guid id);
    IEnumerable<TBusiness> FindAll();
    TBusiness Insert(TBusiness bObj);
    void Delete(TBusiness bObj);

    void Update(TBusiness bObj);
  }
}
