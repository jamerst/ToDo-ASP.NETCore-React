using System.Collections.Generic;

namespace sample_app.Models {
        public class User {
        public int id {get; set;}
        public string email {get; set;}
        public string passwordHash {get; set;}
        public string passwordSalt {get; set;}
        public int passHashSize {get; set;}
        public int passHashIterations {get; set;}
        public int passSaltSize {get; set;}
        public bool admin {get; set;}
        public List<ToDoList> lists {get; set;}
    }
}