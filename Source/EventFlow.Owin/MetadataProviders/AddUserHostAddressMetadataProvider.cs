﻿// The MIT License (MIT)
//
// Copyright (c) 2015 Rasmus Mikkelsen
// https://github.com/rasmus/EventFlow
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.Linq;
using EventFlow.Aggregates;
using EventFlow.EventStores;
using Microsoft.Owin;

namespace EventFlow.Owin.MetadataProviders
{
    public class AddUserHostAddressMetadataProvider : IMetadataProvider
    {
        private readonly IOwinContext _owinContext;

        private static readonly IEnumerable<string> HeaderPriority = new[]
            {
                "X-Forwarded-For",
                "HTTP_X_FORWARDED_FOR",
                "X-Real-IP",
                "REMOTE_ADDR",
            };

        public AddUserHostAddressMetadataProvider(
            IOwinContext owinContext)
        {
            _owinContext = owinContext;
        }

        public IEnumerable<KeyValuePair<string, string>> ProvideMetadata<TAggregate>(
            string id,
            IAggregateEvent aggregateEvent,
            IMetadata metadata)
            where TAggregate : IAggregateRoot
        {
            var headerValue = HeaderPriority
                .Select(h =>
                    {
                        string[] value;
                        return _owinContext.Request.Headers.TryGetValue(h, out value)
                            ? string.Join(string.Empty, value)
                            : string.Empty;
                    })
                .FirstOrDefault(v => !string.IsNullOrEmpty(v));

            if (string.IsNullOrEmpty(headerValue))
            {
                yield break;
            }

            yield return new KeyValuePair<string, string>("user_host_address", headerValue);
        }
    }
}