using static InlineIL.IL;

namespace ScrubJay.Text.Utilities;

public static class FastMath
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int HalfRoundDown(int value)
    {
        Emit.Ldarg(nameof(value));
        Emit.Ldc_I4_1();
        Emit.Shr();
        return Return<int>();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Twice(int value)
    {
        Emit.Ldarg(nameof(value));
        Emit.Ldc_I4_1();
        Emit.Shl();
        return Return<int>();
    }
}