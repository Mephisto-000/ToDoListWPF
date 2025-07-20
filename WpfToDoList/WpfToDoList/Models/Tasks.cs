using System;


namespace WpfToDoList.Models
{
    public class Tasks
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public string? Priority { get; set; }
        public DateTime Date { get; set; }
    }
}
