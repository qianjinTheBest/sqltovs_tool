using MySql.Data.MySqlClient;
using mysqldriver.mysql_tool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using tool_common;

namespace mysqldriver
{
    /// <summary>
    /// mysql驱动
    /// </summary>
    public class MySqlDriverHelper
    {
        #region 静态参数
        static string _connstr = ConfigHelper.GetCommonSettingsSingleValue("MySqlConnecting");
        #endregion 静态参数

        #region 查询 
        /// <summary>
        /// 查询，列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="insertSql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IList<T> GetEntityList<T>(string selectSql, MySqlDBEnum dataBaseEnum, Dictionary<string, object> cmdParms) where T : new()
        {
            _connstr = GetBaseConnectString(dataBaseEnum);
            List<T> entity = new List<T>();
            DataSet dataSet = null;
            MySqlCommand cmd = new MySqlCommand();
            MySqlConnection conn = new MySqlConnection(_connstr);
            try
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, selectSql, cmdParms);
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                adapter.SelectCommand = cmd;
                dataSet = new DataSet();
                adapter.Fill(dataSet);
                cmd.Parameters.Clear();
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    foreach (DataTable item in dataSet.Tables)
                    {
                        entity.AddRange(DataTableToList<T>(item));
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteException(ex, "selectSql = " + selectSql);
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return entity;
        }

        /// <summary>
        /// 查询单个实体数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selectSql"></param>
        /// <param name="dataBaseEnum"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static T GetEntity<T>(string selectSql, MySqlDBEnum dataBaseEnum, Dictionary<string, object> cmdParms) where T : new()
        {
            _connstr = GetBaseConnectString(dataBaseEnum);
            T entity = new T();
            DataTable dt;
            MySqlCommand cmd = new MySqlCommand();
            MySqlConnection conn = new MySqlConnection(_connstr);
            try
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, selectSql, cmdParms);
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                adapter.SelectCommand = cmd;
                dt = new DataTable();
                adapter.Fill(dt);
                cmd.Parameters.Clear();
                if (dt != null && dt.Rows.Count > 0) entity = DataTableToEntity<T>(dt);
                return entity;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteException(ex, "selectSql = " + selectSql);
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return entity;
        }
        /// <summary>
        /// 获取单个字段数值
        /// </summary>
        /// <param name="selectSql"></param>
        /// <param name="dataBaseEnum"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static string GetScalarValue(string selectSql, MySqlDBEnum dataBaseEnum, Dictionary<string, object> cmdParms)
        {
            _connstr = GetBaseConnectString(dataBaseEnum);
            string entity = string.Empty;

            MySqlCommand cmd = new MySqlCommand();
            MySqlConnection conn = new MySqlConnection(_connstr);
            try
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, selectSql, cmdParms);

