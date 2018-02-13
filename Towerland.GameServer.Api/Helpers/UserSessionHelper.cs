using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Towerland.GameServer.Api.Exceptions;
using Towerland.GameServer.Domain.Helpers;

namespace Towerland.GameServer.Api.Helpers
{
  public static class UserSessionHelper
  {
    //private static readonly byte[] Key = Encoding.ASCII.GetBytes(Guid.NewGuid().ToString("N"));
    private static readonly byte[] Key = Encoding.ASCII.GetBytes(Guid.Parse("71dc126b-f804-4cd5-93ec-dfa5087ba2da").ToString("N"));
    private static readonly byte[] Iv = { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xa, 0xb, 0xc, 0xd, 0xe, 0xf };

    public static async Task<string> GetSessionHashAsync(Guid userId)
    {
      return await userId.ToString("N").EncryptAes256Async(Key, Iv);
    }

    public static async Task<Guid> GetUserIdAsync(string sessionHash)
    {
      try
      {
        return Guid.Parse(await sessionHash.DecryptAes256Async(Key, Iv));
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