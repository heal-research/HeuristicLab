using System.Collections.Generic;

namespace HeuristicLab.Persistence.Core {

  public class Tag {
    public List<Thunk> finalFixes;
    public string Name { get; private set; }
    public object Value;      

    public Tag(string name, object value) {
      Name = name;
      Value = value;
    }
    public Tag(object value) {
      Name = null;
      Value = value;
    }
    public void SafeSet(Setter setter) {
      if ( Value != null && Value.GetType() == typeof(ParentReference))
        finalFixes.Add(() => setter(Value));
      else
        setter(Value);
    }
  }  

}