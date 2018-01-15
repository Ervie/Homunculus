using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarekMotykaBot.Services
{
    public interface IDiscordService
    {
        IConfiguration Configuration { get; set; }
    }
}
