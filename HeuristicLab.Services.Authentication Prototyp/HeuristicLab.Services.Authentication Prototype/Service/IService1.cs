using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Service {
  // HINWEIS: Wenn Sie hier den Schnittstellennamen "IService1" ändern, müssen Sie ebenfalls den Verweis auf "IService1" in "App.config" aktualisieren.
  [ServiceContract]
  public interface IService1 {
    [OperationContract]
    string GetData(int value);

    [OperationContract]
    CompositeType GetDataUsingDataContract(CompositeType composite);

    // AUFGABE: Hier Dienstvorgänge hinzufügen
  }

  // Verwenden Sie einen Datenvertrag, wie im folgenden Beispiel dargestellt, um Dienstvorgängen zusammengesetzte Typen hinzuzufügen.
  [DataContract]
  public class CompositeType {
    bool boolValue = true;
    string stringValue = "Hello ";

    [DataMember]
    public bool BoolValue {
      get { return boolValue; }
      set { boolValue = value; }
    }

    [DataMember]
    public string StringValue {
      get { return stringValue; }
      set { stringValue = value; }
    }
  }
}
