# ToDoListWPF
A simple **WPF To-Do List** management system using MVVM pattern, SQL Server database, and clean C# codebase.

## Features

🖥️ 使用 WPF (MVVM 架構)

📅 代辦清單管理（新增、查詢、刪除）

🔎 支援欄位搜尋與排序（Id, Priority, Date, Content）

💾 使用 SQL Server，連線字串可設定於 `appsettings.json`

![](https://github.com/Mephisto-000/ToDoListWPF/blob/main/imgs/screenshot.png)



## Project Structure

```
WpfToDoList/
│
├─ DB_Scripts/         # SQL 腳本（建立資料庫/資料表用）
├─ Models/             # 實體資料模型（Tasks.cs ...）
├─ ViewModels/         # ViewModel（TaskViewModel.cs ...）
├─ Views/              # 畫面與後端（MainWindow.xaml / MainWindow.xaml.cs ...）
├─ appsettings.json    # 連線字串等設定檔
├─ README.md
└─ WpfToDoList.sln     # VS 方案檔
```



## Quick Start

1. 建立資料庫與資料表
   1. 執行 `DB_Scripts/CreateDB.sql`
   2. 執行 `DB_Scripts/TbToDoList`
2. 在 `WpfToDoList/` 資料夾底下新增 `appsettings.json`

內容 : 

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ToDoListDB;User Id=Your ID;Password=Your Password;Encrypt=True;TrustServerCertificate=True;Connect Timeout=30"
  }
}
```

3. Visual Studio 開啟 → `Build` → `Run` (F5)



## Usage

- **新增任務**
   選擇 Priority → 輸入內容 → 按下「Add」

- **刪除任務**
   點選 DataGrid 某列 → 按下「Delete」

- **搜尋任務**
   選擇要查詢的欄位 → 輸入關鍵字（支援模糊查詢）

- **排序任務**
   選擇排序欄位、勾選反向排序



## Technology Stack

- .NET 8
- WPF (C#)
- Microsoft SQL Server 2022
