using HMS.DataAccess.Entity;
using HMS.DataAccess.Infrastructure;
using HMS.DataAccess.UnitOfwork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HMS.DataAccess.Service
{
    public class CommonService : IDisposable
    {
        User User { get; }
        UnitOfWork DbContext { get; }
        public CommonService()
        {
            DbContext = new UnitOfWork(new ConnectionFactory());
        }
        public CommonService(Database database)
        {
            DbContext = new UnitOfWork(new ConnectionFactory(database));
        }
        public CommonService(User user)
        {
            DbContext = new UnitOfWork(new ConnectionFactory(Database.VEN), user);
        }

        public string GetTemplateId(string TemplateName)
        {
            try
            {
                return DbContext.Common.GetTemplateId(TemplateName);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SendMail(Mailing Entity)
        {
            try
            {
                DbContext.Common.SendMail(Entity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Menu> GetMenu()
        {
            try
            {
                var List = DbContext.Common.GetMenu();

                var Menu = List.GroupBy(x => new { x.MenuId, x.Menu, x.MenuSort })
                               .Select(x => new Menu
                               {
                                   Id = x.Key.MenuId,
                                   Name = x.Key.Menu,
                                   Sort = x.Key.MenuSort,
                                   FormList = List.Where(y => y.MenuId == x.Key.MenuId).Select(y => new Form
                                   {
                                       Id = y.FormId,
                                       Name = y.Form,
                                       Action = y.Action,
                                       Controller = y.Controller,
                                       Sort = y.FormSort
                                   }).OrderBy(o => o.Action).ToList()
                               }).ToList();

                return Menu;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public MenuDetail GetForm(string Form)
        {
            var form = DbContext.Common.GetMenu(Form)?.FirstOrDefault();

            return form;
        }
        public IEnumerable<T> GetQuery<T>(string Type)
        {
            return DbContext.Common.GetQuery<T>(Type);
        }
        public void Dispose()
        {
            DbContext?.Dispose();
        }
    }
}
