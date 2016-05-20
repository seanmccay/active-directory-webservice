using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteWindowsAgent
{
    public interface IReturnValueField
    {
        string GetValue();
        string GetFieldName();
    }
}
