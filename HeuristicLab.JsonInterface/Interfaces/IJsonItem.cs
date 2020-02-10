using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HeuristicLab.JsonInterface {
  public interface IJsonItem {
    string Name { get; set; }

    string Description { get; set; }

    string Path {
      get;
    }

    [JsonIgnore]
    IList<IJsonItem> Children { get; } //TODO: IEnumerable

    [JsonIgnore]
    IJsonItem Parent { get; set; }

    object Value { get; set; }

    IEnumerable<object> Range { get; set; }

    string ActualName { get; set; }

    IJsonItemValidator GetValidator();

    void AddChildren(params IJsonItem[] childs);

    void AddChildren(IEnumerable<IJsonItem> childs);
  }
}
