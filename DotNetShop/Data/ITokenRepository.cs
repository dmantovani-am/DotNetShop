namespace DotNetShop.Data;

public interface ITokenRepository
{
    Token GenerateToken(int days);
}
