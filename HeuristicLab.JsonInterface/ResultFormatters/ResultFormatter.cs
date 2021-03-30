using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {

  /// <summary>
  /// Base Class for ResultFormatter
  /// </summary>
  public abstract class ResultFormatter : IResultFormatter {
    public abstract int Priority { get; }
    public abstract bool CanFormatType(Type t);
    public abstract string Format(object o);

    /// <summary>
    /// static property to get all existing result formatters
    /// </summary>
    public static IEnumerable<IResultFormatter> All { get; } =
      PluginInfrastructure.ApplicationManager.Manager.GetInstances<IResultFormatter>();

    /// <summary>
    /// static method to get all existing result formatters for a specific type
    /// </summary>
    /// <param name="t">target type</param>
    /// <returns>collection of found result formatters or null if none was found</returns>
    public static IEnumerable<IResultFormatter> ForType(Type t) =>
      All?.Where(x => x.CanFormatType(t)).OrderBy(x => x.Priority);
  }
}
