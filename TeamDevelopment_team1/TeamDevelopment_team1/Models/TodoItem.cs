using System.ComponentModel.DataAnnotations;

namespace TeamDevelopment_team1.Models
{
    // ================================================================

    // このクラスは、Todosデータベーステーブルの1行を表します。
    // DapperがSQL Serverから行を読み込む際、
    // 列名とプロパティ名を照合することで、これらのプロパティを自動的に設定します。
    //
    //  SQL column    DB type        C# property      C# type
    //  ----------    -------        -----------      -------
    //  Id            INT            Id               int
    //  Title         NVARCHAR(100)  Title            string
    //  Detail        NVARCHAR(1000) Detail           string?   (nullable)
    //  DueDate       DATE           DueDate          DateTime? (nullable)
    //  Priority      TINYINT        Priority         Priority  (enum)
    //  IsCompleted   BIT            IsCompleted      bool
    //  CreatedAt     DATETIME       CreatedAt        DateTime
    // ================================================================
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Detail { get; set; }
        public DateTime? DueDate { get; set; }
        public Priority Priority { get; set; } = Priority.Mid;
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }


        /* ── 新機能：担当者 ───────────────────────────────────── */
        // DB列から外部キーの値を格納します
        [Display(Name = "担当者")]
        public int? AssigneeId { get; set; } = 0;   // ← デフォルト0は未割り当てを意味します

        // JOINから名前を格納 — DB列ではありません
        // DapperはSQLの "u.Name AS AssigneeName" からこれを設定します
        public string? AssigneeName { get; set; }

        // ── 計算プロパティ ───────────────────────────────────────
        // これらは列ではありません。上記の値から動的に計算されます。
        // Razor ビューは、これらの値を使用して、
        //.cshtml にロジックを記述することなく、表示する色やラベルを決定します。

        // この作業は期限を過ぎてもまだ完了していないのですか？
        public bool IsOverdue
        {
            get
            {
                if (IsCompleted) return false; // 完了した作業は期限切れにはなりません
                if (DueDate == null) return false; // 期限がない場合、期限切れにはなりません
                return DueDate.Value.Date < DateTime.Today;
            }
        }

        // この作業は今日が期限で、まだ完了していないのですか？
        public bool IsDueToday
        {
            get
            {
                if (IsCompleted) return false;
                if (DueDate == null) return false;
                return DueDate.Value.Date == DateTime.Today;
            }
        }

        //テーブルの優先度列に表示されるテキストラベル
        public string PriorityLabel
        {
            get
            {
                if (Priority == Priority.High) return "🔴 高";
                if (Priority == Priority.Mid) return "🟡 中";
                return "🟢 低";
            }
        }

        // 色付きの優先度バッジのCSSクラス名
        public string PriorityBadgeClass
        {
            get
            {
                if (Priority == Priority.High) return "badge-high";
                if (Priority == Priority.Mid) return "badge-mid";
                return "badge-low";
            }
        }
    }

}

