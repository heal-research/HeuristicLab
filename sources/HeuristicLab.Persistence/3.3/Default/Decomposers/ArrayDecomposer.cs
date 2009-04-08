using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Collections.Generic;

namespace HeuristicLab.Persistence.Default.Decomposers {
    
  public class ArrayDecomposer : IDecomposer {

    public int Priority {
      get { return 100; }
    }

    public bool CanDecompose(Type type) {
      return type.IsArray || type == typeof(Array);
    }

    public IEnumerable<Tag> DeCompose(object array) {
      Array a = (Array)array;      
      yield return new Tag("rank", a.Rank);
      int[] lengths = new int[a.Rank];
      int[] lowerBounds = new int[a.Rank];
      for (int i = 0; i < a.Rank; i++) {
        lengths[i] = a.GetLength(i);
        yield return new Tag("length_" + i, a.GetLength(i));
      }
      for (int i = 0; i < a.Rank; i++) {
        lowerBounds[i] = a.GetLowerBound(i);
        yield return new Tag("lowerBound_" + i, a.GetLowerBound(i));
      }
      int[] positions = (int[])lowerBounds.Clone();
      while (positions[a.Rank - 1] < lengths[a.Rank - 1] + lowerBounds[a.Rank - 1]) {
        yield return new Tag(a.GetValue(positions));
        positions[0] += 1;
        for (int i = 0; i < a.Rank - 1; i++) {
          if (positions[i] >= lowerBounds[i] + lengths[i]) {
            positions[i] = lowerBounds[i];
            positions[i + 1] += 1;
          } else {
            break;
          }
        }
      }
    }

    public object CreateInstance(Type t) {
      return null;
    }

    public object Populate(object instance, IEnumerable<Tag> elements, Type t) {
      IEnumerator<Tag> e = elements.GetEnumerator();
      e.MoveNext();
      int rank = (int)e.Current.Value;
      int[] lengths = new int[rank];
      for (int i = 0; i < rank; i++) {
        e.MoveNext();
        lengths[i] = (int)e.Current.Value;
      }
      int[] lowerBounds = new int[rank];
      for (int i = 0; i < rank; i++) {
        e.MoveNext();
        lowerBounds[i] = (int)e.Current.Value;
      }      
      Array a = Array.CreateInstance(t.GetElementType(), lengths, lowerBounds);
      int[] positions = (int[])lowerBounds.Clone();
      while (e.MoveNext()) {
        int[] currentPositions = positions;
        e.Current.SafeSet(value => a.SetValue(value, currentPositions));        
        positions[0] += 1;
        for (int i = 0; i < rank-1; i++) {
          if (positions[i] >= lengths[i]+lowerBounds[i]) {
            positions[i] = lowerBounds[i];
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
