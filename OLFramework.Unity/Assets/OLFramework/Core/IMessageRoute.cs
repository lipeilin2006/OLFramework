using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLFramework.Core
{
	/// <summary>
	/// 路由处理接口
	/// </summary>
	public interface IMessageRoute
	{
		/// <summary>
		/// 接受到特定消息时触发，可以使用异步
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public Message OnMessage(Message message);
	}
}
