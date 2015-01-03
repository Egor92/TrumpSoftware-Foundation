using System;
using System.Runtime.CompilerServices;

namespace TrumpSoftware.Common.Log
{
    public interface ILogger
    {
        void EnterToMember(Type classType, [CallerMemberName] string memberName = null);
        void LeaveFromMember(Type classType, [CallerMemberName] string memberName = null);
        void WriteVariableValue(Type classType, string variableName, object value, [CallerMemberName] string memberName = null);
        void WriteLine(string messageFormat, params object[] parameters);
        void WriteDelimeter();
    }
}