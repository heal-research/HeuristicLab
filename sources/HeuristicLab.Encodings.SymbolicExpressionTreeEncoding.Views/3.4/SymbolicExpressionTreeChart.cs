#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Point = System.Drawing.Point;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  public partial class SymbolicExpressionTreeChart : UserControl {
    private Image image;
    private readonly StringFormat stringFormat;
    private Dictionary<ISymbolicExpressionTreeNode, VisualSymbolicExpressionTreeNode> visualTreeNodes;
    private Dictionary<Tuple<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode>, VisualSymbolicExpressionTreeNodeConnection> visualLines;
    private readonly ReingoldTilfordLayoutEngine<ISymbolicExpressionTreeNode> layoutEngine;
    private readonly SymbolicExpressionTreeLayoutAdapter layoutAdapter;

    private const int preferredNodeWidth = 70;
    private const int preferredNodeHeight = 46;
    private const int minHorizontalDistance = 20;
    private const int minVerticalDistance = 20;


    public SymbolicExpressionTreeChart() {
      InitializeComponent();
      this.image = new Bitmap(Width, Height);
      this.stringFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
      this.spacing = 5;
      this.lineColor = Color.Black;
      this.backgroundColor = Color.White;
      this.textFont = new Font(FontFamily.GenericSansSerif, 12);
      layoutEngine = new ReingoldTilfordLayoutEngine<ISymbolicExpressionTreeNode>();
      layoutAdapter = new SymbolicExpressionTreeLayoutAdapter();
    }

    public SymbolicExpressionTreeChart(ISymbolicExpressionTree tree)
      : this() {
      layoutEngine = new ReingoldTilfordLayoutEngine<ISymbolicExpressionTreeNode>();
      layoutAdapter = new SymbolicExpressionTreeLayoutAdapter();
      this.Tree = tree;
    }

    #region Public properties
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
          IEnumerable<ISymbolicExpressionTreeNode> nodes;
          if (tree.Root.SubtreeCount == 1) nodes = tree.Root.GetSubtree(0).IterateNodesPrefix();
          else nodes = tree.Root.IterateNodesPrefix();
          foreach (ISymbolicExpressionTreeNode node in nodes) {
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
    #endregion

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
          DrawFunctionTree(tree, graphics, preferredNodeWidth, preferredNodeHeight, minHorizontalDistance, minVerticalDistance);
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

    protected virtual void SymbolicExpressionTreeChart_MouseClick(object sender, MouseEventArgs e) {
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

    protected virtual void SymbolicExpressionTreeChart_MouseDoubleClick(object sender, MouseEventArgs e) {
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
    private void DrawFunctionTree(ISymbolicExpressionTree symbolicExpressionTree, Graphics graphics, int preferredWidth, int preferredHeight, int minHDistance, int minVDistance) {      
      var layoutNodes = layoutAdapter.Convert(symbolicExpressionTree).ToList();
      if(symbolicExpressionTree.Root.SubtreeCount==1) layoutNodes.RemoveAt(0);
      layoutEngine.Reset();
      layoutEngine.Root = layoutNodes[0];
      layoutEngine.AddNodes(layoutNodes);
      layoutEngine.MinHorizontalSpacing = (preferredWidth + minHDistance);
      layoutEngine.MinVerticalSpacing = (preferredHeight + minVDistance);
      layoutEngine.CalculateLayout();
      var bounds = layoutEngine.Bounds();

      double verticalScalingFactor = 1.0;
      double layoutHeight = (bounds.Height + preferredHeight);
      if (this.Height < layoutHeight)
        verticalScalingFactor = this.Height / layoutHeight;

      double horizontalScalingFactor = 1.0;
      double layoutWidth = (bounds.Width + preferredWidth);
      if (this.Width < layoutWidth)
        horizontalScalingFactor = this.Width / layoutWidth;

      double horizontalOffset;
      if (this.Width > layoutWidth)
        horizontalOffset = (this.Width - layoutWidth) / 2.0;
      else
        horizontalOffset = preferredWidth / 2.0;

      var levels = layoutNodes.GroupBy(n => n.Level, n => n).ToList();
      for (int i = levels.Count - 1; i >= 0; --i) {
        var nodes = levels[i].ToList();

        double minSpacing = double.MaxValue;
        if (nodes.Count > 1) {
          for (int j = 1; j < nodes.Count() - 1; ++j) {
            var distance = nodes[j].X - nodes[j - 1].X; // guaranteed to be > 0
            if (minSpacing > distance) minSpacing = distance;
          }
        }
        minSpacing *= horizontalScalingFactor;

        int minWidth = (int)Math.Round(preferredWidth * horizontalScalingFactor);
        int width = (int)Math.Min(minSpacing, preferredWidth) - 2; // leave some pixels so that node margins don't overlap

        foreach (var layoutNode in nodes) {
          var visualNode = visualTreeNodes[layoutNode.Content];
          visualNode.Width = width;
          visualNode.Height = (int)Math.Round(preferredHeight * verticalScalingFactor);
          visualNode.X = (int)Math.Round((layoutNode.X + horizontalOffset) * horizontalScalingFactor + (minWidth - width) / 2.0);
          visualNode.Y = (int)Math.Round(layoutNode.Y * verticalScalingFactor);
          DrawTreeNode(graphics, visualNode);
        }
      }
      // draw node connections
      foreach (var visualNode in visualTreeNodes.Values) {
        var node = visualNode.SymbolicExpressionTreeNode;
        foreach (var subtree in node.Subtrees) {
          var visualLine = GetVisualSymbolicExpressionTreeNodeConnection(node, subtree);
          var visualSubtree = visualTreeNodes[subtree];
          var origin = new Point(visualNode.X + visualNode.Width / 2, visualNode.Y + visualNode.Height);
          var target = new Point(visualSubtree.X + visualSubtree.Width / 2, visualSubtree.Y);
          graphics.Clip = new Region(new Rectangle(Math.Min(origin.X, target.X), origin.Y, Math.Max(origin.X, target.X), target.Y));
          using (var linePen = new Pen(visualLine.LineColor)) {
            linePen.DashStyle = visualLine.DashStyle;
            graphics.DrawLine(linePen, origin, target);
          }
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

    public void SaveImageAsBitmap(string filename) {
      if (tree == null) return;
      Image image = new Bitmap(Width, Height);
      using (Graphics g = Graphics.FromImage(image)) {
        DrawFunctionTree(tree, g, preferredNodeWidth, preferredNodeHeight, minHorizontalDistance, minVerticalDistance);
      }
      image.Save(filename);
    }

    public void SaveImageAsEmf(string filename) {
      if (tree == null) return;
      using (Graphics g = CreateGraphics()) {
        using (Metafile file = new Metafile(filename, g.GetHdc())) {
          using (Graphics emfFile = Graphics.FromImage(file)) {
            DrawFunctionTree(tree, emfFile, preferredNodeWidth, preferredNodeHeight, minHorizontalDistance, minVerticalDistance);
          }
        }
        g.ReleaseHdc();
      }
    }
    #endregion
    #region export pgf/tikz
    private void exportLatexToolStripMenuItem_Click(object sender, EventArgs e) {
      using (var dialog = new SaveFileDialog { Filter = "Tex (*.tex)|*.tex" }) {
        if (dialog.ShowDialog() != DialogResult.OK) return;
        string filename = dialog.FileName.ToLower();
        var formatter = new SymbolicExpressionTreeLatexFormatter();
        File.WriteAllText(filename, formatter.Format(Tree));
      }
    }
    #endregion
  }
}
