namespace tools_sqltonet.tool_models
{
    /// <summary>
    /// 行模型：表字段模型
    /// </summary>
    public class RowModel
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string rowname { get; set; }
        /// <summary>
        /// 字段类型   如：bigint
        /// </summary>
        public string rowtype { get; set; }
        /// <summary>
        /// 字段注释
        /// </summary>
        public string rownote { get; set; }
    }
    /// <summary>
    /// 类字段模型
    /// </summary>
    public class CSRowModel
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string rowname { get; set; }
        /// <summary>
        /// 字段类型   如：long
        /// </summary>
        public string rowtype { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string rowvalue { get; set; }
        /// <summary>
        /// 字段注释
        /// </summary>
        public string rownote { get; set; }
    }
}
