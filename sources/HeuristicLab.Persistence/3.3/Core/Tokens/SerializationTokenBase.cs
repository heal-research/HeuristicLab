using HeuristicLab.Persistence.Interfaces;
using System.Reflection;
using System.Text;

namespace HeuristicLab.Persistence.Core.Tokens {

  /// <summary>
  /// Common base class for all serialization tokens.
  /// </summary>
  public abstract class SerializationTokenBase : ISerializationToken {

    /// <summary>
    /// The token's name.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializationTokenBase"/> class.
    /// </summary>
    /// <param name="name">The token name.</param>
    public SerializationTokenBase(string name) {
      Name = name;
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append(this.GetType().Name).Append('(');
      foreach (FieldInfo fi in this.GetType().GetFields()) {
        sb.Append(fi.Name).Append('=').Append(fi.GetValue(this)).Append(", ");
      }
      sb.Append(')');
      return sb.ToString();
    }
  }

}