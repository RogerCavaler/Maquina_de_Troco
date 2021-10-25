using MQTroco.Context;
using MQTroco.Models;
using MQTroco.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQTroco.Repositories
{
    public class CaixaRepository : ICaixaRepository
    {
        private readonly AppDbContext _context;

        public CaixaRepository(AppDbContext context)
        {
            _context = context;
        }
        public List<CaixaModel> Caixas => _context.Caixas.ToList();

        public string AddCaixa(CaixaModel caixa)
        {
            try
            {
                _context.Caixas.Add(caixa);
                _context.SaveChanges();
                return caixa.Id;
            }
            catch
            {
                throw;
            }
        }

        public CaixaModel GetCaixaById(string id)
        {
            try
            {
                return _context.Caixas.Where(w => w.Id == id).FirstOrDefault();
            }
            catch
            {
                return new CaixaModel();
            }
        }

        public bool UpdateCaixa(CaixaModel caixa)
        {
            try
            {
                _context.Caixas.Attach(caixa);
                _context.Entry(caixa).Property(p => p.DataFechamento).IsModified = true;
                return _context.SaveChanges() > 0;
            }
            catch
            {
                throw;
            }
        }
    }
}
