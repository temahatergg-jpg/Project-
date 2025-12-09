using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1 { 
    public class MainForm : Form
    {
        private readonly bool _isAdmin;
        private readonly DataGridView _gridTasks;
        private readonly NumericUpDown _nudMinDiff;
        private readonly NumericUpDown _nudMaxDiff;
        private readonly CheckedListBox _clbTags;
        private readonly Button _btnRefresh;
        private readonly Button _btnAdd;
        private readonly Button _btnEdit;
        private readonly Button _btnDelete;
        private readonly Button _btnExport;

        private BindingList<TaskItem> _bindingTasks;
        private List<TagItem> _allTags;

        public MainForm(bool isAdmin)
        {
            _isAdmin = isAdmin;

            Text = isAdmin ? "База задач (администратор)" : "База задач (гость)";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;

            var panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90
            };

            var lblMin = new Label
            {
                Text = "Сложность от:",
                Location = new Point(10, 10),
                AutoSize = true
            };

            _nudMinDiff = new NumericUpDown
            {
                Location = new Point(110, 8),
                Width = 60,
                Minimum = 1,
                Maximum = 10,
                Value = 1
            };

            var lblMax = new Label
            {
                Text = "до:",
                Location = new Point(180, 10),
                AutoSize = true
            };

            _nudMaxDiff = new NumericUpDown
            {
                Location = new Point(210, 8),
                Width = 60,
                Minimum = 1,
                Maximum = 10,
                Value = 10
            };

            var lblTags = new Label
            {
                Text = "Теги (все выбранные):",
                Location = new Point(290, 10),
                AutoSize = true
            };

            _clbTags = new CheckedListBox
            {
                Location = new Point(290, 30),
                Width = 350,
                Height = 50
            };

            _btnRefresh = new Button
            {
                Text = "Обновить",
                Location = new Point(10, 40),
                Width = 90
            };
            _btnRefresh.Click += BtnRefreshOnClick;

            _btnAdd = new Button
            {
                Text = "Добавить",
                Location = new Point(110, 40),
                Width = 90
            };
            _btnAdd.Click += BtnAddOnClick;

            _btnEdit = new Button
            {
                Text = "Изменить",
                Location = new Point(210, 40),
                Width = 90
            };
            _btnEdit.Click += BtnEditOnClick;

            _btnDelete = new Button
            {
                Text = "Удалить",
                Location = new Point(210, 65),
                Width = 90
            };
            _btnDelete.Click += BtnDeleteOnClick;

            _btnExport = new Button
            {
                Text = "Отчет (CSV)",
                Location = new Point(110, 65),
                Width = 90
            };
            _btnExport.Click += BtnExportOnClick;

            panelTop.Controls.Add(lblMin);
            panelTop.Controls.Add(_nudMinDiff);
            panelTop.Controls.Add(lblMax);
            panelTop.Controls.Add(_nudMaxDiff);
            panelTop.Controls.Add(lblTags);
            panelTop.Controls.Add(_clbTags);
            panelTop.Controls.Add(_btnRefresh);
            panelTop.Controls.Add(_btnAdd);
            panelTop.Controls.Add(_btnEdit);
            panelTop.Controls.Add(_btnDelete);
            panelTop.Controls.Add(_btnExport);

            _gridTasks = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = true
            };

            Controls.Add(_gridTasks);
            Controls.Add(panelTop);

            if (!_isAdmin)
            {
                _btnAdd.Enabled = false;
                _btnEdit.Enabled = false;
                _btnDelete.Enabled = false;
            }

            Load += MainFormOnLoad;
        }

        private void MainFormOnLoad(object sender, EventArgs e)
        {
            _allTags = Db.GetAllTags();
            _clbTags.Items.Clear();
            foreach (TagItem tag in _allTags)
            {
                _clbTags.Items.Add(tag);
            }

            RefreshTasks();
        }

        private void BtnRefreshOnClick(object sender, EventArgs e)
        {
            RefreshTasks();
        }

        private void RefreshTasks()
        {
            int minDiff = (int)_nudMinDiff.Value;
            int maxDiff = (int)_nudMaxDiff.Value;

            if (maxDiff < minDiff)
            {
                int tmp = minDiff;
                minDiff = maxDiff;
                maxDiff = tmp;
                _nudMinDiff.Value = minDiff;
                _nudMaxDiff.Value = maxDiff;
            }

            var selectedTagIds = new List<int>();
            foreach (object checkedItem in _clbTags.CheckedItems)
            {
                var tag = checkedItem as TagItem;
                if (tag != null)
                {
                    selectedTagIds.Add(tag.Id);
                }
            }

            List<TaskItem> tasks = Db.GetTasksFiltered(minDiff, maxDiff, selectedTagIds);
            _bindingTasks = new BindingList<TaskItem>(tasks);
            _gridTasks.DataSource = _bindingTasks;
        }

        private TaskItem GetSelectedTask()
        {
            if (_gridTasks.CurrentRow == null)
            {
                return null;
            }

            var task = _gridTasks.CurrentRow.DataBoundItem as TaskItem;
            return task;
        }

        private void BtnAddOnClick(object sender, EventArgs e)
        {
            var allTags = Db.GetAllTags();
            var allContests = Db.GetAllContests();

            using (var form = new TaskEditForm(null, allTags, allContests))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    TaskItem newTask = form.Task;
                    Db.InsertTask(newTask);
                    RefreshTasks();
                }
            }
        }

        private void BtnEditOnClick(object sender, EventArgs e)
        {
            TaskItem selected = GetSelectedTask();
            if (selected == null)
            {
                MessageBox.Show("Выберите задачу.", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            TaskItem fullTask = Db.GetTaskById(selected.Id);
            if (fullTask == null)
            {
                MessageBox.Show("Задача не найдена в базе.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var allTags = Db.GetAllTags();
            var allContests = Db.GetAllContests();

            using (var form = new TaskEditForm(fullTask, allTags, allContests))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Db.UpdateTask(form.Task);
                    RefreshTasks();
                }
            }
        }

        private void BtnDeleteOnClick(object sender, EventArgs e)
        {
            TaskItem selected = GetSelectedTask();
            if (selected == null)
            {
                MessageBox.Show("Выберите задачу для удаления.", "Сообщение", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Удалить выбранную задачу?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            Db.DeleteTask(selected.Id);
            RefreshTasks();
        }

        private void BtnExportOnClick(object sender, EventArgs e)
        {
            if (_bindingTasks == null || _bindingTasks.Count == 0)
            {
                MessageBox.Show("Нет данных для отчета.", "Сообщение", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "CSV файл|*.csv",
                FileName = "tasks_report.csv"
            };

            DialogResult result = dialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("Id;TitleRu;Difficulty;CodeforcesPrepared;YandexPrepared");

            foreach (TaskItem task in _bindingTasks)
            {
                string line = task.Id + ";" +
                              task.TitleRu.Replace(";", ",") + ";" +
                              task.Difficulty + ";" +
                              (task.CodeforcesPrepared ? "1" : "0") + ";" +
                              (task.YandexPrepared ? "1" : "0");
                sb.AppendLine(line);
            }

            File.WriteAllText(dialog.FileName, sb.ToString(), Encoding.UTF8);

            MessageBox.Show("Отчет сохранён: " + dialog.FileName, "Готово", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
