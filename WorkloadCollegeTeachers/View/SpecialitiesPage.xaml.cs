using DevExpress.XtraPrinting;
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
using System.Data.Entity;
using ClosedXML.Excel;

namespace WorkloadCollegeTeachers.View
{
    /// <summary>
    /// Логика взаимодействия для Specialities.xaml
    /// </summary>
    public partial class Specialities : Page
    {
        Core db = new Core();

        public Specialities()
        {
            InitializeComponent();
            GetAllSpecialties();
        }

        private void GetAllSpecialties()
        {
            //Вывод в DataGrid специальностей и связанных с ними групп
            var specialties = db.context.Specialties.Include(s => s.Groups).ToList();

            DataGridSpecialties.ItemsSource = specialties.Select(s => new
            {
                Specialties = s,
                s.Code,
                s.Name,
                Groups = string.Join(", ", s.Groups.Select(g => g.Name_group))
            }).ToList();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }


        private void DataGridSpecialties_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SpecialitiesAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //Удаление специальностей
            Core db = new Core();
            try
            {
                if (DataGridSpecialties.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, выберите специальность для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // Извлекаем анонимный объект из выбранного элемента
                    var selectedData = DataGridSpecialties.SelectedItem as dynamic;
                    Specialties selectedSpecialty = selectedData.Specialties;

                    if (selectedSpecialty != null)
                    {
                        var specialtyToRemove = db.context.Specialties.FirstOrDefault(s => s.specialties_id == selectedSpecialty.specialties_id);
                        if (specialtyToRemove != null)
                        {
                            db.context.Specialties.Remove(specialtyToRemove);
                            db.context.SaveChanges();

                            MessageBox.Show("Специальность удалена!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                            GetAllSpecialties();
                        }
                        else
                        {
                            MessageBox.Show("Выбранная специальность не найдена в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении специальности.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Number == 547) // Конфликт удаления, если объект используется в других местах
                {
                    MessageBox.Show("Невозможно удалить специальность, так как она используется в других таблицах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Невозможно удалить специальность, так как она используется в других таблицах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Невозможно удалить специальность.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AddSpecialtiesPage addSpecialtiesPage = new AddSpecialtiesPage();
            addSpecialtiesPage.SpecialtiesAdded += AddSpecialtiesPage_SpecialtiesAdded;
            this.NavigationService.Navigate(new AddSpecialtiesPage());
        }

        private void AddSpecialtiesPage_SpecialtiesAdded(object sender, SpecialtiesEventArgs e)
        {
            // Обновляем DataGrid после добавления новой специальности
            GetAllSpecialties();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            GetAllSpecialties();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //Кнопка "Редактировать"
            if (DataGridSpecialties.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите специальность для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Извлекаем выбранную специальность
            var selectedData = DataGridSpecialties.SelectedItem as dynamic;
            Specialties selectedSpecialties = selectedData.Specialties;

            // Создаем и открываем страницу редактирования, передавая данные выбранной специальности
            EditSpecialtiesPage editPage = new EditSpecialtiesPage(selectedSpecialties,db);
            editPage.UpdatedSpecialty += EditPage_SpecialtiesUpdated;

            // Переходим на страницу редактирования
            NavigationService.Navigate(editPage);
        }

        private void EditPage_SpecialtiesUpdated(object sender, SpecialtiesEventArgs e)
        {
            // Обновить DataGrid после редактирования
            GetAllSpecialties();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (DataGridSpecialties.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите специальность для формирования учебного плана.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Извлекаем выбранную специальность
            var selectedData = DataGridSpecialties.SelectedItem as dynamic;
            Specialties selectedSpecialty = selectedData.Specialties;

            // Получаем связанные дисциплины и их часы
            var disciplines = db.context.AcademicDisciplines.Where(ad => ad.Specialties.Any(s => s.specialties_id == selectedSpecialty.specialties_id)).Select(ad => new
                {
                    ad.Name_discipline,
                    hours_theory = (int?)(ad.Hours_theory ?? 0),
                    hours_practice = (int?)(ad.Hours_practice ?? 0),
                    hours_independent = (int?)(ad.Hours_independent ?? 0),
                    hours_coursework = (int?)(ad.Hours_coursework ?? 0),
                ad.Number_term,
                }).ToList();

            // Создаем новый Excel-файл
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Учебный план");

                // Заголовки столбцов
                worksheet.Cell(1, 1).Value = "Наименование циклов, дисциплин, профессиональных модулей, МДК, практик";
                worksheet.Range("A1", "D5").Merge().AddToNamed("Titles");

                worksheet.Cell(1, 5).Value = "Учебная нагрузка обучающихся (час)";
                worksheet.Range("E1", "L1").Merge().AddToNamed("Titles");

                worksheet.Cell(2, 5).Value = "Самостоятельная работа";
                worksheet.Range("E2", "F5").Merge().AddToNamed("Titles");

                worksheet.Cell(2, 7).Value = "Во взаимодействии с преподавателем";
                worksheet.Range("G2", "L2").Merge().AddToNamed("Titles");

                worksheet.Cell(3, 7).Value = "В т.ч.";
                worksheet.Range("G3", "L3").Merge().AddToNamed("Titles");

                worksheet.Cell(4, 7).Value = "Теоретическое обучение";
                worksheet.Range("G4", "H5").Merge().AddToNamed("Titles");

                worksheet.Cell(4, 9).Value = "Практические занятия";
                worksheet.Range("I4", "J5").Merge().AddToNamed("Titles");

                worksheet.Cell(4, 11).Value = "Курсовой проект (работа)";
                worksheet.Range("K4", "L5").Merge().AddToNamed("Titles");

                worksheet.Cell(1, 13).Value = "Семестр";
                worksheet.Range("M1", "N5").Merge().AddToNamed("Titles");

                int currentRow = 6;
                int totalHoursTheory = 0;
                int totalHoursPractice = 0;
                int totalHoursIndependent = 0;
                int totalHoursCoursework = 0;

                foreach (var discipline in disciplines)
                {
                    worksheet.Cell(currentRow, 1).Value = discipline.Name_discipline;
                    worksheet.Range($"A{currentRow}:D{currentRow}").Merge();
                    worksheet.Range($"A{currentRow}:D{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                    worksheet.Cell(currentRow, 5).Value = discipline.hours_independent;
                    worksheet.Range($"E{currentRow}:F{currentRow}").Merge();
                    worksheet.Range($"E{currentRow}:F{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                    worksheet.Cell(currentRow, 7).Value = discipline.hours_theory;
                    worksheet.Range($"G{currentRow}:H{currentRow}").Merge();
                    worksheet.Range($"G{currentRow}:H{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                    worksheet.Cell(currentRow, 9).Value = discipline.hours_practice;
                    worksheet.Range($"I{currentRow}:J{currentRow}").Merge();
                    worksheet.Range($"I{currentRow}:J{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                    worksheet.Cell(currentRow, 11).Value = discipline.hours_coursework;
                    worksheet.Range($"K{currentRow}:L{currentRow}").Merge();
                    worksheet.Range($"K{currentRow}:L{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                    worksheet.Cell(currentRow, 13).Value = discipline.Number_term;
                    worksheet.Range($"M{currentRow}:N{currentRow}").Merge();
                    worksheet.Range($"M{currentRow}:N{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                    totalHoursTheory += discipline.hours_theory ?? 0;
                    totalHoursPractice += discipline.hours_practice ?? 0;
                    totalHoursIndependent += discipline.hours_independent ?? 0;
                    totalHoursCoursework += discipline.hours_coursework ?? 0;

                    currentRow++;
                }

                // "Профессиональный цикл" - суммирование часов
                worksheet.Cell(currentRow, 1).Value = "Профессиональный цикл";
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Range($"A{currentRow}:D{currentRow}").Merge();
                worksheet.Range($"A{currentRow}:D{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                worksheet.Cell(currentRow, 5).Value = totalHoursIndependent;
                worksheet.Range($"E{currentRow}:F{currentRow}").Merge();
                worksheet.Range($"E{currentRow}:F{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                worksheet.Cell(currentRow, 7).Value = totalHoursTheory;
                worksheet.Range($"G{currentRow}:H{currentRow}").Merge();
                worksheet.Range($"G{currentRow}:H{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                worksheet.Cell(currentRow, 9).Value = totalHoursPractice;
                worksheet.Range($"I{currentRow}:J{currentRow}").Merge();
                worksheet.Range($"I{currentRow}:J{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                worksheet.Cell(currentRow, 11).Value = totalHoursCoursework;
                worksheet.Range($"K{currentRow}:L{currentRow}").Merge();
                worksheet.Range($"K{currentRow}:L{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

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

                // Сохраняем файл
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = $"Учебный план",
                    DefaultExt = ".xlsx",
                    Filter = "Excel Files|*.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    workbook.SaveAs(saveFileDialog.FileName);
                    MessageBox.Show("Учебный план успешно сохранен!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
