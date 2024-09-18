﻿







using MemoryPack.Formatters;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Numerics;

namespace MemoryPack;

public static partial class MemoryPackFormatterProvider
{
    internal static void RegisterWellKnownTypesFormatters()
    {

        Register(new UnmanagedFormatter<SByte>());
        Register(new UnmanagedArrayFormatter<SByte>());
        Register(new NullableFormatter<SByte>());

        Register(new UnmanagedFormatter<Byte>());
        Register(new UnmanagedArrayFormatter<Byte>());
        Register(new NullableFormatter<Byte>());

        Register(new UnmanagedFormatter<Int16>());
        Register(new UnmanagedArrayFormatter<Int16>());
        Register(new NullableFormatter<Int16>());

        Register(new UnmanagedFormatter<UInt16>());
        Register(new UnmanagedArrayFormatter<UInt16>());
        Register(new NullableFormatter<UInt16>());

        Register(new UnmanagedFormatter<Int32>());
        Register(new UnmanagedArrayFormatter<Int32>());
        Register(new NullableFormatter<Int32>());

        Register(new UnmanagedFormatter<UInt32>());
        Register(new UnmanagedArrayFormatter<UInt32>());
        Register(new NullableFormatter<UInt32>());

        Register(new UnmanagedFormatter<Int64>());
        Register(new UnmanagedArrayFormatter<Int64>());
        Register(new NullableFormatter<Int64>());

        Register(new UnmanagedFormatter<UInt64>());
        Register(new UnmanagedArrayFormatter<UInt64>());
        Register(new NullableFormatter<UInt64>());

        Register(new UnmanagedFormatter<Char>());
        Register(new UnmanagedArrayFormatter<Char>());
        Register(new NullableFormatter<Char>());

        Register(new UnmanagedFormatter<Single>());
        Register(new UnmanagedArrayFormatter<Single>());
        Register(new NullableFormatter<Single>());

        Register(new UnmanagedFormatter<Double>());
        Register(new UnmanagedArrayFormatter<Double>());
        Register(new NullableFormatter<Double>());

        Register(new UnmanagedFormatter<Decimal>());
        Register(new UnmanagedArrayFormatter<Decimal>());
        Register(new NullableFormatter<Decimal>());

        Register(new UnmanagedFormatter<Boolean>());
        Register(new UnmanagedArrayFormatter<Boolean>());
        Register(new NullableFormatter<Boolean>());

        Register(new UnmanagedFormatter<IntPtr>());
        Register(new UnmanagedArrayFormatter<IntPtr>());
        Register(new NullableFormatter<IntPtr>());

        Register(new UnmanagedFormatter<UIntPtr>());
        Register(new UnmanagedArrayFormatter<UIntPtr>());
        Register(new NullableFormatter<UIntPtr>());

        Register(new UnmanagedFormatter<DateTime>());
        Register(new UnmanagedArrayFormatter<DateTime>());
        Register(new NullableFormatter<DateTime>());

        Register(new UnmanagedFormatter<DateTimeOffset>());
        Register(new UnmanagedArrayFormatter<DateTimeOffset>());
        Register(new NullableFormatter<DateTimeOffset>());

        Register(new UnmanagedFormatter<TimeSpan>());
        Register(new UnmanagedArrayFormatter<TimeSpan>());
        Register(new NullableFormatter<TimeSpan>());

        Register(new UnmanagedFormatter<Guid>());
        Register(new UnmanagedArrayFormatter<Guid>());
        Register(new NullableFormatter<Guid>());

        Register(new UnmanagedFormatter<Complex>());
        Register(new UnmanagedArrayFormatter<Complex>());
        Register(new NullableFormatter<Complex>());

        Register(new UnmanagedFormatter<Plane>());
        Register(new UnmanagedArrayFormatter<Plane>());
        Register(new NullableFormatter<Plane>());

        Register(new UnmanagedFormatter<Quaternion>());
        Register(new UnmanagedArrayFormatter<Quaternion>());
        Register(new NullableFormatter<Quaternion>());

        Register(new UnmanagedFormatter<Matrix3x2>());
        Register(new UnmanagedArrayFormatter<Matrix3x2>());
        Register(new NullableFormatter<Matrix3x2>());

        Register(new UnmanagedFormatter<Matrix4x4>());
        Register(new UnmanagedArrayFormatter<Matrix4x4>());
        Register(new NullableFormatter<Matrix4x4>());

        Register(new UnmanagedFormatter<Vector2>());
        Register(new UnmanagedArrayFormatter<Vector2>());
        Register(new NullableFormatter<Vector2>());

        Register(new UnmanagedFormatter<Vector3>());
        Register(new UnmanagedArrayFormatter<Vector3>());
        Register(new NullableFormatter<Vector3>());

        Register(new UnmanagedFormatter<Vector4>());
        Register(new UnmanagedArrayFormatter<Vector4>());
        Register(new NullableFormatter<Vector4>());

#if NET7_0_OR_GREATER

        Register(new UnmanagedFormatter<Rune>());
        Register(new UnmanagedArrayFormatter<Rune>());
        Register(new NullableFormatter<Rune>());

        Register(new UnmanagedFormatter<DateOnly>());
        Register(new UnmanagedArrayFormatter<DateOnly>());
        Register(new NullableFormatter<DateOnly>());

        Register(new UnmanagedFormatter<TimeOnly>());
        Register(new UnmanagedArrayFormatter<TimeOnly>());
        Register(new NullableFormatter<TimeOnly>());

        Register(new UnmanagedFormatter<Half>());
        Register(new UnmanagedArrayFormatter<Half>());
        Register(new NullableFormatter<Half>());

        Register(new UnmanagedFormatter<Int128>());
        Register(new UnmanagedArrayFormatter<Int128>());
        Register(new NullableFormatter<Int128>());

        Register(new UnmanagedFormatter<UInt128>());
        Register(new UnmanagedArrayFormatter<UInt128>());
        Register(new NullableFormatter<UInt128>());

#endif

        Register(new StringFormatter());
        Register(new ArrayFormatter<String>());

        Register(new VersionFormatter());
        Register(new ArrayFormatter<Version>());

        Register(new UriFormatter());
        Register(new ArrayFormatter<Uri>());

        Register(new TimeZoneInfoFormatter());
        Register(new ArrayFormatter<TimeZoneInfo>());

        Register(new BigIntegerFormatter());
        Register(new ArrayFormatter<BigInteger>());

        Register(new BitArrayFormatter());
        Register(new ArrayFormatter<BitArray>());

        Register(new StringBuilderFormatter());
        Register(new ArrayFormatter<StringBuilder>());

        Register(new TypeFormatter());
        Register(new ArrayFormatter<Type>());

        Register(new CultureInfoFormatter());
        Register(new ArrayFormatter<CultureInfo>());

        Register(new ObjectFormatter());
        Register(new ArrayFormatter<Object>());

        Register(new Vector2Formatter());
        Register(new ArrayFormatter<Vector2>());

        Register(new Vector3Formatter());
        Register(new ArrayFormatter<Vector3>());

    }
}
