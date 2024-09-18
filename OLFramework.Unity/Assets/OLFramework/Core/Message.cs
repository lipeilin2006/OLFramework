using System.Collections.Concurrent;
using System.Collections.Generic;
using MemoryPack;

#nullable enable
namespace OLFramework.Core
{
    /// <summary>
    /// 消息包主体
    /// </summary>
    [MemoryPackable]
    public partial class Message
    {
        private static ConcurrentQueue<Message> messagePool = new ConcurrentQueue<Message>();

        /// <summary>
        /// 路由
        /// </summary>
        public int Route { get; private set; }
        /// <summary>
        /// 消息类型，两种：需要响应和不需要响应
        /// </summary>
        public MessageType MessageType { get; private set; }
        /// <summary>
        /// 消息状态
        /// </summary>
        public Status Status { get; private set; } = Status.Success;
        /// <summary>
        /// 唯一消息id
        /// </summary>
        public int MessageID { get; private set; } = 0;
        /// <summary>
        /// 这里可以放各种基础类型数据，包括System.Numerics.Vector2和System.Numerics.Vector3
        /// </summary>
        public Dictionary<int, object> Values { get; set; }

        [MemoryPackConstructor]
        private Message(int route, MessageType messageType, Status status, int messageID, Dictionary<int, object> values)
        {
            Route = route;
            MessageType = messageType;
            Status = status;
            MessageID = messageID;
            Values = values;
        }
        public Message(string route, MessageType messageType, Dictionary<int, object>? values = null, Status status = Status.Success)
        {
            Route = route.Hash();
            MessageType = messageType;
            if (values == null)
                Values = new();
            else
                Values = values;
            Status = status;
        }

        public static Message Take(string route, MessageType messageType, Dictionary<int, object>? values = null, Status status = Status.Success)
        {
            if (messagePool.TryDequeue(out Message? message))
            {
                message.Route = route.Hash();
                message.MessageType = messageType;
                if (values == null)
                    message.Values = new();
                else
                    message.Values = values;
                message.Status = status;
                return message;
            }
            else
            {
                return new Message(route, messageType, values, status);
            }
        }

        public static Message Take(int route, MessageType messageType, Status status, int messageID, Dictionary<int, object> values)
        {
            if (messagePool.TryDequeue(out Message? message))
            {
                message.Route = route;
                message.MessageType = messageType;
                message.Status = status;
                message.MessageID = messageID;
                message.Values = values;
                return message;
            }
            else
            {
                return new Message(route, messageType, status, messageID, values);
            }
        }

        public void Recycle()
        {
            Route = 0;
            MessageType = 0;
            Status = Status.Success;
            Values = new();
            MessageID = 0;
            messagePool.Enqueue(this);
            UnityEngine.Debug.Log("Recycle");
            UnityEngine.Debug.Log($"message pool count : {messagePool.Count}");
            
        }

        /// <summary>
        /// 设置MessageID
        /// </summary>
        /// <param name="messageID"></param>
        public void SetID(int messageID)
        {
            MessageID = messageID;
        }
        /// <summary>
        /// 获取Values中的指定值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object? GetValue(string name)
        {
            if (HasValue(name)) return Values[name.Hash()];
            return null;
        }
        /// <summary>
        /// 设置Values中指定值，若没有则会创建
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetValue(string name, object value)
        {
            if (HasValue(name))
            {
                Values[name.Hash()] = value;
            }
            else
            {
                Values.Add(name.Hash(), value);
            }
        }

        public bool HasValue(string name)
        {

            return Values.ContainsKey(name.Hash());
        }

        public Message ResponseFailure()
        {
            return Take(Route, MessageType.Response, Status.Failure, MessageID, new());
        }
        public Message ResponseFailure(Dictionary<int, object> values)
        {
            return Take(Route, MessageType.Response, Status.Failure, MessageID, values);
        }
        public Message ResponseSuccess()
        {
            return Take(Route, MessageType.Response, Status.Success, MessageID, new());
        }
        public Message ResponseSuccess(Dictionary<int, object> values)
        {
            return Take(Route, MessageType.Response, Status.Success, MessageID, values);
        }
        public bool isSucceed()
        {
            if (Status == Status.Success)
            {
                return true;
            }
            return false;
        }
    }
    public enum MessageType
    {
        Request = 0,
        Response = 1
    }
    public enum Status
    {
        Success, Failure
    }
}