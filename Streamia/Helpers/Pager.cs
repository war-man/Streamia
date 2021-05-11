using Streamia.Models;
using Streamia.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Helpers
{
    public class Pager<Model> where Model : BaseEntity
    {
        private readonly IRepository<Model> repository;
        private readonly int currentPage;
        private readonly int recordsPerPage;
        private int totalPages;

        public Pager(IRepository<Model> repository, int? currentPage, int recordsPerPage)
        {
            this.repository = repository;
            this.currentPage = currentPage ?? 1;
            this.recordsPerPage = recordsPerPage;
        }

        public async Task<IEnumerable<Model>> GetPaginatedList()
        {
            double recordCount = await repository.Count();
            totalPages = (int) Math.Ceiling(recordCount / (recordsPerPage));
            int skipRange = (currentPage - 1) * recordsPerPage;
            return await repository.Paginate(skipRange, recordsPerPage);
        }

        public IDictionary<string, dynamic> GetPaginationData()
        {
            return new Dictionary<string, dynamic>
            {
                { "HasPrevious", currentPage > 1 },
                { "HasNext", currentPage <  totalPages },
                { "CurrentPage", currentPage },
                { "TotalPages", totalPages }
            };
        }
    }
}
