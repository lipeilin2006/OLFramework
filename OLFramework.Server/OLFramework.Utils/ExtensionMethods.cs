using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLFramework.Utils
{

	public static class ExtensionMethods
	{
		public static int Hash(this string s)
		{
			int hash = 0;
			int i = 0;

			while (i < s.Length)
			{
				hash += s[i];
				hash += (hash << 10);
				hash ^= (hash >> 6);

				i++;
			}

			hash += (hash << 3);
			hash ^= (hash >> 11);
			hash += (hash << 15);

			if (hash < 0)
			{
				hash = -hash;
			}

			return hash;
		}
	}
}
