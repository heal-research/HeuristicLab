using System.Collections;
using System.Text;
using HeuristicLab.Persistence.Interfaces;
using System;
using HeuristicLab.Persistence.Default.ViewOnly;

namespace HeuristicLab.Persistence.Default.Xml.Compact {
                                                              
  public abstract class NumberArray2XmlFormatter : IFormatter {

    public abstract Type Type { get; }
    public IFormat Format { get { return XmlFormat.Instance; } }
    protected virtual string Separator { get { return ";"; } }
    protected abstract string formatValue(object o);
    protected abstract object parseValue(string o);

    public object DoFormat(object obj) {
      Array a = (Array)obj;
      int[] lengths = new int[a.Rank];
      int[] lowerBounds = new int[a.Rank];
      StringBuilder sb = new StringBuilder();
      sb.Append(a.Rank);
      for (int i = 0; i < a.Rank; i++) {
        sb.Append(Separator);
        sb.Append(a.GetLength(i));
        lengths[i] = a.GetLength(i);
      }
      for (int i = 0; i < a.Rank; i++) {
        sb.Append(Separator);
        sb.Append(a.GetLowerBound(i));
        lowerBounds[i] = a.GetLowerBound(i);
      }
      int[] positions = (int[])lowerBounds.Clone();
      while (positions[a.Rank - 1] < lengths[a.Rank - 1] + lowerBounds[a.Rank - 1]) {
        Console.WriteLine(ViewOnlyGenerator.Serialize(positions));
        sb.Append(Separator);
        sb.Append(formatValue(a.GetValue(positions)));
        positions[0] += 1;
        for (int i = 0; i < a.Rank - 1; i++) {
          if (positions[i] >= lengths[i] + lowerBounds[i]) {
            positions[i] = lowerBounds[i];
            positions[i + 1] += 1;
          } else {
            break;
          }
        }
      }      
      return sb.ToString();
    }

    public object Parse(object o) {
      IEnumerator values =
        ((string)o)
        .Split(new[] { Separator },
        StringSplitOptions.RemoveEmptyEntries).GetEnumerator();
      values.MoveNext();
      int rank = int.Parse((string)values.Current);
      int[] lengths = new int[rank];
      for (int i = 0; i < rank; i++) {
        values.MoveNext();
        lengths[i] = int.Parse((string)values.Current);
      }
      int[] lowerBounds = new int[rank];
      for (int i = 0; i < rank; i++) {
        values.MoveNext();
        lowerBounds[i] = int.Parse((string)values.Current);
      }
      Array a = Array.CreateInstance(this.Type.GetElementType(), lengths, lowerBounds);
      int[] positions = new int[rank];
      while (values.MoveNext()) {
        a.SetValue(parseValue((string)values.Current), positions);
        positions[0] += 1;
        for (int i = 0; i < rank - 1; i++) {
          if (positions[i] >= lengths[i]) {
            positions[i] = 0;
            positions[i + 1] += 1;
          } else {
            break;
          }
        }
      }
      return a;
    }
  }
  
}