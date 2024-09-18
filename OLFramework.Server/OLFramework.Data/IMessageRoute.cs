using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLFramework.Data
{
	public interface IMessageRoute
	{
		public Message OnMessage(Player player, Message message);
	}
}