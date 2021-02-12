using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public interface IResultFormatter {
    int Priority { get; }

    bool CanFormatType(Type t);

    string Format(object o);
  }
}
