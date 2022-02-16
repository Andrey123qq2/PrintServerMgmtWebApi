using Infrastructure.PrintServer;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace API.Middlewares
{
    class ErrorHandlerStatusCodeFactory
    {
        private static readonly Dictionary<Type, int> _mapExceptionToStatusCode = new Dictionary<Type, int>
            {
                {  typeof(PrinterNotFoundException), StatusCodes.Status400BadRequest },
                {  typeof(UnauthorizedAccessException), StatusCodes.Status403Forbidden },
            };
        public static int Create(Type type)
        {
            if (!_mapExceptionToStatusCode.ContainsKey(type))
                return StatusCodes.Status500InternalServerError;
            return _mapExceptionToStatusCode[type];
        }

    }
}
