using BlogSystem.Models;

namespace BlogSystem.Services
{
    public interface ITokenHandler
    {
        public string CreateJWT(BsUser userInfo);
        public bool ValidateJWT(string token);
        public string DecodeJWT(string token);
    }
}
