using UnityEngine;

namespace MemoryPack.Formatters
{
    public class ObjectFormatter : MemoryPackFormatter<object?>
    {
        private Dictionary<byte, Type> index_type = new()
        {
            {1,typeof(byte) },
            {2,typeof(sbyte) },
            {3,typeof(short) },
            {4,typeof(ushort) },
            {5,typeof(int) },
            {6,typeof(uint) },
            {7,typeof(long) },
            {8,typeof(ulong) },
            {9,typeof(float) },
            {10,typeof(double) },
            {11,typeof(decimal) },
            {12,typeof(string) },
            {13,typeof(Vector2) },
            {14,typeof(Vector3) }
        };
        private Dictionary<Type, byte> type_index = new()
        {
            {typeof(byte),1},
            {typeof(sbyte),2 },
            {typeof(short),3},
            {typeof(ushort),4},
            {typeof(int),5},
            {typeof(uint),6},
            {typeof(long),7},
            {typeof(ulong),8},
            {typeof(float),9},
            {typeof(double),10},
            {typeof(decimal),11},
            {typeof(string),12},
            {typeof(Vector2),13 },
            {typeof(Vector3),14}
        };
        public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref object? value)
        {
            if (value == null)
            {
                writer.WriteNullObjectHeader();
                return;
            }

            Type type = value.GetType();

            if (!type_index.ContainsKey(type))
                throw new ObjectFormatterException();

            var formatter = MemoryPackFormatterProvider.GetFormatter(type);

            writer.WriteValue(type_index[type]);
            formatter.Serialize(ref writer, ref value);
        }

        public override void Deserialize(ref MemoryPackReader reader, scoped ref object? value)
        {
            if (reader.PeekIsNull())
            {
                reader.Advance(1);
                value = null;
                return;
            }

            byte index = reader.ReadValue<byte>();
            if (!index_type.ContainsKey(index))
                throw new ObjectFormatterException();

            var formatter = MemoryPackFormatterProvider.GetFormatter(index_type[index]);

            formatter.Deserialize(ref reader, ref value);
        }
    }

    public class ObjectFormatterException : Exception { }
}
