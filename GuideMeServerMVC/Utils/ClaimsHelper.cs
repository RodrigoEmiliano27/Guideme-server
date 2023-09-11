using System.Security.Claims;
using System.Security.Principal;

namespace GuideMeServerMVC.Utils
{
    public static class ClaimsHelper
    {
        public static int GetIntClaim(ClaimsIdentity _claims, string nome)
        {
            string claimEncontrado = string.Empty;
            if (_claims != null)
                claimEncontrado = _claims.FindFirst(nome).Value;

            return !string.IsNullOrEmpty(claimEncontrado) ? Convert.ToInt32(claimEncontrado) : -1;
        }
    }
}
