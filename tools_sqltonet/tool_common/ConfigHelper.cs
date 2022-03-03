using Microsoft.Extensions.Configuration;
using System.IO;

namespace tool_common
{
    /// <summary>
    /// 读取配置文件
    /// </summary>
    public sealed class ConfigHelper
    {
        private static IConfiguration CommonSettings { get; set; }
        static ConfigHelper()
        {
            CommonSettings = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory() + "/Config").AddJsonFile("CommonSettings.json").Build();
        }
        #region 公共独立配置
        /// <summary>
        /// 获取公共独立配置中独立类中的值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetCommonSettings(string name)
        {
            return CommonSettings.GetConnectionString(name);
        }
        /// <summary>
        /// 获取公共独立配置单个值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetCommonSettingsSingleValue(string key)
        {
            return CommonSettings.GetValue<string>(key);
        }
        #endregion 公共独立配置
    }
}
