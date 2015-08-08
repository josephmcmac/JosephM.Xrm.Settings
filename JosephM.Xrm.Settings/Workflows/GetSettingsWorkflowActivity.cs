using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Schema;

namespace JosephM.Xrm.Settings.Workflows
{
    public class GetSettingsWorkflowActivity : XrmWorkflowActivityRegistration
    {
        [Output("Settings")]
        [ReferenceTarget(Entities.jmcg_settings)]
        public OutArgument<EntityReference> Settings { get; set; }

        protected override XrmWorkflowActivityInstanceBase CreateInstance()
        {
            return new GetSettingsWorkflowActivityInstance();
        }
    }

    public class GetSettingsWorkflowActivityInstance : SettingsWorkflowActivityInstance<GetSettingsWorkflowActivity>
    {
        protected override void Execute()
        {
            var settings = GetTheSettings();
            Arguments.Settings.Set(ExecutionContext, settings.ToEntityReference());
        }

        public Entity GetTheSettings()
        {
            var settings = XrmService.GetFirst(Entities.jmcg_settings, new string[0]);
            if (settings == null)
                throw new NullReferenceException(
                    string.Format("Error no {0} record has been created or you do not have permission to read it",
                        XrmService.GetEntityLabel(Entities.jmcg_settings)));
            return settings;
        }
    }
}
