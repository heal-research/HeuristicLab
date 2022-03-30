namespace HeuristicLab.JsonInterface {
  public interface IJsonItemValidator {
    /// <summary>
    /// Validate method to validate a JsonItem.
    /// </summary>
    /// <returns>
    /// The result of the validation process, 
    /// contains a sucess flag and a list of errors 
    /// (if validation failed).
    /// </returns>
    ValidationResult Validate();
  }
}
