﻿using CodeFlix.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CodeFlix.Data
{
    public class DBInitializer
    {
        public DBInitializer(CodeFlixContext? context, RoleManager<CodeFlixRole>? roleManager, UserManager<CodeFlixUser>? userManager)
        {
            CodeFlixRole codeFlixRole;
            CodeFlixUser codeFlixUser;

            if (context != null)
            {
                context.Database.Migrate();
                context.SaveChanges();
            }
            if (roleManager != null)
            {
                if (roleManager.Roles.Count() == 0)
                {
                    codeFlixRole = new CodeFlixRole("Administrator");
                    roleManager.CreateAsync(codeFlixRole).Wait();
                    codeFlixRole = new CodeFlixRole("ContentAdmin");
                    roleManager.CreateAsync(codeFlixRole).Wait();
                }
            }
            if (userManager != null)
            {
                if (userManager.Users.Count() == 0)
                {
                    codeFlixUser = new CodeFlixUser();
                    codeFlixUser.UserName = "Administrator";
                    codeFlixUser.Email = "admin@codeflix.com";
                    codeFlixUser.Name = "CodeFlix Admin";
                    codeFlixUser.PhoneNumber = "1112223344";
                    codeFlixUser.Passive = false;
                    userManager.CreateAsync(codeFlixUser, "Admin123!").Wait();
                    userManager.AddToRoleAsync(codeFlixUser, "Administrator").Wait();

                    codeFlixUser = new CodeFlixUser();
                    codeFlixUser.UserName = "ContentAdmin";
                    codeFlixUser.Email = "contentadmin@codeflix.com";
                    codeFlixUser.Name = "CodeFlix Content Admin";
                    codeFlixUser.PhoneNumber = "1234567890";
                    codeFlixUser.Passive = false;
                    userManager.CreateAsync(codeFlixUser, "ContentAdmin123!").Wait();
                    userManager.AddToRoleAsync(codeFlixUser, "ContentAdmin").Wait();
                }
            }
        }
    }
}
