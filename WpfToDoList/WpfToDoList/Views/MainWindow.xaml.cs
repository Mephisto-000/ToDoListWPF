using System.Windows;
using WpfToDoList.ViewModels; // 引用你的 ViewModel 命名空間

namespace WpfToDoList.Views
{
    /// <summary>
    /// MainWindow 後端程式，負責初始化畫面與按鈕事件
    /// </summary>
    public partial class MainWindow : Window
    {
        private TaskViewModel vm; // ViewModel 欄位

        /// <summary>
        /// 建構子：初始化畫面與資料繫結
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            vm = new TaskViewModel();
            this.DataContext = vm; // 設定 DataContext 讓 XAML 可直接資料繫結
        }

        /// <summary>
        /// Add 按鈕點擊事件：呼叫 ViewModel 的 AddTask()
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            vm.AddTask();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            vm.DeleteTask();
        }
    }
}
