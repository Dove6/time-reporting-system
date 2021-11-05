using System;
using System.ComponentModel.DataAnnotations;

namespace TRS.Models.DomainModels
{
    public class User : IEquatable<User>
    {
        [Required]
        public string Name { get; }

        public User(string name)
        {
            Name = name;
        }

        public bool Equals(User other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((User)obj);
        }

        public override int GetHashCode()
        {
            return Name?.GetHashCode() ?? 0;
        }
    }
}
