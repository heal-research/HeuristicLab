#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing.Drawing2D;
using System.Linq;
using HeuristicLab.Netron;
using Netron.Diagramming.Core;

namespace HeuristicLab.Operators.Views.GraphVisualization.Views {
  public class OperatorShape : ComplexShapeBase {
    private static int LABEL_HEIGHT = 16;
    private static int LABEL_WIDTH = 180;
    private static int LABEL_SPACING = 3;
    private int headerHeight = 30;

    private ExpandableIconMaterial expandIconMaterial;
    public OperatorShape()
      : base() {
      this.Resizable = false;
      this.additionalConnectors = new List<IConnector>();
      this.labels = new List<string>();
    }

    public override string EntityName {
      get { return "Operator Shape"; }
    }

    public bool Collapsed {
      get { return this.expandIconMaterial.Collapsed; }
      set {
        if (this.expandIconMaterial.Collapsed != value)
          this.expandIconMaterial.Collapsed = value;
      }
    }

    private Color lineColor;
    public Color LineColor {
      get { return this.lineColor; }
      set { this.lineColor = value; }
    }

    private float lineWidth;
    public float LineWidth {
      get { return this.lineWidth; }
      set { this.lineWidth = value; }
    }

    private Color color;
    public Color Color {
      get { return this.color; }
      set { this.color = value; }
    }

    private string title;
    public string Title {
      get { return title; }
      set { title = value; }
    }

    public string Subtitle { get; set; }

    private IconMaterial iconMaterial;
    public Bitmap Icon {
      get { return this.iconMaterial.Icon; }
      set {
        this.iconMaterial.Icon = value;
        this.iconMaterial.Transform(new Rectangle(new Point(Rectangle.X + 5, Rectangle.Y + 5), this.iconMaterial.Icon.Size));
      }
    }

    #region additional connectors
    private List<IConnector> additionalConnectors;
    public IEnumerable<string> AdditionalConnectorNames {
      get { return this.additionalConnectors.Select(c => c.Name); }
    }

    private IConnector predecessor;
    public IConnector Predecessor {
      get { return this.predecessor; }
    }

    private IConnector successor;
    public IConnector Successor {
      get { return this.successor; }
    }

    private IConnector CreateConnector(string connectorName, Point location) {
      Connector connector = new Connector(location, this.Model);
      connector.ConnectorStyle = ConnectorStyle.Square;
      connector.Parent = this;
      connector.Name = connectorName;
      return connector;
    }

    public void AddConnector(string connectorName) {
      IConnector connector = this.CreateConnector(connectorName, this.BottomRightCorner);

      this.additionalConnectors.Add(connector);
      this.Connectors.Add(connector);
      this.UpdateConnectorLocation();
    }

    public void RemoveConnector(string connectorName) {
      IConnector connector = this.additionalConnectors.Where(c => c.Name == connectorName).FirstOrDefault();
      if (connector != null) {
        this.additionalConnectors.Remove(connector);
        this.Connectors.Remove(connector);
        this.UpdateConnectorLocation();
      }
    }

    private void UpdateConnectorLocation() {
      if (this.additionalConnectors.Count == 0)
        return;

      int spacing = this.Rectangle.Width / this.additionalConnectors.Count;
      int margin = spacing / 2;
      int posX = margin + this.Rectangle.X;
      for (int i = 0; i < this.additionalConnectors.Count; i++) {
        this.additionalConnectors[i].MoveBy(new Point(posX - this.additionalConnectors[i].Point.X, 0));
        posX += spacing;
      }
    }
    #endregion

    #region label material
    private List<string> labels;
    public IEnumerable<string> Labels {
      get { return this.labels; }
    }

    public void UpdateLabels(IEnumerable<string> labels) {
      this.labels = new List<string>(labels);
      this.expandIconMaterial.Visible = this.labels.Count != 0;
      this.UpdateLabels();
    }
    #endregion

