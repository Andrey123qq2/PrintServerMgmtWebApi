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
                {  typeof(PrinterCreateConflictException), StatusCodes.Status409Conflict },
            };
        public static int Create(Type type)
        {
            if (!_mapExceptionToStatusCode.TryGetValue(type, out int code))
                return StatusCodes.Status500InternalServerError;
            return code;
        }
    }
}