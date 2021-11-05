﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TRS.Models.DomainModels
{
    public class Project : IEquatable<Project>
    {
        [Required]
        public string Code { get; }

        [Required]
        public string Manager { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Budget { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        public HashSet<Category> Subactivities { get; } = new();

        public Project(string code)
        {
            Code = code;
        }

        public bool Equals(Project other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Code == other.Code;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Project)obj);
        }

        public override int GetHashCode()
        {
            return Code?.GetHashCode() ?? 0;
        }
    }
}
