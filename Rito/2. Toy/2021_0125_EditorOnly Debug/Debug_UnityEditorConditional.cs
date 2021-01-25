using System;
using UnityEngine;
using System.Diagnostics;

using Object = UnityEngine.Object;

// 날짜 : 2021-01-25 PM 2:45:53
// 작성자 : Rito
namespace Rito
{
    /// <summary> 에디터 전용 디버그 래퍼 클래스 </summary>
    public static class Debug
    {
        /***********************************************************************
        *                               Properties
        ***********************************************************************/
        #region .
        public static ILogger logger => UnityEngine.Debug.unityLogger;
        public static ILogger unityLogger => UnityEngine.Debug.unityLogger;
        public static bool developerConsoleVisible
        {
            get => UnityEngine.Debug.developerConsoleVisible;
            set => UnityEngine.Debug.developerConsoleVisible = value;
        }
        public static bool isDebugBuild => UnityEngine.Debug.isDebugBuild;

        #endregion
        /***********************************************************************
        *                               Mark
        ***********************************************************************/
        #region .
        /// <summary> 메소드 호출 전파 추적용 메소드 </summary>
        [Conditional("UNITY_EDITOR")]
        public static void Mark(
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
        )
        {
            int begin = sourceFilePath.LastIndexOf(@"\");
            int end = sourceFilePath.LastIndexOf(@".cs");
            string className = sourceFilePath.Substring(begin + 1, end - begin - 1);

            UnityEngine.Debug.Log($"[Mark] {className}.{memberName}, {sourceLineNumber}");
        }

        #endregion
        /***********************************************************************
        *                               Assert
        ***********************************************************************/
        #region .

        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, string message, Object context)
            => UnityEngine.Debug.Assert(condition, message, context);

        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition)
            => UnityEngine.Debug.Assert(condition);

        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, object message, Object context)
            => UnityEngine.Debug.Assert(condition, message, context);

        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, string message)
            => UnityEngine.Debug.Assert(condition, message);

        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, object message)
            => UnityEngine.Debug.Assert(condition, message);

        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, Object context)
            => UnityEngine.Debug.Assert(condition, context);


        [Conditional("UNITY_EDITOR")]
        public static void AssertFormat(bool condition, Object context, string format, params object[] args)
            => UnityEngine.Debug.AssertFormat(condition, context, format, args);

        [Conditional("UNITY_EDITOR")]
        public static void AssertFormat(bool condition, string format, params object[] args)
            => UnityEngine.Debug.AssertFormat(condition, format, args);

        #endregion
        /***********************************************************************
        *                               Log
        ***********************************************************************/
        #region .

        [Conditional("UNITY_EDITOR")]
        public static void Log(object message)
            => UnityEngine.Debug.Log(message);

        [Conditional("UNITY_EDITOR")]
        public static void Log(object message, Object context)
            => UnityEngine.Debug.Log(message, context);

        [Conditional("UNITY_EDITOR")]
        public static void LogFormat(string format, params object[] args)
            => UnityEngine.Debug.LogFormat(format, args);

        [Conditional("UNITY_EDITOR")]
        public static void LogFormat(Object context, string format, params object[] args)
            => UnityEngine.Debug.LogFormat(context, format, args);

        [Conditional("UNITY_EDITOR")]
        public static void LogFormat(LogType logType, LogOption logOptions, Object context, string format, params object[] args)
            => UnityEngine.Debug.LogFormat(logType, logOptions, context, format, args);

        #endregion
        /***********************************************************************
        *                               LogAssertion
        ***********************************************************************/
        #region .

        [Conditional("UNITY_EDITOR")]
        public static void LogAssertion(object message, Object context)
            => UnityEngine.Debug.LogAssertion(message, context);

        [Conditional("UNITY_EDITOR")]
        public static void LogAssertion(object message)
            => UnityEngine.Debug.LogAssertion(message);

        [Conditional("UNITY_EDITOR")]
        public static void LogAssertionFormat(Object context, string format, params object[] args)
            => UnityEngine.Debug.LogAssertionFormat(context, format, args);

        [Conditional("UNITY_EDITOR")]
        public static void LogAssertionFormat(string format, params object[] args)
            => UnityEngine.Debug.LogAssertionFormat(format, args);

        #endregion
        /***********************************************************************
        *                               LogWarning
        ***********************************************************************/
        #region .

        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(object message, Object context)
            => UnityEngine.Debug.LogWarning(message, context);

        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(object message)
            => UnityEngine.Debug.LogWarning(message);

        [Conditional("UNITY_EDITOR")]
        public static void LogWarningFormat(Object context, string format, params object[] args)
            => UnityEngine.Debug.LogWarningFormat(context, format, args);

        [Conditional("UNITY_EDITOR")]
        public static void LogWarningFormat(string format, params object[] args)
            => UnityEngine.Debug.LogWarningFormat(format, args);

        #endregion
        /***********************************************************************
        *                               LogError
        ***********************************************************************/
        #region .

        [Conditional("UNITY_EDITOR")]
        public static void LogError(object message, Object context)
            => UnityEngine.Debug.LogError(message, context);

        [Conditional("UNITY_EDITOR")]
        public static void LogError(object message)
            => UnityEngine.Debug.LogError(message);

        [Conditional("UNITY_EDITOR")]
        public static void LogErrorFormat(Object context, string format, params object[] args)
            => UnityEngine.Debug.LogErrorFormat(context, format, args);

        [Conditional("UNITY_EDITOR")]
        public static void LogErrorFormat(string format, params object[] args)
            => UnityEngine.Debug.LogErrorFormat(format, args);

        #endregion
        /***********************************************************************
        *                               LogException
        ***********************************************************************/
        #region .

        [Conditional("UNITY_EDITOR")]
        public static void LogException(Exception exception)
            => UnityEngine.Debug.LogException(exception);

        [Conditional("UNITY_EDITOR")]
        public static void LogException(Exception exception, Object context)
            => UnityEngine.Debug.LogException(exception, context);

        #endregion
        /***********************************************************************
        *                               DrawLine
        ***********************************************************************/
        #region .

        [Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 start, Vector3 end)
            => UnityEngine.Debug.DrawLine(start, end);

        [Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 start, Vector3 end, Color color)
            => UnityEngine.Debug.DrawLine(start, end, color);

        [Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
            => UnityEngine.Debug.DrawLine(start, end, color, duration);

        [Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
            => UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);

        #endregion
        /***********************************************************************
        *                               DrawRay
        ***********************************************************************/
        #region .

        [Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest)
            => UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);

        [Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
            => UnityEngine.Debug.DrawRay(start, dir, color, duration);

        [Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 start, Vector3 dir, Color color)
            => UnityEngine.Debug.DrawRay(start, dir, color);

        [Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 start, Vector3 dir)
            => UnityEngine.Debug.DrawRay(start, dir);

        #endregion
        /***********************************************************************
        *                               Etc
        ***********************************************************************/
        #region .

        [Conditional("UNITY_EDITOR")]
        public static void Break()
            => UnityEngine.Debug.Break();

        [Conditional("UNITY_EDITOR")]
        public static void DebugBreak()
            => UnityEngine.Debug.DebugBreak();

        [Conditional("UNITY_EDITOR")]
        public static void ClearDeveloperConsole()
            => UnityEngine.Debug.ClearDeveloperConsole();

        #endregion
    }
}