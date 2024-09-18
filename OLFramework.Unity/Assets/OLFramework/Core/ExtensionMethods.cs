using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace OLFramework.Core
{
	/// <summary>
	/// 一些简单的扩展方法
	/// </summary>
	public static class ExtensionMethods
	{
		/// <summary>
		/// 获取string的int hash，内置的gethash只能做到进程内唯一，因此添加了一个自定义的hash函数
		/// 在网络同步中使用hash代替string作为键，能有效减小网络包体，但每次计算hash都意味着额外性能开销（基本可以忽略）
		/// </summary>
		/// <param name="s">string</param>
		/// <returns>int hash</returns>

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

		// 下面的方法都是unity和system间vector向量类型的互转
		public static Vector2 ToUnityVec2(this System.Numerics.Vector2 vec2)
		{
			return new(vec2.X, vec2.Y);
		}
		public static System.Numerics.Vector2 ToSystemVec2(this Vector2 vec2)
		{
			return new(vec2.x, vec2.y);
		}
		public static Vector3 ToUnityVec3(this System.Numerics.Vector3 vec3)
		{
			return new(vec3.X, vec3.Y, vec3.Z);
		}

		public static System.Numerics.Vector3 ToSystemVec3(this Vector3 vec3)
		{
			return new(vec3.x, vec3.y, vec3.z);
		}
	}
}
