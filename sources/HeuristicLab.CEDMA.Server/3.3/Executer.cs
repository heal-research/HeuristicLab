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
using System.Text;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;
using System.Net;
using System.ServiceModel;
using HeuristicLab.CEDMA.DB.Interfaces;
using HeuristicLab.CEDMA.DB;
using System.ServiceModel.Description;
using System.Linq;
using HeuristicLab.CEDMA.Core;
using HeuristicLab.GP.StructureIdentification;
using HeuristicLab.Data;
using HeuristicLab.Grid;
using System.Diagnostics;
using HeuristicLab.Core;
using System.Threading;

namespace HeuristicLab.CEDMA.Server {
  public class Executer {
    private IDispatcher dispatcher;
    private JobManager jobManager;
    private IStore store;
    private Dictionary<WaitHandle, Execution> activeExecutions;

    private TimeSpan StartJobInterval {
      get { return TimeSpan.FromMilliseconds(500); }
    }

    private TimeSpan WaitForFinishedJobsTimeout {
      get { return TimeSpan.FromMilliseconds(100); }
    }

    private int maxActiveJobs;
    public int MaxActiveJobs {
      get { return maxActiveJobs; }
      set {
        if (value < 0) throw new ArgumentException("Only positive values are allowed for MaxActiveJobs");
        maxActiveJobs = value;
      }
    }

    public Executer(IDispatcher dispatcher, IStore store, string gridUrl) {
      activeExecutions = new Dictionary<WaitHandle, Execution>();
      maxActiveJobs = 10;
      this.dispatcher = dispatcher;
      this.store = store;
      this.jobManager = new JobManager(gridUrl);
      jobManager.Reset();
    }

    internal void Start() {
      new Thread(StartJobs).Start();
    }

    private void StartJobs() {
      List<WaitHandle> wh = new List<WaitHandle>();
      Dictionary<WaitHandle, AtomicOperation> activeOperations = new Dictionary<WaitHandle, AtomicOperation>();
      while (true) {
        try {
          // start new jobs as long as there are less than MaxActiveJobs 
          while (wh.Count < MaxActiveJobs) {
            Thread.Sleep(StartJobInterval);
            // get an execution from the dispatcher and execute in grid via job-manager
            Execution execution = dispatcher.GetNextJob();
            if (execution != null) {
              AtomicOperation op = new AtomicOperation(execution.Engine.OperatorGraph.InitialOperator, execution.Engine.GlobalScope);
              WaitHandle opWh = jobManager.BeginExecuteOperation(execution.Engine.GlobalScope, op);
              wh.Add(opWh);
              activeOperations.Add(opWh, op);
              lock (activeExecutions) {
                activeExecutions.Add(opWh, execution);
              }
            }
          }
          // wait until any job is finished
          WaitHandle[] whArr = wh.ToArray();
          int readyHandleIndex = WaitHandle.WaitAny(whArr, WaitForFinishedJobsTimeout);
          if (readyHandleIndex != WaitHandle.WaitTimeout) {
            WaitHandle readyHandle = whArr[readyHandleIndex];
            AtomicOperation finishedOp = activeOperations[readyHandle];
            wh.Remove(readyHandle);
            Execution finishedExecution = null;
            lock (activeExecutions) {
              finishedExecution = activeExecutions[readyHandle];
              activeExecutions.Remove(readyHandle);
            }
            activeOperations.Remove(readyHandle);
            ProcessingEngine finishedEngine = null;
            try {
              finishedEngine = jobManager.EndExecuteOperation(finishedOp);
            }
            catch (Exception badEx) {
              Trace.WriteLine("CEDMA Executer: Exception in job execution thread. " + badEx.Message);
            }
            if (finishedEngine != null) {
              StoreResults(finishedExecution, finishedEngine);
            }
          }
        }
        catch (Exception ex) {
          Trace.WriteLine("CEDMA Executer: Exception in job-management thread. " + ex.Message);
        }
      }
    }

