using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.JsonInterface;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.PluginInfrastructure.Manager;
using HeuristicLab.Problems.TravelingSalesman;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface.Tests {
  [TestClass]
  public class GeneratorInstantiatorTest {
    private string TmpDirectoryPath => Path.Combine(Directory.GetCurrentDirectory(), "JsonInterfaceTestEnvironment");
    private string TemplateName => Path.Combine(TmpDirectoryPath, "Template");
    private string TemplateFilePath => $"{TemplateName}.json";
    private string ConfigFilePath => Path.Combine(TmpDirectoryPath, "Config.json");
    private string OutFilePath => Path.Combine(TmpDirectoryPath, "out.json");

    [TestInitialize()]
    public void CreateTempFiles() {
      string pluginPath = Path.GetFullPath(Directory.GetCurrentDirectory());
      var pluginManager = new PluginManager(pluginPath);
      pluginManager.DiscoverAndCheckPlugins();


      Directory.CreateDirectory(TmpDirectoryPath);

      GeneticAlgorithm alg = new GeneticAlgorithm();
      alg.PopulationSize.Value = 200;
      alg.Problem = new TravelingSalesmanProblem();
      
      var rootItem = JsonItemConverter.Extract(alg);
      foreach (var item in rootItem)
        item.Active = true; //activate all items
      JsonTemplateGenerator.GenerateTemplate(TemplateName, alg, rootItem);


      var populationItem = 
        (IntJsonItem)rootItem.Where(x => x.Name.Contains("PopulationSize")).FirstOrDefault();

      populationItem.Value = 500;

      JArray configItems = new JArray();
      configItems.Add(populationItem.GenerateJObject());

      File.WriteAllText(ConfigFilePath, SingleLineArrayJsonWriter.Serialize(configItems));

    }

    private void CreateTestEnvironment(IOptimizer optimizer, Func<IJsonItem,IEnumerable<IJsonItem>> selectConfigItems, Action body) {
      // initialization
      Directory.CreateDirectory(TmpDirectoryPath);

      var rootItem = JsonItemConverter.Extract(optimizer);
      foreach (var item in rootItem)
        item.Active = true; //activate all items
      JsonTemplateGenerator.GenerateTemplate(TemplateName, optimizer, rootItem);

      JArray configItemsJArray = new JArray();
      var configItems = selectConfigItems(rootItem);

      foreach(var item in configItems) {
        configItemsJArray.Add(item.GenerateJObject());
      }
      File.WriteAllText(ConfigFilePath, SingleLineArrayJsonWriter.Serialize(configItemsJArray));

      // execute test
      body();

      // cleanup
      var files = Directory.GetFiles(TmpDirectoryPath);
      foreach (var file in files)
        File.Delete(file);
      Directory.Delete(TmpDirectoryPath);
    }

    [TestMethod]
    public void TestGATSP() {
      var ga = new GeneticAlgorithm();
      ga.PopulationSize.Value = 200;
      ga.Problem = new TravelingSalesmanProblem();

      CreateTestEnvironment(ga, 
        r => r.Where(x => x.Name.Contains("PopulationSize"))
              .Cast<IntJsonItem>()
              .Select(x => { x.Value = 500; return x; }),
        () => {
          var result = JsonTemplateInstantiator.Instantiate(TemplateFilePath, ConfigFilePath);
          var optimizer = (GeneticAlgorithm)result.Optimizer;
          Assert.AreEqual(500, optimizer.PopulationSize.Value);

          PluginInfrastructure.Main.HeadlessRun(new string[] { "/start:JsonInterface", TemplateFilePath, ConfigFilePath, OutFilePath });
          Assert.IsTrue(File.Exists(OutFilePath));

          var runs = JArray.Parse(File.ReadAllText(OutFilePath));
          var results = (JObject)runs[0];
          results.Property("Generations");
        });
    }

    [TestMethod]
    public void TestGASymReg() {
      var ga = new GeneticAlgorithm();
      ga.PopulationSize.Value = 200;
      ga.Problem = new Problems.DataAnalysis.Symbolic.Regression.SymbolicRegressionSingleObjectiveProblem();

      CreateTestEnvironment(ga,
        r => r.Where(x => x.Name.Contains("PopulationSize"))
              .Cast<IntJsonItem>()
              .Select(x => { x.Value = 500; return x; }),
        () => {
          var result = JsonTemplateInstantiator.Instantiate(TemplateFilePath, ConfigFilePath);
          var optimizer = (GeneticAlgorithm)result.Optimizer;
          Assert.AreEqual(500, optimizer.PopulationSize.Value);

          PluginInfrastructure.Main.HeadlessRun(new string[] { "/start:JsonInterface", TemplateFilePath, ConfigFilePath, OutFilePath });
          Assert.IsTrue(File.Exists(OutFilePath));

          var runs = JArray.Parse(File.ReadAllText(OutFilePath));
          var results = (JObject)runs[0];
          results.Property("Generations");
        });
    }

    [TestMethod]
    public void TestOSGASymReg() {
      var osga = new Algorithms.OffspringSelectionGeneticAlgorithm.OffspringSelectionGeneticAlgorithm();
      osga.PopulationSize.Value = 200;
      osga.Problem = new Problems.DataAnalysis.Symbolic.Regression.SymbolicRegressionSingleObjectiveProblem();

      CreateTestEnvironment(osga,
        r => r.Where(x => x.Name.Contains("PopulationSize"))
              .Cast<IntJsonItem>()
              .Select(x => { x.Value = 500; return x; }),
        () => {
          var result = JsonTemplateInstantiator.Instantiate(TemplateFilePath, ConfigFilePath);
          var optimizer = (Algorithms.OffspringSelectionGeneticAlgorithm.OffspringSelectionGeneticAlgorithm)result.Optimizer;
          Assert.AreEqual(500, optimizer.PopulationSize.Value);

          PluginInfrastructure.Main.HeadlessRun(new string[] { "/start:JsonInterface", TemplateFilePath, ConfigFilePath, OutFilePath });
          Assert.IsTrue(File.Exists(OutFilePath));

          var runs = JArray.Parse(File.ReadAllText(OutFilePath));
          var results = (JObject)runs[0];
          results.Property("Generations");
        });
    }

    [TestMethod]
    public void TestOSGATSP() {
      var osga = new Algorithms.OffspringSelectionGeneticAlgorithm.OffspringSelectionGeneticAlgorithm();
      osga.PopulationSize.Value = 200;
      osga.Problem = new TravelingSalesmanProblem();

      CreateTestEnvironment(osga,
        r => r.Where(x => x.Name.Contains("PopulationSize"))
              .Cast<IntJsonItem>()
              .Select(x => { x.Value = 500; return x; }),
        () => {
          var result = JsonTemplateInstantiator.Instantiate(TemplateFilePath, ConfigFilePath);
          var optimizer = (Algorithms.OffspringSelectionGeneticAlgorithm.OffspringSelectionGeneticAlgorithm)result.Optimizer;
          Assert.AreEqual(500, optimizer.PopulationSize.Value);

          PluginInfrastructure.Main.HeadlessRun(new string[] { "/start:JsonInterface", TemplateFilePath, ConfigFilePath, OutFilePath });
          Assert.IsTrue(File.Exists(OutFilePath));

          var runs = JArray.Parse(File.ReadAllText(OutFilePath));
          var results = (JObject)runs[0];
          results.Property("Generations");
        });
    }
  }
}
