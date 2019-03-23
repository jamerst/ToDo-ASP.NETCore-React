using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using sample_app.Models;
using sample_app.Services;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace sample_app.Controllers {
    [Authorize(Roles = "Admin")]
    public class CreateController : AdminController {
        public CreateController(ToDoContext context, IConfiguration configuration) : base(context, configuration) {}

        [HttpPost]
        public async Task<IActionResult> createUser(string email, string password, bool admin) {
            var user = await _context.users.FirstOrDefaultAsync(u => u.email == email);
            if (user != default(User)) {
                return Json(new jsonResponse { success = false, errMsg = "Email already in use" } );
            }

            user.email = email;

            user.passHashIterations = Convert.ToInt32(configuration["passHashIterations"]);
            user.passHashSize = Convert.ToInt32(configuration["passHashSize"]);
            user.passSaltSize = Convert.ToInt32(configuration["passSaltSize"]);

            user.passwordSalt = AuthService.generateSalt(user.passSaltSize);
            user.passwordHash = AuthService.hashPassword(password, user.passwordSalt, user.passHashIterations, user.passHashSize);

            user.admin = admin;

            try {
                _context.users.Add(user);
                _context.SaveChanges();
            } catch {
                return Json(new jsonResponse {success = false, errMsg = "Failed to create user record"});
            }

            return Json(new jsonResponse {success = true});
        }

        [HttpPost]
        public async Task<IActionResult> createList(int userId, string listName) {
            User user = await _context.users.Include(u => u.lists).FirstOrDefaultAsync(u => u.id == userId);
            // equivalent SQL (to get lists): SELECT * FROM lists WHERE userid = userId LIMIT 1;

            if (user == default(User)) {
                return Json(new jsonResponse { success = false, errMsg = "UID does not exist" });
            }

            ToDoList newList = new ToDoList();
            newList.name = listName;

            user.lists.Add(newList);

            try {
                _context.SaveChanges();
            } catch {
                return Json(new jsonResponse {success = false, errMsg = "Failed to create new list"});
            }

            return Json(new jsonResponse {success = true});
        }

        [HttpPost]
        public async Task<IActionResult> createItem(int listId, string text, bool complete) {
            ToDoList list = await _context.lists.Include(l => l.items).FirstOrDefaultAsync(l => l.id == listId);
            // equivalent SQL (to get items): SELECT * FROM items WHERE listid = listId LIMIT 1;

            // if no list returned
            if (list == default(ToDoList)) {
                return Json(new jsonResponse {success = false, errMsg = "List not found"});
            }

            // create new list
            Item newItem = new Item();
            newItem.text = text;
            newItem.complete = complete;

            try {
                list.items.Add(newItem);
                await _context.SaveChangesAsync();
            } catch {
                return Json(new jsonResponse {success = false, errMsg = "Failed to create new list item"});
            }

            return Json(new jsonResponse {success = true});
        }
    }
}