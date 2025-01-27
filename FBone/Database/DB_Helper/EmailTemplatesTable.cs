using FBone.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Database.DB_Helper
{
    public class EmailTemplateTable
    {
        private readonly AppDBContext _context;
        public EmailTemplateTable(AppDBContext context)
        {
            this._context = context;
        }

        public IQueryable<EmailTemplate> GetTemplates()
        {
            return _context.EmailTemplate;
        }        

        public EmailTemplate GetEmailTemplateById(int id)
        {
            return _context.EmailTemplate.FirstOrDefault(i => i.Id == id);
        }

        public EmailTemplate GetEmailTemplateByEmailId(string emailId)
        {
            return _context.EmailTemplate.FirstOrDefault(i => i.EmailId == emailId);
        }

        internal void SaveEmailTemplate(EmailTemplate entity)
        {
            _context.Entry(entity).State = entity.Id == 0 ? EntityState.Added : EntityState.Modified;
            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            _context.EmailTemplate.Remove(new EmailTemplate() { Id = id });
            _context.SaveChanges();
        }

    }
}
