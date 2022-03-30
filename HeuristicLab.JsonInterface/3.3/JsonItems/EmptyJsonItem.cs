using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  public class EmptyJsonItem : JsonItem {
    protected override ValidationResult Validate() => ValidationResult.Successful();

    public EmptyJsonItem(IJsonConvertable convertable, JsonItemConverter converter) :
      base(convertable.ToString(), convertable, converter) { }

    public EmptyJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }

  }
}
