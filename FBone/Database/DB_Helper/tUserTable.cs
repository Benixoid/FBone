using FBone.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FBone.Database.DB_Helper
{
    public class tUserTable
    {
        private readonly AppDBContext _context;
        public tUserTable(AppDBContext context)
        {
            this._context = context;
        }

        public IQueryable<tUser> GetUsers()
        {
            return _context.tUser.Include(i => i.Position).Include(i => i.Area).Include(i => i.Facility);
        }

        public IQueryable<tUser> GetActiveUsers()
        {
            return _context.tUser.Include(i => i.Position).Include(i => i.Area).Include(i => i.Facility).Where(i => i.IsActive);
        }

        public tUser GetUserById(int id)
        {
            return _context.tUser.Include(i => i.Position).FirstOrDefault(i => i.Id == id);
        }
        public tUser GetUserByCAI(string cai)
        {
            return _context.tUser.Include(i => i.Position).Include(i => i.Area).FirstOrDefault(i => i.CAI == cai);
        }

        internal void SaveUser(tUser entity)
        {
            if (entity.Id == 0)
                _context.Entry(entity).State = EntityState.Added;
            else
                _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        internal bool CanCreateAkt(string userId)
        {
            return GetUserByCAI(userId).Position.CanCreateAct;
        }
        internal bool CanCreateAudit(string userId)
        {
            return GetUserByCAI(userId).Position.IsAuditCreator;
        }

        internal bool CanTranslateAkt(string userId)
        {
            return GetUserByCAI(userId).Position.CanTranslateAct;
        }

        internal bool IsShiftEngineer(string userId)
        {
            return GetUserByCAI(userId).Position.IsShiftEngineer;
        }
        internal bool IsNModeAdministrator(string userId)
        {
            return GetUserByCAI(userId).Position.IsNModeAdministrator;
        }
        internal bool IsNModeEditor(string userId)
        {
            var user = GetUserByCAI(userId);
            if (user == null || user.Position == null) return false;
            if (user.Position.IsNModeAdministrator) return true;
            return user.Position.IsNModeEditor;
        }
        internal bool IsNModeUser(string userId)
        {
            var user = GetUserByCAI(userId);
            if (user == null || user.Position == null) return false;

            if (user.Position.IsNModeAdministrator || user.Position.IsNModeEditor) return true;
            return user.Position.IsNModeUser;
        }
        internal string GetUsersList(int positionId, string userLang)
        {
            var res = "";
            List<string> users = new List<string>();
            var userlist = GetUsersByPosition(positionId, userLang);
            if (userLang == "KK")
            {
                userlist = userlist.OrderBy(i => i.Name_KK);
                users = userlist.Select(i => i.Name_KK).ToList();
            }
            else if (userLang == "RU")
            {
                userlist = userlist.OrderBy(i => i.Name_RU);
                users = userlist.Select(i => i.Name_RU).ToList();
            }
            else if (userLang == "EN")
            {
                userlist = userlist.OrderBy(i => i.Name_EN);
                users = userlist.Select(i => i.Name_EN).ToList();
            }
            res = string.Join(Environment.NewLine, users);
            return res;
        }
        public IQueryable<tUser> GetUsersByPosition(int pos, string lang)
        {
            var userlist = _context.tUser.OrderBy(i => i.Name_EN).Where(i => i.PositionId == pos && i.IsActive);
            if (lang == "KK")
            {
                userlist = userlist.OrderBy(i => i.Name_KK);
            }
            else if (lang == "RU")
            {
                userlist = userlist.OrderBy(i => i.Name_RU);
            }
            else if (lang == "EN")
            {
                userlist = userlist.OrderBy(i => i.Name_EN);
            }
            return userlist;
        }
    }
}
