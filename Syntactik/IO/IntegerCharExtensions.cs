namespace Syntactik.IO
{
    public static class IntegerCharExtensions
    {
        public static bool IsIndentCharacter(this int c)
        {
            return c == '\t' || c == ' ';
        }

        public static bool IsSpaceCharacter(this int c)
        {
            return c == ' ' || c == '\t';
        }

        public static bool IsEndOfOpenString(this int c)
        {
            if (c > 61) return false;
            return c == '=' || c == ':' || c == ',' || c == '\'' || c == '"' || c == ')' || c == '(';
        }

        public static bool IsEndOfOpenName(this int c)
        {
            if (c > 61) return false;
            return c == '=' || c == ':' ||c == '\r' || c == '\n' || c == ',' || c =='\'' || c == '"' || c == ')' || c == '(';
        }

        public static bool IsNewLineCharacter(this int c)
        {
            return c == '\r' || c == '\n';
        }


        
    }
}
