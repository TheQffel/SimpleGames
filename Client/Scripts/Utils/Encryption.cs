using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class Encryption : MonoBehaviour
{
	public static byte[] Encrypt(string Text)
	{
		using (Aes Aes = new AesManaged())
		{
			Aes.Padding = PaddingMode.PKCS7;
			Aes.KeySize = 128;
			int KeyStrengthInBytes = Aes.KeySize / 8;
			Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes("gB01kzxkTirN", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 100);
			Aes.Key = rfc2898.GetBytes(KeyStrengthInBytes);
			Aes.IV = rfc2898.GetBytes(KeyStrengthInBytes);

			using (MemoryStream MemoryStream = new MemoryStream())
			{
				using (CryptoStream CryptoStream = new CryptoStream(MemoryStream, Aes.CreateEncryptor(), CryptoStreamMode.Write))
				{
					CryptoStream.Write(Encoding.Unicode.GetBytes(Text), 0, Encoding.Unicode.GetBytes(Text).Length);
				}
				return MemoryStream.ToArray();
			}
		}
	}

	public static string Decrypt(byte[] Text)
	{
		using (Aes Aes = new AesManaged())
		{
			Aes.Padding = PaddingMode.PKCS7;
			Aes.KeySize = 128;
			int KeyStrengthInBytes = Aes.KeySize / 8;
			Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes("gB01kzxkTirN", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 100);
			Aes.Key = rfc2898.GetBytes(KeyStrengthInBytes);
			Aes.IV = rfc2898.GetBytes(KeyStrengthInBytes);

			using (MemoryStream MemoryStream = new MemoryStream())
			{
				using (CryptoStream CryptoStream = new CryptoStream(MemoryStream, Aes.CreateDecryptor(), CryptoStreamMode.Write))
				{
					CryptoStream.Write(Text, 0, Text.Length);
				}
				return Encoding.Unicode.GetString(MemoryStream.ToArray());
			}
		}
	}
}

