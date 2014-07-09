#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Views;
using View = HeuristicLab.MainForm.WindowsForms.View;

namespace HeuristicLab.DataPreprocessing.Views {
  public class DataPreprocessorStarter : IDataPreprocessorStarter {

    public void Start(IDataAnalysisProblemData problemData, View currentView) {
      IAlgorithm algorithm;
      IDataAnalysisProblem problem;
      IItem parentItem = GetMostOuterContent(currentView, out algorithm, out problem);
      var context = new PreprocessingContext(problemData, algorithm, problem);
      MainFormManager.MainForm.ShowContent(context);
    }

    private IItem GetMostOuterContent(Control control, out IAlgorithm algorithm, out IDataAnalysisProblem problem) {
      algorithm = null;
      problem = null;
      ItemView itemView = null;
      do {
        control = control.Parent;
        if (control is ItemView) {
          itemView = (ItemView)control;
          if (itemView.Content is IAlgorithm) {
            algorithm = (IAlgorithm)itemView.Content;
          }
          if (itemView.Content is IDataAnalysisProblem) {
            problem = (IDataAnalysisProblem)itemView.Content;
          }
        }
      } while (control != null);

      return itemView.Content;
    }
  }
}
