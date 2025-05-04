using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{

    public class Todo
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public bool IsDone { get; set; }

        public DateTime? DueDate { get; set; }
    }
}


