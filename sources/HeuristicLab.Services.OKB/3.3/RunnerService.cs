#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Data.Linq;
using System.Linq;
using System.ServiceModel;
using HeuristicLab.Services.OKB.DataAccess;
using log4net;

namespace HeuristicLab.Services.OKB {

  /// <summary>
  /// Implementation of the <see cref="IRunnerService"/>.
  /// </summary>
  [ServiceBehavior(
    InstanceContextMode = InstanceContextMode.PerSession)]
  public class RunnerService : IRunnerService, IDisposable {

    private Guid sessionID;
    private static ILog logger = LogManager.GetLogger(typeof(RunnerService));

    private void Log(string message, params object[] args) {
      using (log4net.ThreadContext.Stacks["NDC"].Push(sessionID.ToString())) {
        logger.Info(String.Format(message, args));
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RunnerService"/> class.
    /// </summary>
    public RunnerService() {
      sessionID = Guid.NewGuid();
      Log("Instantiating new service");
    }

    /// <summary>
    /// Obtain a "starter kit" of information, containing:
    /// <list>
    /// <item>algorithm classes including algorithms</item>
    /// <item>problem classes including problems</item>
    /// <item>projects</item>
    /// </list>
    /// </summary>
    public StarterKit GetStarterKit(string platformName) {
      Log("distributing starter kit");
      Platform platform;
      using (OKBDataContext okb = new OKBDataContext()) {
        try {
          platform = okb.Platforms.Single(p => p.Name == platformName);
        }
        catch (InvalidOperationException) {
          throw new FaultException(String.Format(
            "Invalid platform name \"{0}\" available platforms are: \"{1}\"",
            platformName,
            string.Join("\", \"", okb.Platforms.Select(p => p.Name).ToArray())));
        }
      }
      using (OKBDataContext okb = new OKBDataContext()) {
        DataLoadOptions dlo = new DataLoadOptions();
        dlo.LoadWith<AlgorithmClass>(ac => ac.Algorithms);
        dlo.AssociateWith<AlgorithmClass>(ac => ac.Algorithms.Where(a => a.Platform == platform));
        dlo.LoadWith<ProblemClass>(p => p.Problems);
        dlo.AssociateWith<ProblemClass>(pc => pc.Problems.Where(p => p.Platform == platform));
        okb.LoadOptions = dlo;
        var StarterKit = new StarterKit() {
          AlgorithmClasses = okb.AlgorithmClasses.ToList(),
          ProblemClasses = okb.ProblemClasses.ToList(),
          Projects = okb.Projects.ToList()
        };
        return StarterKit;
      }
    }

    /// <summary>
    /// Augument the given algorithm and problem entities with information
    /// to conduct an experiment. This will add algorithm parameters and results and
    /// problem characteristic values and solution representation as well as data
    /// necessary for deserialization.
    /// </summary>
    public ExperimentKit PrepareExperiment(Algorithm algorithm, Problem problem) {
      Log("preparing experiment: {0}@{1}", algorithm.Name, problem.Name);
      OKBDataContext okb = new OKBDataContext();
      try {
        DataLoadOptions dlo = new DataLoadOptions();
        dlo.LoadWith<Problem>(p => p.SolutionRepresentation);
        dlo.LoadWith<Parameter>(p => p.DataType);
        dlo.LoadWith<Result>(r => r.DataType);
        dlo.LoadWith<Algorithm_Parameter>(ap => ap.Parameter);
        dlo.LoadWith<Problem_Parameter>(pp => pp.Parameter);
        dlo.LoadWith<Algorithm_Result>(ar => ar.Result);
        okb.LoadOptions = dlo;
        algorithm = okb.Algorithms.Single(a => a.Id == algorithm.Id);
        problem = okb.Problems.Single(p => p.Id == problem.Id);
        algorithm.Algorithm_Parameters.Load();
        algorithm.Algorithm_Results.Load();
        problem.IntProblemCharacteristicValues.Load();
        problem.FloatProblemCharacteristicValues.Load();
        problem.CharProblemCharacteristicValues.Load();
        problem.Problem_Parameters.Load();
        return new ExperimentKit() {
          Algorithm = algorithm,
          Problem = problem
        };
      }
      catch (Exception x) {
        Log("exception caught: " + x.ToString());
        throw new FaultException("Excaption caught: " + x.ToString());
      }
    }

    /// <summary>
    /// Adds the a new <see cref="Experiment"/>
    /// 	<see cref="Run"/>.
    /// The <see cref="Experiment"/> is created if necessary as well as
    /// all <see cref="Parameter"/>s and <see cref="Result"/>s. If an
    /// identical experiment has been conducted before the new run is
    /// linked to this previous experiment instead.
    /// </summary>
    /// <param name="algorithm">The algorithm.</param>
    /// <param name="problem">The problem.</param>
    /// <param name="project">The project.</param>
    public void AddRun(Algorithm algorithm, Problem problem, Project project) {
      Log("adding run for {0}@{1}({2})[{3}, {4}]",
        algorithm.Name, problem.Name, project.Name, currentUser.Name, currentClient.Name);
      try {
        using (OKBDataContext okb = new OKBDataContext()) {
          Experiment experiment = GetOrCreateExperiment(algorithm, problem, project, currentUser, okb);
          Run run = new Run() {
            Experiment = experiment,
            UserId = currentUser.Id,
            ClientId = currentClient.Id,
            FinishedDate = DateTime.Now,
            ResultValues = algorithm.ResultValues
          };
          okb.Runs.InsertOnSubmit(run);
          okb.SubmitChanges();
        }
      }
      catch (Exception x) {
        Log(x.ToString());
        throw new FaultException("Could not add run: " + x.ToString());
      }
    }

    private Experiment GetOrCreateExperiment(Algorithm algorithm, Problem problem,
        Project project, User user, OKBDataContext okb) {
      MatchResults(algorithm.Results, okb);
      EnsureParametersExist(algorithm.Parameters, okb);
      var experimentQuery = CreateExperimentQuery(algorithm, problem, project, okb);
      if (experimentQuery.Count() > 0) {
        if (experimentQuery.Count() > 1)
          Log("Warning: duplicate experiment found");
        Log("reusing existing experiment");
        Experiment experiment = experimentQuery.First();
        return experiment;
      } else {
        Log("creating new experiment");
        Experiment experiment = new Experiment() {
          AlgorithmId = algorithm.Id,
          ProblemId = problem.Id,
          ProjectId = project.Id,
          ParameterValues = algorithm.ParameterValues
        };
        okb.Experiments.InsertOnSubmit(experiment);
        return experiment;
      }
    }

    private void MatchResults(IQueryable<Result> results, OKBDataContext okb) {
      foreach (var result in results.Where(r => r.Name != null)) {
        result.Id = okb.Results.Single(r => r.Name == result.Name).Id;
        Log("mapping named result {0} -> Id {1}", result.Name, result.Id);
        var value = result.ResultValue;
        if (value != null) {
          result.ResultValue = (IResultValue)Activator.CreateInstance(value.GetType());
          result.ResultValue.Result = result;
          result.ResultValue.Value = value.Value;
        }
      }
    }

    private void EnsureParametersExist(IQueryable<Parameter> parameters, OKBDataContext okb) {
      foreach (var param in parameters.Where(p => p.DataType != null && p.DataType.ClrName != null)) {
        DataType dataType = GetOrCreateDataType(okb, param.DataType.ClrName);
        param.DataType = new DataType() { Id = dataType.Id };
        Log("mapping datatype {0} to id {1}", dataType.ClrName, dataType.Id);
      }
      okb.SubmitChanges();
      var namedParams = parameters.Where(p => p.Name != null);
      var newParams = namedParams.Except(okb.Parameters, new NameComprarer());
      foreach (var p in newParams) {
        Log("creating new parameter {0} ({2}:{1})", p.Name, p.DataType.ClrName, p.DataType.Id);
        okb.Parameters.InsertOnSubmit(new Parameter() {
          Name = p.Name,
          DataTypeId = p.DataTypeId,
        });
      }
      okb.SubmitChanges();
      foreach (var np in namedParams) {
        np.Id = okb.Parameters.Single(p => p.Name == np.Name).Id;
        Log("mapping named parameter {0} -> Id {1}", np.Name, np.Id);
        var value = np.ParameterValue;
        if (value != null) {
          OperatorParameterValue opVal = value as OperatorParameterValue;
          np.ParameterValue = (IParameterValue)Activator.CreateInstance(value.GetType());
          np.ParameterValue.Parameter = np;
          np.ParameterValue.Value = value.Value;
          if (opVal != null) {
            OperatorParameterValue newVal = np.ParameterValue as OperatorParameterValue;
            DataType dataType = GetOrCreateDataType(okb, opVal.DataType.ClrName);
            newVal.DataType = new DataType() { Id = dataType.Id };
            Log("mapping operator parameter datatype {0} to id {1}", dataType.ClrName, dataType.Id);
          }
        }
      }
    }

    private DataType GetOrCreateDataType(OKBDataContext okb, string clrName) {
      DataType dataType = okb.DataTypes.SingleOrDefault(dt => dt.ClrName == clrName);
      if (dataType == null) {
        Log("creating new datatype for ", clrName);
        dataType = new DataType() {
          ClrName = clrName,
          SqlName = "BLOB",
        };
        okb.DataTypes.InsertOnSubmit(dataType);
        okb.SubmitChanges();
      }
      return dataType;
    }

    private IQueryable<Experiment> CreateExperimentQuery(Algorithm algorithm, Problem problem, Project project,
        OKBDataContext okb) {
      var experimentQuery =
        from x in okb.Experiments
        where x.Algorithm == algorithm
        where x.Problem == problem
        where x.ProjectId == project.Id
        select x;
      foreach (IntParameterValue ipv in algorithm.IntParameterValues) {
        experimentQuery = experimentQuery
          .Where(x => x.IntParameterValues.Any(p =>
            p.ParameterId == ipv.ParameterId && p.Value == ipv.Value));
      }
      foreach (FloatParameterValue fpv in algorithm.FloatParameterValues) {
        experimentQuery = experimentQuery
          .Where(x => x.FloatParameterValues.Any(p =>
            p.ParameterId == fpv.ParameterId && p.Value == fpv.Value));
      }
      foreach (CharParameterValue cpv in algorithm.CharParameterValues) {
        experimentQuery = experimentQuery
          .Where(x => x.CharParameterValues.Any(p =>
            p.ParameterId == cpv.ParameterId && p.Value == cpv.Value));
      }
      foreach (OperatorParameterValue opv in algorithm.OperatorParameterValues) {
        experimentQuery = experimentQuery
          .Where(x => x.OperatorParameterValues.Any(p =>
            p.ParameterId == opv.ParameterId && p.DataTypeId == opv.DataTypeId));
      }
      Log("experiment query: ", experimentQuery.Expression.ToString());
      return experimentQuery;
    }

    /// <summary>
    /// Determines whether this instance is connected.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this instance is connected; otherwise, <c>false</c>.
    /// </returns>
    public bool IsConnected() {
      return currentUser != null;
    }

    User currentUser = null;
    Client currentClient = null;

    /// <summary>
    /// Logs the specified username in. In case the user or client
    /// does not exist yet, they are created on the server. This
    /// method is currently not used for authentication but merely
    /// for auditing.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="clientname">The clientname.</param>
    /// <returns>
    ///   <c>true</c> if the login was successful; <c>false</c> otherwise.
    /// </returns>
    public bool Login(string clientname) {
      string username = ServiceSecurityContext.Current.PrimaryIdentity.Name;

      Log("Authenticating {0}@{1}", username, clientname);
      if (string.IsNullOrEmpty(username) ||
          string.IsNullOrEmpty(clientname) ||
          ServiceSecurityContext.Current.IsAnonymous) {
        Log("rejecting anonymous login");
        return false;
      }
      using (OKBDataContext okb = new OKBDataContext()) {
        currentUser = okb.Users.SingleOrDefault(u => u.Name == username);
        currentClient = okb.Clients.SingleOrDefault(c => c.Name == clientname);
        if (currentUser == null) {
          currentUser = new User() { Name = username, Id = Guid.NewGuid() };
          okb.Users.InsertOnSubmit(currentUser);
          okb.SubmitChanges();
        }
        if (currentClient == null) {
          currentClient = new Client() { Name = clientname, Id = Guid.NewGuid() };
          okb.Clients.InsertOnSubmit(currentClient);
          okb.SubmitChanges();
        }
        Log("  user = {0}, client = {1}", currentUser, currentClient);
        return true;
      }
    }

    /// <summary>
    /// Logout out and closes the connection.
    /// </summary>
    public void Logout() {
      Log("Logging out");
      currentUser = null;
      currentClient = null;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() {
      Log("Closing session...");
      Logout();
    }
  }

  /// <summary>
  /// Compares two parameters by name.
  /// </summary>
  internal class NameComprarer : IEqualityComparer<Parameter> {

    /// <summary>
    /// Determines whether the specified objects are equal.
    /// </summary>
    /// <param name="x">The first object of type <paramref name="T"/> to compare.</param>
    /// <param name="y">The second object of type <paramref name="T"/> to compare.</param>
    /// <returns>
    /// true if the specified objects are equal; otherwise, false.
    /// </returns>
    public bool Equals(Parameter x, Parameter y) {
      return x.Name == y.Name;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
    /// </exception>
    public int GetHashCode(Parameter obj) {
      return obj.Name.GetHashCode();
    }

  }
}