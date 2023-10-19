namespace GridShared.Filtering
{
    public enum GridFilterType
    {
        Equals = 1,
        Contains = 2,
        StartsWith = 3,
        EndsWidth = 4,
        GreaterThan = 5,
        LessThan = 6,
        GreaterThanOrEquals = 7,
        LessThanOrEquals = 8,
        Condition = 9,
        NotEquals = 10,
        IsNull = 11,
        IsNotNull = 12,
        IsDuplicated = 13,
        IsNotDuplicated = 14
     }
}