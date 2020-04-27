namespace HeuristicLab.JsonInterface {

  public interface IArrayJsonItem : IValueJsonItem {
    /// <summary>
    /// Property to define an array item to be resizable.
    /// </summary>
    bool Resizable { get; set; }
  }
}
