//using API.partonair.Token;
//using API.partonair_v01.Token;
//using DomainLayer.partonair.Entities;
//using InfrastructureLayer.partonair.Persistence;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Security.Claims;

//namespace API.partonair.Controllers
//{
//    [AllowAnonymous]
//    [Route("api/[controller]")]
//    [ApiController]
//    public class SigninGoogleController : ControllerBase
//    {
//        private readonly AppDbContext _appDbContext;
//        private readonly HttpContext _http;
//        private readonly TokenService _tokenService;

//        public SigninGoogleController(AppDbContext appDbContext, HttpContext http, TokenService tokenService)
//        {
//            _appDbContext = appDbContext;
//            _http = http;
//            _tokenService = tokenService;
//        }

//        [HttpGet]
//        public IActionResult Login(string returnUrl = "/")
//        {
//            return Challenge(new AuthenticationProperties { RedirectUri = returnUrl });
//        }

//        [HttpGet("/signin-callback")]
//        public async Task<IActionResult> ExternalLoginCallback()
//        {
//            var result = await _http.AuthenticateAsync("External");
//            var principal = result.Principal;

//            if (principal is null || !principal.Identities.Any(i => i.IsAuthenticated))
//                return Unauthorized();

//            var email = principal.FindFirstValue(ClaimTypes.Email);
//            var name = principal.FindFirstValue(ClaimTypes.Name);
//            var provider = result.Properties?.Items["LoginProvider"];
//            var providerUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

//            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
//            if (user == null)
//            {
//                user = new User
//                {
//                    Email = email!,
//                    Role = DomainLayer.partonair.Enums.Roles.Visitor,
//                };

//                _appDbContext.Users.Add(user);
//                await _appDbContext.SaveChangesAsync();
//            }

//            var token = _tokenService.CreateToken(user);

//            return Redirect($"https://localhost:5002/auth-callback?token={token}");
//        }


//    }

//    public class ApplicationUser
//    {
//        public string Id { get; set; }
//        public string Email { get; set; } = string.Empty;
//        public string Role { get; set; } = "Visitor";
//        public DateTime CreatedAt { get; set; }

//        // Pour provider externe
//        public string Provider { get; set; } = string.Empty;
//        public string ProviderUserId { get; set; } = string.Empty;
//    }

//}
///*
// app.MapGet("/signin-callback", async (HttpContext http, ApplicationDbContext db, ITokenService tokenService) =>
//{
//    var result = await http.AuthenticateAsync();
//    var principal = result.Principal;

//    if (principal is null || !principal.Identities.Any(i => i.IsAuthenticated))
//        return Results.Unauthorized();

//    var email = principal.FindFirstValue(ClaimTypes.Email);
//    var name = principal.FindFirstValue(ClaimTypes.Name);

//    var user = db.Users.FirstOrDefault(u => u.Email == email);
//    if (user is null)
//    {
//        user = new ApplicationUser { Email = email!, Name = name!, CreatedAt = DateTime.UtcNow };
//        db.Users.Add(user);
//        await db.SaveChangesAsync();
//    }

//    var token = tokenService.GenerateToken(user);

//    return Results.Redirect($"https://localhost:5002/auth-callback?token={token}");
//});
 
// */