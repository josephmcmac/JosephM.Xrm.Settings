using JosephM.ObjectEncryption;
using JosephM.Record.Application.Dialog;
using JosephM.Xrm.Settings.Test;

namespace JosephM.Xrm.Settings.EncryptXrmConfiguration.Module
{
    public class EncryptXrmConfigurationDialog : ObjectEncryptDialog<TestXrmConfiguration>
    {
        public EncryptXrmConfigurationDialog(IDialogController dialogController) : base(dialogController)
        {
        }
    }
}
