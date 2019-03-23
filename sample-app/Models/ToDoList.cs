using Newtonsoft.Json;
using System.Collections.Generic;

namespace sample_app.Models {
    public class ToDoList {
        public int id {get; set;}
        public string name {get; set;}
        public List<Item> items {get; set;}
        [JsonIgnore]
        public User user {get; set;}
    }
}