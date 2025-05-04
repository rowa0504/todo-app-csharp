using Microsoft.AspNetCore.Mvc;
using TodoApp.Models; // 修正
using Microsoft.AspNetCore.Authorization;  // この名前空間を追加
using System.Linq;
using TodoApp.Data; // Ensure this matches the namespace where TodoContext is defined

namespace TodoApp.Controllers
{
    [Authorize]  // ここで全てのアクションに認証を要求
    public class TodoController : Controller
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
        }

        // GET: /Todo
        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            var totalCount = _context.Todos.Count();
            var todos = _context.Todos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return View(todos);
        }

        // POST: /Todo/Create
        [HttpPost]
        public IActionResult Create(string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                var todo = new TodoApp.Models.Todo // 修正
                {
                    Title = title,
                    IsDone = false,
                    // Define a default due date or remove this line if not needed
                    DueDate = DateTime.Now // Example: default due date set to 7 days from now
                };
                _context.Todos.Add(todo);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Done(int id)
        {
            var todo = _context.Todos.Find(id);
            if (todo == null)
            {
                return NotFound("指定されたTodoが見つかりませんでした。");
            }
            todo.IsDone = true;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: /Todo/Edit/3
        public IActionResult Edit(int id)
        {
            var todo = _context.Todos.Find(id);
            if (todo == null)
            {
                return NotFound("Todoが見つかりませんでした。");
            }
            return View(todo);
        }

        // POST: /Todo/Edit/3
        [HttpPost]
        public IActionResult Edit(int id, string title, DateTime? dueDate)
        {
            var todo = _context.Todos.Find(id);
            if (todo == null)
            {
                return NotFound("Todoが見つかりませんでした。");
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                todo.Title = title;
            }

            // 期限が入力された場合、期限を更新
            if (dueDate.HasValue)
            {
                todo.DueDate = dueDate.Value;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: /Todo/Delete/3
        public IActionResult Delete(int id)
        {
            var todo = _context.Todos.Find(id);
            if (todo == null)
            {
                return NotFound("Todoが見つかりませんでした。");
            }

            return View(todo);
        }

        // POST: /Todo/Delete/3
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var todo = _context.Todos.Find(id);
            if (todo == null)
            {
                return NotFound("Todoが見つかりませんでした。");
            }

            _context.Todos.Remove(todo);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: /Todo/Details/5
        public IActionResult Details(int id)
        {
            var todo = _context.Todos.Find(id);
            if (todo == null)
            {
                return NotFound("Todoが見つかりませんでした。");
            }
            return View(todo);
        }
    }
}
