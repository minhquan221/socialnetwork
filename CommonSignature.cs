using Jose;
using OpenSSL.X509Certificate2Provider;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using JWTLibrary;

namespace SocialWeb.Common
{
    public class CommonSignature
    {
        private static CommonSignature _current = new CommonSignature();

        public static CommonSignature Current
        {
            get
            {
                return _current != null ? _current : new CommonSignature();
            }
        }

        public string SignWithLibrary<T>(T data, string privateKey) where T : new()
        {
            return JWTHelper.Current.Sign<T>(data, privateKey, JWTLibrary.Helper.Algorithm.RSA, JWTLibrary.Helper.KeySize.S256);
        }

        public bool Verify(string token, string publicKey)
        {
            return JWTHelper.Current.Validate(token, publicKey, JWTLibrary.Helper.KeySize.S256);
        }

        public T Decode<T>(string token, string publicKey) where T : new()
        {
            return JWTHelper.Current.Decode<T>(token, publicKey, JWTLibrary.Helper.KeySize.S256);
        }
    }

}