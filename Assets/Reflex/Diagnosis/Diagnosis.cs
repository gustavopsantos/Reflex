using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Reflex
{
    internal static class Diagnosis
    {
        public static List<CallSite> GetCallSite(int skipFrames)
        {
            var result = new List<CallSite>();
            var stackTrace = new StackTrace(skipFrames, true);
            var frames = stackTrace.GetFrames();

            foreach (var frame in frames.Where(f => f.GetFileName() != null))
            {
                var methodName = frame.GetMethod()?.Name;
                var className = frame.GetMethod()?.DeclaringType?.FullName;
                var lineNumber = frame.GetFileLineNumber();
                var filePath = GetUnityPath(frame.GetFileName());
                result.Add(new CallSite(className, methodName, filePath, lineNumber));
            }

            return result;
        }

        private static string GetUnityPath(string path)
        {
            return path.Replace("\\", "/").Replace(Application.dataPath, "Assets");
        }
    }
}