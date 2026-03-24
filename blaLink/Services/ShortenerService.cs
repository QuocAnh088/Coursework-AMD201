namespace blaLink.Services
{
    public class ShortenerService
    {
        // Base62 encoding characters (26 lowercase + 26 uppercase + 10 digits = 62)
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public string Encode(int i)
        {
            if (i == 0) return Alphabet[0].ToString();
            var s = string.Empty;
            while (i > 0)
            {
                s = Alphabet[i % 62] + s;
                i = i / 62;
            }
            return s;
        }
    }
}
