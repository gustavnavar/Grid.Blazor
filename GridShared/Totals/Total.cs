using System;
using System.Runtime.Serialization;

namespace GridShared.Totals
{
    [DataContract]
    public class Total
    {
        [DataMember(Order = 1)]
        public bool IsNumber { get; set; } = false;

        [DataMember(Order = 2)]
        public decimal? Number { get; set; }

        [DataMember(Order = 3)]
        public bool IsDateTime { get; set; } = false;

        [DataMember(Order = 4)]
        public DateTime? DateTime { get; set; }

        [DataMember(Order = 5)]
        public bool IsString { get; set; } = false;

        [DataMember(Order = 6)]
        public string String { get; set; }

        public Total()
        { }

        public Total(decimal? number)
        {
            IsNumber = true;
            Number = number;
        }

        public Total(DateTime? dateTime)
        {
            IsDateTime = true;
            DateTime = dateTime;
        }

        public Total(string str)
        {
            IsString = true;
            String = str;
        }

        public string GetString(string valuePattern)
        {
            object value;
            if (IsNumber)
                value = Number;
            else if (IsDateTime)
                value = DateTime;
            else if (IsString)
                value = String;
            else
                return null;

            try
            {
                if (!string.IsNullOrEmpty(valuePattern))
                    return string.Format(valuePattern, value);
                else
                    return value?.ToString();
            }
            catch (Exception)
            {
                return value?.ToString();
            }
        }
    }
}
