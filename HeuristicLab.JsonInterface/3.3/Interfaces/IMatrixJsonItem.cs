using System.Collections.Generic;

namespace HeuristicLab.JsonInterface {
  public interface IMatrixJsonItem : IValueJsonItem {
    /// <summary>
    /// Flag to define resizable rows. 
    /// </summary>
    bool RowsResizable { get; set; }
    /// <summary>
    /// Flag to define resizable columns.
    /// </summary>
    bool ColumnsResizable { get; set; }
    /// <summary>
    /// IEnumerable of row names.
    /// </summary>
    IEnumerable<string> RowNames { get; set; }
    /// <summary>
    /// IEnumerable of column names.
    /// </summary>
    IEnumerable<string> ColumnNames { get; set; }
  }
}
