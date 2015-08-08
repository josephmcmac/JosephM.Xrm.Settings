using System;

namespace JosephM.Xrm.Settings
{
    public abstract class XrmWorkflowActivityInstance<T> : XrmWorkflowActivityInstanceBase
        where T : XrmWorkflowActivityRegistration
    {
        public T Arguments
        {
            get
            {
                if (Activity is T)
                    return (T)Activity;
                throw new Exception(string.Format("Activity Should Be Of Type {0}", typeof(T).Name));
            }
        }
    }
}