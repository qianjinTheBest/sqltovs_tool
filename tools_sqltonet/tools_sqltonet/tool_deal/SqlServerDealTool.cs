using sqlserverdriver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tool_common;
using tools_sqltonet.tool_models;

namespace tools_sqltonet.tool_deal
{
    public static class SqlServerDealTool
    {
        static SqlServerDriverHelper driverHelper = new SqlServerDriverHelper();
        #region 公有方法
        /// <summary>
        /// 查询指定库所有表名
        /// </summary>
        /// <param name="databasename"></param>
        /// <returns></returns>
        public static void GetAllTbNameByDataBase(string databasename)
        {
            if (string.IsNullOrEmpty(databasename)) return;
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(" use " + databasename + "");
            sql.AppendLine(" SELECT a.name tablename");
            sql.AppendLine(" FROM sysobjects AS a INNER JOIN sysindexes AS b ON a.id = b.id");
            sql.AppendLine(" WHERE (a.type = 'u') AND (b.indid IN (0, 1))");
            sql.AppendLine(" ORDER BY a.name,b.rows DESC");
            sql.AppendLine(" SELECT Name FROM Master..SysDatabases ORDER BY Name ");
            var tbmodels = driverHelper.GetEntityList<TableModel>(sql.ToString(), sqlserverdriver.sqlserver_tool.SqlServerDBEnum.SqlClientConnecting, null);
            if (tbmodels != null && tbmodels.Any())
            {
                foreach (var item in tbmodels)
                {
                    WriteTbToCs(databasename, item.tablename);
                }
            }
        }
        /// <summary>
        /// 写入单个表
        /// </summary>
        /// <param name="databasename"></param>
        /// <param name="tablename"></param>
        public static void WriteTbToCs(string databasename, string tablename)
        {
            if (string.IsNullOrEmpty(databasename) || string.IsNullOrEmpty(tablename)) return;

            List<string> writedata = new List<string>();
            writedata.Add("using System; ");
            writedata.Add("using System.Text;");
            writedata.Add("using System.Collections.Generic;  ");
            writedata.Add("using System.Data; ");
            writedata.Add("namespace " + databasename + ".Model{ ");

            //1、查询表名与注释
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(" use "+ databasename + "");
            sql.AppendLine(" SELECT top 1");
            sql.AppendLine("   tablename  = case when a.colorder=1 then d.name else '' end,");
            sql.AppendLine("   tablernote = case when a.colorder=1 then isnull(f.value,'') else '' end");
            sql.AppendLine(" FROM syscolumns a");
            sql.AppendLine(" left join systypes b");
            sql.AppendLine(" on a.xusertype=b.xusertype");
            sql.AppendLine(" inner join sysobjects d");
            sql.AppendLine(" on a.id=d.id and d.xtype='U' and d.name<>'dtproperties'");
            sql.AppendLine(" left join syscomments e");
            sql.AppendLine(" on a.cdefault=e.id");
            sql.AppendLine(" left join sys.extended_properties g");
            sql.AppendLine(" on a.id=G.major_id and a.colid=g.minor_id");
            sql.AppendLine(" left join sys.extended_properties f");
            sql.AppendLine(" on d.id=f.major_id and f.minor_id=0");
            sql.AppendLine(" where d.name=@TBName");
            sql.AppendLine(" order by a.id,a.colorder");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("TBName", tablename);
            var tbmodel = driverHelper.GetEntity<TableModel>(sql.ToString(), sqlserverdriver.sqlserver_tool.SqlServerDBEnum.SqlClientConnecting, param);
            if (tbmodel != null)
            {
                //类名和注释
                if (!string.IsNullOrEmpty(tbmodel.tablenote))
                {
                    writedata.Add("		/// <summary>");
                    writedata.Add($"		/// {tbmodel.tablenote}");
                    writedata.Add("		/// </summary>");
                }
                writedata.Add($"		public class {tbmodel.tablename}");
                writedata.Add("		{");
            }

            
            //2、查询表明细数据
            sql.Clear();
            sql.AppendLine(" use " + databasename + "");
            sql.AppendLine(" SELECT");
            sql.AppendLine("   rowname = a.name,");
            sql.AppendLine("   rowtype = b.name,");
            sql.AppendLine("   rownote = isnull(g.[value],'')");
            sql.AppendLine(" FROM syscolumns a");
            sql.AppendLine(" left join systypes b");
            sql.AppendLine(" on a.xusertype=b.xusertype");
            sql.AppendLine(" inner join sysobjects d");
            sql.AppendLine(" on a.id=d.id and d.xtype='U' and d.name<>'dtproperties'");
            sql.AppendLine(" left join syscomments e");
            sql.AppendLine(" on a.cdefault=e.id");
            sql.AppendLine(" left join sys.extended_properties g");
            sql.AppendLine(" on a.id=G.major_id and a.colid=g.minor_id");
            sql.AppendLine(" left join sys.extended_properties f");
            sql.AppendLine(" on d.id=f.major_id and f.minor_id=0");
            sql.AppendLine(" where d.name=@TBName");
            sql.AppendLine(" order by a.id,a.colorder");

            var rowmodels = driverHelper.GetEntityList<RowModel>(sql.ToString(), sqlserverdriver.sqlserver_tool.SqlServerDBEnum.SqlClientConnecting, param);
            if (rowmodels != null && rowmodels.Any())
            {
                var csrowmodel = GetStaticAttribute(rowmodels.ToList());
                //构造函数
                if (tbmodel != null&& !string.IsNullOrEmpty(tbmodel.tablename))
                {
                    writedata.Add("		#region 构造函数");
                    writedata.Add($"		public {tbmodel.tablename}()");
                    writedata.Add("		{");
                    foreach (var row_1 in csrowmodel)
                    {
                        writedata.Add($"		this._{row_1.rowname.ToLower()}={row_1.rowvalue};");
                    }
                    writedata.Add("		}");
                    writedata.Add("		#endregion");
                }
                writedata.Add("		");
                //私有成员
                writedata.Add("		#region 成员");
                foreach (var row_1 in csrowmodel)
                {
                    writedata.Add($"		private {row_1.rowtype} _{row_1.rowname.ToLower()};");
                }
                writedata.Add("		#endregion");
                writedata.Add("		");
                //属性
                writedata.Add("		#region 属性");
                foreach (var row_1 in csrowmodel)
                {
                    //字段名和注释
                    if (!string.IsNullOrEmpty(row_1.rownote))
                    {
                        writedata.Add("		/// <summary>");
                        writedata.Add($"		/// {row_1.rownote}");
                        writedata.Add("		/// </summary>");
                    }
                    writedata.Add($"		public {row_1.rowtype} {row_1.rowname}");
                    writedata.Add("		{");
                    writedata.Add("		get{ return _"+ row_1.rowname.ToLower() + ";}");
                    writedata.Add("		set{ _" + row_1.rowname.ToLower() + "=value; }");
                    writedata.Add("		}");
                }
                writedata.Add("		#endregion");
            }

            writedata.Add("		}");
            writedata.Add("}");
            //3、组装
            if (writedata.Any())
            {
                WriteCSHelper.Instance.DeleteCS(tablename);//删除存在的
                foreach (var item in writedata)
                {
                    WriteCSHelper.Instance.WriteCS(tablename, item);
                }
            }
        }
        #endregion

