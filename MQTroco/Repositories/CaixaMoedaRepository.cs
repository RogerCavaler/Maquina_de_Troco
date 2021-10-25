using Microsoft.EntityFrameworkCore;
using MQTroco.Context;
using MQTroco.Models;
using MQTroco.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQTroco.Repositories
{
    public class CaixaMoedaRepository : ICaixaMoedaRepository
    {
        private readonly AppDbContext _context;

        public CaixaMoedaRepository(AppDbContext context)
        {
            _context = context;
        }
        public void AddMoeda(CaixaMoedaModel moeda)
        {
            try
            {
                moeda.Id = Common.NewId();
                _context.CaixaMoedas.Add(moeda);
                _context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public List<CaixaMoedaModel> CaixaMoedas(string idcaixa)
        {
            return _context.CaixaMoedas.Where(w => w.CaixaModelId == idcaixa).Include(i => i.CaixaModel).Include(i => i.MoedaModel).ToList();
        }

        public CaixaMoedaModel CaixaMoedasById(string id)
        {
            return _context.CaixaMoedas.Where(w => w.Id == id).Include(i => i.CaixaModel).Include(i => i.MoedaModel).FirstOrDefault();
        }

        public void UpdateMoeda(CaixaMoedaModel moeda)
        {
            try
            {
                _context.CaixaMoedas.Attach(moeda);
                _context.Entry(moeda).Property(p => p.QtdMoeda).IsModified = true;
                _context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
    }
}
