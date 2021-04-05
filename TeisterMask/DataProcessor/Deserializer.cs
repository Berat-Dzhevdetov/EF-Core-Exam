namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            XmlSerializer xmlDeser = new XmlSerializer(typeof(List<ImportProjectDTO>), new XmlRootAttribute("Projects"));
            var projectsDTO = (List<ImportProjectDTO>)xmlDeser.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();

            foreach (var projDTO in projectsDTO)
            {
                if (!IsValid(projDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime.TryParseExact(projDTO.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime openDate);
                DateTime.TryParseExact(projDTO.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDate);

                if (openDate == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var realProject = new Project()
                {
                    Name = projDTO.Name,
                    OpenDate = openDate,
                    DueDate = dueDate,
                };

                foreach (var task in projDTO.Tasks)
                {

                    if (!IsValid(task))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime.TryParseExact(task.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime taskOpenDate);
                    DateTime.TryParseExact(task.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime taskDueDate);

                    if (taskOpenDate == null || taskDueDate == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (taskOpenDate < openDate || taskDueDate > dueDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var realTask = new Task()
                    {
                        Name = task.Name,
                        OpenDate = taskOpenDate,
                        DueDate = taskDueDate,
                        ExecutionType = (ExecutionType)task.ExecutionType,
                        LabelType = (LabelType)task.LabelType,
                    };

                    realProject.Tasks.Add(realTask);
                }

                context.Projects.Add(realProject);
                context.SaveChanges();
                sb.AppendLine(string.Format(SuccessfullyImportedProject, realProject.Name, realProject.Tasks.Count));
            }
            Console.WriteLine(context.Projects.Count());
            Console.WriteLine(context.Tasks.Count());
            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var employeesDTO = ExamHelper.FromStringJSONToClass<ImportEmployeeDTO>(jsonString);

            var stb = new StringBuilder();

            foreach (var empDT in employeesDTO)
            {
                if (!IsValid(empDT))
                {
                    stb.AppendLine(ErrorMessage);
                    continue;
                }

                var realEmployee = new Employee()
                {
                    Username = empDT.Username,
                    Email = empDT.Email,
                    Phone = empDT.Phone,
                };

                foreach (var task in empDT.Tasks.Distinct())
                {
                    var findTask = context.Tasks.FirstOrDefault(x => x.Id == task);

                    if (findTask == null)
                    {
                        stb.AppendLine(ErrorMessage);
                        continue;
                    }

                    realEmployee.EmployeesTasks.Add(new EmployeeTask() { Task = findTask, Employee = realEmployee });
                }

                context.Employees.Add(realEmployee);
                context.SaveChanges();
                stb.AppendLine(string.Format(SuccessfullyImportedEmployee, realEmployee.Username, realEmployee.EmployeesTasks.Count));
            }
            return stb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}