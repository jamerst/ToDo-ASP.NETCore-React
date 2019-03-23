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
    public class UpdateController : AdminController {
        public UpdateController(ToDoContext context, IConfiguration configuration) : base(context, configuration) {}

        [HttpPost]
        public async Task<IActionResult> updateUser(int userId, string email, string password, bool admin) {
            User user = await _context.users.FirstOrDefaultAsync(u => u.id == userId);

            if (user == default(User)) {
                return Json(new jsonResponse {success = false, errMsg = "UID does not exist"});
            }

            user.email = email;

            user.passHashIterations = Convert.ToInt32(configuration["passHashIterations"]);
            user.passHashSize = Convert.ToInt32(configuration["passHashSize"]);
            user.passSaltSize = Convert.ToInt32(configuration["passSaltSize"]);

            user.passwordSalt = AuthService.generateSalt(user.passSaltSize);
            user.passwordHash = AuthService.hashPassword(password, user.passwordSalt, user.passHashIterations, user.passHashSize);

            user.admin = admin;

            try {
                _context.SaveChanges();
            } catch {
                return Json(new jsonResponse {success = false, errMsg = "Failed to update user record"});
            }

            return Json(new jsonResponse {success = true});
        }

        [HttpPost]
        public async Task<IActionResult> updateList(int listId, string listName) {
            ToDoList list = await _context.lists.FirstOrDefaultAsync(l => l.id == listId);
            
            if (list == default(ToDoList)) {
                return Json(new jsonResponse {success = false, errMsg = "List ID does not exist"});
            }

            list.name = listName;

            try {
                _context.SaveChanges();
            } catch {
                return Json(new jsonResponse {success = false, errMsg = "Failed to update list"});
            }

            return Json(new jsonResponse {success = true});
        }

        public async Task<IActionResult> updateItem(int itemId, string text, bool complete) {
            Item item = await _context.items.FirstOrDefaultAsync(i => i.id == itemId);
            
            if (item == default(Item)) {
                return Json(new jsonResponse {success = false, errMsg = "Item ID does not exist"});
            }

            item.text = text;

            try {
                _context.SaveChanges();
            } catch {
                return Json(new jsonResponse {success = false, errMsg = "Failed to update item"});
            }

            return Json(new jsonResponse {success = true});
        }
    }
}