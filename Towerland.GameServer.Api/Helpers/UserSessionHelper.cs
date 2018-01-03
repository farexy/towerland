using System;
using System.Text;
using Towerland.GameServer.Domain.Helpers;

namespace GameServer.Api.Helpers
{
  public static class UserSessionHelper
  {
    private static readonly byte[] Key = Encoding.ASCII.GetBytes(Guid.NewGuid().ToString("N"));
    private static readonly byte[] Iv = new byte[] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
    
    public static string GetSessionHash(Guid userId)
    {
      return userId.ToString("N").EncryptAes256(Key, Iv);
    }

    public static Guid GetUserId(string sessionHash)
    {
      return Guid.Parse(sessionHash.DecryptAes256(Key, Iv));
    }

    public static bool IsValid(string sessionHash)
    {
      return Guid.TryParse(sessionHash, out Guid _);
    }
  }
}