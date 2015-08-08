using JosephM.Xrm.Settings.Workflows;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JosephM.Xrm.Settings.Test
{
    [TestClass]
    public class GetSettingsActivityTests : JosephMXrmTest
    {
        [TestMethod]
        public void GetSettingsActivityTest()
        {
            var workflow = CreateWorkflowInstance<GetSettingsWorkflowActivityInstance>();
            Assert.IsNotNull(workflow.GetTheSettings());
        }
    }
}