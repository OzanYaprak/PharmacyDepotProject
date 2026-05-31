using System.Security.Cryptography;
using System.Text;

namespace Security.Hashing;

public static class HashingHelper
{
    public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using HMACSHA512 hmacSHA512 = new HMACSHA512();

        passwordSalt = hmacSHA512.Key;
        passwordHash = hmacSHA512.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using HMACSHA512 hmacSHA512 = new HMACSHA512(passwordSalt);
        
        byte[] computedHash = hmacSHA512.ComputeHash(Encoding.UTF8.GetBytes(password));

        return computedHash.SequenceEqual(passwordHash);
    }
}
