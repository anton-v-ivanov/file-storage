using System;
using Microsoft.Extensions.Options;

namespace FileStorage.Authentication
{
    internal class AuthValidator : IAuthValidator
    {
        private readonly AuthenticationOptions _options;

        public AuthValidator(IOptions<AuthenticationOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }

        public bool Validate(string username, string password, string method, string resource)
        {
            var authData = GetResourceData(method, resource);
            if (authData == null)
                return false;
            return authData.UserName == username && authData.Password == password;
        }

        private AuthOptionDefinition GetResourceData(string method, string resource)
        {
            foreach (var data in _options.AuthData)
            {
                if (data.Method.Equals(method, StringComparison.OrdinalIgnoreCase) && ResourcesEqual(data.Resource, resource))
                {
                    return data;
                }
            }
            return null;
        }

        private bool ResourcesEqual(string dataResource, string resource)
        {
            if (dataResource.Equals(resource, StringComparison.OrdinalIgnoreCase))
                return true;

            if (!dataResource.EndsWith("{id}"))
                return false;

            var pureDataResource = dataResource.Replace("/{id}", string.Empty);
            var pureResource = resource.Substring(0, resource.LastIndexOf('/'));
            return pureDataResource.Equals(pureResource, StringComparison.OrdinalIgnoreCase);
        }
    }
}