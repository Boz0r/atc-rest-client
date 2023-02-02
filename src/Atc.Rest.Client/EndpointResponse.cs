using System;
using System.Collections.Generic;
using System.Net;

#pragma warning disable SA1402 // File may only contain a single type

namespace Atc.Rest.Client
{
    public class EndpointResponse : IEndpointResponse
    {
        public EndpointResponse(EndpointResponse response)
            : this(
                response?.IsSuccess ?? throw new System.ArgumentNullException(nameof(response)),
                response.StatusCode,
                response.Content,
                response.ContentObject,
                response.Headers)
        {
        }

        public EndpointResponse(
            bool isSuccess,
            HttpStatusCode statusCode,
            string content,
            object? contentObject,
            IReadOnlyDictionary<string, IEnumerable<string>> headers)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            Content = content;
            ContentObject = contentObject;
            Headers = headers;
        }

        public bool IsSuccess { get; }

        public HttpStatusCode StatusCode { get; }

        public string Content { get; }

        public object? ContentObject { get; }

        public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; }

        protected TResult CastContent<TResult>()
            where TResult : class
        {
            return ContentObject as TResult ??
                   throw new InvalidCastException($"ContentObject is not of type {typeof(TResult).Name}");
        }
    }

    public class EndpointResponse<T>
        : EndpointResponse
        where T : class
    {
        public EndpointResponse(EndpointResponse response)
            : base(response)
        {
        }

        public EndpointResponse(
            bool isSuccess,
            HttpStatusCode statusCode,
            string content,
            T? contentObject,
            IReadOnlyDictionary<string, IEnumerable<string>> headers)
            : base(isSuccess, statusCode, content, contentObject, headers)
        {
        }

        public T? SuccessContent => IsSuccess ? CastContent<T>() : null;
    }

    public class EndpointResponse<TSuccessContent, TErrorContent>
        : EndpointResponse
        where TSuccessContent : class
        where TErrorContent : class
    {
        public EndpointResponse(EndpointResponse response)
            : base(response)
        {
        }

        public EndpointResponse(
            bool isSuccess,
            HttpStatusCode statusCode,
            string content,
            object? contentObject,
            IReadOnlyDictionary<string, IEnumerable<string>> headers)
            : base(isSuccess, statusCode, content, contentObject, headers)
        {
        }

        public TSuccessContent? SuccessContent => IsSuccess ? CastContent<TSuccessContent>() : null;

        public TErrorContent? ErrorContent => !IsSuccess ? CastContent<TErrorContent>() : null;
    }
}