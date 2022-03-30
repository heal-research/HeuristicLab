using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  public class UnsupportedJsonItem : JsonItem {

    protected override ValidationResult Validate() => ValidationResult.Successful();

    public UnsupportedJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }
}
