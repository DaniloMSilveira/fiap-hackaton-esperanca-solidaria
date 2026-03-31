using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EsperancaSolidaria.Infra.Security.Models
{
    public class IdentityCustomUser : IdentityUser
    {
        [Column(TypeName = "nvarchar(256)")]
        [MaxLength(256)]
        public string? Nome { get; set; }
    }
}