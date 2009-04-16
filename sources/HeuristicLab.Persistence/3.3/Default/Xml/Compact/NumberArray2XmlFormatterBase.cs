using System.Collections;
using System.Text;
using HeuristicLab.Persistence.Interfaces;
using System;

namespace HeuristicLab.Persistence.Default.Xml.Compact {

  public abstract class NumberArray2XmlFormatterBase<T> : FormatterBase<T, XmlString> {

    protected virtual string Separator { get { return ";"; } }
    protected abstract string FormatValue(object o);
    protected abstract object ParseValue(string o);

    public override XmlString Format(T t) {
      object o = (object)t;
      Array a = (Array)o;
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
        sb.Append(Separator);
        sb.Append(FormatValue(a.GetValue(positions)));
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
      return new XmlString(sb.ToString());
    }

    public override T Parse(XmlString x) {
      IEnumerator values =
        x.Data.Split(new[] { Separator },
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
      Array a = Array.CreateInstance(this.SourceType.GetElementType(), lengths, lowerBounds);
      int[] positions = new int[rank];
      while (values.MoveNext()) {
        a.SetValue(ParseValue((string)values.Current), positions);
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
      object o = a;
      return (T)o;
    }
  }

}