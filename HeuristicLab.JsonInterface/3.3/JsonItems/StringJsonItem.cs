using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  public class StringJsonItem : ConcreteRestrictedValueJsonItem<string> {
    public StringJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }

  public class StringArrayJsonItem : ConcreteRestrictedArrayJsonItem<string> {
    public StringArrayJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }
}
