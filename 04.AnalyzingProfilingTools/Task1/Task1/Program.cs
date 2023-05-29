using System.Diagnostics;
using System.Security.Cryptography;

Stopwatch sw = Stopwatch.StartNew();
Random r = new Random();
byte[] salt = new byte[16];
r.NextBytes(salt);

sw.Start();
var pass = GeneratePasswordHashUsingSalt("Passw0rd", salt );
sw.Stop();

Console.WriteLine($"GeneratePasswordHashUsingSalt time={sw.ElapsedMilliseconds} ms.");
Console.ReadLine();

static string GeneratePasswordHashUsingSalt(string passwordText, byte[] salt)
{
    var iterate = 10000;
    var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);
    byte[] hash = pbkdf2.GetBytes(20);

    var passwordHash = Convert.ToBase64String(salt.Concat(hash).ToArray());
    return passwordHash;
}