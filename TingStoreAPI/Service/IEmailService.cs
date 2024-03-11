using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TingStoreAPI.Helper;

namespace TingStoreAPI.Service
{
    public interface IEmailService
    {
        Task SendEmailAsync(Mailrequest mailrequest);
    }
}