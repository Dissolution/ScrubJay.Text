namespace ScrubJay.Text.Building;

using System.Runtime.CompilerServices;

public class TextBuilder : IDisposable
{
    public static TextBuilder Borrow()
    {
        return new TextBuilder();
    }

    private char[] _charArray;
    private int _count;

    internal int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charArray.Length;
    }

    public int Length => _count;

    private TextBuilder()
    {
        _charArray = TextPool.Rent();
        _count = 0;
    }

    private void GrowBy(int minCapacityIncrease)
    {
        int newCapacity = (Capacity + minCapacityIncrease) * 2;
        var newArray = TextPool.Rent(newCapacity);
        TextHelper.Unsafe.CopyTo(_charArray, newArray, _count);
        TextPool.Return(_charArray);
        _charArray = newArray;
    }
    
    public TextBuilder Append(char ch)
    {
        if (_count >= _charArray.Length)
        {
            GrowBy(1);
        }
        _charArray[_count++] = ch;
        return this;
    }
    

    public void Dispose()
    {
        char[]? toReturn = _charArray;
        _charArray = null!; // We want any further use to cause bad things to happen
        TextPool.Return(toReturn);
    }
}