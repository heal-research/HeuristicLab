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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  public partial class SymbolicExpressionTreeChart : UserControl {
    private Image image;
    private StringFormat stringFormat;
    private Dictionary<ISymbolicExpressionTreeNode, VisualSymbolicExpressionTreeNode> visualTreeNodes;
    private Dictionary<Tuple<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode>, VisualSymbolicExpressionTreeNodeConnection> visualLines;

    public SymbolicExpressionTreeChart() {
      InitializeComponent();
      this.image = new Bitmap(Width, Height);
      this.stringFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
      this.spacing = 5;
      this.lineColor = Color.Black;
      this.backgroundColor = Color.White;
      this.textFont = new Font("Times New Roman", 8);
    }

    public SymbolicExpressionTreeChart(ISymbolicExpressionTree tree)
      : this() {
      this.Tree = tree;
    }

    private int spacing;
    public int Spacing {
      get { return this.spacing; }
      set {
        this.spacing = value;
        this.Repaint();
      }
    }

    private Color lineColor;
    public Color LineColor {
      get { return this.lineColor; }
      set {
        this.lineColor = value;
        this.Repaint();
      }
    }

    private Color backgroundColor;
    public Color BackgroundColor {
      get { return this.backgroundColor; }
      set {
        this.backgroundColor = value;
        this.Repaint();
      }
    }

    private Font textFont;
    public Font TextFont {
      get { return this.textFont; }
      set {
        this.textFont = value;
        this.Repaint();
      }
    }

    private ISymbolicExpressionTree tree;
    public ISymbolicExpressionTree Tree {
      get { return this.tree; }
      set {
        tree = value;
        visualTreeNodes = new Dictionary<ISymbolicExpressionTreeNode, VisualSymbolicExpressionTreeNode>();
        visualLines = new Dictionary<Tuple<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode>, VisualSymbolicExpressionTreeNodeConnection>();
        if (tree != null) {
          foreach (ISymbolicExpressionTreeNode node in tree.IterateNodesPrefix()) {
            visualTreeNodes[node] = new VisualSymbolicExpressionTreeNode(node);
            if (node.Parent != null) visualLines[Tuple.Create(node.Parent, node)] = new VisualSymbolicExpressionTreeNodeConnection();
          }
        }
        Repaint();
      }
    }

    private bool suspendRepaint;
    public bool SuspendRepaint {
      get { return suspendRepaint; }
      set { suspendRepaint = value; }
    }

    protected override void OnPaint(PaintEventArgs e) {
      e.Graphics.DrawImage(image, 0, 0);
      base.OnPaint(e);
    }
    protected override void OnResize(EventArgs e) {
      base.OnResize(e);
      if (this.Width <= 1 || this.Height <= 1)
        this.image = new Bitmap(1, 1);
      else
        this.image = new Bitmap(Width, Height);
      this.Repaint();
    }

    public void Repaint() {
      if (!suspendRepaint) {
        this.GenerateImage();
        this.Refresh();
      }
    }

    public void RepaintNodes() {
      if (!suspendRepaint) {
        using (var graphics = Graphics.FromImage(image)) {
          graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
          graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
          foreach (var visualNode in visualTreeNodes.Values) {
            DrawTreeNode(graphics, visualNode);
          }
        }
        this.Refresh();
      }
    }

    public void RepaintNode(VisualSymbolicExpressionTreeNode visualNode) {
      if (!suspendRepaint) {
        using (var graphics = Graphics.FromImage(image)) {
          graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
          graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
          DrawTreeNode(graphics, visualNode);
        }
        this.Refresh();
      }
    }

    private void GenerateImage() {
      using (Graphics graphics = Graphics.FromImage(image)) {
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        graphics.Clear(backgroundColor);
        if (tree != null) {
          int height = this.Height / tree.Depth;
          DrawFunctionTree(tree, graphics, 0, 0, this.Width, height);
        }
      }
    }

    public VisualSymbolicExpressionTreeNode GetVisualSymbolicExpressionTreeNode(ISymbolicExpressionTreeNode symbolicExpressionTreeNode) {
      if (visualTreeNodes.ContainsKey(symbolicExpressionTreeNode))
        return visualTreeNodes[symbolicExpressionTreeNode];
      return null;
    }

    public VisualSymbolicExpressionTreeNodeConnection GetVisualSymbolicExpressionTreeNodeConnection(ISymbolicExpressionTreeNode parent, ISymbolicExpressionTreeNode child) {
      if (child.Parent != parent) throw new ArgumentException();
      var key = Tuple.Create(parent, child);
      VisualSymbolicExpressionTreeNodeConnection connection = null;
      visualLines.TryGetValue(key, out connection);
      return connection;
    }

    #region events
    public event MouseEventHandler SymbolicExpressionTreeNodeClicked;
    protected virtual void OnSymbolicExpressionTreeNodeClicked(object sender, MouseEventArgs e) {
      var clicked = SymbolicExpressionTreeNodeClicked;
      if (clicked != null)
        clicked(sender, e);
    }

    private void SymbolicExpressionTreeChart_MouseClick(object sender, MouseEventArgs e) {
      VisualSymbolicExpressionTreeNode visualTreeNode = FindVisualSymbolicExpressionTreeNodeAt(e.X, e.Y);
      if (visualTreeNode != null) {
        OnSymbolicExpressionTreeNodeClicked(visualTreeNode, e);
      }
    }

    public event MouseEventHandler SymbolicExpressionTreeNodeDoubleClicked;
    protected virtual void OnSymbolicExpressionTreeNodeDoubleClicked(object sender, MouseEventArgs e) {
      var doubleClicked = SymbolicExpressionTreeNodeDoubleClicked;
      if (doubleClicked != null)
        doubleClicked(sender, e);
    }

    private void SymbolicExpressionTreeChart_MouseDoubleClick(object sender, MouseEventArgs e) {
      VisualSymbolicExpressionTreeNode visualTreeNode = FindVisualSymbolicExpressionTreeNodeAt(e.X, e.Y);
      if (visualTreeNode != null)
        OnSymbolicExpressionTreeNodeDoubleClicked(visualTreeNode, e);
    }

    public event ItemDragEventHandler SymbolicExpressionTreeNodeDrag;
    protected virtual void OnSymbolicExpressionTreeNodeDragDrag(object sender, ItemDragEventArgs e) {
      var dragged = SymbolicExpressionTreeNodeDrag;
      if (dragged != null)
        dragged(sender, e);
    }

    private VisualSymbolicExpressionTreeNode draggedSymbolicExpressionTree;
    private MouseButtons dragButtons;
    private void SymbolicExpressionTreeChart_MouseDown(object sender, MouseEventArgs e) {
      this.dragButtons = e.Button;
      this.draggedSymbolicExpressionTree = FindVisualSymbolicExpressionTreeNodeAt(e.X, e.Y);
    }
    private void SymbolicExpressionTreeChart_MouseUp(object sender, MouseEventArgs e) {
      this.draggedSymbolicExpressionTree = null;
      this.dragButtons = MouseButtons.None;
    }

    private void SymbolicExpressionTreeChart_MouseMove(object sender, MouseEventArgs e) {
      VisualSymbolicExpressionTreeNode visualTreeNode = FindVisualSymbolicExpressionTreeNodeAt(e.X, e.Y);
      if (draggedSymbolicExpressionTree != null &&
        draggedSymbolicExpressionTree != visualTreeNode) {
        OnSymbolicExpressionTreeNodeDragDrag(draggedSymbolicExpressionTree, new ItemDragEventArgs(dragButtons, draggedSymbolicExpressionTree));
        draggedSymbolicExpressionTree = null;
      } else if (draggedSymbolicExpressionTree == null &&
        visualTreeNode != null) {
        string tooltipText = visualTreeNode.ToolTip;
        if (this.toolTip.GetToolTip(this) != tooltipText)
          this.toolTip.SetToolTip(this, tooltipText);

      } else if (visualTreeNode == null)
        this.toolTip.SetToolTip(this, "");
    }

    public VisualSymbolicExpressionTreeNode FindVisualSymbolicExpressionTreeNodeAt(int x, int y) {
      foreach (var visualTreeNode in visualTreeNodes.Values) {
        if (x >= visualTreeNode.X && x <= visualTreeNode.X + visualTreeNode.Width &&
            y >= visualTreeNode.Y && y <= visualTreeNode.Y + visualTreeNode.Height)
          return visualTreeNode;
      }
      return null;
    }
    #endregion

    #region methods for painting the symbolic expression tree
    private void DrawFunctionTree(ISymbolicExpressionTree tree, Graphics graphics, int x, int y, int width, int height) {
      DrawFunctionTree(tree.Root, graphics, x, y, width, height, Point.Empty);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="functionTree"> function tree to draw</param>
    /// <param name="graphics">graphics object to draw on</param>
    /// <param name="x">x coordinate of drawing area</param>
    /// <param name="y">y coordinate of drawing area</param>
    /// <param name="width">width of drawing area</param>
    /// <param name="height">height of drawing area</param>
    private void DrawFunctionTree(ISymbolicExpressionTreeNode node, Graphics graphics, int x, int y, int width, int height, Point connectionPoint) {
      VisualSymbolicExpressionTreeNode visualTreeNode = visualTreeNodes[node];
      float center_x = x + width / 2;
      float center_y = y + height / 2;
      int actualWidth = width - spacing;
      int actualHeight = height - spacing;

      using (var textBrush = new SolidBrush(visualTreeNode.TextColor))
      using (var nodeLinePen = new Pen(visualTreeNode.LineColor))
      using (var nodeFillBrush = new SolidBrush(visualTreeNode.FillColor)) {

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

        //draw terminal node
        if (node.SubtreeCount == 0) {
          graphics.FillRectangle(nodeFillBrush, visualTreeNode.X, visualTreeNode.Y, visualTreeNode.Width, visualTreeNode.Height);
          graphics.DrawRectangle(nodeLinePen, visualTreeNode.X, visualTreeNode.Y, visualTreeNode.Width, visualTreeNode.Height);
        } else {
          graphics.FillEllipse(nodeFillBrush, visualTreeNode.X, visualTreeNode.Y, visualTreeNode.Width, visualTreeNode.Height);
          graphics.DrawEllipse(nodeLinePen, visualTreeNode.X, visualTreeNode.Y, visualTreeNode.Width, visualTreeNode.Height);
        }

        //draw name of symbol
        var text = node.ToString();
        graphics.DrawString(text, textFont, textBrush, new RectangleF(visualTreeNode.X, visualTreeNode.Y, visualTreeNode.Width, visualTreeNode.Height), stringFormat);

        //draw connection line to parent node
        if (!connectionPoint.IsEmpty && node.Parent != null) {
          var visualLine = GetVisualSymbolicExpressionTreeNodeConnection(node.Parent, node);
          using (Pen linePen = new Pen(visualLine.LineColor)) {
            linePen.DashStyle = visualLine.DashStyle;
            graphics.DrawLine(linePen, connectionPoint, new Point(visualTreeNode.X + visualTreeNode.Width / 2, visualTreeNode.Y));
          }
        }

        //calculate areas for the subtrees according to their tree size and call drawFunctionTree
        Point connectFrom = new Point(visualTreeNode.X + visualTreeNode.Width / 2, visualTreeNode.Y + visualTreeNode.Height);
        int[] xBoundaries = new int[node.SubtreeCount + 1];
        xBoundaries[0] = x;
        for (int i = 0; i < node.SubtreeCount; i++) {
          xBoundaries[i + 1] = (int)(xBoundaries[i] + (width * (double)node.GetSubtree(i).GetLength()) / (node.GetLength() - 1));
          DrawFunctionTree(node.GetSubtree(i), graphics, xBoundaries[i], y + height, xBoundaries[i + 1] - xBoundaries[i], height, connectFrom);
        }
      }
    }

    protected void DrawTreeNode(VisualSymbolicExpressionTreeNode visualTreeNode) {
      using (var graphics = Graphics.FromImage(image)) {
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        DrawTreeNode(graphics, visualTreeNode);
      }
    }

    protected void DrawTreeNode(Graphics graphics, VisualSymbolicExpressionTreeNode visualTreeNode) {
      graphics.Clip = new Region(new Rectangle(visualTreeNode.X, visualTreeNode.Y, visualTreeNode.Width + 1, visualTreeNode.Height + 1));
      graphics.Clear(backgroundColor);
      var node = visualTreeNode.SymbolicExpressionTreeNode;
      using (var textBrush = new SolidBrush(visualTreeNode.TextColor))
      using (var nodeLinePen = new Pen(visualTreeNode.LineColor))
      using (var nodeFillBrush = new SolidBrush(visualTreeNode.FillColor)) {
        //draw terminal node
        if (node.SubtreeCount == 0) {
          graphics.FillRectangle(nodeFillBrush, visualTreeNode.X, visualTreeNode.Y, visualTreeNode.Width, visualTreeNode.Height);
          graphics.DrawRectangle(nodeLinePen, visualTreeNode.X, visualTreeNode.Y, visualTreeNode.Width, visualTreeNode.Height);
        } else {
          graphics.FillEllipse(nodeFillBrush, visualTreeNode.X, visualTreeNode.Y, visualTreeNode.Width, visualTreeNode.Height);
          graphics.DrawEllipse(nodeLinePen, visualTreeNode.X, visualTreeNode.Y, visualTreeNode.Width, visualTreeNode.Height);
        }
        //draw name of symbol
        var text = node.ToString();
        graphics.DrawString(text, textFont, textBrush, new RectangleF(visualTreeNode.X, visualTreeNode.Y, visualTreeNode.Width, visualTreeNode.Height), stringFormat);
      }
    }
    #endregion

    #region save image
    private void saveImageToolStripMenuItem_Click(object sender, EventArgs e) {
      if (saveFileDialog.ShowDialog() == DialogResult.OK) {
        string filename = saveFileDialog.FileName.ToLower();
        if (filename.EndsWith("bmp")) SaveImageAsBitmap(filename);
        else if (filename.EndsWith("emf")) SaveImageAsEmf(filename);
        else SaveImageAsBitmap(filename);
      }
    }

    private void SaveImageAsBitmap(string filename) {
      if (tree == null) return;
      Image image = new Bitmap(Width, Height);
      using (Graphics g = Graphics.FromImage(image)) {
        int height = this.Height / tree.Depth;
        DrawFunctionTree(tree, g, 0, 0, Width, height);
      }
      image.Save(filename);
    }

    private void SaveImageAsEmf(string filename) {
      if (tree == null) return;
      using (Graphics g = CreateGraphics()) {
        using (Metafile file = new Metafile(filename, g.GetHdc())) {
          using (Graphics emfFile = Graphics.FromImage(file)) {
            int height = this.Height / tree.Depth;
            DrawFunctionTree(tree, emfFile, 0, 0, Width, height);
          }
        }
        g.ReleaseHdc();
      }
    }
    #endregion
  }
}
