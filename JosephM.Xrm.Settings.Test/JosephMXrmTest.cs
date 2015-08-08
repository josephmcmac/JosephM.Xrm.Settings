using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using JosephM.Xrm.Settings.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JosephM.Xrm.Settings.Test
{
    public abstract class JosephMXrmTest : XrmTest
    {
        public override IXrmConfiguration XrmConfiguration
        {
            get
            {
                var readEncryptedFonfig = File.ReadAllText("XrmConfiguration.xml");
                var decrypt = StringEncryptor.Decrypt(readEncryptedFonfig);
                var deserialiser = new DataContractSerializer(typeof (XrmConfiguration));
                var byteArray = Encoding.UTF8.GetBytes(decrypt);
                using (var stream = new MemoryStream(byteArray))
                    return (XrmConfiguration)deserialiser.ReadObject(stream);
            }
        }

        protected override IEnumerable<string> EntitiesToDelete
        {
            get { return base.EntitiesToDelete; }
        }
    }
}