namespace GridShared.Utility
{
    public sealed class VariableReference
    {
        public object Variable { get; set; }

        public VariableReference()
        {
        }

        public VariableReference(object variable)
        {
            Variable = variable;
        }
    }
}
