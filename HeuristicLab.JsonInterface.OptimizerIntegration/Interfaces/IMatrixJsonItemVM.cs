using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public interface IMatrixJsonItemVM : IJsonItemVM {
    bool RowsResizable { get; set; }
    bool ColumnsResizable { get; set; }
    IEnumerable<string> RowNames { get; set; }
    IEnumerable<string> ColumnNames { get; set; }
  }
}
