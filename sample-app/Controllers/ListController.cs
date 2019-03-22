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
    [Authorize]
    public class ListController : Controller {
        private readonly int currentUser;
        private readonly ToDoContext _context;

        public ListController(ToDoContext dbContext, IHttpContextAccessor httpContext) {
            currentUser = AuthService.getCurrentUser(httpContext);
            _context = dbContext;
        }

        class jsonResponse {
            public bool success;
            public string errMsg;
        }

        class listResponse {
            public bool success;
            public string errMsg;
            public List<ToDoList> lists;
            public int code;
        }

        [HttpGet]
        public async Task<IActionResult> getLists() {
            User user = await _context.users.Include(u => u.lists).ThenInclude(l => l.items).FirstOrDefaultAsync(u => u.id == currentUser);

            if (user == default(User)) {
                return Json(new listResponse { success = false, errMsg = "Invalid UID", code = 401 });
            }

            return Json(new listResponse { success = true, lists = user.lists });
        }

        [HttpPost]
        public async Task<IActionResult> createList(string listName) {
            User user = await _context.users.Include(u => u.lists).FirstOrDefaultAsync(u => u.id == currentUser);

            if (user == default(User)) {
                return Json(new jsonResponse { success = false, errMsg = "Invalid UID" });
            }

            ToDoList newList = new ToDoList();
            newList.name = listName;

            user.lists.Add(newList);

            try {
                await _context.SaveChangesAsync();
            } catch {
                return Json(new jsonResponse {success = false, errMsg = "Failed to create new list"});
            }

            return Json(new jsonResponse {success = true});
        }

        [HttpPost]
        public async Task<IActionResult> createItem(int listId, string text) {
            User user = await _context.users.Include(u => u.lists).FirstOrDefaultAsync(u => u.id == currentUser);

            if (user == default(User)) {
                return Json(new jsonResponse { success = false, errMsg = "Invalid UID" });
            }

            ToDoList list = await _context.lists.Include(l => l.items).FirstOrDefaultAsync(l => l.id == listId);

            // if no list returned
            if (list == default(ToDoList)) {
                return Json(new jsonResponse {success = false, errMsg = "List not found"});
            }

            // if list not in user.lists
            if (!user.lists.Contains(list)) {
                return Json(new jsonResponse { success = false, errMsg = "Access denied, this list does not belong to you" });
            }

            // create new list
            Item newItem = new Item();
            newItem.text = text;

            try {
                list.items.Add(newItem);
                await _context.SaveChangesAsync();
            } catch {
                return Json(new jsonResponse {success = false, errMsg = "Failed to create new list item"});
            }

            return Json(new jsonResponse {success = true});
        }

        public async Task<IActionResult> updateItem(int itemId, bool complete) {
            User user = await _context.users.Include(u => u.lists).ThenInclude(l => l.items).FirstOrDefaultAsync(u => u.id == currentUser);

            if (user == default(User)) {
                return Json(new jsonResponse { success = false, errMsg = "Invalid UID" });
            }
            
            Item item = await _context.items.FirstOrDefaultAsync(i => i.id == itemId);

            // if no item returned
            if (item == default(Item)) {
                return Json(new jsonResponse {success = false, errMsg = "Item id " + itemId + " not found"});
            }
            
            ToDoList list = await _context.lists.FirstOrDefaultAsync(l => l.items.Contains(item));

            // if list containing item does not belong to user
            if (!user.lists.Contains(list)) {
                return Json(new jsonResponse {success = false, errMsg = "Access denied, this list does not belong to you"});
            }

            try {
                item.complete = complete;
                await _context.SaveChangesAsync();
            } catch {
                return Json(new jsonResponse { success = false, errMsg = "Failed to update item" });
            }
            
            return Json(new jsonResponse {success = true});
        }

        [HttpPost]
        public async Task<IActionResult> deleteList(int listId) {
            User user = await _context.users.Include(u => u.lists).FirstOrDefaultAsync(u => u.id == currentUser);

            if (user == default(User)) {
                return Json(new jsonResponse { success = false, errMsg = "Invalid UID" });
            }

            ToDoList list = await _context.lists.Include(l => l.items).FirstOrDefaultAsync(l => l.id == listId);

            if (list == default(ToDoList) && !user.lists.Contains(list)) {
                return Json(new jsonResponse {success = false, errMsg = "List does not exist"});
            }

            try {
                _context.RemoveRange(list.items); // delete all list items first
                _context.lists.Remove(list); // then remove list
                await _context.SaveChangesAsync();
            } catch {
                return Json(new jsonResponse { success = false, errMsg = "Failed to delete list" });
            }

            return Json(new jsonResponse { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> deleteItem(int itemId) {
            User user = await _context.users.Include(u => u.lists).ThenInclude(l => l.items).FirstOrDefaultAsync(u => u.id == currentUser);

            if (user == default(User)) {
                return Json(new jsonResponse { success = false, errMsg = "Invalid UID" });
            }
            
            Item item = await _context.items.FirstOrDefaultAsync(i => i.id == itemId);

            // if no item returned
            if (item == default(Item)) {
                return Json(new jsonResponse {success = false, errMsg = "Item not found"});
            }
            
            ToDoList list = await _context.lists.FirstOrDefaultAsync(l => l.items.Contains(item));

            // if list containing item does not belong to user
            if (!user.lists.Contains(list)) {
                return Json(new jsonResponse {success = false, errMsg = "Access denied, this list does not belong to you"});
            }

            try {
                _context.items.Remove(item);
                await _context.SaveChangesAsync();
            } catch {
                return Json(new jsonResponse { success = false, errMsg = "Failed to delete item" });
            }

            return Json(new jsonResponse {success = true});
        }
    }
}