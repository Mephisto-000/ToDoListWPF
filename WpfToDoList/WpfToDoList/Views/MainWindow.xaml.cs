using System.Windows;
using WpfToDoList.ViewModels; // 引用 ViewModel 命名空間

namespace WpfToDoList.Views
{
    /// <summary>
    /// MainWindow 後端程式，負責初始化畫面與按鈕事件
    /// </summary>
    public partial class MainWindow : Window
    {
        // ViewModel 欄位
        private readonly TaskViewModel vm;

        #region 建構子與初始化

        /// <summary>
        /// 建構子：初始化畫面與資料繫結
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            vm = new TaskViewModel();
            this.DataContext = vm; // 設定 DataContext 讓 XAML 可直接資料繫結
        }

        #endregion

        #region 按鈕事件處理

        /// <summary>
        /// Add 按鈕點擊事件：呼叫 ViewModel 的 AddTask()
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            vm.AddTask();
        }

        /// <summary>
        /// Delete 按鈕點擊事件：呼叫 ViewModel 的 DeleteTask()
        /// </summary>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            vm.DeleteTask();
        }

        #endregion
    }
}
