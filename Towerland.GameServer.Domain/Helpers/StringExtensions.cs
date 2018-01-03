using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Towerland.GameServer.Domain.Helpers
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
      byte[] salt = new byte[16];
      new RNGCryptoServiceProvider().GetBytes(salt);
      
      var pbkdf2 = new Rfc2898DeriveBytes(s, salt, 100);
      byte[] hash = pbkdf2.GetBytes(20);
      
      byte[] hashBytes = new byte[36];
      Array.Copy(salt, 0, hashBytes, 0, 16);
      Array.Copy(hash, 0, hashBytes, 16, 20);
      
      return Convert.ToBase64String(hashBytes);
    }
    
    public static string EncryptAes256(this string s, byte[] key, byte[] iv)
    {
      Aes encryptor = Aes.Create();
      encryptor.Mode = CipherMode.CBC;

      encryptor.Key = key;
      encryptor.IV = iv;

      MemoryStream memoryStream = new MemoryStream();
      ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();

      CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);

      byte[] plainBytes = Encoding.ASCII.GetBytes(s);

      cryptoStream.Write(plainBytes, 0, plainBytes . Length);

      cryptoStream . FlushFinalBlock();

      byte[] cipherBytes = memoryStream.ToArray();

      memoryStream.Close();
      cryptoStream.Close();

      string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);

      return cipherText;
    }
    
    public static string DecryptAes256(this string cipher, byte[] key, byte[] iv)
    {
      Aes encryptor = Aes.Create();
      encryptor.Mode = CipherMode.CBC;

      encryptor.Key = key;
      encryptor.IV = iv;

      MemoryStream memoryStream = new MemoryStream();
      ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();

      CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);

      string text = String.Empty;

      try {
        byte[] cipherBytes = Convert.FromBase64String(cipher);
        cryptoStream.Write(cipherBytes, 0, cipherBytes . Length);

        cryptoStream.FlushFinalBlock();
        byte[] plainBytes = memoryStream.ToArray();

        text = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
      } finally {
        memoryStream.Close();
        cryptoStream.Close();
      }

      return text;
    }
  }
}