        #region 私有处理方法
        /// <summary>
        /// 根据字段类型转化net的类型
        /// </summary>
        /// <param name="rowtype"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetRowValueByAttribute(string rowtype)
        {
            byte s = 0;
            Dictionary<string, string> param = new Dictionary<string, string>();
            switch (rowtype)
            {
                case "bigint":
                    param.Add("long","0");
                    break;
                case "smallint":
                    param.Add("short", "0");
                    break;
                case "real":
                    param.Add("Single", "0");
                    break;
                case "decimal":
                case "money":
                case "numeric":
                case "smallmoney":
                    param.Add("Decimal", "0");
                    break;
                case "float":
                    param.Add("Double", "0");
                    break;
                case "int":
                    param.Add("int", "0");
                    break;
                case "binary":
                case "image":
                case "varbinary":
                    param.Add("byte[]", "new byte[1]");
                    break;
                case "bit":
                    param.Add("bool", "false");
                    break;
                case "tinyint":
                    param.Add("byte", "0");
                    break;
                case "date":
                case "datetime":
                case "smalldatetime":
                case "timestamp":
                    param.Add("DateTime", "DateTime.Now");
                    break;
                default:
                    param.Add("string", "string.Empty");
                    break;
            }
            return param;
        }
        private static List<CSRowModel> GetStaticAttribute(List<RowModel> rowmodels)
        {
            List<CSRowModel> result = new List<CSRowModel>();
            foreach (var item in rowmodels)
            {
                string dic_key = string.Empty;
                string dic_val = string.Empty;
                var itemdics = GetRowValueByAttribute(item.rowtype);
                foreach (var dicitem in itemdics)
                {
                    dic_key = dicitem.Key;
                    dic_val = dicitem.Value;
                }
                result.Add(new CSRowModel()
                {
                    rowname = item.rowname,
                    rownote = item.rownote,
                    rowtype = dic_key,
                    rowvalue = dic_val
                }); ;
            }
            return result;
        }
        #endregion
    }
}
