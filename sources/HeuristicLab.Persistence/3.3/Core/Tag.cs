using System.Collections.Generic;

namespace HeuristicLab.Persistence.Core {

  public class Tag {
    internal List<Thunk> globalFinalFixes; // reference to final fixes of Deserializer
    public string Name { get; private set; }
    public object Value { get; set; }     

    public Tag(string name, object value) {
      Name = name;
      Value = value;
    }
    public Tag(object value) {
      Name = null;
      Value = value;
    }
    public void SafeSet(Setter setter) {
      if ( Value as ParentReference != null)
        globalFinalFixes.Add(() => setter(Value));
      else
        setter(Value);
    }
  }  

}