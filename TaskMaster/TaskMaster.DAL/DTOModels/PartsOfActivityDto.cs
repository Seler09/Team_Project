﻿using System;

namespace TaskMaster.DAL.DTOModels
{
    public class PartsOfActivityDto
    {
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public TimeSpan Duration { get; set; }

        public ActivityDto Activity { get; set; }
    }
}