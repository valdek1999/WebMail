using System;
using System.Collections.Generic;
using System.Linq;
using WebMail.Models;
using WebMail.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebMail.Config;

namespace WebMail.Repository
{
    /// <summary>
    /// A class for managing the context of a message database
    /// </summary>
    public class MailRepository : IRepository<Mail>
    {
        private readonly WebMailDbContext _context;

        /// <summary>
        /// Property of the list of all messages
        /// </summary>
        public IEnumerable<Mail> All => _context.Mail.Include(x => x.Recipients).ToList();
        public MailRepository(WebMailDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Adds the Mail entity to the database
        /// </summary>
        /// <param name="entity">The Mail entity</param>
        /// <returns>Returns the Mail object</returns>
        public Mail Add(Mail entity)
        {
            _context.Mail.Add(entity);
            _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Deletes a message from the database
        /// </summary>
        /// <param name="entity"> The Mail entity</param>
        public void Delete(Mail entity)
        {
            _context.Mail.Remove(entity);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates a message in the database
        /// </summary>
        /// <param name="entity"> The Mail entity</param>
        public void Update(Mail entity)
        {
            _context.Mail.Update(entity);
            _context.SaveChanges();
        }

        /// <summary>
        /// Search for a message by id
        /// </summary>
        /// <param name="Id">ID of the Mail object</param>
        /// <returns></returns>
        public Mail FindById(int Id)
        {
            return _context.Mail.FirstOrDefault(e => e.Id == Id);    
        }

        /// <summary>
        /// Searches for a list of all messages by a certain condition
        /// </summary>
        /// <param name="predicate">Search condition</param>
        /// <returns>IEnumerable<Mail> list of all found objects by predicate</returns>
        public IEnumerable<Mail> FindAll(Func<Mail,bool> predicate)
        {
            IEnumerable<Mail> mails = All
                .Where(predicate)
                .AsQueryable()
                .Include(x => x.Recipients)
                .ToList();
            return mails;
        }        
    }
}