                object entityScalar = cmd.ExecuteScalar();
                entity = entityScalar.ToString();
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteException(ex, "selectSql = " + selectSql);
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return entity;
        }
        #endregion 查询
        #region 添加
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="insertSql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static bool AddEntity(string insertSql, MySqlDBEnum dataBaseEnum, Dictionary<string, object> cmdParms)
        {
            _connstr = GetBaseConnectString(dataBaseEnum);
            int entityrows = 0;//影响行数

            MySqlCommand cmd = new MySqlCommand();
            MySqlConnection conn = new MySqlConnection(_connstr);
            try
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, insertSql, cmdParms);

                entityrows = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteException(ex, "insertSql = " + insertSql);
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return entityrows > 0;
        }
        /// <summary>
        /// 新增，带事务
        /// </summary>
        /// <param name="varTrans"></param>
        /// <param name="conn"></param>
        /// <param name="insertSql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static bool AddEntityTransaction(MySqlTransaction varTrans, MySqlConnection conn, string insertSql, Dictionary<string, object> cmdParms)
        {
            int entityrows = 0;//影响行数
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                PrepareCommand(cmd, conn, varTrans, CommandType.Text, insertSql, cmdParms);

                entityrows = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteException(ex, "insertSql = " + insertSql);
            }
            finally
            {
                cmd.Dispose();
            }
            return entityrows > 0;
        }
        /// <summary>
        /// 新增
        /// 完整实体类插入，返回主键。
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="dataBaseEnum">数据库枚举</param>
        /// <returns></returns>
        public static long AddEntityBackId<T>(T entity, MySqlDBEnum dataBaseEnum)
        {
            _connstr = GetBaseConnectString(dataBaseEnum);
            long entityid = 0;
            string insertSql = string.Empty;
            Dictionary<string, object> cmdParms = new Dictionary<string, object>();

            MySqlCommand cmd = new MySqlCommand();
            MySqlConnection conn = new MySqlConnection(_connstr);
            try
            {
                cmdParms = GetAddEntitySql(entity, out insertSql);
                PrepareCommand(cmd, conn, null, CommandType.Text, insertSql, cmdParms);

                object id = cmd.ExecuteScalar();
                //cmd.ExecuteNonQuery();
                //object id = cmd.LastInsertedId;
                entityid = DataConvertHelper.ConvertObjectToInt64(id);
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteException(ex, "insertSql = " + insertSql);
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return entityid;
        }
        /// <summary>
        /// 新增,带事务
        /// 完整实体类插入，返回主键。
        /// </summary>
        /// <param name="varTrans">事务</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public static long AddEntityBackIdTransaction<T>(MySqlTransaction varTrans, MySqlConnection conn, T entity)
        {
            long entityid = 0;
            string insertSql = string.Empty;
            Dictionary<string, object> cmdParms = new Dictionary<string, object>();

            cmdParms = GetAddEntitySql(entity, out insertSql);
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                PrepareCommand(cmd, conn, varTrans, CommandType.Text, insertSql, cmdParms);

                object id = cmd.ExecuteScalar();
                entityid = DataConvertHelper.ConvertObjectToInt64(id);
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteException(ex, "insertSql = " + insertSql);
            }
            finally
            {
                cmd.Dispose();
            }
            return entityid;
        }
        /// <summary>
        /// 批量插入  https://www.jb51.net/article/104210.htm  暂时不支持
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityList"></param>
        /// <param name="dataBaseEnum"></param>
        private static void AddEntityList<T>(List<T> entityList, MySqlDBEnum dataBaseEnum)
        {
            _connstr = GetBaseConnectString(dataBaseEnum);
            if (entityList == null || !entityList.Any()) return;
            var t = entityList[0];


            using (MySqlConnection conn = new MySqlConnection(_connstr))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                da.TableMappings.Add("Table", t.GetType().Name);
                DataTable entityTB = ListToDataTable(entityList);
                da.Fill(entityTB);

                try
                {

                    MySqlCommandBuilder cmdBuilder = new MySqlCommandBuilder(da);

                }
                catch (Exception ex)
                {
                    LogHelper.Instance.WriteException(ex, "AddEntityList,异常：Table_Name=" + t.GetType().Name);
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        #endregion 添加
        #region 修改
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="updateSql"></param>
        /// <param name="dataBaseEnum"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static bool UdpEntity(string updateSql, MySqlDBEnum dataBaseEnum, Dictionary<string, object> cmdParms)
        {
            _connstr = GetBaseConnectString(dataBaseEnum);
            bool isUdp = false;

            MySqlCommand cmd = new MySqlCommand();
            MySqlConnection conn = new MySqlConnection(_connstr);
            try
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, updateSql, cmdParms);

                isUdp = cmd.ExecuteNonQuery() > 0;
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteException(ex, "updateSql = " + updateSql);
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return isUdp;
        }
        /// <summary>
        /// 更新数据  事务
        /// </summary>
        /// <param name="varTrans"></param>
        /// <param name="conn"></param>
        /// <param name="updateSql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static bool UdpEntityTransaction(MySqlTransaction varTrans, MySqlConnection conn, string updateSql, Dictionary<string, object> cmdParms)
        {
            bool isUdp = false;

            MySqlCommand cmd = new MySqlCommand();
            try
            {
                PrepareCommand(cmd, conn, varTrans, CommandType.Text, updateSql, cmdParms);

                isUdp = cmd.ExecuteNonQuery() > 0;
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteException(ex, "updateSql = " + updateSql);
            }
            finally
            {
                cmd.Dispose();
            }
            return isUdp;
        }

        #endregion 修改
        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="deleteSql"></param>
        /// <param name="dataBaseEnum"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static bool DelEntity(string deleteSql, MySqlDBEnum dataBaseEnum, Dictionary<string, object> cmdParms)
        {
            _connstr = GetBaseConnectString(dataBaseEnum);
            bool isDel = false;

            MySqlCommand cmd = new MySqlCommand();
            MySqlConnection conn = new MySqlConnection(_connstr);
            try
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, deleteSql, cmdParms);

                isDel = cmd.ExecuteNonQuery() > 0;
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteException(ex, "deleteSql = " + deleteSql);
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return isDel;
        }
        /// <summary>
        /// 删除 事务
        /// </summary>
        /// <param name="deleteSql"></param>
        /// <param name="dataBaseEnum"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static bool DelEntityTransaction(MySqlTransaction varTrans, MySqlConnection conn, string deleteSql, Dictionary<string, object> cmdParms)
        {
            bool isDel = false;

            MySqlCommand cmd = new MySqlCommand();
            try
            {
                PrepareCommand(cmd, conn, varTrans, CommandType.Text, deleteSql, cmdParms);

                isDel = cmd.ExecuteNonQuery() > 0;
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteException(ex, "deleteSql = " + deleteSql);
            }
            finally
            {
                cmd.Dispose();
            }
            return isDel;
        }
        #endregion 删除

        #region 事务处理
        public static MySqlConnection OpenMySqlConnection(MySqlDBEnum dataBaseEnum)
        {
            MySqlConnection conn = new MySqlConnection(GetBaseConnectString(dataBaseEnum));
            conn.Open();
            return conn;
        }
        /// <summary>
        /// 开始构建事务
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="dataBaseEnum"></param>
        /// <returns></returns>
        public static MySqlTransaction BeginTransaction(MySqlConnection conn)
        {
            return conn.BeginTransaction();
        }
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="conn"></param>
        public static void CommitTransaction(MySqlTransaction tx, MySqlConnection conn)
        {
            tx.Commit();
            conn.Close();
            conn.Dispose();
        }
        /// <summary>
        /// 回滚
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="conn"></param>
        public static void RollbackTransaction(MySqlTransaction tx, MySqlConnection conn)
        {
            tx.Rollback();
            conn.Close();
            conn.Dispose();
        }
        #endregion 事务处理

        #region 获取数据库连接
        private static string GetBaseConnectString(MySqlDBEnum dataBaseEnum)
        {
            _connstr = GetConnectString(dataBaseEnum);
            return _connstr;
        }
        private static string GetConnectString(MySqlDBEnum dataBaseEnum)
        {
            string _conn = string.Empty;
            switch (dataBaseEnum)
            {
                case MySqlDBEnum.MySqlConnecting:
                    _conn = ConfigHelper.GetCommonSettingsSingleValue("MySqlConnecting");
                    break;
                default://默认app后台
                    _conn = ConfigHelper.GetCommonSettingsSingleValue("MySqlConnecting");
                    break;
            }
            return _conn;
        }
        /// <summary>
        /// 执行数据库命令前的准备工作  
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, CommandType cmdType, string cmdText, Dictionary<string, object> param)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;
            cmd.CommandTimeout = 100;

            if (param != null)
            {
                foreach (var item in param)
                {
                    if (item.Value == null || item.Value as string == "")
                    {
                        cmd.Parameters.Add(new MySqlParameter("@" + item.Key, DBNull.Value));
                    }
                    else
                    {
                        cmd.Parameters.Add(new MySqlParameter("@" + item.Key, item.Value));
                    }
                }
            }
        }
        #endregion 获取数据库连接

        #region dataset转list
        /// <summary>
        /// 转化datatable到list集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static List<T> DataTableToList<T>(DataTable dt) where T : new()
        {
            // 定义集合 
            List<T> ts = new List<T>();
            //定义一个临时变量 
            string tempName = string.Empty;
            //遍历DataTable中所有的数据行 
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性 
                PropertyInfo[] propertys = t.GetType().GetProperties();
                //遍历该对象的所有属性 
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;//将属性名称赋值给临时变量 
                                       //检查DataTable是否包含此列（列名==对象的属性名）  
                    if (dt.Columns.Contains(tempName))
                    {
                        //取值 
                        object value = dr[tempName];
                        //如果非空，则赋给对象的属性 
                        if (value != DBNull.Value)
                        {
                            if (pi.PropertyType.FullName == "System.Boolean")
                            {
                                value = value.ToString() == "1" ? true : false;
                            }
                            pi.SetValue(t, value, null);
                        }
                    }
                }
                //对象添加到泛型集合中 
                ts.Add(t);
            }
            return ts;
        }
        #endregion dataset转list
        #region list 转datatable
        /// <summary>
        /// list 转datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        private static DataTable ListToDataTable<T>(List<T> items)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (items.Count() > 0)
            {
                for (int i = 0; i < items.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(items.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }

        #endregion list 转datatable
        #region datatable转entity
        /// <summary>
        /// 转化datatable到实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static T DataTableToEntity<T>(DataTable dt) where T : new()
        {
            T entity = new T();
            PropertyInfo[] pilist = entity.GetType().GetProperties();
            foreach (PropertyInfo pi in pilist)
            {
                string tempName = pi.Name;
                // 检查DataTable是否包含此列
                if (dt.Columns.Contains(tempName))
                {
                    // 判断此属性是否有Setter
                    if (!pi.CanWrite) continue;
                    object value = null;
                    if (pi.PropertyType.FullName == "System.String")
                    {
                        value = dt.Rows[0][tempName].ToString();
                    }
                    else
                    {
                        value = dt.Rows[0][tempName];
                    }
                    if (value != DBNull.Value)
                    {
                        if (pi.PropertyType.FullName == "System.Boolean")
                        {
                            value = value.ToString() == "1" ? true : false;
                        }
                        pi.SetValue(entity, value, null);
                    }
                }
            }
            return entity;
        }
        #endregion datatable转entity
        #region entity 转化insert sql
        /// <summary>
        /// 实体类解析sql参数数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="insertsql"></param>
        /// <returns></returns>
        private static Dictionary<string, object> GetAddEntitySql<T>(T t, out string insertsql)
        {
            insertsql = $" insert into {t.GetType().Name} (";
            string valuesql = " values (";
            Dictionary<string, object> param = new Dictionary<string, object>();
            PropertyInfo[] pilist = t.GetType().GetProperties();
            foreach (PropertyInfo pi in pilist)
            {
                if (pi.Name.ToUpper() == "ID") continue;//主键跳过（自增）
                insertsql = insertsql + $"{pi.Name}" + ",";
                valuesql = valuesql + $"@{pi.Name}" + ",";
                param.Add($"{pi.Name}", pi.GetValue(t, null));
            }
            insertsql = insertsql.Trim(',');
            valuesql = valuesql.Trim(',');
            insertsql = insertsql + ")" + valuesql + ") ;select @@identity";

            return param;
        }
        #endregion  entity 转化insert sql
    }
}