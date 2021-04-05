using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ExportDto
{
    [XmlType("Project")]
    public class ExportProjectWithTheirTasksDTO
    {
        [XmlAttribute("TasksCount")]
        public int TasksCount { get; set; }

        public string ProjectName { get; set; }
        public string HasEndDate { get; set; }

        [XmlArray("Tasks")]
        public TasksExportDTO[] Tasks { get; set; }
    }
    [XmlType("Task")]
    public class TasksExportDTO
    {

        public string Name { get; set; }
        public string Label { get; set; }
    }
}
