using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;

namespace DigitalPublishingPlatform.Models
{
    public class EncodingPreset 
    {

        public EncodingPreset() {
            Id = NextId;
            Target = MediaTarget.Unknown;
            IsAvailable = false;
            Definition = MediaDefinition.None;
            ShortDescription = String.Empty;
            Width = 0;
        }

        private static int _id = 0;
        private static int NextId {
            get { return ++_id; }
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
        public MediaType MediaType { get; set; }
        public MediaDefinition Definition { get; set; }
        public MediaTarget Target { get; set; }
        public string ShortDescription { get; set; }
        public int Width { get; set; }
        public bool IsAvailable { get; set; }

        public static IList<EncodingPreset> GetPresets() {
            return new List<EncodingPreset> {
                  new EncodingPreset {                    
                    Name = "Thumbnails",
                    Description = @"Produces a series of JPEG thumbnails 5 seconds apart, 300 pixels wide. The height is determined by the source frame size.
Use this preset name to generate a series of thumbnails for use in Xbox Live Applications.",
                    Group = "Thumbnails",
                    MediaType = MediaType.Image,
                    Definition = MediaDefinition.None,
                    Target = MediaTarget.Thumbnail,
                    IsAvailable = true,                    
                    ShortDescription = "Video Thumbnails",
                    Width = 300,

                },                
                 new EncodingPreset {
                    Name = "H264 Broadband 1080p",
                    Description = @"Produces a single MP4 file with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using AAC
- 1080p video CBR encoded at 6750 kbps using H.264 High Profile
Use this preset name to produce a downloadable file for 1080p (16:9 aspect ratio) content for delivery over broadband connections. The output file extension is *.mp4. If the source frame size is not 1920x1080, the video will be scaled horizontally to the width of the profile target of 1920 pixels, and its height will be scaled to match the aspect ratio of the source.",
                    Group = "H.264 Coding Standard",
                    MediaType = MediaType.Video,
                    IsAvailable = true,
                    Definition = MediaDefinition.Hd,
                    Target = MediaTarget.Html5,
                    ShortDescription = "Html5 HD - 1080p",
                    Width = 1920
                }, 
                new EncodingPreset {
                    Name = "H264 Broadband 720p",
                    Description = @"Produces a single MP4 file with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using AAC
- 720p video CBR encoded at 4500 kbps using H.264 Main Profile
Use this preset name to produce a downloadable file for 720p (16:9 aspect ratio) content for delivery over broadband connections. The output file extension is *. mp4. If the source frame size is not 1280x720, the video will be scaled horizontally to the width of the profile target of 1280 pixels, and its height will be scaled to match the aspect ratio of the source.",
                    Group = "H.264 Coding Standard",
                    MediaType = MediaType.Video,
                    IsAvailable = true,
                    Definition = MediaDefinition.Hd,
                    Target = MediaTarget.Html5,
                    ShortDescription = "Html5 HD - 720p",
                    Width = 1280
                }, 
                 new EncodingPreset {
                    Name = "H264 Broadband SD 16x9",
                    Description = @"Produces a single MP4 file with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using AAC
- SD video VBR encoded at 2200 kbps using H.264 Main Profile
Use this preset name to produce a downloadable file for SD (16:9 aspect ratio) content for delivery over broadband connections. The output file extension is *. mp4. If the source frame size is not 852x480, the video will be scaled horizontally to the width of the profile target of 852 pixels, and its height will be scaled to match the aspect ratio of the source.",
                    Group = "H.264 Coding Standard",
                    MediaType = MediaType.Video,
                    IsAvailable = true,
                    Definition = MediaDefinition.Sd,
                    Target = MediaTarget.Html5,
                    ShortDescription = "Html5 16x9 - 480p",
                    Width = 852
                }, 
                 new EncodingPreset {
                    Name = "H264 Broadband SD 4x3",
                    Description = @"Produces a single MP4 file with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using AAC
- SD video VBR encoded at 1800 kbps using H.264 Main Profile
Use this preset name to produce a downloadable file for SD (4:3 aspect ratio) content for delivery over broadband connections. The output file extension is *. mp4. If the source frame size is not 640x480, the video will be scaled horizontally to the width of the profile target of 640 pixels, and its height will be scaled to match the aspect ratio of the source.",
                    Group = "H.264 Coding Standard",
                    MediaType = MediaType.Video,
                    IsAvailable = true,
                    Definition = MediaDefinition.Sd,
                    Target = MediaTarget.Html5,
                    ShortDescription = "Html5 4x3 - 480p",
                    Width = 640
                }, 
                new EncodingPreset {
                    Name = "H264 Smooth Streaming 1080p",
                    Description = @"Produces a Smooth Streaming asset with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using AAC
- 1080p video CBR encoded at 8 bitrates ranging from 6000 kbps to 400 kbps using H.264 High Profile, and two second GOPs
Use this preset name to produce an asset from 1080p (16:9 aspect ratio) content for delivery via IIS Smooth Streaming. If the source frame size is not 1920x1080, will stretch the video at the highest bitrate horizontally to 1920 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.",
                    Group = "H.264 Coding Standard",
                    MediaType = MediaType.Video,
                    IsAvailable = true,
                    Definition = MediaDefinition.Hd,
                    Target = MediaTarget.SmoothStreaming,
                    ShortDescription = "Windows8 HD - 1080p",
                    Width = 1920
                }, 
                new EncodingPreset {
                    Name = "H264 Smooth Streaming 720p",
                    Description = @"Produces a Smooth Streaming asset with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 96 kbps using AAC
- 720p video CBR encoded at 6 bitrates ranging from 3400 kbps to 400 kbps using H.264 Main Profile, and two second GOPs
Use this preset name to produce an asset from 720p (16:9 aspect ratio) content for delivery via IIS Smooth Streaming. If the source frame size is not 1280x720, will stretch the video at the highest bitrate horizontally to 1280 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.",
                    Group = "H.264 Coding Standard",
                    MediaType = MediaType.Video,
                    IsAvailable = true,
                    Definition = MediaDefinition.Hd,
                    Target = MediaTarget.SmoothStreaming,
                    ShortDescription = "Windows8 HD - 720p",
                    Width = 1280
                }, 
                new EncodingPreset {
                    Name = "H264 Smooth Streaming 720p for 3G or 4G",
                    Description = @"Produces a Smooth Streaming asset with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 56 kbps using AAC
- 720p video CBR encoded at 8 bitrates ranging from 3400 kbps to 150 kbps using H.264 Main Profile, and two second GOPs.
Same as H264 Smooth Streaming 720p, with audio lowered to 56 kbps, and two additional lower bitrate video layers added at 250 kbps and 150 kbps. These lowest bitrate encodes should help when streaming over 3G or 4G connections to mobile devices",
                    Group = "H.264 Coding Standard",
                    MediaType = MediaType.Video,
                    IsAvailable = false,
                    Definition = MediaDefinition.Hd,
                    Target = MediaTarget.SmoothStreaming,
                    ShortDescription = "Windows8 HD (3G/4G) - 720p",
                    Width = 1280
                }, 
                new EncodingPreset {
                    Name = "H264 Smooth Streaming SD 16x9",
                    Description = @"Produces a Smooth Streaming asset with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 96 kbps using AAC
- SD video CBR encoded at 5 bitrates ranging from 1900 kbps to 400 kbps using H.264 Main Profile, and two second GOPs
Use this preset name to produce an asset from SD (16:9 aspect ratio) content for delivery via IIS Smooth Streaming. If the source frame size is not 852x480, will stretch the video at the highest bitrate horizontally to 852 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.",
                    Group = "H.264 Coding Standard",
                    MediaType = MediaType.Video,
                    IsAvailable = true,
                    Definition = MediaDefinition.Sd,
                    Target = MediaTarget.SmoothStreaming,
                    ShortDescription = "Windows8 16x9 - 480p",
                    Width = 852
                }, 
                new EncodingPreset {
                    Name = "H264 Smooth Streaming SD 4x3",
                    Description = @"Produces a Smooth Streaming asset with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 96 kbps using AAC
- SD video CBR encoded at 5 bitrates ranging from 1600 kbps to 400 kbps using H.264 Main Profile, and two second GOPs
Use this preset name to produce an asset from SD (4:3 aspect ratio) content for delivery via IIS Smooth Streaming. If the source frame size is not 640x480, will stretch the video at the highest bitrate horizontally to 640 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.",
                    Group = "H.264 Coding Standard",
                    MediaType = MediaType.Video,
                    IsAvailable = true,
                    Definition = MediaDefinition.Sd,
                    Target = MediaTarget.SmoothStreaming,
                    ShortDescription = "Windows8 4x3 - 480p",
                    Width = 640
                }, 

                #region Hidden presets
		new EncodingPreset {
                    Name = "WMA High Quality Audio",
                    Description = @"Produces a Windows Media file 44.1 kHz 16 bits/sample stereo audio encoded using WMA.
Use this preset name to produce an audio-only file for music services. The output file extension is *.wma.",
                    Group = "Audio Coding Standard",
                    MediaType = MediaType.Audio
                },
                 new EncodingPreset {
                    Name = "AAC Good Quality Audio",
                    Description = @"Produces an MP4 file containing 44.1 kHz 16 bits/sample stereo audio CBR encoded at 192 kbps using AAC.
Use this preset name to produce an audio-only file for music services. The output file extension is *.mp4.",
                    Group = "Audio Coding Standard",
                    MediaType = MediaType.Audio
                },
                 new EncodingPreset {
                    Name = "VC1 Broadband 1080p",
                    Description = @"Produces a single Windows Media file with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using WMA Pro
- 1080p video VBR encoded at 6750 kbps using VC-1 Advanced Profile
Use this preset name to produce a downloadable file for 1080p (16:9 aspect ratio) content for delivery over broadband connections. The output file extension is *.wmv. If the source frame size is not 1920x1080, the video will be scaled horizontally to the width of the profile target (e.g. 1920, 1280, 852 or 640 pixels), and its height will be scaled to match the aspect ratio of the source.",
                    Group = "VC-1 Coding Standard",
                    MediaType = MediaType.Video
                },     
                new EncodingPreset {
                    Name = "VC1 Broadband 720p",
                    Description = @"Produces a single Windows Media file with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using WMA Pro
- 720p video VBR encoded at 4500 kbps using VC-1 Advanced Profile
Use this preset name to produce a downloadable file for 720p (16:9 aspect ratio) content for delivery over broadband connections. The output file extension is *.wmv. If the source frame size is not 1280x720, the video will be scaled horizontally to the width of the profile target of 1280 pixels, and its height will be scaled to match the aspect ratio of the source.",
                    Group = "VC-1 Coding Standard",
                    MediaType = MediaType.Video
                },     
                new EncodingPreset {
                    Name = "VC1 Broadband SD 16x9",
                    Description = @"Produces a single Windows Media file with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using WMA Pro
- SD video VBR encoded at 2200 kbps using VC-1 Advanced Profile
Use this preset name to produce a downloadable file for SD (16:9 aspect ratio) content for delivery over broadband connections. The output file extension is *.wmv. If the source frame size is not 852x480, the video will be scaled horizontally to the width of the profile target of 852 pixels, and its height will be scaled to match the aspect ratio of the source.",
                    Group = "VC-1 Coding Standard",
                    MediaType = MediaType.Video
                },     
                new EncodingPreset {
                    Name = "VC1 Broadband SD 4x3",
                    Description = @"Produces a single Windows Media file with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using WMA Pro
- SD video VBR encoded at 1800 kbps using VC-1 Advanced Profile
Use this preset name to produce a downloadable file for SD (4:3 aspect ratio) content for delivery over broadband connections. The output file extension is *.wmv. If the source frame size is not 640x480, the video will be scaled horizontally to the width of the profile target of 640 pixels, and its height will be scaled to match the aspect ratio of the source.",
                    Group = "VC-1 Coding Standard",
                    MediaType = MediaType.Video
                },  
                new EncodingPreset {
                    Name = "VC1 Smooth Streaming 1080p",
                    Description = @"Produces a Smooth Streaming asset with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using WMA Pro
- 1080p video VBR encoded at 8 bitrates ranging from 6000 kbps to 400 kbps using VC-1 Advanced Profile, and two second GOPs
Use this preset name to produce an asset from 1080p (16:9 aspect ratio) content for delivery via IIS Smooth Streaming. If the source frame size is not 1920x1080, will stretch the video at the highest bitrate horizontally to 1920 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled respectively.",
                    Group = "VC-1 Coding Standard",
                    MediaType = MediaType.Video
                },  
                new EncodingPreset {
                    Name = "VC1 Smooth Streaming 720p",
                    Description = @"Produces a Smooth Streaming asset with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using WMA Pro
- 720p video VBR encoded at 6 bitrates ranging from 3400 kbps to 400 kbps using VC-1 Advanced Profile, and two second GOPs
Use this preset name to produce an asset from 720p (16:9 aspect ratio) content for delivery via IIS Smooth Streaming. If the source frame size is not 1280x720, will stretch the video at the highest bitrate horizontally to 1280 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled respectively.",
                    Group = "VC-1 Coding Standard",
                    MediaType = MediaType.Video
                }, 
                new EncodingPreset {
                    Name = "VC1 Smooth Streaming SD 16x9",
                    Description = @"Produces a Smooth Streaming asset with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 64 kbps using WMA Pro
- SD video VBR encoded at 5 bitrates ranging from 1900 kbps to 400 kbps using VC-1 Advanced Profile, and two second GOPs
Use this preset name to produce an asset from SD (16:9 aspect ratio) content for delivery via IIS Smooth Streaming. If the source frame size is not 852x480, will stretch the video at the highest bitrate horizontally to 852 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled respectively.",
                    Group = "VC-1 Coding Standard",
                    MediaType = MediaType.Video
                }, 
                new EncodingPreset {
                    Name = "VC1 Smooth Streaming SD 4x3",
                    Description = @"Produces a Smooth Streaming asset with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 64 kbps using WMA Pro
- SD video VBR encoded at 5 bitrates ranging from 1600 kbps to 400 kbps using VC-1 Advanced Profile, and two second GOPs
Use this preset name to produce an asset from SD (4:3 aspect ratio) content for delivery via IIS Smooth Streaming. If the source frame size is not 640x480, will stretch the video at the highest bitrate horizontally to 640 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled respectively.",
                    Group = "VC-1 Coding Standard",
                    MediaType = MediaType.Video
                }, 
                new EncodingPreset {
                    Name = "VC1 Smooth Streaming 1080p Xbox Live ADK",
                    Description = @"Produces a Smooth Streaming asset with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using WMA Pro
- 1080p video VBR encoded at 10 bitrates ranging from 9000 kbps to 350 kbps using VC-1 Advanced Profile, and two second GOPs
Use this preset name to produce an asset from 1080p (16:9 aspect ratio) content for delivery via IIS Smooth Streaming to Xbox Live Applications. If the source frame size is not 1920x1080, will stretch the video at the highest bitrate horizontally to 1920 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled respectively.",
                    Group = "VC-1 Coding Standard",
                    MediaType = MediaType.Video
                }, 
                new EncodingPreset {
                    Name = "VC1 Smooth Streaming 720p Xbox Live ADK",
                    Description = @"Produces a Smooth Streaming asset with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using WMA Pro
- 720p video VBR encoded at 8 bitrates ranging from 4500 kbps to 350 kbps using VC-1 Advanced Profile, and two second GOPs
Use this preset name to produce an asset from 720p (16:9 aspect ratio) content for delivery via IIS Smooth Streaming to Xbox Live Applications. If the source frame size is not 1280x720, will stretch the video at the highest bitrate horizontally to 1280 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled respectively.",
                    Group = "VC-1 Coding Standard",
                    MediaType = MediaType.Video
                }, 
                new EncodingPreset {
                    Name = "H264 Adaptive Bitrate MP4 Set 1080p",
                    Description = @"Produces an asset with multiple GOP-aligned MP4 files:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using AAC
- 1080p video CBR encoded at 8 bitrates ranging from 6000 kbps to 400 kbps using H.264 High Profile, and two second GOPs
Use this preset name to produce an asset from 1080p (16:9 aspect ratio) content for delivery via one of many adaptive streaming technologies after suitable packaging. If the source frame size is not 1920x1080, will stretch the video at the highest bitrate horizontally to 1920 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.",
                    Group = "H.264 Coding Standard",
                    MediaType = MediaType.Video
                }, 
                new EncodingPreset {
                    Name = "H264 Adaptive Bitrate MP4 Set 720p",
                    Description = @"Produces an asset with multiple GOP-aligned MP4 files:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 96 kbps using AAC
- 720p video CBR encoded at 6 bitrates ranging from 3400 kbps to 400 kbps using H.264 Main Profile, and two second GOPs
Use this preset name to produce an asset from 720p (16:9 aspect ratio) content for delivery via one of many adaptive streaming technologies after suitable packaging. If the source frame size is not 1280x720, will stretch the video at the highest bitrate horizontally to 1280 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.",
                    Group = "H.264 Coding Standard",
                    MediaType = MediaType.Video
                }, 
                new EncodingPreset {
                    Name = "H264 Adaptive Bitrate MP4 Set SD 16x9",
                    Description = @"Produces an asset with multiple GOP-aligned MP4 files:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 96 kbps using AAC
- SD video CBR encoded at 5 bitrates ranging from 1900 kbps to 400 kbps using H.264 Main Profile, and two second GOPs
Use this preset name to produce an asset from SD (16:9 aspect ratio) content for delivery via one of many adaptive streaming technologies after suitable packaging. If the source frame size is not 852x480, will stretch the video at the highest bitrate horizontally to 852 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.",
                    Group = "H.264 Coding Standard",
                    MediaType = MediaType.Video
                }, 
                new EncodingPreset {
                    Name = "H264 Adaptive Bitrate MP4 Set SD 4x3",
                    Description = @"Produces an asset with multiple GOP-aligned MP4 files:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 96 kbps using AAC
- SD video CBR encoded at 5 bitrates ranging from 1600 kbps to 400 kbps using H.264 Main Profile, and two second GOPs
Use this preset name to produce an asset from SD (4:3 aspect ratio) content for delivery via one of many adaptive streaming technologies after suitable packaging. If the source frame size is not 640x480, will stretch the video at the highest bitrate horizontally to 640 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.",
                    Group = "H.264 Coding Standard",
                    MediaType = MediaType.Video
                }, 
                new EncodingPreset {
                    Name = "H264 Adaptive Bitrate MP4 Set 720p for iOS Cellular Only",
                    Description = @"Produces an asset with multiple GOP-aligned MP4 files:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 56 kbps using AAC
- 720p video CBR encoded at 6 bitrates ranging from 3400 kbps to 400 kbps using H.264 Main Profile, and two second GOPs
Use this preset to produce an asset from 720p (16:9 aspect ratio) content for delivery via one of many adaptive streaming technologies after suitable packaging. If source frame size is not 1280x720, will stretch the video at the highest bitrate horizontally to 1280 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.
Audio is encoded at a low bitrate of 56 kbps, in order to satisfy App Store requirements for HLS. For more information, see Resolving App Store Approval Issues for HTTP Live Streaming.",
                    Group = "H.264 Coding Standard",
                    MediaType = MediaType.Video
                }, 
                new EncodingPreset {
                    Name = "H264 Adaptive Bitrate MP4 Set SD 16x9 for iOS Cellular Only",
                    Description = @"Produces an asset with multiple GOP-aligned MP4 files:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 56 kbps using AAC
- SD video CBR encoded at 5 bitrates ranging from 1900 kbps to 400 kbps using H.264 Main Profile, and two second GOPs
Use this preset to produce an asset from SD (16:9 aspect ratio) content for delivery via one of many adaptive streaming technologies after suitable packaging. If source frame size is not 852x480, will stretch the video at the highest bitrate horizontally to 852 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.
Audio is encoded at a low bitrate of 56 kbps, in order to satisfy App Store requirements for HLS. For more information, see Resolving App Store Approval Issues for HTTP Live Streaming.",
                    Group = "H.264 Coding Standard",
                    MediaType = MediaType.Video
                }, 
                new EncodingPreset {
                    Name = "H264 Adaptive Bitrate MP4 Set SD 4x3 for iOS Cellular Only",
                    Description = @"Produces an asset with multiple GOP-aligned MP4 files:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 56 kbps using AAC
- SD video CBR encoded at 5 bitrates ranging from 1600 kbps to 400 kbps using H.264 Main Profile, and two second GOPs
Use this preset to produce an asset from SD (4:3 aspect ratio) content for delivery via one of many adaptive streaming technologies after suitable packaging. If source frame size is not 640x480, will stretch the video at the highest bitrate horizontally to 640 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.
Audio is encoded at a low bitrate of 56 kbps, in order to satisfy App Store requirements for HLS. For more information, see Resolving App Store Approval Issues for HTTP Live Streaming.",
                    Group = "H.264 Coding Standard",
                    MediaType = MediaType.Video
                }, 
                new EncodingPreset {
                    Name = "H264 Smooth Streaming 720p Xbox Live ADK",
                    Description = @"Produces a Smooth Streaming asset with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 96 kbps using AAC
- 720p video CBR encoded at 8 bitrates ranging from 4500 kbps to 350 kbps using H.264 High Profile, and two second GOPs
Use this preset name to produce an asset from 720p (16:9 aspect ratio) content for delivery via IIS Smooth Streaming to Xbox Live Applications. If the source frame size is not 1280x720, will stretch the video at the highest bitrate horizontally to 1280 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled respectively.",
                    Group = "H.264 Coding Standard",
                    MediaType = MediaType.Video
                }, 
                new EncodingPreset {
                    Name = "H264 Smooth Streaming Windows Phone 7 Series",
                    Description = @"Produces a Smooth Streaming asset with:
- 44.1 kHz 16 bits/sample stereo audio CBR encoded at 64 kbps using HE-AAC Level 1
- SD video CBR encoded at 5 bitrates ranging from 1000 kbps to 200 kbps using H.264 Main Profile, and two second GOPs
Use this preset name to produce an asset from SD (16:9 aspect ratio) content for delivery via IIS Smooth Streaming to Windows Phone 7 Series devices. If the source frame size is not 640x360, will stretch the video at all bitrates horizontally to 640 pixels, and the height will increase/decrease correspondingly.",
                    Group = "H.264 Coding Standard",
                    MediaType = MediaType.Video
                }, 
	#endregion 
            };
        }
    }
}