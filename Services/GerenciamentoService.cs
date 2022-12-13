using GerenciamentoAPI.Helpers;
using GerenciamentoAPI.Models;
using GerenciamentoAPI.Repository;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace GerenciamentoAPI.Services
{
    public class GerenciamentoService
    {
        private readonly GerenciamentoDB _db;
        public GerenciamentoService(GerenciamentoDB db)
        {
            _db = db;
        }

        public User GetUser(int Id)
        {
            var users = (from user in _db.Users
                         where user.Id == Id
                         select user);
            if (users.Any())
                return users.First();
            else
                throw new UserNotFoundException("Usuário não encontrado para o Id especificado.");
        }

        public User GetUser(string Email)
        {
            IEnumerable<User> users = (from user in _db.Users
                                       where user.Email == Email
                                       select user);
            if (users.Any())
                return users.First();
            else
                throw new UserNotFoundException("Usuário não encontrado!");
        }

        public IEnumerable<BaseUser> ListUsers()
        {
            return _db.Users.Cast<BaseUser>();
        }

        public void SaveUser(UserInsertRequest user)
        {
            if (!Regexes.Email.IsMatch(user.Email))
            {
                throw new UserValidationException("Email invalido!");
            } 
            else if (user.Password.Length < 6)
            {
                throw new UserValidationException("Senha deve conter ao menos 6 caracteres.");
            }
            else if (user.Name.Length < 3)
            {
                throw new UserValidationException("Nome deve conter ao menos 3 caracteres.");
            }

            try
            {
                IEnumerable<User> users = (from e in _db.Users
                                           where e.Email == user.Email
                                           select e);
                if (users.Any())
                    throw new UserValidationException("Email já está em uso!");
            }
            catch (Exception)
            {
                throw;
            }

            string hashedPassword = PasswordHelper.Hash(user.Password);

            try
            {
                var newUser = new User()
                {
                    Email = user.Email,
                    Name = user.Name,
                    Password = hashedPassword,
                    Roles = Roles.Employee.ToString()
                };
                _db.Users.Add(newUser);
                _db.SaveChanges();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException exception)
            {
                throw exception;
            }
        }

        public IEnumerable<BaseActivity> ListActivities()
        {
            return _db.Activities.Cast<BaseActivity>();
        }

        public IEnumerable<BaseActivity> ListUserActivities(int userId)
        {
            var activities = from u in _db.Users
                             where u.Id == userId
                             from a in u.Activities
                             select new BaseActivity()
                             {
                                 Id = a.Id,
                                 ClosedAt = a.ClosedAt,
                                 CreatedAt = a.CreatedAt,
                                 Description = a.Description,
                                 Name = a.Name,
                                 Status = a.Status
                             };

            return activities.Cast<BaseActivity>();
        }

        public void SaveActivity(ActivityInsertRequest activity, int userId)
        {
            try
            {
                var user = GetUser(userId);
                var newActivity = new Activity()
                {
                    CreatedAt = DateTime.UtcNow,
                    Name = activity.Name,
                    Description = activity.Description,
                    Status = Status.OnGoing,
                };
                newActivity.Employees.Add(user);

                _db.Activities.Add(newActivity);
                _db.SaveChanges();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException exception)
            {
                throw exception;
            }
        }

        public void DeleteActivity(int activityId)
        {
            try
            {
                if (!_db.Activities.Any(u => u.Id == activityId))
                    throw new ActivityNotFoundException("Atividade não encontrada!");

                var activity = _db.Activities.First(u => u.Id == activityId);
                _db.Activities.Remove(activity);
                _db.SaveChanges();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException exception)
            {
                throw exception;
            }
        }

        public void SetActivityStatus(int activityId, Status status)
        {
            try
            {
                if (!_db.Activities.Any(u => u.Id == activityId))
                    throw new ActivityNotFoundException("Atividade não encontrada!");

                var activity = _db.Activities.First(u => u.Id == activityId);
                activity.Status = status;
                activity.ClosedAt = DateTime.UtcNow;
                _db.SaveChanges();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException exception)
            {
                throw exception;
            }
        }

        public void AddUserToActivity(int activityId, string email)
        {
            try
            {
                if (!_db.Users.Any(u => u.Email == email))
                    throw new UserNotFoundException("Usuario não encontrado!");

                if (!_db.Activities.Any(u => u.Id == activityId))
                    throw new ActivityNotFoundException("Atividade não encontrada!");

                var user = _db.Users.First(u => u.Email == email);
                var activity = _db.Activities.First(u => u.Id == activityId);

                activity.Employees.Add(user);
                _db.SaveChanges();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException exception)
            {
                throw exception;
            }
        }

        public IEnumerable<BaseUser> ListActivityUsers(int activityId)
        {
            var users = from a in _db.Activities
                        where a.Id == activityId
                        from u in a.Employees
                        select new BaseUser()
                        {
                            Id = u.Id,
                            Email = u.Email,
                            Name = u.Name,
                        };
            return users.Cast<BaseUser>();
        }

        public ActivitiesStatus GetActivitiesStatus(int userId)
        {
            var groupStatus = from u in _db.Users
                              where u.Id == userId
                              select new ActivitiesStatus()
                              {
                                  OnGoing = u.Activities.Count(x => x.Status == Status.OnGoing),
                                  Finished = u.Activities.Count(x => x.Status == Status.Finished),
                                  Interrupted = u.Activities.Count(x => x.Status == Status.Interrupted),
                              };


            if (!groupStatus.Any())
                return new() { Finished = 0, Interrupted = 0, OnGoing = 0 };

            var d = groupStatus.First();
            return d;
        }
    }
}
