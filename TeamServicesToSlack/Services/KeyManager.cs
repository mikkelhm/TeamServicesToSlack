using System;

namespace TeamServicesToSlack.Services
{
    public class KeyManager
    {
        // FAKE it untill you make it
        public KeyManager()
        {
            
        }

        public static string GetSecret(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }

    }
}
