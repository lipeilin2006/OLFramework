using UnityEngine;

namespace MemoryPack.Formatters;
public class Vector2Formatter : MemoryPackFormatter<Vector2>
{
    public override void Deserialize(ref MemoryPackReader reader, scoped ref Vector2 value)
    {
        value = new Vector2(reader.ReadValue<float>(), reader.ReadValue<float>());
    }

    public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Vector2 value)
    {
        writer.WriteValue(value.x);
        writer.WriteValue(value.y);
    }
}
public class Vector3Formatter : MemoryPackFormatter<Vector3>
{
    public override void Deserialize(ref MemoryPackReader reader, scoped ref Vector3 value)
    {
        value = new Vector3(reader.ReadValue<float>(), reader.ReadValue<float>(), reader.ReadValue<float>());
    }

    public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Vector3 value)
    {
        writer.WriteValue(value.x);
        writer.WriteValue(value.y);
        writer.WriteValue(value.z);
    }
}
