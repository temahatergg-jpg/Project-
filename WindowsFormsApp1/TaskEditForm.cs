using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class TaskEditForm : Form
    {
        private readonly TextBox _txtTitle;
        private readonly TextBox _txtStatement;
        private readonly TextBox _txtIdea;
        private readonly TextBox _txtPolygonUrl;
        private readonly CheckBox _chkCodeforces;
        private readonly CheckBox _chkYandex;
        private readonly NumericUpDown _nudDifficulty;
        private readonly TextBox _txtNote;
        private readonly CheckedListBox _clbTags;
        private readonly CheckedListBox _clbContests;
        private readonly Button _btnOk;
        private readonly Button _btnCancel;

        private readonly List<TagItem> _allTags;
        private readonly List<ContestItem> _allContests;

        public TaskItem Task { get; private set; }

        public TaskEditForm(TaskItem task, List<TagItem> tags, List<ContestItem> contests)
        {
            _allTags = tags;
            _allContests = contests;

            Text = task == null ? "Добавление задачи" : "Редактирование задачи";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(900, 600);

            var lblTitle = new Label
            {
                Text = "Название:",
                Location = new Point(10, 10),
                AutoSize = true
            };

            _txtTitle = new TextBox
            {
                Location = new Point(100, 8),
                Width = 760
            };

            var lblStatement = new Label
            {
                Text = "Краткое условие:",
                Location = new Point(10, 40),
                AutoSize = true
            };

            _txtStatement = new TextBox
            {
                Location = new Point(10, 60),
                Width = 480,
                Height = 150,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            var lblIdea = new Label
            {
                Text = "Краткая идея:",
                Location = new Point(10, 220),
                AutoSize = true
            };

            _txtIdea = new TextBox
            {
                Location = new Point(10, 240),
                Width = 480,
                Height = 100,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            var lblPolygon = new Label
            {
                Text = "Polygon URL:",
                Location = new Point(10, 350),
                AutoSize = true
            };

            _txtPolygonUrl = new TextBox
            {
                Location = new Point(10, 370),
                Width = 480
            };
            _txtPolygonUrl.TextChanged += TxtPolygonUrlOnTextChanged;

            _chkCodeforces = new CheckBox
            {
                Text = "Подготовлено для Codeforces",
                Location = new Point(10, 400),
                AutoSize = true
            };

            _chkYandex = new CheckBox
            {
                Text = "Подготовлено для Яндекс.Контеста",
                Location = new Point(10, 430),
                AutoSize = true
            };

            var lblDiff = new Label
            {
                Text = "Сложность (1-10):",
                Location = new Point(10, 460),
                AutoSize = true
            };

            _nudDifficulty = new NumericUpDown
            {
                Location = new Point(140, 458),
                Width = 60,
                Minimum = 1,
                Maximum = 10,
                Value = 5
            };

            var lblNote = new Label
            {
                Text = "Примечание:",
                Location = new Point(10, 490),
                AutoSize = true
            };

            _txtNote = new TextBox
            {
                Location = new Point(10, 510),
                Width = 480,
                Height = 40,
                Multiline = true
            };

            var lblTags = new Label
            {
                Text = "Теги:",
                Location = new Point(510, 40),
                AutoSize = true
            };

            _clbTags = new CheckedListBox
            {
                Location = new Point(510, 60),
                Width = 350,
                Height = 180
            };

            foreach (TagItem tag in _allTags)
            {
                _clbTags.Items.Add(tag);
            }

            var lblContests = new Label
            {
                Text = "Контесты:",
                Location = new Point(510, 250),
                AutoSize = true
            };

            _clbContests = new CheckedListBox
            {
                Location = new Point(510, 270),
                Width = 350,
                Height = 180
            };

            foreach (ContestItem contest in _allContests)
            {
                _clbContests.Items.Add(contest);
            }

            _btnOk = new Button
            {
                Text = "Сохранить",
                Location = new Point(600, 510),
                Width = 120
            };
            _btnOk.Click += BtnOkOnClick;

            _btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(740, 510),
                Width = 120
            };
            _btnCancel.Click += (sender, args) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            Controls.Add(lblTitle);
            Controls.Add(_txtTitle);
            Controls.Add(lblStatement);
            Controls.Add(_txtStatement);
            Controls.Add(lblIdea);
            Controls.Add(_txtIdea);
            Controls.Add(lblPolygon);
            Controls.Add(_txtPolygonUrl);
            Controls.Add(_chkCodeforces);
            Controls.Add(_chkYandex);
            Controls.Add(lblDiff);
            Controls.Add(_nudDifficulty);
            Controls.Add(lblNote);
            Controls.Add(_txtNote);
            Controls.Add(lblTags);
            Controls.Add(_clbTags);
            Controls.Add(lblContests);
            Controls.Add(_clbContests);
            Controls.Add(_btnOk);
            Controls.Add(_btnCancel);

            if (task != null)
            {
                LoadTask(task);
            }
        }

        private void LoadTask(TaskItem task)
        {
            Task = new TaskItem
            {
                Id = task.Id,
                TitleRu = task.TitleRu,
                ShortStatement = task.ShortStatement,
                ShortIdea = task.ShortIdea,
                PolygonUrl = task.PolygonUrl,
                CodeforcesPrepared = task.CodeforcesPrepared,
                YandexPrepared = task.YandexPrepared,
                Difficulty = task.Difficulty,
                Note = task.Note,
                TagIds = new List<int>(task.TagIds),
                ContestIds = new List<int>(task.ContestIds)
            };

            _txtTitle.Text = Task.TitleRu;
            _txtStatement.Text = Task.ShortStatement;
            _txtIdea.Text = Task.ShortIdea;
            _txtPolygonUrl.Text = Task.PolygonUrl;
            _chkCodeforces.Checked = Task.CodeforcesPrepared;
            _chkYandex.Checked = Task.YandexPrepared;
            _nudDifficulty.Value = Task.Difficulty;
            _txtNote.Text = Task.Note;

            for (int i = 0; i < _clbTags.Items.Count; i++)
            {
                var tag = _clbTags.Items[i] as TagItem;
                if (tag != null && Task.TagIds.Contains(tag.Id))
                {
                    _clbTags.SetItemChecked(i, true);
                }
            }

            for (int i = 0; i < _clbContests.Items.Count; i++)
            {
                var contest = _clbContests.Items[i] as ContestItem;
                if (contest != null && Task.ContestIds.Contains(contest.Id))
                {
                    _clbContests.SetItemChecked(i, true);
                }
            }
        }

        private void TxtPolygonUrlOnTextChanged(object sender, EventArgs e)
        {
            string url = _txtPolygonUrl.Text.Trim();
            _chkCodeforces.Checked = !string.IsNullOrWhiteSpace(url);
        }

        private void BtnOkOnClick(object sender, EventArgs e)
        {
            string title = _txtTitle.Text.Trim();
            string statement = _txtStatement.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Введите название задачи.", "Сообщение", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(statement))
            {
                MessageBox.Show("Введите краткое условие.", "Сообщение", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (Task == null)
            {
                Task = new TaskItem();
            }

            Task.TitleRu = title;
            Task.ShortStatement = statement;
            Task.ShortIdea = _txtIdea.Text.Trim();
            Task.PolygonUrl = _txtPolygonUrl.Text.Trim();
            Task.CodeforcesPrepared = _chkCodeforces.Checked;
            Task.YandexPrepared = _chkYandex.Checked;
            Task.Difficulty = (int)_nudDifficulty.Value;
            Task.Note = _txtNote.Text.Trim();

            Task.TagIds = new List<int>();
            foreach (object checkedItem in _clbTags.CheckedItems)
            {
                var tag = checkedItem as TagItem;
                if (tag != null)
                {
                    Task.TagIds.Add(tag.Id);
                }
            }

            Task.ContestIds = new List<int>();
            foreach (object checkedItem in _clbContests.CheckedItems)
            {
                var contest = checkedItem as ContestItem;
                if (contest != null)
                {
                    Task.ContestIds.Add(contest.Id);
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
