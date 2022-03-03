using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tools_sqltonet
{
	public class QJJob
	{
		#region 构造函数
		public QJJob()
		{
			this._id = 0;
			this._qjdepartid = 0;
			this._joben = string.Empty;
			this._jobcn = string.Empty;
			this._isdeleted = false;
			this._createtime = DateTime.Now;
		}
		#endregion

		#region 成员
		private long _id;
		private long _qjdepartid;
		private string _joben;
		private string _jobcn;
		private bool _isdeleted;
		private DateTime _createtime;
		#endregion

		#region 属性
		/// <summary>
		/// 主键
		/// </summary>
		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}
		/// <summary>
		/// 部门Id主键
		/// </summary>
		public long QJDepartId
		{
			get { return _qjdepartid; }
			set { _qjdepartid = value; }
		}
		/// <summary>
		/// 英文名称
		/// </summary>
		public string JobEN
		{
			get { return _joben; }
			set { _joben = value; }
		}
		/// <summary>
		/// 名称
		/// </summary>
		public string JobCN
		{
			get { return _jobcn; }
			set { _jobcn = value; }
		}
		public bool IsDeleted
		{
			get { return _isdeleted; }
			set { _isdeleted = value; }
		}
		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreateTime
		{
			get { return _createtime; }
			set { _createtime = value; }
		}
		#endregion
	}
}