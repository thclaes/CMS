﻿using System.Collections.Generic;

namespace CMS.Data.Entities
{
    public class Subject
    {
        public int SubjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Attachment> Attachments { get; set; }
    }
}