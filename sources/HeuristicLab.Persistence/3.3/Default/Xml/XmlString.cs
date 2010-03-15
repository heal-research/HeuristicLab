using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Default.Xml {

  /// <summary>
  /// XML friendly encapsulation of string data.
  /// </summary>
  [StorableClass]  
  public class XmlString : ISerialData {

    /// <summary>
    /// Gets the XML string data. Essentially marks the string as
    /// XML compatible string.
    /// </summary>
    /// <value>The XML string data.</value>
    [Storable]
    public string Data { get; private set; }

    private XmlString() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlString"/> class.
    /// </summary>
    /// <param name="data">The xml data.</param>
    public XmlString(string data) {
      Data = data;
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("XmlString(").Append(Data).Append(')');
      return sb.ToString();
    }
  }
}