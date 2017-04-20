﻿using System.Collections.Generic;

namespace TaskMaster.DAL.Models
{
    public class Task
    {
        public Task() { }
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string Descryption { get; set; }
        public ICollection<Activity> Activity { get; set; }
        public ICollection<Favorites> Favorites { get; set; }

    }
}