    private void expandIconMaterial_OnExpand(object sender, EventArgs e) {
      this.UpdateLabels();
    }

    private void expandIconMaterial_OnCollapse(object sender, EventArgs e) {
      this.UpdateLabels();
    }

    private Size CalculateSize() {
      int width = this.Rectangle.Width;
      int height = headerHeight;
      if (!Collapsed)
        height += this.labels.Count * (LABEL_HEIGHT + LABEL_SPACING);
      return new Size(width, height);
    }

    private void UpdateLabels() {
      Size newSize = CalculateSize();
      if (this.Rectangle.Size != newSize) {
        foreach (IConnector connector in this.additionalConnectors)
          connector.MoveBy(new Point(0, newSize.Height - this.Rectangle.Height));
        this.mRectangle = new Rectangle(this.Rectangle.Location, newSize);
        this.Invalidate();
        this.RaiseOnChange(this, new EntityEventArgs(this));
      }
    }

    protected override void Initialize() {
      base.Initialize();

      //the initial size
      this.Transform(0, 0, 200, headerHeight);
      this.color = Color.LightBlue;

      this.iconMaterial = new IconMaterial();
      this.iconMaterial.Gliding = false;
      this.Children.Add(iconMaterial);

      Bitmap expandBitmap = new Bitmap(HeuristicLab.Common.Resources.VSImageLibrary.Expand);
      Bitmap collapseBitmap = new Bitmap(HeuristicLab.Common.Resources.VSImageLibrary.Collapse);
      this.expandIconMaterial = new ExpandableIconMaterial(expandBitmap, collapseBitmap);
      this.expandIconMaterial.Gliding = false;
      this.expandIconMaterial.Transform(new Rectangle(new Point(Rectangle.Right - 20, Rectangle.Y + 7), expandIconMaterial.Icon.Size));
      this.expandIconMaterial.Visible = false;
      this.expandIconMaterial.OnExpand += new EventHandler(expandIconMaterial_OnExpand);
      this.expandIconMaterial.OnCollapse += new EventHandler(expandIconMaterial_OnCollapse);
      this.Children.Add(expandIconMaterial);

      this.predecessor = this.CreateConnector(OperatorShapeInfoFactory.PredecessorConnector, new Point(Rectangle.Left, Center.Y));
      this.Connectors.Add(predecessor);

      this.successor = this.CreateConnector(OperatorShapeInfoFactory.SuccessorConnector, (new Point(Rectangle.Right, Center.Y)));
      this.Connectors.Add(successor);
    }

