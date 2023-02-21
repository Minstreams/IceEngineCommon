using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using IceEngine;
using IceEngine.Internal;
using IceEngine.Framework.Internal;
using System.Linq.Expressions;

// Ice命名空间内只有所有子系统静态类，这样设计有助于运行时代码快速定位到子系统
namespace Ice
{
    /// <summary>
    /// 冰屿，IceEngine的核心
    /// </summary>
    public static class Island
    {
        #region 全局实例对象
        /// <summary>
        /// 系统配置
        /// </summary>
        public static SettingGlobal Setting => SettingGlobal.Setting;
        #endregion

        #region 子系统控制
        /// <summary>
        /// 子系统表
        /// </summary>
        public static List<Type> SubSystemList
        {
            get
            {
                if (_subsystemList is null)
                {
                    _subsystemList = new List<Type>();
                    static void CollectSubSystemFromAssembly(Assembly a) => _subsystemList.AddRange(a.GetTypes().Where(t => !t.IsGenericType && t.IsSubclassOf(typeof(IceSystem))));

                    var iceAssembly = typeof(IceSystem).Assembly;
                    CollectSubSystemFromAssembly(iceAssembly);

                    var iceName = iceAssembly.GetName().Name;
                    foreach (var a in AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetReferencedAssemblies().Any(a => a.Name == iceName))) CollectSubSystemFromAssembly(a);
                }
                return _subsystemList;
            }
        }
        static List<Type> _subsystemList;


        static Dictionary<string, Action> _subsystemActionCache = new();
        /// <summary>
        /// 调用所有子系统上存在的同名静态方法
        /// </summary>
        public static void CallSubSystem(string methodName)
        {
            if (!_subsystemActionCache.TryGetValue(methodName, out Action action))
            {
                List<Expression> callList = new();
                foreach (var s in SubSystemList)
                {
                    MethodInfo m = s.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    if (m != null)
                    {
                        callList.Add(Expression.Call(m));
                    }
                }
                action = Expression.Lambda<Action>(Expression.Block(callList)).Compile();
                _subsystemActionCache.Add(methodName, action);
            }
            action();
        }
        #endregion
       
        #region Debug
        /// <summary>
        /// 全局Log事件
        /// </summary>
        public static event Action<string> LogAction;
        static string _ProcessLog(object message, string mid, string prefix)
        {
            int skipFrames = 3;
            if (string.IsNullOrEmpty(prefix))
            {
                prefix = $"【{"IceIsland"}】";
                skipFrames = 2;
            }
            string log = $"{prefix}{mid}{message}";
            LogAction?.Invoke($"{log}\n\n{new System.Diagnostics.StackTrace(skipFrames, true)}");
            return log;
        }
        [System.Diagnostics.Conditional("DEBUG")]
        public static void Log(object message, object context = null, string prefix = null)
        {
            var log = _ProcessLog(message, null, prefix);
#if UNITY_EDITOR
            if (HideLog) return;
#endif
            if (context == null) Debug.Log(log);
            else Debug.Log(log, context);
        }
        public static void LogImportant(object message, object context = null, string prefix = null)
        {
            var log = _ProcessLog(message, "[<color=#0FF>Important</color>]", prefix);

            if (context == null) Debug.Log(log);
            else Debug.Log(log, context);
        }
        public static void LogWarning(object message, object context = null, string prefix = null)
        {
            var log = _ProcessLog(message, "[<color=#FC0>Warning</color>]", prefix);

            if (context == null) Debug.LogWarning(log);
            else Debug.LogWarning(log, context);
        }
        public static void LogError(object message, object context = null, string prefix = null)
        {
            var log = _ProcessLog(message, "[<color=#F00>Error</color>]", prefix);

            if (context == null) Debug.LogError(log);
            else Debug.LogError(log, context);
        }
        #endregion
    }
}
