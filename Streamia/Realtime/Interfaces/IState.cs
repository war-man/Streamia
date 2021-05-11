using Streamia.Models;
using Streamia.Models.Enums;
using Streamia.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Realtime.Interfaces
{
    public interface IState<T> where T : BaseEntity
    {
        public Task Update(int sourceId, StreamState state);
        public IList<T> Start(IList<T> servers, string preCommand, string command);
        public IList<T> Stop(IList<T> servers, string preCommand);
    }
}
