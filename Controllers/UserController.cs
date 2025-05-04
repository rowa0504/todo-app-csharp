using Microsoft.AspNetCore.Mvc;
using TodoApp.Data;
using TodoApp.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace TodoApp.Controllers
{
    public class UserController : Controller
    {
        private readonly TodoContext _context;

        public UserController(TodoContext context)
        {
            _context = context;
        }

        // ユーザー一覧表示
        // ユーザー一覧表示
        public IActionResult Index()
        {
            // Data層のUserをModels層のUserに変換
            var users = _context.Users
                                .Select(u => new TodoApp.Models.User
                                {
                                    Id = u.Id,
                                    Username = u.Username ?? string.Empty,
                                    Password = u.Password
                                })
                                .ToList();
            return View(users);
        }


        // ユーザー登録フォーム
        public IActionResult Create()
        {
            return View();
        }

        // ユーザー登録処理
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TodoApp.Models.User user)
        {
            if (ModelState.IsValid)
            {
                // Model層のUserをそのままデータベースに保存
                _context.Users.Add(user);  // ここで TodoApp.Models.User を直接使用
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }



        // ユーザー詳細表示
        public IActionResult Details(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // ログインフォームの表示
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users
                    .FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

                if (user != null)
                {
                    // Cookie認証のためのClaimsを作成
                    var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username ?? string.Empty),
                new("UserId", user.Id.ToString())
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };

                    // 認証Cookieを発行
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties
                    );

                    // ReturnUrlが指定されていればそこへリダイレクト
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    // 指定がなければTodo一覧へ
                    return RedirectToAction("Index", "Todo");
                }

                ModelState.AddModelError("", "無効なユーザー名またはパスワードです。");
            }

            return View(model);
        }


        // ログアウト処理
        public IActionResult Logout()
        {
            // セッションからユーザー情報を削除
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Login");
        }

        // 編集画面表示
        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound("ユーザーが見つかりませんでした。");
            }

            return View(user);
        }

        // 編集処理
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TodoApp.Models.User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    _context.SaveChanges();
                }
                catch (Exception)
                {
                    return BadRequest("編集中にエラーが発生しました。");
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // 削除画面表示
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound("ユーザーが見つかりませんでした。");
            }

            return View(user);
        }

        // 削除処理
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound("ユーザーが見つかりませんでした。");
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }


}
