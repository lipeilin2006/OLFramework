using System;

namespace OLFramework.Core
{
#nullable enable
	public abstract class SyncVar
	{
        /// <summary>
        /// �ñ����ı�������Hash
        /// </summary>
        public int NameHash { get; internal set; }
        /// <summary>
        /// ��ʶ�����Ƿ�ı�
        /// </summary>
        public bool isChanged { get; set; } = false;
        /// <summary>
        /// ���ñ�����ֵ����Ϊ���������ֵ���Ա��أ�ֵ��������
        /// </summary>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        public abstract void Set(object value, SyncDirection direction = SyncDirection.LocalToRemote);
		/// <summary>
		/// ��ȡԭʼ��object��һ���ǿ���ڲ�ʹ��
		/// </summary>
		/// <returns></returns>
		public abstract object? GetObject();
	}
	//ͬ������
	public enum SyncDirection
	{
		LocalToRemote = 0, RemoteToLocal = 1
	}

    public class SyncVar<T> : SyncVar
    {
        /// <summary>
        /// ��ͬ������������Networkobject��ʶ
        /// </summary>
        private NetworkIdentify networkIdentify;
        /// <summary>
        /// �������򱾵�ͬ���������ĺ�����һ�㲻��Ϊ��
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
        /// ��ȡ��������ת��Ϊ�ض�����
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
                //�ӱ���������ͬ������Ҫ������״̬���Ϊ�Ѹı�
                this.value = (T)value;
                networkIdentify.Changed();
                isChanged = true;
            }
            else
            {
                //�������򱾵�ͬ��
                this.value = (T)value;
                OnNetwrokSet(value);
                isChanged = false;
            }
        }
    }
}