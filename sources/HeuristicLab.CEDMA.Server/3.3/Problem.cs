#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using System.Xml;
using HeuristicLab.Operators;

namespace HeuristicLab.CEDMA.Server {


  /// <summary>
  /// Problem describes the data mining task.
  /// Contains the actual data and meta-data:
  ///  * which variables should be modelled 
  ///  * regression, time-series or classification problem
  /// </summary>
  public class Problem {
    internal event EventHandler Changed;

    private HeuristicLab.DataAnalysis.Dataset dataset;
    public HeuristicLab.DataAnalysis.Dataset Dataset {
      get { return dataset; }
      set {
        if (value != dataset) {
          dataset = value;
        }
      }
    }

    private int trainingSamplesStart;
    public int TrainingSamplesStart {
      get { return trainingSamplesStart; }
      set { trainingSamplesStart = value; }
    }

    private int trainingSamplesEnd;
    public int TrainingSamplesEnd {
      get { return trainingSamplesEnd; }
      set { trainingSamplesEnd = value; }
    }

    private int validationSamplesStart;
    public int ValidationSamplesStart {
      get { return validationSamplesStart; }
      set { validationSamplesStart = value; }
    }

    private int validationSamplesEnd;
    public int ValidationSamplesEnd {
      get { return validationSamplesEnd; }
      set { validationSamplesEnd = value; }
    }

    private int testSamplesStart;
    public int TestSamplesStart {
      get { return testSamplesStart; }
      set { testSamplesStart = value; }
    }

    private int testSamplesEnd;
    public int TestSamplesEnd {
      get { return testSamplesEnd; }
      set { testSamplesEnd = value; }
    }

    //private List<int> allowedInputVariables;
    //public List<int> AllowedInputVariables {
    //  get { return allowedInputVariables; }
    //}

    //private List<int> allowedTargetVariables;
    //public List<int> AllowedTargetVariables {
    //  get { return allowedTargetVariables; }
    //}

    private bool autoRegressive;
    public bool AutoRegressive {
      get { return autoRegressive; }
      set { autoRegressive = value; }
    }

    private int minTimeOffset;
    public int MinTimeOffset {
      get { return minTimeOffset; }
      set { minTimeOffset = value; }
    }

    private int maxTimeOffset;
    public int MaxTimeOffset {
      get { return maxTimeOffset; }
      set { maxTimeOffset = value; }
    }

    private LearningTask learningTask;
    public LearningTask LearningTask {
      get { return learningTask; }
      set { learningTask = value; }
    }

    public Problem()
      : base() {
      //allowedInputVariables = new List<int>();
      //allowedTargetVariables = new List<int>();
      Dataset = new HeuristicLab.DataAnalysis.Dataset();
    }

    public string GetVariableName(int index) {
      return dataset.GetVariableName(index);
    }

    public IView CreateView() {
      return new ProblemView(this);
    }

    internal void FireChanged() {
      if (Changed != null) Changed(this, new EventArgs());
    }
  }
}
