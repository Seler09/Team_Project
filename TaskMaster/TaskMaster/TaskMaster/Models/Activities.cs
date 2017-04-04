﻿using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace TaskMaster.Models
{
    public class Activities
    {
        [PrimaryKey,AutoIncrement]
        public int ActivityId { get; set; }
        public string Comment { get; set; }
        public int UserId { get; set; }
        public int TaskId { get; set; }
        public int GroupId { get; set; }
        public int Status { get; set; }
    }
}