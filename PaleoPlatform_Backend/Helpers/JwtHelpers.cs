namespace PaleoPlatform_Backend.Helpers
{
    public static class JwtHelpers
    {
        public static string GetTokenFromRequest(HttpRequest request)
        {
            var token = request.Headers["Authorization"]
                .FirstOrDefault()?
                .Split(" ")
                .Last();

            if (string.IsNullOrEmpty(token))
            {
                token = request.Cookies["auth_token"];
            }

            return token;
        }
    }

}
