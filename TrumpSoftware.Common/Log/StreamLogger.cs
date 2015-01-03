using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TrumpSoftware.Common.Log
{
    public class StreamLogger : ILogger, IDisposable
    {
        private readonly StreamWriter _streamWriter;

        public StreamLogger(StreamWriter streamWriter)
        {
            if (streamWriter == null)
                throw new ArgumentNullException("streamWriter");
            _streamWriter = streamWriter;
        }

        public void EnterToMember(Type classType, [CallerMemberName] string memberName = null)
        {
            var str = string.Format("({0}) Task='{1}'. Enter to {2}.{3}", DateTime.Now.ToString("HH:mm:ss.ffff"), Task.CurrentId, classType.Name, memberName);
            _streamWriter.WriteLine(str);
        }

        public void LeaveFromMember(Type classType, [CallerMemberName] string memberName = null)
        {
            var str = string.Format("({0}) Task='{1}'. Leave from {2}.{3}", DateTime.Now.ToString("HH:mm:ss.ffff"), Task.CurrentId, classType.Name, memberName);
            _streamWriter.WriteLine(str);
        }

        public void WriteVariableValue(Type classType, string variableName, object value, [CallerMemberName] string memberName = null)
        {
            var str = string.Format("({0}) Task='{1}'. In method '{2}.{3}' variable {4}='{5}'", DateTime.Now.ToString("HH:mm:ss.ffff"), Task.CurrentId, classType.Name, memberName, variableName, value);
            _streamWriter.WriteLine(str);
        }

        public void WriteLine(string messageFormat, params object[] parameters)
        {
            string message = string.Format(messageFormat, parameters);
            var str = string.Format("({0}) {1}", DateTime.Now.ToString("HH:mm:ss.ffff"), message);
            _streamWriter.WriteLine(str);
        }

        public void WriteDelimeter()
        {
            _streamWriter.WriteLine("=========================================");
        }

        public void Dispose()
        {
            _streamWriter.Dispose();
        }
    }
}
