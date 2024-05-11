﻿namespace ScrubJay.Text.Extensions;

public static class CompatExtensions
{
#if NET481 || NETSTANDARD2_0 || NETSTANDARD2_1
    public static ref readonly char GetPinnableReference(this string str)
    {
        unsafe
        {
            fixed (char* strPtr = str)
            {
                return ref Unsafe.AsRef<char>(strPtr);
            }
        }
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref char GetPinnableReference(this char[] charArray)
    {
#if NET6_0_OR_GREATER
        return ref MemoryMarshal.GetArrayDataReference<char>(charArray);
#else
        return ref charArray[0];
#endif
    }
}