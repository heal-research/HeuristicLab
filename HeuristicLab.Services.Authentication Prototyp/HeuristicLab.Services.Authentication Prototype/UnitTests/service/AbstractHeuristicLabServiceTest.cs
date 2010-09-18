using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Service.Services.Administration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.service {
  public abstract class AbstractHeuristicLabServiceTest : AbstractHeuristicLabTest{

    private ServiceHost sh;

    [TestInitialize()]
    public override void updateDBConnection() {
      base.updateDBConnection();
      sh = new ServiceHost(typeof(AuthorizationManagementService));
      sh.Open();
    }

    [TestCleanup()]
    public virtual void closeDBConnection() {
      sh.Close();
      base.closeDBConnection();
      
    }
  }
}
