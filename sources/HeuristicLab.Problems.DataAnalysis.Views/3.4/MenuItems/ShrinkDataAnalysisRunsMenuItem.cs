#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using HeuristicLab.Optimizer;

using MenuItem = HeuristicLab.MainForm.WindowsForms.MenuItem;
using ValuesType = System.Collections.Generic.Dictionary<string, System.Collections.IList>;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  internal sealed class ShrinkDataAnalysisRunsMenuItem : MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "Remove Duplicate Datasets"; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&Edit", "&Data Analysis" }; }
    }
    public override int Position {
      get { return 5300; }
    }
    public override string ToolTipText {
      get { return "This command shrinks the memory usage of data analysis optimizers by checking and unifying duplicate datasets."; }
    }

    protected override void OnToolStripItemSet(EventArgs e) {
      ToolStripItem.Enabled = false;
    }

    protected override void OnActiveViewChanged(object sender, EventArgs e) {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      if (activeView == null) {
        ToolStripItem.Enabled = false;
        return;
      }

      var content = activeView.Content;
      RunCollection runCollection = content as RunCollection;
      if (runCollection == null && content is IOptimizer)
        runCollection = ((IOptimizer)content).Runs;

      if (runCollection == null) {
        ToolStripItem.Enabled = false;
        return;
      }

      ToolStripItem.Enabled = runCollection.Any(run => run.Parameters.Any(p => p.Value is IDataAnalysisProblemData));
    }

    public override void Execute() {
      IContentView activeView = (IContentView)MainFormManager.MainForm.ActiveView;
      var mainForm = (MainForm.WindowsForms.MainForm)MainFormManager.MainForm;
      mainForm.AddOperationProgressToContent(activeView.Content, "Removing duplicate datasets.");

      Action<IContentView> action = (view) => {
        var variableValuesMapping = new Dictionary<ValuesType, ValuesType>();
        foreach (var problemData in view.Content.GetObjectGraphObjects(excludeStaticMembers: true).OfType<IDataAnalysisProblemData>()) {
          var originalValues = variableValuesGetter(problemData.Dataset);
          var matchingValues = GetEqualValues(originalValues, variableValuesMapping);
          variableValuesSetter(problemData.Dataset, matchingValues);
        }
      };

      action.BeginInvoke(activeView, delegate(IAsyncResult result) {
        action.EndInvoke(result);
        mainForm.RemoveOperationProgressFromContent(activeView.Content);
      }, null);
    }

    private static ValuesType GetEqualValues(ValuesType originalValues, Dictionary<ValuesType, ValuesType> variableValuesMapping) {
      if (variableValuesMapping.ContainsKey(originalValues)) return variableValuesMapping[originalValues];

      var matchingValues = variableValuesMapping.FirstOrDefault(kv => kv.Key == kv.Value && EqualVariableValues(originalValues, kv.Key)).Key ?? originalValues;
      variableValuesMapping[originalValues] = matchingValues;
      return matchingValues;
    }

    private static bool EqualVariableValues(ValuesType values1, ValuesType values2) {
      //compare variable names for equality
      if (!values1.Keys.SequenceEqual(values2.Keys)) return false;
      foreach (var key in values1.Keys) {
        var v1 = values1[key];
        var v2 = values2[key];
        if (v1.Count != v2.Count) return false;
        for (int i = 0; i < v1.Count; i++) {
          if (!v1[i].Equals(v2[i])) return false;
        }
      }
      return true;
    }

    private static readonly Action<Dataset, Dictionary<string, IList>> variableValuesSetter;
    private static readonly Func<Dataset, Dictionary<string, IList>> variableValuesGetter;
    /// <summary>
    /// The static initializer is used to create expressions for getting and setting the private variableValues field in the dataset.
    /// This is done by expressions because the field is private and compiled expression calls are much faster compared to standad reflection calls.
    /// </summary>
    static ShrinkDataAnalysisRunsMenuItem() {
      var dataset = Expression.Parameter(typeof(Dataset));
      var variableValues = Expression.Parameter(typeof(ValuesType));
      var valuesExpression = Expression.Field(dataset, "variableValues");
      var assignExpression = Expression.Assign(valuesExpression, variableValues);

      var variableValuesSetExpression = Expression.Lambda<Action<Dataset, ValuesType>>(assignExpression, dataset, variableValues);
      variableValuesSetter = variableValuesSetExpression.Compile();

      var variableValuesGetExpression = Expression.Lambda<Func<Dataset, ValuesType>>(valuesExpression, dataset);
      variableValuesGetter = variableValuesGetExpression.Compile();
    }
  }
}
