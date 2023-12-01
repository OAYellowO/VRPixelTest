#if UNITY_ANDROID && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using UnityEngine;

namespace Unity.XR.OpenXR.Features.PICOSupport
{
    public class PLog
    {
        public static string TAG = "[PoxrUnity] ";
        //   7--all print, 4--info to fatal, 3--warning to fatal,
        //   2--error to fatal, 1--only fatal print
        public static LogLevel logLevel = LogLevel.LogInfo;

        public enum LogLevel
        {
            LogFatal = 1,
            LogError = 2,
            LogWarn = 3,
            LogInfo = 4,
            LogDebug = 5,
            LogVerbose,
        }

        public static void v(string message)
        {
            if (LogLevel.LogVerbose <= logLevel)
                Debug.Log(string.Format("{0} FrameID={1}>>>>>>{2}", TAG, Time.frameCount, message));
        }

        public static void d(string message)
        {
            if (LogLevel.LogDebug <= logLevel)
                Debug.Log(string.Format("{0} FrameID={1}>>>>>>{2}", TAG, Time.frameCount, message));
        }

        public static void i(string message)
        {
            if (LogLevel.LogInfo <= logLevel)
                Debug.Log(string.Format("{0} FrameID={1}>>>>>>{2}", TAG, Time.frameCount, message));
        }

        public static void w(string message)
        {
            if (LogLevel.LogWarn <= logLevel)
                Debug.LogWarning(string.Format("{0} FrameID={1}>>>>>>{2}", TAG, Time.frameCount, message));
        }

        public static void e(string message)
        {
            if (LogLevel.LogError <= logLevel)
                Debug.LogError(string.Format("{0} FrameID={1}>>>>>>{2}", TAG, Time.frameCount, message));
        }

        public static void f(string message)
        {
            if (LogLevel.LogFatal <= logLevel)
                Debug.LogError(string.Format("{0} FrameID={1}>>>>>>{2}", TAG, Time.frameCount, message));
        }
    }
}