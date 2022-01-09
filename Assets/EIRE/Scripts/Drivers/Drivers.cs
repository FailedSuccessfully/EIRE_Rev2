public interface IDriveable
{
    public void AcceptDriver(Driver<IDriveable> driver);
}

public abstract class Driver<T> where T : IDriveable
{
    protected T context;
    public virtual void Mount(T ctx) => context = ctx;
    public abstract void Drive();
}