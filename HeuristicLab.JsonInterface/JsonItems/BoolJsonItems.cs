using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  public class BoolJsonItem : ValueJsonItem<bool> {
    protected override ValidationResult Validate() => ValidationResult.Successful();

    public BoolJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }

  public class BoolArrayJsonItem : ArrayJsonItem<bool> {
    protected override ValidationResult Validate() => ValidationResult.Successful();

    public BoolArrayJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }

  public class BoolMatrixJsonItem : MatrixJsonItem<bool> {
    protected override ValidationResult Validate() => ValidationResult.Successful();

    public BoolMatrixJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }

  }
}
