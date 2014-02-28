
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  public class BoxesLayoutEngine<T> : ILayoutEngine<T> where T : class {
    private readonly Dictionary<T, VisualTreeNode<T>> nodeMap;

    public int NodeWidth { get; set; }
    public int NodeHeight { get; set; }
    public int HorizontalSpacing { get; set; }
    public int VerticalSpacing { get; set; }
    private VisualTreeNode<T> layoutRoot;

    public int Width { get; private set; }
    public int Height { get; private set; }

    public Func<T, IEnumerable<T>> GetChildren { get; set; }
    public Func<T, int> GetLength { get; set; }
    public Func<T, int> GetDepth { get; set; }

    public BoxesLayoutEngine() {
      nodeMap = new Dictionary<T, VisualTreeNode<T>>();
    }

    public void CalculateLayout() {
      throw new Exception("The BoxesLayoutEngine does not support arbitrary bounds. Please use method CalculateLayout(Width, Height)");
    }

    public void CalculateLayout(float width, float height) {
      Width = (int)Math.Round(width);
      Height = (int)Math.Round(height);
      Reset();
      RecursiveLayout(layoutRoot, 0, 0, Width, Height / GetDepth(layoutRoot.Content));
    }

    public void Initialize(T root, Func<T, IEnumerable<T>> getChildren, Func<T, int> getLength, Func<T, int> getDepth) {
      if (getChildren == null || getLength == null || getDepth == null)
        throw new ArgumentNullException("The BoxesLayoutEngine requires all of the lambdas: (getChildren, getLength and getDepth) to be defined.");
      GetChildren = getChildren;
      GetLength = getLength;
      GetDepth = getDepth;
      Clear();
      Expand(root); // produce the nodeMap
      layoutRoot = nodeMap[root];
    }

    private void Expand(T root) {
      var node = new VisualTreeNode<T>(root) {
        PreferredWidth = NodeWidth,
        PreferredHeight = NodeHeight
      };
      nodeMap.Add(root, node);
      var children = GetChildren(root).ToList();
      if (children.Any()) {
        foreach (var child in children) {
          Expand(child);
        }
      }
    }

    public void Center(float width, float height) {
      // does nothing because the BoxesLayout centers the tree by default
    }

    public void Clear() {
      nodeMap.Clear();
      layoutRoot = null;
    }

    public void Reset() {
      foreach (var node in nodeMap.Values) {
        node.X = 0;
        node.Y = 0;
      }
    }

    public Dictionary<T, PointF> GetCoordinates() {
      return nodeMap.ToDictionary(x => x.Key, x => new PointF(x.Value.X, x.Value.Y));
    }

    public void FitToBounds(float width, float height) {
      // does nothing because the BoxesLayout is by default stretched on the whole drawing area
    }

    private void RecursiveLayout(VisualTreeNode<T> visualTreeNode, int x, int y, int width, int height) {
      float center_x = x + width / 2;
      float center_y = y + height / 2;
      int actualWidth = width - VerticalSpacing;
      int actualHeight = height - VerticalSpacing;

      //calculate size of node
      if (actualWidth >= visualTreeNode.PreferredWidth && actualHeight >= visualTreeNode.PreferredHeight) {
        visualTreeNode.Width = visualTreeNode.PreferredWidth;
        visualTreeNode.Height = visualTreeNode.PreferredHeight;
        visualTreeNode.X = (int)center_x - visualTreeNode.Width / 2;
        visualTreeNode.Y = (int)center_y - visualTreeNode.Height / 2;
      }
        //width too small to draw in desired sized
      else if (actualWidth < visualTreeNode.PreferredWidth && actualHeight >= visualTreeNode.PreferredHeight) {
        visualTreeNode.Width = actualWidth;
        visualTreeNode.Height = visualTreeNode.PreferredHeight;
        visualTreeNode.X = x;
        visualTreeNode.Y = (int)center_y - visualTreeNode.Height / 2;
      }
        //height too small to draw in desired sized
      else if (actualWidth >= visualTreeNode.PreferredWidth && actualHeight < visualTreeNode.PreferredHeight) {
        visualTreeNode.Width = visualTreeNode.PreferredWidth;
        visualTreeNode.Height = actualHeight;
        visualTreeNode.X = (int)center_x - visualTreeNode.Width / 2;
        visualTreeNode.Y = y;
      }
        //width and height too small to draw in desired size
      else {
        visualTreeNode.Width = actualWidth;
        visualTreeNode.Height = actualHeight;
        visualTreeNode.X = x;
        visualTreeNode.Y = y;
      }
      //calculate areas for the subtrees according to their tree size and call drawFunctionTree
      var node = visualTreeNode.Content;
      var children = GetChildren(node).ToList();
      int[] xBoundaries = new int[children.Count + 1];
      xBoundaries[0] = x;
      for (int i = 0; i < children.Count; i++) {
        xBoundaries[i + 1] = (int)(xBoundaries[i] + (width * (double)GetLength(children[i])) / (GetLength(node) - 1));
        RecursiveLayout(nodeMap[children[i]], xBoundaries[i], y + height, xBoundaries[i + 1] - xBoundaries[i], height);
      }
    }

    public IEnumerable<VisualTreeNode<T>> GetVisualNodes() {
      return nodeMap.Values;
    }
  }
}
