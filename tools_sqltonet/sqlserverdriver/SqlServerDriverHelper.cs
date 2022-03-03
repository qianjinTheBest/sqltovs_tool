using sqlserverdriver.sqlserver_tool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using tool_common;

namespace sqlserverdriver
{
    /// <summary>
    /// sqlserver驱动
    /// </summary>
    public class SqlServerDriverHelper
    {
        #region 静态参数
        static string _connstr = ConfigHelper.GetCommonSettingsSingleValue("SqlClientConnecting");
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
        public IList<T> GetEntityList<T>(string selectSql, SqlServerDBEnum dataBaseEnum, Dictionary<string, object> cmdParms) where T : new()
        {
            _connstr = GetConnectString(dataBaseEnum);
            List<T> entity = new List<T>();
            DataSet dataSet = null;
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(_connstr);
            try
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, selectSql, cmdParms);
                SqlDataAdapter adapter = new SqlDataAdapter();
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
        /// 查询，有In条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selectSql"></param>
        /// <param name="dataBaseEnum"></param>
        /// <param name="cmdParms"></param>
        /// <param name="cmdParamsIn">in条件参数，其中值为字符串如：1,2,3,a,好,4。尽量in的数据不要太多</param>
        /// <returns></returns>
        public IList<T> GetEntityListIn<T>(string selectSql, SqlServerDBEnum dataBaseEnum, Dictionary<string, object> cmdParms, Dictionary<string, string> cmdParamsIn) where T : new()
        {
            _connstr = GetConnectString(dataBaseEnum);
            List<T> entity = new List<T>();
            DataSet dataSet = null;
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(_connstr);
            try
            {
                foreach (var inParam in cmdParamsIn)
                {
                    string paramkey = "";
                    //处理值数据
                    string[] paramval = inParam.Value.Split(',');
                    for (int i = 0; i < paramval.Length; i++)
                    {
                        cmdParms.Add(inParam.Key + i.ToString(), paramval[i]);
                        paramkey = paramkey + "@" + inParam.Key + i.ToString() + ",";
                    }
                    selectSql = selectSql.Replace("@" + inParam.Key, paramkey.Trim(','));
                }
                PrepareCommand(cmd, conn, null, CommandType.Text, selectSql, cmdParms);
                SqlDataAdapter adapter = new SqlDataAdapter();
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
        public T GetEntity<T>(string selectSql, SqlServerDBEnum dataBaseEnum, Dictionary<string, object> cmdParms) where T : new()
        {
            _connstr = GetConnectString(dataBaseEnum);
            T entity = new T();
            DataTable dt;
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(_connstr);
            try
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, selectSql, cmdParms);
                SqlDataAdapter adapter = new SqlDataAdapter();
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
        public string GetScalarValue(string selectSql, SqlServerDBEnum dataBaseEnum, Dictionary<string, object> cmdParms)
        {
            _connstr = GetConnectString(dataBaseEnum);
            string entity = string.Empty;
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(_connstr);
            try
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, selectSql, cmdParms);
                SqlDataAdapter adapter = new SqlDataAdapter();
                object entityScalar = cmd.ExecuteScalar();
                entity = entityScalar == null ? "" : entityScalar.ToString();
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
        public bool AddEntity(string insertSql, SqlServerDBEnum dataBaseEnum, Dictionary<string, object> cmdParms)
        {
            _connstr = GetConnectString(dataBaseEnum);
            int entityrows = 0;//影响行数

            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(_connstr);
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
        /// <param name="varTrans">事务</param>
        /// <param name="conn">连接</param>
        /// <param name="insertSql">sql语句</param>
        /// <param name="cmdParms">参数</param>
        /// <returns></returns>
        public bool AddEntityTransaction(SqlTransaction varTrans, SqlConnection conn, string insertSql, Dictionary<string, object> cmdParms)
        {
            int entityrows = 0;//影响行数
            SqlCommand cmd = new SqlCommand();
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
        public long AddEntityBackId<T>(T entity, SqlServerDBEnum dataBaseEnum)
        {
            _connstr = GetConnectString(dataBaseEnum);
            long entityid = 0;
            string insertSql = string.Empty;
            Dictionary<string, object> cmdParms = new Dictionary<string, object>();

            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(_connstr);
            try
            {
                cmdParms = GetAddEntitySql(entity, out insertSql);
                PrepareCommand(cmd, conn, null, CommandType.Text, insertSql, cmdParms);

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
        public long AddEntityBackIdTransaction<T>(SqlTransaction varTrans, SqlConnection conn, T entity)
        {
            long entityid = 0;
            string insertSql = string.Empty;
            Dictionary<string, object> cmdParms = new Dictionary<string, object>();

            SqlCommand cmd = new SqlCommand();
            try
            {
                cmdParms = GetAddEntitySql(entity, out insertSql);
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
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityList"></param>
        /// <param name="dataBaseEnum"></param>
        public void AddEntityList<T>(List<T> entityList, SqlServerDBEnum dataBaseEnum)
        {
            _connstr = GetConnectString(dataBaseEnum);
            if (entityList == null || !entityList.Any()) return;
            var t = entityList[0];

            using (SqlConnection conn = new SqlConnection(_connstr))
            {
                conn.Open();
                DataTable entityTB = ListToDataTable(entityList);
                try
                {
                    SqlBulkCopy bulkCopy = new SqlBulkCopy(conn);
                    bulkCopy.DestinationTableName = t.GetType().Name;
                    bulkCopy.BatchSize = entityTB.Rows.Count;
                    bulkCopy.WriteToServer(entityTB);
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.WriteException(ex, "AddEntityList = ");
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
        /// <param name="cmdParms">参数</param>
        /// <returns></returns>
        public bool UdpEntity(string updateSql, SqlServerDBEnum dataBaseEnum, Dictionary<string, object> cmdParms)
        {
            _connstr = GetConnectString(dataBaseEnum);
            bool isUdp = false;

            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(_connstr);
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
        public bool UdpEntityTransaction(SqlTransaction varTrans, SqlConnection conn, string updateSql, Dictionary<string, object> cmdParms)
        {
            bool isUdp = false;
            SqlCommand cmd = new SqlCommand();
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
        public bool DelEntity(string deleteSql, SqlServerDBEnum dataBaseEnum, Dictionary<string, object> cmdParms)
        {
            _connstr = GetConnectString(dataBaseEnum);
            bool isDel = false;

            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(_connstr);
            try
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, deleteSql, cmdParms);

                isDel = cmd.ExecuteNonQuery() > 0;
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteException(ex, "deleteSql =" + deleteSql);
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
        /// <param name="varTrans"></param>
        /// <param name="conn"></param>
        /// <param name="deleteSql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public bool DelEntityTransaction(SqlTransaction varTrans, SqlConnection conn, string deleteSql, Dictionary<string, object> cmdParms)
        {
            bool isDel = false;

            SqlCommand cmd = new SqlCommand();
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


        #region 获取数据库连接
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <param name="dataBaseEnum"></param>
        /// <returns></returns>
        public string GetBaseConnectString(SqlServerDBEnum dataBaseEnum)
        {
            _connstr = GetConnectString(dataBaseEnum);
            return _connstr;
        }
        private string GetConnectString(SqlServerDBEnum dataBaseEnum)
        {
            string _conn = string.Empty;
            switch (dataBaseEnum)
            {
                case SqlServerDBEnum.SqlClientConnecting:
                    _conn = ConfigHelper.GetCommonSettingsSingleValue("SqlClientConnecting");
                    break;
                default://默认app后台
                    _conn = ConfigHelper.GetCommonSettingsSingleValue("SqlClientConnecting");
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
        private void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, Dictionary<string, object> param)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (param != null)
            {
                foreach (var item in param)
                {
                    if (item.Value == null)
                    {
                        cmd.Parameters.Add(new SqlParameter("@" + item.Key, DBNull.Value));
                    }
                    else
                    {
                        // bool isString = item.Value as string == "";
                        cmd.Parameters.Add(new SqlParameter("@" + item.Key, item.Value));
                    }
                }
            }
        }
        #endregion 获取数据库连接


        #region 事务处理
        /// <summary>
        /// 获取数据库的连接
        /// </summary>
        /// <param name="dataBaseEnum"></param>
        /// <returns></returns>
        public SqlConnection OpenSqlConnection(SqlServerDBEnum dataBaseEnum)
        {
            SqlConnection conn = new SqlConnection(GetConnectString(dataBaseEnum));
            conn.Open();
            return conn;
        }
        /// <summary>
        /// 开始构建事务
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="dataBaseEnum"></param>
        /// <returns></returns>
        public SqlTransaction BeginTransaction(SqlConnection conn)
        {
            return conn.BeginTransaction();
        }
        /// <summary>
        /// 事务提交
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="conn"></param>
        public void CommitTransaction(SqlTransaction tx, SqlConnection conn)
        {
            tx.Commit();
            conn.Close();
            conn.Dispose();
        }
        /// <summary>
        /// 事务回滚
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="conn"></param>
        public void RollbackTransaction(SqlTransaction tx, SqlConnection conn)
        {
            tx.Rollback();
            conn.Close();
            conn.Dispose();
        }
        #endregion 事务处理

        #region dataset转list
        /// <summary>
        /// 转化datatable到list集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<T> DataTableToList<T>(DataTable dt) where T : new()
        {
            // 定义集合 
            List<T> ts = new List<T>();
            //定义一个临时变量 
            string tempName = string.Empty;

            // 获得此模型的公共属性 
            //遍历DataTable中所有的数据行 
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
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
        private DataTable ListToDataTable<T>(List<T> items)
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
        private T DataTableToEntity<T>(DataTable dt) where T : new()
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
                        pi.SetValue(entity, value, null);
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
        private Dictionary<string, object> GetAddEntitySql<T>(T t, out string insertsql)
        {
            insertsql = $" insert into {t.GetType().Name} (";
            string valuesql = " output INSERTED.ID values (";
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
            insertsql = insertsql + ")" + valuesql + ")";

            return param;
        }
        #endregion  entity 转化insert sql

    }
}