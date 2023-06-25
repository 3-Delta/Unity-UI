using System.Runtime.CompilerServices;

public class ArrayUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InIndexRange(int index, int count)
    {
        return ((uint) index) < ((uint) count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InIndexRange(int index, int minIndex, int count)
    {
        return ((uint) (index - minIndex)) < ((uint) (count - minIndex));
    }
}