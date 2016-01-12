using System.Runtime.Serialization;

namespace HeuristicLab.Services.OKB.RunCreation {
  [DataContract]
  public class UnknownCharacteristicType {
    [DataMember]
    public string Message { get; set; }

    public UnknownCharacteristicType(string message, params object[] c) {
      Message = string.Format(message, c);
    }
  }
}
