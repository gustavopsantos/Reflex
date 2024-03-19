using System;
using UnityEngine;

namespace Reflex.PlayModeTests
{
    public class ApplicationStackTraceLogTypeScope : IDisposable
    {
        private readonly LogType _logType;
        private readonly StackTraceLogType _previousStackTraceLogType;
        
        public ApplicationStackTraceLogTypeScope(LogType logType, StackTraceLogType stackTraceLogType)
        {
            _logType = logType;
            _previousStackTraceLogType = Application.GetStackTraceLogType(logType);
            Application.SetStackTraceLogType(logType, stackTraceLogType);
        }
        
        public void Dispose()
        {
            Application.SetStackTraceLogType(_logType, _previousStackTraceLogType);
        }
    }
}