using System;
using System.IO;
using System.IO.Abstractions;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace PemToXml
{
    public class RsaConverter
    {
        private readonly IFileSystem _fileSystem;

        public RsaConverter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public RsaConverter() : this(new FileSystem())
        {

        }

        public string PemToXmlString(string pathToPemFile)
        {
            ValidatePathExist(pathToPemFile);
            using (var reader = File.OpenText(pathToPemFile))
            {
                var pemReader = new PemReader(reader);
                var pemObject = pemReader.ReadObject();

                RSA rsa;
                RSAParameters rsaParameters;
                switch (pemObject)
                {
                    case AsymmetricCipherKeyPair privatePublicPair:
                        rsa = RSA.Create();
                        var privateCertParameters = (privatePublicPair.Private as RsaPrivateCrtKeyParameters);
                        rsaParameters = DotNetUtilities.ToRSAParameters(privateCertParameters);
                        rsa.ImportParameters(rsaParameters);
                        return RsaParametersToXmlString(rsa.ExportParameters(true));
                    case RsaKeyParameters publicKeyParameters:
                        rsa = RSA.Create();
                        rsaParameters = DotNetUtilities.ToRSAParameters(publicKeyParameters);
                        rsa.ImportParameters(rsaParameters);
                        return RsaParametersToXmlString(rsa.ExportParameters(false));
                    default:
                        throw new ArgumentException("Invalid pem key format");
                }
            }

        }

        private string RsaParametersToXmlString(RSAParameters parameters)
        {

            return $"<RSAKeyValue>" +
                       $"<Modulus>{(parameters.Modulus != null ? Convert.ToBase64String(parameters.Modulus) : null)}</Modulus>" +
                       $"<Exponent>{(parameters.Exponent != null ? Convert.ToBase64String(parameters.Exponent) : null)}</Exponent>" +
                       $"<P>{(parameters.P != null ? Convert.ToBase64String(parameters.P) : null)}</P>" +
                       $"<Q>{(parameters.Q != null ? Convert.ToBase64String(parameters.Q) : null)}</Q>" +
                       $"<DP>{(parameters.DP != null ? Convert.ToBase64String(parameters.DP) : null)}</DP>" +
                       $"<DQ>{(parameters.DQ != null ? Convert.ToBase64String(parameters.DQ) : null)}</DQ>" +
                       $"<InverseQ>{(parameters.InverseQ != null ? Convert.ToBase64String(parameters.InverseQ) : null)}</InverseQ>" +
                       $"<D>{(parameters.D != null ? Convert.ToBase64String(parameters.D) : null)}</D>" +
                   $"</RSAKeyValue>";
        }

        private void ValidatePathExist(string path)
        {
            if (!_fileSystem.File.Exists(path))
            {
                throw new ArgumentException($"Path not exist: {path}");
            }
        }
    }
}