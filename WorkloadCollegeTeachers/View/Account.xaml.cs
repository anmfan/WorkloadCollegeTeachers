using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorkloadCollegeTeachers.Model;
using System.Data.SqlClient;
using System.Xml.Linq;
using Microsoft.Win32;
using System.IO;
using static WorkloadCollegeTeachers.View.AddTeacherPage;
using System.Windows.Markup;
using Microsoft.SqlServer.Server;
using System.Data.Entity;
using static WorkloadCollegeTeachers.Model.GroupsOnSpecialties;
using System.Reflection.Emit;
using DevExpress.XtraPrinting;
using Xceed.Words.NET;
using ClosedXML.Excel;
using static WorkloadCollegeTeachers.View.AddDisciplinesTeacherPage;
using System.Data.Entity.Infrastructure;

namespace WorkloadCollegeTeachers.View
{
    /// <summary>
    /// Логика взаимодействия для Account.xaml
    /// </summary>
    public partial class Account : Page
    {
        Core db = new Core();
        List<Teachers> arrayTeachers;

        public Account()
        {
            InitializeComponent();
            LoadTeachers();
        }


        public void UpdateDatGrid()
        {
            try
            {
                var teachers = db.context.Teachers.Include(t => t.Specialties).ToList();
                DataGridTeachers.ItemsSource = teachers;
                DataGridTeachers.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при обновлении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void ListTeachers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void GridTeachers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DataGridTeachers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void ShowDisciplineDetails(Teachers teacher)
        {
            //Вывод всех дисциплин преподавателя и их часы
            try
            {
                var disciplines = db.context.AcademicDisciplines.Where(d => d.Specialties.Any(s => s.specialties_id == teacher.specialties_ID)).ToList();

                if (disciplines.Any())
                {
                    var disciplineInfo = new StringBuilder();
                    disciplineInfo.AppendLine($"Дисциплины для специальности:\r\r\n {teacher.Specialties.Name}\n");
                    disciplineInfo.AppendLine(new string('-', 40));

                    foreach (var discipline in disciplines)
                    {
                        disciplineInfo.AppendLine($"Название: {discipline.Name_discipline}");
                        disciplineInfo.AppendLine($"Часы теории: {discipline.Hours_theory}");
                        disciplineInfo.AppendLine($"Часы практики: {discipline.Hours_practice}");
                        disciplineInfo.AppendLine($"Часы самостоятельной подготовки: {discipline.Hours_independent}");
                        disciplineInfo.AppendLine($"Часы курсовой подготовки: {discipline.Hours_coursework}");
                        disciplineInfo.AppendLine($"Семестр: {discipline.Number_term}");
                        disciplineInfo.AppendLine(new string('-', 40));
                    }

                    MessageBox.Show(disciplineInfo.ToString(), "Информация о дисциплинах", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Для этой специальности нет дисциплин.", "Информация о дисциплинах", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadTeachers()
        {
            try
            {
                var teachers = db.context.Teachers.Include(t => t.Specialties).ToList();

                // Заполнение свойства SpecialtyName для каждого преподавателя
                foreach (var teacher in teachers)
                {
                    teacher.SpecialtyName = teacher.Specialties?.Name;
                }

                // Заполнение DataGrid преподавателями
                DataGridTeachers.ItemsSource = teachers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //Удаление преподавателей

            Core db = new Core();

            try
            {
                if (DataGridTeachers.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, выберите преподавателя для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // Получаем ID преподавателя
                    var selectedTeacher = DataGridTeachers.SelectedItem as Teachers;
                    if (selectedTeacher != null)
                    {
                        int teach_id = selectedTeacher.TeacherID;
                        // Найдите преподавателя в базе данных
                        var teacher = db.context.Teachers.Include(t => t.Users).SingleOrDefault(t => t.TeacherID == teach_id);


                        if (teacher != null)
                        {
                            // Удаляем пользователя, связанного с этим преподавателем
                            if (teacher.Users != null)
                            {
                                db.context.Users.Remove(teacher.Users);
                            }

                            // Удаляем самого преподавателя
                            db.context.Teachers.Remove(teacher);

                            db.context.SaveChanges();
                            MessageBox.Show("Преподаватель удалён!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                            UpdateDatGrid();
                        }
                        else
                        {
                            MessageBox.Show("Преподаватель не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {

                    }
                }
            }
            catch (DbUpdateException dbEx)
            {
                // Обработка ошибок обновления базы данных
                if (dbEx.InnerException?.InnerException is SqlException sqlEx && sqlEx.Number == 547) // 547 - ошибка нарушения ограничения внешнего ключа
                {
                    MessageBox.Show("Невозможно удалить преподавателя, так как у него есть прикрепленные дисциплины.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Произошла ошибка при удалении преподавателя: " + dbEx.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex}");
            }
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        private void AddTeacherButtonClick(object sender, RoutedEventArgs e)
        {
            AddTeacherPage addPage = new AddTeacherPage();
            addPage.TeacherAdded += AddPage_TeacherAdded;
            NavigationService.Navigate(addPage);
        }
        public void AddPage_TeacherAdded(object sender, TeacherEventArgs e)
        {
            LoadTeachers();
            var newTeacher = e.TeacherAdded;
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void RefreshButtonClick(object sender, RoutedEventArgs e)
        {
            UpdateDatGrid();
            TeacherDisciplinesDataGrid.ItemsSource = null;
        }

        private void EditTeacherButtonClick(object sender, RoutedEventArgs e)
        {
            //Метод для передачи выбранного преподавателя на страницу редактирования
            try
            {
                if (DataGridTeachers.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, выберите преподавателя для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Teachers selectedTeacher = (Teachers)DataGridTeachers.SelectedItem;
                var teacherWithUser = db.context.Teachers.Include(t => t.Users).FirstOrDefault(t => t.TeacherID == selectedTeacher.TeacherID);

                if (teacherWithUser == null || teacherWithUser.Users == null)
                {
                    MessageBox.Show("Не удалось найти пользователя, связанного с этим преподавателем.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Users selectedUser = teacherWithUser.Users;

                EditTeacherPage editPage = new EditTeacherPage(teacherWithUser, selectedUser, db);
                editPage.TeacherUpdated += EditPage_TeacherUpdated;

                if (this.NavigationService != null)
                {
                    this.NavigationService.Navigate(editPage);
                }
                else
                {
                    MessageBox.Show("NavigationService не доступен.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditPage_TeacherUpdated(object sender, TeacherEventArgs e)
        {
            //Обновить DataGrid после редактирования
            LoadTeachers();
        }

        private void LoadButton(object sender, RoutedEventArgs e)
        {
            if (DataGridTeachers.SelectedItem == null)
            {
                MessageBox.Show("Выберите преподавателя для вывода нагрузки!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (DataGridTeachers.SelectedItem is Teachers selectedTeacher)
            {
                ShowDisciplineDetails(selectedTeacher);
            }
        }

        private void OutputDisciplinesButtonClick_Click(object sender, RoutedEventArgs e)
        {
            // Метод для отображения преподавателя, его дисциплин и нагрузки в нижний DataGrid
            Teachers selectedTeacher = DataGridTeachers.SelectedItem as Teachers;

            if (selectedTeacher != null)
            {
                try
                {
                    // Поиск дисциплин выбранного преподавателя в таблице Load_of_teachers по его ID
                    var teacherDisciplines = db.context.LoadOfTeachers.Where(l => l.IDTeacher == selectedTeacher.ID).ToList();

                    // Суммарная нагрузка часов
                    var totalLoadHours = teacherDisciplines.Sum(l => l.Load_Hours);

                    // Удаление дубликатов дисциплин
                    var distinctDisciplines = teacherDisciplines.Select(l => l.Name_Discipline).Distinct().ToList();

                    if (distinctDisciplines.Any())
                    {
                        var disciplinesString = string.Join(", ", distinctDisciplines);

                        // Создание объекта, содержащего все данные о преподавателе и его дисциплины
                        var teacherData = new
                        {
                            selectedTeacher.fio,
                            selectedTeacher.Age,
                            selectedTeacher.SpecialtyName,
                            Disciplines = disciplinesString,
                            TotalLoadHours = totalLoadHours
                        };

                        TeacherDisciplinesDataGrid.ItemsSource = new List<object> { teacherData };
                    }
                    else
                    {
                        var teacherEmptyData = new
                        {
                            selectedTeacher.fio,
                            selectedTeacher.Age,
                            selectedTeacher.SpecialtyName
                        };
                        // Отображение данных о преподавателе, если у него нет дисциплин
                        TeacherDisciplinesDataGrid.ItemsSource = new List<object> { teacherEmptyData };
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите преподавателя.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddDisciplinesButtonClick_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridTeachers.SelectedItem is Teachers selectedTeacher)
            {
                var addDisciplinePage = new AddDisciplinesTeacherPage(selectedTeacher, db);
                addDisciplinePage.DisciplineAdded += AddDisciplinePage_DisciplineAdded;
                this.NavigationService.Navigate(addDisciplinePage);
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите преподавателя для добавления дисциплины.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddDisciplinePage_DisciplineAdded(object sender, DisciplineEventArgs e)
        {
            LoadTeachers();
        }

        private void DeleteDisciplinesButtonClick_Click(object sender, RoutedEventArgs e)
        {
            //Метод для выбора преподавателя и его данные передаются на страницу удаления дисциплины
            if (DataGridTeachers.SelectedItem is Teachers selectedTeacher)
            {
                var removeDisciplinePage = new DeleteDisciplinesTeacherPage(selectedTeacher, db);
                this.NavigationService.Navigate(removeDisciplinePage);
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите преподавателя для удаления дисциплины.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void StatisticsTeachersButton(object sender, RoutedEventArgs e)
        {
            //Вывод статистики по педагогической нагрузке (дисциплины + суммарные часы)

            var teachers = db.context.Teachers.Include(t => t.Specialties).Include(t => t.LoadOfTeachers).ToList();
            string filePath = @"C:\Users\mizev\Desktop\Статистика преподавателей.docx";

            using (var doc = DocX.Create(filePath))
            {
                var table = doc.AddTable(teachers.Count + 1, 4);
                table.Rows[0].Cells[0].Paragraphs[0].Append("Преподаватель").Bold().Alignment = Xceed.Document.NET.Alignment.center;
                table.Rows[0].Cells[1].Paragraphs[0].Append("Специальность").Bold().Alignment = Xceed.Document.NET.Alignment.center;
                table.Rows[0].Cells[2].Paragraphs[0].Append("Дисциплины").Bold().Alignment = Xceed.Document.NET.Alignment.center;
                table.Rows[0].Cells[3].Paragraphs[0].Append("Нагрузка (суммарные часы)").Bold().Alignment = Xceed.Document.NET.Alignment.center;

                for (int i = 0; i < teachers.Count; i++)
                {
                    var teacher = teachers[i];
                    var disciplines = teacher.LoadOfTeachers.Select(l => l.Name_Discipline).ToList();
                    var totalLoadHours = teacher.LoadOfTeachers.Sum(l => l.Load_Hours);

                    table.Rows[i + 1].Cells[0].Paragraphs[0].Append(teacher.fio);
                    table.Rows[i + 1].Cells[1].Paragraphs[0].Append(teacher.Specialties.Name);
                    table.Rows[i + 1].Cells[2].Paragraphs[0].Append(string.Join(", ", disciplines));
                    table.Rows[i + 1].Cells[3].Paragraphs[0].Append(totalLoadHours.ToString());
                }

                doc.InsertTable(table);
                doc.Save();

                MessageBox.Show("Документ успешно создан!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void GenerateExcelReport(Teachers selectedTeacher)
        {
            if (DataGridTeachers.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите преподавателя для формирования учебной нагрузки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Учебная нагрузка");


                    // Получение данных дисциплин для выбранного преподавателя
                    var disciplineData = db.context.LoadOfTeachers.Where(l => l.IDTeacher == selectedTeacher.ID).ToList();

                if (disciplineData.Count == 0)
                {
                    // Если у преподавателя нет дисциплин
                    MessageBox.Show("У этого преподавателя нет дисциплин", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Заголовки
                worksheet.Cell(1, 1).Value = "Дисциплина";
                    worksheet.Range("A1", "C2").Merge().AddToNamed("Titles");

                    worksheet.Cell(1, 4).Value = "Группа";
                    worksheet.Range("D1", "E2").Merge().AddToNamed("Titles");

                    worksheet.Cell(1, 6).Value = "I семестр";
                    worksheet.Range("F1", "M1").Merge().AddToNamed("Titles");

                    worksheet.Cell(2, 6).Value = "Теория";
                    worksheet.Range("F2", "G2").Merge().AddToNamed("Titles");

                    worksheet.Cell(2, 8).Value = "Практика";
                    worksheet.Range("H2", "I2").Merge().AddToNamed("Titles");

                    worksheet.Cell(2, 10).Value = "Курсовая подготовка";
                    worksheet.Range("J2", "K2").Merge().AddToNamed("Titles");

                    worksheet.Cell(2, 12).Value = "Самостоятельная подготовка";
                    worksheet.Range("L2", "M2").Merge().AddToNamed("Titles");

                    worksheet.Cell(1, 14).Value = "II семестр";
                    worksheet.Range("N1", "U1").Merge().AddToNamed("Titles");

                    worksheet.Cell(2, 14).Value = "Теория";
                    worksheet.Range("N2", "O2").Merge().AddToNamed("Titles");

                    worksheet.Cell(2, 16).Value = "Практика";
                    worksheet.Range("P2", "Q2").Merge().AddToNamed("Titles");

                    worksheet.Cell(2, 18).Value = "Курсовая подготовка";
                    worksheet.Range("R2", "S2").Merge().AddToNamed("Titles");

                    worksheet.Cell(2, 20).Value = "Самостоятельная подготовка";
                    worksheet.Range("T2", "U2").Merge().AddToNamed("Titles");

                    int currentRow = 3;

                    // Заполнение данными
                    foreach (var item in disciplineData)
                    {
                        if (!string.IsNullOrEmpty(item.Name_Discipline))
                        {
                            var academicDiscipline = db.context.AcademicDisciplines.FirstOrDefault(d => d.Name_discipline == item.Name_Discipline && d.Number_term == item.Term_Work);

                            var disciplineGroups = disciplineData.Where(d => d.Name_Discipline == item.Name_Discipline && d.TeacherGroup != null)
                                .Select(d => db.context.Groups.FirstOrDefault(g => g.ID == d.TeacherGroup)?.Name_group)
                                .ToList();

                            if (academicDiscipline != null && disciplineGroups.Count > 0)
                            {
                                foreach (var group in disciplineGroups)
                                {
                                    bool hasHours = academicDiscipline.Hours_theory > 0 ||
                                                    academicDiscipline.Hours_practice > 0 ||
                                                    academicDiscipline.Hours_coursework > 0 ||
                                                    academicDiscipline.Hours_independent > 0;

                                    if (hasHours)
                                    {
                                        worksheet.Cell(currentRow, 1).Value = item.Name_Discipline;
                                        worksheet.Range($"A{currentRow}:C{currentRow}").Merge();
                                        worksheet.Range($"A{currentRow}:C{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                                        worksheet.Cell(currentRow, 4).Value = group;
                                        worksheet.Range($"D{currentRow}:E{currentRow}").Merge();
                                        worksheet.Range($"D{currentRow}:E{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                                        if (item.Term_Work == 1)
                                        {
                                            worksheet.Cell(currentRow, 6).Value = academicDiscipline.Hours_theory;
                                            worksheet.Range($"F{currentRow}:G{currentRow}").Merge();
                                            worksheet.Range($"F{currentRow}:G{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                                            worksheet.Cell(currentRow, 8).Value = academicDiscipline.Hours_practice;
                                            worksheet.Range($"H{currentRow}:I{currentRow}").Merge();
                                            worksheet.Range($"H{currentRow}:I{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                                            worksheet.Cell(currentRow, 10).Value = academicDiscipline.Hours_coursework;
                                            worksheet.Range($"J{currentRow}:K{currentRow}").Merge();
                                            worksheet.Range($"J{currentRow}:K{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                                            worksheet.Cell(currentRow, 12).Value = academicDiscipline.Hours_independent;
                                            worksheet.Range($"L{currentRow}:M{currentRow}").Merge();
                                            worksheet.Range($"L{currentRow}:M{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                                        }
                                        else if (item.Term_Work == 2)
                                        {
                                            worksheet.Cell(currentRow, 14).Value = academicDiscipline.Hours_theory;
                                            worksheet.Range($"N{currentRow}:O{currentRow}").Merge();
                                            worksheet.Range($"N{currentRow}:O{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                                            worksheet.Cell(currentRow, 16).Value = academicDiscipline.Hours_practice;
                                            worksheet.Range($"P{currentRow}:Q{currentRow}").Merge();
                                            worksheet.Range($"P{currentRow}:Q{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                                            worksheet.Cell(currentRow, 18).Value = academicDiscipline.Hours_coursework;
                                            worksheet.Range($"R{currentRow}:S{currentRow}").Merge();
                                            worksheet.Range($"R{currentRow}:S{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                                            worksheet.Cell(currentRow, 20).Value = academicDiscipline.Hours_independent;
                                            worksheet.Range($"T{currentRow}:U{currentRow}").Merge();
                                            worksheet.Range($"T{currentRow}:U{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                                        }

                                        currentRow++;
                                    }
                                }
                            }
                        }
                    }

                    // Добавление жирных границ ко всем заполненным ячейкам
                    for (int row = 3; row < currentRow; row++)
                    {
                        worksheet.Range($"A{row}:U{row}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                        worksheet.Range($"F{row}:U{row}").Style.Border.InsideBorder = XLBorderStyleValues.Thick;

                        worksheet.Range($"F{row}:G{row}").Merge(); worksheet.Range($"N{row}:O{row}").Merge();
                        worksheet.Range($"H{row}:I{row}").Merge(); worksheet.Range($"P{row}:Q{row}").Merge();
                        worksheet.Range($"J{row}:K{row}").Merge(); worksheet.Range($"R{row}:S{row}").Merge();
                        worksheet.Range($"L{row}:M{row}").Merge(); worksheet.Range($"T{row}:U{row}").Merge();
                    }

                    worksheet.Range($"N3:U{currentRow - 1}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;


                    // Стили к заголовкам
                    var titleStyle = workbook.Style;
                    var nameStyle = worksheet.Style;
                    nameStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    nameStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    titleStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    titleStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    titleStyle.Font.Bold = true;
                    titleStyle.Alignment.WrapText = true;
                    workbook.NamedRanges.NamedRange("Titles").Ranges.Style = titleStyle;
                    workbook.NamedRanges.NamedRange("Titles").Ranges.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                // Сохранение файла с диалогом выбора пути
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Workbook|*.xlsx",
                    Title = "Сохранить учебную нагрузку"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    workbook.SaveAs(saveFileDialog.FileName);
                    MessageBox.Show("Учебная нагрузка успешно сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        private void LoadOfOneTeacher(object sender, RoutedEventArgs e)
        {
                var selectedTeacher = (Teachers)DataGridTeachers.SelectedItem;
                GenerateExcelReport(selectedTeacher);
        }

        private void GenerateExcelReportForAllTeachers()
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var teachers = db.context.Teachers.ToList();

                    foreach (var teacher in teachers)
                    {
                        var worksheet = workbook.Worksheets.Add(teacher.Name);

                        // Получение данных дисциплин для выбранного преподавателя
                        var disciplineData = db.context.LoadOfTeachers.Where(l => l.IDTeacher == teacher.ID).ToList();

                        if (disciplineData.Count == 0)
                        {
                            // Если у преподавателя нет дисциплин
                            worksheet.Cell(1, 1).Value = "Этот преподаватель не имеет специальности, дисциплин и прикрепленных к ним групп";
                            worksheet.Range("A1", "E3").Merge();
                            continue;
                        }

                        // Заголовки
                        worksheet.Cell(1, 1).Value = "Дисциплина";
                        worksheet.Range("A1", "C2").Merge().AddToNamed("Titles");

                        worksheet.Cell(1, 4).Value = "Группа";
                        worksheet.Range("D1", "E2").Merge().AddToNamed("Titles");

                        worksheet.Cell(1, 6).Value = "I семестр";
                        worksheet.Range("F1", "M1").Merge().AddToNamed("Titles");

                        worksheet.Cell(2, 6).Value = "Теория";
                        worksheet.Range("F2", "G2").Merge().AddToNamed("Titles");

                        worksheet.Cell(2, 8).Value = "Практика";
                        worksheet.Range("H2", "I2").Merge().AddToNamed("Titles");

                        worksheet.Cell(2, 10).Value = "Курсовая подготовка";
                        worksheet.Range("J2", "K2").Merge().AddToNamed("Titles");

                        worksheet.Cell(2, 12).Value = "Самостоятельная подготовка";
                        worksheet.Range("L2", "M2").Merge().AddToNamed("Titles");

                        worksheet.Cell(1, 14).Value = "II семестр";
                        worksheet.Range("N1", "U1").Merge().AddToNamed("Titles");

                        worksheet.Cell(2, 14).Value = "Теория";
                        worksheet.Range("N2", "O2").Merge().AddToNamed("Titles");

                        worksheet.Cell(2, 16).Value = "Практика";
                        worksheet.Range("P2", "Q2").Merge().AddToNamed("Titles");

                        worksheet.Cell(2, 18).Value = "Курсовая подготовка";
                        worksheet.Range("R2", "S2").Merge().AddToNamed("Titles");

                        worksheet.Cell(2, 20).Value = "Самостоятельная подготовка";
                        worksheet.Range("T2", "U2").Merge().AddToNamed("Titles");

                        int currentRow = 3;

                        // Заполнение данными
                        foreach (var item in disciplineData)
                        {
                            if (!string.IsNullOrEmpty(item.Name_Discipline))
                            {
                                var academicDiscipline = db.context.AcademicDisciplines.FirstOrDefault(d => d.Name_discipline == item.Name_Discipline && d.Number_term == item.Term_Work);

                                var disciplineGroups = disciplineData.Where(d => d.Name_Discipline == item.Name_Discipline && d.TeacherGroup != null)
                                    .Select(d => db.context.Groups.FirstOrDefault(g => g.ID == d.TeacherGroup)?.Name_group)
                                    .ToList();

                                bool hasGroups = disciplineGroups.Count > 0;
                                bool hasHours = academicDiscipline != null &&
                                                (academicDiscipline.Hours_theory > 0 ||
                                                academicDiscipline.Hours_practice > 0 ||
                                                academicDiscipline.Hours_coursework > 0 ||
                                                academicDiscipline.Hours_independent > 0);

                                if (academicDiscipline != null && hasHours)
                                {
                                    // Объединение строк с одинаковой дисциплиной
                                    int startRow = currentRow;

                                    worksheet.Cell(currentRow, 1).Value = item.Name_Discipline;
                                    worksheet.Range($"A{currentRow}:C{currentRow}").Merge();
                                    worksheet.Range($"A{currentRow}:C{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    worksheet.Range($"A{currentRow}:C{currentRow}").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                    worksheet.Range($"A{currentRow}:C{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                                    if (hasGroups)
                                    {
                                        foreach (var group in disciplineGroups)
                                        {
                                            worksheet.Cell(currentRow, 4).Value = group;
                                            worksheet.Range($"D{currentRow}:E{currentRow}").Merge();
                                            worksheet.Range($"D{currentRow}:E{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                                            FillSemesterData(worksheet, currentRow, item.Term_Work, academicDiscipline);

                                            currentRow++;
                                        }
                                    }
                                    else
                                    {
                                        worksheet.Cell(currentRow, 4).Value = "Нет групп";
                                        worksheet.Range($"D{currentRow}:E{currentRow}").Merge();
                                        worksheet.Range($"D{currentRow}:E{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                                        FillSemesterData(worksheet, currentRow, item.Term_Work, academicDiscipline);

                                        currentRow++;
                                    }

                                    // Объединение ячеек дисциплины
                                    worksheet.Range($"A{startRow}:C{currentRow - 1}").Merge();
                                    worksheet.Range($"A{startRow}:C{currentRow - 1}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                                }
                            }
                        }

                        // Добавление жирных границ ко всем заполненным ячейкам
                        for (int row = 3; row < currentRow; row++)
                        {
                            worksheet.Range($"A{row}:U{row}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                            worksheet.Range($"F{row}:U{row}").Style.Border.InsideBorder = XLBorderStyleValues.Thick;

                            worksheet.Range($"F{row}:G{row}").Merge(); worksheet.Range($"N{row}:O{row}").Merge();
                            worksheet.Range($"H{row}:I{row}").Merge(); worksheet.Range($"P{row}:Q{row}").Merge();
                            worksheet.Range($"J{row}:K{row}").Merge(); worksheet.Range($"R{row}:S{row}").Merge();
                            worksheet.Range($"L{row}:M{row}").Merge(); worksheet.Range($"T{row}:U{row}").Merge();
                        }

                        worksheet.Range($"N3:U{currentRow - 1}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                        // Стили к заголовкам
                        var titleStyle = workbook.Style;
                        var nameStyle = worksheet.Style;
                        nameStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        nameStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        titleStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        titleStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        titleStyle.Font.Bold = true;
                        titleStyle.Alignment.WrapText = true;
                        workbook.NamedRanges.NamedRange("Titles").Ranges.Style = titleStyle;
                        workbook.NamedRanges.NamedRange("Titles").Ranges.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                    }

                    // Сохранение файла с диалогом выбора пути
                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel Workbook|*.xlsx",
                        Title = "Сохранить учебную нагрузку"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        workbook.SaveAs(saveFileDialog.FileName);
                        MessageBox.Show("Учебная нагрузка успешно сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void FillSemesterData(IXLWorksheet worksheet, int row, int termWork, AcademicDisciplines academicDiscipline)
        {
            if (termWork == 1)
            {
                worksheet.Cell(row, 6).Value = academicDiscipline.Hours_theory;
                worksheet.Range($"F{row}:G{row}").Merge();
                worksheet.Range($"F{row}:G{row}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                worksheet.Cell(row, 8).Value = academicDiscipline.Hours_practice;
                worksheet.Range($"H{row}:I{row}").Merge();
                worksheet.Range($"H{row}:I{row}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                worksheet.Cell(row, 10).Value = academicDiscipline.Hours_coursework;
                worksheet.Range($"J{row}:K{row}").Merge();
                worksheet.Range($"J{row}:K{row}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                worksheet.Cell(row, 12).Value = academicDiscipline.Hours_independent;
                worksheet.Range($"L{row}:M{row}").Merge();
                worksheet.Range($"L{row}:M{row}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            }
            else if (termWork == 2)
            {
                worksheet.Cell(row, 14).Value = academicDiscipline.Hours_theory;
                worksheet.Range($"N{row}:O{row}").Merge();
                worksheet.Range($"N{row}:O{row}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                worksheet.Cell(row, 16).Value = academicDiscipline.Hours_practice;
                worksheet.Range($"P{row}:Q{row}").Merge();
                worksheet.Range($"P{row}:Q{row}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                worksheet.Cell(row, 18).Value = academicDiscipline.Hours_coursework;
                worksheet.Range($"R{row}:S{row}").Merge();
                worksheet.Range($"R{row}:S{row}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                worksheet.Cell(row, 20).Value = academicDiscipline.Hours_independent;
                worksheet.Range($"T{row}:U{row}").Merge();
                worksheet.Range($"T{row}:U{row}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            }
        }

        private void LoadOfAllTeacher(object sender, RoutedEventArgs e)
        {
            GenerateExcelReportForAllTeachers();
        }

    }
} 

