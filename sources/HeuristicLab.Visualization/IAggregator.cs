using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Visualization {
  public interface IAggregator : IDataRow {
    void AddWatch(IDataRow dataRow);
    void RemoveWatch(IDataRow dataRow);
  }
}
