#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  public class BoxesLayoutEngine<T> : ILayoutEngine<T> where T : class {
    public int NodeWidth { get; set; }
    public int NodeHeight { get; set; }
    public int HorizontalSpacing { get; set; }
    public int VerticalSpacing { get; set; }

    private readonly Func<T, IEnumerable<T>> GetChildren;
    private readonly Func<T, int> GetLength;
    private readonly Func<T, int> GetDepth;

    public BoxesLayoutEngine(Func<T, IEnumerable<T>> GetChildren, Func<T, int> GetLength, Func<T, int> GetDepth) {
      if (GetChildren == null) throw new ArgumentNullException("GetChildren");
      if (GetLength == null) throw new ArgumentNullException("GetLength");
      if (GetDepth == null) throw new ArgumentNullException("GetDepth");

      this.GetChildren = GetChildren;
      this.GetLength = GetLength;
      this.GetDepth = GetDepth;
    }


    public IEnumerable<VisualTreeNode<T>> CalculateLayout(T root, float width, float height) {
      var nodeMap = new Dictionary<T, VisualTreeNode<T>>();
      CreateVisualNodes(root, nodeMap);
      RecursiveLayout(nodeMap, nodeMap[root], 0, 0, (int)Math.Round(width), (int)Math.Round(height) / GetDepth(root));
      return nodeMap.Values;
    }

    private void CreateVisualNodes(T root, Dictionary<T, VisualTreeNode<T>> map) {
      var node = new VisualTreeNode<T>(root) {
        PreferredWidth = NodeWidth,
        PreferredHeight = NodeHeight
      };

      map.Add(root, node);
      var children = GetChildren(root).ToList();
      if (children.Any()) {
        foreach (var child in children) {
          CreateVisualNodes(child, map);
        }
      }
    }

    private void RecursiveLayout(Dictionary<T, VisualTreeNode<T>> nodeMap, VisualTreeNode<T> visualTreeNode, int x, int y, int width, int height) {
      float center_x = x + width / 2;
      float center_y = y + height / 2;
      int actualWidth = width - HorizontalSpacing;
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
      //calculate areas for the subtrees according to their tree size 
      var node = visualTreeNode.Content;
      var children = GetChildren(node).ToList();
      int[] xBoundaries = new int[children.Count + 1];
      xBoundaries[0] = x;
      for (int i = 0; i < children.Count; i++) {
        xBoundaries[i + 1] = (int)(xBoundaries[i] + (width * (double)GetLength(children[i])) / (GetLength(node) - 1));
        RecursiveLayout(nodeMap, nodeMap[children[i]], xBoundaries[i], y + height, xBoundaries[i + 1] - xBoundaries[i], height);
      }
    }
  }
}
