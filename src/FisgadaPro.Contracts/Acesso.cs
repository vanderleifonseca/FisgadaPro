namespace FisgadaPro.Contracts;
public sealed record LoginRequest(string Email, string Senha);
public sealed record TokenResponse(string AccessToken, int ExpiresIn);
public sealed record SessaoDto(string Nome, string Perfil, IReadOnlyList<ClubeDto> Clubes);
