﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class USER
    {
        public USER() {}
        [Key]
        public int ID { get; set; }
        public string NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public ICollection<EVENT> MY_EVENT { get; set; }
        public ICollection<TOKEN> MY_TOKEN { get; set; }
    }
}