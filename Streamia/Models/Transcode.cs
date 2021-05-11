using Streamia.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models
{
    public class Transcode : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [Display(Name ="Video Codec")]
        public string VideoCodec { get; set; }

        [Display(Name = "Video Profile")]
        public string VideoProfile { get; set; }

        [Display(Name = "Audio Codec")]
        public string AudioCodec { get; set; }

        [Display(Name = "Audio Channels")]
        public uint AudioChannels { get; set; }

        [Display(Name = "Analyze Duration")]
        public uint AnalyzeDuration { get; set; }
        public uint Probsize { get; set; }
        public string Preset { get; set; }
        public uint CRF { get; set; } = 20;
        public uint Threads { get; set; }

        // 1920x1080
        [Display(Name = "Minimum Bitrate")]
        public uint MinBitrate_1080 { get; set; }

        [Display(Name = "Maximum Bitrate")]
        public uint MaxBitrate_1080 { get; set; } = 5350;

        [Display(Name = "Video Bitrate")]
        public uint VideoBitrate_1080 { get; set; } = 5000;

        [Display(Name = "Audio Bitrate")]
        public uint AudioBitrate_1080 { get; set; } = 192;

        [Display(Name = "Audio Sample Rate")]
        public uint AudioSampleRate_1080 { get; set; } = 48000;

        [Display(Name = "Buffer Size")]
        public uint BufferSize_1080 { get; set; } = 7500;

        // 1280x720
        [Display(Name = "Minimum Bitrate")]
        public uint MinBitrate_720 { get; set; }

        [Display(Name = "Maximum Bitrate")]
        public uint MaxBitrate_720 { get; set; } = 2996;

        [Display(Name = "Video Bitrate")]
        public uint VideoBitrate_720 { get; set; } = 2800;

        [Display(Name = "Audio Bitrate")]
        public uint AudioBitrate_720 { get; set; } = 128;

        [Display(Name = "Audio Sample Rate")]
        public uint AudioSampleRate_720 { get; set; } = 48000;

        [Display(Name = "Buffer Size")]
        public uint BufferSize_720 { get; set; } = 4200;

        // 842x480
        [Display(Name = "Minimum Bitrate")]
        public uint MinBitrate_480 { get; set; }

        [Display(Name = "Maximum Bitrate")]
        public uint MaxBitrate_480 { get; set; } = 1498;

        [Display(Name = "Video Bitrate")]
        public uint VideoBitrate_480 { get; set; } = 1400;

        [Display(Name = "Audio Bitrate")]
        public uint AudioBitrate_480 { get; set; } = 128;

        [Display(Name = "Audio Sample Rate")]
        public uint AudioSampleRate_480 { get; set; } = 48000;

        [Display(Name = "Buffer Size")]
        public uint BufferSize_480 { get; set; } = 2100;

        // 640x360
        [Display(Name = "Minimum Bitrate")]
        public uint MinBitrate_360 { get; set; }

        [Display(Name = "Maximum Bitrate")]
        public uint MaxBitrate_360 { get; set; } = 856;

        [Display(Name = "Video Bitrate")]
        public uint VideoBitrate_360 { get; set; } = 800;

        [Display(Name = "Audio Bitrate")]
        public uint AudioBitrate_360 { get; set; } = 96;

        [Display(Name = "Audio Sample Rate")]
        public uint AudioSampleRate_360 { get; set; } = 48000;

        [Display(Name = "Buffer Size")]
        public uint BufferSize_360 { get; set; } = 1200;
    }
}
