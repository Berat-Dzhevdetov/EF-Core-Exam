namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Data;
    using TeisterMask.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var projects = context.Projects
                .Where(x => x.Tasks.Any())
                .ToArray()
                .Select(x => new ExportProjectWithTheirTasksDTO
                {
                    ProjectName = x.Name,
                    TasksCount = x.Tasks.Count,
                    HasEndDate = x.DueDate == null ? "No" : "Yes",
                    Tasks = x.Tasks.ToArray().Select(x => new TasksExportDTO
                    {
                        Name = x.Name,
                        Label = x.LabelType.ToString()
                    }).OrderBy(x => x.Name).ToArray()
                }).OrderByDescending(x=> x.TasksCount).ThenBy(x=>x.ProjectName).ToArray();
            var result = ExamHelper.FromDTOToStringXML<ExportProjectWithTheirTasksDTO[]>(projects, "Projects");
            return result;
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employees = context.Employees
                .Where(x => x.EmployeesTasks.Any(s => s.Task.OpenDate >= date))
                .ToArray()
                .Select(x => new
                {
                    Username = x.Username,
                    Tasks = x.EmployeesTasks
                    .Where(t => t.Task.OpenDate >= date)
                    .ToArray()
                    .OrderByDescending(s => s.Task.DueDate)
                    .ThenBy(s => s.Task.Name)
                    .Select(s => new
                    {
                        TaskName = s.Task.Name,
                        OpenDate = s.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                        DueDate = s.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        LabelType = s.Task.LabelType.ToString(),
                        ExecutionType = s.Task.ExecutionType.ToString(),
                    }).ToList()
                })
                .OrderByDescending(x => x.Tasks.Count)
                .ThenBy(x => x.Username)
                .Take(10)
                .ToArray();

            string result = ExamHelper.FromDTOToJSONString(employees);
            return result;
        }
    }
}