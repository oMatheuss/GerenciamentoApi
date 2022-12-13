using Microsoft.EntityFrameworkCore;

namespace GerenciamentoAPI.Models
{
    public class BaseUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    [Index(nameof(Email), IsUnique = true)]
    public class User : BaseUser
    {
        public string Password { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;
        public ICollection<Activity> Activities { get; set; }

        public User()
        {
            Activities = new HashSet<Activity>();
        }
    }

    public class BaseActivity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ClosedAt { get; set; }
    }

    public class Activity : BaseActivity
    {
        public ICollection<User> Employees { get; set; }

        public Activity()
        {
            Employees = new HashSet<User>();
        }
    }

    public enum Status
    {
        OnGoing,
        Finished,
        Interrupted,
    }

    public class Roles
    {
        private Roles(string value) { Value = value; }

        public string Value { get; private set; }

        public static Roles Employee { get { return new Roles("employee"); } }
        public static Roles Manager { get { return new Roles("manager"); } }

        public override string ToString()
        {
            return Value;
        }
    }

    public class UserInsertRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class ActivityInsertRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class AddActivityToUserRequest
    {
        public int ActivityId { get; set; }
    }

    public class ActivitiesStatus
    {
        public int OnGoing { get; set; }
        public int Finished { get; set; }
        public int Interrupted { get; set; }
    }

    public class ActivityUserRequest
    {
        public string Email { get; set; }
        public int ActivityId { get; set; }
    }

    public class ActivityStatusRequest
    {
        public int ActivityId { get; set; }
        public Status Status { get; set; }
    }
}