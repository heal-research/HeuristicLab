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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Services.Deployment.Test {


  /// <summary>
  ///This is a test class for PluginStoreTest and is intended
  ///to contain all PluginStoreTest Unit Tests
  ///</summary>
  [TestClass()]
  public class PluginStoreTest {

    private System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext {
      get {
        return testContextInstance;
      }
      set {
        testContextInstance = value;
      }
    }

    #region Additional test attributes
    // 
    //You can use the following additional attributes as you write your tests:
    //
    //Use ClassInitialize to run code before running the first test in the class
    //[ClassInitialize()]
    //public static void MyClassInitialize(TestContext testContext)
    //{
    //}
    //
    //Use ClassCleanup to run code after all tests in a class have run
    //[ClassCleanup()]
    //public static void MyClassCleanup()
    //{
    //}
    //
    //Use TestInitialize to run code before running each test
    //[TestInitialize()]
    //public void MyTestInitialize()
    //{
    //}
    //
    //Use TestCleanup to run code after each test has run
    //[TestCleanup()]
    //public void MyTestCleanup()
    //{
    //}
    //
    #endregion


    /// <summary>
    ///A test for PluginFile
    ///</summary>
    [TestMethod()]
    public void PluginFileTest() {
      PluginStore target = new PluginStore();
      #region insert and retrieve a plugin file
      int oldCount = target.Plugins.Count();
      // store a new entry in the db
      string name = RandomName();
      target.Persist(new PluginDescription(name, new Version(0, 1)), enc.GetBytes("Zipped " + name));
      int newCount = target.Plugins.Count();
      // make sure the new entry was added to the db
      Assert.AreEqual(oldCount + 1, newCount);

      // get matching description from db and try to retrieve the stored file 
      PluginDescription pluginDescription = target.Plugins.Where(p => p.Name == name).Single();
      byte[] expected = enc.GetBytes("Zipped " + name);
      byte[] actual = target.PluginFile(pluginDescription);
      // check retrieved file
      Assert.AreEqual(expected.Length, actual.Length);
      for (int i = 0; i < expected.Length; i++) {
        Assert.AreEqual(expected[i], actual[i]);
      }
      #endregion
    }

    /// <summary>
    ///A test for Persist
    ///</summary>
    [TestMethod()]
    public void PersistPluginTest() {
      var vers01 = new Version(0, 1);
      PluginStore target = new PluginStore();

      #region persist single plugin without dependencies
      {
        string name = RandomName();
        int oldCount = target.Plugins.Count();
        PluginDescription pluginDescription = new PluginDescription(name, vers01);
        byte[] pluginPackage = enc.GetBytes("Zipped " + name);
        target.Persist(pluginDescription, pluginPackage);
        int newCount = target.Plugins.Count();
        Assert.AreEqual(oldCount + 1, newCount);
      }
      #endregion

      #region persist a product with same name and version as an existent product
      {
        string name = RandomName();
        int oldCount = target.Plugins.Count();
        PluginDescription pluginDescription = new PluginDescription(name, vers01);
        byte[] pluginPackage = enc.GetBytes("Zipped " + name);
        target.Persist(pluginDescription, pluginPackage);
        int newCount = target.Plugins.Count();
        Assert.AreEqual(oldCount + 1, newCount);

        // insert same name and version
        oldCount = target.Plugins.Count();
        pluginDescription = new PluginDescription(name, vers01);
        pluginPackage = enc.GetBytes("Zipped " + name);
        target.Persist(pluginDescription, pluginPackage);
        newCount = target.Plugins.Count();
        // make sure old entry was updated
        Assert.AreEqual(oldCount, newCount);

        // insert new with different version
        oldCount = target.Plugins.Count();
        pluginDescription = new PluginDescription(name, new Version(0, 2));
        pluginPackage = enc.GetBytes("Zipped " + name);
        target.Persist(pluginDescription, pluginPackage);
        newCount = target.Plugins.Count();
        // make sure a new entry was created
        Assert.AreEqual(oldCount + 1, newCount);
      }
      #endregion

      #region persist a plugin with an already persisted dependency
      {
        string name = RandomName();
        PluginDescription dependency = new PluginDescription(name, vers01);
        // insert dependency first
        target.Persist(dependency, enc.GetBytes("Zipped " + name));

        // persist another plugin that has a dependency on the first plugin
        string name2 = RandomName();
        int oldCount = target.Plugins.Count();

        PluginDescription newEntity = new PluginDescription(name2, vers01, Enumerable.Repeat(dependency, 1));
        byte[] newEntityPackage = enc.GetBytes("Zipped " + name2);
        target.Persist(newEntity, newEntityPackage);
        int newCount = target.Plugins.Count();
        Assert.AreEqual(oldCount + 1, newCount); // only one new plugin should be added

        // retrieve second plugin and check dependencies
        newEntity = target.Plugins.Where(p => p.Name == name2).Single();
        Assert.AreEqual(newEntity.Dependencies.Count(), 1);
        Assert.AreEqual(newEntity.Dependencies.First().Name, name);
      }
      #endregion

      #region try to persist a new  plugin with a non-existant dependency and check the expected exception
      {
        // try to persist a new plugin with a non-existant dependency
        try {
          string pluginName = RandomName();
          string dependencyName = RandomName();
          var dependency = new PluginDescription(dependencyName, vers01);
          var newEntity = new PluginDescription(pluginName, vers01, Enumerable.Repeat(dependency, 1));
          target.Persist(newEntity, enc.GetBytes("Zipped " + pluginName));
          Assert.Fail("persist should fail with ArgumentException");
        }
        catch (ArgumentException) {
          // this is expected
          Assert.IsTrue(true, "expected exception");
        }
      }
      #endregion

      #region update the plugin file of an existing plugin
      {
        // insert new plugin
        string pluginName = RandomName();
        var newPlugin = new PluginDescription(pluginName, vers01);
        target.Persist(newPlugin, enc.GetBytes("Zipped " + pluginName));

        // update the plugin file
        byte[] expected = enc.GetBytes("Zipped2 " + pluginName);
        target.Persist(newPlugin, expected);
        // check if the updated file is returned
        byte[] actual = target.PluginFile(newPlugin);
        // check retrieved file
        Assert.AreEqual(expected.Length, actual.Length);
        for (int i = 0; i < expected.Length; i++) {
          Assert.AreEqual(expected[i], actual[i]);
        }
      }
      #endregion

      #region update the dependencies of an existing plugin
      {
        string dependency1Name = RandomName();
        var newDependency1 = new PluginDescription(dependency1Name, vers01);
        target.Persist(newDependency1, enc.GetBytes("Zipped " + dependency1Name));

        string pluginName = RandomName();
        var newPlugin = new PluginDescription(pluginName, vers01, Enumerable.Repeat(newDependency1, 1));
        target.Persist(newPlugin, enc.GetBytes("Zipped " + pluginName));

        // retrieve plugin
        var dbPlugin = target.Plugins.Where(p => p.Name == pluginName).Single();
        Assert.AreEqual(dbPlugin.Dependencies.Count(), 1);
        Assert.AreEqual(dbPlugin.Dependencies.First().Name, dependency1Name);

        // change dependencies 
        string dependency2Name = RandomName();
        var newDependency2 = new PluginDescription(dependency2Name, vers01);
        target.Persist(newDependency2, enc.GetBytes("Zipped " + pluginName));

        newPlugin = new PluginDescription(pluginName, vers01, new PluginDescription[] { newDependency1, newDependency2 });
        target.Persist(newPlugin, enc.GetBytes("Zipped " + pluginName));
        // retrieve plugin
        dbPlugin = target.Plugins.Where(p => p.Name == pluginName).Single();
        Assert.AreEqual(dbPlugin.Dependencies.Count(), 2);
      }
      #endregion

      #region try to insert a plugin that references the same dependency twice and check if the correct exception is thrown
      {
        string depName = RandomName();
        var depPlugin = new PluginDescription(depName, vers01);
        target.Persist(depPlugin, enc.GetBytes("Zipped " + depName));

        // insert new plugin
        string pluginName = RandomName();
        var plugin = new PluginDescription(pluginName, vers01, new PluginDescription[] { depPlugin, depPlugin });
        try {
          target.Persist(plugin, enc.GetBytes("Zipped " + depName));
          Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException) {
          Assert.IsTrue(true, "Exception thrown as expected");
        }
      }
      #endregion

      #region try to insert a plugin that with a cyclic reference
      {
        string depName = RandomName();
        var depPlugin = new PluginDescription(depName, vers01);
        target.Persist(depPlugin, enc.GetBytes("Zipped " + depName));

        // update the plugin so that it has a dependency on itself 
        var plugin = new PluginDescription(depName, vers01, new PluginDescription[] { depPlugin });
        try {
          target.Persist(plugin, enc.GetBytes("Zipped " + depName));
          Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException) {
          Assert.IsTrue(true, "Exception thrown as expected");
        }
      }
      #endregion

    }

    /// <summary>
    ///A test for Persist
    ///</summary>
    [TestMethod()]
    public void PersistProductTest() {
      Version vers01 = new Version(0, 1);
      PluginStore target = new PluginStore();

      #region persist a product without plugins to the db
      {
        string prodName = RandomName();
        int oldCount = target.Products.Count();
        ProductDescription product = new ProductDescription(prodName, vers01);
        target.Persist(product);
        int newCount = target.Products.Count();
        Assert.AreEqual(oldCount + 1, newCount);
      }
      #endregion

      #region persist a product with the same name and version as an existant product
      {
        string prodName = RandomName();
        int oldCount = target.Products.Count();
        ProductDescription product = new ProductDescription(prodName, vers01);
        target.Persist(product);
        int newCount = target.Products.Count();
        Assert.AreEqual(oldCount + 1, newCount);

        // write a product with same name and version
        oldCount = target.Products.Count();
        product = new ProductDescription(prodName, vers01);
        target.Persist(product);
        newCount = target.Products.Count();

        // make sure that the old entry was updated
        Assert.AreEqual(oldCount, newCount);

        // write a product with same name and different version
        oldCount = target.Products.Count();
        product = new ProductDescription(prodName, new Version(0, 2));
        target.Persist(product);
        newCount = target.Products.Count();

        // make sure that a new entry was created
        Assert.AreEqual(oldCount + 1, newCount);
      }
      #endregion

      #region try to persist a product referencing an non-existant plugin and check the expected exception
      {
        // try to persist a product referencing a non-existant plugin
        string prodName = RandomName();
        string pluginName = RandomName();
        var plugin = new PluginDescription(pluginName, vers01);
        var product = new ProductDescription(prodName, vers01, Enumerable.Repeat(plugin, 1));
        try {
          target.Persist(product);
          Assert.Fail("persist should fail with ArgumentException");
        }
        catch (ArgumentException) {
          // this is expected
          Assert.IsTrue(true, "expected exception");
        }
      }
      #endregion

      #region persist a product with a single plugin reference
      {
        string prodName = RandomName();
        string pluginName = RandomName();
        var plugin = new PluginDescription(pluginName, vers01);
        var product = new ProductDescription(prodName, vers01, Enumerable.Repeat(plugin, 1));

        // persist the plugin first
        int oldCount = target.Products.Count();
        target.Persist(plugin, enc.GetBytes("Zipped " + plugin.Name));
        target.Persist(product);
        int newCount = target.Products.Count();
        // make sure the store went through
        Assert.AreEqual(oldCount + 1, newCount);
        // retrieve the product and check if the plugin list was stored/retrieved correctly
        var dbProd = target.Products.Where(p => p.Name == prodName).Single();
        Assert.AreEqual(dbProd.Plugins.Count(), 1);
        Assert.AreEqual(dbProd.Plugins.First().Name, pluginName);
      }
      #endregion

      #region update the plugin list of an existing product
      {
        string prodName = RandomName();
        string plugin1Name = RandomName();
        var plugin1 = new PluginDescription(plugin1Name, vers01);
        var product = new ProductDescription(prodName, vers01, Enumerable.Repeat(plugin1, 1));

        // persist the plugin first
        int oldCount = target.Products.Count();
        target.Persist(plugin1, enc.GetBytes("Zipped " + plugin1.Name));
        target.Persist(product);
        int newCount = target.Products.Count();
        // make sure the store went through
        Assert.AreEqual(oldCount + 1, newCount);

        var plugin2Name = RandomName();
        var plugin2 = new PluginDescription(plugin2Name, vers01);
        target.Persist(plugin2, enc.GetBytes("Zipped " + plugin2.Name));
        product = new ProductDescription(prodName, vers01, new PluginDescription[] { plugin1, plugin2 });
        oldCount = target.Products.Count();
        target.Persist(product);
        newCount = target.Products.Count();
        // make sure that a new entry was not created
        Assert.AreEqual(oldCount, newCount);

        // check the plugin list of the product
        var dbProduct = target.Products.Where(p => p.Name == prodName).Single();
        Assert.AreEqual(dbProduct.Plugins.Count(), 2);
      }
      #endregion

      #region insert a product which references the same plugin twice and check if the correct exception is thrown
      {
        string prodName = RandomName();
        string plugin1Name = RandomName();
        var plugin1 = new PluginDescription(plugin1Name, vers01);
        var product = new ProductDescription(prodName, vers01, Enumerable.Repeat(plugin1, 2));

        // persist the plugin first
        target.Persist(plugin1, enc.GetBytes("Zipped " + plugin1.Name));
        try {
          target.Persist(product);
          Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException) {
          Assert.IsTrue(true, "Expected exception was thrown.");
        }
      }
      #endregion
    }

    [TestMethod]
    public void InsertPluginPerformanceTest() {
      int nPlugins = 100;
      int maxDependencies = 10;
      int avgZipFileLength = 32000;

      var store = new PluginStore();
      Version vers01 = new Version(0, 1);
      // create a random byte array to represent file length
      byte[] zippedConstant = new byte[avgZipFileLength];
      Random r = new Random();
      r.NextBytes(zippedConstant);

      Stopwatch stopWatch = new Stopwatch();
      stopWatch.Start();
      // create plugins
      List<PluginDescription> plugins = new List<PluginDescription>();
      for (int i = 0; i < nPlugins; i++) {
        string name = RandomName();
        var dependencies = store.Plugins;
        if (dependencies.Count() > maxDependencies) dependencies = dependencies.Take(maxDependencies);
        var plugin = new PluginDescription(name, vers01, dependencies);
        store.Persist(plugin, zippedConstant);
        plugins.Add(plugin);
      }
      stopWatch.Stop();
      Assert.Inconclusive("Created " + nPlugins + " plugins in " + stopWatch.ElapsedMilliseconds +
        " ms (" + (double)stopWatch.ElapsedMilliseconds / nPlugins + " ms / plugin )");
    }
    [TestMethod]
    public void InsertProductPerformanceTest() {
      int nProducts = 50;
      int avgProductPlugins = 30;

      var store = new PluginStore();
      Version vers01 = new Version(0, 1);
      Stopwatch stopWatch = new Stopwatch();
      Random r = new Random();
      var plugins = store.Plugins.ToList();
      stopWatch.Start();
      // create products
      for (int i = 0; i < nProducts; i++) {
        string name = RandomName();
        List<PluginDescription> prodPlugins = new List<PluginDescription>();
        for (int j = 0; j < avgProductPlugins; j++) {
          var selectedPlugin = plugins[r.Next(0, plugins.Count)];
          if (!prodPlugins.Contains(selectedPlugin)) prodPlugins.Add(selectedPlugin);
        }
        var prod = new ProductDescription(name, vers01, prodPlugins);
        store.Persist(prod);
      }
      stopWatch.Stop();
      Assert.Inconclusive("Created " + nProducts + " products in " + stopWatch.ElapsedMilliseconds +
        " ms (" + (double)stopWatch.ElapsedMilliseconds / nProducts + " ms / product )");
    }

    private string RandomName() {
      return Guid.NewGuid().ToString();
    }
  }
}
