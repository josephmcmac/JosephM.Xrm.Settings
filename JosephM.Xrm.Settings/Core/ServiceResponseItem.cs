﻿#region

using System;

#endregion

namespace JosephM.Xrm.Settings.Core
{
    public class ServiceResponseItem
    {
        public bool HasError
        {
            get { return Exception != null; }
        }

        public Exception Exception { get; set; }

        public string ExceptionStackTrace
        {
            get { return Exception == null ? null : Exception.StackTrace; }
        }
    }
}