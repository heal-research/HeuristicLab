using System;
using System.Data.Linq.Mapping;
namespace HeuristicLab.Services.Hive.DataAccess {
  partial class SlaveStatistics {
  }

  partial class HiveDataContext {
    // source: http://stackoverflow.com/questions/648196/random-row-from-linq-to-sql
    [Function(Name = "NEWID", IsComposable = true)]
    public Guid Random() { 
      // to prove not used by our C# code... 
      throw new NotImplementedException();
    }
  }
}
