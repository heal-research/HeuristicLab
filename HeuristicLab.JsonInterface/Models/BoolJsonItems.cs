namespace HeuristicLab.JsonInterface {
  public class BoolJsonItem : ValueJsonItem<bool> {
    protected override ValidationResult Validate() => ValidationResult.Successful();
  }

  public class BoolArrayJsonItem : ArrayJsonItem<bool> {
    protected override ValidationResult Validate() => ValidationResult.Successful();
  }

  public class BoolMatrixJsonItem : MatrixJsonItem<bool> {
    protected override ValidationResult Validate() => ValidationResult.Successful();
  }
}
