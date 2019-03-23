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
    [Route("/api/list/[action]")]
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
            // Include and ThenInclude ensure that the child records in lists and items are also fetched
            // equivalent SQL (can't be done in a single statement): SELECT * FROM users WHERE id = currentUser; SELECT * FROM lists WHERE userid = currentUser; SELECT * FROM items WHERE listid IN (SELECT id FROM lists WHERE userid = currentUser)

            if (user == default(User)) {
                return Json(new listResponse { success = false, errMsg = "Invalid UID", code = 401 });
            }

            return Json(new listResponse { success = true, lists = user.lists });
        }

        [HttpPost]
        public async Task<IActionResult> createList(string listName) {
            User user = await _context.users.Include(u => u.lists).FirstOrDefaultAsync(u => u.id == currentUser); // get user and all their lists

            if (user == default(User)) {
                return Json(new jsonResponse { success = false, errMsg = "Invalid UID" });
            }

            ToDoList newList = new ToDoList();
            newList.name = listName;

            try {
                user.lists.Add(newList);
                // equivalent SQL: INSERT INTO lists VALUES (NULL,listName,currentUser);
                _context.SaveChanges();
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
                // equivalent SQL: INSERT INTO items VALUES(NULL,text,0,listId);
                _context.SaveChanges();
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
                // equivalent SQL: UPDATE items SET complete=1 WHERE id = itemId
                _context.SaveChanges();
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
                _context.Remove(list); // remove list - cascade delete removes items
                _context.SaveChanges();
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
                _context.SaveChanges();
            } catch {
                return Json(new jsonResponse { success = false, errMsg = "Failed to delete item" });
            }

            return Json(new jsonResponse {success = true});
        }
    }
}