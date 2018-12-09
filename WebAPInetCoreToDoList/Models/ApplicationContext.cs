using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebAPInetCoreToDoList.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string adminRoleName = "admin";
            string userRoleName = "user";

            string adminEmail = "admin@mail.ru";
            string adminPassword = "123456";

            Role adminRole = new Role { Id = 1, Name = adminRoleName };
            Role userRole = new Role { Id = 2, Name = userRoleName };

            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, userRole });

            var admin = new User
            {
                Id = 1,
                Email = adminEmail,
                Password = adminPassword,
                RoleId = adminRole.Id
            };

            var user1 = new User
            {
                Id = 2,
                Email = "somemail@gmail.com",
                Password = "qwertyuiop",
                RoleId = userRole.Id
            };

            modelBuilder.Entity<User>().HasData(new User[] { admin, user1 });

            // adm
            var testItem = new TodoItem
            {
                Id = 1,
                Name = "Drink beer",
                IsComplete = false,
                UserId = admin.Id
            };

            var testItem2 = new TodoItem
            {
                Id = 2,
                Name = "Make kursach",
                IsComplete = true,
                UserId = admin.Id
            };

            // usr
            var testItem3 = new TodoItem
            {
                Id = 3,
                Name = "Make test task",
                IsComplete = false,
                UserId = user1.Id
            };

            var testItem4 = new TodoItem
            {
                Id = 4,
                Name = "Play quantum break",
                IsComplete = true,
                UserId = user1.Id
            };

            var testItem5 = new TodoItem
            {
                Id = 5,
                Name = "Learn web api",
                IsComplete = false,
                UserId = user1.Id
            };


            modelBuilder.Entity<TodoItem>().HasData(new TodoItem[] { testItem, testItem2, testItem3, testItem4, testItem5 });


            base.OnModelCreating(modelBuilder);
        }

    }
}
