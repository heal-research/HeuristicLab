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
    private Dictionary<ISymbolicExpressionTreeNode, VisualTreeNode<ISymbolicExpressionTreeNode>> visualTreeNodes;
    private Dictionary<Tuple<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode>, VisualTreeNodeConnection> visualLines;
    private ILayoutEngine<ISymbolicExpressionTreeNode> layoutEngine;

    private const int preferredNodeWidth = 70;
    private const int preferredNodeHeight = 46;
    private const int minHorizontalDistance = 30;
    private const int minVerticalDistance = 30;

    public SymbolicExpressionTreeChart() {
      InitializeComponent();
      this.image = new Bitmap(Width, Height);
      this.stringFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
      this.spacing = 5;
      this.lineColor = Color.Black;
      this.backgroundColor = Color.White;
      this.textFont = new Font(FontFamily.GenericSansSerif, 12);
      layoutEngine = new ReingoldTilfordLayoutEngine<ISymbolicExpressionTreeNode> {
        NodeWidth = preferredNodeWidth,
        NodeHeight = preferredNodeHeight,
        HorizontalSpacing = minHorizontalDistance,
        VerticalSpacing = minVerticalDistance
      };
    }

    private ILayoutEngine<ISymbolicExpressionTreeNode> TreeLayoutEngine {
      get { return layoutEngine; }
      set {
        layoutEngine = value;
        InitializeLayout();
        Repaint();
      }
    }

    public SymbolicExpressionTreeChart(ISymbolicExpressionTree tree)
      : this() {
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
        if (tree != null) {
          //the layout engine needs to be initialized here so that the visualNodes and the visualLines dictionaries are populated 
          InitializeLayout();
          Repaint();
        }
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
      else {
        this.image = new Bitmap(Width, Height);
      }
      this.Repaint();
    }

    public event EventHandler Repainted;//expose this event to notify the parent control that the tree was repainted
    protected virtual void OnRepaint(object sender, EventArgs e) {
      var repainted = Repainted;
      if (repainted != null) {
        repainted(sender, e);
      }
    }

    public void Repaint() {
      if (!suspendRepaint) {
        this.GenerateImage();
        this.Refresh();
        OnRepaint(this, EventArgs.Empty);
      }
    }

    public void RepaintNodes() {
      if (!suspendRepaint) {
        using (var graphics = Graphics.FromImage(image)) {
          graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
          graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
          foreach (var visualNode in visualTreeNodes.Values) {
            DrawTreeNode(graphics, visualNode);
            if (visualNode.Content.SubtreeCount > 0) {
              foreach (var visualSubtree in visualNode.Content.Subtrees.Select(s => visualTreeNodes[s])) {
                DrawLine(graphics, visualNode, visualSubtree);
              }
            }
          }
        }
        this.Refresh();
      }
    }

    public void RepaintNode(VisualTreeNode<ISymbolicExpressionTreeNode> visualNode) {
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
          DrawFunctionTree(graphics, preferredNodeWidth, preferredNodeHeight, minHorizontalDistance, minVerticalDistance);
        }
      }
    }

    public VisualTreeNode<ISymbolicExpressionTreeNode> GetVisualSymbolicExpressionTreeNode(ISymbolicExpressionTreeNode symbolicExpressionTreeNode) {
      if (visualTreeNodes.ContainsKey(symbolicExpressionTreeNode))
        return visualTreeNodes[symbolicExpressionTreeNode];
      return null;
    }

    public VisualTreeNodeConnection GetVisualSymbolicExpressionTreeNodeConnection(ISymbolicExpressionTreeNode parent, ISymbolicExpressionTreeNode child) {
      if (child.Parent != parent) throw new ArgumentException();
      var key = Tuple.Create(parent, child);
      VisualTreeNodeConnection connection = null;
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
      var visualTreeNode = FindVisualSymbolicExpressionTreeNodeAt(e.X, e.Y);
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
      VisualTreeNode<ISymbolicExpressionTreeNode> visualTreeNode = FindVisualSymbolicExpressionTreeNodeAt(e.X, e.Y);
      if (visualTreeNode != null)
        OnSymbolicExpressionTreeNodeDoubleClicked(visualTreeNode, e);
    }

    public event ItemDragEventHandler SymbolicExpressionTreeNodeDrag;
    protected virtual void OnSymbolicExpressionTreeNodeDragDrag(object sender, ItemDragEventArgs e) {
      var dragged = SymbolicExpressionTreeNodeDrag;
      if (dragged != null)
        dragged(sender, e);
    }

    private VisualTreeNode<ISymbolicExpressionTreeNode> draggedSymbolicExpressionTree;
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
      VisualTreeNode<ISymbolicExpressionTreeNode> visualTreeNode = FindVisualSymbolicExpressionTreeNodeAt(e.X, e.Y);
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

    public VisualTreeNode<ISymbolicExpressionTreeNode> FindVisualSymbolicExpressionTreeNodeAt(int x, int y) {
      foreach (var visualTreeNode in visualTreeNodes.Values) {
        if (x >= visualTreeNode.X && x <= visualTreeNode.X + visualTreeNode.Width &&
            y >= visualTreeNode.Y && y <= visualTreeNode.Y + visualTreeNode.Height)
          return visualTreeNode;
      }
      return null;
    }
    #endregion

    #region initialize the layout
    private void InitializeLayout() {
      var actualRoot = tree.Root;
      if (actualRoot.Symbol is ProgramRootSymbol && actualRoot.SubtreeCount == 1) {
        actualRoot = tree.Root.GetSubtree(0);
      }
      layoutEngine.Initialize(actualRoot, n => n.Subtrees, n => n.GetLength(), n => n.GetDepth());
      layoutEngine.CalculateLayout(this.Width, this.Height);
      UpdateDictionaries();
    }

    private void UpdateDictionaries() {
      var visualNodes = layoutEngine.GetVisualNodes().ToList();
      //populate the visual nodes and visual connections dictionaries
      visualTreeNodes = visualNodes.ToDictionary(x => x.Content, x => x);
      visualLines = new Dictionary<Tuple<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode>, VisualTreeNodeConnection>();
      foreach (var node in visualNodes.Select(n => n.Content)) {
        foreach (var subtree in node.Subtrees) {
          visualLines.Add(new Tuple<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode>(node, subtree), new VisualTreeNodeConnection());
        }
      }
    }
    #endregion
    #region methods for painting the symbolic expression tree
    private void DrawFunctionTree(Graphics graphics, int preferredWidth, int preferredHeight, int minHDistance, int minVDistance) {
      //we assume here that the layout has already been initialized when the symbolic expression tree was changed
      //recalculate layout according to new node widths and spacing
      layoutEngine.NodeWidth = preferredWidth;
      layoutEngine.NodeHeight = preferredHeight;
      layoutEngine.HorizontalSpacing = minHDistance;
      layoutEngine.VerticalSpacing = minVDistance;
      layoutEngine.CalculateLayout(Width, Height);
      UpdateDictionaries();//this will reset the visual nodes, therefore colors will be reset to default values
      var visualNodes = visualTreeNodes.Values;
      //draw nodes and connections
      foreach (var visualNode in visualNodes) {
        DrawTreeNode(visualNode);
        var node = visualNode.Content;
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

    protected void DrawTreeNode(VisualTreeNode<ISymbolicExpressionTreeNode> visualTreeNode) {
      using (var graphics = Graphics.FromImage(image)) {
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        DrawTreeNode(graphics, visualTreeNode);
      }
    }

    protected void DrawTreeNode(Graphics graphics, VisualTreeNode<ISymbolicExpressionTreeNode> visualTreeNode) {
      graphics.Clip = new Region(new Rectangle(visualTreeNode.X, visualTreeNode.Y, visualTreeNode.Width + 1, visualTreeNode.Height + 1));
      graphics.Clear(backgroundColor);
      var node = visualTreeNode.Content;
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

    protected void DrawLine(Graphics graphics, VisualTreeNode<ISymbolicExpressionTreeNode> startNode, VisualTreeNode<ISymbolicExpressionTreeNode> endNode) {
      var origin = new Point(startNode.X + startNode.Width / 2, startNode.Y + startNode.Height);
      var target = new Point(endNode.X + endNode.Width / 2, endNode.Y);
      graphics.Clip = new Region(new Rectangle(Math.Min(origin.X, target.X), origin.Y, Math.Max(origin.X, target.X), target.Y));
      var visualLine = GetVisualSymbolicExpressionTreeNodeConnection(startNode.Content, endNode.Content);
      using (var linePen = new Pen(visualLine.LineColor)) {
        linePen.DashStyle = visualLine.DashStyle;
        graphics.DrawLine(linePen, origin, target);
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
        DrawFunctionTree(g, preferredNodeWidth, preferredNodeHeight, minHorizontalDistance, minVerticalDistance);
      }
      image.Save(filename);
    }

    public void SaveImageAsEmf(string filename) {
      if (tree == null) return;
      using (Graphics g = CreateGraphics()) {
        using (Metafile file = new Metafile(filename, g.GetHdc())) {
          using (Graphics emfFile = Graphics.FromImage(file)) {
            DrawFunctionTree(emfFile, preferredNodeWidth, preferredNodeHeight, minHorizontalDistance, minVerticalDistance);
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

    private void reingoldTilfordToolStripMenuItem_Click(object sender, EventArgs e) {
      TreeLayoutEngine = new ReingoldTilfordLayoutEngine<ISymbolicExpressionTreeNode> {
        NodeWidth = preferredNodeWidth,
        NodeHeight = preferredNodeHeight,
        HorizontalSpacing = minHorizontalDistance,
        VerticalSpacing = minVerticalDistance
      };
    }

    private void boxesToolStripMenuItem_Click(object sender, EventArgs e) {
      TreeLayoutEngine = new BoxesLayoutEngine<ISymbolicExpressionTreeNode> {
        NodeWidth = preferredNodeWidth,
        NodeHeight = preferredNodeHeight,
        HorizontalSpacing = minHorizontalDistance,
        VerticalSpacing = minVerticalDistance
      };
    }
  }
}
