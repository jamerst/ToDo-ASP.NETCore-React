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

    public class ReadController : AdminController {
        public ReadController(ToDoContext context, IConfiguration configuration) : base(context, configuration) {}

        [HttpGet]
        public async Task<IActionResult> getUsers() {
            var users = await _context.users.Select(u => new User {id = u.id, email = u.email, admin = u.admin}).ToListAsync();
            // equivalent SQL: SELECT id, email, admin FROM users;

            return Json(new collectionResponse<User> {success = true, collection = users});
        }

        [HttpGet]
        public async Task<IActionResult> getLists(int id) {
            User user = await _context.users.Include(u => u.lists).ThenInclude(l => l.items).FirstOrDefaultAsync(u => u.id == id);

            if (user == default(User)) {
                return Json(new jsonResponse {success = false, errMsg = "UID does not exist"});
            }

            return Json(new collectionResponse<ToDoList> {success = true, collection = user.lists});
        }

        [HttpGet]
        public async Task<IActionResult> getItems(int id) {
            ToDoList list = await _context.lists.Include(l => l.items).FirstOrDefaultAsync(l => l.id == id);

            if (list == default(ToDoList)) {
                return Json(new jsonResponse {success = false, errMsg = "List ID does not exist"});
            }

            return Json(new collectionResponse<Item> {success = true, collection = list.items});
        }
    }
}