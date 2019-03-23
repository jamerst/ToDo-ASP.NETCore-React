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
    [Route("/api/admin/[action]/{id?}")]
    // base class used for all admin actions
    public class AdminController : Controller {
        protected readonly ToDoContext _context;
        protected IConfiguration configuration;

        public AdminController(ToDoContext context, IConfiguration configuration) {
            _context = context;
            this.configuration = configuration;
        }

        protected class jsonResponse {
            public bool success;
            public string errMsg;
        }

        protected class collectionResponse<T> {
            public bool success;
            public List<T> collection;
        }
    }

}