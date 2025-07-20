using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfToDoList.Models;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Collections.Generic;
using System.Linq;

namespace WpfToDoList.ViewModels
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        // DataGrid 資料來源
        private ObservableCollection<Tasks> _tasks;
        public ObservableCollection<Tasks> Tasks
        {
            get => _tasks;
            set { _tasks = value; OnPropertyChanged(); }
        }

        // 儲存所有原始資料，用於排序時參考
        private List<Tasks> _allTasks = new List<Tasks>();

        // 排序下拉選單
        public ObservableCollection<string> SortByList { get; set; }
            = new ObservableCollection<string> { "Id", "Priority", "Date", "Content" };

        private string _selectedSortBy = "Id";
        public string SelectedSortBy
        {
            get => _selectedSortBy;
            set
            {
                _selectedSortBy = value;
                OnPropertyChanged();
                ApplySort();
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
                ApplySort();
            }
        }

        // 優先順序下拉選單
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

        // DataGrid 選中項目
        private Tasks? _selectedTask;
        public Tasks? SelectedTask
        {
            get => _selectedTask;
            set { _selectedTask = value; OnPropertyChanged(); }
        }

        // 連線字串（改成你的 mdf 實際路徑）
        private readonly string connStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Github\WPFExample\WpfToDoList\WpfToDoList\ToDoListDB.mdf;Integrated Security=True;Connect Timeout=30";

        /// <summary>
        /// 建構子
        /// </summary>
        public TaskViewModel()
        {
            Tasks = LoadTasks();
            SelectedPriority = PriorityList[0];
            // 預設排序
            SelectedSortBy = SortByList[0];
            IsDescending = false;
        }

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
            // 載入後馬上依照目前排序
            ApplySort();
            return new ObservableCollection<Tasks>(_allTasks);
        }

        /// <summary>
        /// 依照排序條件重整 Tasks
        /// </summary>
        public void ApplySort()
        {
            if (_allTasks == null) return;
            IEnumerable<Tasks> sorted = _allTasks;

            switch (SelectedSortBy)
            {
                case "Id":
                    sorted = IsDescending
                        ? _allTasks.OrderByDescending(x => x.Id)
                        : _allTasks.OrderBy(x => x.Id);
                    break;
                case "Priority":
                    // 將 Priority 1 ~ 5 視為數字排序
                    sorted = IsDescending
                        ? _allTasks.OrderByDescending(x => PriorityStringToInt(x.Priority))
                        : _allTasks.OrderBy(x => PriorityStringToInt(x.Priority));
                    break;
                case "Date":
                    sorted = IsDescending
                        ? _allTasks.OrderByDescending(x => x.Date)
                        : _allTasks.OrderBy(x => x.Date);
                    break;
                case "Content":
                    sorted = IsDescending
                        ? _allTasks.OrderByDescending(x => x.Content)
                        : _allTasks.OrderBy(x => x.Content);
                    break;
            }
            Tasks = new ObservableCollection<Tasks>(sorted);
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

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
