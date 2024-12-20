﻿using System.Net;

namespace JwtSample.Middleware;

public class ErrorResult
{
    public ErrorResult()
    {

    }

    public ErrorResult(HttpStatusCode statusCode)
    {
        Message = statusCode switch
        {
            HttpStatusCode.BadRequest => "The request is incorrect or corrupt, and the server can't understand it.",
            HttpStatusCode.Unauthorized => "The request lacks valid permission for the target resource.",
            _ => "An unspecified error occurs"
        };
    }

    public string Message { get; set; } = string.Empty;
}