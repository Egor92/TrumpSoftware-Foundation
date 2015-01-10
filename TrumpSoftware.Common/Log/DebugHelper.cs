using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TrumpSoftware.Common.Log
{
    public static class DebugHelper
    {
        public static void EnterToMember(Type classType, [CallerMemberName] string memberName = null)
        {
            Debug.WriteLine("({0}){1} Enter to {2}.{3}", DateTime.Now.ToString("HH:mm:ss.ffff"), GetTaskIdString(), classType.Name, memberName);
        }

        public static void LeaveFromMember(Type classType, [CallerMemberName] string memberName = null)
        {
            Debug.WriteLine("({0}){1} Leave from {2}.{3}", DateTime.Now.ToString("HH:mm:ss.ffff"), GetTaskIdString(), classType.Name, memberName);
        }

        public static void WriteVariableValue(Type classType, string variableName, object value, [CallerMemberName] string memberName = null)
        {
            Debug.WriteLine("({0}){1} In method '{2}.{3}' variable {4}='{5}'", DateTime.Now.ToString("HH:mm:ss.ffff"), GetTaskIdString(), classType.Name, memberName, variableName, value);
        }

        public static void WriteLine(string messageFormat, params object[] parameters)
        {
            string message = string.Format(messageFormat, parameters);
            Debug.WriteLine("({0}) {1}", DateTime.Now.ToString("HH:mm:ss.ffff"), message);
        }

        public static void WriteDelimeter()
        {
            Debug.WriteLine("=========================================");
        }

        private static string GetTaskIdString()
        {
            var teskId = Task.CurrentId;
            return teskId != null
                ? string.Format("Task='{0}'.", teskId)
                : string.Empty;
        }
    }
}
