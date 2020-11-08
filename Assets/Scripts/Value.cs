using System;

[Serializable]
public struct Value
{
    public int value;
    public int max;

    public Value(int value, int max)
    {
        this.value = value;
        this.max = max;
    }
}