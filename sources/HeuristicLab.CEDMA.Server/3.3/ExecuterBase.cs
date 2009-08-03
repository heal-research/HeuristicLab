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
using System.ServiceModel.Description;
using System.Linq;
using HeuristicLab.CEDMA.Core;
using HeuristicLab.Data;
using HeuristicLab.Grid;
using System.Diagnostics;
using HeuristicLab.Core;
using System.Threading;
using HeuristicLab.Modeling;
using HeuristicLab.Modeling.Database;

namespace HeuristicLab.CEDMA.Server {
  public abstract class ExecuterBase : IExecuter {
    internal event EventHandler Changed;

    private IDispatcher dispatcher;
    protected IDispatcher Dispatcher {
      get { return dispatcher; }
    }    
    private IModelingDatabase databaseService;

    private int maxActiveJobs;
    public int MaxActiveJobs {
      get { return maxActiveJobs; }
      set {
        if (value < 0) throw new ArgumentException("Only positive values are allowed for MaxActiveJobs");
        maxActiveJobs = value;
        OnChanged();
      }
    }

    public ExecuterBase(IDispatcher dispatcher, IModelingDatabase databaseService) {
      maxActiveJobs = 10;
      this.dispatcher = dispatcher;
      this.databaseService = databaseService;
    }

    public void Start() {
      new Thread(StartJobs).Start();
    }

    protected abstract void StartJobs();

    protected void SetResults(IScope src, IScope target) {
      foreach (HeuristicLab.Core.IVariable v in src.Variables) {
        target.AddVariable(v);
      }
      foreach (IScope subScope in src.SubScopes) {
        target.AddSubScope(subScope);
      }
      foreach (KeyValuePair<string, string> alias in src.Aliases) {
        target.AddAlias(alias.Key, alias.Value);
      }
    }

    protected void StoreResults(HeuristicLab.Modeling.IAlgorithm finishedAlgorithm) {
      databaseService.Persist(finishedAlgorithm);
      //Entity modelEntity = new Entity(Ontology.CedmaNameSpace + Guid.NewGuid());
      //IModel model = finishedAlgorithm.Model;
      //List<Statement> statements = new List<Statement>();
      //statements.Add(new Statement(modelEntity, Ontology.InstanceOf, Ontology.TypeModel));
      //statements.Add(new Statement(modelEntity, Ontology.TargetVariable, new Literal(model.TargetVariable)));
      //statements.Add(new Statement(modelEntity, Ontology.Name, new Literal(finishedAlgorithm.Name)));
      
      //statements.Add(new Statement(modelEntity, Ontology.TrainingMeanSquaredError, new Literal(model.TrainingMeanSquaredError)));
      //statements.Add(new Statement(modelEntity, Ontology.ValidationMeanSquaredError, new Literal(model.ValidationMeanSquaredError)));
      //statements.Add(new Statement(modelEntity, Ontology.TestMeanSquaredError, new Literal(model.TestMeanSquaredError)));
      //statements.Add(new Statement(modelEntity, Ontology.TrainingCoefficientOfDetermination, new Literal(model.TrainingCoefficientOfDetermination)));
      //statements.Add(new Statement(modelEntity, Ontology.ValidationCoefficientOfDetermination, new Literal(model.ValidationCoefficientOfDetermination)));
      //statements.Add(new Statement(modelEntity, Ontology.TestCoefficientOfDetermination, new Literal(model.TestCoefficientOfDetermination)));
      //statements.Add(new Statement(modelEntity, Ontology.TrainingVarianceAccountedFor, new Literal(model.TrainingVarianceAccountedFor)));
      //statements.Add(new Statement(modelEntity, Ontology.ValidationVarianceAccountedFor, new Literal(model.ValidationVarianceAccountedFor)));
      //statements.Add(new Statement(modelEntity, Ontology.TestVarianceAccountedFor, new Literal(model.TestVarianceAccountedFor)));
      //statements.Add(new Statement(modelEntity, Ontology.TrainingMeanAbsolutePercentageError, new Literal(model.TrainingMeanAbsolutePercentageError)));
      //statements.Add(new Statement(modelEntity, Ontology.ValidationMeanAbsolutePercentageError, new Literal(model.ValidationMeanAbsolutePercentageError)));
      //statements.Add(new Statement(modelEntity, Ontology.TestMeanAbsolutePercentageError, new Literal(model.TestMeanAbsolutePercentageError)));
      //statements.Add(new Statement(modelEntity, Ontology.TrainingMeanAbsolutePercentageOfRangeError, new Literal(model.TrainingMeanAbsolutePercentageOfRangeError)));
      //statements.Add(new Statement(modelEntity, Ontology.ValidationMeanAbsolutePercentageOfRangeError, new Literal(model.ValidationMeanAbsolutePercentageOfRangeError)));
      //statements.Add(new Statement(modelEntity, Ontology.TestMeanAbsolutePercentageOfRangeError, new Literal(model.TestMeanAbsolutePercentageOfRangeError)));

      //for (int i = 0; i < finishedAlgorithm.Dataset.Columns; i++) {
      //  try {
      //    string variableName = finishedAlgorithm.Dataset.GetVariableName(i);
      //    double qualImpact = model.GetVariableQualityImpact(variableName);
      //    double evalImpact = model.GetVariableEvaluationImpact(variableName);

      //    Entity inputVariableEntity = new Entity(Ontology.CedmaNameSpace + Guid.NewGuid());
      //    statements.Add(new Statement(inputVariableEntity, Ontology.InstanceOf, Ontology.TypeVariableImpact));
      //    statements.Add(new Statement(modelEntity, Ontology.HasInputVariable, inputVariableEntity));
      //    statements.Add(new Statement(inputVariableEntity, Ontology.EvaluationImpact, new Literal(evalImpact)));
      //    statements.Add(new Statement(inputVariableEntity, Ontology.QualityImpact, new Literal(qualImpact)));
      //    statements.Add(new Statement(inputVariableEntity, Ontology.Name, new Literal(variableName)));
      //  }
      //  catch (ArgumentException) {
      //    // ignore
      //  }
      //}

      //byte[] serializedModel = PersistenceManager.SaveToGZip(model.Data);
      //statements.Add(new Statement(modelEntity, Ontology.SerializedData, new Literal(Convert.ToBase64String(serializedModel))));
      //store.AddRange(statements);
    }

    public abstract string[] GetJobs();

    protected internal void OnChanged() {
      if (Changed != null) Changed(this, new EventArgs());
    }

    #region IViewable Members

    public IView CreateView() {
      return new ExecuterView(this);
    }

    #endregion
  }
}
