using Microsoft.AspNetCore.Identity;
using BCrypt;
using PersistenceService.Models;

namespace PersistenceService.Utils;

public class BcryptPasswordHasher : IPasswordHasher<User>
{
    public string HashPassword(User user, string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public PasswordVerificationResult VerifyHashedPassword(
        User user,
        string hashedPassword,
        string providedPassword
    )
    {
        var valid = BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        return valid
            ? PasswordVerificationResult.Success
            : PasswordVerificationResult.Failed;
    }
}
