namespace GridShared.Columns
{
    public enum InputType
    {
        None,
        Text,
        TextArea,
        Date,
        Time,
        DateTimeLocal,
        Week,
        Month,
        File,
        Number
    }

    public static class InputTypeExtensions
    {
        public static string ToTypeAttr(this InputType me)
        {
            switch (me)
            {
                case InputType.Text:
                    return "text";
                case InputType.Date:
                    return "date";
                case InputType.Time:
                    return "time";
                case InputType.DateTimeLocal:
                    return "datetime-local";
                case InputType.Week:
                    return "week";
                case InputType.Month:
                    return "month";
                case InputType.File:
                    return "file";
                case InputType.Number:
                    return "number";
                default:
                    return "";
            }
        }
    }
}