using HeuristicLab.Persistence.Interfaces;
using System.Reflection;
using System.Text;

namespace HeuristicLab.Persistence.Core.Tokens {

  public abstract class SerializationTokenBase : ISerializationToken {
    public readonly string Name;
    public SerializationTokenBase(string name) {
      Name = name;
    }
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