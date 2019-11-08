using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
  public class ConvertableAttribute : Attribute {
    public static bool IsConvertable(object obj) => obj.GetType().GetCustomAttribute<ConvertableAttribute>(true) != null;
  }
}
