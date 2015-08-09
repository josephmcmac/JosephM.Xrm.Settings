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
        protected override IEnumerable<string> EntitiesToDelete
        {
            get { return base.EntitiesToDelete; }
        }
    }
}