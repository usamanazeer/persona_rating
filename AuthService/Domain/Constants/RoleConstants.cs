namespace AuthService.Domain.Constants;

public static class RoleConstants
{
    public static class Ids
    {
        public const int RegisteredUser = 1;
        public const int Verifier = 2;
        public const int AdminBackOffice = 3;
        public const int PublicFigure = 4;
    }
    public static class Names
    {
        public const string RegisteredUser = "user";
        public const string Verifier = "verifier";
        public const string AdminBackOffice = "admin";
        public const string PublicFigure = "figure";
    }
}