
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  public class ReingoldTilfordLayoutEngine<T> where T : class {
    private readonly Dictionary<T, LayoutNode<T>> nodeMap; // provides a reverse mapping T => LayoutNode

    public ReingoldTilfordLayoutEngine() {
      nodeMap = new Dictionary<T, LayoutNode<T>>();
    }

    public Dictionary<T, LayoutNode<T>> NodeMap { get { return nodeMap; } }

    public void AddNode(T content) {
      if (nodeMap.ContainsKey(content)) {
        throw new ArgumentException("Content already present in the dictionary.");
      }
      var node = new LayoutNode<T> { Content = content };
      nodeMap.Add(content, node);
    }

    public void AddNode(LayoutNode<T> node) {
      var content = node.Content;
      if (nodeMap.ContainsKey(content)) {
        throw new ArgumentException("Content already present in the dictionary.");
      }
      nodeMap.Add(content, node);
    }

    public void AddNodes(IEnumerable<LayoutNode<T>> nodes) {
      foreach (var node in nodes)
        nodeMap.Add(node.Content, node);
    }

    public LayoutNode<T> GetNode(T content) {
      LayoutNode<T> layoutNode;
      nodeMap.TryGetValue(content, out layoutNode);
      return layoutNode;
    }

    private float minHorizontalSpacing = 5;
    public float MinHorizontalSpacing {
      get { return minHorizontalSpacing; }
      set { minHorizontalSpacing = value; }
    }

    private float minVerticalSpacing = 5;
    public float MinVerticalSpacing {
      get { return minVerticalSpacing; }
      set { minVerticalSpacing = value; }
    }

    private LayoutNode<T> root;
    public LayoutNode<T> Root {
      get { return root; }
      set {
        root = value;
      }
    }

    public void ResetCoordinates() {
      foreach (var node in nodeMap.Values) {
        node.X = 0;
        node.Y = 0;
      }
    }

    /// <summary>
    /// Transform LayoutNode coordinates so that all coordinates are positive and start from 0.
    /// </summary>
    private void NormalizeCoordinates() {
      var list = nodeMap.Values.ToList();
      float xmin = 0, ymin = 0;
      for (int i = 0; i < list.Count; ++i) {
        if (xmin > list[i].X) xmin = list[i].X;
        if (ymin > list[i].Y) ymin = list[i].Y;
      }
      for (int i = 0; i < list.Count; ++i) {
        list[i].X -= xmin;
        list[i].Y -= ymin;
      }
    }

    public void Reset() {
      root = null;
      nodeMap.Clear();
    }

    public void ResetParameters() {
      foreach (var layoutNode in nodeMap.Values) {
        // reset layout-related parameters 
        layoutNode.Ancestor = layoutNode;
        layoutNode.Thread = null;
        layoutNode.Change = 0;
        layoutNode.Shift = 0;
        layoutNode.Prelim = 0;
        layoutNode.Mod = 0;
      }
    }

    public void CalculateLayout() {
      if (root == null)
        throw new Exception("Root cannot be null.");
      ResetCoordinates(); // reset node X,Y coordinates
      ResetParameters(); // reset node parameters like Mod, Shift etc. 
      FirstWalk(root);
      SecondWalk(root, -root.Prelim);
      NormalizeCoordinates();
    }

    /// <summary>
    /// Returns a map of coordinates for each LayoutNode in the symbolic expression tree.
    /// </summary>
    /// <returns></returns>
    public Dictionary<T, PointF> GetNodeCoordinates() {
      return nodeMap.ToDictionary(x => x.Key, x => new PointF(x.Value.X, x.Value.Y));
    }

    /// <summary>
    /// Returns the bounding box for this layout. When the layout is normalized, the rectangle should be [0,0,xmin,xmax].
    /// </summary>
    /// <returns></returns>
    public RectangleF Bounds() {
      float xmin, xmax, ymin, ymax; xmin = xmax = ymin = ymax = 0;
      var list = nodeMap.Values.ToList();
      for (int i = 0; i < list.Count; ++i) {
        float x = list[i].X, y = list[i].Y;
        if (xmin > x) xmin = x;
        if (xmax < x) xmax = x;
        if (ymin > y) ymin = y;
        if (ymax < y) ymax = y;
      }
      return new RectangleF(xmin, ymin, xmax + minHorizontalSpacing, ymax + minVerticalSpacing);
    }

    private void FirstWalk(LayoutNode<T> v) {
      LayoutNode<T> w;
      if (v.IsLeaf) {
        w = v.LeftSibling;
        if (w != null) {
          v.Prelim = w.Prelim + minHorizontalSpacing;
        }
      } else {
        var defaultAncestor = v.Children[0]; // leftmost child

        foreach (var child in v.Children) {
          FirstWalk(child);
          Apportion(child, ref defaultAncestor);
        }
        ExecuteShifts(v);
        var leftmost = v.Children.First();
        var rightmost = v.Children.Last();
        float midPoint = (leftmost.Prelim + rightmost.Prelim) / 2;
        w = v.LeftSibling;
        if (w != null) {
          v.Prelim = w.Prelim + minHorizontalSpacing;
          v.Mod = v.Prelim - midPoint;
        } else {
          v.Prelim = midPoint;
        }
      }
    }

    private void SecondWalk(LayoutNode<T> v, float m) {
      v.X = v.Prelim + m;
      v.Y = v.Level * minVerticalSpacing;
      if (v.IsLeaf) return;
      foreach (var child in v.Children) {
        SecondWalk(child, m + v.Mod);
      }
    }

    private void Apportion(LayoutNode<T> v, ref LayoutNode<T> defaultAncestor) {
      var w = v.LeftSibling;
      if (w == null) return;
      LayoutNode<T> vip = v;
      LayoutNode<T> vop = v;
      LayoutNode<T> vim = w;
      LayoutNode<T> vom = vip.LeftmostSibling;

      float sip = vip.Mod;
      float sop = vop.Mod;
      float sim = vim.Mod;
      float som = vom.Mod;

      while (vim.NextRight != null && vip.NextLeft != null) {
        vim = vim.NextRight;
        vip = vip.NextLeft;
        vom = vom.NextLeft;
        vop = vop.NextRight;
        vop.Ancestor = v;
        float shift = (vim.Prelim + sim) - (vip.Prelim + sip) + minHorizontalSpacing;
        if (shift > 0) {
          var ancestor = Ancestor(vim, v) ?? defaultAncestor;
          MoveSubtree(ancestor, v, shift);
          sip += shift;
          sop += shift;
        }
        sim += vim.Mod;
        sip += vip.Mod;
        som += vom.Mod;
        sop += vop.Mod;
      }
      if (vim.NextRight != null && vop.NextRight == null) {
        vop.Thread = vim.NextRight;
        vop.Mod += (sim - sop);
      }
      if (vip.NextLeft != null && vom.NextLeft == null) {
        vom.Thread = vip.NextLeft;
        vom.Mod += (sip - som);
        defaultAncestor = v;
      }
    }

    private void MoveSubtree(LayoutNode<T> wm, LayoutNode<T> wp, float shift) {
      int subtrees = wp.Number - wm.Number; // TODO: Investigate possible bug (if the value ever happens to be zero) - happens when the tree is actually a graph (but that's outside the use case of this algorithm which only works with trees)
      if (subtrees == 0) throw new Exception("MoveSubtree failed: check if object is really a tree (no cycles)");
      wp.Change -= shift / subtrees;
      wp.Shift += shift;
      wm.Change += shift / subtrees;
      wp.Prelim += shift;
      wp.Mod += shift;
    }

    private void ExecuteShifts(LayoutNode<T> v) {
      if (v.IsLeaf) return;
      float shift = 0;
      float change = 0;
      for (int i = v.Children.Count - 1; i >= 0; --i) {
        var w = v.Children[i];
        w.Prelim += shift;
        w.Mod += shift;
        change += w.Change;
        shift += (w.Shift + change);
      }
    }

    private LayoutNode<T> Ancestor(LayoutNode<T> u, LayoutNode<T> v) {
      var ancestor = u.Ancestor;
      if (ancestor == null) return null;
      return ancestor.Parent == v.Parent ? ancestor : null;
    }
  }
}
