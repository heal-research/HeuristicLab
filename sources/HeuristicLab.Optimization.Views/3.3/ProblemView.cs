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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.Instances.Views;

namespace HeuristicLab.Optimization.Views {
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [View("Problem View")]
  [Content(typeof(IProblem), true)]
  public partial class ProblemView : ParameterizedNamedItemView {

    private static Type neededViewType = typeof(ProblemInstanceConsumerView);

    public new IProblem Content {
      get { return (IProblem)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public ProblemView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      IProblemInstanceConsumer consumer = Content as IProblemInstanceConsumer;
      if (consumer != null) {
        IEnumerable<Type> viewTypes = MainFormManager.GetViewTypes(consumer.GetType(), true);
        Type genericView = viewTypes.Where(x => x.IsSubclassOf(neededViewType)).First();
        ProblemInstanceConsumerViewHost.Content = null; //necessary to enable the change of the ViewType
        ProblemInstanceConsumerViewHost.ViewType = genericView;
        ProblemInstanceConsumerViewHost.Content = consumer;
        ProblemInstanceConsumerView view = (ProblemInstanceConsumerView)ProblemInstanceConsumerViewHost.ActiveView;
        problemInstanceSplitContainer.Panel1Collapsed = !view.ProblemInstanceProviders.Any();
      } else {
        problemInstanceSplitContainer.Panel1Collapsed = true;
      }
      SetEnabledStateOfControls();
    }

  }
}
