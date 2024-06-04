/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using System.Security.Cryptography;

namespace WebApi.Services;

public interface ICryptographyService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}

public class CryptographyService(
    ILogger<CryptographyService> logger) : ICryptographyService
{
    private const int SaltSize = 16;
    private const int HashSize = 20;
    private const int Iterations = 10000;

    private readonly ILogger<CryptographyService> _logger = logger;

    public string HashPassword(string password)
    {
        byte[] salt;
        RandomNumberGenerator.Fill(salt = new byte[SaltSize]);

        var pbkdf2 = new Rfc2898DeriveBytes(
            password, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(HashSize);

        byte[] hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        string hashedPassword = Convert.ToBase64String(hashBytes);

        _logger.LogInformation("Password hashed successfully.");

        return hashedPassword;
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        byte[] hashBytes = Convert.FromBase64String(hashedPassword);

        byte[] salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);

        var pbkdf2 = new Rfc2898DeriveBytes(
            password, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(HashSize);

        for (int i = 0; i < HashSize; i++)
        {
            if (hashBytes[i + SaltSize] != hash[i])
            {
                _logger.LogWarning("Password verification failed.");
                return false;
            }
        }

        _logger.LogInformation("Password verified successfully.");

        return true;
    }
}
