
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  public class ReingoldTilfordLayoutEngine<T> : ILayoutEngine<T> where T : class {
    public int NodeWidth { get; set; }
    public int NodeHeight { get; set; }
    private int minHorizontalSpacing = 5;
    public int HorizontalSpacing {
      get { return minHorizontalSpacing; }
      set { minHorizontalSpacing = value; }
    }

    private int minVerticalSpacing = 5;
    public int VerticalSpacing {
      get { return minVerticalSpacing; }
      set { minVerticalSpacing = value; }
    }

    private readonly Func<T, IEnumerable<T>> GetChildren;

    public ReingoldTilfordLayoutEngine(Func<T, IEnumerable<T>> GetChildren) {
      this.GetChildren = GetChildren;
    }

    public IEnumerable<VisualTreeNode<T>> CalculateLayout(T root) {
      return CalculateLayout(root, 0, 0);
    }

    public IEnumerable<VisualTreeNode<T>> CalculateLayout(T root, float width, float height) {
      Dictionary<T, LayoutNode<T>> layoutNodeMap = new Dictionary<T, LayoutNode<T>>();
      var layoutRoot = new LayoutNode<T> { Content = root, Width = NodeWidth, Height = NodeHeight, };
      layoutRoot.Ancestor = layoutRoot;
      Expand(layoutRoot, layoutNodeMap);

      FirstWalk(layoutRoot);
      SecondWalk(layoutRoot, -layoutRoot.Prelim);
      NormalizeCoordinates(layoutNodeMap.Values);
      if (width > 0 && height > 0) {
        FitToBounds(width, height, layoutNodeMap.Values);
        Center(width, height, layoutNodeMap.Values);
      }

      return layoutNodeMap.Values.Select(x => new VisualTreeNode<T>(x.Content) {
        Width = (int)Math.Round(x.Width),
        Height = (int)Math.Round(x.Height),
        X = (int)Math.Round(x.X),
        Y = (int)Math.Round(x.Y)
      });
    }

    private void Expand(LayoutNode<T> lRoot, Dictionary<T, LayoutNode<T>> map) {
      map.Add(lRoot.Content, lRoot);
      var children = GetChildren(lRoot.Content).ToList();
      if (!children.Any()) return;
      lRoot.Children = new List<LayoutNode<T>>(children.Count);
      for (int i = 0; i < children.Count; ++i) {
        var node = new LayoutNode<T> {
          Content = children[i],
          Number = i,
          Parent = lRoot,
          Level = lRoot.Level + 1,
          Width = NodeWidth,
          Height = NodeHeight
        };
        node.Ancestor = node;
        lRoot.Children.Add(node);
        Expand(node, map);
      }
    }


    /// <summary>
    /// Transform LayoutNode coordinates so that all coordinates are positive and start from (0,0)
    /// </summary>
    private static void NormalizeCoordinates(IEnumerable<LayoutNode<T>> nodes) {
      float xmin = 0, ymin = 0;
      foreach (var node in nodes) {
        if (xmin > node.X) xmin = node.X;
        if (ymin > node.Y) ymin = node.Y;
      }
      foreach (var node in nodes) {
        node.X -= xmin;
        node.Y -= ymin;
      }
    }

    private void Center(float width, float height, IEnumerable<LayoutNode<T>> nodes) {
      // center layout on screen
      var bounds = Bounds(nodes);
      float dx = 0, dy = 0;
      if (width > bounds.Width) { dx = (width - bounds.Width) / 2f; }
      if (height > bounds.Height) { dy = (height - bounds.Height) / 2f; }
      foreach (var node in nodes) { node.Translate(dx, dy); }
    }

    private void FitToBounds(float width, float height, IEnumerable<LayoutNode<T>> nodes) {
      var bounds = Bounds(nodes);
      var myWidth = bounds.Width;
      var myHeight = bounds.Height;

      if (myWidth <= width && myHeight <= height) return; // no need to fit since we are within bounds

      var layers = nodes.GroupBy(node => node.Level, node => node).ToList();

      if (myWidth > width) {
        // need to scale horizontally
        float x = width / myWidth;
        foreach (var node in layers.SelectMany(g => g)) {
          node.X *= x;
          node.Width *= x;
        }
        float spacing = minHorizontalSpacing * x;
        foreach (var layer in layers) {
          var nodesLayer = layer.ToList();
          float minWidth = float.MaxValue;
          for (int i = 0; i < nodesLayer.Count - 1; ++i) { minWidth = Math.Min(minWidth, nodesLayer[i + 1].X - nodesLayer[i].X); }
          float w = Math.Min(NodeWidth, minWidth - spacing);
          foreach (var node in nodesLayer) {
            node.X += (node.Width - w) / 2f;
            node.Width = w;
            //this is a simple solution to ensure that the leftmost and rightmost nodes are not drawn partially offscreen due to scaling and offset
            //this should work well enough 99.9% of the time with no noticeable visual difference
            if (node.X < 0) {
              node.Width += node.X;
              node.X = 0;
            } else if (node.X + node.Width > width) {
              node.Width = width - node.X;
            }
          }
        }
      }
      if (myHeight > height) {
        // need to scale vertically
        float x = height / myHeight;
        foreach (var node in layers.SelectMany(g => g)) {
          node.Y *= x;
          node.Height *= x;
        }
      }
    }


    /// <summary>
    /// Returns the bounding box for this layout. When the layout is normalized, the rectangle should be [0,0,xmin,xmax].
    /// </summary>
    /// <returns></returns>
    private RectangleF Bounds(IEnumerable<LayoutNode<T>> nodes) {
      float xmin = 0, xmax = 0, ymin = 0, ymax = 0;
      foreach (LayoutNode<T> node in nodes) {
        float x = node.X, y = node.Y;
        if (xmin > x) xmin = x;
        if (xmax < x) xmax = x;
        if (ymin > y) ymin = y;
        if (ymax < y) ymax = y;
      }
      return new RectangleF(xmin, ymin, xmax + NodeWidth, ymax + NodeHeight);
    }

    #region methods specific to the reingold-tilford layout algorithm
    private void FirstWalk(LayoutNode<T> v) {
      LayoutNode<T> w;
      if (v.IsLeaf) {
        w = v.LeftSibling;
        if (w != null) {
          v.Prelim = w.Prelim + minHorizontalSpacing + NodeWidth;
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
          v.Prelim = w.Prelim + minHorizontalSpacing + NodeWidth;
          v.Mod = v.Prelim - midPoint;
        } else {
          v.Prelim = midPoint;
        }
      }
    }

    private void SecondWalk(LayoutNode<T> v, float m) {
      v.X = v.Prelim + m;
      v.Y = v.Level * (minVerticalSpacing + NodeHeight);
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
        float shift = (vim.Prelim + sim) - (vip.Prelim + sip) + minHorizontalSpacing + NodeWidth;
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
    #endregion
  }
}
