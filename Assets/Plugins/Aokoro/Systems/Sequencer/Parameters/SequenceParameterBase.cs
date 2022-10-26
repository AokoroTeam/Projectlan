namespace Aokoro.Sequencing
{
    public class SequenceParameterBase<T> : IParameter, IParameter<T>
    {
        public string name;
        public T Value = default;
        public SequenceParameterBase(string name, T value)
        {
            this.Value = value;
            this.name = name;
        }

        string IParameter.name => name;

        object IParameter.Value => Value;

        T IParameter<T>.Value => Value;
    }
}