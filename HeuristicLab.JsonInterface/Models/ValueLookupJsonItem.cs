using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HeuristicLab.JsonInterface {
  public class ValueLookupJsonItem : LookupJsonItem, IValueLookupJsonItem {
    [JsonIgnore]
    public IJsonItem JsonItemReference { get; set; }
  }
}
