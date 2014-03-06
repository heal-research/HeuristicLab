
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  interface ILayoutEngine<T> where T : class {
    int NodeWidth { get; set; }
    int NodeHeight { get; set; }
    int HorizontalSpacing { get; set; }
    int VerticalSpacing { get; set; }

    void CalculateLayout();
    void CalculateLayout(float width, float height);
    void Initialize(T root, Func<T, IEnumerable<T>> getChildren, Func<T, int> getLength = null, Func<T, int> getDepth = null);
    void Clear();
    void Reset();

    // function members necessary to navigate the tree structure
    Func<T, IEnumerable<T>> GetChildren { get; set; }
    Func<T, int> GetLength { get; set; }
    Func<T, int> GetDepth { get; set; }

    IEnumerable<T> GetContentNodes();
    IEnumerable<VisualTreeNode<T>> GetVisualNodes();
    Dictionary<T, PointF> GetCoordinates();
  }
}
