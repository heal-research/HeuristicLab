namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public interface IValueLookupJsonItemVM : ILookupJsonItemVM {
    IJsonItem JsonItemReference { get; }
  }
}
