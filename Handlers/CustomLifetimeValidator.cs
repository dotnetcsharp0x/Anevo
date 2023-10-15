using Microsoft.IdentityModel.Tokens;

namespace Anevo.Handlers {
    public class CustomLifetime {
        static public bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken tokenToValidate, TokenValidationParameters @param) {
            if (expires != null) {
                return expires > DateTime.UtcNow;
            }
            return false;
        }
    }
}