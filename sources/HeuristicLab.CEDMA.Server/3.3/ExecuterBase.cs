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
using HeuristicLab.Modeling;

namespace HeuristicLab.CEDMA.Server {
  public abstract class ExecuterBase : IExecuter {
    private IDispatcher dispatcher;
    protected IDispatcher Dispatcher {
      get { return dispatcher; }
    }
    private IStore store;

    private int maxActiveJobs;
    public int MaxActiveJobs {
      get { return maxActiveJobs; }
      set {
        if (value < 0) throw new ArgumentException("Only positive values are allowed for MaxActiveJobs");
        maxActiveJobs = value;
      }
    }

    public ExecuterBase(IDispatcher dispatcher, IStore store) {
      maxActiveJobs = 10;
      this.dispatcher = dispatcher;
      this.store = store;
    }

    public void Start() {
      new Thread(StartJobs).Start();
    }

    protected abstract void StartJobs();

    protected void StoreResults(IAlgorithm finishedAlgorithm) {
      Entity modelEntity = new Entity(Ontology.CedmaNameSpace + Guid.NewGuid());
      // store.Add(new Statement(finishedAlgorithm.DataSetEntity, Ontology.PredicateHasModel, modelEntity));
      StoreModelAttribute(modelEntity, Ontology.TargetVariable, finishedAlgorithm.Model.TargetVariable);
      StoreModelAttribute(modelEntity, Ontology.AlgorithmName, finishedAlgorithm.Description);
      
      IModel model = finishedAlgorithm.Model;
      StoreModelAttribute(modelEntity, Ontology.TrainingMeanSquaredError, model.TrainingMeanSquaredError);
      StoreModelAttribute(modelEntity, Ontology.ValidationMeanSquaredError, model.ValidationMeanSquaredError);
      StoreModelAttribute(modelEntity, Ontology.TestMeanSquaredError, model.TestMeanSquaredError);
      StoreModelAttribute(modelEntity, Ontology.TrainingCoefficientOfDetermination, model.TrainingCoefficientOfDetermination);
      StoreModelAttribute(modelEntity, Ontology.ValidationCoefficientOfDetermination, model.ValidationCoefficientOfDetermination);
      StoreModelAttribute(modelEntity, Ontology.TestCoefficientOfDetermination, model.TestCoefficientOfDetermination);
      StoreModelAttribute(modelEntity, Ontology.TrainingVarianceAccountedFor, model.TrainingVarianceAccountedFor);
      StoreModelAttribute(modelEntity, Ontology.ValidationVarianceAccountedFor, model.ValidationVarianceAccountedFor);
      StoreModelAttribute(modelEntity, Ontology.TestVarianceAccountedFor, model.TestVarianceAccountedFor);
      StoreModelAttribute(modelEntity, Ontology.TrainingMeanAbsolutePercentageError, model.TrainingMeanAbsolutePercentageError);
      StoreModelAttribute(modelEntity, Ontology.ValidationMeanAbsolutePercentageError, model.ValidationMeanAbsolutePercentageError);
      StoreModelAttribute(modelEntity, Ontology.TestMeanAbsolutePercentageError, model.TestMeanAbsolutePercentageError);
      StoreModelAttribute(modelEntity, Ontology.TrainingMeanAbsolutePercentageOfRangeError, model.TrainingMeanAbsolutePercentageOfRangeError);
      StoreModelAttribute(modelEntity, Ontology.ValidationMeanAbsolutePercentageOfRangeError, model.ValidationMeanAbsolutePercentageOfRangeError);
      StoreModelAttribute(modelEntity, Ontology.TestMeanAbsolutePercentageOfRangeError, model.TestMeanAbsolutePercentageOfRangeError);

      byte[] serializedModel = PersistenceManager.SaveToGZip(model.Data);
      store.Add(new Statement(modelEntity, Ontology.PredicateSerializedData, new Literal(Convert.ToBase64String(serializedModel))));
    }

    private void StoreModelAttribute(Entity model, Entity predicate, object value) {
      store.Add(new Statement(model, predicate, new Literal(value)));
    }

    public abstract string[] GetJobs();
  }
}
