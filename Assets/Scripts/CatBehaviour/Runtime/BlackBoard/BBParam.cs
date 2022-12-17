namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 黑板参数
    /// </summary>
    public class BBParam<T> : IBBParam
    {
        public T Param;

        public override string ToString()
        {
            return Param.ToString();
        }
    }
}