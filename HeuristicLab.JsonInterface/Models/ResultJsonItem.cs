namespace HeuristicLab.JsonInterface {
  public class ResultJsonItem : JsonItem, IResultJsonItem {
    protected override ValidationResult Validate() => ValidationResult.Successful();
  }
}
