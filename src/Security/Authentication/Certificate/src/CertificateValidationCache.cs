// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Microsoft.AspNetCore.Authentication.Certificate
{
    /// <summary>
    /// MemoryCache based implementation used to store <see cref="AuthenticateResult"/> results after the certificate has been validated
    /// </summary>
    public class CertificateValidationCache : ICertificateValidationCache
    {
        private MemoryCache _cache;

        public CertificateValidationCache()
        {
             _cache = new MemoryCache(new MemoryCacheOptions { SizeLimit = CacheSize });
        }


        /// <summary>
        /// The expiration that should be used for entries in the MemoryCache, defaults to 30 minutes.
        /// </summary>
        public TimeSpan CacheEntryExpiration { get; set; } = TimeSpan.FromMinutes(30);

        /// <summary>
        /// How many validated certificate results to store in the cache, defaults to 1024.
        /// </summary>
        public int CacheSize { get; set; } = 1024;

        /// <summary>
        /// Get the <see cref="AuthenticateResult"/> for the connection and certificate.
        /// </summary>
        /// <param name="context">The HttpContext.</param>
        /// <param name="certificate">The certificate.</param>
        /// <returns>the <see cref="AuthenticateResult"/></returns>
        public AuthenticateResult Get(HttpContext context, X509Certificate2 certificate)
            => _cache.Get(ComputeKey(context, certificate)) as AuthenticateResult;

        /// <summary>
        /// Store a <see cref="AuthenticateResult"/> for the connection and certificate
        /// </summary>
        /// <param name="context">The HttpContext.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="result">the <see cref="AuthenticateResult"/></param>
        public void Put(HttpContext context, X509Certificate2 certificate, AuthenticateResult result)
            =>  _cache.Set(ComputeKey(context, certificate), result, new MemoryCacheEntryOptions().SetSize(1).SetSlidingExpiration(CacheEntryExpiration));

        private string ComputeKey(HttpContext context, X509Certificate2 certificate)
            => $"{certificate.GetCertHashString(HashAlgorithmName.SHA256)}";
    }

}
