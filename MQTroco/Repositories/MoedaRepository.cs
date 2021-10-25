using MQTroco.Context;
using MQTroco.Models;
using MQTroco.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQTroco.Repositories
{
    public class MoedaRepository : IMoedaRepository
    {
        private readonly AppDbContext _context;

        public MoedaRepository(AppDbContext context)
        {
            _context = context;
        }
        public List<MoedaModel> Moedas => _context.Moedas.ToList();

        public string AddMoeda(MoedaModel moeda)
        {
            throw new NotImplementedException();
        }

        public MoedaModel GetMoedaById(string moeda)
        {
            try
            {
                return _context.Moedas.Where(w => w.Id == moeda).FirstOrDefault();
            }
            catch
            {
                return new MoedaModel();
            }
        }
    }
}
