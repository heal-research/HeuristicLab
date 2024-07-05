﻿#region License Information
// CShell, A Simple C# Scripting IDE
// Copyright (C) 2013  Arnova Asset Management Ltd., Lukas Buhler
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

// This file is based on code from the SharpDevelop project:
//   Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \Doc\sharpdevelop-copyright.txt)
//   This code is distributed under the GNU LGPL (for details please see \Doc\COPYING.LESSER.txt)

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
using System.Diagnostics;
using System.Linq;
//using ICSharpCode.NRefactory.Completion;
//using ICSharpCode.NRefactory.CSharp.Completion;
//using ICSharpCode.NRefactory.Editor;

namespace HeuristicLab.CodeEditor {
  internal class CSharpOverloadProvider /*: IUpdatableOverloadProvider, IParameterDataProvider*/ {
    /*
    private readonly CSharpCodeCompletionContext context;
    private readonly int startOffset;
    internal readonly IList<CSharpInsightItem> items;
    private int selectedIndex;

    public CSharpOverloadProvider(CSharpCodeCompletionContext context, int startOffset, IEnumerable<CSharpInsightItem> items) {
      Debug.Assert(items != null);
      this.context = context;
      this.startOffset = startOffset;
      this.selectedIndex = 0;
      this.items = items.ToList();

      Update(context);
    }

    public bool RequestClose { get; set; }

    public int Count {
      get { return items.Count; }
    }

    public object CurrentContent {
      get { return items[selectedIndex].Content; }
    }

    public object CurrentHeader {
      get { return items[selectedIndex].Header; }
    }

    public string CurrentIndexText {
      get { return (selectedIndex + 1).ToString() + " of " + this.Count.ToString(); }
    }

    public int SelectedIndex {
      get { return selectedIndex; }
      set {
        selectedIndex = value;
        if (selectedIndex >= items.Count)
          selectedIndex = items.Count - 1;
        if (selectedIndex < 0)
          selectedIndex = 0;
        OnPropertyChanged("SelectedIndex");
        OnPropertyChanged("CurrentIndexText");
        OnPropertyChanged("CurrentHeader");
        OnPropertyChanged("CurrentContent");
      }
    }

    public void Update(IDocument document, int offset) {
      var completionContext = new CSharpCodeCompletionContext(document, offset, context.ProjectContent);
      Update(completionContext);
    }

    public void Update(CSharpCodeCompletionContext completionContext) {
      var completionFactory = new CSharpCodeCompletionDataFactory(completionContext);
      var pce = new CSharpParameterCompletionEngine(
          completionContext.Document,
          completionContext.CompletionContextProvider,
          completionFactory,
          completionContext.ProjectContent,
          completionContext.TypeResolveContextAtCaret
      );

      int parameterIndex = pce.GetCurrentParameterIndex(startOffset, completionContext.Offset);
      if (parameterIndex < 0 || !items.Any()) {
        RequestClose = true;
        return;
      } else {
        if (parameterIndex > items[selectedIndex].Method.Parameters.Count) {
          var newItem = items.FirstOrDefault(i => parameterIndex <= i.Method.Parameters.Count);
          SelectedIndex = items.IndexOf(newItem);
        }
        if (parameterIndex > 0)
          parameterIndex--; // NR returns 1-based parameter index
        foreach (var item in items) {
          item.HighlightParameter(parameterIndex);
        }
      }
    }

    #region IParameterDataProvider implementation
    int IParameterDataProvider.StartOffset {
      get { return startOffset; }
    }

    string IParameterDataProvider.GetHeading(int overload, string[] parameterDescription, int currentParameter) {
      throw new NotImplementedException();
    }

    string IParameterDataProvider.GetDescription(int overload, int currentParameter) {
      throw new NotImplementedException();
    }

    string IParameterDataProvider.GetParameterDescription(int overload, int paramIndex) {
      throw new NotImplementedException();
    }

    string IParameterDataProvider.GetParameterName(int overload, int currentParameter) {
      throw new NotImplementedException();
    }

    int IParameterDataProvider.GetParameterCount(int overload) {
      throw new NotImplementedException();
    }

    bool IParameterDataProvider.AllowParameterList(int overload) {
      throw new NotImplementedException();
    }
    #endregion

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName) {
      var args = new PropertyChangedEventArgs(propertyName);
      if (PropertyChanged != null)
        PropertyChanged(this, args);
    }
    */
  }
}
