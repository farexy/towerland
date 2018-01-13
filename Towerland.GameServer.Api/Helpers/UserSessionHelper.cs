using System;
using System.Security.Cryptography;
using System.Text;
using Towerland.GameServer.Api.Exceptions;
using Towerland.GameServer.Domain.Helpers;

namespace Towerland.GameServer.Api.Helpers
{
  public static class UserSessionHelper
  {
    //private static readonly byte[] Key = Encoding.ASCII.GetBytes(Guid.NewGuid().ToString("N"));
    private static readonly byte[] Key = Encoding.ASCII.GetBytes(Guid.Parse("71dc126b-f804-4cd5-93ec-dfa5087ba2da").ToString("N"));
    private static readonly byte[] Iv = { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xa, 0xb, 0xc, 0xd, 0xe, 0xf };
    
    public static string GetSessionHash(Guid userId)
    {
      return userId.ToString("N").EncryptAes256(Key, Iv);
    }

    public static Guid GetUserId(string sessionHash)
    {
      try
      {
        return Guid.Parse(sessionHash.DecryptAes256(Key, Iv));
      }
      catch (CryptographicException)
      {
        throw new ApiException("Access is denied due to invalid session key");
      }
    }

    public static bool IsValid(string sessionHash)
    {
      return Guid.TryParse(sessionHash, out Guid _);
    }
  }
}