using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using Newtonsoft.Json;

namespace HeuristicLab.Manufacture {   
  public class JCObject { //Blueprint, Component,  ?
    public string Name { get; set; }
    public string Type { get; set; }
    public IList<ParameterData> FreeParameters { get; set; }
    public IList<ParameterData> StaticParameters { get; set; }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append($"{Name}: [");
      foreach(var x in StaticParameters) {
        sb.Append(x?.ToString());
      }
      sb.Append($"\n]");
      return sb.ToString();
    }
  }

  public class ParameterData {
    public string Name { get; set; }
    [JsonIgnore]
    public string Type { get; set; }
    public object Default { get; set; }
    //[JsonIgnore]
    public string Path { get; set; }
    public IList<object> Range { get; set; }
    public IList<ParameterData> Parameters { get; set; }
    public IList<ParameterData> Operators { get; set; }

    public ParameterData() { }
    /*
    public ParameterData(ParameterData original) {
      this.Name = original.Name;
      this.Type = original.Type;
      this.Default = original.Default;
      this.Path = original.Path;
      this.Name = original.Name;
      this.Name = original.Name;
    }*/
    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append($"\n{Name}, {Default}, Range[");
      if(Range != null) {
        foreach (var x in Range) {
          sb.Append(x.ToString() + ", ");
        }
      }
      sb.Append($"],");
      return sb.ToString();
    }
  }


}
