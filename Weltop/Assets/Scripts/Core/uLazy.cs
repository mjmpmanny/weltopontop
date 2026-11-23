using System;

public class uLazy<T>
{
    public Func<T> val;
    public uLazy(Func<T> val) {
        this.val = val;
    }
    public static implicit operator T(uLazy<T> L) => L.val.Invoke();
}
 