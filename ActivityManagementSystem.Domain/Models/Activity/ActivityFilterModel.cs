using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class ActivityFilterModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Type { get; set; }

        public long? DepartmentId { get; set; }

        public List<FilterColumn> FilterColumns { get; set; }
    }

    public class FilterColumn
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
