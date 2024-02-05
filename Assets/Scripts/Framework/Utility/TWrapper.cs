using System;

public struct stWrapper1<T0, T1>
{
    public readonly T0 o;
    public readonly T1 wrapper1;

    public stWrapper1(T0 o, T1 wrapper1)
    {
        this.o = o;
        this.wrapper1 = wrapper1;
    }
}

public struct stWrapper2<T0, T1, T2>
{
    public readonly T0 o;
    public readonly T1 wrapper1;
    public readonly T2 wrapper2;

    public stWrapper2(T0 o, T1 wrapper1, T2 wrapper2)
    {
        this.o = o;
        this.wrapper1 = wrapper1;
        this.wrapper2 = wrapper2;
    }
}

public class Wrapper1<T0, T1>
{
    public readonly T0 o;
    public readonly T1 wrapper1;

    public Wrapper1(T0 o, T1 wrapper1)
    {
        this.o = o;
        this.wrapper1 = wrapper1;
    }
}

public class Wrapper2<T0, T1, T2>
{
    public readonly T0 o;
    public readonly T1 wrapper1;
    public readonly T2 wrapper2;

    public Wrapper2(T0 o, T1 wrapper1, T2 wrapper2)
    {
        this.o = o;
        this.wrapper1 = wrapper1;
        this.wrapper2 = wrapper2;
    }
}