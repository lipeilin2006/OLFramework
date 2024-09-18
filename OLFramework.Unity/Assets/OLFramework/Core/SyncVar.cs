using System;

namespace OLFramework.Core
{
#nullable enable
	public abstract class SyncVar
	{
        /// <summary>
        /// 该变量的变量名的Hash
        /// </summary>
        public int NameHash { get; internal set; }
        /// <summary>
        /// 标识变量是否改变
        /// </summary>
        public bool isChanged { get; set; } = false;
        /// <summary>
        /// 设置变量的值，分为两种情况：值来自本地，值来自网络
        /// </summary>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        public abstract void Set(object value, SyncDirection direction = SyncDirection.LocalToRemote);
		/// <summary>
		/// 获取原始的object，一般是框架内部使用
		/// </summary>
		/// <returns></returns>
		public abstract object? GetObject();
	}
	//同步方向
	public enum SyncDirection
	{
		LocalToRemote = 0, RemoteToLocal = 1
	}

    public class SyncVar<T> : SyncVar
    {
        /// <summary>
        /// 该同步变量所属的Networkobject标识
        /// </summary>
        private NetworkIdentify networkIdentify;
        /// <summary>
        /// 从网络向本地同步所触发的函数，一般不能为空
        /// </summary>
        public Action<object> OnNetwrokSet { get; private set; }
        public T? value { get; private set; }


        public SyncVar(string name,NetworkIdentify networkIdentify, Action<object> onNetwrokSet)
        {
            NameHash = name.Hash();
            this.networkIdentify = networkIdentify;
            OnNetwrokSet = onNetwrokSet;
            networkIdentify.AddSyncVar(NameHash, this);
        }


        /// <summary>
        /// 获取并将变量转换为特定类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? Get()
        {
            return value;
        }
        public override object? GetObject()
        {
            return value;
        }
        public override void Set(object value, SyncDirection direction = SyncDirection.LocalToRemote)
        {
            if (direction == SyncDirection.LocalToRemote)
            {
                //从本地向网络同步，需要将变量状态标记为已改变
                this.value = (T)value;
                networkIdentify.Changed();
                isChanged = true;
            }
            else
            {
                //从网络向本地同步
                this.value = (T)value;
                OnNetwrokSet(value);
                isChanged = false;
            }
        }
    }
}