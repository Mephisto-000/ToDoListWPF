using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using WpfToDoList.Models;

namespace WpfToDoList.ViewModels
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        #region 資料欄位與屬性宣告

        // DataGrid 資料來源
        private ObservableCollection<Tasks> _tasks = new ObservableCollection<Tasks>();
        public ObservableCollection<Tasks> Tasks
        {
            get => _tasks;
            set { _tasks = value; OnPropertyChanged(); }
        }

        // 儲存所有原始資料（用於搜尋/排序不重複查資料庫）
        private List<Tasks> _allTasks = new List<Tasks>();

        // 排序/搜尋欄位選單
        public ObservableCollection<string> SortByList { get; set; }
            = new ObservableCollection<string> { "Id", "Priority", "Date", "Content" };

        // === 搜尋用屬性 ===
        private string? _searchField = "Content"; // 預設查 Content
        public string? SearchField
        {
            get => _searchField;
            set
            {
                _searchField = value;
                OnPropertyChanged();
                ApplyFilterAndSort();
            }
        }

        private string? _searchKeyword = "";
        public string? SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged();
                ApplyFilterAndSort();
            }
        }

        // === 排序用屬性 ===
        private string _selectedSortBy = "Id";
        public string SelectedSortBy
        {
            get => _selectedSortBy;
            set
            {
                _selectedSortBy = value;
                OnPropertyChanged();
                ApplyFilterAndSort();
            }
        }

        private bool _isDescending;
        public bool IsDescending
        {
            get => _isDescending;
            set
            {
                _isDescending = value;
                OnPropertyChanged();
                ApplyFilterAndSort();
            }
        }

        // 優先順序下拉選單（新增時使用）
        public ObservableCollection<string> PriorityList { get; set; }
            = new ObservableCollection<string> { "Priority 1", "Priority 2", "Priority 3", "Priority 4", "Priority 5" };

        private string? _selectedPriority;
        public string? SelectedPriority
        {
            get => _selectedPriority;
            set { _selectedPriority = value; OnPropertyChanged(); }
        }

        private string? _newTaskContent;
        public string? NewTaskContent
        {
            get => _newTaskContent;
            set { _newTaskContent = value; OnPropertyChanged(); }
        }

        // DataGrid 選中項目（刪除用）
        private Tasks? _selectedTask;
        public Tasks? SelectedTask
        {
            get => _selectedTask;
            set { _selectedTask = value; OnPropertyChanged(); }
        }

        // 連線字串 
        private readonly string connStr;

        #endregion

        #region 建構子

        /// <summary>
        /// 建構子：初始化組態、載入資料、設定預設值
        /// </summary>
        public TaskViewModel()
        {
            // 讀取 appsettings.json 的連線字串
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            connStr = config.GetConnectionString("DefaultConnection")!;
            if (string.IsNullOrWhiteSpace(connStr))
                throw new InvalidOperationException("資料庫連線字串未設定於 appsettings.json！");

            Tasks = LoadTasks();
            SelectedPriority = PriorityList[0];
            SelectedSortBy = SortByList[0];
            SearchField = "Content"; // 預設搜尋 Content
            SearchKeyword = "";
            IsDescending = false;
        }

        #endregion

        #region 資料存取與查詢/排序

        /// <summary>
        /// 從資料庫載入所有任務
        /// </summary>
        public ObservableCollection<Tasks> LoadTasks()
        {
            var tasks = new ObservableCollection<Tasks>();
            var raw = new List<Tasks>();
            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string sql = "SELECT Id, Content, Priority, Date FROM TbToDoList";
                    using (var cmd = new SqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var t = new Tasks
                            {
                                Id = reader.GetInt32(0),
                                Content = reader.IsDBNull(1) ? null : reader.GetString(1),
                                Priority = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Date = reader.GetDateTime(3)
                            };
                            tasks.Add(t);
                            raw.Add(t);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("讀取資料庫失敗：" + ex.Message, "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            _allTasks = raw;
            // 載入後馬上依照目前搜尋＋排序
            ApplyFilterAndSort();
            return new ObservableCollection<Tasks>(_allTasks);
        }

        /// <summary>
        /// 搜尋 + 排序
        /// </summary>
        public void ApplyFilterAndSort()
        {
            if (_allTasks == null) return;
            IEnumerable<Tasks> result = _allTasks;

            // 先做模糊查詢（依 SearchField + SearchKeyword）
            if (!string.IsNullOrWhiteSpace(SearchKeyword) && !string.IsNullOrWhiteSpace(SearchField))
            {
                var keyword = SearchKeyword.ToLower();
                switch (SearchField)
                {
                    case "Id":
                        if (int.TryParse(SearchKeyword, out int idVal))
                            result = result.Where(x => x.Id.ToString().Contains(idVal.ToString()));
                        else
                            result = Enumerable.Empty<Tasks>();
                        break;
                    case "Priority":
                        result = result.Where(x => (x.Priority ?? "").ToLower().Contains(keyword));
                        break;
                    case "Date":
                        // 支援 yyyy-MM-dd 與 yyyyMMdd 兩種格式模糊查詢
                        result = result.Where(x =>
                            x.Date.ToString("yyyy-MM-dd").Contains(SearchKeyword) ||
                            x.Date.ToString("yyyyMMdd").Contains(SearchKeyword)
                        );
                        break;
                    case "Content":
                        result = result.Where(x => (x.Content ?? "").ToLower().Contains(keyword));
                        break;
                }
            }

            // 再做排序
            switch (SelectedSortBy)
            {
                case "Id":
                    result = IsDescending
                        ? result.OrderByDescending(x => x.Id)
                        : result.OrderBy(x => x.Id);
                    break;
                case "Priority":
                    result = IsDescending
                        ? result.OrderByDescending(x => PriorityStringToInt(x.Priority))
                        : result.OrderBy(x => PriorityStringToInt(x.Priority));
                    break;
                case "Date":
                    result = IsDescending
                        ? result.OrderByDescending(x => x.Date)
                        : result.OrderBy(x => x.Date);
                    break;
                case "Content":
                    result = IsDescending
                        ? result.OrderByDescending(x => x.Content)
                        : result.OrderBy(x => x.Content);
                    break;
            }
            Tasks = new ObservableCollection<Tasks>(result);
        }

        /// <summary>
        /// Priority 欄位 "Priority 1"~"Priority 5" 轉換為數字
        /// </summary>
        private int PriorityStringToInt(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 99;
            var arr = s.Split(' ');
            if (arr.Length == 2 && int.TryParse(arr[1], out int num)) return num;
            return 99;
        }

        #endregion

        #region 新增/刪除功能

        /// <summary>
        /// 新增任務
        /// </summary>
        public void AddTask()
        {
            if (string.IsNullOrWhiteSpace(NewTaskContent) || string.IsNullOrWhiteSpace(SelectedPriority))
            {
                MessageBox.Show("請先選擇優先順序並輸入內容！", "提醒", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string sql = "INSERT INTO TbToDoList (Content, Priority, Date) VALUES (@Content, @Priority, @Date)";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Content", NewTaskContent);
                        cmd.Parameters.AddWithValue("@Priority", SelectedPriority);
                        cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }
                Tasks = LoadTasks();
                NewTaskContent = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("新增資料失敗：" + ex.Message, "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 刪除任務
        /// </summary>
        public void DeleteTask()
        {
            if (SelectedTask == null)
            {
                MessageBox.Show("請先選取要刪除的任務", "提醒", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string sql = "DELETE FROM TbToDoList WHERE Id = @Id";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", SelectedTask.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
                Tasks = LoadTasks();
                SelectedTask = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("刪除資料失敗：" + ex.Message, "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
