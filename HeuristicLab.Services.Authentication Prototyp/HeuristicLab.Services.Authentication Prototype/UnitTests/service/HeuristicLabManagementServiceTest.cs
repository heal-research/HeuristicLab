using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.service {
  /// <summary>
  /// Zusammenfassungsbeschreibung für HeuristicLabManagementServiceTest
  /// </summary>
  [TestClass]
  public class HeuristicLabManagementServiceTest : AbstractHeuristicLabServiceTest {
    

    private TestContext testContextInstance;

    /// <summary>
    ///Ruft den Textkontext mit Informationen über
    ///den aktuellen Testlauf sowie Funktionalität für diesen auf oder legt diese fest.
    ///</summary>
    public TestContext TestContext {
      get {
        return testContextInstance;
      }
      set {
        testContextInstance = value;
      }
    }

    [TestMethod()]
    public void TestCreateRoleTest() {
      new ServiceManagementRemote.AuthorizationManagementServiceClient().CreateRole("myRole", true);
    }

    
  }
}
