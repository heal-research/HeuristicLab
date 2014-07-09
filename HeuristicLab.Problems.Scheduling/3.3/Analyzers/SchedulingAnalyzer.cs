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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.ScheduleEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Scheduling {
  [Item("SchedulingAnalyzer", "Represents the generalized form of Analyzers for Scheduling Problems.")]
  [StorableClass]
  public abstract class SchedulingAnalyzer : SingleSuccessorOperator, IAnalyzer {
    public virtual bool EnabledByDefault {
      get { return true; }
    }

    [StorableConstructor]
    protected SchedulingAnalyzer(bool deserializing) : base(deserializing) { }
    protected SchedulingAnalyzer(SchedulingAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }

    #region Parameter Properties
    public LookupParameter<BoolValue> MaximizationParameter {
      get { return (LookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public LookupParameter<Schedule> BestSolutionParameter {
      get { return (LookupParameter<Schedule>)Parameters["BestSolution"]; }
    }
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    public LookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public LookupParameter<Schedule> BestKnownSolutionParameter {
      get { return (LookupParameter<Schedule>)Parameters["BestKnownSolution"]; }
    }
    public ScopeTreeLookupParameter<Schedule> ScheduleParameter {
      get { return (ScopeTreeLookupParameter<Schedule>)Parameters["Schedule"]; }
    }
    #endregion

    public SchedulingAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the JSSP solutions which should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<Schedule>("Schedule", "The solutions from which the best solution has to be chosen from."));
      Parameters.Add(new LookupParameter<Schedule>("BestSolution", "The best JSSP solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the best JSSP solution should be stored."));
      Parameters.Add(new LookupParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this JSSP instance."));
      Parameters.Add(new LookupParameter<Schedule>("BestKnownSolution", "The best known solution of this JSSP instance."));
    }
  }
}
