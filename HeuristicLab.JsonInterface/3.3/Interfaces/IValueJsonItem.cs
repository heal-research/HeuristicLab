namespace HeuristicLab.JsonInterface {
  public interface IValueJsonItem : IJsonItem {
    /// <summary>
    /// Represent the value of an IItem.
    /// </summary>
    object Value { get; set; }
  }

  public interface IValueJsonItem<T> : IValueJsonItem {
    new T Value { get; set; }
  }
}
