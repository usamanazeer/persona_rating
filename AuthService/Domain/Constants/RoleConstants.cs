using AuthService.Domain.Entities;

namespace AuthService.Domain.Constants;

public static class RoleIds
{
    public const int RegisteredUser = 1;
    public const int Verifier = 2;
    public const int AdminBackOffice = 3;
    public const int PublicFigure = 4;
}
public static class RoleNames {
    public const string RegisteredUser = "Registered User";
    public const string Verifier = "Verifier";
    public const string AdminBackOffice = "Admin";
    public const string PublicFigure = "PublicFigure";
}