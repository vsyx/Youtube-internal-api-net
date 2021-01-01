namespace YoutubeApi.Util
{
    public class JsonUtils
    {
        public static int FindEndingJsonBracket(string text, int start = 0, int? end = null)
        {
            if (end == null)
            {
                end = text.Length;
            }

            uint bracketCount = 0;
            bool isPreviousEscape = false;
            bool isWithinKeyOrValue = false;

            for (int i = start; i < end; i++)
            {
                char c = text[i];
                // doesn't take in account unicode escape sequences, shouldn't affect the counting anyway
                if (isPreviousEscape) 
                {
                    isPreviousEscape = false;
                }
                else if (!isWithinKeyOrValue)
                {
                    switch (c)
                    {
                        case '}':
                            bracketCount--;
                            if (bracketCount == 0)
                            {
                                return i; 
                            }
                            break;
                        case '{':
                            bracketCount++;
                            break;
                        case '"':
                            isWithinKeyOrValue = true;
                            break;
                    }
                }
                else
                {
                    switch (c)
                    {
                        case '"':
                            isWithinKeyOrValue = false;
                            break;
                        case '\\':
                            isPreviousEscape = true;
                            break;
                    }

                }
            }

            return -1;
        }
    }
}
