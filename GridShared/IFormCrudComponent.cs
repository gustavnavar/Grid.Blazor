namespace GridShared
{
    public interface IFormCrudComponent<T>
    {
        T Item { get; }
        GridMode ReturnMode { get; }
    }
}
