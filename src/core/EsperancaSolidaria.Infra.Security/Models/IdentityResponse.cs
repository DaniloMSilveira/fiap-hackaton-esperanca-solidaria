using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsperancaSolidaria.Infra.Security.Models
{
    public class IdentityResponse
    {
        public bool Success { get; private set; }
        public List<string> Errors { get; private set; }
        
        public IdentityResponse(string error)
        {
            Success = false;
            Errors = [error];   
        }

        public IdentityResponse(bool success, List<string> errors = null)
        {
            Success = success;
            Errors = errors;
        }
    }
}