#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.MainForm.WindowsForms;
using View = HeuristicLab.MainForm.WindowsForms.View;

namespace HeuristicLab.Core.Views {
  public partial class BreadcrumbViewHost : ViewHost {
    private const string Separator = ">>";
    private const string Ellipsis = "...";

    private readonly Font font = new Font("Microsoft Sans Serif", 7f);
    private readonly List<Label> labels = new List<Label>();
    private readonly LinkedList<Tuple<string, IContent>> breadcrumbs = new LinkedList<Tuple<string, IContent>>();

    private Size requiredSize;
    private Point requiredLocation;
    private bool enableBreadcrumbs;
    private bool showSingle;

    [DefaultValue(false)]
    public bool EnableBreadcrumbs {
      get { return enableBreadcrumbs; }
      set {
        if (enableBreadcrumbs == value) return;
        enableBreadcrumbs = value;
        ConfigureViewLayout(ActiveView as ContentView);
      }
    }

    [DefaultValue(false)]
    public bool ShowSingle {
      get { return showSingle; }
      set {
        if (showSingle == value) return;
        showSingle = value;

        if (enableBreadcrumbs && !breadcrumbs.Any())
          AddBreadcrumbs(Content);

        ConfigureViewLayout(ActiveView as ContentView);
      }
    }

    public IEnumerable<IContent> Breadcrumbs { get { return breadcrumbs.Select(x => x.Item2); } }

    public BreadcrumbViewHost() {
      InitializeComponent();
    }

    #region Overrides
    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        breadcrumbs.Clear();
      } else {
        if (enableBreadcrumbs && showSingle)
          if (!breadcrumbs.Any())
            AddBreadcrumbs(Content);
          else if (breadcrumbs.Count == 1) {
            breadcrumbs.Clear();
            AddBreadcrumbs(Content);
          }
      }

      UpdateBreadcrumbTrail();
      ConfigureViewLayout(ActiveView as ContentView);
    }

    protected override void OnSizeChanged(EventArgs e) {
      UpdateBreadcrumbTrail();
      base.OnSizeChanged(e);
    }

    protected override void ConfigureViewLayout(View view) {
      if (view == null) return;

      base.ConfigureViewLayout(view);

      requiredSize = view.Size;
      requiredLocation = Point.Empty;

      if (enableBreadcrumbs && (breadcrumbs.Count > 1 || breadcrumbs.Count == 1 && showSingle)) {
        requiredSize.Height -= font.Height + 6;
        requiredLocation.Y += font.Height + 8;

        if (view.Dock == DockStyle.Fill)
          view.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
      }

      view.Size = requiredSize;
      view.Location = requiredLocation;
    }
    #endregion

    public void AddBreadcrumbs(params IContent[] crumbs) {
      foreach (var c in crumbs) {
        var item = c as INamedItem;
        string text = item != null ? item.Name : c != null ? c.ToString() : "null";
        AddBreadcrumb(text, c);
      }
    }

    public void AddBreadcrumb(string text, IContent content) {
      if (breadcrumbs.Any(x => x.Item2 == content)) return;
      breadcrumbs.AddLast(Tuple.Create(text, content));
    }

    #region Event Handlers
    private void label_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      var label = (LinkLabel)sender;
      var tuple = (Tuple<string, IContent>)label.Tag;

      while (breadcrumbs.Any() && breadcrumbs.Last.Value != tuple)
        breadcrumbs.RemoveLast();
      if (breadcrumbs.Count == 1 && !showSingle)
        breadcrumbs.RemoveLast();

      Content = tuple.Item2;
    }
    #endregion

    #region Helpers
    private void UpdateBreadcrumbTrail() {
      foreach (var l in labels)
        Controls.Remove(l);
      labels.Clear();

      ConfigureViewLayout(ActiveView as ContentView);

      if (!enableBreadcrumbs || !breadcrumbs.Any() || breadcrumbs.Count == 1 && !showSingle) return;

      if (!AddLabelsLeftToRight())
        AddLabelsRightToLeft();
    }

    private bool AddLabelsLeftToRight() {
      var curLoc = new Point(3, 0);
      int idx = 0;

      foreach (var bc in breadcrumbs) {
        Label label;

        if (bc == breadcrumbs.Last.Value) {
          label = new Label { Location = curLoc, AutoSize = true, Font = font, Text = bc.Item1 };
        } else {
          label = new LinkLabel { Location = curLoc, AutoSize = true, Font = font, Text = bc.Item1, Tag = bc };
          ((LinkLabel)label).LinkClicked += label_LinkClicked;
        }

        labels.Add(label);
        label.Size = label.GetPreferredSize(Size.Empty);
        curLoc.X += label.Size.Width;

        if (++idx < breadcrumbs.Count) {
          var separator = new Label { Location = curLoc, AutoSize = true, Font = font, Text = Separator };
          labels.Add(separator);
          separator.Size = separator.GetPreferredSize(Size.Empty);
          curLoc.X += separator.Size.Width;
        }
      }

      double width = Width;
      if (ViewsLabelVisible)
        width -= viewsLabel.Width + viewsLabel.Margin.Left + viewsLabel.Margin.Right;
      bool success = curLoc.X <= width;

      if (success)
        Controls.AddRange(labels.ToArray());

      return success;
    }

    private void AddLabelsRightToLeft() {
      if (!labels.Any()) return;

      var curLoc = new Point(3, 0);

      var ellipsis = new LinkLabel { Location = curLoc, AutoSize = true, Font = font, Text = Ellipsis };
      labels.Insert(0, ellipsis);
      Controls.Add(ellipsis);
      curLoc.X += ellipsis.Size.Width;

      double x = Width;
      if (ViewsLabelVisible)
        x -= viewsLabel.Width + viewsLabel.Margin.Left + viewsLabel.Margin.Right;

      int i = labels.Count - 1;

      while (i >= 1) {
        x -= labels[i].Size.Width;
        x -= labels[i - 1].Size.Width;
        if (x < curLoc.X) break;
        i -= 2;
      }

      ellipsis.Tag = breadcrumbs.ElementAt(i / 2);
      ellipsis.LinkClicked += label_LinkClicked;

      while (++i < labels.Count) {
        var l = labels[i];
        l.Location = curLoc;
        Controls.Add(l);
        curLoc.X += l.Size.Width;
      }
    }
    #endregion
  }
}