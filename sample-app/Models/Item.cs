using Newtonsoft.Json;

namespace sample_app.Models {
    public class Item {
        public int id {get; set;}
        public string text {get; set;}
        public bool complete {get; set;}
        [JsonIgnore] // ignore field when serialising - causes a loop otherwise

        public ToDoList list {get; set;}
    }
}