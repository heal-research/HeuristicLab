using System;
using System.Collections.Generic;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  public interface ILayoutAdapter<T> where T : class {
    IEnumerable<LayoutNode<T>> Convert(T root, Func<T, LayoutNode<T>> convertFunc);
  }
}