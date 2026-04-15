using LinasHotell.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinasHotell.Controllers
{
    public class RoomController
    {
        private readonly IRoomRepository _repository;

        public RoomController(IRoomRepository repository)
        {
            _repository = repository;
        }
    }
}
