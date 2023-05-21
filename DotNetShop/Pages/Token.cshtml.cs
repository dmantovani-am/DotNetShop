using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetShop.Pages
{
    [Authorize]
    public class JwtTokenModel : PageModel
    {
        readonly ITokenRepository _tokenRepository;

        public Token? Token { get; set; }

        public JwtTokenModel(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        public void OnGet(int days = 30)
        {
            Token = _tokenRepository.GenerateToken(days);
        }
    }
}
