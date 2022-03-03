using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tools_sqltonet.tool_models
{
    /// <summary>
    /// 表模型
    /// </summary>
    public class TableModel
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string tablename { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public string tablenote { get; set; }
    }
}
