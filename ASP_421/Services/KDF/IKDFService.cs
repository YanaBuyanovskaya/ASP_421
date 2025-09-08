namespace ASP_421.Services.KDF
{
    // Key Derivation Service by RFC 2998 https://datatracker.ietf.org/doc/html/rfc2898
    public interface IKDFService
    {
        String DK(String password, String salt); //Derived Key

    }
}
