using System;
using System.Collections.Generic;
using System.Net.Http;

namespace FileStorage.Authentication
{
    public class AuthenticationOptions
    {
        public Dictionary<string, string> Resources { get; set; }
        internal List<AuthOptionDefinition> AuthData { get; private set; }

        public void Parse()
        {
            AuthData = new List<AuthOptionDefinition>();

            foreach (var kvp in Resources)
            {
                var tmp = kvp.Key.Split(' ');
                var tmp1 = kvp.Value.Split(':');
                AuthData.Add(new AuthOptionDefinition
                {
                    Method = tmp[0],
                    Resource = tmp[1],
                    UserName = tmp1[0],
                    Password = tmp1[1]
                });
            }
        }
    }

    internal class AuthOptionDefinition
    {
        public string Method { get; set; }
        public string Resource { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}