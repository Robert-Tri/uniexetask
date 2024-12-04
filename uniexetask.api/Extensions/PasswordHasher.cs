namespace uniexetask.api.Extensions
{
    public class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        public static string GenerateRandomPassword(int length)
        {
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";

            var random = new Random();

            var password = new List<char>
            {
                upperChars[random.Next(upperChars.Length)],
                lowerChars[random.Next(lowerChars.Length)],
                digits[random.Next(digits.Length)],
            };

            const string allChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            for (int i = password.Count; i < length; i++)
            {
                password.Add(allChars[random.Next(allChars.Length)]);
            }

            return new string(password.OrderBy(c => random.Next()).ToArray());
        }
    }
}
