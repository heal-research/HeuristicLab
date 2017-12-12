#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  [View("Mathematical Representation")]
  [Content(typeof(ISymbolicDataAnalysisModel))]
  public partial class SymbolicDataAnalysisModelMathView : AsynchronousContentView {
    protected readonly SymbolicDataAnalysisExpressionLatexFormatter Formatter = new SymbolicDataAnalysisExpressionLatexFormatter();
    public SymbolicDataAnalysisModelMathView()
      : base() {
      InitializeComponent();
      webBrowser.ScrollBarsEnabled = true;
      webBrowser.ScriptErrorsSuppressed = false;

      // update for the first time after page has loaded
      webBrowser.DocumentCompleted += (sender, args) => RefreshHtmlPage();

      string hlDir = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
      webBrowser.Navigate("file://" + Path.Combine(hlDir, "displayModelFrame.html"));
    }

    public new ISymbolicDataAnalysisModel Content {
      get { return (ISymbolicDataAnalysisModel)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      RefreshHtmlPage();
    }

    private void RefreshHtmlPage() {
      // only update if the page has loaded already
      if (webBrowser.Document == null || webBrowser.Document.Body == null) return;

      if (Content != null) {
        HtmlElement newElement = webBrowser.Document.GetElementById("model");
        newElement.InnerText = GetFormattedTree();
        webBrowser.Document.InvokeScript("refreshModel");
      } else {
        HtmlElement newElement = webBrowser.Document.GetElementById("model");
        newElement.InnerText = string.Empty;
      }
    }

    protected virtual string GetFormattedTree() {
      return Formatter.Format(Content.SymbolicExpressionTree);
    }
  }
}
