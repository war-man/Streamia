using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Helpers
{
    public static class Transcoder
    {
        /*
        - APNG(Animated protable network graphics)
        - Chinese AVS (Audio video standard ) (AVS1-P2 jizhum profile)(encoder libxaavs ) 
        - Cinepak 
        - ffmpeg video codec #1 
        - flash screen video  V1 
        - flash screen video  V2 
        - FLV / sorenson spark / sorenson H263(flash video )(decodes:Flv) (encoder flv )
        - GIF (graphic interchange formate ) 
        - H.261 
        - H.263
        - H.264 / AVC /Mpeg-4 AVC /Mpeg4 part 10 (encode libx264 libx264rgb)
        - H.265
        - MPEG-1 video 
        - MPEG-2 video
        - MPEG-4
        - microsoft video 1 
        */
        public static string[] VideoCodecs = {
            "copy",
            "h261",
            "h263",
            "h263p",
            "h264",
            "hevc",
            "apng",
            "cavs",
            "cinepak",
            "ffv1",
            "flashsv",
            "flashsv2",
            "flv1",
            "gif",
            "mpeg1video",
            "mpeg2video",
            "mpeg4",
            "msmpeg4v2",
            "msmpeg4v3",
            "msvideo1",
            "png",
            "qtrle",
            "roq",
            "rv10",
            "rv20",
            "snow",
            "svq1",
            "theora",
            "vp8",
            "vp9",
            "wmv1",
            "wmv2",
            "zmbv"
        };

        /*
            Uncompressed audio formats (WAV - AIFF - AU - raw - PCM ) 
            Formats with lossless compression (FLAC - Monkey's Audio - TTA - WavPack - ATRAC - .ape )
            Advanced lossless(ALAC - .m4a - MPEG-4 ALS - MPEG-4 DST - Windows Media Audio - SHN )
            Format with lossy compression ( OPUS  - MP3 - Vorbis - AAC - MusePack - Windows Media Audio Lossy ) 
        
            "MKV","MKA", "MP4", "MPA", "FLV"
        ,   "F4V","3GP","3G2","MPG","PS","TS Stream","flac"
        ,   "ALAC",".m4a","MPEG-4 ALS","MPEG-4 DST"," Windows Media Audio","SHN"
        ,   "OPUS","MP3","Vorbis","AAC","MusePack","Windows Media Audio Lossy","ac3","libfdk_aac"
         */
        public static string[] AudioCodecs = {
            "copy",
            "aac",
            "ac3",
            "mp2",
            "mp3",
            "wmav1",
            "wmav2",
            "wavpack",
            "adpcm_adx",
            "adpcm_g722",
            "adpcm_g726",
            "adpcm_ima_qt",
            "adpcm_ima_wav",
            "adpcm_ms",
            "adpcm_swf",
            "adpcm_yamaha",
            "comfortnoise",
            "dts",
            "eac3",
            "g723_1",
            "nellymoser",
            "opus",
            "pcm_alaw",
            "pcm_mulaw",
            "ra_144",
            "roq_dpcm",
            "vorbis"
        };

        public static string[] Presets =
        {
            "default",
            "ultrafast",
            "superfast",
            "veryfast",
            "faster",
            "fast",
            "slow",
            "slower",
            "veryslow",
            "placebo"
        };

        public static string[] VideoProfiles =
        {
            "none",
            "baseline -level 3.0",
            "baseline -level 3.1",
            "main -level 3.1",
            "main -level 4.0",
            "high -level 4.0",
            "high -level 4.1",
            "high -level 4.2"
        };

        public static string[] AsbectRatios = {
            "1:1",
            "2:1",
            "3:2",
            "4:1",
            "4:3",
            "5:4",
            "7:6",
            "16:9",
            "16:10", 
            "21:9",
            "32:9",
            "256:135",
            "1.85:1",
            "2.35:1",
            "2.39:1"
        };

        public static int[] AudioBitrates = { 
            96,
            112,
            128,
            160,
            192,
            256,
            320 
        };
    }
}
