namespace Syntactik
{
    public static class ErrorCodes
    {
        public static string[] Errors =
        {
            "Unexpected character(s) `{0}`.", // 0
            "{0} is expected.", // 1
            "Invalid indentation.", //2
            "Block indent mismatch.",// 3
            "Invalid indent multiplicity.",// 4
            "Mixed indentation.", //5
            "Invalid indentation size.",// 6
        };

        public static string Format(int code, params object[] args)
        {
            return string.Format(Errors[code], args);
        }
    }
}
