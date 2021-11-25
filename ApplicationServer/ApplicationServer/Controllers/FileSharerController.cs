using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ApplicationServer.Models;
using ApplicationServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApplicationServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileSharerController : ControllerBase
    {

        private readonly IdentityRepository _identity;
        private readonly FileRepository _file;


        public FileSharerController(IdentityRepository identity, FileRepository file)
        {
            _identity = identity;
            _file = file;
        }

        [HttpGet("file")]
        public async Task<ActionResult<GetFileResponse>> GetFile([FromQuery]string url, [FromQuery]string taggedUsername, [FromQuery]string signature)
        {
            var req = new GetFileRequest(url, taggedUsername);
            var hash = req.GetSHA256();
            if (!await CheckSignature(hash, Convert.FromBase64String(signature), taggedUsername)) return Unauthorized();
            var res = await _file.GetFile(req);
            if (res is null) return NotFound();
            return Ok(res);
        }

        [HttpPost]
        public async Task<ActionResult<SubmitFileResponse>> SubmitFile(SubmitFileSignedRequest request)
        {
            var (req, signature) = request;
            var hash = req.GetSHA256();
            if (!await CheckSignature(hash, Convert.FromBase64String(signature), req.TaggedUsername))
                return Unauthorized();
            var res = await _file.StoreFile(req);
            if (res is null) return StatusCode(500);
            return Ok(res);
        }

        [HttpDelete("file")]
        public async Task<ActionResult> DeleteFile([FromBody]DeleteFileSignedRequest request)
        {
            var (req, signature) = request;
            var hash = req.GetSHA256();
            if (!await CheckSignature(hash, Convert.FromBase64String(signature), req.TaggedUsername))
                return Unauthorized();
            var res = await _file.DeleteFile(req);
            if (!res) return NotFound();
            return StatusCode(204);
        }

        [HttpPost("share")]
        public async Task<ActionResult<SharingFileResponse>> ShareFile([FromBody]SharingFileSignedRequest request)
        {
            var (req, signature) = request;
            var hash = req.GetSHA256();
            if (!await CheckSignature(hash, Convert.FromBase64String(signature), req.CallerTaggedUsername))
                return Unauthorized();
            var res = await _file.ShareFile(req);
            if (res is null) return StatusCode(500);
            return Ok(res);
        }

        [HttpDelete("unshare")]
        public async Task<ActionResult<SharingFileResponse>> UnshareFile([FromBody]UnsharingFileSignedRequest request)
        {
            var (req, signature) = request;
            var hash = req.GetSHA256();
            if (!await CheckSignature(hash, Convert.FromBase64String(signature), req.CallerTaggedUsername))
                return Unauthorized();
            var res = await _file.UnshareFile(req);
            if (res is null) return StatusCode(500);
            return Ok(res);
        }

        private async Task<bool> CheckSignature(byte[] hash, byte[] signature,string taggedUsername)
        {
            var key = await _identity.GetPublicKey(taggedUsername);
            if (key == null) return false;
            var rsa = RSA.Create();
            rsa.ImportParameters(key.ToRsaParams());
            var deformatter = new RSAPKCS1SignatureDeformatter();
            deformatter.SetKey(rsa);
            deformatter.SetHashAlgorithm("SHA256");
            return deformatter.VerifySignature(hash, signature);
        }
    }
}