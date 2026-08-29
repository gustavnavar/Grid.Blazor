using GridShared.Sorting;
using GridShared.Utility;
using System;
using System.Runtime.Serialization;

namespace GridShared.Totals
{
    [DataContract]
    public class Total
    {
        [DataMember(Order = 1)]
        public GridTotalType Type { get; set; } = GridTotalType.None;

        [DataMember(Order = 2)]
        public decimal? Number { get; set; }

        [DataMember(Order = 3)]
        public DateTime? DateTime { get; set; }

        [DataMember(Order = 4)]
        public string String { get; set; }

        public Total()
        { }

        public Total(decimal? number)
        {
            Type = GridTotalType.Number;
            Number = number;
        }

        public Total(DateTime? dateTime)
        {
            Type = GridTotalType.DateTime;
            DateTime = dateTime;
        }

        public Total(string str)
        {
            Type = GridTotalType.String;
            String = str;
        }

        public string GetString(string valuePattern)
        {
            object value;
            if (Type == GridTotalType.Number)
                value = Number;
            else if (Type == GridTotalType.DateTime)
                value = DateTime;
            else if (Type == GridTotalType.String)
                value = String;
            else
                return null;

            try
            {
                if (!string.IsNullOrEmpty(valuePattern))
                    return string.Format(valuePattern, value);
                else
                    // The oldest and the newest row of a date column are read in the same
                    // breath as the column itself, so they are written the same way it is.
                    return GridDateTimeFormats.ToDisplayValue(value) ?? value?.ToString();
            }
            catch (Exception)
            {
                return value?.ToString();
            }
        }
    }
}
