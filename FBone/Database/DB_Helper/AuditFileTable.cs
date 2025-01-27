using FBone.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Database.DB_Helper
{
    public class AuditFileTable
    {
        private readonly AppDBContext _context;
        public AuditFileTable(AppDBContext context)
        {
            this._context = context;
        }

        public AuditFile GetFileById(int id)
        {
            return _context.AuditFiles.FirstOrDefault(x => x.Id == id);
        }

        public int UploadFile(AuditFile auditFile)
        {
            RemoveNotLinkedFiles();
            _context.AuditFiles.Add(auditFile);
            _context.SaveChanges();
            return auditFile.Id;
        }
        private void RemoveNotLinkedFiles()
        {
            var listToDelete = _context.AuditFiles.Where(a => a.AuditId == null && a.CreatedDate < DateTime.Now.AddHours(-5)).ToList();
            if (listToDelete.Count > 0)
            {
                _context.RemoveRange(listToDelete);
                _context.SaveChanges();
            }
        }

        public void DeleteFiles(int auditId)
        {
            RemoveNotLinkedFiles();
            var auditFiles = _context.AuditFiles.Where(c => c.AuditId == auditId).ToList();
            if (auditFiles == null || auditFiles.Count == 0)
                return;
            _context.RemoveRange(auditFiles);
            _context.SaveChanges();            
        }

        public void LinkFileToAudit(int auditId, int fileId)
        {
            var audit = _context.Audits.AsNoTracking().Any(c => c.Id == auditId);
            if (!audit) throw new ArgumentException($"There is no audit with id {auditId}");
            var file = _context.AuditFiles.FirstOrDefault(ch => ch.Id == fileId);
            if (file == null) throw new ArgumentException($"There is no file with id {fileId}");
            file.AuditId = auditId;
            _context.AuditFiles.Update(file);
            _context.SaveChanges();
        }

        public (string, byte[]) GetFileContent(int auditId, int fileId)
        {
            var auditFile = _context.AuditFiles.FirstOrDefault(c => c.AuditId == auditId && c.Id == fileId);
            if (auditFile is null) throw new ArgumentException($"There is no file attached to audit {auditId}");

            return (auditFile.Name, auditFile.File);
        }

        public AuditFile GetFileByAuditId(int auditId)
        {
            return _context.AuditFiles.FirstOrDefault(ch => ch.AuditId == auditId);
        }
    }
}
