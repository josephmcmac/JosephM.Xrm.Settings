using System.Runtime.Serialization;

namespace JosephM.Xrm.Settings.Core
{
    [DataContract]
    public class RecordField : PicklistOption
    {
        public RecordField()
        {
        }

        public RecordField(string key, string value)
            : base(key, value)
        {
        }
    }
}