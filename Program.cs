using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;

var builder = WebApplication.CreateBuilder(args);

// 認証ミドルウェアの設定
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";  // ログインページへのパスを指定
        options.LogoutPath = "/User/Logout";  // ログアウトページへのパスを指定
    });

// セッション設定
builder.Services.AddDistributedMemoryCache(); // セッションストレージの設定
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // セッションのタイムアウト
});

// データベース設定
builder.Services.AddDbContext<TodoContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));  // SQLiteの接続文字列を指定

builder.Services.AddControllersWithViews();  // MVCのビューコントローラーを追加

var app = builder.Build();

// HTTPSリダイレクトや静的ファイル設定
app.UseHttpsRedirection();
app.UseStaticFiles();

// ルーティングミドルウェア
app.UseRouting();

// セッションミドルウェアを追加
app.UseSession();  // セッションを有効化

// 認証ミドルウェアを追加（認証・認可）
app.UseAuthentication();  // 認証を有効化
app.UseAuthorization();   // 認可を有効化

// ルーティング設定（コントローラとビューのマッピング）
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");  // デフォルトのコントローラとアクションを設定

app.Run();
