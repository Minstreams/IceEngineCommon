using System;
using System.IO;
using System.Reflection;

namespace IceEngine.Framework
{
    namespace Internal
    {
        /// <summary>
        /// 冰屿系统运行时系统配置的基类，此类用于获取反射信息
        /// </summary>
        public abstract class IceSetting
        {

        }
    }

    /// <summary>
    /// 冰屿系统运行时系统配置的基类，文件存于Settings目录下，自动化的单例功能
    /// 配置类命名必须以Setting开头！参考<see cref="IceSystem.TypeName"/>
    /// 可以通过IceSettingPathAttribute来配置资源存储的目录
    /// </summary>
    public abstract class IceSetting<T> : Internal.IceSetting where T : Internal.IceSetting
    {
        static T _setting;
        public static T Setting
        {
            get
            {
                if (_setting == null)
                {
                    var tT = typeof(T);
                    var tName = tT.Name;

                    // 计算path
                    string filePath = "Settings";
                    var path = tT.GetCustomAttribute<IceSettingPathAttribute>()?.Path;
                    if (!string.IsNullOrEmpty(path)) filePath += $"/{path}";
                    filePath += $"/{tName}.config";

                    // 先尝试加载已有的
                    _setting = Ice.Save.Json.LoadFromFile<T>(filePath);

                    // 若没有再创建或抛异常
                    if (_setting == null)
                    {
                        // 创建目录
                        filePath.TryCreateFolderOfPath();

                        // 创建资源
                        _setting = Activator.CreateInstance<T>();
                        Ice.Save.Json.SaveToFile(_setting, filePath, false, true);
                        Debug.Log($"Create {tName} to {filePath}");
                    }
                    else
                    {
                        Debug.Log($"Load {tName} from {filePath}");
                    }
                }

                return _setting;
            }
        }
        public void Save()
        {
            var tT = typeof(T);
            var tName = tT.Name;

            string filePath = "Settings";
            var path = tT.GetCustomAttribute<IceSettingPathAttribute>()?.Path;
            if (!string.IsNullOrEmpty(path)) filePath += $"/{path}";
            filePath += $"/{tName}.config";

            Ice.Save.Json.SaveToFile(this, filePath, false, true);
            Debug.Log($"Save {tName} to {filePath}");
        }
    }
}