    private void StoreResults(Execution finishedExecution, ProcessingEngine finishedEngine) {
      Entity model = new Entity(Ontology.CedmaNameSpace + Guid.NewGuid());
      store.Add(new Statement(finishedExecution.DataSetEntity, Ontology.PredicateHasModel, model));
      StoreModelAttribute(model, Ontology.TargetVariable, finishedExecution.TargetVariable);
      StoreModelAttribute(model, Ontology.AlgorithmName, finishedExecution.Description);
      Scope bestModelScope = finishedEngine.GlobalScope.GetVariableValue<Scope>("BestValidationSolution", false);
      StoreModelVariable(model, Ontology.TrainingMeanSquaredError, bestModelScope, "Quality");
      StoreModelVariable(model, Ontology.ValidationMeanSquaredError, bestModelScope, "ValidationQuality");
      StoreModelVariable(model, Ontology.TestMeanSquaredError, bestModelScope, "TestQuality");
      StoreModelVariable(model, Ontology.TrainingMeanAbsolutePercentageError, bestModelScope, "TrainingMAPE");
      StoreModelVariable(model, Ontology.ValidationMeanAbsolutePercentageError, bestModelScope, "ValidationMAPE");
      StoreModelVariable(model, Ontology.TestMeanAbsolutePercentageError, bestModelScope, "TestMAPE");
      StoreModelVariable(model, Ontology.TrainingMeanAbsolutePercentageOfRangeError, bestModelScope, "TrainingMAPRE");
      StoreModelVariable(model, Ontology.ValidationMeanAbsolutePercentageOfRangeError, bestModelScope, "ValidationMAPRE");
      StoreModelVariable(model, Ontology.TestMeanAbsolutePercentageOfRangeError, bestModelScope, "TestMAPRE");
      StoreModelVariable(model, Ontology.TrainingCoefficientOfDetermination, bestModelScope, "TrainingR2");
      StoreModelVariable(model, Ontology.ValidationCoefficientOfDetermination, bestModelScope, "ValidationR2");
      StoreModelVariable(model, Ontology.TestCoefficientOfDetermination, bestModelScope, "TestR2");
      StoreModelVariable(model, Ontology.TrainingCoefficientOfDetermination, bestModelScope, "TrainingVAF");
      StoreModelVariable(model, Ontology.ValidationCoefficientOfDetermination, bestModelScope, "ValidationVAF");
      StoreModelVariable(model, Ontology.TestCoefficientOfDetermination, bestModelScope, "TestVAF");
      StoreModelVariable(model, Ontology.TrainingTheilsInequalityCoefficient, bestModelScope, "TrainingTheilInequalityCoefficient");
      StoreModelVariable(model, Ontology.ValidationTheilsInequalityCoefficient, bestModelScope, "ValidationTheilInequalityCoefficient");
      StoreModelVariable(model, Ontology.TestTheilsInequalityCoefficient, bestModelScope, "TestTheilInequalityCoefficient");
      StoreModelVariable(model, Ontology.TrainingAccuracy, bestModelScope, "TrainingAccuracy");
      StoreModelVariable(model, Ontology.ValidationAccuracy, bestModelScope, "ValidationAccuracy");
      StoreModelVariable(model, Ontology.TestAccuracy, bestModelScope, "TestAccuracy");
      StoreModelVariable(model, Ontology.TreeSize, bestModelScope, "TreeSize");
      StoreModelVariable(model, Ontology.TreeHeight, bestModelScope, "TreeHeight");
      StoreModelVariable(model, Ontology.EvaluatedSolutions, bestModelScope, "EvaluatedSolutions");

      byte[] serializedModel = PersistenceManager.SaveToGZip(bestModelScope.GetVariableValue("FunctionTree", false));
      store.Add(new Statement(model, Ontology.PredicateSerializedData, new Literal(Convert.ToBase64String(serializedModel))));
    }

    private void StoreModelVariable(Entity model, Entity entity, Scope scope, string variableName) {
      if (scope.GetVariable(variableName) != null)
        StoreModelAttribute(model, entity, scope.GetVariableValue<ObjectData>(variableName, false).Data);
    }

    private void StoreModelAttribute(Entity model, Entity predicate, object value) {
      store.Add(new Statement(model, predicate, new Literal(value)));
    }

    internal string[] GetJobs() {
      lock (activeExecutions) {
        string[] retVal = new string[activeExecutions.Count];
        int i = 0;
        foreach (Execution e in activeExecutions.Values) {
          retVal[i++] = "Target-Variable: " + e.TargetVariable + " " + e.Description;
        }
        return retVal;
      }
    }
  }
}
