using JosephM.ObjectEncryption;
using JosephM.Record.Application.Dialog;

namespace JosephM.Xrm.Settings.EncryptXrmConfiguration.Module
{
    public class EncryptXrmConfigurationDialog : ObjectEncryptDialog<XrmConfiguration>
    {
        public EncryptXrmConfigurationDialog(IDialogController dialogController) : base(dialogController)
        {
        }
    }
}