    public override void Paint(Graphics g) {
      base.Paint(g);

      g.SmoothingMode = SmoothingMode.HighQuality;

      using (Pen pen = new Pen(lineColor, lineWidth)) {

        SizeF titleSize = g.MeasureString(this.Title, ArtPalette.DefaultBoldFont, Rectangle.Width - 45);
        titleSize.Height += 10; //add spacing
        SizeF subtitleSize = g.MeasureString(this.Subtitle, ArtPalette.DefaultFont, Rectangle.Width - 45);
        subtitleSize.Height += 5; //add spacing
        if (this.Title == this.Subtitle || string.IsNullOrEmpty(this.Subtitle)) subtitleSize = new SizeF(0, 0);

        if ((int)titleSize.Height + (int)subtitleSize.Height != Rectangle.Height) {
          headerHeight = (int)titleSize.Height + (int)subtitleSize.Height;
          this.UpdateLabels();
        }

        GraphicsPath path = new GraphicsPath();
        path.AddArc(Rectangle.X, Rectangle.Y, 20, 20, -180, 90);
        path.AddLine(Rectangle.X + 10, Rectangle.Y, Rectangle.X + Rectangle.Width - 10, Rectangle.Y);
        path.AddArc(Rectangle.X + Rectangle.Width - 20, Rectangle.Y, 20, 20, -90, 90);
        path.AddLine(Rectangle.X + Rectangle.Width, Rectangle.Y + 10, Rectangle.X + Rectangle.Width, Rectangle.Y + Rectangle.Height - 10);
        path.AddArc(Rectangle.X + Rectangle.Width - 20, Rectangle.Y + Rectangle.Height - 20, 20, 20, 0, 90);
        path.AddLine(Rectangle.X + Rectangle.Width - 10, Rectangle.Y + Rectangle.Height, Rectangle.X + 10, Rectangle.Y + Rectangle.Height);
        path.AddArc(Rectangle.X, Rectangle.Y + Rectangle.Height - 20, 20, 20, 90, 90);
        path.AddLine(Rectangle.X, Rectangle.Y + Rectangle.Height - 10, Rectangle.X, Rectangle.Y + 10);
        //shadow
        if (ArtPalette.EnableShadows) {
          Region darkRegion = new Region(path);
          darkRegion.Translate(5, 5);
          g.FillRegion(ArtPalette.ShadowBrush, darkRegion);
        }
        //background
        g.FillPath(Brush, path);

        using (LinearGradientBrush gradientBrush = new LinearGradientBrush(Rectangle.Location, new Point(Rectangle.X + Rectangle.Width, Rectangle.Y), this.Color, Color.White)) {
          Region gradientRegion = new Region(path);
          g.FillRegion(gradientBrush, gradientRegion);
        }

        if (!this.Collapsed) {
          TextStyle textStyle = new TextStyle(Color.Black, new Font("Arial", 7), StringAlignment.Near, StringAlignment.Near);
          StringFormat stringFormat = textStyle.StringFormat;
          stringFormat.Trimming = StringTrimming.EllipsisWord;
          stringFormat.FormatFlags = StringFormatFlags.LineLimit;
          Rectangle rect;

          const int verticalHeaderSpacing = 5;
          Point separationLineStart = new Point(Rectangle.X + 25, Rectangle.Y + headerHeight - verticalHeaderSpacing);
          Point separationLineEnd = new Point(Rectangle.X + Rectangle.Width - 25, Rectangle.Y + headerHeight - verticalHeaderSpacing);
          using (LinearGradientBrush brush = new LinearGradientBrush(separationLineStart, separationLineEnd, Color.Black, Color.White)) {
            using (Pen separationLinePen = new Pen(brush)) {
              g.DrawLine(separationLinePen, separationLineStart, separationLineEnd);
            }
          }


          for (int i = 0; i < this.labels.Count; i++) {
            rect = new Rectangle(Rectangle.X + 25, Rectangle.Y + headerHeight + i * (LABEL_HEIGHT + LABEL_SPACING), LABEL_WIDTH, LABEL_HEIGHT);
            g.DrawString(textStyle.GetFormattedText(this.labels[i]), textStyle.Font, textStyle.GetBrush(), rect, stringFormat);
          }
        }

        //the border
        g.DrawPath(pen, path);

        //the title
        g.DrawString(this.Title, ArtPalette.DefaultBoldFont, Brushes.Black,
                     new Rectangle(Rectangle.X + 25, Rectangle.Y + 5,
                                   Rectangle.Width - 45, Rectangle.Height - 5 - (int)subtitleSize.Height));

        //the subtitle
        if (this.Title != this.Subtitle || string.IsNullOrEmpty(this.Subtitle)) {
          g.DrawString(this.Subtitle, ArtPalette.DefaultFont, Brushes.Black,
                       new Rectangle(Rectangle.X + 25, Rectangle.Y + (int)titleSize.Height -5 ,
                                     Rectangle.Width - 45, Rectangle.Height - 5));
        }


        //the material
        foreach (IShapeMaterial material in Children)
          material.Paint(g);

        //the connectors
        if (this.ShowConnectors) {
          foreach (IConnector t in Connectors)
            t.Paint(g);
        }
      }
    }
  }
}
