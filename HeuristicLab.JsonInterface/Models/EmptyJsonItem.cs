namespace HeuristicLab.JsonInterface {
  public class EmptyJsonItem : JsonItem {
    protected override ValidationResult Validate() => ValidationResult.Successful();
  }
}
