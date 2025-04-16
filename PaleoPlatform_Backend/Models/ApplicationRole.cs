﻿using Microsoft.AspNetCore.Identity;

namespace PaleoPlatform_Backend.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(roleName) { }
    }
}
