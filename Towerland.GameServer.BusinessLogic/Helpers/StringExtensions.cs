using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Towerland.GameServer.BusinessLogic.Helpers
{
  public static class StringExtensions
  {
    public static T FromJsonString<T>(this string s)
    {
      return JsonConvert.DeserializeObject<T>(s);
    }

    public static string ToJsonString<T>(this T o)
    {
      return JsonConvert.SerializeObject(o);
    }
    
    public static string ToPwdHash(this string s)
    {
      byte[] salt = { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xa, 0xb, 0xc, 0xd, 0xe, 0xf };
      
      var pbkdf2 = new Rfc2898DeriveBytes(s, salt, 100);
      byte[] hash = pbkdf2.GetBytes(20);
      
      byte[] hashBytes = new byte[36];
      Array.Copy(salt, 0, hashBytes, 0, 16);
      Array.Copy(hash, 0, hashBytes, 16, 20);
      
      return Convert.ToBase64String(hashBytes);
    }
    
    public static async Task<string> EncryptAes256Async(this string s, byte[] key, byte[] iv)
    {
      Aes encryptor = Aes.Create();
      encryptor.Mode = CipherMode.CBC;

      encryptor.Key = key;
      encryptor.IV = iv;

      MemoryStream memoryStream = new MemoryStream();
      ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();

      CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);

      byte[] plainBytes = Encoding.ASCII.GetBytes(s);

      await cryptoStream.WriteAsync(plainBytes, 0, plainBytes . Length);

      cryptoStream.FlushFinalBlock();

      byte[] cipherBytes = memoryStream.ToArray();

      memoryStream.Close();
      cryptoStream.Close();

      string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);

      return cipherText;
    }
    
    public static async Task<string> DecryptAes256Async(this string cipher, byte[] key, byte[] iv)
    {
      Aes encryptor = Aes.Create();
      encryptor.Mode = CipherMode.CBC;

      encryptor.Key = key;
      encryptor.IV = iv;

      ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();

      using (MemoryStream memoryStream = new MemoryStream())
      using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write))
      {
        byte[] cipherBytes = Convert.FromBase64String(cipher);
        await cryptoStream.WriteAsync(cipherBytes, 0, cipherBytes . Length);

        cryptoStream.FlushFinalBlock();
        byte[] plainBytes = memoryStream.ToArray();

        return Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
      }
    }
  }
}
