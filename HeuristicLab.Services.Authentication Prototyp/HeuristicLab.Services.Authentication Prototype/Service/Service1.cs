using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Service {
  // HINWEIS: Wenn Sie hier den Klassennamen "Service1" ändern, müssen Sie ebenfalls den Verweis auf "Service1" in "App.config" aktualisieren.
  public class Service1 : IService1 {
    public string GetData(int value) {
      return string.Format("You entered: {0}", value);
    }

    public CompositeType GetDataUsingDataContract(CompositeType composite) {
      if (composite.BoolValue) {
        composite.StringValue += "Suffix";
      }
      return composite;
    }
  }
}
