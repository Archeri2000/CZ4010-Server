using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using IdentityServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IdentityDbContext _dbContext;

        public IdentityController(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("GetIdentity")]
        public async Task<RSAPubKey> GetIdentity([FromQuery] string taggedUsername)
        {
            return (await _dbContext.Identities.FindAsync(taggedUsername)).PublicKey;
        }
        
        [HttpPost("CreateIdentity")]
        public async Task<ActionResult<CreateIdentityResponse>> CreateIdentity([FromBody] CreateIdentitySignedRequest request)
        {
            if (!VerifyRequest(request)) return Unauthorized();
            var taggedUsername = request.Request.Username + GenerateTag();
            while (await _dbContext.Identities.AnyAsync(x => x.TaggedUsername == taggedUsername))
            {
                taggedUsername = request.Request.Username + GenerateTag();
            }
            await _dbContext.Identities.AddAsync(new IdentityModel(taggedUsername, request.Request.PublicKey));
            await _dbContext.SaveChangesAsync();
            return Ok(new CreateIdentityResponse(request.Request.PublicKey, taggedUsername));
        }

        private static bool VerifyRequest(CreateIdentitySignedRequest request)
        {
            var (createIdentityRequest, signature) = request;
            var publicKey = createIdentityRequest.PublicKey.ToRsaParams();
            var rsa = RSA.Create();
            rsa.ImportParameters(publicKey);
            var deformatter = new RSAPKCS1SignatureDeformatter(rsa);
            deformatter.SetHashAlgorithm("SHA256");
            var hash = createIdentityRequest.GetSHA256();
            return deformatter.VerifySignature(hash, Convert.FromBase64String(signature));
        }

        private static string GenerateTag()
        {
            var rng = new Random();
            return $"#{rng.Next(9999):D4}";
        }
    }
}