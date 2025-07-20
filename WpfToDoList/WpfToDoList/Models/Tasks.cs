namespace WpfToDoList.Models
{
    /// <summary>
    /// 代表一筆待辦事項資料的 Model。
    /// </summary>
    public class Tasks
    {
        /// <summary>
        /// 主鍵流水號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 任務內容
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 優先順序
        /// </summary>
        public string? Priority { get; set; }

        /// <summary>
        /// 建立（或顯示）日期與時間
        /// </summary>
        public DateTime Date { get; set; }
    }
}
