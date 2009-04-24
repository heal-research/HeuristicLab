using System.Collections;
using System.Text;
using HeuristicLab.Persistence.Interfaces;
using System;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Persistence.Default.Xml.Compact {

  public abstract class NumberArray2XmlFormatterBase<T> : CompactXmlFormatterBase<T> where T : class {

    protected virtual string Separator { get { return ";"; } }
    protected abstract string FormatValue(object o);
    protected abstract object ParseValue(string o);

    public override XmlString Format(T t) {      
      Array a = (Array)(object)t;
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
      try {
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
        int[] positions = (int[])lowerBounds.Clone();
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
        if (positions[rank - 1] != lowerBounds[rank - 1] + lengths[rank - 1])
          throw new PersistenceException("Insufficient number of elements while trying to fill number array.");
        return (T)(object)a;
      } catch (InvalidOperationException e) {
        throw new PersistenceException("Insufficient information to rebuild number array.", e);
      } catch (InvalidCastException e) {
        throw new PersistenceException("Invalid element data or meta data to reconstruct number array.", e);
      } catch (OverflowException e) {
        throw new PersistenceException("Overflow during element parsing while trying to reconstruct number array.", e);
      } 
    }
  }

}