#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Drawing;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  public class VisualSymbolicExpressionTreeNode : object {
    private static readonly Color defaultLineColor = Color.Black;
    private static readonly Color defaultTextColor = Color.Black;
    private static readonly Color defaultFillColor = Color.White;
    private const int defaultPreferredWidth = 70;
    private const int defaultPreferredHeight = 46;

    public VisualSymbolicExpressionTreeNode(ISymbolicExpressionTreeNode symbolicExpressionTreeNode) :
      this(symbolicExpressionTreeNode, defaultLineColor) {
    }

    public VisualSymbolicExpressionTreeNode(ISymbolicExpressionTreeNode symbolicExpressionTreeNode, Color lineColor) :
      this(symbolicExpressionTreeNode, lineColor, defaultTextColor) {
    }

    public VisualSymbolicExpressionTreeNode(ISymbolicExpressionTreeNode symbolicExpressionTreeNode, Color lineColor, Color textColor) :
      this(symbolicExpressionTreeNode, lineColor, textColor, defaultFillColor) {
    }

    public VisualSymbolicExpressionTreeNode(ISymbolicExpressionTreeNode symbolicExpressionTreeNode, Color lineColor, Color textColor, Color fillColor) :
      this(symbolicExpressionTreeNode, lineColor, textColor, fillColor, defaultPreferredWidth, defaultPreferredHeight) {
    }

    public VisualSymbolicExpressionTreeNode(ISymbolicExpressionTreeNode symbolicExpressionTreeNode, Color lineColor, Color textColor, Color fillColor, int width, int height) {
      this.symbolicExpressionTreeNode = symbolicExpressionTreeNode;
      this.lineColor = lineColor;
      this.textColor = textColor;
      this.fillColor = fillColor;
      this.preferredWidth = width;
      this.preferredHeight = height;
      this.ToolTip = symbolicExpressionTreeNode.ToString();
    }

    #region members for internal use only
    private int x;
    public int X {
      get { return x; }
      internal set { this.x = value; }
    }

    private int y;
    public int Y {
      get { return y; }
      internal set { this.y = value; }
    }

    private int width;
    public int Width {
      get { return this.width; }
      internal set { this.width = value; }
    }

    private int height;
    public int Height {
      get { return this.height; }
      internal set { this.height = value; }
    }
    #endregion

    private ISymbolicExpressionTreeNode symbolicExpressionTreeNode;
    public ISymbolicExpressionTreeNode SymbolicExpressionTreeNode {
      get { return this.symbolicExpressionTreeNode; }
      set {
        symbolicExpressionTreeNode = value;
        ToolTip = SymbolicExpressionTreeNode.ToString();
      }
    }

    private int preferredWidth;
    public int PreferredWidth {
      get { return this.preferredWidth; }
      set { this.preferredWidth = value; }
    }

    private int preferredHeight;
    public int PreferredHeight {
      get { return this.preferredHeight; }
      set { this.preferredHeight = value; }
    }

    private Color lineColor;
    public Color LineColor {
      get { return this.lineColor; }
      set { this.lineColor = value; }
    }

    private Color textColor;
    public Color TextColor {
      get { return this.textColor; }
      set { this.textColor = value; }
    }

    private Color fillColor;
    public Color FillColor {
      get { return this.fillColor; }
      set { this.fillColor = value; }
    }

    private string toolTip;
    public string ToolTip {
      get { return toolTip; }
      set { toolTip = value; }
    }
  }
}
