using IceEngine.Framework;

namespace IceEngine.Internal
{
    /// <summary>
    /// 全局系统配置
    /// </summary>
    public class SettingGlobal : IceSetting<SettingGlobal>
    {
        public float timeMarkMinutes = 10;  // 时间戳记录间隔时长（分钟）
        public int logMaxLineCount = 2048;  // 一个log文件最大行数
        public float logMaxHours = 24;      // log文件最大小时
    }
}
