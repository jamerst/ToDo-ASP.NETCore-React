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
    public class DeleteController : AdminController {
        public DeleteController(ToDoContext context, IConfiguration configuration) : base(context, configuration) {}

         [HttpPost]
        public async Task<IActionResult> deleteUser(int id) {
            User user = await _context.users.FirstOrDefaultAsync(u => u.id == id);
            //equivalent SQL: SELECT * FROM users WHERE users.id = id LIMIT 1;

            if (user == default(User)) {
                return Json(new jsonResponse { success = false, errMsg = "UID does not exist"});
            }

            try {
                _context.Remove(user); // deleting user deletes all associated lists and items through CASCADE DELETE
                // equivalent SQL: DELETE FROM users WHERE users.id = id;
                _context.SaveChanges();
            } catch {
                return Json(new jsonResponse {success = false, errMsg = "Failed to delete user"});
            }

            return Json(new jsonResponse {success = true});
        }

        [HttpPost]
        public async Task<IActionResult> deleteList(int id) {
            ToDoList list = await _context.lists.FirstOrDefaultAsync(l => l.id == id);
            // equivalent SQL: SELECT * FROM lists WHERE list.id = id LIMIT 1;

            if (list == default(ToDoList)) {
                return Json(new jsonResponse { success = false, errMsg = "List ID does not exist"});
            }

            try {
                _context.Remove(list);
                // equivalent SQL: DELETE FROM lists WHERE lists.id = id;
                _context.SaveChanges();
            } catch {
                return Json(new jsonResponse {success = false, errMsg = "Failed to delete list"});
            }

            return Json(new jsonResponse {success = true});
        }

        [HttpPost]
        public async Task<IActionResult> deleteItem(int id) {
            Item item = await _context.items.FirstOrDefaultAsync(i => i.id == id);
            // equivalent SQL: SELECT * FROM items WHERE items.id = id LIMIT 1;

            if (item == default(Item)) {
                return Json(new jsonResponse { success = false, errMsg = "Item ID does not exist"});
            }

            try {
                _context.Remove(item);
                // equivalent SQL: DELETE FROM items WHERE items.id = id;
                _context.SaveChanges();
            } catch {
                return Json(new jsonResponse {success = false, errMsg = "Failed to delete item"});
            }

            return Json(new jsonResponse {success = true});
        }
    }
}