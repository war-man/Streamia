using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models.Enums
{
    public enum StreamState
    {
        Stopped,
        Live,
        Transcoding,
        Ready,
        Error
    }
}
