using System;
using Sonic;
using SlimDX.Direct3D9;
using System.Runtime.InteropServices;
using System.Drawing;
using DirectShow;
using System.Collections.Generic;
using SlimDX;
using System.Text;
using System.Security;
using System.Threading;
using System.Windows.Forms;

namespace EVR
{
    #region Declarations

    public static class MFServices
    {
        public static readonly Guid MF_TIMECODE_SERVICE = new Guid(0xa0d502a7, 0x0eb3, 0x4885, 0xb1, 0xb9, 0x9f, 0xeb, 0x0d, 0x08, 0x34, 0x54);
        public static readonly Guid MF_PROPERTY_HANDLER_SERVICE = new Guid(0xa3face02, 0x32b8, 0x41dd, 0x90, 0xe7, 0x5f, 0xef, 0x7c, 0x89, 0x91, 0xb5);
        public static readonly Guid MF_METADATA_PROVIDER_SERVICE = new Guid(0xdb214084, 0x58a4, 0x4d2e, 0xb8, 0x4f, 0x6f, 0x75, 0x5b, 0x2f, 0x7a, 0xd);
        public static readonly Guid MF_PMP_SERVER_CONTEXT = new Guid(0x2f00c910, 0xd2cf, 0x4278, 0x8b, 0x6a, 0xd0, 0x77, 0xfa, 0xc3, 0xa2, 0x5f);
        public static readonly Guid MF_QUALITY_SERVICES = new Guid(0xb7e2be11, 0x2f96, 0x4640, 0xb5, 0x2c, 0x28, 0x23, 0x65, 0xbd, 0xf1, 0x6c);
        public static readonly Guid MF_RATE_CONTROL_SERVICE = new Guid(0x866fa297, 0xb802, 0x4bf8, 0x9d, 0xc9, 0x5e, 0x3b, 0x6a, 0x9f, 0x53, 0xc9);
        public static readonly Guid MF_REMOTE_PROXY = new Guid(0x2f00c90e, 0xd2cf, 0x4278, 0x8b, 0x6a, 0xd0, 0x77, 0xfa, 0xc3, 0xa2, 0x5f);
        public static readonly Guid MF_SAMI_SERVICE = new Guid(0x49a89ae7, 0xb4d9, 0x4ef2, 0xaa, 0x5c, 0xf6, 0x5a, 0x3e, 0x5, 0xae, 0x4e);
        public static readonly Guid MF_SOURCE_PRESENTATION_PROVIDER_SERVICE = new Guid(0xe002aadc, 0xf4af, 0x4ee5, 0x98, 0x47, 0x05, 0x3e, 0xdf, 0x84, 0x04, 0x26);
        public static readonly Guid MF_TOPONODE_ATTRIBUTE_EDITOR_SERVICE = new Guid(0x65656e1a, 0x077f, 0x4472, 0x83, 0xef, 0x31, 0x6f, 0x11, 0xd5, 0x08, 0x7a);
        public static readonly Guid MF_WORKQUEUE_SERVICES = new Guid(0x8e37d489, 0x41e0, 0x413a, 0x90, 0x68, 0x28, 0x7c, 0x88, 0x6d, 0x8d, 0xda);
        public static readonly Guid MFNET_SAVEJOB_SERVICE = new Guid(0xb85a587f, 0x3d02, 0x4e52, 0x95, 0x65, 0x55, 0xd3, 0xec, 0x1e, 0x7f, 0xf7);
        public static readonly Guid MFNETSOURCE_STATISTICS_SERVICE = new Guid(0x3cb1f275, 0x0505, 0x4c5d, 0xae, 0x71, 0x0a, 0x55, 0x63, 0x44, 0xef, 0xa1);
        public static readonly Guid MR_AUDIO_POLICY_SERVICE = new Guid(0x911fd737, 0x6775, 0x4ab0, 0xa6, 0x14, 0x29, 0x78, 0x62, 0xfd, 0xac, 0x88);
        public static readonly Guid MR_POLICY_VOLUME_SERVICE = new Guid(0x1abaa2ac, 0x9d3b, 0x47c6, 0xab, 0x48, 0xc5, 0x95, 0x6, 0xde, 0x78, 0x4d);
        public static readonly Guid MR_STREAM_VOLUME_SERVICE = new Guid(0xf8b5fa2f, 0x32ef, 0x46f5, 0xb1, 0x72, 0x13, 0x21, 0x21, 0x2f, 0xb2, 0xc4);
        public static readonly Guid MR_VIDEO_RENDER_SERVICE = new Guid(0x1092a86c, 0xab1a, 0x459a, 0xa3, 0x36, 0x83, 0x1f, 0xbc, 0x4d, 0x11, 0xff);
        public static readonly Guid MR_VIDEO_MIXER_SERVICE = new Guid(0x73cd2fc, 0x6cf4, 0x40b7, 0x88, 0x59, 0xe8, 0x95, 0x52, 0xc8, 0x41, 0xf8);
        public static readonly Guid MR_VIDEO_ACCELERATION_SERVICE = new Guid(0xefef5175, 0x5c7d, 0x4ce2, 0xbb, 0xbd, 0x34, 0xff, 0x8b, 0xca, 0x65, 0x54);
        public static readonly Guid MR_BUFFER_SERVICE = new Guid(0xa562248c, 0x9ac6, 0x4ffc, 0x9f, 0xba, 0x3a, 0xf8, 0xf8, 0xad, 0x1a, 0x4d);
        public static readonly Guid MF_PMP_SERVICE = new Guid(0x2F00C90C, 0xD2CF, 0x4278, 0x8B, 0x6A, 0xD0, 0x77, 0xFA, 0xC3, 0xA2, 0x5F);
        public static readonly Guid MF_LOCAL_MFT_REGISTRATION_SERVICE = new Guid(0xddf5cf9c, 0x4506, 0x45aa, 0xab, 0xf0, 0x6d, 0x5d, 0x94, 0xdd, 0x1b, 0x4a);
    }

    public static class MFAttributesClsid
    {
        // Audio Renderer Attributes
        public static readonly Guid MF_AUDIO_RENDERER_ATTRIBUTE_ENDPOINT_ID = new Guid(0xb10aaec3, 0xef71, 0x4cc3, 0xb8, 0x73, 0x5, 0xa9, 0xa0, 0x8b, 0x9f, 0x8e);
        public static readonly Guid MF_AUDIO_RENDERER_ATTRIBUTE_ENDPOINT_ROLE = new Guid(0x6ba644ff, 0x27c5, 0x4d02, 0x98, 0x87, 0xc2, 0x86, 0x19, 0xfd, 0xb9, 0x1b);
        public static readonly Guid MF_AUDIO_RENDERER_ATTRIBUTE_FLAGS = new Guid(0xede4b5e0, 0xf805, 0x4d6c, 0x99, 0xb3, 0xdb, 0x01, 0xbf, 0x95, 0xdf, 0xab);
        public static readonly Guid MF_AUDIO_RENDERER_ATTRIBUTE_SESSION_ID = new Guid(0xede4b5e3, 0xf805, 0x4d6c, 0x99, 0xb3, 0xdb, 0x01, 0xbf, 0x95, 0xdf, 0xab);

        // Byte Stream Attributes
        public static readonly Guid MF_BYTESTREAM_ORIGIN_NAME = new Guid(0xfc358288, 0x3cb6, 0x460c, 0xa4, 0x24, 0xb6, 0x68, 0x12, 0x60, 0x37, 0x5a);
        public static readonly Guid MF_BYTESTREAM_CONTENT_TYPE = new Guid(0xfc358289, 0x3cb6, 0x460c, 0xa4, 0x24, 0xb6, 0x68, 0x12, 0x60, 0x37, 0x5a);
        public static readonly Guid MF_BYTESTREAM_DURATION = new Guid(0xfc35828a, 0x3cb6, 0x460c, 0xa4, 0x24, 0xb6, 0x68, 0x12, 0x60, 0x37, 0x5a);
        public static readonly Guid MF_BYTESTREAM_LAST_MODIFIED_TIME = new Guid(0xfc35828b, 0x3cb6, 0x460c, 0xa4, 0x24, 0xb6, 0x68, 0x12, 0x60, 0x37, 0x5a);
        public static readonly Guid MF_BYTESTREAM_IFO_FILE_URI = new Guid(0xfc35828c, 0x3cb6, 0x460c, 0xa4, 0x24, 0xb6, 0x68, 0x12, 0x60, 0x37, 0x5a);
        public static readonly Guid MF_BYTESTREAM_DLNA_PROFILE_ID = new Guid(0xfc35828d, 0x3cb6, 0x460c, 0xa4, 0x24, 0xb6, 0x68, 0x12, 0x60, 0x37, 0x5a);

        // Enhanced Video Renderer Attributes
        public static readonly Guid MF_ACTIVATE_CUSTOM_VIDEO_MIXER_ACTIVATE = new Guid(0xba491361, 0xbe50, 0x451e, 0x95, 0xab, 0x6d, 0x4a, 0xcc, 0xc7, 0xda, 0xd8);
        public static readonly Guid MF_ACTIVATE_CUSTOM_VIDEO_MIXER_CLSID = new Guid(0xba491360, 0xbe50, 0x451e, 0x95, 0xab, 0x6d, 0x4a, 0xcc, 0xc7, 0xda, 0xd8);
        public static readonly Guid MF_ACTIVATE_CUSTOM_VIDEO_MIXER_FLAGS = new Guid(0xba491362, 0xbe50, 0x451e, 0x95, 0xab, 0x6d, 0x4a, 0xcc, 0xc7, 0xda, 0xd8);
        public static readonly Guid MF_ACTIVATE_CUSTOM_VIDEO_PRESENTER_ACTIVATE = new Guid(0xba491365, 0xbe50, 0x451e, 0x95, 0xab, 0x6d, 0x4a, 0xcc, 0xc7, 0xda, 0xd8);
        public static readonly Guid MF_ACTIVATE_CUSTOM_VIDEO_PRESENTER_CLSID = new Guid(0xba491364, 0xbe50, 0x451e, 0x95, 0xab, 0x6d, 0x4a, 0xcc, 0xc7, 0xda, 0xd8);
        public static readonly Guid MF_ACTIVATE_CUSTOM_VIDEO_PRESENTER_FLAGS = new Guid(0xba491366, 0xbe50, 0x451e, 0x95, 0xab, 0x6d, 0x4a, 0xcc, 0xc7, 0xda, 0xd8);
        public static readonly Guid MF_ACTIVATE_VIDEO_WINDOW = new Guid(0x9a2dbbdd, 0xf57e, 0x4162, 0x82, 0xb9, 0x68, 0x31, 0x37, 0x76, 0x82, 0xd3);
        public static readonly Guid MF_SA_REQUIRED_SAMPLE_COUNT = new Guid(0x18802c61, 0x324b, 0x4952, 0xab, 0xd0, 0x17, 0x6f, 0xf5, 0xc6, 0x96, 0xff);
        public static readonly Guid VIDEO_ZOOM_RECT = new Guid(0x7aaa1638, 0x1b7f, 0x4c93, 0xbd, 0x89, 0x5b, 0x9c, 0x9f, 0xb6, 0xfc, 0xf0);

        // Event Attributes

        // MF_EVENT_DO_THINNING {321EA6FB-DAD9-46e4-B31D-D2EAE7090E30}
        public static readonly Guid MF_EVENT_DO_THINNING = new Guid(0x321ea6fb, 0xdad9, 0x46e4, 0xb3, 0x1d, 0xd2, 0xea, 0xe7, 0x9, 0xe, 0x30);

        // MF_EVENT_OUTPUT_NODE {830f1a8b-c060-46dd-a801-1c95dec9b107}
        public static readonly Guid MF_EVENT_OUTPUT_NODE = new Guid(0x830f1a8b, 0xc060, 0x46dd, 0xa8, 0x01, 0x1c, 0x95, 0xde, 0xc9, 0xb1, 0x07);

        // MF_EVENT_MFT_INPUT_STREAM_ID {F29C2CCA-7AE6-42d2-B284-BF837CC874E2}
        public static readonly Guid MF_EVENT_MFT_INPUT_STREAM_ID = new Guid(0xf29c2cca, 0x7ae6, 0x42d2, 0xb2, 0x84, 0xbf, 0x83, 0x7c, 0xc8, 0x74, 0xe2);

        // MF_EVENT_MFT_CONTEXT {B7CD31F1-899E-4b41-80C9-26A896D32977}
        public static readonly Guid MF_EVENT_MFT_CONTEXT = new Guid(0xb7cd31f1, 0x899e, 0x4b41, 0x80, 0xc9, 0x26, 0xa8, 0x96, 0xd3, 0x29, 0x77);

        // MF_EVENT_PRESENTATION_TIME_OFFSET {5AD914D1-9B45-4a8d-A2C0-81D1E50BFB07}
        public static readonly Guid MF_EVENT_PRESENTATION_TIME_OFFSET = new Guid(0x5ad914d1, 0x9b45, 0x4a8d, 0xa2, 0xc0, 0x81, 0xd1, 0xe5, 0xb, 0xfb, 0x7);

        // MF_EVENT_SCRUBSAMPLE_TIME {9AC712B3-DCB8-44d5-8D0C-37455A2782E3}
        public static readonly Guid MF_EVENT_SCRUBSAMPLE_TIME = new Guid(0x9ac712b3, 0xdcb8, 0x44d5, 0x8d, 0xc, 0x37, 0x45, 0x5a, 0x27, 0x82, 0xe3);

        // MF_EVENT_SESSIONCAPS {7E5EBCD0-11B8-4abe-AFAD-10F6599A7F42}
        public static readonly Guid MF_EVENT_SESSIONCAPS = new Guid(0x7e5ebcd0, 0x11b8, 0x4abe, 0xaf, 0xad, 0x10, 0xf6, 0x59, 0x9a, 0x7f, 0x42);

        // MF_EVENT_SESSIONCAPS_DELTA {7E5EBCD1-11B8-4abe-AFAD-10F6599A7F42}
        // Type: UINT32
        public static readonly Guid MF_EVENT_SESSIONCAPS_DELTA = new Guid(0x7e5ebcd1, 0x11b8, 0x4abe, 0xaf, 0xad, 0x10, 0xf6, 0x59, 0x9a, 0x7f, 0x42);

        // MF_EVENT_SOURCE_ACTUAL_START {a8cc55a9-6b31-419f-845d-ffb351a2434b}
        public static readonly Guid MF_EVENT_SOURCE_ACTUAL_START = new Guid(0xa8cc55a9, 0x6b31, 0x419f, 0x84, 0x5d, 0xff, 0xb3, 0x51, 0xa2, 0x43, 0x4b);

        // MF_EVENT_SOURCE_CHARACTERISTICS {47DB8490-8B22-4f52-AFDA-9CE1B2D3CFA8}
        public static readonly Guid MF_EVENT_SOURCE_CHARACTERISTICS = new Guid(0x47db8490, 0x8b22, 0x4f52, 0xaf, 0xda, 0x9c, 0xe1, 0xb2, 0xd3, 0xcf, 0xa8);

        // MF_EVENT_SOURCE_CHARACTERISTICS_OLD {47DB8491-8B22-4f52-AFDA-9CE1B2D3CFA8}
        public static readonly Guid MF_EVENT_SOURCE_CHARACTERISTICS_OLD = new Guid(0x47db8491, 0x8b22, 0x4f52, 0xaf, 0xda, 0x9c, 0xe1, 0xb2, 0xd3, 0xcf, 0xa8);

        // MF_EVENT_SOURCE_FAKE_START {a8cc55a7-6b31-419f-845d-ffb351a2434b}
        public static readonly Guid MF_EVENT_SOURCE_FAKE_START = new Guid(0xa8cc55a7, 0x6b31, 0x419f, 0x84, 0x5d, 0xff, 0xb3, 0x51, 0xa2, 0x43, 0x4b);

        // MF_EVENT_SOURCE_PROJECTSTART {a8cc55a8-6b31-419f-845d-ffb351a2434b}
        public static readonly Guid MF_EVENT_SOURCE_PROJECTSTART = new Guid(0xa8cc55a8, 0x6b31, 0x419f, 0x84, 0x5d, 0xff, 0xb3, 0x51, 0xa2, 0x43, 0x4b);

        // MF_EVENT_SOURCE_TOPOLOGY_CANCELED {DB62F650-9A5E-4704-ACF3-563BC6A73364}
        public static readonly Guid MF_EVENT_SOURCE_TOPOLOGY_CANCELED = new Guid(0xdb62f650, 0x9a5e, 0x4704, 0xac, 0xf3, 0x56, 0x3b, 0xc6, 0xa7, 0x33, 0x64);

        // MF_EVENT_START_PRESENTATION_TIME {5AD914D0-9B45-4a8d-A2C0-81D1E50BFB07}
        public static readonly Guid MF_EVENT_START_PRESENTATION_TIME = new Guid(0x5ad914d0, 0x9b45, 0x4a8d, 0xa2, 0xc0, 0x81, 0xd1, 0xe5, 0xb, 0xfb, 0x7);

        // MF_EVENT_START_PRESENTATION_TIME_AT_OUTPUT {5AD914D2-9B45-4a8d-A2C0-81D1E50BFB07}
        public static readonly Guid MF_EVENT_START_PRESENTATION_TIME_AT_OUTPUT = new Guid(0x5ad914d2, 0x9b45, 0x4a8d, 0xa2, 0xc0, 0x81, 0xd1, 0xe5, 0xb, 0xfb, 0x7);

        // MF_EVENT_TOPOLOGY_STATUS {30C5018D-9A53-454b-AD9E-6D5F8FA7C43B}
        public static readonly Guid MF_EVENT_TOPOLOGY_STATUS = new Guid(0x30c5018d, 0x9a53, 0x454b, 0xad, 0x9e, 0x6d, 0x5f, 0x8f, 0xa7, 0xc4, 0x3b);

        public static readonly Guid MF_SESSION_APPROX_EVENT_OCCURRENCE_TIME = new Guid(0x190e852f, 0x6238, 0x42d1, 0xb5, 0xaf, 0x69, 0xea, 0x33, 0x8e, 0xf8, 0x50);

        // Media Session Attributes

        public static readonly Guid MF_SESSION_CONTENT_PROTECTION_MANAGER = new Guid(0x1e83d482, 0x1f1c, 0x4571, 0x84, 0x5, 0x88, 0xf4, 0xb2, 0x18, 0x1f, 0x74);
        public static readonly Guid MF_SESSION_GLOBAL_TIME = new Guid(0x1e83d482, 0x1f1c, 0x4571, 0x84, 0x5, 0x88, 0xf4, 0xb2, 0x18, 0x1f, 0x72);
        public static readonly Guid MF_SESSION_QUALITY_MANAGER = new Guid(0x1e83d482, 0x1f1c, 0x4571, 0x84, 0x5, 0x88, 0xf4, 0xb2, 0x18, 0x1f, 0x73);
        public static readonly Guid MF_SESSION_REMOTE_SOURCE_MODE = new Guid(0xf4033ef4, 0x9bb3, 0x4378, 0x94, 0x1f, 0x85, 0xa0, 0x85, 0x6b, 0xc2, 0x44);
        public static readonly Guid MF_SESSION_SERVER_CONTEXT = new Guid(0xafe5b291, 0x50fa, 0x46e8, 0xb9, 0xbe, 0xc, 0xc, 0x3c, 0xe4, 0xb3, 0xa5);
        public static readonly Guid MF_SESSION_TOPOLOADER = new Guid(0x1e83d482, 0x1f1c, 0x4571, 0x84, 0x5, 0x88, 0xf4, 0xb2, 0x18, 0x1f, 0x71);

        // Media Type Attributes

        // {48eba18e-f8c9-4687-bf11-0a74c9f96a8f}   MF_MT_MAJOR_TYPE                {GUID}
        public static readonly Guid MF_MT_MAJOR_TYPE = new Guid(0x48eba18e, 0xf8c9, 0x4687, 0xbf, 0x11, 0x0a, 0x74, 0xc9, 0xf9, 0x6a, 0x8f);

        // {f7e34c9a-42e8-4714-b74b-cb29d72c35e5}   MF_MT_SUBTYPE                   {GUID}
        public static readonly Guid MF_MT_SUBTYPE = new Guid(0xf7e34c9a, 0x42e8, 0x4714, 0xb7, 0x4b, 0xcb, 0x29, 0xd7, 0x2c, 0x35, 0xe5);

        // {c9173739-5e56-461c-b713-46fb995cb95f}   MF_MT_ALL_SAMPLES_INDEPENDENT   {UINT32 (BOOL)}
        public static readonly Guid MF_MT_ALL_SAMPLES_INDEPENDENT = new Guid(0xc9173739, 0x5e56, 0x461c, 0xb7, 0x13, 0x46, 0xfb, 0x99, 0x5c, 0xb9, 0x5f);

        // {b8ebefaf-b718-4e04-b0a9-116775e3321b}   MF_MT_FIXED_SIZE_SAMPLES        {UINT32 (BOOL)}
        public static readonly Guid MF_MT_FIXED_SIZE_SAMPLES = new Guid(0xb8ebefaf, 0xb718, 0x4e04, 0xb0, 0xa9, 0x11, 0x67, 0x75, 0xe3, 0x32, 0x1b);

        // {3afd0cee-18f2-4ba5-a110-8bea502e1f92}   MF_MT_COMPRESSED                {UINT32 (BOOL)}
        public static readonly Guid MF_MT_COMPRESSED = new Guid(0x3afd0cee, 0x18f2, 0x4ba5, 0xa1, 0x10, 0x8b, 0xea, 0x50, 0x2e, 0x1f, 0x92);

        // {dad3ab78-1990-408b-bce2-eba673dacc10}   MF_MT_SAMPLE_SIZE               {UINT32}
        public static readonly Guid MF_MT_SAMPLE_SIZE = new Guid(0xdad3ab78, 0x1990, 0x408b, 0xbc, 0xe2, 0xeb, 0xa6, 0x73, 0xda, 0xcc, 0x10);

        // 4d3f7b23-d02f-4e6c-9bee-e4bf2c6c695d     MF_MT_WRAPPED_TYPE              {Blob}
        public static readonly Guid MF_MT_WRAPPED_TYPE = new Guid(0x4d3f7b23, 0xd02f, 0x4e6c, 0x9b, 0xee, 0xe4, 0xbf, 0x2c, 0x6c, 0x69, 0x5d);

        // {37e48bf5-645e-4c5b-89de-ada9e29b696a}   MF_MT_AUDIO_NUM_CHANNELS            {UINT32}
        public static readonly Guid MF_MT_AUDIO_NUM_CHANNELS = new Guid(0x37e48bf5, 0x645e, 0x4c5b, 0x89, 0xde, 0xad, 0xa9, 0xe2, 0x9b, 0x69, 0x6a);

        // {5faeeae7-0290-4c31-9e8a-c534f68d9dba}   MF_MT_AUDIO_SAMPLES_PER_SECOND      {UINT32}
        public static readonly Guid MF_MT_AUDIO_SAMPLES_PER_SECOND = new Guid(0x5faeeae7, 0x0290, 0x4c31, 0x9e, 0x8a, 0xc5, 0x34, 0xf6, 0x8d, 0x9d, 0xba);

        // {fb3b724a-cfb5-4319-aefe-6e42b2406132}   MF_MT_AUDIO_FLOAT_SAMPLES_PER_SECOND {double}
        public static readonly Guid MF_MT_AUDIO_FLOAT_SAMPLES_PER_SECOND = new Guid(0xfb3b724a, 0xcfb5, 0x4319, 0xae, 0xfe, 0x6e, 0x42, 0xb2, 0x40, 0x61, 0x32);

        // {1aab75c8-cfef-451c-ab95-ac034b8e1731}   MF_MT_AUDIO_AVG_BYTES_PER_SECOND    {UINT32}
        public static readonly Guid MF_MT_AUDIO_AVG_BYTES_PER_SECOND = new Guid(0x1aab75c8, 0xcfef, 0x451c, 0xab, 0x95, 0xac, 0x03, 0x4b, 0x8e, 0x17, 0x31);

        // {322de230-9eeb-43bd-ab7a-ff412251541d}   MF_MT_AUDIO_BLOCK_ALIGNMENT         {UINT32}
        public static readonly Guid MF_MT_AUDIO_BLOCK_ALIGNMENT = new Guid(0x322de230, 0x9eeb, 0x43bd, 0xab, 0x7a, 0xff, 0x41, 0x22, 0x51, 0x54, 0x1d);

        // {f2deb57f-40fa-4764-aa33-ed4f2d1ff669}   MF_MT_AUDIO_BITS_PER_SAMPLE         {UINT32}
        public static readonly Guid MF_MT_AUDIO_BITS_PER_SAMPLE = new Guid(0xf2deb57f, 0x40fa, 0x4764, 0xaa, 0x33, 0xed, 0x4f, 0x2d, 0x1f, 0xf6, 0x69);

        // {d9bf8d6a-9530-4b7c-9ddf-ff6fd58bbd06}   MF_MT_AUDIO_VALID_BITS_PER_SAMPLE   {UINT32}
        public static readonly Guid MF_MT_AUDIO_VALID_BITS_PER_SAMPLE = new Guid(0xd9bf8d6a, 0x9530, 0x4b7c, 0x9d, 0xdf, 0xff, 0x6f, 0xd5, 0x8b, 0xbd, 0x06);

        // {aab15aac-e13a-4995-9222-501ea15c6877}   MF_MT_AUDIO_SAMPLES_PER_BLOCK       {UINT32}
        public static readonly Guid MF_MT_AUDIO_SAMPLES_PER_BLOCK = new Guid(0xaab15aac, 0xe13a, 0x4995, 0x92, 0x22, 0x50, 0x1e, 0xa1, 0x5c, 0x68, 0x77);

        // {55fb5765-644a-4caf-8479-938983bb1588}`  MF_MT_AUDIO_CHANNEL_MASK            {UINT32}
        public static readonly Guid MF_MT_AUDIO_CHANNEL_MASK = new Guid(0x55fb5765, 0x644a, 0x4caf, 0x84, 0x79, 0x93, 0x89, 0x83, 0xbb, 0x15, 0x88);

        // {9d62927c-36be-4cf2-b5c4-a3926e3e8711}`  MF_MT_AUDIO_FOLDDOWN_MATRIX         {BLOB, MFFOLDDOWN_MATRIX}
        public static readonly Guid MF_MT_AUDIO_FOLDDOWN_MATRIX = new Guid(0x9d62927c, 0x36be, 0x4cf2, 0xb5, 0xc4, 0xa3, 0x92, 0x6e, 0x3e, 0x87, 0x11);

        // {0x9d62927d-36be-4cf2-b5c4-a3926e3e8711}`  MF_MT_AUDIO_WMADRC_PEAKREF         {UINT32}
        public static readonly Guid MF_MT_AUDIO_WMADRC_PEAKREF = new Guid(0x9d62927d, 0x36be, 0x4cf2, 0xb5, 0xc4, 0xa3, 0x92, 0x6e, 0x3e, 0x87, 0x11);

        // {0x9d62927e-36be-4cf2-b5c4-a3926e3e8711}`  MF_MT_AUDIO_WMADRC_PEAKTARGET        {UINT32}
        public static readonly Guid MF_MT_AUDIO_WMADRC_PEAKTARGET = new Guid(0x9d62927e, 0x36be, 0x4cf2, 0xb5, 0xc4, 0xa3, 0x92, 0x6e, 0x3e, 0x87, 0x11);

        // {0x9d62927f-36be-4cf2-b5c4-a3926e3e8711}`  MF_MT_AUDIO_WMADRC_AVGREF         {UINT32}
        public static readonly Guid MF_MT_AUDIO_WMADRC_AVGREF = new Guid(0x9d62927f, 0x36be, 0x4cf2, 0xb5, 0xc4, 0xa3, 0x92, 0x6e, 0x3e, 0x87, 0x11);

        // {0x9d629280-36be-4cf2-b5c4-a3926e3e8711}`  MF_MT_AUDIO_WMADRC_AVGTARGET      {UINT32}
        public static readonly Guid MF_MT_AUDIO_WMADRC_AVGTARGET = new Guid(0x9d629280, 0x36be, 0x4cf2, 0xb5, 0xc4, 0xa3, 0x92, 0x6e, 0x3e, 0x87, 0x11);

        // {a901aaba-e037-458a-bdf6-545be2074042}   MF_MT_AUDIO_PREFER_WAVEFORMATEX     {UINT32 (BOOL)}
        public static readonly Guid MF_MT_AUDIO_PREFER_WAVEFORMATEX = new Guid(0xa901aaba, 0xe037, 0x458a, 0xbd, 0xf6, 0x54, 0x5b, 0xe2, 0x07, 0x40, 0x42);

        // {BFBABE79-7434-4d1c-94F0-72A3B9E17188} MF_MT_AAC_PAYLOAD_TYPE       {UINT32}
        public static readonly Guid MF_MT_AAC_PAYLOAD_TYPE = new Guid(0xbfbabe79, 0x7434, 0x4d1c, 0x94, 0xf0, 0x72, 0xa3, 0xb9, 0xe1, 0x71, 0x88);

        // {7632F0E6-9538-4d61-ACDA-EA29C8C14456} MF_MT_AAC_AUDIO_PROFILE_LEVEL_INDICATION       {UINT32}
        public static readonly Guid MF_MT_AAC_AUDIO_PROFILE_LEVEL_INDICATION = new Guid(0x7632f0e6, 0x9538, 0x4d61, 0xac, 0xda, 0xea, 0x29, 0xc8, 0xc1, 0x44, 0x56);

        // {1652c33d-d6b2-4012-b834-72030849a37d}   MF_MT_FRAME_SIZE                {UINT64 (HI32(Width),LO32(Height))}
        public static readonly Guid MF_MT_FRAME_SIZE = new Guid(0x1652c33d, 0xd6b2, 0x4012, 0xb8, 0x34, 0x72, 0x03, 0x08, 0x49, 0xa3, 0x7d);

        // {c459a2e8-3d2c-4e44-b132-fee5156c7bb0}   MF_MT_FRAME_RATE                {UINT64 (HI32(Numerator),LO32(Denominator))}
        public static readonly Guid MF_MT_FRAME_RATE = new Guid(0xc459a2e8, 0x3d2c, 0x4e44, 0xb1, 0x32, 0xfe, 0xe5, 0x15, 0x6c, 0x7b, 0xb0);

        // {c6376a1e-8d0a-4027-be45-6d9a0ad39bb6}   MF_MT_PIXEL_ASPECT_RATIO        {UINT64 (HI32(Numerator),LO32(Denominator))}
        public static readonly Guid MF_MT_PIXEL_ASPECT_RATIO = new Guid(0xc6376a1e, 0x8d0a, 0x4027, 0xbe, 0x45, 0x6d, 0x9a, 0x0a, 0xd3, 0x9b, 0xb6);

        // {8772f323-355a-4cc7-bb78-6d61a048ae82}   MF_MT_DRM_FLAGS                 {UINT32 (anyof MFVideoDRMFlags)}
        public static readonly Guid MF_MT_DRM_FLAGS = new Guid(0x8772f323, 0x355a, 0x4cc7, 0xbb, 0x78, 0x6d, 0x61, 0xa0, 0x48, 0xae, 0x82);

        // {4d0e73e5-80ea-4354-a9d0-1176ceb028ea}   MF_MT_PAD_CONTROL_FLAGS         {UINT32 (oneof MFVideoPadFlags)}
        public static readonly Guid MF_MT_PAD_CONTROL_FLAGS = new Guid(0x4d0e73e5, 0x80ea, 0x4354, 0xa9, 0xd0, 0x11, 0x76, 0xce, 0xb0, 0x28, 0xea);

        // {68aca3cc-22d0-44e6-85f8-28167197fa38}   MF_MT_SOURCE_CONTENT_HINT       {UINT32 (oneof MFVideoSrcContentHintFlags)}
        public static readonly Guid MF_MT_SOURCE_CONTENT_HINT = new Guid(0x68aca3cc, 0x22d0, 0x44e6, 0x85, 0xf8, 0x28, 0x16, 0x71, 0x97, 0xfa, 0x38);

        // {65df2370-c773-4c33-aa64-843e068efb0c}   MF_MT_CHROMA_SITING             {UINT32 (anyof MFVideoChromaSubsampling)}
        public static readonly Guid MF_MT_VIDEO_CHROMA_SITING = new Guid(0x65df2370, 0xc773, 0x4c33, 0xaa, 0x64, 0x84, 0x3e, 0x06, 0x8e, 0xfb, 0x0c);

        // {e2724bb8-e676-4806-b4b2-a8d6efb44ccd}   MF_MT_INTERLACE_MODE            {UINT32 (oneof MFVideoInterlaceMode)}
        public static readonly Guid MF_MT_INTERLACE_MODE = new Guid(0xe2724bb8, 0xe676, 0x4806, 0xb4, 0xb2, 0xa8, 0xd6, 0xef, 0xb4, 0x4c, 0xcd);

        // {5fb0fce9-be5c-4935-a811-ec838f8eed93}   MF_MT_TRANSFER_FUNCTION         {UINT32 (oneof MFVideoTransferFunction)}
        public static readonly Guid MF_MT_TRANSFER_FUNCTION = new Guid(0x5fb0fce9, 0xbe5c, 0x4935, 0xa8, 0x11, 0xec, 0x83, 0x8f, 0x8e, 0xed, 0x93);

        // {dbfbe4d7-0740-4ee0-8192-850ab0e21935}   MF_MT_VIDEO_PRIMARIES           {UINT32 (oneof MFVideoPrimaries)}
        public static readonly Guid MF_MT_VIDEO_PRIMARIES = new Guid(0xdbfbe4d7, 0x0740, 0x4ee0, 0x81, 0x92, 0x85, 0x0a, 0xb0, 0xe2, 0x19, 0x35);

        // {47537213-8cfb-4722-aa34-fbc9e24d77b8}   MF_MT_CUSTOM_VIDEO_PRIMARIES    {BLOB (MT_CUSTOM_VIDEO_PRIMARIES)}
        public static readonly Guid MF_MT_CUSTOM_VIDEO_PRIMARIES = new Guid(0x47537213, 0x8cfb, 0x4722, 0xaa, 0x34, 0xfb, 0xc9, 0xe2, 0x4d, 0x77, 0xb8);

        // {3e23d450-2c75-4d25-a00e-b91670d12327}   MF_MT_YUV_MATRIX                {UINT32 (oneof MFVideoTransferMatrix)}
        public static readonly Guid MF_MT_YUV_MATRIX = new Guid(0x3e23d450, 0x2c75, 0x4d25, 0xa0, 0x0e, 0xb9, 0x16, 0x70, 0xd1, 0x23, 0x27);

        // {53a0529c-890b-4216-8bf9-599367ad6d20}   MF_MT_VIDEO_LIGHTING            {UINT32 (oneof MFVideoLighting)}
        public static readonly Guid MF_MT_VIDEO_LIGHTING = new Guid(0x53a0529c, 0x890b, 0x4216, 0x8b, 0xf9, 0x59, 0x93, 0x67, 0xad, 0x6d, 0x20);

        // {c21b8ee5-b956-4071-8daf-325edf5cab11}   MF_MT_VIDEO_NOMINAL_RANGE       {UINT32 (oneof MFNominalRange)}
        public static readonly Guid MF_MT_VIDEO_NOMINAL_RANGE = new Guid(0xc21b8ee5, 0xb956, 0x4071, 0x8d, 0xaf, 0x32, 0x5e, 0xdf, 0x5c, 0xab, 0x11);

        // {66758743-7e5f-400d-980a-aa8596c85696}   MF_MT_GEOMETRIC_APERTURE        {BLOB (MFVideoArea)}
        public static readonly Guid MF_MT_GEOMETRIC_APERTURE = new Guid(0x66758743, 0x7e5f, 0x400d, 0x98, 0x0a, 0xaa, 0x85, 0x96, 0xc8, 0x56, 0x96);

        // {d7388766-18fe-48c6-a177-ee894867c8c4}   MF_MT_MINIMUM_DISPLAY_APERTURE  {BLOB (MFVideoArea)}
        public static readonly Guid MF_MT_MINIMUM_DISPLAY_APERTURE = new Guid(0xd7388766, 0x18fe, 0x48c6, 0xa1, 0x77, 0xee, 0x89, 0x48, 0x67, 0xc8, 0xc4);

        // {79614dde-9187-48fb-b8c7-4d52689de649}   MF_MT_PAN_SCAN_APERTURE         {BLOB (MFVideoArea)}
        public static readonly Guid MF_MT_PAN_SCAN_APERTURE = new Guid(0x79614dde, 0x9187, 0x48fb, 0xb8, 0xc7, 0x4d, 0x52, 0x68, 0x9d, 0xe6, 0x49);

        // {4b7f6bc3-8b13-40b2-a993-abf630b8204e}   MF_MT_PAN_SCAN_ENABLED          {UINT32 (BOOL)}
        public static readonly Guid MF_MT_PAN_SCAN_ENABLED = new Guid(0x4b7f6bc3, 0x8b13, 0x40b2, 0xa9, 0x93, 0xab, 0xf6, 0x30, 0xb8, 0x20, 0x4e);

        // {20332624-fb0d-4d9e-bd0d-cbf6786c102e}   MF_MT_AVG_BITRATE               {UINT32}
        public static readonly Guid MF_MT_AVG_BITRATE = new Guid(0x20332624, 0xfb0d, 0x4d9e, 0xbd, 0x0d, 0xcb, 0xf6, 0x78, 0x6c, 0x10, 0x2e);

        // {799cabd6-3508-4db4-a3c7-569cd533deb1}   MF_MT_AVG_BIT_ERROR_RATE        {UINT32}
        public static readonly Guid MF_MT_AVG_BIT_ERROR_RATE = new Guid(0x799cabd6, 0x3508, 0x4db4, 0xa3, 0xc7, 0x56, 0x9c, 0xd5, 0x33, 0xde, 0xb1);

        // {c16eb52b-73a1-476f-8d62-839d6a020652}   MF_MT_MAX_KEYFRAME_SPACING      {UINT32}
        public static readonly Guid MF_MT_MAX_KEYFRAME_SPACING = new Guid(0xc16eb52b, 0x73a1, 0x476f, 0x8d, 0x62, 0x83, 0x9d, 0x6a, 0x02, 0x06, 0x52);

        // {644b4e48-1e02-4516-b0eb-c01ca9d49ac6}   MF_MT_DEFAULT_STRIDE            {UINT32 (INT32)} // in bytes
        public static readonly Guid MF_MT_DEFAULT_STRIDE = new Guid(0x644b4e48, 0x1e02, 0x4516, 0xb0, 0xeb, 0xc0, 0x1c, 0xa9, 0xd4, 0x9a, 0xc6);

        // {6d283f42-9846-4410-afd9-654d503b1a54}   MF_MT_PALETTE                   {BLOB (array of MFPaletteEntry - usually 256)}
        public static readonly Guid MF_MT_PALETTE = new Guid(0x6d283f42, 0x9846, 0x4410, 0xaf, 0xd9, 0x65, 0x4d, 0x50, 0x3b, 0x1a, 0x54);

        // {b6bc765f-4c3b-40a4-bd51-2535b66fe09d}   MF_MT_USER_DATA                 {BLOB}
        public static readonly Guid MF_MT_USER_DATA = new Guid(0xb6bc765f, 0x4c3b, 0x40a4, 0xbd, 0x51, 0x25, 0x35, 0xb6, 0x6f, 0xe0, 0x9d);

        // {73d1072d-1870-4174-a063-29ff4ff6c11e}
        public static readonly Guid MF_MT_AM_FORMAT_TYPE = new Guid(0x73d1072d, 0x1870, 0x4174, 0xa0, 0x63, 0x29, 0xff, 0x4f, 0xf6, 0xc1, 0x1e);

        // {91f67885-4333-4280-97cd-bd5a6c03a06e}   MF_MT_MPEG_START_TIME_CODE      {UINT32}
        public static readonly Guid MF_MT_MPEG_START_TIME_CODE = new Guid(0x91f67885, 0x4333, 0x4280, 0x97, 0xcd, 0xbd, 0x5a, 0x6c, 0x03, 0xa0, 0x6e);

        // {ad76a80b-2d5c-4e0b-b375-64e520137036}   MF_MT_MPEG2_PROFILE             {UINT32 (oneof AM_MPEG2Profile)}
        public static readonly Guid MF_MT_MPEG2_PROFILE = new Guid(0xad76a80b, 0x2d5c, 0x4e0b, 0xb3, 0x75, 0x64, 0xe5, 0x20, 0x13, 0x70, 0x36);

        // {96f66574-11c5-4015-8666-bff516436da7}   MF_MT_MPEG2_LEVEL               {UINT32 (oneof AM_MPEG2Level)}
        public static readonly Guid MF_MT_MPEG2_LEVEL = new Guid(0x96f66574, 0x11c5, 0x4015, 0x86, 0x66, 0xbf, 0xf5, 0x16, 0x43, 0x6d, 0xa7);

        // {31e3991d-f701-4b2f-b426-8ae3bda9e04b}   MF_MT_MPEG2_FLAGS               {UINT32 (anyof AMMPEG2_xxx flags)}
        public static readonly Guid MF_MT_MPEG2_FLAGS = new Guid(0x31e3991d, 0xf701, 0x4b2f, 0xb4, 0x26, 0x8a, 0xe3, 0xbd, 0xa9, 0xe0, 0x4b);

        // {3c036de7-3ad0-4c9e-9216-ee6d6ac21cb3}   MF_MT_MPEG_SEQUENCE_HEADER      {BLOB}
        public static readonly Guid MF_MT_MPEG_SEQUENCE_HEADER = new Guid(0x3c036de7, 0x3ad0, 0x4c9e, 0x92, 0x16, 0xee, 0x6d, 0x6a, 0xc2, 0x1c, 0xb3);

        // {84bd5d88-0fb8-4ac8-be4b-a8848bef98f3}   MF_MT_DV_AAUX_SRC_PACK_0        {UINT32}
        public static readonly Guid MF_MT_DV_AAUX_SRC_PACK_0 = new Guid(0x84bd5d88, 0x0fb8, 0x4ac8, 0xbe, 0x4b, 0xa8, 0x84, 0x8b, 0xef, 0x98, 0xf3);

        // {f731004e-1dd1-4515-aabe-f0c06aa536ac}   MF_MT_DV_AAUX_CTRL_PACK_0       {UINT32}
        public static readonly Guid MF_MT_DV_AAUX_CTRL_PACK_0 = new Guid(0xf731004e, 0x1dd1, 0x4515, 0xaa, 0xbe, 0xf0, 0xc0, 0x6a, 0xa5, 0x36, 0xac);

        // {720e6544-0225-4003-a651-0196563a958e}   MF_MT_DV_AAUX_SRC_PACK_1        {UINT32}
        public static readonly Guid MF_MT_DV_AAUX_SRC_PACK_1 = new Guid(0x720e6544, 0x0225, 0x4003, 0xa6, 0x51, 0x01, 0x96, 0x56, 0x3a, 0x95, 0x8e);

        // {cd1f470d-1f04-4fe0-bfb9-d07ae0386ad8}   MF_MT_DV_AAUX_CTRL_PACK_1       {UINT32}
        public static readonly Guid MF_MT_DV_AAUX_CTRL_PACK_1 = new Guid(0xcd1f470d, 0x1f04, 0x4fe0, 0xbf, 0xb9, 0xd0, 0x7a, 0xe0, 0x38, 0x6a, 0xd8);

        // {41402d9d-7b57-43c6-b129-2cb997f15009}   MF_MT_DV_VAUX_SRC_PACK          {UINT32}
        public static readonly Guid MF_MT_DV_VAUX_SRC_PACK = new Guid(0x41402d9d, 0x7b57, 0x43c6, 0xb1, 0x29, 0x2c, 0xb9, 0x97, 0xf1, 0x50, 0x09);

        // {2f84e1c4-0da1-4788-938e-0dfbfbb34b48}   MF_MT_DV_VAUX_CTRL_PACK         {UINT32}
        public static readonly Guid MF_MT_DV_VAUX_CTRL_PACK = new Guid(0x2f84e1c4, 0x0da1, 0x4788, 0x93, 0x8e, 0x0d, 0xfb, 0xfb, 0xb3, 0x4b, 0x48);

        // Sample Attributes

        // {941ce0a3-6ae3-4dda-9a08-a64298340617}   MFSampleExtension_BottomFieldFirst
        public static readonly Guid MFSampleExtension_BottomFieldFirst = new Guid(0x941ce0a3, 0x6ae3, 0x4dda, 0x9a, 0x08, 0xa6, 0x42, 0x98, 0x34, 0x06, 0x17);

        // MFSampleExtension_CleanPoint {9cdf01d8-a0f0-43ba-b077-eaa06cbd728a}
        public static readonly Guid MFSampleExtension_CleanPoint = new Guid(0x9cdf01d8, 0xa0f0, 0x43ba, 0xb0, 0x77, 0xea, 0xa0, 0x6c, 0xbd, 0x72, 0x8a);

        // {6852465a-ae1c-4553-8e9b-c3420fcb1637}   MFSampleExtension_DerivedFromTopField
        public static readonly Guid MFSampleExtension_DerivedFromTopField = new Guid(0x6852465a, 0xae1c, 0x4553, 0x8e, 0x9b, 0xc3, 0x42, 0x0f, 0xcb, 0x16, 0x37);

        // MFSampleExtension_Discontinuity {9cdf01d9-a0f0-43ba-b077-eaa06cbd728a}
        public static readonly Guid MFSampleExtension_Discontinuity = new Guid(0x9cdf01d9, 0xa0f0, 0x43ba, 0xb0, 0x77, 0xea, 0xa0, 0x6c, 0xbd, 0x72, 0x8a);

        // {b1d5830a-deb8-40e3-90fa-389943716461}   MFSampleExtension_Interlaced
        public static readonly Guid MFSampleExtension_Interlaced = new Guid(0xb1d5830a, 0xdeb8, 0x40e3, 0x90, 0xfa, 0x38, 0x99, 0x43, 0x71, 0x64, 0x61);

        // {304d257c-7493-4fbd-b149-9228de8d9a99}   MFSampleExtension_RepeatFirstField
        public static readonly Guid MFSampleExtension_RepeatFirstField = new Guid(0x304d257c, 0x7493, 0x4fbd, 0xb1, 0x49, 0x92, 0x28, 0xde, 0x8d, 0x9a, 0x99);

        // {9d85f816-658b-455a-bde0-9fa7e15ab8f9}   MFSampleExtension_SingleField
        public static readonly Guid MFSampleExtension_SingleField = new Guid(0x9d85f816, 0x658b, 0x455a, 0xbd, 0xe0, 0x9f, 0xa7, 0xe1, 0x5a, 0xb8, 0xf9);

        // MFSampleExtension_Token {8294da66-f328-4805-b551-00deb4c57a61}
        public static readonly Guid MFSampleExtension_Token = new Guid(0x8294da66, 0xf328, 0x4805, 0xb5, 0x51, 0x00, 0xde, 0xb4, 0xc5, 0x7a, 0x61);

        // Sample Grabber Sink Attributes
        public static readonly Guid MF_SAMPLEGRABBERSINK_SAMPLE_TIME_OFFSET = new Guid(0x62e3d776, 0x8100, 0x4e03, 0xa6, 0xe8, 0xbd, 0x38, 0x57, 0xac, 0x9c, 0x47);

        // Stream descriptor Attributes

        public static readonly Guid MF_SD_LANGUAGE = new Guid(0xaf2180, 0xbdc2, 0x423c, 0xab, 0xca, 0xf5, 0x3, 0x59, 0x3b, 0xc1, 0x21);
        public static readonly Guid MF_SD_PROTECTED = new Guid(0xaf2181, 0xbdc2, 0x423c, 0xab, 0xca, 0xf5, 0x3, 0x59, 0x3b, 0xc1, 0x21);
        public static readonly Guid MF_SD_SAMI_LANGUAGE = new Guid(0x36fcb98a, 0x6cd0, 0x44cb, 0xac, 0xb9, 0xa8, 0xf5, 0x60, 0xd, 0xd0, 0xbb);

        // Topology Attributes
        public static readonly Guid MF_TOPOLOGY_NO_MARKIN_MARKOUT = new Guid(0x7ed3f804, 0x86bb, 0x4b3f, 0xb7, 0xe4, 0x7c, 0xb4, 0x3a, 0xfd, 0x4b, 0x80);
        public static readonly Guid MF_TOPOLOGY_PROJECTSTART = new Guid(0x7ed3f802, 0x86bb, 0x4b3f, 0xb7, 0xe4, 0x7c, 0xb4, 0x3a, 0xfd, 0x4b, 0x80);
        public static readonly Guid MF_TOPOLOGY_PROJECTSTOP = new Guid(0x7ed3f803, 0x86bb, 0x4b3f, 0xb7, 0xe4, 0x7c, 0xb4, 0x3a, 0xfd, 0x4b, 0x80);
        public static readonly Guid MF_TOPOLOGY_RESOLUTION_STATUS = new Guid(0x494bbcde, 0xb031, 0x4e38, 0x97, 0xc4, 0xd5, 0x42, 0x2d, 0xd6, 0x18, 0xdc);

        // Topology Node Attributes
        public static readonly Guid MF_TOPONODE_CONNECT_METHOD = new Guid(0x494bbcf1, 0xb031, 0x4e38, 0x97, 0xc4, 0xd5, 0x42, 0x2d, 0xd6, 0x18, 0xdc);
        public static readonly Guid MF_TOPONODE_D3DAWARE = new Guid(0x494bbced, 0xb031, 0x4e38, 0x97, 0xc4, 0xd5, 0x42, 0x2d, 0xd6, 0x18, 0xdc);
        public static readonly Guid MF_TOPONODE_DECODER = new Guid(0x494bbd02, 0xb031, 0x4e38, 0x97, 0xc4, 0xd5, 0x42, 0x2d, 0xd6, 0x18, 0xdc);
        public static readonly Guid MF_TOPONODE_DECRYPTOR = new Guid(0x494bbcfa, 0xb031, 0x4e38, 0x97, 0xc4, 0xd5, 0x42, 0x2d, 0xd6, 0x18, 0xdc);
        public static readonly Guid MF_TOPONODE_DISABLE_PREROLL = new Guid(0x14932f9e, 0x9087, 0x4bb4, 0x84, 0x12, 0x51, 0x67, 0x14, 0x5c, 0xbe, 0x04);
        public static readonly Guid MF_TOPONODE_DISCARDABLE = new Guid(0x494bbcfb, 0xb031, 0x4e38, 0x97, 0xc4, 0xd5, 0x42, 0x2d, 0xd6, 0x18, 0xdc);
        public static readonly Guid MF_TOPONODE_DRAIN = new Guid(0x494bbce9, 0xb031, 0x4e38, 0x97, 0xc4, 0xd5, 0x42, 0x2d, 0xd6, 0x18, 0xdc);
        public static readonly Guid MF_TOPONODE_ERROR_MAJORTYPE = new Guid(0x494bbcfd, 0xb031, 0x4e38, 0x97, 0xc4, 0xd5, 0x42, 0x2d, 0xd6, 0x18, 0xdc);
        public static readonly Guid MF_TOPONODE_ERROR_SUBTYPE = new Guid(0x494bbcfe, 0xb031, 0x4e38, 0x97, 0xc4, 0xd5, 0x42, 0x2d, 0xd6, 0x18, 0xdc);
        public static readonly Guid MF_TOPONODE_ERRORCODE = new Guid(0x494bbcee, 0xb031, 0x4e38, 0x97, 0xc4, 0xd5, 0x42, 0x2d, 0xd6, 0x18, 0xdc);
        public static readonly Guid MF_TOPONODE_FLUSH = new Guid(0x494bbce8, 0xb031, 0x4e38, 0x97, 0xc4, 0xd5, 0x42, 0x2d, 0xd6, 0x18, 0xdc);
        public static readonly Guid MF_TOPONODE_LOCKED = new Guid(0x494bbcf7, 0xb031, 0x4e38, 0x97, 0xc4, 0xd5, 0x42, 0x2d, 0xd6, 0x18, 0xdc);
        public static readonly Guid MF_TOPONODE_MARKIN_HERE = new Guid(0x494bbd00, 0xb031, 0x4e38, 0x97, 0xc4, 0xd5, 0x42, 0x2d, 0xd6, 0x18, 0xdc);
        public static readonly Guid MF_TOPONODE_MARKOUT_HERE = new Guid(0x494bbd01, 0xb031, 0x4e38, 0x97, 0xc4, 0xd5, 0x42, 0x2d, 0xd6, 0x18, 0xdc);
        public static readonly Guid MF_TOPONODE_MEDIASTART = new Guid(0x835c58ea, 0xe075, 0x4bc7, 0xbc, 0xba, 0x4d, 0xe0, 0x00, 0xdf, 0x9a, 0xe6);
        public static readonly Guid MF_TOPONODE_MEDIASTOP = new Guid(0x835c58eb, 0xe075, 0x4bc7, 0xbc, 0xba, 0x4d, 0xe0, 0x00, 0xdf, 0x9a, 0xe6);
        public static readonly Guid MF_TOPONODE_NOSHUTDOWN_ON_REMOVE = new Guid(0x14932f9c, 0x9087, 0x4bb4, 0x84, 0x12, 0x51, 0x67, 0x14, 0x5c, 0xbe, 0x04);
        public static readonly Guid MF_TOPONODE_PRESENTATION_DESCRIPTOR = new Guid(0x835c58ed, 0xe075, 0x4bc7, 0xbc, 0xba, 0x4d, 0xe0, 0x00, 0xdf, 0x9a, 0xe6);
        public static readonly Guid MF_TOPONODE_PRIMARYOUTPUT = new Guid(0x6304ef99, 0x16b2, 0x4ebe, 0x9d, 0x67, 0xe4, 0xc5, 0x39, 0xb3, 0xa2, 0x59);
        public static readonly Guid MF_TOPONODE_RATELESS = new Guid(0x14932f9d, 0x9087, 0x4bb4, 0x84, 0x12, 0x51, 0x67, 0x14, 0x5c, 0xbe, 0x04);
        public static readonly Guid MF_TOPONODE_SEQUENCE_ELEMENTID = new Guid(0x835c58ef, 0xe075, 0x4bc7, 0xbc, 0xba, 0x4d, 0xe0, 0x00, 0xdf, 0x9a, 0xe6);
        public static readonly Guid MF_TOPONODE_SOURCE = new Guid(0x835c58ec, 0xe075, 0x4bc7, 0xbc, 0xba, 0x4d, 0xe0, 0x00, 0xdf, 0x9a, 0xe6);
        public static readonly Guid MF_TOPONODE_STREAM_DESCRIPTOR = new Guid(0x835c58ee, 0xe075, 0x4bc7, 0xbc, 0xba, 0x4d, 0xe0, 0x00, 0xdf, 0x9a, 0xe6);
        public static readonly Guid MF_TOPONODE_STREAMID = new Guid(0x14932f9b, 0x9087, 0x4bb4, 0x84, 0x12, 0x51, 0x67, 0x14, 0x5c, 0xbe, 0x04);
        public static readonly Guid MF_TOPONODE_TRANSFORM_OBJECTID = new Guid(0x88dcc0c9, 0x293e, 0x4e8b, 0x9a, 0xeb, 0xa, 0xd6, 0x4c, 0xc0, 0x16, 0xb0);
        public static readonly Guid MF_TOPONODE_WORKQUEUE_ID = new Guid(0x494bbcf8, 0xb031, 0x4e38, 0x97, 0xc4, 0xd5, 0x42, 0x2d, 0xd6, 0x18, 0xdc);
        public static readonly Guid MF_TOPONODE_WORKQUEUE_MMCSS_CLASS = new Guid(0x494bbcf9, 0xb031, 0x4e38, 0x97, 0xc4, 0xd5, 0x42, 0x2d, 0xd6, 0x18, 0xdc);
        public static readonly Guid MF_TOPONODE_WORKQUEUE_MMCSS_TASKID = new Guid(0x494bbcff, 0xb031, 0x4e38, 0x97, 0xc4, 0xd5, 0x42, 0x2d, 0xd6, 0x18, 0xdc);

        // Transform Attributes
        public static readonly Guid MF_ACTIVATE_MFT_LOCKED = new Guid(0xc1f6093c, 0x7f65, 0x4fbd, 0x9e, 0x39, 0x5f, 0xae, 0xc3, 0xc4, 0xfb, 0xd7);
        public static readonly Guid MF_SA_D3D_AWARE = new Guid(0xeaa35c29, 0x775e, 0x488e, 0x9b, 0x61, 0xb3, 0x28, 0x3e, 0x49, 0x58, 0x3b);

        // {53476A11-3F13-49fb-AC42-EE2733C96741} MFT_SUPPORT_DYNAMIC_FORMAT_CHANGE {UINT32 (BOOL)}
        public static readonly Guid MFT_SUPPORT_DYNAMIC_FORMAT_CHANGE = new Guid(0x53476a11, 0x3f13, 0x49fb, 0xac, 0x42, 0xee, 0x27, 0x33, 0xc9, 0x67, 0x41);

        // Presentation Descriptor Attributes

        public static readonly Guid MF_PD_APP_CONTEXT = new Guid(0x6c990d32, 0xbb8e, 0x477a, 0x85, 0x98, 0xd, 0x5d, 0x96, 0xfc, 0xd8, 0x8a);
        public static readonly Guid MF_PD_DURATION = new Guid(0x6c990d33, 0xbb8e, 0x477a, 0x85, 0x98, 0xd, 0x5d, 0x96, 0xfc, 0xd8, 0x8a);
        public static readonly Guid MF_PD_LAST_MODIFIED_TIME = new Guid(0x6c990d38, 0xbb8e, 0x477a, 0x85, 0x98, 0xd, 0x5d, 0x96, 0xfc, 0xd8, 0x8a);
        public static readonly Guid MF_PD_MIME_TYPE = new Guid(0x6c990d37, 0xbb8e, 0x477a, 0x85, 0x98, 0xd, 0x5d, 0x96, 0xfc, 0xd8, 0x8a);
        public static readonly Guid MF_PD_PMPHOST_CONTEXT = new Guid(0x6c990d31, 0xbb8e, 0x477a, 0x85, 0x98, 0xd, 0x5d, 0x96, 0xfc, 0xd8, 0x8a);
        public static readonly Guid MF_PD_SAMI_STYLELIST = new Guid(0xe0b73c7f, 0x486d, 0x484e, 0x98, 0x72, 0x4d, 0xe5, 0x19, 0x2a, 0x7b, 0xf8);
        public static readonly Guid MF_PD_TOTAL_FILE_SIZE = new Guid(0x6c990d34, 0xbb8e, 0x477a, 0x85, 0x98, 0xd, 0x5d, 0x96, 0xfc, 0xd8, 0x8a);
        public static readonly Guid MF_PD_AUDIO_ENCODING_BITRATE = new Guid(0x6c990d35, 0xbb8e, 0x477a, 0x85, 0x98, 0xd, 0x5d, 0x96, 0xfc, 0xd8, 0x8a);
        public static readonly Guid MF_PD_VIDEO_ENCODING_BITRATE = new Guid(0x6c990d36, 0xbb8e, 0x477a, 0x85, 0x98, 0xd, 0x5d, 0x96, 0xfc, 0xd8, 0x8a);

        // wmcontainer.h Attributes
        public static readonly Guid MF_PD_ASF_FILEPROPERTIES_FILE_ID = new Guid(0x3de649b4, 0xd76d, 0x4e66, 0x9e, 0xc9, 0x78, 0x12, 0xf, 0xb4, 0xc7, 0xe3);
        public static readonly Guid MF_PD_ASF_FILEPROPERTIES_CREATION_TIME = new Guid(0x3de649b6, 0xd76d, 0x4e66, 0x9e, 0xc9, 0x78, 0x12, 0xf, 0xb4, 0xc7, 0xe3);
        public static readonly Guid MF_PD_ASF_FILEPROPERTIES_PACKETS = new Guid(0x3de649b7, 0xd76d, 0x4e66, 0x9e, 0xc9, 0x78, 0x12, 0xf, 0xb4, 0xc7, 0xe3);
        public static readonly Guid MF_PD_ASF_FILEPROPERTIES_PLAY_DURATION = new Guid(0x3de649b8, 0xd76d, 0x4e66, 0x9e, 0xc9, 0x78, 0x12, 0xf, 0xb4, 0xc7, 0xe3);
        public static readonly Guid MF_PD_ASF_FILEPROPERTIES_SEND_DURATION = new Guid(0x3de649b9, 0xd76d, 0x4e66, 0x9e, 0xc9, 0x78, 0x12, 0xf, 0xb4, 0xc7, 0xe3);
        public static readonly Guid MF_PD_ASF_FILEPROPERTIES_PREROLL = new Guid(0x3de649ba, 0xd76d, 0x4e66, 0x9e, 0xc9, 0x78, 0x12, 0xf, 0xb4, 0xc7, 0xe3);
        public static readonly Guid MF_PD_ASF_FILEPROPERTIES_FLAGS = new Guid(0x3de649bb, 0xd76d, 0x4e66, 0x9e, 0xc9, 0x78, 0x12, 0xf, 0xb4, 0xc7, 0xe3);
        public static readonly Guid MF_PD_ASF_FILEPROPERTIES_MIN_PACKET_SIZE = new Guid(0x3de649bc, 0xd76d, 0x4e66, 0x9e, 0xc9, 0x78, 0x12, 0xf, 0xb4, 0xc7, 0xe3);
        public static readonly Guid MF_PD_ASF_FILEPROPERTIES_MAX_PACKET_SIZE = new Guid(0x3de649bd, 0xd76d, 0x4e66, 0x9e, 0xc9, 0x78, 0x12, 0xf, 0xb4, 0xc7, 0xe3);
        public static readonly Guid MF_PD_ASF_FILEPROPERTIES_MAX_BITRATE = new Guid(0x3de649be, 0xd76d, 0x4e66, 0x9e, 0xc9, 0x78, 0x12, 0xf, 0xb4, 0xc7, 0xe3);
        public static readonly Guid MF_PD_ASF_CONTENTENCRYPTION_TYPE = new Guid(0x8520fe3d, 0x277e, 0x46ea, 0x99, 0xe4, 0xe3, 0xa, 0x86, 0xdb, 0x12, 0xbe);
        public static readonly Guid MF_PD_ASF_CONTENTENCRYPTION_KEYID = new Guid(0x8520fe3e, 0x277e, 0x46ea, 0x99, 0xe4, 0xe3, 0xa, 0x86, 0xdb, 0x12, 0xbe);
        public static readonly Guid MF_PD_ASF_CONTENTENCRYPTION_SECRET_DATA = new Guid(0x8520fe3f, 0x277e, 0x46ea, 0x99, 0xe4, 0xe3, 0xa, 0x86, 0xdb, 0x12, 0xbe);
        public static readonly Guid MF_PD_ASF_CONTENTENCRYPTION_LICENSE_URL = new Guid(0x8520fe40, 0x277e, 0x46ea, 0x99, 0xe4, 0xe3, 0xa, 0x86, 0xdb, 0x12, 0xbe);
        public static readonly Guid MF_PD_ASF_CONTENTENCRYPTIONEX_ENCRYPTION_DATA = new Guid(0x62508be5, 0xecdf, 0x4924, 0xa3, 0x59, 0x72, 0xba, 0xb3, 0x39, 0x7b, 0x9d);
        public static readonly Guid MF_PD_ASF_LANGLIST = new Guid(0xf23de43c, 0x9977, 0x460d, 0xa6, 0xec, 0x32, 0x93, 0x7f, 0x16, 0xf, 0x7d);
        public static readonly Guid MF_PD_ASF_MARKER = new Guid(0x5134330e, 0x83a6, 0x475e, 0xa9, 0xd5, 0x4f, 0xb8, 0x75, 0xfb, 0x2e, 0x31);
        public static readonly Guid MF_PD_ASF_SCRIPT = new Guid(0xe29cd0d7, 0xd602, 0x4923, 0xa7, 0xfe, 0x73, 0xfd, 0x97, 0xec, 0xc6, 0x50);
        public static readonly Guid MF_PD_ASF_CODECLIST = new Guid(0xe4bb3509, 0xc18d, 0x4df1, 0xbb, 0x99, 0x7a, 0x36, 0xb3, 0xcc, 0x41, 0x19);
        public static readonly Guid MF_PD_ASF_METADATA_IS_VBR = new Guid(0x5fc6947a, 0xef60, 0x445d, 0xb4, 0x49, 0x44, 0x2e, 0xcc, 0x78, 0xb4, 0xc1);
        public static readonly Guid MF_PD_ASF_METADATA_V8_VBRPEAK = new Guid(0x5fc6947b, 0xef60, 0x445d, 0xb4, 0x49, 0x44, 0x2e, 0xcc, 0x78, 0xb4, 0xc1);
        public static readonly Guid MF_PD_ASF_METADATA_V8_BUFFERAVERAGE = new Guid(0x5fc6947c, 0xef60, 0x445d, 0xb4, 0x49, 0x44, 0x2e, 0xcc, 0x78, 0xb4, 0xc1);
        public static readonly Guid MF_PD_ASF_METADATA_LEAKY_BUCKET_PAIRS = new Guid(0x5fc6947d, 0xef60, 0x445d, 0xb4, 0x49, 0x44, 0x2e, 0xcc, 0x78, 0xb4, 0xc1);
        public static readonly Guid MF_PD_ASF_DATA_START_OFFSET = new Guid(0xe7d5b3e7, 0x1f29, 0x45d3, 0x88, 0x22, 0x3e, 0x78, 0xfa, 0xe2, 0x72, 0xed);
        public static readonly Guid MF_PD_ASF_DATA_LENGTH = new Guid(0xe7d5b3e8, 0x1f29, 0x45d3, 0x88, 0x22, 0x3e, 0x78, 0xfa, 0xe2, 0x72, 0xed);
        public static readonly Guid MF_SD_ASF_EXTSTRMPROP_LANGUAGE_ID_INDEX = new Guid(0x48f8a522, 0x305d, 0x422d, 0x85, 0x24, 0x25, 0x2, 0xdd, 0xa3, 0x36, 0x80);
        public static readonly Guid MF_SD_ASF_EXTSTRMPROP_AVG_DATA_BITRATE = new Guid(0x48f8a523, 0x305d, 0x422d, 0x85, 0x24, 0x25, 0x2, 0xdd, 0xa3, 0x36, 0x80);
        public static readonly Guid MF_SD_ASF_EXTSTRMPROP_AVG_BUFFERSIZE = new Guid(0x48f8a524, 0x305d, 0x422d, 0x85, 0x24, 0x25, 0x2, 0xdd, 0xa3, 0x36, 0x80);
        public static readonly Guid MF_SD_ASF_EXTSTRMPROP_MAX_DATA_BITRATE = new Guid(0x48f8a525, 0x305d, 0x422d, 0x85, 0x24, 0x25, 0x2, 0xdd, 0xa3, 0x36, 0x80);
        public static readonly Guid MF_SD_ASF_EXTSTRMPROP_MAX_BUFFERSIZE = new Guid(0x48f8a526, 0x305d, 0x422d, 0x85, 0x24, 0x25, 0x2, 0xdd, 0xa3, 0x36, 0x80);
        public static readonly Guid MF_SD_ASF_STREAMBITRATES_BITRATE = new Guid(0xa8e182ed, 0xafc8, 0x43d0, 0xb0, 0xd1, 0xf6, 0x5b, 0xad, 0x9d, 0xa5, 0x58);
        public static readonly Guid MF_SD_ASF_METADATA_DEVICE_CONFORMANCE_TEMPLATE = new Guid(0x245e929d, 0xc44e, 0x4f7e, 0xbb, 0x3c, 0x77, 0xd4, 0xdf, 0xd2, 0x7f, 0x8a);
        public static readonly Guid MF_PD_ASF_INFO_HAS_AUDIO = new Guid(0x80e62295, 0x2296, 0x4a44, 0xb3, 0x1c, 0xd1, 0x3, 0xc6, 0xfe, 0xd2, 0x3c);
        public static readonly Guid MF_PD_ASF_INFO_HAS_VIDEO = new Guid(0x80e62296, 0x2296, 0x4a44, 0xb3, 0x1c, 0xd1, 0x3, 0xc6, 0xfe, 0xd2, 0x3c);
        public static readonly Guid MF_PD_ASF_INFO_HAS_NON_AUDIO_VIDEO = new Guid(0x80e62297, 0x2296, 0x4a44, 0xb3, 0x1c, 0xd1, 0x3, 0xc6, 0xfe, 0xd2, 0x3c);
        public static readonly Guid MF_ASFSTREAMCONFIG_LEAKYBUCKET1 = new Guid(0xc69b5901, 0xea1a, 0x4c9b, 0xb6, 0x92, 0xe2, 0xa0, 0xd2, 0x9a, 0x8a, 0xdd);
        public static readonly Guid MF_ASFSTREAMCONFIG_LEAKYBUCKET2 = new Guid(0xc69b5902, 0xea1a, 0x4c9b, 0xb6, 0x92, 0xe2, 0xa0, 0xd2, 0x9a, 0x8a, 0xdd);

        // Arbitrary

        // {9E6BD6F5-0109-4f95-84AC-9309153A19FC}   MF_MT_ARBITRARY_HEADER          {MT_ARBITRARY_HEADER}
        public static readonly Guid MF_MT_ARBITRARY_HEADER = new Guid(0x9e6bd6f5, 0x109, 0x4f95, 0x84, 0xac, 0x93, 0x9, 0x15, 0x3a, 0x19, 0xfc);

        // {5A75B249-0D7D-49a1-A1C3-E0D87F0CADE5}   MF_MT_ARBITRARY_FORMAT          {Blob}
        public static readonly Guid MF_MT_ARBITRARY_FORMAT = new Guid(0x5a75b249, 0xd7d, 0x49a1, 0xa1, 0xc3, 0xe0, 0xd8, 0x7f, 0xc, 0xad, 0xe5);

        // Image

        // {ED062CF4-E34E-4922-BE99-934032133D7C}   MF_MT_IMAGE_LOSS_TOLERANT       {UINT32 (BOOL)}
        public static readonly Guid MF_MT_IMAGE_LOSS_TOLERANT = new Guid(0xed062cf4, 0xe34e, 0x4922, 0xbe, 0x99, 0x93, 0x40, 0x32, 0x13, 0x3d, 0x7c);

        // MPEG-4 Media Type Attributes

        // {261E9D83-9529-4B8F-A111-8B9C950A81A9}   MF_MT_MPEG4_SAMPLE_DESCRIPTION   {BLOB}
        public static readonly Guid MF_MT_MPEG4_SAMPLE_DESCRIPTION = new Guid(0x261e9d83, 0x9529, 0x4b8f, 0xa1, 0x11, 0x8b, 0x9c, 0x95, 0x0a, 0x81, 0xa9);

        // {9aa7e155-b64a-4c1d-a500-455d600b6560}   MF_MT_MPEG4_CURRENT_SAMPLE_ENTRY {UINT32}
        public static readonly Guid MF_MT_MPEG4_CURRENT_SAMPLE_ENTRY = new Guid(0x9aa7e155, 0xb64a, 0x4c1d, 0xa5, 0x00, 0x45, 0x5d, 0x60, 0x0b, 0x65, 0x60);

        // Save original format information for AVI and WAV files

        // {d7be3fe0-2bc7-492d-b843-61a1919b70c3}   MF_MT_ORIGINAL_4CC               (UINT32)
        public static readonly Guid MF_MT_ORIGINAL_4CC = new Guid(0xd7be3fe0, 0x2bc7, 0x492d, 0xb8, 0x43, 0x61, 0xa1, 0x91, 0x9b, 0x70, 0xc3);

        // {8cbbc843-9fd9-49c2-882f-a72586c408ad}   MF_MT_ORIGINAL_WAVE_FORMAT_TAG   (UINT32)
        public static readonly Guid MF_MT_ORIGINAL_WAVE_FORMAT_TAG = new Guid(0x8cbbc843, 0x9fd9, 0x49c2, 0x88, 0x2f, 0xa7, 0x25, 0x86, 0xc4, 0x08, 0xad);

        // Video Capture Media Type Attributes

        // {D2E7558C-DC1F-403f-9A72-D28BB1EB3B5E}   MF_MT_FRAME_RATE_RANGE_MIN      {UINT64 (HI32(Numerator),LO32(Denominator))}
        public static readonly Guid MF_MT_FRAME_RATE_RANGE_MIN = new Guid(0xd2e7558c, 0xdc1f, 0x403f, 0x9a, 0x72, 0xd2, 0x8b, 0xb1, 0xeb, 0x3b, 0x5e);

        // {E3371D41-B4CF-4a05-BD4E-20B88BB2C4D6}   MF_MT_FRAME_RATE_RANGE_MAX      {UINT64 (HI32(Numerator),LO32(Denominator))}
        public static readonly Guid MF_MT_FRAME_RATE_RANGE_MAX = new Guid(0xe3371d41, 0xb4cf, 0x4a05, 0xbd, 0x4e, 0x20, 0xb8, 0x8b, 0xb2, 0xc4, 0xd6);

        public static readonly Guid MF_TOPOLOGY_DXVA_MODE = new Guid(0x1e8d34f6, 0xf5ab, 0x4e23, 0xbb, 0x88, 0x87, 0x4a, 0xa3, 0xa1, 0xa7, 0x4d);
        public static readonly Guid MF_TOPOLOGY_STATIC_PLAYBACK_OPTIMIZATIONS = new Guid(0xb86cac42, 0x41a6, 0x4b79, 0x89, 0x7a, 0x1a, 0xb0, 0xe5, 0x2b, 0x4a, 0x1b);
        public static readonly Guid MF_TOPOLOGY_PLAYBACK_MAX_DIMS = new Guid(0x5715cf19, 0x5768, 0x44aa, 0xad, 0x6e, 0x87, 0x21, 0xf1, 0xb0, 0xf9, 0xbb);

        public static readonly Guid MF_TOPOLOGY_HARDWARE_MODE = new Guid(0xd2d362fd, 0x4e4f, 0x4191, 0xa5, 0x79, 0xc6, 0x18, 0xb6, 0x67, 0x6, 0xaf);
        public static readonly Guid MF_TOPOLOGY_PLAYBACK_FRAMERATE = new Guid(0xc164737a, 0xc2b1, 0x4553, 0x83, 0xbb, 0x5a, 0x52, 0x60, 0x72, 0x44, 0x8f);
        public static readonly Guid MF_TOPOLOGY_DYNAMIC_CHANGE_NOT_ALLOWED = new Guid(0xd529950b, 0xd484, 0x4527, 0xa9, 0xcd, 0xb1, 0x90, 0x95, 0x32, 0xb5, 0xb0);
        public static readonly Guid MF_TOPOLOGY_ENUMERATE_SOURCE_TYPES = new Guid(0x6248c36d, 0x5d0b, 0x4f40, 0xa0, 0xbb, 0xb0, 0xb3, 0x05, 0xf7, 0x76, 0x98);
        public static readonly Guid MF_TOPOLOGY_START_TIME_ON_PRESENTATION_SWITCH = new Guid(0xc8cc113f, 0x7951, 0x4548, 0xaa, 0xd6, 0x9e, 0xd6, 0x20, 0x2e, 0x62, 0xb3);

        public static readonly Guid MF_PD_PLAYBACK_ELEMENT_ID = new Guid(0x6c990d39, 0xbb8e, 0x477a, 0x85, 0x98, 0xd, 0x5d, 0x96, 0xfc, 0xd8, 0x8a);
        public static readonly Guid MF_PD_PREFERRED_LANGUAGE = new Guid(0x6c990d3A, 0xbb8e, 0x477a, 0x85, 0x98, 0xd, 0x5d, 0x96, 0xfc, 0xd8, 0x8a);
        public static readonly Guid MF_PD_PLAYBACK_BOUNDARY_TIME = new Guid(0x6c990d3b, 0xbb8e, 0x477a, 0x85, 0x98, 0xd, 0x5d, 0x96, 0xfc, 0xd8, 0x8a);
        public static readonly Guid MF_PD_AUDIO_ISVARIABLEBITRATE = new Guid(0x33026ee0, 0xe387, 0x4582, 0xae, 0x0a, 0x34, 0xa2, 0xad, 0x3b, 0xaa, 0x18);

        public static readonly Guid MF_SD_STREAM_NAME = new Guid(0x4f1b099d, 0xd314, 0x41e5, 0xa7, 0x81, 0x7f, 0xef, 0xaa, 0x4c, 0x50, 0x1f);
        public static readonly Guid MF_SD_MUTUALLY_EXCLUSIVE = new Guid(0x23ef79c, 0x388d, 0x487f, 0xac, 0x17, 0x69, 0x6c, 0xd6, 0xe3, 0xc6, 0xf5);

        public static readonly Guid MF_SAMPLEGRABBERSINK_IGNORE_CLOCK = new Guid(0x0efda2c0, 0x2b69, 0x4e2e, 0xab, 0x8d, 0x46, 0xdc, 0xbf, 0xf7, 0xd2, 0x5d);
        public static readonly Guid MF_BYTESTREAMHANDLER_ACCEPTS_SHARE_WRITE = new Guid(0xa6e1f733, 0x3001, 0x4915, 0x81, 0x50, 0x15, 0x58, 0xa2, 0x18, 0xe, 0xc8);

        public static readonly Guid MF_TRANSCODE_CONTAINERTYPE = new Guid(0x150ff23f, 0x4abc, 0x478b, 0xac, 0x4f, 0xe1, 0x91, 0x6f, 0xba, 0x1c, 0xca);
        public static readonly Guid MF_TRANSCODE_SKIP_METADATA_TRANSFER = new Guid(0x4e4469ef, 0xb571, 0x4959, 0x8f, 0x83, 0x3d, 0xcf, 0xba, 0x33, 0xa3, 0x93);
        public static readonly Guid MF_TRANSCODE_TOPOLOGYMODE = new Guid(0x3e3df610, 0x394a, 0x40b2, 0x9d, 0xea, 0x3b, 0xab, 0x65, 0xb, 0xeb, 0xf2);
        public static readonly Guid MF_TRANSCODE_ADJUST_PROFILE = new Guid(0x9c37c21b, 0x60f, 0x487c, 0xa6, 0x90, 0x80, 0xd7, 0xf5, 0xd, 0x1c, 0x72);

        public static readonly Guid MF_TRANSCODE_ENCODINGPROFILE = new Guid(0x6947787c, 0xf508, 0x4ea9, 0xb1, 0xe9, 0xa1, 0xfe, 0x3a, 0x49, 0xfb, 0xc9);
        public static readonly Guid MF_TRANSCODE_QUALITYVSSPEED = new Guid(0x98332df8, 0x03cd, 0x476b, 0x89, 0xfa, 0x3f, 0x9e, 0x44, 0x2d, 0xec, 0x9f);
        public static readonly Guid MF_TRANSCODE_DONOT_INSERT_ENCODER = new Guid(0xf45aa7ce, 0xab24, 0x4012, 0xa1, 0x1b, 0xdc, 0x82, 0x20, 0x20, 0x14, 0x10);

        public static readonly Guid MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE = new Guid(0xc60ac5fe, 0x252a, 0x478f, 0xa0, 0xef, 0xbc, 0x8f, 0xa5, 0xf7, 0xca, 0xd3);
        public static readonly Guid MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_HW_SOURCE = new Guid(0xde7046ba, 0x54d6, 0x4487, 0xa2, 0xa4, 0xec, 0x7c, 0xd, 0x1b, 0xd1, 0x63);
        public static readonly Guid MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME = new Guid(0x60d0e559, 0x52f8, 0x4fa2, 0xbb, 0xce, 0xac, 0xdb, 0x34, 0xa8, 0xec, 0x1);
        public static readonly Guid MF_DEVSOURCE_ATTRIBUTE_MEDIA_TYPE = new Guid(0x56a819ca, 0xc78, 0x4de4, 0xa0, 0xa7, 0x3d, 0xda, 0xba, 0xf, 0x24, 0xd4);
        public static readonly Guid MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_CATEGORY = new Guid(0x77f0ae69, 0xc3bd, 0x4509, 0x94, 0x1d, 0x46, 0x7e, 0x4d, 0x24, 0x89, 0x9e);
        public static readonly Guid MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_SYMBOLIC_LINK = new Guid(0x58f0aad8, 0x22bf, 0x4f8a, 0xbb, 0x3d, 0xd2, 0xc4, 0x97, 0x8c, 0x6e, 0x2f);
        public static readonly Guid MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_MAX_BUFFERS = new Guid(0x7dd9b730, 0x4f2d, 0x41d5, 0x8f, 0x95, 0xc, 0xc9, 0xa9, 0x12, 0xba, 0x26);
        public static readonly Guid MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_ENDPOINT_ID = new Guid(0x30da9258, 0xfeb9, 0x47a7, 0xa4, 0x53, 0x76, 0x3a, 0x7a, 0x8e, 0x1c, 0x5f);
        public static readonly Guid MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_ROLE = new Guid(0xbc9d118e, 0x8c67, 0x4a18, 0x85, 0xd4, 0x12, 0xd3, 0x0, 0x40, 0x5, 0x52);

        public static readonly Guid MFSampleExtension_DeviceTimestamp = new Guid(0x8f3e35e7, 0x2dcd, 0x4887, 0x86, 0x22, 0x2a, 0x58, 0xba, 0xa6, 0x52, 0xb0);

        public static readonly Guid MF_TRANSFORM_ASYNC = new Guid(0xf81a699a, 0x649a, 0x497d, 0x8c, 0x73, 0x29, 0xf8, 0xfe, 0xd6, 0xad, 0x7a);
        public static readonly Guid MF_TRANSFORM_ASYNC_UNLOCK = new Guid(0xe5666d6b, 0x3422, 0x4eb6, 0xa4, 0x21, 0xda, 0x7d, 0xb1, 0xf8, 0xe2, 0x7);
        public static readonly Guid MF_TRANSFORM_FLAGS_Attribute = new Guid(0x9359bb7e, 0x6275, 0x46c4, 0xa0, 0x25, 0x1c, 0x1, 0xe4, 0x5f, 0x1a, 0x86);
        public static readonly Guid MF_TRANSFORM_CATEGORY_Attribute = new Guid(0xceabba49, 0x506d, 0x4757, 0xa6, 0xff, 0x66, 0xc1, 0x84, 0x98, 0x7e, 0x4e);
        public static readonly Guid MFT_TRANSFORM_CLSID_Attribute = new Guid(0x6821c42b, 0x65a4, 0x4e82, 0x99, 0xbc, 0x9a, 0x88, 0x20, 0x5e, 0xcd, 0xc);
        public static readonly Guid MFT_INPUT_TYPES_Attributes = new Guid(0x4276c9b1, 0x759d, 0x4bf3, 0x9c, 0xd0, 0xd, 0x72, 0x3d, 0x13, 0x8f, 0x96);
        public static readonly Guid MFT_OUTPUT_TYPES_Attributes = new Guid(0x8eae8cf3, 0xa44f, 0x4306, 0xba, 0x5c, 0xbf, 0x5d, 0xda, 0x24, 0x28, 0x18);
        public static readonly Guid MFT_ENUM_HARDWARE_URL_Attribute = new Guid(0x2fb866ac, 0xb078, 0x4942, 0xab, 0x6c, 0x0, 0x3d, 0x5, 0xcd, 0xa6, 0x74);
        public static readonly Guid MFT_FRIENDLY_NAME_Attribute = new Guid(0x314ffbae, 0x5b41, 0x4c95, 0x9c, 0x19, 0x4e, 0x7d, 0x58, 0x6f, 0xac, 0xe3);
        public static readonly Guid MFT_CONNECTED_STREAM_ATTRIBUTE = new Guid(0x71eeb820, 0xa59f, 0x4de2, 0xbc, 0xec, 0x38, 0xdb, 0x1d, 0xd6, 0x11, 0xa4);
        public static readonly Guid MFT_CONNECTED_TO_HW_STREAM = new Guid(0x34e6e728, 0x6d6, 0x4491, 0xa5, 0x53, 0x47, 0x95, 0x65, 0xd, 0xb9, 0x12);
        public static readonly Guid MFT_PREFERRED_OUTPUTTYPE_Attribute = new Guid(0x7e700499, 0x396a, 0x49ee, 0xb1, 0xb4, 0xf6, 0x28, 0x2, 0x1e, 0x8c, 0x9d);
        public static readonly Guid MFT_PROCESS_LOCAL_Attribute = new Guid(0x543186e4, 0x4649, 0x4e65, 0xb5, 0x88, 0x4a, 0xa3, 0x52, 0xaf, 0xf3, 0x79);
        public static readonly Guid MFT_PREFERRED_ENCODER_PROFILE = new Guid(0x53004909, 0x1ef5, 0x46d7, 0xa1, 0x8e, 0x5a, 0x75, 0xf8, 0xb5, 0x90, 0x5f);
        public static readonly Guid MFT_HW_TIMESTAMP_WITH_QPC_Attribute = new Guid(0x8d030fb8, 0xcc43, 0x4258, 0xa2, 0x2e, 0x92, 0x10, 0xbe, 0xf8, 0x9b, 0xe4);
        public static readonly Guid MFT_FIELDOFUSE_UNLOCK_Attribute = new Guid(0x8ec2e9fd, 0x9148, 0x410d, 0x83, 0x1e, 0x70, 0x24, 0x39, 0x46, 0x1a, 0x8e);
        public static readonly Guid MFT_CODEC_MERIT_Attribute = new Guid(0x88a7cb15, 0x7b07, 0x4a34, 0x91, 0x28, 0xe6, 0x4c, 0x67, 0x3, 0xc4, 0xd3);
        public static readonly Guid MFT_ENUM_TRANSCODE_ONLY_ATTRIBUTE = new Guid(0x111ea8cd, 0xb62a, 0x4bdb, 0x89, 0xf6, 0x67, 0xff, 0xcd, 0xc2, 0x45, 0x8b);

        public static readonly Guid MF_MP2DLNA_USE_MMCSS = new Guid(0x54f3e2ee, 0xa2a2, 0x497d, 0x98, 0x34, 0x97, 0x3a, 0xfd, 0xe5, 0x21, 0xeb);
        public static readonly Guid MF_MP2DLNA_VIDEO_BIT_RATE = new Guid(0xe88548de, 0x73b4, 0x42d7, 0x9c, 0x75, 0xad, 0xfa, 0xa, 0x2a, 0x6e, 0x4c);
        public static readonly Guid MF_MP2DLNA_AUDIO_BIT_RATE = new Guid(0x2d1c070e, 0x2b5f, 0x4ab3, 0xa7, 0xe6, 0x8d, 0x94, 0x3b, 0xa8, 0xd0, 0x0a);
        public static readonly Guid MF_MP2DLNA_ENCODE_QUALITY = new Guid(0xb52379d7, 0x1d46, 0x4fb6, 0xa3, 0x17, 0xa4, 0xa5, 0xf6, 0x09, 0x59, 0xf8);
        public static readonly Guid MF_MP2DLNA_STATISTICS = new Guid(0x75e488a3, 0xd5ad, 0x4898, 0x85, 0xe0, 0xbc, 0xce, 0x24, 0xa7, 0x22, 0xd7);

        public static readonly Guid MF_SINK_WRITER_ASYNC_CALLBACK = new Guid(0x48cb183e, 0x7b0b, 0x46f4, 0x82, 0x2e, 0x5e, 0x1d, 0x2d, 0xda, 0x43, 0x54);
        public static readonly Guid MF_SINK_WRITER_DISABLE_THROTTLING = new Guid(0x08b845d8, 0x2b74, 0x4afe, 0x9d, 0x53, 0xbe, 0x16, 0xd2, 0xd5, 0xae, 0x4f);
        public static readonly Guid MF_READWRITE_DISABLE_CONVERTERS = new Guid(0x98d5b065, 0x1374, 0x4847, 0x8d, 0x5d, 0x31, 0x52, 0x0f, 0xee, 0x71, 0x56);
        public static readonly Guid MF_READWRITE_ENABLE_HARDWARE_TRANSFORMS = new Guid(0xa634a91c, 0x822b, 0x41b9, 0xa4, 0x94, 0x4d, 0xe4, 0x64, 0x36, 0x12, 0xb0);

        public static readonly Guid MF_SOURCE_READER_ASYNC_CALLBACK = new Guid(0x1e3dbeac, 0xbb43, 0x4c35, 0xb5, 0x07, 0xcd, 0x64, 0x44, 0x64, 0xc9, 0x65);
        public static readonly Guid MF_SOURCE_READER_D3D_MANAGER = new Guid(0xec822da2, 0xe1e9, 0x4b29, 0xa0, 0xd8, 0x56, 0x3c, 0x71, 0x9f, 0x52, 0x69);
        public static readonly Guid MF_SOURCE_READER_DISABLE_DXVA = new Guid(0xaa456cfd, 0x3943, 0x4a1e, 0xa7, 0x7d, 0x18, 0x38, 0xc0, 0xea, 0x2e, 0x35);
        public static readonly Guid MF_SOURCE_READER_MEDIASOURCE_CONFIG = new Guid(0x9085abeb, 0x0354, 0x48f9, 0xab, 0xb5, 0x20, 0x0d, 0xf8, 0x38, 0xc6, 0x8e);
        public static readonly Guid MF_SOURCE_READER_MEDIASOURCE_CHARACTERISTICS = new Guid(0x6d23f5c8, 0xc5d7, 0x4a9b, 0x99, 0x71, 0x5d, 0x11, 0xf8, 0xbc, 0xa8, 0x80);
        public static readonly Guid MF_SOURCE_READER_ENABLE_VIDEO_PROCESSING = new Guid(0xfb394f3d, 0xccf1, 0x42ee, 0xbb, 0xb3, 0xf9, 0xb8, 0x45, 0xd5, 0x68, 0x1d);
        public static readonly Guid MF_SOURCE_READER_DISCONNECT_MEDIASOURCE_ON_SHUTDOWN = new Guid(0x56b67165, 0x219e, 0x456d, 0xa2, 0x2e, 0x2d, 0x30, 0x04, 0xc7, 0xfe, 0x56);
    }

    public enum MFStartup
    {
        NoSocket = 0x1,
        Lite = 0x1,
        Full = 0
    }

    public enum MFVPMessageType
    {
        Flush = 0,
        InvalidateMediaType,
        ProcessInputNotify,
        BeginStreaming,
        EndStreaming,
        EndOfStream,
        Step,
        CancelStep
    }

    [Flags]
    public enum MFTInputStreamInfoFlags
    {
        WholeSamples = 0x1,
        SingleSamplePerBuffer = 0x2,
        FixedSampleSize = 0x4,
        HoldsBuffers = 0x8,
        DoesNotAddRef = 0x100,
        Removable = 0x200,
        Optional = 0x400,
        ProcessesInPlace = 0x800
    }

    public enum MFTMessageType
    {
        CommandDrain = 1,
        CommandFlush = 0,
        NotifyBeginStreaming = 0x10000000,
        NotifyEndOfStream = 0x10000002,
        NotifyEndStreaming = 0x10000001,
        NotifyStartOfStream = 0x10000003,
        SetD3DManager = 2,
        DropSamples = 0x00000003,
        CommandMarker = 0x20000000
    }

    [Flags]
    public enum MFTOutputStreamInfoFlags
    {
        None = 0,
        WholeSamples = 0x00000001,
        SingleSamplePerBuffer = 0x00000002,
        FixedSampleSize = 0x00000004,
        Discardable = 0x00000008,
        Optional = 0x00000010,
        ProvidesSamples = 0x00000100,
        CanProvideSamples = 0x00000200,
        LazyRead = 0x00000400,
        Removable = 0x00000800
    }

    [Flags]
    public enum MFVideoFlags : long
    {
        PAD_TO_Mask = 0x0001 | 0x0002,
        PAD_TO_None = 0 * 0x0001,
        PAD_TO_4x3 = 1 * 0x0001,
        PAD_TO_16x9 = 2 * 0x0001,
        SrcContentHintMask = 0x0004 | 0x0008 | 0x0010,
        SrcContentHintNone = 0 * 0x0004,
        SrcContentHint16x9 = 1 * 0x0004,
        SrcContentHint235_1 = 2 * 0x0004,
        AnalogProtected = 0x0020,
        DigitallyProtected = 0x0040,
        ProgressiveContent = 0x0080,
        FieldRepeatCountMask = 0x0100 | 0x0200 | 0x0400,
        FieldRepeatCountShift = 8,
        ProgressiveSeqReset = 0x0800,
        PanScanEnabled = 0x20000,
        LowerFieldFirst = 0x40000,
        BottomUpLinearRep = 0x80000,
        DXVASurface = 0x100000,
        RenderTargetSurface = 0x400000,
        ForceQWORD = 0x7FFFFFFF
    }

    [Flags]
    public enum MFMediaEqual
    {
        None = 0,
        MajorTypes = 0x00000001,
        FormatTypes = 0x00000002,
        FormatData = 0x00000004,
        FormatUserData = 0x00000008
    }
    public enum MFAttributeType
    {
        None = 0x0,
        Blob = 0x1011,
        Double = 0x5,
        Guid = 0x48,
        IUnknown = 13,
        String = 0x1f,
        Uint32 = 0x13,
        Uint64 = 0x15
    }

    [Flags]
    public enum MFTSetTypeFlags
    {
        None = 0,
        TestOnly = 0x00000001
    }

    [Flags]
    public enum ProcessOutputStatus
    {
        None = 0,
        NewStreams = 0x00000100
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct MFTInputStreamInfo
    {
        public long hnsMaxLatency;
        public MFTInputStreamInfoFlags dwFlags;
        public int cbSize;
        public int cbMaxLookahead;
        public int cbAlignment;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MFTOutputStreamInfo
    {
        public MFTOutputStreamInfoFlags dwFlags;
        public int cbSize;
        public int cbAlignment;
    }

    [Flags]
    public enum MFTOutputStatusFlags
    {
        None = 0,
        SampleReady = 0x00000001
    }

    [Flags]
    public enum MFTInputStatusFlags
    {
        None = 0,
        AcceptData = 0x00000001
    }

    [Flags]
    public enum MFTProcessOutputFlags
    {
        None = 0,
        DiscardWhenNoBuffer = 0x00000001
    }

    [Flags]
    public enum MFTOutputDataBufferFlags
    {
        None = 0,
        Incomplete = 0x01000000,
        FormatChange = 0x00000100,
        StreamEnd = 0x00000200,
        NoSample = 0x00000300
    };

    public enum MFVideoChromaSubsampling
    {
        Cosited = 7,
        DV_PAL = 6,
        ForceDWORD = 0x7fffffff,
        Horizontally_Cosited = 4,
        Last = 8,
        MPEG1 = 1,
        MPEG2 = 5,
        ProgressiveChroma = 8,
        Unknown = 0,
        Vertically_AlignedChromaPlanes = 1,
        Vertically_Cosited = 2
    }

    public enum MFAttributesMatchType
    {
        OurItems,
        TheirItems,
        AllItems,
        InterSection,
        Smaller
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MFAYUVSample
    {
        public byte bCrValue;
        public byte bCbValue;
        public byte bYValue;
        public byte bSampleAlpha8;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MFARGB
    {
        public byte rgbBlue;
        public byte rgbGreen;
        public byte rgbRed;
        public byte rgbAlpha;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct MFPaletteEntry
    {
        [FieldOffset(0)]
        public MFARGB ARGB;
        [FieldOffset(0)]
        public MFAYUVSample AYCbCr;
    }

    public enum MediaEventType
    {
        MEUnknown = 0,
        MEError = 1,
        MEExtendedType = 2,
        MENonFatalError = 3,
        MEGenericV1Anchor = MENonFatalError,
        MESessionUnknown = 100,
        MESessionTopologySet = (MESessionUnknown + 1),
        MESessionTopologiesCleared = (MESessionTopologySet + 1),
        MESessionStarted = (MESessionTopologiesCleared + 1),
        MESessionPaused = (MESessionStarted + 1),
        MESessionStopped = (MESessionPaused + 1),
        MESessionClosed = (MESessionStopped + 1),
        MESessionEnded = (MESessionClosed + 1),
        MESessionRateChanged = (MESessionEnded + 1),
        MESessionScrubSampleComplete = (MESessionRateChanged + 1),
        MESessionCapabilitiesChanged = (MESessionScrubSampleComplete + 1),
        MESessionTopologyStatus = (MESessionCapabilitiesChanged + 1),
        MESessionNotifyPresentationTime = (MESessionTopologyStatus + 1),
        MENewPresentation = (MESessionNotifyPresentationTime + 1),
        MELicenseAcquisitionStart = (MENewPresentation + 1),
        MELicenseAcquisitionCompleted = (MELicenseAcquisitionStart + 1),
        MEIndividualizationStart = (MELicenseAcquisitionCompleted + 1),
        MEIndividualizationCompleted = (MEIndividualizationStart + 1),
        MEEnablerProgress = (MEIndividualizationCompleted + 1),
        MEEnablerCompleted = (MEEnablerProgress + 1),
        MEPolicyError = (MEEnablerCompleted + 1),
        MEPolicyReport = (MEPolicyError + 1),
        MEBufferingStarted = (MEPolicyReport + 1),
        MEBufferingStopped = (MEBufferingStarted + 1),
        MEConnectStart = (MEBufferingStopped + 1),
        MEConnectEnd = (MEConnectStart + 1),
        MEReconnectStart = (MEConnectEnd + 1),
        MEReconnectEnd = (MEReconnectStart + 1),
        MERendererEvent = (MEReconnectEnd + 1),
        MESessionStreamSinkFormatChanged = (MERendererEvent + 1),
        MESessionV1Anchor = MESessionStreamSinkFormatChanged,
        MESourceUnknown = 200,
        MESourceStarted = (MESourceUnknown + 1),
        MEStreamStarted = (MESourceStarted + 1),
        MESourceSeeked = (MEStreamStarted + 1),
        MEStreamSeeked = (MESourceSeeked + 1),
        MENewStream = (MEStreamSeeked + 1),
        MEUpdatedStream = (MENewStream + 1),
        MESourceStopped = (MEUpdatedStream + 1),
        MEStreamStopped = (MESourceStopped + 1),
        MESourcePaused = (MEStreamStopped + 1),
        MEStreamPaused = (MESourcePaused + 1),
        MEEndOfPresentation = (MEStreamPaused + 1),
        MEEndOfStream = (MEEndOfPresentation + 1),
        MEMediaSample = (MEEndOfStream + 1),
        MEStreamTick = (MEMediaSample + 1),
        MEStreamThinMode = (MEStreamTick + 1),
        MEStreamFormatChanged = (MEStreamThinMode + 1),
        MESourceRateChanged = (MEStreamFormatChanged + 1),
        MEEndOfPresentationSegment = (MESourceRateChanged + 1),
        MESourceCharacteristicsChanged = (MEEndOfPresentationSegment + 1),
        MESourceRateChangeRequested = (MESourceCharacteristicsChanged + 1),
        MESourceMetadataChanged = (MESourceRateChangeRequested + 1),
        MESequencerSourceTopologyUpdated = (MESourceMetadataChanged + 1),
        MESourceV1Anchor = MESequencerSourceTopologyUpdated,
        MESinkUnknown = 300,
        MEStreamSinkStarted = (MESinkUnknown + 1),
        MEStreamSinkStopped = (MEStreamSinkStarted + 1),
        MEStreamSinkPaused = (MEStreamSinkStopped + 1),
        MEStreamSinkRateChanged = (MEStreamSinkPaused + 1),
        MEStreamSinkRequestSample = (MEStreamSinkRateChanged + 1),
        MEStreamSinkMarker = (MEStreamSinkRequestSample + 1),
        MEStreamSinkPrerolled = (MEStreamSinkMarker + 1),
        MEStreamSinkScrubSampleComplete = (MEStreamSinkPrerolled + 1),
        MEStreamSinkFormatChanged = (MEStreamSinkScrubSampleComplete + 1),
        MEStreamSinkDeviceChanged = (MEStreamSinkFormatChanged + 1),
        MEQualityNotify = (MEStreamSinkDeviceChanged + 1),
        MESinkInvalidated = (MEQualityNotify + 1),
        MEAudioSessionNameChanged = (MESinkInvalidated + 1),
        MEAudioSessionVolumeChanged = (MEAudioSessionNameChanged + 1),
        MEAudioSessionDeviceRemoved = (MEAudioSessionVolumeChanged + 1),
        MEAudioSessionServerShutdown = (MEAudioSessionDeviceRemoved + 1),
        MEAudioSessionGroupingParamChanged = (MEAudioSessionServerShutdown + 1),
        MEAudioSessionIconChanged = (MEAudioSessionGroupingParamChanged + 1),
        MEAudioSessionFormatChanged = (MEAudioSessionIconChanged + 1),
        MEAudioSessionDisconnected = (MEAudioSessionFormatChanged + 1),
        MEAudioSessionExclusiveModeOverride = (MEAudioSessionDisconnected + 1),
        MESinkV1Anchor = MEAudioSessionExclusiveModeOverride,
        METrustUnknown = 400,
        MEPolicyChanged = (METrustUnknown + 1),
        MEContentProtectionMessage = (MEPolicyChanged + 1),
        MEPolicySet = (MEContentProtectionMessage + 1),
        METrustV1Anchor = MEPolicySet,
        MEWMDRMLicenseBackupCompleted = 500,
        MEWMDRMLicenseBackupProgress = 501,
        MEWMDRMLicenseRestoreCompleted = 502,
        MEWMDRMLicenseRestoreProgress = 503,
        MEWMDRMLicenseAcquisitionCompleted = 506,
        MEWMDRMIndividualizationCompleted = 508,
        MEWMDRMIndividualizationProgress = 513,
        MEWMDRMProximityCompleted = 514,
        MEWMDRMLicenseStoreCleaned = 515,
        MEWMDRMRevocationDownloadCompleted = 516,
        MEWMDRMV1Anchor = MEWMDRMRevocationDownloadCompleted,
        METransformUnknown = 600,
        METransformNeedInput,
        METransformHaveOutput,
        METransformDrainComplete,
        METransformMarker,
        MEReservedMax = 10000
    }

    public enum MFVideoInterlaceMode
    {
        FieldInterleavedLowerFirst = 4,
        FieldInterleavedUpperFirst = 3,
        FieldSingleLower = 6,
        FieldSingleUpper = 5,
        ForceDWORD = 0x7fffffff,
        Last = 8,
        MixedInterlaceOrProgressive = 7,
        Progressive = 2,
        Unknown = 0
    }

    public enum MFVideoTransferFunction
    {
        Unknown = 0,
        Func10 = 1,
        Func18 = 2,
        Func20 = 3,
        Func22 = 4,
        Func240M = 6,
        Func28 = 8,
        Func709 = 5,
        ForceDWORD = 0x7fffffff,
        Last = 9,
        sRGB = 7,
        Log_100 = 9,
        Log_316 = 10,
        x709_sym = 11 // symmetric 709
    }

    public enum MFVideoPrimaries
    {
        BT470_2_SysBG = 4,
        BT470_2_SysM = 3,
        BT709 = 2,
        EBU3213 = 7,
        ForceDWORD = 0x7fffffff,
        Last = 9,
        reserved = 1,
        SMPTE_C = 8,
        SMPTE170M = 5,
        SMPTE240M = 6,
        Unknown = 0
    }

    public enum MFVideoPadFlags
    {
        PAD_TO_None = 0,
        PAD_TO_4x3 = 1,
        PAD_TO_16x9 = 2
    }

    public enum MFRateDirection
    {
        Forward = 0,
        Reverse
    }

    public enum MFVideoTransferMatrix
    {
        BT601 = 2,
        BT709 = 1,
        ForceDWORD = 0x7fffffff,
        Last = 4,
        SMPTE240M = 3,
        Unknown = 0
    }

    public enum MFVideoLighting
    {
        Bright = 1,
        Dark = 4,
        Dim = 3,
        ForceDWORD = 0x7fffffff,
        Last = 5,
        Office = 2,
        Unknown = 0
    }

    public enum MFNominalRange
    {
        MFNominalRange_Unknown = 0,
        MFNominalRange_Normal = 1,
        MFNominalRange_Wide = 2,

        MFNominalRange_0_255 = 1,
        MFNominalRange_16_235 = 2,
        MFNominalRange_48_208 = 3,
        MFNominalRange_64_127 = 4,

        MFNominalRange_Last,
        MFNominalRange_ForceDWORD = 0x7fffffff,
    }

    public enum MFServiceLookUpType
    {
        UPSTREAM = 0,
        UPSTREAM_DIRECT = (UPSTREAM + 1),
        DOWNSTREAM = (UPSTREAM_DIRECT + 1),
        DOWNSTREAM_DIRECT = (DOWNSTREAM + 1),
        ALL = (DOWNSTREAM_DIRECT + 1),
        GLOBAL = (ALL + 1)
    }

    [Flags]
    public enum MFVideoAspectRatioMode
    {
        None = 0x00000000,
        PreservePicture = 0x00000001,
        PreservePixel = 0x00000002,
        NonLinearStretch = 0x00000004,
        Mask = 0x00000007
    }

    [Flags]
    public enum MFVideoRenderPrefs
    {
        None = 0,
        DoNotRenderBorder = 0x00000001,
        DoNotClipToDevice = 0x00000002,
        AllowOutputThrottling = 0x00000004,
        ForceOutputThrottling = 0x00000008,
        ForceBatching = 0x00000010,
        AllowBatching = 0x00000020,
        ForceScaling = 0x00000040,
        AllowScaling = 0x00000080,
        DoNotRepaintOnStop = 0x00000100,
        Mask = 0x000001ff,
    }

    [Flags]
    public enum MFClockCharacteristicsFlags
    {
        None = 0,
        Frequency10Mhz = 0x2,
        AlwaysRunning = 0x4,
        IsSystemClock = 0x8
    }

    public enum MFVideoSrcContentHintFlags
    {
        None = 0,
        F16x9 = 1,
        F235_1 = 2
    }

    [Flags]
    public enum MFClockRelationalFlags
    {
        None = 0,
        JitterNeverAhead = 0x1
    }

    public enum MFClockState
    {
        Invalid,
        Running,
        Stopped,
        Paused
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct MFClockProperties
    {
        public long qwCorrelationRate;
        public Guid guidClockId;
        public MFClockRelationalFlags dwClockFlags;
        public long qwClockFrequency;
        public int dwClockTolerance;
        public int dwClockJitter;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MFTOutputDataBuffer
    {
        public int dwStreamID;
        public IntPtr pSample; // Doesn't release correctly when marshaled as IMFSample
        public MFTOutputDataBufferFlags dwStatus;
        [MarshalAs(UnmanagedType.Interface)]
        public IMFCollection pEvents;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class MfFloat
    {
        private float Value;

        public MfFloat(float v)
        {
            Value = v;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static implicit operator float(MfFloat l)
        {
            return l.Value;
        }

        public static implicit operator MfFloat(float l)
        {
            return new MfFloat(l);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class MFInt
    {
        protected int m_value;

        public MFInt(int v)
        {
            m_value = v;
        }

        public int GetValue()
        {
            return m_value;
        }

        // While I *could* enable this code, it almost certainly won't do what you
        // think it will.  Generally you don't want to create a *new* instance of
        // MFInt and assign a value to it.  You want to assign a value to an
        // existing instance.  In order to do this automatically, .Net would have
        // to support overloading operator =.  But since it doesn't, use Assign()

        //public static implicit operator MFInt(int f)
        //{
        //    return new MFInt(f);
        //}

        public static implicit operator int(MFInt f)
        {
            return f.m_value;
        }

        public int ToInt32()
        {
            return m_value;
        }

        public void Assign(int f)
        {
            m_value = f;
        }

        public override string ToString()
        {
            return m_value.ToString();
        }

        public override int GetHashCode()
        {
            return m_value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is MFInt)
            {
                return ((MFInt)obj).m_value == m_value;
            }

            return Convert.ToInt32(obj) == m_value;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MFRatio
    {
        public int Numerator;
        public int Denominator;

        public MFRatio(int n, int d)
        {
            Numerator = n;
            Denominator = d;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public class MFVideoArea
    {
        public MFOffset OffsetX;
        public MFOffset OffsetY;
        public Size Area;

        public MFVideoArea()
        {
            OffsetX = new MFOffset();
            OffsetY = new MFOffset();
        }

        public MFVideoArea(float x, float y, int width, int height)
        {
            OffsetX = new MFOffset(x);
            OffsetY = new MFOffset(y);
            Area = new Size(width, height);
        }

        public void MakeArea(float x, float y, int width, int height)
        {
            OffsetX.MakeOffset(x);
            OffsetY.MakeOffset(y);
            Area.Width = width;
            Area.Height = height;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public class MFOffset
    {
        public short fract;
        public short Value;

        public MFOffset()
        {
        }

        public MFOffset(float v)
        {
            Value = (short)v;
            fract = (short)(65536 * (v - Value));
        }

        public void MakeOffset(float v)
        {
            Value = (short)v;
            fract = (short)(65536 * (v - Value));
        }

        public float GetOffset()
        {
            return ((float)Value) + (((float)fract) / 65536.0f);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct MFVideoInfo
    {
        public int dwWidth;
        public int dwHeight;
        public MFRatio PixelAspectRatio;
        public MFVideoChromaSubsampling SourceChromaSubsampling;
        public MFVideoInterlaceMode InterlaceMode;
        public MFVideoTransferFunction TransferFunction;
        public MFVideoPrimaries ColorPrimaries;
        public MFVideoTransferMatrix TransferMatrix;
        public MFVideoLighting SourceLighting;
        public MFRatio FramesPerSecond;
        public MFNominalRange NominalRange;
        public MFVideoArea GeometricAperture;
        public MFVideoArea MinimumDisplayAperture;
        public MFVideoArea PanScanAperture;
        public MFVideoFlags VideoFlags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct MFVideoCompressedInfo
    {
        public long AvgBitrate;
        public long AvgBitErrorRate;
        public int MaxKeyFrameSpacing;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MFVideoSurfaceInfo
    {
        public int Format;
        public int PaletteEntries;
        public MFPaletteEntry[] Palette;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public class MFVideoFormat
    {
        public int dwSize;
        public MFVideoInfo videoInfo;
        public Guid guidFormat;
        public MFVideoCompressedInfo compressedInfo;
        public MFVideoSurfaceInfo surfaceInfo;
    }

    [StructLayout(LayoutKind.Explicit)]
    public class ConstPropVariant : IDisposable
    {
        public enum VariantType : short
        {
            None = 0,
            Short = 2,
            Int32 = 3,
            Float = 4,
            Double = 5,
            IUnknown = 13,
            UByte = 17,
            UShort = 18,
            UInt32 = 19,
            Int64 = 20,
            UInt64 = 21,
            String = 31,
            Guid = 72,
            Blob = 0x1000 + 17,
            StringArray = 0x1000 + 31
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct Blob
        {
            public int cbSize;
            public IntPtr pBlobData;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct CALPWstr
        {
            public int cElems;
            public IntPtr pElems;
        }

        #region Member variables

        [FieldOffset(0)]
        protected VariantType type;

        [FieldOffset(2)]
        protected short reserved1;

        [FieldOffset(4)]
        protected short reserved2;

        [FieldOffset(6)]
        protected short reserved3;

        [FieldOffset(8)]
        protected short iVal;

        [FieldOffset(8)]
        protected ushort uiVal;

        [FieldOffset(8)]
        protected byte bVal;

        [FieldOffset(8)]
        protected int intValue;

        [FieldOffset(8)]
        protected uint uintVal;

        [FieldOffset(8)]
        protected float fltVal;

        [FieldOffset(8)]
        protected long longValue;

        [FieldOffset(8)]
        protected ulong ulongValue;

        [FieldOffset(8)]
        protected double doubleValue;

        [FieldOffset(8)]
        protected Blob blobValue;

        [FieldOffset(8)]
        protected IntPtr ptr;

        [FieldOffset(8)]
        protected CALPWstr calpwstrVal;

        #endregion

        public ConstPropVariant()
        {
            type = VariantType.None;
        }

        protected ConstPropVariant(VariantType v)
        {
            type = v;
        }

        public static explicit operator string(ConstPropVariant f)
        {
            return f.GetString();
        }

        public static explicit operator string[](ConstPropVariant f)
        {
            return f.GetStringArray();
        }

        public static explicit operator byte(ConstPropVariant f)
        {
            return f.GetUByte();
        }

        public static explicit operator short(ConstPropVariant f)
        {
            return f.GetShort();
        }

        
        public static explicit operator ushort(ConstPropVariant f)
        {
            return f.GetUShort();
        }

        public static explicit operator int(ConstPropVariant f)
        {
            return f.GetInt();
        }

        
        public static explicit operator uint(ConstPropVariant f)
        {
            return f.GetUInt();
        }

        public static explicit operator float(ConstPropVariant f)
        {
            return f.GetFloat();
        }

        public static explicit operator double(ConstPropVariant f)
        {
            return f.GetDouble();
        }

        public static explicit operator long(ConstPropVariant f)
        {
            return f.GetLong();
        }

        
        public static explicit operator ulong(ConstPropVariant f)
        {
            return f.GetULong();
        }

        public static explicit operator Guid(ConstPropVariant f)
        {
            return f.GetGuid();
        }

        public static explicit operator byte[](ConstPropVariant f)
        {
            return f.GetBlob();
        }

        // I decided not to do implicits since perf is likely to be
        // better recycling the PropVariant, and the only way I can
        // see to support Implicit is to create a new PropVariant.
        // Also, since I can't free the previous instance, IUnknowns
        // will linger until the GC cleans up.  Not what I think I
        // want.

        public MFAttributeType GetMFAttributeType()
        {
            switch (type)
            {
                case VariantType.None:
                case VariantType.UInt32:
                case VariantType.UInt64:
                case VariantType.Double:
                case VariantType.Guid:
                case VariantType.String:
                case VariantType.Blob:
                case VariantType.IUnknown:
                    {
                        return (MFAttributeType)type;
                    }
                default:
                    {
                        throw new Exception("Type is not a MFAttributeType");
                    }
            }
        }

        public VariantType GetVariantType()
        {
            return type;
        }

        public string[] GetStringArray()
        {
            if (type == VariantType.StringArray)
            {
                string[] sa;

                int iCount = calpwstrVal.cElems;
                sa = new string[iCount];

                for (int x = 0; x < iCount; x++)
                {
                    sa[x] = Marshal.PtrToStringUni(Marshal.ReadIntPtr(calpwstrVal.pElems, x * IntPtr.Size));
                }

                return sa;
            }
            throw new ArgumentException("PropVariant contents not a string array");
        }

        public string GetString()
        {
            if (type == VariantType.String)
            {
                return Marshal.PtrToStringUni(ptr);
            }
            throw new ArgumentException("PropVariant contents not a string");
        }

        public byte GetUByte()
        {
            if (type == VariantType.UByte)
            {
                return bVal;
            }
            throw new ArgumentException("PropVariant contents not a byte");
        }

        public short GetShort()
        {
            if (type == VariantType.Short)
            {
                return iVal;
            }
            throw new ArgumentException("PropVariant contents not an Short");
        }

        
        public ushort GetUShort()
        {
            if (type == VariantType.UShort)
            {
                return uiVal;
            }
            throw new ArgumentException("PropVariant contents not an UShort");
        }

        public int GetInt()
        {
            if (type == VariantType.Int32)
            {
                return intValue;
            }
            throw new ArgumentException("PropVariant contents not an int32");
        }

        
        public uint GetUInt()
        {
            if (type == VariantType.UInt32)
            {
                return uintVal;
            }
            throw new ArgumentException("PropVariant contents not an uint32");
        }

        public long GetLong()
        {
            if (type == VariantType.Int64)
            {
                return longValue;
            }
            throw new ArgumentException("PropVariant contents not an int64");
        }

        
        public ulong GetULong()
        {
            if (type == VariantType.UInt64)
            {
                return ulongValue;
            }
            throw new ArgumentException("PropVariant contents not an uint64");
        }

        public float GetFloat()
        {
            if (type == VariantType.Float)
            {
                return fltVal;
            }
            throw new ArgumentException("PropVariant contents not a Float");
        }

        public double GetDouble()
        {
            if (type == VariantType.Double)
            {
                return doubleValue;
            }
            throw new ArgumentException("PropVariant contents not a double");
        }

        public Guid GetGuid()
        {
            if (type == VariantType.Guid)
            {
                return (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));
            }
            throw new ArgumentException("PropVariant contents not a Guid");
        }

        public byte[] GetBlob()
        {
            if (type == VariantType.Blob)
            {
                byte[] b = new byte[blobValue.cbSize];

                Marshal.Copy(blobValue.pBlobData, b, 0, blobValue.cbSize);

                return b;
            }
            throw new ArgumentException("PropVariant contents are not a Blob");
        }

        public object GetIUnknown()
        {
            if (type == VariantType.IUnknown)
            {
                return Marshal.GetObjectForIUnknown(ptr);
            }
            throw new ArgumentException("PropVariant contents not an IUnknown");
        }

        public override string ToString()
        {
            // This method is primarily intended for debugging so that a readable string will show
            // up in the output window
            string sRet;

            switch (type)
            {
                case VariantType.None:
                    {
                        sRet = "<Empty>";
                        break;
                    }

                case VariantType.Blob:
                    {
                        const string FormatString = "x2"; // Hex 2 digit format
                        const int MaxEntries = 16;

                        byte[] blob = GetBlob();

                        // Number of bytes we're going to format
                        int n = Math.Min(MaxEntries, blob.Length);

                        if (n == 0)
                        {
                            sRet = "<Empty Array>";
                        }
                        else
                        {
                            // Only format the first MaxEntries bytes
                            sRet = blob[0].ToString(FormatString);
                            for (int i = 1; i < n; i++)
                            {
                                sRet += ',' + blob[i].ToString(FormatString);
                            }

                            // If the string is longer, add an indicator
                            if (blob.Length > n)
                            {
                                sRet += "...";
                            }
                        }
                        break;
                    }

                case VariantType.Float:
                    {
                        sRet = GetFloat().ToString();
                        break;
                    }

                case VariantType.Double:
                    {
                        sRet = GetDouble().ToString();
                        break;
                    }

                case VariantType.Guid:
                    {
                        sRet = GetGuid().ToString();
                        break;
                    }

                case VariantType.IUnknown:
                    {
                        sRet = GetIUnknown().ToString();
                        break;
                    }

                case VariantType.String:
                    {
                        sRet = GetString();
                        break;
                    }

                case VariantType.Short:
                    {
                        sRet = GetShort().ToString();
                        break;
                    }

                case VariantType.UByte:
                    {
                        sRet = GetUByte().ToString();
                        break;
                    }

                case VariantType.UShort:
                    {
                        sRet = GetUShort().ToString();
                        break;
                    }

                case VariantType.Int32:
                    {
                        sRet = GetInt().ToString();
                        break;
                    }

                case VariantType.UInt32:
                    {
                        sRet = GetUInt().ToString();
                        break;
                    }

                case VariantType.Int64:
                    {
                        sRet = GetLong().ToString();
                        break;
                    }

                case VariantType.UInt64:
                    {
                        sRet = GetULong().ToString();
                        break;
                    }

                case VariantType.StringArray:
                    {
                        sRet = "";
                        foreach (string entry in GetStringArray())
                        {
                            sRet += (sRet.Length == 0 ? "\"" : ",\"") + entry + '\"';
                        }
                        break;
                    }
                default:
                    {
                        sRet = base.ToString();
                        break;
                    }
            }

            return sRet;
        }

        public override int GetHashCode()
        {
            // Give a (slightly) better hash value in case someone uses PropVariants
            // in a hash table.
            int iRet;

            switch (type)
            {
                case VariantType.None:
                    {
                        iRet = base.GetHashCode();
                        break;
                    }

                case VariantType.Blob:
                    {
                        iRet = GetBlob().GetHashCode();
                        break;
                    }

                case VariantType.Float:
                    {
                        iRet = GetFloat().GetHashCode();
                        break;
                    }

                case VariantType.Double:
                    {
                        iRet = GetDouble().GetHashCode();
                        break;
                    }

                case VariantType.Guid:
                    {
                        iRet = GetGuid().GetHashCode();
                        break;
                    }

                case VariantType.IUnknown:
                    {
                        iRet = GetIUnknown().GetHashCode();
                        break;
                    }

                case VariantType.String:
                    {
                        iRet = GetString().GetHashCode();
                        break;
                    }

                case VariantType.UByte:
                    {
                        iRet = GetUByte().GetHashCode();
                        break;
                    }

                case VariantType.Short:
                    {
                        iRet = GetShort().GetHashCode();
                        break;
                    }

                case VariantType.UShort:
                    {
                        iRet = GetUShort().GetHashCode();
                        break;
                    }

                case VariantType.Int32:
                    {
                        iRet = GetInt().GetHashCode();
                        break;
                    }

                case VariantType.UInt32:
                    {
                        iRet = GetUInt().GetHashCode();
                        break;
                    }

                case VariantType.Int64:
                    {
                        iRet = GetLong().GetHashCode();
                        break;
                    }

                case VariantType.UInt64:
                    {
                        iRet = GetULong().GetHashCode();
                        break;
                    }

                case VariantType.StringArray:
                    {
                        iRet = GetStringArray().GetHashCode();
                        break;
                    }
                default:
                    {
                        iRet = base.GetHashCode();
                        break;
                    }
            }

            return iRet;
        }

        public override bool Equals(object obj)
        {
            bool bRet;
            PropVariant p = obj as PropVariant;

            if ((((object)p) == null) || (p.type != type))
            {
                bRet = false;
            }
            else
            {
                switch (type)
                {
                    case VariantType.None:
                        {
                            bRet = true;
                            break;
                        }

                    case VariantType.Blob:
                        {
                            byte[] b1;
                            byte[] b2;

                            b1 = GetBlob();
                            b2 = p.GetBlob();

                            if (b1.Length == b2.Length)
                            {
                                bRet = true;
                                for (int x = 0; x < b1.Length; x++)
                                {
                                    if (b1[x] != b2[x])
                                    {
                                        bRet = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                bRet = false;
                            }
                            break;
                        }

                    case VariantType.Float:
                        {
                            bRet = GetFloat() == p.GetFloat();
                            break;
                        }

                    case VariantType.Double:
                        {
                            bRet = GetDouble() == p.GetDouble();
                            break;
                        }

                    case VariantType.Guid:
                        {
                            bRet = GetGuid() == p.GetGuid();
                            break;
                        }

                    case VariantType.IUnknown:
                        {
                            bRet = GetIUnknown() == p.GetIUnknown();
                            break;
                        }

                    case VariantType.String:
                        {
                            bRet = GetString() == p.GetString();
                            break;
                        }

                    case VariantType.UByte:
                        {
                            bRet = GetUByte() == p.GetUByte();
                            break;
                        }

                    case VariantType.Short:
                        {
                            bRet = GetShort() == p.GetShort();
                            break;
                        }

                    case VariantType.UShort:
                        {
                            bRet = GetUShort() == p.GetUShort();
                            break;
                        }

                    case VariantType.Int32:
                        {
                            bRet = GetInt() == p.GetInt();
                            break;
                        }

                    case VariantType.UInt32:
                        {
                            bRet = GetUInt() == p.GetUInt();
                            break;
                        }

                    case VariantType.Int64:
                        {
                            bRet = GetLong() == p.GetLong();
                            break;
                        }

                    case VariantType.UInt64:
                        {
                            bRet = GetULong() == p.GetULong();
                            break;
                        }

                    case VariantType.StringArray:
                        {
                            string[] sa1;
                            string[] sa2;

                            sa1 = GetStringArray();
                            sa2 = p.GetStringArray();

                            if (sa1.Length == sa2.Length)
                            {
                                bRet = true;
                                for (int x = 0; x < sa1.Length; x++)
                                {
                                    if (sa1[x] != sa2[x])
                                    {
                                        bRet = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                bRet = false;
                            }
                            break;
                        }
                    default:
                        {
                            bRet = base.Equals(obj);
                            break;
                        }
                }
            }

            return bRet;
        }

        public static bool operator ==(ConstPropVariant pv1, ConstPropVariant pv2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(pv1, pv2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)pv1 == null) || ((object)pv2 == null))
            {
                return false;
            }

            return pv1.Equals(pv2);
        }

        public static bool operator !=(ConstPropVariant pv1, ConstPropVariant pv2)
        {
            return !(pv1 == pv2);
        }

        #region IDisposable Members

        public void Dispose()
        {
            // If we are a ConstPropVariant, we must *not* call PropVariantClear.  That
            // would release the *caller's* copy of the data, which would probably make
            // him cranky.  If we are a PropVariant, the PropVariant.Dispose gets called
            // as well, which *does* do a PropVariantClear.
            type = VariantType.None;
#if DEBUG
            longValue = 0;
#endif
        }

        #endregion
    }

    [StructLayout(LayoutKind.Explicit)]
    public class PropVariant : ConstPropVariant
    {
        #region Declarations

        [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false), SuppressUnmanagedCodeSecurity]
        protected static extern void PropVariantCopy(
            [Out, MarshalAs(UnmanagedType.LPStruct)] PropVariant pvarDest,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant pvarSource
            );

        [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false), SuppressUnmanagedCodeSecurity]
        protected static extern void PropVariantClear(
            [In, MarshalAs(UnmanagedType.LPStruct)] PropVariant pvar
            );

        #endregion

        public PropVariant()
            : base(VariantType.None)
        {
        }

        public PropVariant(string value)
            : base(VariantType.String)
        {
            ptr = Marshal.StringToCoTaskMemUni(value);
        }

        public PropVariant(string[] value)
            : base(VariantType.StringArray)
        {
            calpwstrVal.cElems = value.Length;
            calpwstrVal.pElems = Marshal.AllocCoTaskMem(IntPtr.Size * value.Length);

            for (int x = 0; x < value.Length; x++)
            {
                Marshal.WriteIntPtr(calpwstrVal.pElems, x * IntPtr.Size, Marshal.StringToCoTaskMemUni(value[x]));
            }
        }

        public PropVariant(byte value)
            : base(VariantType.UByte)
        {
            bVal = value;
        }

        public PropVariant(short value)
            : base(VariantType.Short)
        {
            iVal = value;
        }

        
        public PropVariant(ushort value)
            : base(VariantType.UShort)
        {
            uiVal = value;
        }

        public PropVariant(int value)
            : base(VariantType.Int32)
        {
            intValue = value;
        }

        
        public PropVariant(uint value)
            : base(VariantType.UInt32)
        {
            uintVal = value;
        }

        public PropVariant(float value)
            : base(VariantType.Float)
        {
            fltVal = value;
        }

        public PropVariant(double value)
            : base(VariantType.Double)
        {
            doubleValue = value;
        }

        public PropVariant(long value)
            : base(VariantType.Int64)
        {
            longValue = value;
        }

        
        public PropVariant(ulong value)
            : base(VariantType.UInt64)
        {
            ulongValue = value;
        }

        public PropVariant(Guid value)
            : base(VariantType.Guid)
        {
            ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(value));
            Marshal.StructureToPtr(value, ptr, false);
        }

        public PropVariant(byte[] value)
            : base(VariantType.Blob)
        {
            blobValue.cbSize = value.Length;
            blobValue.pBlobData = Marshal.AllocCoTaskMem(value.Length);
            Marshal.Copy(value, 0, blobValue.pBlobData, value.Length);
        }

        public PropVariant(object value)
            : base(VariantType.IUnknown)
        {
            ptr = Marshal.GetIUnknownForObject(value);
        }

        public PropVariant(IntPtr value)
        {
            Marshal.PtrToStructure(value, this);
        }

        public PropVariant(ConstPropVariant value)
        {
            if (value != null)
            {
                PropVariantCopy(this, value);
            }
            else
            {
                throw new NullReferenceException("null passed to PropVariant constructor");
            }
        }

        ~PropVariant()
        {
            Clear();
        }

        public void Copy(PropVariant pval)
        {
            if (pval == null)
            {
                throw new Exception("Null PropVariant sent to Copy");
            }

            pval.Clear();

            PropVariantCopy(pval, this);
        }

        public void Clear()
        {
            PropVariantClear(this);
        }

        #region IDisposable Members

        new public void Dispose()
        {
            Clear();
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    internal class PVMarshaler : ICustomMarshaler
    {
        // The managed object passed in to MarshalManagedToNative
        protected PropVariant m_prop;

        public IntPtr MarshalManagedToNative(object managedObj)
        {
            IntPtr p;

            // Cast the object back to a PropVariant
            m_prop = managedObj as PropVariant;

            if (m_prop != null)
            {
                // Release any memory currently allocated
                m_prop.Clear();

                // Create an appropriately sized buffer, blank it, and send it to
                // the marshaler to make the COM call with.
                int iSize = GetNativeDataSize();
                p = Marshal.AllocCoTaskMem(iSize);

                if (IntPtr.Size == 4)
                {
                    Marshal.WriteInt64(p, 0);
                    Marshal.WriteInt64(p, 8, 0);
                }
                else
                {
                    Marshal.WriteInt64(p, 0);
                    Marshal.WriteInt64(p, 8, 0);
                    Marshal.WriteInt64(p, 16, 0);
                }
            }
            else
            {
                p = IntPtr.Zero;
            }

            return p;
        }

        // Called just after invoking the COM method.  The IntPtr is the same one that just got returned
        // from MarshalManagedToNative.  The return value is unused.
        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            Marshal.PtrToStructure(pNativeData, m_prop);
            m_prop = null;

            return m_prop;
        }

        public void CleanUpManagedData(object ManagedObj)
        {
            m_prop = null;
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeCoTaskMem(pNativeData);
        }

        // The number of bytes to marshal out
        public int GetNativeDataSize()
        {
            return Marshal.SizeOf(typeof(PropVariant));
        }

        // This method is called by interop to create the custom marshaler.  The (optional)
        // cookie is the value specified in MarshalCookie="asdf", or "" is none is specified.
        public static ICustomMarshaler GetInstance(string cookie)
        {
            return new PVMarshaler();
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public class MFVideoNormalizedRect
    {
        public float left;
        public float top;
        public float right;
        public float bottom;

        public MFVideoNormalizedRect()
        {
            left = 0;
            top = 0;
            right = 1;
            bottom = 1;
        }

        public MFVideoNormalizedRect(float l, float t, float r, float b)
        {
            left = l;
            top = t;
            right = r;
            bottom = b;
        }

        public override string ToString()
        {
            return string.Format("left = {0}, top = {1}, right = {2}, bottom = {3}", left, top, right, bottom);
        }

        public override int GetHashCode()
        {
            return left.GetHashCode() |
                top.GetHashCode() |
                right.GetHashCode() |
                bottom.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is MFVideoNormalizedRect)
            {
                MFVideoNormalizedRect cmp = (MFVideoNormalizedRect)obj;

                return right == cmp.right && bottom == cmp.bottom && left == cmp.left && top == cmp.top;
            }

            return false;
        }

        public bool IsEmpty()
        {
            return (right <= left || bottom <= top);
        }

        public void CopyFrom(MFVideoNormalizedRect from)
        {
            left = from.left;
            top = from.top;
            right = from.right;
            bottom = from.bottom;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MT_CustomVideoPrimaries
    {
        public float fRx;
        public float fRy;
        public float fGx;
        public float fGy;
        public float fBx;
        public float fBy;
        public float fWx;
        public float fWy;
    }

    #endregion

    #region Interfaces

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("FA993888-4383-415A-A930-DD472A8CF6F7")]
    public interface IMFGetService
    {
        [PreserveSig]
        int GetService(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidService,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            out IntPtr ppvObject
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("83E91E85-82C1-4ea7-801D-85DC50B75086")]
    public interface IEVRFilterConfig
    {
        [PreserveSig]
        int SetNumberOfStreams(
            int dwMaxStreams
            );

        [PreserveSig]
        int GetNumberOfStreams(
            out int pdwMaxStreams
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("1F6A9F17-E70B-4E24-8AE4-0B2C3BA7A4AE"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFVideoPositionMapper
    {
        [PreserveSig]
        int MapOutputCoordinateToInputStream(
            [In] float xOut,
            [In] float yOut,
            [In] int dwOutputStreamIndex,
            [In] int dwInputStreamIndex,
            out float pxIn,
            out float pyIn
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("DFDFD197-A9CA-43D8-B341-6AF3503792CD"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFVideoRenderer
    {
        [PreserveSig]
        int InitializeRenderer(
            [In] IMFTransform pVideoMixer,
            [In] IMFVideoPresenter pVideoPresenter
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("5BC8A76B-869A-46A3-9B03-FA218A66AEBE")]
    public interface IMFCollection
    {
        [PreserveSig]
        int GetElementCount(
            out int pcElements
            );

        [PreserveSig]
        int GetElement(
            [In] int dwElementIndex,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppUnkElement
            );

        [PreserveSig]
        int AddElement(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnkElement
            );

        [PreserveSig]
        int RemoveElement(
            [In] int dwElementIndex,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppUnkElement
            );

        [PreserveSig]
        int InsertElementAt(
            [In] int dwIndex,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnknown
            );

        [PreserveSig]
        int RemoveAllElements();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("BF94C121-5B05-4E6F-8000-BA598961414D")]
    public interface IMFTransform
    {
        [PreserveSig]
        int GetStreamLimits(
            [Out] MFInt pdwInputMinimum,
            [Out] MFInt pdwInputMaximum,
            [Out] MFInt pdwOutputMinimum,
            [Out] MFInt pdwOutputMaximum
            );

        [PreserveSig]
        int GetStreamCount(
            [Out] MFInt pcInputStreams,
            [Out] MFInt pcOutputStreams
            );

        [PreserveSig]
        int GetStreamIDs(
            int dwInputIDArraySize,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] int[] pdwInputIDs,
            int dwOutputIDArraySize,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] int[] pdwOutputIDs
            );

        [PreserveSig]
        int GetInputStreamInfo(
            int dwInputStreamID,
            out MFTInputStreamInfo pStreamInfo
            );

        [PreserveSig]
        int GetOutputStreamInfo(
            int dwOutputStreamID,
            out MFTOutputStreamInfo pStreamInfo
            );

        [PreserveSig]
        int GetAttributes(
            [MarshalAs(UnmanagedType.Interface)] out IMFAttributes pAttributes
            );

        [PreserveSig]
        int GetInputStreamAttributes(
            int dwInputStreamID,
            [MarshalAs(UnmanagedType.Interface)] out IMFAttributes pAttributes
            );

        [PreserveSig]
        int GetOutputStreamAttributes(
            int dwOutputStreamID,
            [MarshalAs(UnmanagedType.Interface)] out IMFAttributes pAttributes
            );

        [PreserveSig]
        int DeleteInputStream(
            int dwStreamID
            );

        [PreserveSig]
        int AddInputStreams(
            int cStreams,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] int[] adwStreamIDs
            );

        [PreserveSig]
        int GetInputAvailableType(
            int dwInputStreamID,
            int dwTypeIndex,
            [MarshalAs(UnmanagedType.Interface)] out IMFMediaType ppType
            );

        [PreserveSig]
        int GetOutputAvailableType(
            int dwOutputStreamID,
            int dwTypeIndex,
            [MarshalAs(UnmanagedType.Interface)] out IMFMediaType ppType
            );

        [PreserveSig]
        int SetInputType(
            int dwInputStreamID,
            [In, MarshalAs(UnmanagedType.Interface)] IMFMediaType pType,
            MFTSetTypeFlags dwFlags
            );

        [PreserveSig]
        int SetOutputType(
            int dwOutputStreamID,
            [In, MarshalAs(UnmanagedType.Interface)] IMFMediaType pType,
            MFTSetTypeFlags dwFlags
            );

        [PreserveSig]
        int GetInputCurrentType(
            int dwInputStreamID,
            [MarshalAs(UnmanagedType.Interface)] out IMFMediaType ppType
            );

        [PreserveSig]
        int GetOutputCurrentType(
            int dwOutputStreamID,
            [MarshalAs(UnmanagedType.Interface)] out IMFMediaType ppType
            );

        [PreserveSig]
        int GetInputStatus(
            int dwInputStreamID,
            out MFTInputStatusFlags pdwFlags
            );

        [PreserveSig]
        int GetOutputStatus(
            out MFTOutputStatusFlags pdwFlags
            );

        [PreserveSig]
        int SetOutputBounds(
            long hnsLowerBound,
            long hnsUpperBound
            );

        [PreserveSig]
        int ProcessEvent(
            int dwInputStreamID,
            [In, MarshalAs(UnmanagedType.Interface)] IMFMediaEvent pEvent
            );

        [PreserveSig]
        int ProcessMessage(
            MFTMessageType eMessage,
            IntPtr ulParam
            );

        [PreserveSig]
        int ProcessInput(
            int dwInputStreamID,
            [MarshalAs(UnmanagedType.Interface)] IMFSample pSample,
            int dwFlags // Must be zero
            );

        [PreserveSig]
        int ProcessOutput(
            MFTProcessOutputFlags dwFlags,
            int cOutputBufferCount,
            [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] MFTOutputDataBuffer[] pOutputSamples,
            out ProcessOutputStatus pdwStatus
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("C40A00F2-B93A-4D80-AE8C-5A1C634F58E4"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFSample : IMFAttributes
    {
        #region IMFAttributes methods

        [PreserveSig]
        new int GetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PVMarshaler))] PropVariant pValue
            );

        [PreserveSig]
        new int GetItemType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out MFAttributeType pType
            );

        [PreserveSig]
        new int CompareItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant Value,
            [MarshalAs(UnmanagedType.Bool)] out bool pbResult
            );

        [PreserveSig]
        new int Compare(
            [MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs,
            MFAttributesMatchType MatchType,
            [MarshalAs(UnmanagedType.Bool)] out bool pbResult
            );

        [PreserveSig]
        new int GetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int punValue
            );

        [PreserveSig]
        new int GetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out long punValue
            );

        [PreserveSig]
        new int GetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out double pfValue
            );

        [PreserveSig]
        new int GetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out Guid pguidValue
            );

        [PreserveSig]
        new int GetStringLength(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcchLength
            );

        [PreserveSig]
        new int GetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszValue,
            int cchBufSize,
            out int pcchLength
            );

        [PreserveSig]
        new int GetAllocatedString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [MarshalAs(UnmanagedType.LPWStr)] out string ppwszValue,
            out int pcchLength
            );

        [PreserveSig]
        new int GetBlobSize(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcbBlobSize
            );

        [PreserveSig]
        new int GetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pBuf,
            int cbBufSize,
            out int pcbBlobSize
            );

        // Use GetBlob instead of this
        [PreserveSig]
        new int GetAllocatedBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out IntPtr ip,  // Read w/Marshal.Copy, Free w/Marshal.FreeCoTaskMem
            out int pcbSize
            );

        [PreserveSig]
        new int GetUnknown(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppv
            );

        [PreserveSig]
        new int SetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant Value
            );

        [PreserveSig]
        new int DeleteItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey
            );

        [PreserveSig]
        new int DeleteAllItems();

        [PreserveSig]
        new int SetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            int unValue
            );

        [PreserveSig]
        new int SetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            long unValue
            );

        [PreserveSig]
        new int SetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            double fValue
            );

        [PreserveSig]
        new int SetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidValue
            );

        [PreserveSig]
        new int SetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string wszValue
            );

        [PreserveSig]
        new int SetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf,
            int cbBufSize
            );

        [PreserveSig]
        new int SetUnknown(
            [MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnknown
            );

        [PreserveSig]
        new int LockStore();

        [PreserveSig]
        new int UnlockStore();

        [PreserveSig]
        new int GetCount(
            out int pcItems
            );

        [PreserveSig]
        new int GetItemByIndex(
            int unIndex,
            out Guid pguidKey,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PVMarshaler))] PropVariant pValue
            );

        [PreserveSig]
        new int CopyAllItems(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest
            );

        #endregion

        [PreserveSig]
        int GetSampleFlags(
            out int pdwSampleFlags // Must be zero
            );

        [PreserveSig]
        int SetSampleFlags(
            [In] int dwSampleFlags // Must be zero
            );

        [PreserveSig]
        int GetSampleTime(
            out long phnsSampleTime
            );

        [PreserveSig]
        int SetSampleTime(
            [In] long hnsSampleTime
            );

        [PreserveSig]
        int GetSampleDuration(
            out long phnsSampleDuration
            );

        [PreserveSig]
        int SetSampleDuration(
            [In] long hnsSampleDuration
            );

        [PreserveSig]
        int GetBufferCount(
            out int pdwBufferCount
            );

        [PreserveSig]
        int GetBufferByIndex(
            [In] int dwIndex,
            [MarshalAs(UnmanagedType.Interface)] out IMFMediaBuffer ppBuffer
            );

        [PreserveSig]
        int ConvertToContiguousBuffer(
            [MarshalAs(UnmanagedType.Interface)] out IMFMediaBuffer ppBuffer
            );

        [PreserveSig]
        int AddBuffer(
            [In, MarshalAs(UnmanagedType.Interface)] IMFMediaBuffer pBuffer
            );

        [PreserveSig]
        int RemoveBufferByIndex(
            [In] int dwIndex
            );

        [PreserveSig]
        int RemoveAllBuffers();

        [PreserveSig]
        int GetTotalLength(
            out int pcbTotalLength
            );

        [PreserveSig]
        int CopyToBuffer(
            [In, MarshalAs(UnmanagedType.Interface)] IMFMediaBuffer pBuffer
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("045FA593-8799-42B8-BC8D-8968C6453507")]
    public interface IMFMediaBuffer
    {
        [PreserveSig]
        int Lock(
            out IntPtr ppbBuffer,
            out int pcbMaxLength,
            out int pcbCurrentLength
            );

        [PreserveSig]
        int Unlock();

        [PreserveSig]
        int GetCurrentLength(
            out int pcbCurrentLength
            );

        [PreserveSig]
        int SetCurrentLength(
            [In] int cbCurrentLength
            );

        [PreserveSig]
        int GetMaxLength(
            out int pcbMaxLength
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("DF598932-F10C-4E39-BBA2-C308F101DAA3"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEvent : IMFAttributes
    {
        #region IMFAttributes methods

        [PreserveSig]
        new int GetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PVMarshaler))] PropVariant pValue
            );

        [PreserveSig]
        new int GetItemType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out MFAttributeType pType
            );

        [PreserveSig]
        new int CompareItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant Value,
            [MarshalAs(UnmanagedType.Bool)] out bool pbResult
            );

        [PreserveSig]
        new int Compare(
            [MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs,
            MFAttributesMatchType MatchType,
            [MarshalAs(UnmanagedType.Bool)] out bool pbResult
            );

        [PreserveSig]
        new int GetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int punValue
            );

        [PreserveSig]
        new int GetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out long punValue
            );

        [PreserveSig]
        new int GetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out double pfValue
            );

        [PreserveSig]
        new int GetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out Guid pguidValue
            );

        [PreserveSig]
        new int GetStringLength(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcchLength
            );

        [PreserveSig]
        new int GetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszValue,
            int cchBufSize,
            out int pcchLength
            );

        [PreserveSig]
        new int GetAllocatedString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [MarshalAs(UnmanagedType.LPWStr)] out string ppwszValue,
            out int pcchLength
            );

        [PreserveSig]
        new int GetBlobSize(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcbBlobSize
            );

        [PreserveSig]
        new int GetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pBuf,
            int cbBufSize,
            out int pcbBlobSize
            );

        // Use GetBlob instead of this
        [PreserveSig]
        new int GetAllocatedBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out IntPtr ip,  // Read w/Marshal.Copy, Free w/Marshal.FreeCoTaskMem
            out int pcbSize
            );

        [PreserveSig]
        new int GetUnknown(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppv
            );

        [PreserveSig]
        new int SetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant Value
            );

        [PreserveSig]
        new int DeleteItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey
            );

        [PreserveSig]
        new int DeleteAllItems();

        [PreserveSig]
        new int SetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            int unValue
            );

        [PreserveSig]
        new int SetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            long unValue
            );

        [PreserveSig]
        new int SetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            double fValue
            );

        [PreserveSig]
        new int SetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidValue
            );

        [PreserveSig]
        new int SetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string wszValue
            );

        [PreserveSig]
        new int SetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf,
            int cbBufSize
            );

        [PreserveSig]
        new int SetUnknown(
            [MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnknown
            );

        [PreserveSig]
        new int LockStore();

        [PreserveSig]
        new int UnlockStore();

        [PreserveSig]
        new int GetCount(
            out int pcItems
            );

        [PreserveSig]
        new int GetItemByIndex(
            int unIndex,
            out Guid pguidKey,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PVMarshaler))] PropVariant pValue
            );

        [PreserveSig]
        new int CopyAllItems(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest
            );

        #endregion

        [PreserveSig]
        int GetType(
            out MediaEventType pmet
            );

        [PreserveSig]
        int GetExtendedType(
            out Guid pguidExtendedType
            );

        [PreserveSig]
        int GetStatus(
            [MarshalAs(UnmanagedType.Error)] out int phrStatus
            );

        [PreserveSig]
        int GetValue(
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PVMarshaler))] PropVariant pvValue
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("29AFF080-182A-4A5D-AF3B-448F3A6346CB")]
    public interface IMFVideoPresenter : IMFClockStateSink
    {
        #region IMFClockStateSink

        [PreserveSig]
        new int OnClockStart(
            [In] long hnsSystemTime,
            [In] long llClockStartOffset
            );

        [PreserveSig]
        new int OnClockStop(
            [In] long hnsSystemTime
            );

        [PreserveSig]
        new int OnClockPause(
            [In] long hnsSystemTime
            );

        [PreserveSig]
        new int OnClockRestart(
            [In] long hnsSystemTime
            );

        [PreserveSig]
        new int OnClockSetRate(
            [In] long hnsSystemTime,
            [In] float flRate
            );

        #endregion

        [PreserveSig]
        int ProcessMessage(
            MFVPMessageType eMessage,
            IntPtr ulParam
            );

        [PreserveSig]
        int GetCurrentMediaType(
            [MarshalAs(UnmanagedType.Interface)] out IMFVideoMediaType ppMediaType
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("F6696E82-74F7-4F3D-A178-8A5E09C3659F")]
    public interface IMFClockStateSink
    {
        [PreserveSig]
        int OnClockStart(
            [In] long hnsSystemTime,
            [In] long llClockStartOffset
            );

        [PreserveSig]
        int OnClockStop(
            [In] long hnsSystemTime
            );

        [PreserveSig]
        int OnClockPause(
            [In] long hnsSystemTime
            );

        [PreserveSig]
        int OnClockRestart(
            [In] long hnsSystemTime
            );

        [PreserveSig]
        int OnClockSetRate(
            [In] long hnsSystemTime,
            [In] float flRate
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("2CD2D921-C447-44A7-A13C-4ADABFC247E3"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFAttributes
    {
        [PreserveSig]
        int GetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PVMarshaler))] PropVariant pValue
            );

        [PreserveSig]
        int GetItemType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out MFAttributeType pType
            );

        [PreserveSig]
        int CompareItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant Value,
            [MarshalAs(UnmanagedType.Bool)] out bool pbResult
            );

        [PreserveSig]
        int Compare(
            [MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs,
            MFAttributesMatchType MatchType,
            [MarshalAs(UnmanagedType.Bool)] out bool pbResult
            );

        [PreserveSig]
        int GetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int punValue
            );

        [PreserveSig]
        int GetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out long punValue
            );

        [PreserveSig]
        int GetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out double pfValue
            );

        [PreserveSig]
        int GetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out Guid pguidValue
            );

        [PreserveSig]
        int GetStringLength(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcchLength
            );

        [PreserveSig]
        int GetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszValue,
            int cchBufSize,
            out int pcchLength
            );

        [PreserveSig]
        int GetAllocatedString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [MarshalAs(UnmanagedType.LPWStr)] out string ppwszValue,
            out int pcchLength
            );

        [PreserveSig]
        int GetBlobSize(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcbBlobSize
            );

        [PreserveSig]
        int GetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pBuf,
            int cbBufSize,
            out int pcbBlobSize
            );

        // Use GetBlob instead of this
        [PreserveSig]
        int GetAllocatedBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out IntPtr ip,  // Read w/Marshal.Copy, Free w/Marshal.FreeCoTaskMem
            out int pcbSize
            );

        [PreserveSig]
        int GetUnknown(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppv
            );

        [PreserveSig]
        int SetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant Value
            );

        [PreserveSig]
        int DeleteItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey
            );

        [PreserveSig]
        int DeleteAllItems();

        [PreserveSig]
        int SetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            int unValue
            );

        [PreserveSig]
        int SetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            long unValue
            );

        [PreserveSig]
        int SetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            double fValue
            );

        [PreserveSig]
        int SetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidValue
            );

        [PreserveSig]
        int SetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string wszValue
            );

        [PreserveSig]
        int SetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf,
            int cbBufSize
            );

        [PreserveSig]
        int SetUnknown(
            [MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnknown
            );

        [PreserveSig]
        int LockStore();

        [PreserveSig]
        int UnlockStore();

        [PreserveSig]
        int GetCount(
            out int pcItems
            );

        [PreserveSig]
        int GetItemByIndex(
            int unIndex,
            out Guid pguidKey,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PVMarshaler))] PropVariant pValue
            );

        [PreserveSig]
        int CopyAllItems(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("44AE0FA8-EA31-4109-8D2E-4CAE4997C555")]
    public interface IMFMediaType : IMFAttributes
    {
        #region IMFAttributes methods

        [PreserveSig]
        new int GetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PVMarshaler))] PropVariant pValue
            );

        [PreserveSig]
        new int GetItemType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out MFAttributeType pType
            );

        [PreserveSig]
        new int CompareItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant Value,
            [MarshalAs(UnmanagedType.Bool)] out bool pbResult
            );

        [PreserveSig]
        new int Compare(
            [MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs,
            MFAttributesMatchType MatchType,
            [MarshalAs(UnmanagedType.Bool)] out bool pbResult
            );

        [PreserveSig]
        new int GetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int punValue
            );

        [PreserveSig]
        new int GetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out long punValue
            );

        [PreserveSig]
        new int GetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out double pfValue
            );

        [PreserveSig]
        new int GetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out Guid pguidValue
            );

        [PreserveSig]
        new int GetStringLength(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcchLength
            );

        [PreserveSig]
        new int GetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszValue,
            int cchBufSize,
            out int pcchLength
            );

        [PreserveSig]
        new int GetAllocatedString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [MarshalAs(UnmanagedType.LPWStr)] out string ppwszValue,
            out int pcchLength
            );

        [PreserveSig]
        new int GetBlobSize(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcbBlobSize
            );

        [PreserveSig]
        new int GetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pBuf,
            int cbBufSize,
            out int pcbBlobSize
            );

        // Use GetBlob instead of this
        [PreserveSig]
        new int GetAllocatedBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out IntPtr ip,  // Read w/Marshal.Copy, Free w/Marshal.FreeCoTaskMem
            out int pcbSize
            );

        [PreserveSig]
        new int GetUnknown(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppv
            );

        [PreserveSig]
        new int SetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant Value
            );

        [PreserveSig]
        new int DeleteItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey
            );

        [PreserveSig]
        new int DeleteAllItems();

        [PreserveSig]
        new int SetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            int unValue
            );

        [PreserveSig]
        new int SetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            long unValue
            );

        [PreserveSig]
        new int SetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            double fValue
            );

        [PreserveSig]
        new int SetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidValue
            );

        [PreserveSig]
        new int SetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string wszValue
            );

        [PreserveSig]
        new int SetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf,
            int cbBufSize
            );

        [PreserveSig]
        new int SetUnknown(
            [MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnknown
            );

        [PreserveSig]
        new int LockStore();

        [PreserveSig]
        new int UnlockStore();

        [PreserveSig]
        new int GetCount(
            out int pcItems
            );

        [PreserveSig]
        new int GetItemByIndex(
            int unIndex,
            out Guid pguidKey,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PVMarshaler))] PropVariant pValue
            );

        [PreserveSig]
        new int CopyAllItems(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest
            );

        #endregion

        [PreserveSig]
        int GetMajorType(
            out Guid pguidMajorType
            );

        [PreserveSig]
        int IsCompressedFormat(
            [MarshalAs(UnmanagedType.Bool)] out bool pfCompressed
            );

        [PreserveSig]
        int IsEqual(
            [In, MarshalAs(UnmanagedType.Interface)] IMFMediaType pIMediaType,
            out MFMediaEqual pdwFlags
            );

        [PreserveSig]
        int GetRepresentation(
            [In, MarshalAs(UnmanagedType.Struct)] Guid guidRepresentation,
            out IntPtr ppvRepresentation
            );

        [PreserveSig]
        int FreeRepresentation(
            [In, MarshalAs(UnmanagedType.Struct)] Guid guidRepresentation,
            [In] IntPtr pvRepresentation
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("B99F381F-A8F9-47A2-A5AF-CA3A225A3890"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFVideoMediaType : IMFMediaType
    {
        #region IMFAttributes methods

        [PreserveSig]
        new int GetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PVMarshaler))] PropVariant pValue
            );

        [PreserveSig]
        new int GetItemType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out MFAttributeType pType
            );

        [PreserveSig]
        new int CompareItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant Value,
            [MarshalAs(UnmanagedType.Bool)] out bool pbResult
            );

        [PreserveSig]
        new int Compare(
            [MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs,
            MFAttributesMatchType MatchType,
            [MarshalAs(UnmanagedType.Bool)] out bool pbResult
            );

        [PreserveSig]
        new int GetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int punValue
            );

        [PreserveSig]
        new int GetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out long punValue
            );

        [PreserveSig]
        new int GetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out double pfValue
            );

        [PreserveSig]
        new int GetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out Guid pguidValue
            );

        [PreserveSig]
        new int GetStringLength(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcchLength
            );

        [PreserveSig]
        new int GetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszValue,
            int cchBufSize,
            out int pcchLength
            );

        [PreserveSig]
        new int GetAllocatedString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [MarshalAs(UnmanagedType.LPWStr)] out string ppwszValue,
            out int pcchLength
            );

        [PreserveSig]
        new int GetBlobSize(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcbBlobSize
            );

        [PreserveSig]
        new int GetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pBuf,
            int cbBufSize,
            out int pcbBlobSize
            );

        // Use GetBlob instead of this
        [PreserveSig]
        new int GetAllocatedBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out IntPtr ip,  // Read w/Marshal.Copy, Free w/Marshal.FreeCoTaskMem
            out int pcbSize
            );

        [PreserveSig]
        new int GetUnknown(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppv
            );

        [PreserveSig]
        new int SetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant Value
            );

        [PreserveSig]
        new int DeleteItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey
            );

        [PreserveSig]
        new int DeleteAllItems();

        [PreserveSig]
        new int SetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            int unValue
            );

        [PreserveSig]
        new int SetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            long unValue
            );

        [PreserveSig]
        new int SetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            double fValue
            );

        [PreserveSig]
        new int SetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidValue
            );

        [PreserveSig]
        new int SetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string wszValue
            );

        [PreserveSig]
        new int SetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf,
            int cbBufSize
            );

        [PreserveSig]
        new int SetUnknown(
            [MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnknown
            );

        [PreserveSig]
        new int LockStore();

        [PreserveSig]
        new int UnlockStore();

        [PreserveSig]
        new int GetCount(
            out int pcItems
            );

        [PreserveSig]
        new int GetItemByIndex(
            int unIndex,
            out Guid pguidKey,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PVMarshaler))] PropVariant pValue
            );

        [PreserveSig]
        new int CopyAllItems(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest
            );

        #endregion

        #region IMFMediaType methods

        [PreserveSig]
        new int GetMajorType(
            out Guid pguidMajorType
            );

        [PreserveSig]
        new int IsCompressedFormat(
            [MarshalAs(UnmanagedType.Bool)] out bool pfCompressed
            );

        [PreserveSig]
        new int IsEqual(
            [In, MarshalAs(UnmanagedType.Interface)] IMFMediaType pIMediaType,
            out MFMediaEqual pdwFlags
            );

        [PreserveSig]
        new int GetRepresentation(
            [In, MarshalAs(UnmanagedType.Struct)] Guid guidRepresentation,
            out IntPtr ppvRepresentation
            );

        [PreserveSig]
        new int FreeRepresentation(
            [In, MarshalAs(UnmanagedType.Struct)] Guid guidRepresentation,
            [In] IntPtr pvRepresentation
            );

        #endregion

        [PreserveSig, Obsolete("This method is deprecated by MS")]
        MFVideoFormat GetVideoFormat();

        [Obsolete("This method is deprecated by MS")]
        [PreserveSig]
        int GetVideoRepresentation(
            [In, MarshalAs(UnmanagedType.Struct)] Guid guidRepresentation,
            out IntPtr ppvRepresentation,
            [In] int lStride
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("fa993889-4383-415a-a930-dd472a8cf6f7"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFTopologyServiceLookup
    {
        [PreserveSig]
        int LookupService(
            MFServiceLookUpType Type,
            uint dwIndex,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidService,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.SysInt)] IntPtr[] ppvObjects,
            [In,Out] ref uint pnObjects);
        
    };

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("fa99388a-4383-415a-a930-dd472a8cf6f7"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFTopologyServiceLookupClient
    {
        [PreserveSig]
        int InitServicePointers([In] IntPtr pLookup);

        [PreserveSig]
        int ReleaseServicePointers();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("A38D9567-5A9C-4f3c-B293-8EB415B279BA"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFVideoDeviceID
    {
        [PreserveSig]
        int GetDeviceID(out Guid pDeviceID);
    };

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("0A9CCDBC-D797-4563-9667-94EC5D79292D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFRateSupport
    {
        [PreserveSig]
        int GetSlowestRate(
            [In] MFRateDirection eDirection,
            [In, MarshalAs(UnmanagedType.Bool)] bool fThin,
            out float pflRate
            );

        [PreserveSig]
        int GetFastestRate(
            [In] MFRateDirection eDirection,
            [In, MarshalAs(UnmanagedType.Bool)] bool fThin,
            out float pflRate
            );

        [PreserveSig]
        int IsRateSupported(
            [In, MarshalAs(UnmanagedType.Bool)] bool fThin,
            [In] float flRate,
            [In, Out] MfFloat pflNearestSupportedRate
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("A490B1E4-AB84-4D31-A1B2-181E03B1077A"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFVideoDisplayControl
    {
        [PreserveSig]
        int GetNativeVideoSize(
            [Out] Size pszVideo,
            [Out] Size pszARVideo
            );

        [PreserveSig]
        int GetIdealVideoSize(
            [Out] Size pszMin,
            [Out] Size pszMax
            );

        [PreserveSig]
        int SetVideoPosition(
            [In] MFVideoNormalizedRect pnrcSource,
            [In] DsRect prcDest
            );

        [PreserveSig]
        int GetVideoPosition(
            [Out] MFVideoNormalizedRect pnrcSource,
            [Out] DsRect prcDest
            );

        [PreserveSig]
        int SetAspectRatioMode(
            [In] MFVideoAspectRatioMode dwAspectRatioMode
            );

        [PreserveSig]
        int GetAspectRatioMode(
            out MFVideoAspectRatioMode pdwAspectRatioMode
            );

        [PreserveSig]
        int SetVideoWindow(
            [In] IntPtr hwndVideo
            );

        [PreserveSig]
        int GetVideoWindow(
            out IntPtr phwndVideo
            );

        [PreserveSig]
        int RepaintVideo();

        [PreserveSig]
        int GetCurrentImage(
            IntPtr pBih,
            out IntPtr pDib,
            out int pcbDib,
            out long pTimeStamp
            );

        [PreserveSig]
        int SetBorderColor(
            [In] int Clr
            );

        [PreserveSig]
        int GetBorderColor(
            out int pClr
            );

        [PreserveSig]
        int SetRenderingPrefs(
            [In] MFVideoRenderPrefs dwRenderFlags
            );

        [PreserveSig]
        int GetRenderingPrefs(
            out MFVideoRenderPrefs pdwRenderFlags
            );

        [PreserveSig]
        int SetFullscreen(
            [In, MarshalAs(UnmanagedType.Bool)] bool fFullscreen
            );

        [PreserveSig]
        int GetFullscreen(
            [MarshalAs(UnmanagedType.Bool)] out bool pfFullscreen
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("a0cade0f-06d5-4cf4-a1c7-f3cdd725aa75"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirect3DDeviceManager9
    {
        [PreserveSig]
        int ResetDevice(
            [In,MarshalAs(UnmanagedType.Interface)]  object pDevice,
            [In]  uint resetToken);

        [PreserveSig]
        int CloseDeviceHandle(IntPtr hDevice);

        [PreserveSig]
        int GetVideoService(IntPtr hDevice, Guid riid, out object ppService);

        [PreserveSig]
        int LockDevice(IntPtr hDevice,[Out,MarshalAs(UnmanagedType.Interface)] out object pDevice,[In,MarshalAs(UnmanagedType.Bool)] bool fBlock);

        [PreserveSig]
        int OpenDeviceHandle([Out] out IntPtr phDevice);

        [PreserveSig]
        int TestDevice(IntPtr hDevice);

        [PreserveSig]
        int UnlockDevice(IntPtr hDevice, [In, MarshalAs(UnmanagedType.Bool)] bool fSaveState);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("2EB1E945-18B8-4139-9B1A-D5D584818530")]
    public interface IMFClock
    {
        [PreserveSig]
        int GetClockCharacteristics(
            out MFClockCharacteristicsFlags pdwCharacteristics
            );

        [PreserveSig]
        int GetCorrelatedTime(
            [In] int dwReserved,
            out long pllClockTime,
            out long phnsSystemTime
            );

        [PreserveSig]
        int GetContinuityKey(
            out int pdwContinuityKey
            );

        [PreserveSig]
        int GetState(
            [In] int dwReserved,
            out MFClockState peClockState
            );

        [PreserveSig]
        int GetProperties(
            out MFClockProperties pClockProperties
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("a27003cf-2354-4f2a-8d6a-ab7cff15437e")]
    public interface IMFAsyncCallback
    {
   
        [PreserveSig]
        int GetParameters([Out] out uint dwFlags,[Out] out uint pdwQueue);
        
        [PreserveSig]
        int Invoke( IMFAsyncResult pAsyncResult);
    };

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("ac6b7889-0740-4d51-8619-905994a55cc6")]
    public interface IMFAsyncResult
    {
        [PreserveSig]
        int GetState([Out,MarshalAs(UnmanagedType.IUnknown)] out object ppunkState);
        
        [PreserveSig]
        int GetStatus();
        
        [PreserveSig]
        int SetStatus(int hrStatus);
        
        [PreserveSig]
        int GetObject([Out,MarshalAs(UnmanagedType.IUnknown)] out object ppObject);
        
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object GetStateNoAddRef();
        
    };

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("245BF8E9-0755-40f7-88A5-AE0F18D55E17")]
    public interface IMFTrackedSample
    {
        [PreserveSig]
        int SetAllocator(IMFAsyncCallback pSampleAllocator,[MarshalAs(UnmanagedType.IUnknown)] object pUnkState);
        
    };

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("56C294D0-753E-4260-8D61-A3D8820B1D54"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFDesiredSample
    {
        [PreserveSig]
        int GetDesiredSampleTimeAndDuration(
            out long phnsSampleTime,
            out long phnsSampleDuration
            );

        [PreserveSig]
        int SetDesiredSampleTimeAndDuration(
            [In] long hnsSampleTime,
            [In] long hnsSampleDuration
            );

        [PreserveSig]
        int Clear();
    }

    #endregion

    #region Helper Objects

    public class VTableInterface : COMHelper,IDisposable
    {
        #region Delegates

        private delegate int QueryInterfaceProc(
            IntPtr pUnk,
            ref Guid riid,
            out IntPtr ppvObject
            );

        #endregion

        #region Variables

        protected IntPtr m_pUnknown = IntPtr.Zero;
        private object m_Object = null;
        protected bool m_bCasted = true;

        #endregion

        #region Constructor

        protected VTableInterface(IntPtr pUnknown)
            :this(pUnknown,false)
        {

        }

        protected VTableInterface(IntPtr pUnknown, bool bTryCast)
        {
            if (pUnknown != IntPtr.Zero)
            {
                m_pUnknown = pUnknown;
                Marshal.AddRef(m_pUnknown);
                if (bTryCast)
                {
                    try
                    {
                        m_Object = Marshal.GetObjectForIUnknown(pUnknown);
                    }
                    catch
                    {
                        m_bCasted = false;
                    }
                }
                else
                {
                    m_bCasted = false;
                }
            }
        }

        ~VTableInterface()
        {
            Dispose();
        }

        #endregion

        #region Methods

        public int QueryInterface(ref Guid riid, out IntPtr ppvObject)
        {
            ppvObject = IntPtr.Zero;

            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            QueryInterfaceProc _Proc = GetProcDelegate<QueryInterfaceProc>(0);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                        m_pUnknown,
                        ref riid,
                        out ppvObject
                        );
        }

        #endregion

        #region Helper Methods

        protected T TryCast<T>() where T : class
        {
            if (m_bCasted)
            {
                try
                {
                    T _interface = (T)Marshal.GetTypedObjectForIUnknown(m_pUnknown, typeof(T));
                    return _interface;
                }
                catch
                {
                    m_bCasted = false;
                }
            }
            return null;
        }

        protected T GetProcDelegate<T>(int nIndex) where T : class
        {
            IntPtr pVtable = Marshal.ReadIntPtr(m_pUnknown);
            IntPtr pFunc = Marshal.ReadIntPtr(pVtable, nIndex * IntPtr.Size);
            return (Marshal.GetDelegateForFunctionPointer(pFunc, typeof(T))) as T;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            m_Object = null;
            if (m_pUnknown != IntPtr.Zero)
            {
                Marshal.Release(m_pUnknown);
                m_pUnknown = IntPtr.Zero;
            }
        }

        #endregion
    }

    public class MFTopologyServiceLookup : VTableInterface, IMFTopologyServiceLookup
    {
        #region Delegates

        private delegate int LookupServiceProc(
            IntPtr pUnk,
            MFServiceLookUpType Type,
            uint dwIndex,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidService,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.SysInt)] IntPtr[] ppvObjects,
            [In, Out] ref uint pnObjects);

        #endregion

        #region Constructor

        public MFTopologyServiceLookup(IntPtr pUnknown)
            : base(pUnknown,false)
        {

        }

        #endregion

        #region IMFTopologyServiceLookup Members

        public int LookupService(MFServiceLookUpType _type, uint dwIndex, Guid guidService, Guid riid, IntPtr[] ppvObjects, ref uint pnObjects)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            LookupServiceProc _lookUpProc = GetProcDelegate<LookupServiceProc>(3);

            if (_lookUpProc == null) return E_UNEXPECTED;

            return (HRESULT)_lookUpProc(
                        m_pUnknown,
                        _type,
                        dwIndex,
                        guidService,
                        riid,
                        ppvObjects,
                        ref pnObjects
                        );
        }

        #endregion
    }

    public class MediaEventSink : VTableInterface, IMediaEventSink
    {
        #region Delegates

        private delegate int NotifyProc(
            IntPtr pUnk,
            DsEvCode evCode, 
            IntPtr EventParam1, 
            IntPtr EventParam2);

        #endregion

        #region Constructor

        public MediaEventSink(IntPtr pUnknown)
            : base(pUnknown,false)
        {

        }

        #endregion

        #region IMediaEventSink Members

        public int Notify(DsEvCode evCode, IntPtr EventParam1, IntPtr EventParam2)
        {
            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;
            IMediaEventSink _sink = TryCast<IMediaEventSink>();
            if (_sink != null)
            {
                return _sink.Notify(evCode, EventParam1, EventParam2);
            }
            NotifyProc _NotifyProc = GetProcDelegate<NotifyProc>(3);

            if (_NotifyProc == null) return E_UNEXPECTED;

            return (HRESULT)_NotifyProc(m_pUnknown, evCode, EventParam1, EventParam2);
        }

        #endregion
    }

    public class MFHelper : COMHelper
    {
        #region MF Error HRESULTS

        public static HRESULT MF_E_PLATFORM_NOT_INITIALIZED { get { unchecked { return (HRESULT)0xC00D36B0; } } }
        public static HRESULT MF_E_BUFFERTOOSMALL { get { unchecked { return (HRESULT)0xC00D36B1; } } }
        public static HRESULT MF_E_INVALIDREQUEST { get { unchecked { return (HRESULT)0xC00D36B2; } } }
        public static HRESULT MF_E_INVALIDSTREAMNUMBER { get { unchecked { return (HRESULT)0xC00D36B3; } } }
        public static HRESULT MF_E_INVALIDMEDIATYPE { get { unchecked { return (HRESULT)0xC00D36B4; } } }
        public static HRESULT MF_E_NOTACCEPTING { get { unchecked { return (HRESULT)0xC00D36B5; } } }
        public static HRESULT MF_E_NOT_INITIALIZED { get { unchecked { return (HRESULT)0xC00D36B6; } } }
        public static HRESULT MF_E_UNSUPPORTED_REPRESENTATION { get { unchecked { return (HRESULT)0xC00D36B7; } } }
        public static HRESULT MF_E_NO_MORE_TYPES { get { unchecked { return (HRESULT)0xC00D36B9; } } }
        public static HRESULT MF_E_UNSUPPORTED_SERVICE { get { unchecked { return (HRESULT)0xC00D36BA; } } }
        public static HRESULT MF_E_UNEXPECTED { get { unchecked { return (HRESULT)0xC00D36BB; } } }
        public static HRESULT MF_E_INVALIDNAME { get { unchecked { return (HRESULT)0xC00D36BC; } } }
        public static HRESULT MF_E_INVALIDTYPE { get { unchecked { return (HRESULT)0xC00D36BD; } } }
        public static HRESULT MF_E_INVALID_FILE_FORMAT { get { unchecked { return (HRESULT)0xC00D36BE; } } }
        public static HRESULT MF_E_INVALIDINDEX { get { unchecked { return (HRESULT)0xC00D36BF; } } }
        public static HRESULT MF_E_INVALID_TIMESTAMP { get { unchecked { return (HRESULT)0xC00D36C0; } } }
        public static HRESULT MF_E_UNSUPPORTED_SCHEME { get { unchecked { return (HRESULT)0xC00D36C3; } } }
        public static HRESULT MF_E_UNSUPPORTED_BYTESTREAM_TYPE { get { unchecked { return (HRESULT)0xC00D36C4; } } }
        public static HRESULT MF_E_UNSUPPORTED_TIME_FORMAT { get { unchecked { return (HRESULT)0xC00D36C5; } } }
        public static HRESULT MF_E_NO_SAMPLE_TIMESTAMP { get { unchecked { return (HRESULT)0xC00D36C8; } } }
        public static HRESULT MF_E_NO_SAMPLE_DURATION { get { unchecked { return (HRESULT)0xC00D36C9; } } }
        public static HRESULT MF_E_INVALID_STREAM_DATA { get { unchecked { return (HRESULT)0xC00D36CB; } } }
        public static HRESULT MF_E_RT_UNAVAILABLE { get { unchecked { return (HRESULT)0xC00D36CF; } } }
        public static HRESULT MF_E_UNSUPPORTED_RATE { get { unchecked { return (HRESULT)0xC00D36D0; } } }
        public static HRESULT MF_E_THINNING_UNSUPPORTED { get { unchecked { return (HRESULT)0xC00D36D1; } } }
        public static HRESULT MF_E_REVERSE_UNSUPPORTED { get { unchecked { return (HRESULT)0xC00D36D2; } } }
        public static HRESULT MF_E_UNSUPPORTED_RATE_TRANSITION { get { unchecked { return (HRESULT)0xC00D36D3; } } }
        public static HRESULT MF_E_RATE_CHANGE_PREEMPTED { get { unchecked { return (HRESULT)0xC00D36D4; } } }
        public static HRESULT MF_E_NOT_FOUND { get { unchecked { return (HRESULT)0xC00D36D5; } } }
        public static HRESULT MF_E_NOT_AVAILABLE { get { unchecked { return (HRESULT)0xC00D36D6; } } }
        public static HRESULT MF_E_NO_CLOCK { get { unchecked { return (HRESULT)0xC00D36D7; } } }
        public static HRESULT MF_S_MULTIPLE_BEGIN { get { unchecked { return (HRESULT)0x000D36D8; } } }
        public static HRESULT MF_E_MULTIPLE_BEGIN { get { unchecked { return (HRESULT)0xC00D36D9; } } }
        public static HRESULT MF_E_MULTIPLE_SUBSCRIBERS { get { unchecked { return (HRESULT)0xC00D36DA; } } }
        public static HRESULT MF_E_TIMER_ORPHANED { get { unchecked { return (HRESULT)0xC00D36DB; } } }
        public static HRESULT MF_E_STATE_TRANSITION_PENDING { get { unchecked { return (HRESULT)0xC00D36DC; } } }
        public static HRESULT MF_E_UNSUPPORTED_STATE_TRANSITION { get { unchecked { return (HRESULT)0xC00D36DD; } } }
        public static HRESULT MF_E_UNRECOVERABLE_ERROR_OCCURRED { get { unchecked { return (HRESULT)0xC00D36DE; } } }
        public static HRESULT MF_E_SAMPLE_HAS_TOO_MANY_BUFFERS { get { unchecked { return (HRESULT)0xC00D36DF; } } }
        public static HRESULT MF_E_SAMPLE_NOT_WRITABLE { get { unchecked { return (HRESULT)0xC00D36E0; } } }
        public static HRESULT MF_E_INVALID_KEY { get { unchecked { return (HRESULT)0xC00D36E2; } } }
        public static HRESULT MF_E_BAD_STARTUP_VERSION { get { unchecked { return (HRESULT)0xC00D36E3; } } }
        public static HRESULT MF_E_UNSUPPORTED_CAPTION { get { unchecked { return (HRESULT)0xC00D36E4; } } }
        public static HRESULT MF_E_INVALID_POSITION { get { unchecked { return (HRESULT)0xC00D36E5; } } }
        public static HRESULT MF_E_ATTRIBUTENOTFOUND { get { unchecked { return (HRESULT)0xC00D36E6; } } }
        public static HRESULT MF_E_PROPERTY_TYPE_NOT_ALLOWED { get { unchecked { return (HRESULT)0xC00D36E7; } } }
        public static HRESULT MF_E_PROPERTY_TYPE_NOT_SUPPORTED { get { unchecked { return (HRESULT)0xC00D36E8; } } }
        public static HRESULT MF_E_PROPERTY_EMPTY { get { unchecked { return (HRESULT)0xC00D36E9; } } }
        public static HRESULT MF_E_PROPERTY_NOT_EMPTY { get { unchecked { return (HRESULT)0xC00D36EA; } } }
        public static HRESULT MF_E_PROPERTY_VECTOR_NOT_ALLOWED { get { unchecked { return (HRESULT)0xC00D36EB; } } }
        public static HRESULT MF_E_PROPERTY_VECTOR_REQUIRED { get { unchecked { return (HRESULT)0xC00D36EC; } } }
        public static HRESULT MF_E_OPERATION_CANCELLED { get { unchecked { return (HRESULT)0xC00D36ED; } } }
        public static HRESULT MF_E_BYTESTREAM_NOT_SEEKABLE { get { unchecked { return (HRESULT)0xC00D36EE; } } }
        public static HRESULT MF_E_DISABLED_IN_SAFEMODE { get { unchecked { return (HRESULT)0xC00D36EF; } } }
        public static HRESULT MF_E_CANNOT_PARSE_BYTESTREAM { get { unchecked { return (HRESULT)0xC00D36F0; } } }
        public static HRESULT MF_E_SOURCERESOLVER_MUTUALLY_EXCLUSIVE_FLAGS { get { unchecked { return (HRESULT)0xC00D36F1; } } }
        public static HRESULT MF_E_MEDIAPROC_WRONGSTATE { get { unchecked { return (HRESULT)0xC00D36F2; } } }
        public static HRESULT MF_E_RT_THROUGHPUT_NOT_AVAILABLE { get { unchecked { return (HRESULT)0xC00D36F3; } } }
        public static HRESULT MF_E_RT_TOO_MANY_CLASSES { get { unchecked { return (HRESULT)0xC00D36F4; } } }
        public static HRESULT MF_E_RT_WOULDBLOCK { get { unchecked { return (HRESULT)0xC00D36F5; } } }
        public static HRESULT MF_E_NO_BITPUMP { get { unchecked { return (HRESULT)0xC00D36F6; } } }
        public static HRESULT MF_E_RT_OUTOFMEMORY { get { unchecked { return (HRESULT)0xC00D36F7; } } }
        public static HRESULT MF_E_RT_WORKQUEUE_CLASS_NOT_SPECIFIED { get { unchecked { return (HRESULT)0xC00D36F8; } } }
        public static HRESULT MF_E_INSUFFICIENT_BUFFER { get { unchecked { return (HRESULT)0xC00D7170; } } }
        public static HRESULT MF_E_CANNOT_CREATE_SINK { get { unchecked { return (HRESULT)0xC00D36FA; } } }
        public static HRESULT MF_E_BYTESTREAM_UNKNOWN_LENGTH { get { unchecked { return (HRESULT)0xC00D36FB; } } }
        public static HRESULT MF_E_SESSION_PAUSEWHILESTOPPED { get { unchecked { return (HRESULT)0xC00D36FC; } } }
        public static HRESULT MF_S_ACTIVATE_REPLACED { get { unchecked { return (HRESULT)0x000D36FD; } } }
        public static HRESULT MF_E_FORMAT_CHANGE_NOT_SUPPORTED { get { unchecked { return (HRESULT)0xC00D36FE; } } }
        public static HRESULT MF_E_INVALID_WORKQUEUE { get { unchecked { return (HRESULT)0xC00D36FF; } } }
        public static HRESULT MF_E_DRM_UNSUPPORTED { get { unchecked { return (HRESULT)0xC00D3700; } } }
        public static HRESULT MF_E_UNAUTHORIZED { get { unchecked { return (HRESULT)0xC00D3701; } } }
        public static HRESULT MF_E_OUT_OF_RANGE { get { unchecked { return (HRESULT)0xC00D3702; } } }
        public static HRESULT MF_E_INVALID_CODEC_MERIT { get { unchecked { return (HRESULT)0xC00D3703; } } }
        public static HRESULT MF_E_HW_MFT_FAILED_START_STREAMING { get { unchecked { return (HRESULT)0xC00D3704; } } }
        public static HRESULT MF_S_ASF_PARSEINPROGRESS { get { unchecked { return (HRESULT)0x400D3A98; } } }
        public static HRESULT MF_E_ASF_PARSINGINCOMPLETE { get { unchecked { return (HRESULT)0xC00D3A98; } } }
        public static HRESULT MF_E_ASF_MISSINGDATA { get { unchecked { return (HRESULT)0xC00D3A99; } } }
        public static HRESULT MF_E_ASF_INVALIDDATA { get { unchecked { return (HRESULT)0xC00D3A9A; } } }
        public static HRESULT MF_E_ASF_OPAQUEPACKET { get { unchecked { return (HRESULT)0xC00D3A9B; } } }
        public static HRESULT MF_E_ASF_NOINDEX { get { unchecked { return (HRESULT)0xC00D3A9C; } } }
        public static HRESULT MF_E_ASF_OUTOFRANGE { get { unchecked { return (HRESULT)0xC00D3A9D; } } }
        public static HRESULT MF_E_ASF_INDEXNOTLOADED { get { unchecked { return (HRESULT)0xC00D3A9E; } } }
        public static HRESULT MF_E_ASF_TOO_MANY_PAYLOADS { get { unchecked { return (HRESULT)0xC00D3A9F; } } }
        public static HRESULT MF_E_ASF_UNSUPPORTED_STREAM_TYPE { get { unchecked { return (HRESULT)0xC00D3AA0; } } }
        public static HRESULT MF_E_ASF_DROPPED_PACKET { get { unchecked { return (HRESULT)0xC00D3AA1; } } }
        public static HRESULT MF_E_NO_EVENTS_AVAILABLE { get { unchecked { return (HRESULT)0xC00D3E80; } } }
        public static HRESULT MF_E_INVALID_STATE_TRANSITION { get { unchecked { return (HRESULT)0xC00D3E82; } } }
        public static HRESULT MF_E_END_OF_STREAM { get { unchecked { return (HRESULT)0xC00D3E84; } } }
        public static HRESULT MF_E_SHUTDOWN { get { unchecked { return (HRESULT)0xC00D3E85; } } }
        public static HRESULT MF_E_MP3_NOTFOUND { get { unchecked { return (HRESULT)0xC00D3E86; } } }
        public static HRESULT MF_E_MP3_OUTOFDATA { get { unchecked { return (HRESULT)0xC00D3E87; } } }
        public static HRESULT MF_E_MP3_NOTMP3 { get { unchecked { return (HRESULT)0xC00D3E88; } } }
        public static HRESULT MF_E_MP3_NOTSUPPORTED { get { unchecked { return (HRESULT)0xC00D3E89; } } }
        public static HRESULT MF_E_NO_DURATION { get { unchecked { return (HRESULT)0xC00D3E8A; } } }
        public static HRESULT MF_E_INVALID_FORMAT { get { unchecked { return (HRESULT)0xC00D3E8C; } } }
        public static HRESULT MF_E_PROPERTY_NOT_FOUND { get { unchecked { return (HRESULT)0xC00D3E8D; } } }
        public static HRESULT MF_E_PROPERTY_READ_ONLY { get { unchecked { return (HRESULT)0xC00D3E8E; } } }
        public static HRESULT MF_E_PROPERTY_NOT_ALLOWED { get { unchecked { return (HRESULT)0xC00D3E8F; } } }
        public static HRESULT MF_E_MEDIA_SOURCE_NOT_STARTED { get { unchecked { return (HRESULT)0xC00D3E91; } } }
        public static HRESULT MF_E_UNSUPPORTED_FORMAT { get { unchecked { return (HRESULT)0xC00D3E98; } } }
        public static HRESULT MF_E_MP3_BAD_CRC { get { unchecked { return (HRESULT)0xC00D3E99; } } }
        public static HRESULT MF_E_NOT_PROTECTED { get { unchecked { return (HRESULT)0xC00D3E9A; } } }
        public static HRESULT MF_E_MEDIA_SOURCE_WRONGSTATE { get { unchecked { return (HRESULT)0xC00D3E9B; } } }
        public static HRESULT MF_E_MEDIA_SOURCE_NO_STREAMS_SELECTED { get { unchecked { return (HRESULT)0xC00D3E9C; } } }
        public static HRESULT MF_E_CANNOT_FIND_KEYFRAME_SAMPLE { get { unchecked { return (HRESULT)0xC00D3E9D; } } }
        public static HRESULT MF_E_NETWORK_RESOURCE_FAILURE { get { unchecked { return (HRESULT)0xC00D4268; } } }
        public static HRESULT MF_E_NET_WRITE { get { unchecked { return (HRESULT)0xC00D4269; } } }
        public static HRESULT MF_E_NET_READ { get { unchecked { return (HRESULT)0xC00D426A; } } }
        public static HRESULT MF_E_NET_REQUIRE_NETWORK { get { unchecked { return (HRESULT)0xC00D426B; } } }
        public static HRESULT MF_E_NET_REQUIRE_ASYNC { get { unchecked { return (HRESULT)0xC00D426C; } } }
        public static HRESULT MF_E_NET_BWLEVEL_NOT_SUPPORTED { get { unchecked { return (HRESULT)0xC00D426D; } } }
        public static HRESULT MF_E_NET_STREAMGROUPS_NOT_SUPPORTED { get { unchecked { return (HRESULT)0xC00D426E; } } }
        public static HRESULT MF_E_NET_MANUALSS_NOT_SUPPORTED { get { unchecked { return (HRESULT)0xC00D426F; } } }
        public static HRESULT MF_E_NET_INVALID_PRESENTATION_DESCRIPTOR { get { unchecked { return (HRESULT)0xC00D4270; } } }
        public static HRESULT MF_E_NET_CACHESTREAM_NOT_FOUND { get { unchecked { return (HRESULT)0xC00D4271; } } }
        public static HRESULT MF_I_MANUAL_PROXY { get { unchecked { return (HRESULT)0x400D4272; } } }
        public static HRESULT MF_E_NET_REQUIRE_INPUT { get { unchecked { return (HRESULT)0xC00D4274; } } }
        public static HRESULT MF_E_NET_REDIRECT { get { unchecked { return (HRESULT)0xC00D4275; } } }
        public static HRESULT MF_E_NET_REDIRECT_TO_PROXY { get { unchecked { return (HRESULT)0xC00D4276; } } }
        public static HRESULT MF_E_NET_TOO_MANY_REDIRECTS { get { unchecked { return (HRESULT)0xC00D4277; } } }
        public static HRESULT MF_E_NET_TIMEOUT { get { unchecked { return (HRESULT)0xC00D4278; } } }
        public static HRESULT MF_E_NET_CLIENT_CLOSE { get { unchecked { return (HRESULT)0xC00D4279; } } }
        public static HRESULT MF_E_NET_BAD_CONTROL_DATA { get { unchecked { return (HRESULT)0xC00D427A; } } }
        public static HRESULT MF_E_NET_INCOMPATIBLE_SERVER { get { unchecked { return (HRESULT)0xC00D427B; } } }
        public static HRESULT MF_E_NET_UNSAFE_URL { get { unchecked { return (HRESULT)0xC00D427C; } } }
        public static HRESULT MF_E_NET_CACHE_NO_DATA { get { unchecked { return (HRESULT)0xC00D427D; } } }
        public static HRESULT MF_E_NET_EOL { get { unchecked { return (HRESULT)0xC00D427E; } } }
        public static HRESULT MF_E_NET_BAD_REQUEST { get { unchecked { return (HRESULT)0xC00D427F; } } }
        public static HRESULT MF_E_NET_INTERNAL_SERVER_ERROR { get { unchecked { return (HRESULT)0xC00D4280; } } }
        public static HRESULT MF_E_NET_SESSION_NOT_FOUND { get { unchecked { return (HRESULT)0xC00D4281; } } }
        public static HRESULT MF_E_NET_NOCONNECTION { get { unchecked { return (HRESULT)0xC00D4282; } } }
        public static HRESULT MF_E_NET_CONNECTION_FAILURE { get { unchecked { return (HRESULT)0xC00D4283; } } }
        public static HRESULT MF_E_NET_INCOMPATIBLE_PUSHSERVER { get { unchecked { return (HRESULT)0xC00D4284; } } }
        public static HRESULT MF_E_NET_SERVER_ACCESSDENIED { get { unchecked { return (HRESULT)0xC00D4285; } } }
        public static HRESULT MF_E_NET_PROXY_ACCESSDENIED { get { unchecked { return (HRESULT)0xC00D4286; } } }
        public static HRESULT MF_E_NET_CANNOTCONNECT { get { unchecked { return (HRESULT)0xC00D4287; } } }
        public static HRESULT MF_E_NET_INVALID_PUSH_TEMPLATE { get { unchecked { return (HRESULT)0xC00D4288; } } }
        public static HRESULT MF_E_NET_INVALID_PUSH_PUBLISHING_POINT { get { unchecked { return (HRESULT)0xC00D4289; } } }
        public static HRESULT MF_E_NET_BUSY { get { unchecked { return (HRESULT)0xC00D428A; } } }
        public static HRESULT MF_E_NET_RESOURCE_GONE { get { unchecked { return (HRESULT)0xC00D428B; } } }
        public static HRESULT MF_E_NET_ERROR_FROM_PROXY { get { unchecked { return (HRESULT)0xC00D428C; } } }
        public static HRESULT MF_E_NET_PROXY_TIMEOUT { get { unchecked { return (HRESULT)0xC00D428D; } } }
        public static HRESULT MF_E_NET_SERVER_UNAVAILABLE { get { unchecked { return (HRESULT)0xC00D428E; } } }
        public static HRESULT MF_E_NET_TOO_MUCH_DATA { get { unchecked { return (HRESULT)0xC00D428F; } } }
        public static HRESULT MF_E_NET_SESSION_INVALID { get { unchecked { return (HRESULT)0xC00D4290; } } }
        public static HRESULT MF_E_OFFLINE_MODE { get { unchecked { return (HRESULT)0xC00D4291; } } }
        public static HRESULT MF_E_NET_UDP_BLOCKED { get { unchecked { return (HRESULT)0xC00D4292; } } }
        public static HRESULT MF_E_NET_UNSUPPORTED_CONFIGURATION { get { unchecked { return (HRESULT)0xC00D4293; } } }
        public static HRESULT MF_E_NET_PROTOCOL_DISABLED { get { unchecked { return (HRESULT)0xC00D4294; } } }
        public static HRESULT MF_E_ALREADY_INITIALIZED { get { unchecked { return (HRESULT)0xC00D4650; } } }
        public static HRESULT MF_E_BANDWIDTH_OVERRUN { get { unchecked { return (HRESULT)0xC00D4651; } } }
        public static HRESULT MF_E_LATE_SAMPLE { get { unchecked { return (HRESULT)0xC00D4652; } } }
        public static HRESULT MF_E_FLUSH_NEEDED { get { unchecked { return (HRESULT)0xC00D4653; } } }
        public static HRESULT MF_E_INVALID_PROFILE { get { unchecked { return (HRESULT)0xC00D4654; } } }
        public static HRESULT MF_E_INDEX_NOT_COMMITTED { get { unchecked { return (HRESULT)0xC00D4655; } } }
        public static HRESULT MF_E_NO_INDEX { get { unchecked { return (HRESULT)0xC00D4656; } } }
        public static HRESULT MF_E_CANNOT_INDEX_IN_PLACE { get { unchecked { return (HRESULT)0xC00D4657; } } }
        public static HRESULT MF_E_MISSING_ASF_LEAKYBUCKET { get { unchecked { return (HRESULT)0xC00D4658; } } }
        public static HRESULT MF_E_INVALID_ASF_STREAMID { get { unchecked { return (HRESULT)0xC00D4659; } } }
        public static HRESULT MF_E_STREAMSINK_REMOVED { get { unchecked { return (HRESULT)0xC00D4A38; } } }
        public static HRESULT MF_E_STREAMSINKS_OUT_OF_SYNC { get { unchecked { return (HRESULT)0xC00D4A3A; } } }
        public static HRESULT MF_E_STREAMSINKS_FIXED { get { unchecked { return (HRESULT)0xC00D4A3B; } } }
        public static HRESULT MF_E_STREAMSINK_EXISTS { get { unchecked { return (HRESULT)0xC00D4A3C; } } }
        public static HRESULT MF_E_SAMPLEALLOCATOR_CANCELED { get { unchecked { return (HRESULT)0xC00D4A3D; } } }
        public static HRESULT MF_E_SAMPLEALLOCATOR_EMPTY { get { unchecked { return (HRESULT)0xC00D4A3E; } } }
        public static HRESULT MF_E_SINK_ALREADYSTOPPED { get { unchecked { return (HRESULT)0xC00D4A3F; } } }
        public static HRESULT MF_E_ASF_FILESINK_BITRATE_UNKNOWN { get { unchecked { return (HRESULT)0xC00D4A40; } } }
        public static HRESULT MF_E_SINK_NO_STREAMS { get { unchecked { return (HRESULT)0xC00D4A41; } } }
        public static HRESULT MF_S_SINK_NOT_FINALIZED { get { unchecked { return (HRESULT)0x000D4A42; } } }
        public static HRESULT MF_E_METADATA_TOO_LONG { get { unchecked { return (HRESULT)0xC00D4A43; } } }
        public static HRESULT MF_E_SINK_NO_SAMPLES_PROCESSED { get { unchecked { return (HRESULT)0xC00D4A44; } } }
        public static HRESULT MF_E_VIDEO_REN_NO_PROCAMP_HW { get { unchecked { return (HRESULT)0xC00D4E20; } } }
        public static HRESULT MF_E_VIDEO_REN_NO_DEINTERLACE_HW { get { unchecked { return (HRESULT)0xC00D4E21; } } }
        public static HRESULT MF_E_VIDEO_REN_COPYPROT_FAILED { get { unchecked { return (HRESULT)0xC00D4E22; } } }
        public static HRESULT MF_E_VIDEO_REN_SURFACE_NOT_SHARED { get { unchecked { return (HRESULT)0xC00D4E23; } } }
        public static HRESULT MF_E_VIDEO_DEVICE_LOCKED { get { unchecked { return (HRESULT)0xC00D4E24; } } }
        public static HRESULT MF_E_NEW_VIDEO_DEVICE { get { unchecked { return (HRESULT)0xC00D4E25; } } }
        public static HRESULT MF_E_NO_VIDEO_SAMPLE_AVAILABLE { get { unchecked { return (HRESULT)0xC00D4E26; } } }
        public static HRESULT MF_E_NO_AUDIO_PLAYBACK_DEVICE { get { unchecked { return (HRESULT)0xC00D4E84; } } }
        public static HRESULT MF_E_AUDIO_PLAYBACK_DEVICE_IN_USE { get { unchecked { return (HRESULT)0xC00D4E85; } } }
        public static HRESULT MF_E_AUDIO_PLAYBACK_DEVICE_INVALIDATED { get { unchecked { return (HRESULT)0xC00D4E86; } } }
        public static HRESULT MF_E_AUDIO_SERVICE_NOT_RUNNING { get { unchecked { return (HRESULT)0xC00D4E87; } } }
        public static HRESULT MF_E_TOPO_INVALID_OPTIONAL_NODE { get { unchecked { return (HRESULT)0xC00D520E; } } }
        public static HRESULT MF_E_TOPO_CANNOT_FIND_DECRYPTOR { get { unchecked { return (HRESULT)0xC00D5211; } } }
        public static HRESULT MF_E_TOPO_CODEC_NOT_FOUND { get { unchecked { return (HRESULT)0xC00D5212; } } }
        public static HRESULT MF_E_TOPO_CANNOT_CONNECT { get { unchecked { return (HRESULT)0xC00D5213; } } }
        public static HRESULT MF_E_TOPO_UNSUPPORTED { get { unchecked { return (HRESULT)0xC00D5214; } } }
        public static HRESULT MF_E_TOPO_INVALID_TIME_ATTRIBUTES { get { unchecked { return (HRESULT)0xC00D5215; } } }
        public static HRESULT MF_E_TOPO_LOOPS_IN_TOPOLOGY { get { unchecked { return (HRESULT)0xC00D5216; } } }
        public static HRESULT MF_E_TOPO_MISSING_PRESENTATION_DESCRIPTOR { get { unchecked { return (HRESULT)0xC00D5217; } } }
        public static HRESULT MF_E_TOPO_MISSING_STREAM_DESCRIPTOR { get { unchecked { return (HRESULT)0xC00D5218; } } }
        public static HRESULT MF_E_TOPO_STREAM_DESCRIPTOR_NOT_SELECTED { get { unchecked { return (HRESULT)0xC00D5219; } } }
        public static HRESULT MF_E_TOPO_MISSING_SOURCE { get { unchecked { return (HRESULT)0xC00D521A; } } }
        public static HRESULT MF_E_TOPO_SINK_ACTIVATES_UNSUPPORTED { get { unchecked { return (HRESULT)0xC00D521B; } } }
        public static HRESULT MF_E_SEQUENCER_UNKNOWN_SEGMENT_ID { get { unchecked { return (HRESULT)0xC00D61AC; } } }
        public static HRESULT MF_S_SEQUENCER_CONTEXT_CANCELED { get { unchecked { return (HRESULT)0x000D61AD; } } }
        public static HRESULT MF_E_NO_SOURCE_IN_CACHE { get { unchecked { return (HRESULT)0xC00D61AE; } } }
        public static HRESULT MF_S_SEQUENCER_SEGMENT_AT_END_OF_STREAM { get { unchecked { return (HRESULT)0x000D61AF; } } }
        public static HRESULT MF_E_TRANSFORM_TYPE_NOT_SET { get { unchecked { return (HRESULT)0xC00D6D60; } } }
        public static HRESULT MF_E_TRANSFORM_STREAM_CHANGE { get { unchecked { return (HRESULT)0xC00D6D61; } } }
        public static HRESULT MF_E_TRANSFORM_INPUT_REMAINING { get { unchecked { return (HRESULT)0xC00D6D62; } } }
        public static HRESULT MF_E_TRANSFORM_PROFILE_MISSING { get { unchecked { return (HRESULT)0xC00D6D63; } } }
        public static HRESULT MF_E_TRANSFORM_PROFILE_INVALID_OR_CORRUPT { get { unchecked { return (HRESULT)0xC00D6D64; } } }
        public static HRESULT MF_E_TRANSFORM_PROFILE_TRUNCATED { get { unchecked { return (HRESULT)0xC00D6D65; } } }
        public static HRESULT MF_E_TRANSFORM_PROPERTY_PID_NOT_RECOGNIZED { get { unchecked { return (HRESULT)0xC00D6D66; } } }
        public static HRESULT MF_E_TRANSFORM_PROPERTY_VARIANT_TYPE_WRONG { get { unchecked { return (HRESULT)0xC00D6D67; } } }
        public static HRESULT MF_E_TRANSFORM_PROPERTY_NOT_WRITEABLE { get { unchecked { return (HRESULT)0xC00D6D68; } } }
        public static HRESULT MF_E_TRANSFORM_PROPERTY_ARRAY_VALUE_WRONG_NUM_DIM { get { unchecked { return (HRESULT)0xC00D6D69; } } }
        public static HRESULT MF_E_TRANSFORM_PROPERTY_VALUE_SIZE_WRONG { get { unchecked { return (HRESULT)0xC00D6D6A; } } }
        public static HRESULT MF_E_TRANSFORM_PROPERTY_VALUE_OUT_OF_RANGE { get { unchecked { return (HRESULT)0xC00D6D6B; } } }
        public static HRESULT MF_E_TRANSFORM_PROPERTY_VALUE_INCOMPATIBLE { get { unchecked { return (HRESULT)0xC00D6D6C; } } }
        public static HRESULT MF_E_TRANSFORM_NOT_POSSIBLE_FOR_CURRENT_OUTPUT_MEDIATYPE { get { unchecked { return (HRESULT)0xC00D6D6D; } } }
        public static HRESULT MF_E_TRANSFORM_NOT_POSSIBLE_FOR_CURRENT_INPUT_MEDIATYPE { get { unchecked { return (HRESULT)0xC00D6D6E; } } }
        public static HRESULT MF_E_TRANSFORM_NOT_POSSIBLE_FOR_CURRENT_MEDIATYPE_COMBINATION { get { unchecked { return (HRESULT)0xC00D6D6F; } } }
        public static HRESULT MF_E_TRANSFORM_CONFLICTS_WITH_OTHER_CURRENTLY_ENABLED_FEATURES { get { unchecked { return (HRESULT)0xC00D6D70; } } }
        public static HRESULT MF_E_TRANSFORM_NEED_MORE_INPUT { get { unchecked { return (HRESULT)0xC00D6D72; } } }
        public static HRESULT MF_E_TRANSFORM_NOT_POSSIBLE_FOR_CURRENT_SPKR_CONFIG { get { unchecked { return (HRESULT)0xC00D6D73; } } }
        public static HRESULT MF_E_TRANSFORM_CANNOT_CHANGE_MEDIATYPE_WHILE_PROCESSING { get { unchecked { return (HRESULT)0xC00D6D74; } } }
        public static HRESULT MF_S_TRANSFORM_DO_NOT_PROPAGATE_EVENT { get { unchecked { return (HRESULT)0x000D6D75; } } }
        public static HRESULT MF_E_UNSUPPORTED_D3D_TYPE { get { unchecked { return (HRESULT)0xC00D6D76; } } }
        public static HRESULT MF_E_TRANSFORM_ASYNC_LOCKED { get { unchecked { return (HRESULT)0xC00D6D77; } } }
        public static HRESULT MF_E_TRANSFORM_CANNOT_INITIALIZE_ACM_DRIVER { get { unchecked { return (HRESULT)0xC00D6D78; } } }
        public static HRESULT MF_E_LICENSE_INCORRECT_RIGHTS { get { unchecked { return (HRESULT)0xC00D7148; } } }
        public static HRESULT MF_E_LICENSE_OUTOFDATE { get { unchecked { return (HRESULT)0xC00D7149; } } }
        public static HRESULT MF_E_LICENSE_REQUIRED { get { unchecked { return (HRESULT)0xC00D714A; } } }
        public static HRESULT MF_E_DRM_HARDWARE_INCONSISTENT { get { unchecked { return (HRESULT)0xC00D714B; } } }
        public static HRESULT MF_E_NO_CONTENT_PROTECTION_MANAGER { get { unchecked { return (HRESULT)0xC00D714C; } } }
        public static HRESULT MF_E_LICENSE_RESTORE_NO_RIGHTS { get { unchecked { return (HRESULT)0xC00D714D; } } }
        public static HRESULT MF_E_BACKUP_RESTRICTED_LICENSE { get { unchecked { return (HRESULT)0xC00D714E; } } }
        public static HRESULT MF_E_LICENSE_RESTORE_NEEDS_INDIVIDUALIZATION { get { unchecked { return (HRESULT)0xC00D714F; } } }
        public static HRESULT MF_S_PROTECTION_NOT_REQUIRED { get { unchecked { return (HRESULT)0x000D7150; } } }
        public static HRESULT MF_E_COMPONENT_REVOKED { get { unchecked { return (HRESULT)0xC00D7151; } } }
        public static HRESULT MF_E_TRUST_DISABLED { get { unchecked { return (HRESULT)0xC00D7152; } } }
        public static HRESULT MF_E_WMDRMOTA_NO_ACTION { get { unchecked { return (HRESULT)0xC00D7153; } } }
        public static HRESULT MF_E_WMDRMOTA_ACTION_ALREADY_SET { get { unchecked { return (HRESULT)0xC00D7154; } } }
        public static HRESULT MF_E_WMDRMOTA_DRM_HEADER_NOT_AVAILABLE { get { unchecked { return (HRESULT)0xC00D7155; } } }
        public static HRESULT MF_E_WMDRMOTA_DRM_ENCRYPTION_SCHEME_NOT_SUPPORTED { get { unchecked { return (HRESULT)0xC00D7156; } } }
        public static HRESULT MF_E_WMDRMOTA_ACTION_MISMATCH { get { unchecked { return (HRESULT)0xC00D7157; } } }
        public static HRESULT MF_E_WMDRMOTA_INVALID_POLICY { get { unchecked { return (HRESULT)0xC00D7158; } } }
        public static HRESULT MF_E_POLICY_UNSUPPORTED { get { unchecked { return (HRESULT)0xC00D7159; } } }
        public static HRESULT MF_E_OPL_NOT_SUPPORTED { get { unchecked { return (HRESULT)0xC00D715A; } } }
        public static HRESULT MF_E_TOPOLOGY_VERIFICATION_FAILED { get { unchecked { return (HRESULT)0xC00D715B; } } }
        public static HRESULT MF_E_SIGNATURE_VERIFICATION_FAILED { get { unchecked { return (HRESULT)0xC00D715C; } } }
        public static HRESULT MF_E_DEBUGGING_NOT_ALLOWED { get { unchecked { return (HRESULT)0xC00D715D; } } }
        public static HRESULT MF_E_CODE_EXPIRED { get { unchecked { return (HRESULT)0xC00D715E; } } }
        public static HRESULT MF_E_GRL_VERSION_TOO_LOW { get { unchecked { return (HRESULT)0xC00D715F; } } }
        public static HRESULT MF_E_GRL_RENEWAL_NOT_FOUND { get { unchecked { return (HRESULT)0xC00D7160; } } }
        public static HRESULT MF_E_GRL_EXTENSIBLE_ENTRY_NOT_FOUND { get { unchecked { return (HRESULT)0xC00D7161; } } }
        public static HRESULT MF_E_KERNEL_UNTRUSTED { get { unchecked { return (HRESULT)0xC00D7162; } } }
        public static HRESULT MF_E_PEAUTH_UNTRUSTED { get { unchecked { return (HRESULT)0xC00D7163; } } }
        public static HRESULT MF_E_NON_PE_PROCESS { get { unchecked { return (HRESULT)0xC00D7165; } } }
        public static HRESULT MF_E_REBOOT_REQUIRED { get { unchecked { return (HRESULT)0xC00D7167; } } }
        public static HRESULT MF_S_WAIT_FOR_POLICY_SET { get { unchecked { return (HRESULT)0x000D7168; } } }
        public static HRESULT MF_S_VIDEO_DISABLED_WITH_UNKNOWN_SOFTWARE_OUTPUT { get { unchecked { return (HRESULT)0x000D7169; } } }
        public static HRESULT MF_E_GRL_INVALID_FORMAT { get { unchecked { return (HRESULT)0xC00D716A; } } }
        public static HRESULT MF_E_GRL_UNRECOGNIZED_FORMAT { get { unchecked { return (HRESULT)0xC00D716B; } } }
        public static HRESULT MF_E_ALL_PROCESS_RESTART_REQUIRED { get { unchecked { return (HRESULT)0xC00D716C; } } }
        public static HRESULT MF_E_PROCESS_RESTART_REQUIRED { get { unchecked { return (HRESULT)0xC00D716D; } } }
        public static HRESULT MF_E_USERMODE_UNTRUSTED { get { unchecked { return (HRESULT)0xC00D716E; } } }
        public static HRESULT MF_E_PEAUTH_SESSION_NOT_STARTED { get { unchecked { return (HRESULT)0xC00D716F; } } }
        public static HRESULT MF_E_PEAUTH_PUBLICKEY_REVOKED { get { unchecked { return (HRESULT)0xC00D7171; } } }
        public static HRESULT MF_E_GRL_ABSENT { get { unchecked { return (HRESULT)0xC00D7172; } } }
        public static HRESULT MF_S_PE_TRUSTED { get { unchecked { return (HRESULT)0x000D7173; } } }
        public static HRESULT MF_E_PE_UNTRUSTED { get { unchecked { return (HRESULT)0xC00D7174; } } }
        public static HRESULT MF_E_PEAUTH_NOT_STARTED { get { unchecked { return (HRESULT)0xC00D7175; } } }
        public static HRESULT MF_E_INCOMPATIBLE_SAMPLE_PROTECTION { get { unchecked { return (HRESULT)0xC00D7176; } } }
        public static HRESULT MF_E_PE_SESSIONS_MAXED { get { unchecked { return (HRESULT)0xC00D7177; } } }
        public static HRESULT MF_E_HIGH_SECURITY_LEVEL_CONTENT_NOT_ALLOWED { get { unchecked { return (HRESULT)0xC00D7178; } } }
        public static HRESULT MF_E_TEST_SIGNED_COMPONENTS_NOT_ALLOWED { get { unchecked { return (HRESULT)0xC00D7179; } } }
        public static HRESULT MF_E_ITA_UNSUPPORTED_ACTION { get { unchecked { return (HRESULT)0xC00D717A; } } }
        public static HRESULT MF_E_ITA_ERROR_PARSING_SAP_PARAMETERS { get { unchecked { return (HRESULT)0xC00D717B; } } }
        public static HRESULT MF_E_POLICY_MGR_ACTION_OUTOFBOUNDS { get { unchecked { return (HRESULT)0xC00D717C; } } }
        public static HRESULT MF_E_BAD_OPL_STRUCTURE_FORMAT { get { unchecked { return (HRESULT)0xC00D717D; } } }
        public static HRESULT MF_E_ITA_UNRECOGNIZED_ANALOG_VIDEO_PROTECTION_GUID { get { unchecked { return (HRESULT)0xC00D717E; } } }
        public static HRESULT MF_E_NO_PMP_HOST { get { unchecked { return (HRESULT)0xC00D717F; } } }
        public static HRESULT MF_E_ITA_OPL_DATA_NOT_INITIALIZED { get { unchecked { return (HRESULT)0xC00D7180; } } }
        public static HRESULT MF_E_ITA_UNRECOGNIZED_ANALOG_VIDEO_OUTPUT { get { unchecked { return (HRESULT)0xC00D7181; } } }
        public static HRESULT MF_E_ITA_UNRECOGNIZED_DIGITAL_VIDEO_OUTPUT { get { unchecked { return (HRESULT)0xC00D7182; } } }
        public static HRESULT MF_E_CLOCK_INVALID_CONTINUITY_KEY { get { unchecked { return (HRESULT)0xC00D9C40; } } }
        public static HRESULT MF_E_CLOCK_NO_TIME_SOURCE { get { unchecked { return (HRESULT)0xC00D9C41; } } }
        public static HRESULT MF_E_CLOCK_STATE_ALREADY_SET { get { unchecked { return (HRESULT)0xC00D9C42; } } }
        public static HRESULT MF_E_CLOCK_NOT_SIMPLE { get { unchecked { return (HRESULT)0xC00D9C43; } } }
        public static HRESULT MF_S_CLOCK_STOPPED { get { unchecked { return (HRESULT)0x000D9C44; } } }
        public static HRESULT MF_E_NO_MORE_DROP_MODES { get { unchecked { return (HRESULT)0xC00DA028; } } }
        public static HRESULT MF_E_NO_MORE_QUALITY_LEVELS { get { unchecked { return (HRESULT)0xC00DA029; } } }
        public static HRESULT MF_E_DROPTIME_NOT_SUPPORTED { get { unchecked { return (HRESULT)0xC00DA02A; } } }
        public static HRESULT MF_E_QUALITYKNOB_WAIT_LONGER { get { unchecked { return (HRESULT)0xC00DA02B; } } }
        public static HRESULT MF_E_QM_INVALIDSTATE { get { unchecked { return (HRESULT)0xC00DA02C; } } }
        public static HRESULT MF_E_TRANSCODE_NO_CONTAINERTYPE { get { unchecked { return (HRESULT)0xC00DA410; } } }
        public static HRESULT MF_E_TRANSCODE_PROFILE_NO_MATCHING_STREAMS { get { unchecked { return (HRESULT)0xC00DA411; } } }
        public static HRESULT MF_E_TRANSCODE_NO_MATCHING_ENCODER { get { unchecked { return (HRESULT)0xC00DA412; } } }
        public static HRESULT MF_E_ALLOCATOR_NOT_INITIALIZED { get { unchecked { return (HRESULT)0xC00DA7F8; } } }
        public static HRESULT MF_E_ALLOCATOR_NOT_COMMITED { get { unchecked { return (HRESULT)0xC00DA7F9; } } }
        public static HRESULT MF_E_ALLOCATOR_ALREADY_COMMITED { get { unchecked { return (HRESULT)0xC00DA7FA; } } }
        public static HRESULT MF_E_STREAM_ERROR { get { unchecked { return (HRESULT)0xC00DA7FB; } } }
        public static HRESULT MF_E_INVALID_STREAM_STATE { get { unchecked { return (HRESULT)0xC00DA7FC; } } }
        public static HRESULT MF_E_HW_STREAM_NOT_CONNECTED { get { unchecked { return (HRESULT)0xC00DA7FD; } } }

        #endregion

        #region Classes

        public class MediaTypeBuilder : IDisposable
        {
            #region Variables

            protected IMFMediaType m_pType = null;

            #endregion

            #region Consructor

            // Create a new media type.
            public MediaTypeBuilder()
            {
            }

            ~MediaTypeBuilder()
            {
                Dispose();
            }

            #endregion

            #region Protected Methods

            protected bool IsValid()
            {
                return m_pType != null;
            }

            protected IMFMediaType GetMediaType()
            {
                ASSERT(IsValid());
                return m_pType;
            }

            #endregion

            #region Public Methods

            // Direct wrappers of IMFMediaType methods.
            // (For these methods, we leave parameter validation to the IMFMediaType implementation.)

            // Retrieves the major type GUID.
            public HRESULT GetMajorType(out Guid pGuid)
            {
                return (HRESULT)GetMediaType().GetMajorType(out pGuid);
            }

            // Specifies whether the media data is compressed
            public HRESULT IsCompressedFormat(out bool pbCompressed)
            {
                return (HRESULT)GetMediaType().IsCompressedFormat(out pbCompressed);
            }

            // Compares two media types and determines whether they are identical.
            public HRESULT IsEqual(IMFMediaType pType, out MFMediaEqual pdwFlags)
            {
                return (HRESULT)GetMediaType().IsEqual(pType, out pdwFlags);
            }

            // Retrieves an alternative representation of the media type.
            public HRESULT GetRepresentation(Guid guidRepresentation, out IntPtr ppvRepresentation)
            {
                return (HRESULT)GetMediaType().GetRepresentation(guidRepresentation, out ppvRepresentation);
            }

            // Frees memory that was allocated by the GetRepresentation method.
            public HRESULT FreeRepresentation(Guid guidRepresentation, IntPtr pvRepresentation)
            {
                return (HRESULT)GetMediaType().FreeRepresentation(guidRepresentation, pvRepresentation);
            }

            // Helper methods

            // CopyFrom: Copy all of the attributes from another media type into this type.
            public HRESULT CopyFrom(MediaTypeBuilder pType)
            {
                if (pType == null)
                {
                    return E_POINTER;
                }
                if (!pType.IsValid())
                {
                    return E_UNEXPECTED;
                }
                return CopyFrom(pType.m_pType);
            }

            public HRESULT CopyFrom(IMFMediaType pType)
            {
                if (pType == null)
                {
                    return E_POINTER;
                }
                return (HRESULT)pType.CopyAllItems(m_pType);
            }

            // Returns the underlying IMFMediaType pointer.
            public HRESULT GetMediaType(out IMFMediaType ppType)
            {
                ppType = m_pType;
                if (!IsValid())
                {
                    ASSERT(false);
                    return E_UNEXPECTED;
                }
                return S_OK;
            }

            // Sets the major type GUID.
            public HRESULT SetMajorType(Guid guid)
            {
                return (HRESULT)GetMediaType().SetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, guid);
            }

            // Retrieves the subtype GUID.
            public HRESULT GetSubType(out Guid pGuid)
            {
                return (HRESULT)GetMediaType().GetGUID(MFAttributesClsid.MF_MT_SUBTYPE, out pGuid);
            }

            // Sets the subtype GUID.
            public HRESULT SetSubType(Guid guid)
            {
                return (HRESULT)GetMediaType().SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, guid);
            }

            // Extracts the FOURCC code from the subtype.
            // Not all subtypes follow this pattern.
            public HRESULT GetFourCC(out int pFourCC)
            {
                ASSERT(IsValid());
                Guid _type;
                pFourCC = 0;
                HRESULT hr = GetSubType(out _type);
                if (hr.Failed) return hr;
                FOURCC _fcc = new FOURCC(_type);
                pFourCC = _fcc;
                return hr;
            }

            //  Queries whether each sample is independent of the other samples in the stream.
            public HRESULT GetAllSamplesIndependent(out bool pbIndependent)
            {
                int i;
                HRESULT hr = (HRESULT)GetMediaType().GetUINT32(MFAttributesClsid.MF_MT_ALL_SAMPLES_INDEPENDENT, out i);
                pbIndependent = i != 0;
                return hr;
            }

            //  Specifies whether each sample is independent of the other samples in the stream.
            public HRESULT SetAllSamplesIndependent(bool bIndependent)
            {
                return (HRESULT)GetMediaType().SetUINT32(MFAttributesClsid.MF_MT_ALL_SAMPLES_INDEPENDENT, bIndependent ? 1 : 0);
            }

            // Queries whether the samples have a fixed size.
            public HRESULT GetFixedSizeSamples(out bool pbFixed)
            {
                int i;
                HRESULT hr = (HRESULT)GetMediaType().GetUINT32(MFAttributesClsid.MF_MT_FIXED_SIZE_SAMPLES, out i);
                pbFixed = i != 0;
                return hr;
            }

            // Specifies whether the samples have a fixed size.
            public HRESULT SetFixedSizeSamples(bool bFixed)
            {
                return (HRESULT)GetMediaType().SetUINT32(MFAttributesClsid.MF_MT_FIXED_SIZE_SAMPLES, bFixed ? 1 : 0);
            }

            // Retrieves the size of each sample, in bytes.
            public HRESULT GetSampleSize(out int pnSize)
            {
                return (HRESULT)GetMediaType().GetUINT32(MFAttributesClsid.MF_MT_SAMPLE_SIZE, out pnSize);
            }

            // Sets the size of each sample, in bytes.
            public HRESULT SetSampleSize(int nSize)
            {
                return (HRESULT)GetMediaType().SetUINT32(MFAttributesClsid.MF_MT_SAMPLE_SIZE, nSize);
            }

            // Retrieves a media type that was wrapped by the MFWrapMediaType function.
            public HRESULT Unwrap(out IMFMediaType ppOriginal)
            {
                return (HRESULT)MFUnwrapMediaType(GetMediaType(), out ppOriginal);
            }

            // The following versions return reasonable defaults if the relevant attribute is not present (zero/FALSE).
            // This is useful for making quick comparisons betweeen media types.

            public bool AllSamplesIndependent()
            {
                return MFGetAttributeUINT32(GetMediaType(), MFAttributesClsid.MF_MT_ALL_SAMPLES_INDEPENDENT, 0) != 0;
            }

            public bool FixedSizeSamples()
            {
                return MFGetAttributeUINT32(GetMediaType(), MFAttributesClsid.MF_MT_FIXED_SIZE_SAMPLES, 0) != 0;
            }

            public int SampleSize()
            {
                return MFGetAttributeUINT32(GetMediaType(), MFAttributesClsid.MF_MT_SAMPLE_SIZE, 0);
            }

            #endregion

            #region Virtual Methods

            // Construct from an existing media type.
            protected virtual HRESULT Init(IMFMediaType pType)
            {
                HRESULT hr = S_OK;
                if (pType == null)
                {
                    hr = (HRESULT)MFCreateMediaType(out m_pType);
                }
                else
                {
                    m_pType = pType;
                }
                return hr;
            }

            #endregion 

            #region Static Methods

            public static HRESULT Create<T>(out T ppTypeBuilder) where T : class, new()
            {
                return Create<T>(null, out ppTypeBuilder);
            }

            public static HRESULT Create<T>(IMFMediaType pType, out T ppTypeBuilder) where T : class, new()
            {
                ppTypeBuilder = new T();
                HRESULT hr = (ppTypeBuilder as MediaTypeBuilder).Init(pType);
                if (hr.Failed)
                {
                    (ppTypeBuilder as IDisposable).Dispose();
                    ppTypeBuilder = null;
                }
                return hr;
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                m_pType = null;
            }

            #endregion
        }

        public class VideoTypeBuilder : MediaTypeBuilder
        {
            #region Constructor

            public VideoTypeBuilder()
                : base()
            {
            }

            #endregion

            #region Overridden Methods

            protected override HRESULT Init(IMFMediaType pType)
            {
                HRESULT hr = base.Init(pType);
                if (hr.Failed) return hr;

                Guid _guid;

                if (pType != null)
                {
                    GetMajorType(out _guid);
                    if (_guid != MediaType.Video)
                    {
                        return MF_E_INVALIDTYPE;
                    }
                }
                else
                {
                    hr = SetMajorType(MediaType.Video);
                }
                return hr;
            }

            #endregion

            #region Public Methods

            // Retrieves a description of how the frames are interlaced.
            public HRESULT GetInterlaceMode(out MFVideoInterlaceMode pmode)
            {
                int i;
                HRESULT hr = (HRESULT)GetMediaType().GetUINT32(MFAttributesClsid.MF_MT_INTERLACE_MODE, out i);
                pmode = (MFVideoInterlaceMode)i;
                return hr;
            }

            // Sets a description of how the frames are interlaced.
            public HRESULT SetInterlaceMode(MFVideoInterlaceMode mode)
            {
                return (HRESULT)GetMediaType().SetUINT32(MFAttributesClsid.MF_MT_INTERLACE_MODE, (int)mode);
            }

            // This returns the default or attempts to compute it, in its absence.
            public HRESULT GetDefaultStride(out int pnStride)
            {
                // First try to get it from the attribute.
                HRESULT hr = (HRESULT)GetMediaType().GetUINT32(MFAttributesClsid.MF_MT_DEFAULT_STRIDE, out pnStride);
                if (hr.Failed)
                {
                    // Attribute not set. See if we can calculate the default stride.
                    int _fcc;
                    int width = 0, height = 0;
                    hr = GetFourCC(out _fcc);
                    hr.Assert();
                    // Now we need the image width and height.
                    if (hr.Succeeded)
                    {
                        hr = GetFrameDimensions(out width,out height);
                    }
                    // Now compute the stride for a particular bitmap type
                    if (hr.Succeeded)
                    {
                        hr = (HRESULT)MFGetStrideForBitmapInfoHeader(_fcc, width, out pnStride);
                    }

                    // Set the attribute for later reference.
                    if (hr.Succeeded)
                    {
                        hr = SetDefaultStride(pnStride);
                    }
                }
                return hr;
            }

            // Sets the default stride. Only appropriate for uncompressed data formats.
            public HRESULT SetDefaultStride(int nStride)
            {
                return (HRESULT)GetMediaType().SetUINT32(MFAttributesClsid.MF_MT_DEFAULT_STRIDE, nStride);
            }

            // Retrieves the width and height of the video frame.
            public HRESULT GetFrameDimensions(out int pdwWidthInPixels, out int pdwHeightInPixels)
            {
                return MFGetAttribute2UINT32asUINT64(GetMediaType(), MFAttributesClsid.MF_MT_FRAME_SIZE, out pdwWidthInPixels, out pdwHeightInPixels);
            }

            // Sets the width and height of the video frame.
            public HRESULT SetFrameDimensions(int dwWidthInPixels, int dwHeightInPixels)
            {
                return MFSetAttribute2UINT32asUINT64(GetMediaType(), MFAttributesClsid.MF_MT_FRAME_SIZE, dwWidthInPixels, dwHeightInPixels);
            }

            // Retrieves the data error rate in bit errors per second
            public HRESULT GetDataBitErrorRate(out int pRate)
            {
                return (HRESULT)GetMediaType().GetUINT32(MFAttributesClsid.MF_MT_AVG_BIT_ERROR_RATE, out pRate);
            }

            // Sets the data error rate in bit errors per second
            public HRESULT SetDataBitErrorRate(int rate)
            {
                return (HRESULT)GetMediaType().SetUINT32(MFAttributesClsid.MF_MT_AVG_BIT_ERROR_RATE, rate);
            }

            // Retrieves the approximate data rate of the video stream.
            public HRESULT GetAverageBitRate(out int pRate)
            {
                return (HRESULT)GetMediaType().GetUINT32(MFAttributesClsid.MF_MT_AVG_BITRATE, out pRate);
            }

            // Sets the approximate data rate of the video stream.
            public HRESULT SetAvgerageBitRate(int rate)
            {
                return (HRESULT)GetMediaType().SetUINT32(MFAttributesClsid.MF_MT_AVG_BITRATE, rate);
            }

            // Retrieves custom color primaries.
            public HRESULT GetCustomVideoPrimaries(out MT_CustomVideoPrimaries pPrimaries)
            {
                pPrimaries = new MT_CustomVideoPrimaries();
                return MFGetBlob(GetMediaType(), MFAttributesClsid.MF_MT_CUSTOM_VIDEO_PRIMARIES, pPrimaries);
            }

            // Sets custom color primaries.
            public HRESULT SetCustomVideoPrimaries(MT_CustomVideoPrimaries primary)
            {
                return MFSetBlob(GetMediaType(), MFAttributesClsid.MF_MT_CUSTOM_VIDEO_PRIMARIES, primary);
            }

            // Gets the number of frames per second.
            public HRESULT GetFrameRate(out int pnNumerator, out int pnDenominator)
            {
                return MFGetAttribute2UINT32asUINT64(GetMediaType(), MFAttributesClsid.MF_MT_FRAME_RATE, out pnNumerator, out pnDenominator);
            }

            // Gets the frames per second as a ratio.
            public HRESULT GetFrameRate(out MFRatio pRatio)
            {
                return GetFrameRate(out pRatio.Numerator, out pRatio.Denominator);
            }

            // Sets the number of frames per second.
            public HRESULT SetFrameRate(int nNumerator, int nDenominator)
            {
                return MFSetAttribute2UINT32asUINT64(GetMediaType(), MFAttributesClsid.MF_MT_FRAME_RATE, nNumerator, nDenominator);
            }

            // Sets the number of frames per second, as a ratio.
            public HRESULT SetFrameRate(MFRatio ratio)
            {
                return MFSetAttribute2UINT32asUINT64(GetMediaType(), MFAttributesClsid.MF_MT_FRAME_RATE, ratio.Numerator, ratio.Denominator);
            }

            // Queries the geometric aperture.
            public HRESULT GetGeometricAperture(out MFVideoArea pArea)
            {
                pArea = new MFVideoArea();
                return MFGetBlob(GetMediaType(), MFAttributesClsid.MF_MT_GEOMETRIC_APERTURE, pArea);
            }

            // Sets the geometric aperture.
            public HRESULT SetGeometricAperture(MFVideoArea area)
            {
                return MFSetBlob(GetMediaType(), MFAttributesClsid.MF_MT_GEOMETRIC_APERTURE, area);
            }

            // Retrieves the maximum number of frames from one key frame to the next.
            public HRESULT GetMaxKeyframeSpacing(out int pnSpacing)
            {
                return (HRESULT)GetMediaType().GetUINT32(MFAttributesClsid.MF_MT_MAX_KEYFRAME_SPACING, out pnSpacing);
            }

            // Sets the maximum number of frames from one key frame to the next.
            public HRESULT SetMaxKeyframeSpacing(int nSpacing)
            {
                return (HRESULT)GetMediaType().SetUINT32(MFAttributesClsid.MF_MT_MAX_KEYFRAME_SPACING, nSpacing);
            }

            // Retrieves the region that contains the valid portion of the signal.
            public HRESULT GetMinDisplayAperture(out MFVideoArea pArea)
            {
                pArea = new MFVideoArea();
                return MFGetBlob(GetMediaType(), MFAttributesClsid.MF_MT_MINIMUM_DISPLAY_APERTURE, pArea);
            }

            // Sets the the region that contains the valid portion of the signal.
            public HRESULT SetMinDisplayAperture(MFVideoArea area)
            {
                return MFSetBlob(GetMediaType(), MFAttributesClsid.MF_MT_MINIMUM_DISPLAY_APERTURE, area);
            }

            // Retrieves the aspect ratio of the output rectangle for a video media type.
            public HRESULT GetPadControlFlags(out MFVideoPadFlags pFlags)
            {
                int i;
                HRESULT hr = (HRESULT)GetMediaType().GetUINT32(MFAttributesClsid.MF_MT_PAD_CONTROL_FLAGS, out i);
                pFlags = (MFVideoPadFlags)i;
                return hr;
            }

            // Sets the aspect ratio of the output rectangle for a video media type.
            public HRESULT SetPadControlFlags(MFVideoPadFlags flags)
            {
                return (HRESULT)GetMediaType().SetUINT32(MFAttributesClsid.MF_MT_PAD_CONTROL_FLAGS, (int)flags);
            }

            // Retrieves an array of palette entries for a video media type.
            public HRESULT GetPaletteEntries(out MFPaletteEntry[] paEntries, int nEntries)
            {
                paEntries = new MFPaletteEntry[nEntries];
                return MFGetBlob(GetMediaType(), MFAttributesClsid.MF_MT_PALETTE, paEntries);
            }

            // Sets an array of palette entries for a video media type.
            public HRESULT SetPaletteEntries(MFPaletteEntry[] paEntries, int nEntries)
            {
                return MFSetBlob(GetMediaType(), MFAttributesClsid.MF_MT_PALETTE, paEntries);
            }

            // Retrieves the number of palette entries.
            public HRESULT GetNumPaletteEntries(out int pnEntries)
            {
                pnEntries = 0;
                int iSize = Marshal.SizeOf(typeof(MFPaletteEntry));
                int nBytes = 0;
                HRESULT hr = (HRESULT)GetMediaType().GetBlobSize(MFAttributesClsid.MF_MT_PALETTE, out nBytes);
                if (hr.Succeeded)
                {
                    if (nBytes % iSize != 0)
                    {
                        return E_UNEXPECTED;
                    }
                    pnEntries = nBytes / iSize;
                }
                return hr;
            }

            // Queries the 4×3 region of video that should be displayed in pan/scan mode.
            public HRESULT GetPanScanAperture(out MFVideoArea pArea)
            {
                pArea = new MFVideoArea();
                return MFGetBlob(GetMediaType(), MFAttributesClsid.MF_MT_PAN_SCAN_APERTURE, pArea);
            }

            // Sets the 4×3 region of video that should be displayed in pan/scan mode.
            public HRESULT SetPanScanAperture(MFVideoArea area)
            {
                return MFSetBlob(GetMediaType(), MFAttributesClsid.MF_MT_PAN_SCAN_APERTURE, area);
            }

            // Queries whether pan/scan mode is enabled.
            public HRESULT IsPanScanEnabled(out bool pBool)
            {
                int i;
                HRESULT hr = (HRESULT)GetMediaType().GetUINT32(MFAttributesClsid.MF_MT_PAN_SCAN_ENABLED, out i);
                pBool = i != 0;
                return hr;
            }

            // Sets whether pan/scan mode is enabled.
            public HRESULT SetPanScanEnabled(bool bEnabled)
            {
                int i = 0;
                if (bEnabled)
                {
                    i = 1;
                }
                return (HRESULT)GetMediaType().SetUINT32(MFAttributesClsid.MF_MT_PAN_SCAN_ENABLED, i);
            }

            // Queries the pixel aspect ratio
            public HRESULT GetPixelAspectRatio(out int pnNumerator, out int pnDenominator)
            {
                return MFGetAttribute2UINT32asUINT64(GetMediaType(), MFAttributesClsid.MF_MT_PIXEL_ASPECT_RATIO, out pnNumerator, out pnDenominator);
            }

            // Sets the pixel aspect ratio
            public HRESULT SetPixelAspectRatio(int nNumerator, int nDenominator)
            {
                return MFSetAttribute2UINT32asUINT64(GetMediaType(), MFAttributesClsid.MF_MT_PIXEL_ASPECT_RATIO, nNumerator, nDenominator);
            }

            public HRESULT SetPixelAspectRatio(MFRatio ratio)
            {
                return MFSetAttribute2UINT32asUINT64(GetMediaType(), MFAttributesClsid.MF_MT_PIXEL_ASPECT_RATIO, ratio.Numerator, ratio.Denominator);
            }

            // Queries the intended aspect ratio.
            public HRESULT GetSourceContentHint(out MFVideoSrcContentHintFlags pFlags)
            {
                int i;
                HRESULT hr = (HRESULT)GetMediaType().GetUINT32(MFAttributesClsid.MF_MT_SOURCE_CONTENT_HINT, out i);
                pFlags = (MFVideoSrcContentHintFlags)i;
                return hr;
            }

            // Sets the intended aspect ratio.
            public HRESULT SetSourceContentHint(MFVideoSrcContentHintFlags nFlags)
            {
                return (HRESULT)GetMediaType().SetUINT32(MFAttributesClsid.MF_MT_SOURCE_CONTENT_HINT, (int)nFlags);
            }

            // Queries an enumeration which represents the conversion function from RGB to R'G'B'.
            public HRESULT GetTransferFunction(out MFVideoTransferFunction pnFxn)
            {
                int i;
                HRESULT hr = (HRESULT)GetMediaType().GetUINT32(MFAttributesClsid.MF_MT_TRANSFER_FUNCTION, out i);
                pnFxn = (MFVideoTransferFunction)i;
                return hr;
            }

            // Set an enumeration which represents the conversion function from RGB to R'G'B'.
            public HRESULT SetTransferFunction(MFVideoTransferFunction nFxn)
            {
                return (HRESULT)GetMediaType().SetUINT32(MFAttributesClsid.MF_MT_TRANSFER_FUNCTION, (int)nFxn);
            }

            // Queries how chroma was sampled for a Y'Cb'Cr' video media type.
            public HRESULT GetChromaSiting(out MFVideoChromaSubsampling pSampling)
            {
                int i;
                HRESULT hr = (HRESULT)GetMediaType().GetUINT32(MFAttributesClsid.MF_MT_VIDEO_CHROMA_SITING, out i);
                pSampling = (MFVideoChromaSubsampling)i;
                return hr;
            }

            // Sets how chroma was sampled for a Y'Cb'Cr' video media type.
            public HRESULT SetChromaSiting(MFVideoChromaSubsampling nSampling)
            {
                return (HRESULT)GetMediaType().SetUINT32(MFAttributesClsid.MF_MT_VIDEO_CHROMA_SITING, (int)nSampling);
            }

            // Queries the optimal lighting conditions for viewing.
            public HRESULT GetVideoLighting(out MFVideoLighting pLighting)
            {
                int i;
                HRESULT hr = (HRESULT)GetMediaType().GetUINT32(MFAttributesClsid.MF_MT_VIDEO_LIGHTING, out i);
                pLighting = (MFVideoLighting)i;
                return hr;
            }

            // Sets the optimal lighting conditions for viewing.
            public HRESULT SetVideoLighting(MFVideoLighting nLighting)
            {
                return (HRESULT)GetMediaType().SetUINT32(MFAttributesClsid.MF_MT_VIDEO_LIGHTING, (int)nLighting);
            }

            // Queries the nominal range of the color information in a video media type.
            public HRESULT GetVideoNominalRange(out MFNominalRange pRange)
            {
                int i;
                HRESULT hr = (HRESULT)GetMediaType().GetUINT32(MFAttributesClsid.MF_MT_VIDEO_NOMINAL_RANGE, out i);
                pRange = (MFNominalRange)i;
                return hr;
            }

            // Sets the nominal range of the color information in a video media type.
            public HRESULT SetVideoNominalRange(MFNominalRange nRange)
            {
                return (HRESULT)GetMediaType().SetUINT32(MFAttributesClsid.MF_MT_VIDEO_NOMINAL_RANGE, (int)nRange);
            }

            // Queries the color primaries for a video media type.
            public HRESULT GetVideoPrimaries(out MFVideoPrimaries pPrimaries)
            {
                int i;
                HRESULT hr = (HRESULT)GetMediaType().GetUINT32(MFAttributesClsid.MF_MT_VIDEO_PRIMARIES, out i);
                pPrimaries = (MFVideoPrimaries)i;
                return hr;
            }

            // Sets the color primaries for a video media type.
            public HRESULT SetVideoPrimaries(MFVideoPrimaries nPrimaries)
            {
                return (HRESULT)GetMediaType().SetUINT32(MFAttributesClsid.MF_MT_VIDEO_PRIMARIES, (int)nPrimaries);
            }

            // Gets a enumeration representing the conversion matrix from the
            // Y'Cb'Cr' color space to the R'G'B' color space.
            public HRESULT GetYUVMatrix(out MFVideoTransferMatrix pMatrix)
            {
                int i;
                HRESULT hr = (HRESULT)GetMediaType().GetUINT32(MFAttributesClsid.MF_MT_YUV_MATRIX, out i);
                pMatrix = (MFVideoTransferMatrix)i;
                return hr;
            }

            // Sets an enumeration representing the conversion matrix from the
            // Y'Cb'Cr' color space to the R'G'B' color space.
            public HRESULT SetYUVMatrix(MFVideoTransferMatrix nMatrix)
            {
                return (HRESULT)GetMediaType().SetUINT32(MFAttributesClsid.MF_MT_YUV_MATRIX, (int)nMatrix);
            }

            //
            // The following versions return reasonable defaults if the relevant attribute is not present (zero/FALSE).
            // This is useful for making quick comparisons betweeen media types.
            //

            public MFRatio GetPixelAspectRatio() // Defaults to 1:1 (square pixels)
            {
                MFRatio PAR = new MFRatio();

                try
                {
                    MFGetAttribute2UINT32asUINT64(GetMediaType(), MFAttributesClsid.MF_MT_PIXEL_ASPECT_RATIO, out PAR.Numerator, out PAR.Denominator);
                }
                catch
                {
                    PAR.Numerator = 1;
                    PAR.Denominator = 1;
                }
                return PAR;
            }

            public bool IsPanScanEnabled() // Defaults to FALSE
            {
                return MFGetAttributeUINT32(GetMediaType(), MFAttributesClsid.MF_MT_PAN_SCAN_ENABLED, 0) != 0;
            }

            // Returns (in this order)
            // 1. The pan/scan region, only if pan/scan mode is enabled.
            // 2. The geometric aperture.
            // 3. The entire video area.
            public HRESULT GetVideoDisplayArea(out MFVideoArea pArea)
            {
                HRESULT hr = S_OK;
                bool bPanScan = false;
                int width = 0, height = 0;
                pArea = new MFVideoArea();

                bPanScan = MFGetAttributeUINT32(GetMediaType(), MFAttributesClsid.MF_MT_PAN_SCAN_ENABLED, 0) != 0;

                // In pan/scan mode, try to get the pan/scan region.
                if (bPanScan)
                {
                    hr = MFGetBlob(GetMediaType(), MFAttributesClsid.MF_MT_PAN_SCAN_APERTURE, pArea);
                }

                // If not in pan/scan mode, or there is not pan/scan region, get the geometric aperture.
                if (!bPanScan || hr == MF_E_ATTRIBUTENOTFOUND)
                {
                    hr = MFGetBlob(GetMediaType(), MFAttributesClsid.MF_MT_GEOMETRIC_APERTURE, pArea);

                    // Default: Use the entire video area.
                    if (hr == MF_E_ATTRIBUTENOTFOUND)
                    {
                        hr = MFGetAttribute2UINT32asUINT64(GetMediaType(), MFAttributesClsid.MF_MT_FRAME_SIZE, out width, out height);
                        pArea.MakeArea(0, 0, width, height);
                    }
                }
                return hr;
            }

            #endregion
        }

        #endregion

        #region Functions

        public static HRESULT MFSetBlob(IMFAttributes p, Guid g, object o)
        {
            int iSize = Marshal.SizeOf(o);
            byte[] b = new byte[iSize];

            GCHandle h = GCHandle.Alloc(b, GCHandleType.Pinned);
            try
            {
                IntPtr ip = h.AddrOfPinnedObject();

                Marshal.StructureToPtr(o, ip, false);
            }
            finally
            {
                h.Free();
            }
            return (HRESULT)p.SetBlob(g, b, iSize);
        }

        public static HRESULT MFGetBlob(IMFAttributes p, Guid g, object obj)
        {
            HRESULT hr;
            int iSize;
            int i;

            // Get the blob into a byte array
            hr = (HRESULT)p.GetBlobSize(g, out iSize);
            if (hr.Failed) return hr;

            byte[] b = new byte[iSize];
            hr = (HRESULT)p.GetBlob(g, b, iSize, out i);
            if (hr.Failed) return hr;

            GCHandle h = GCHandle.Alloc(b, GCHandleType.Pinned);

            try
            {
                IntPtr ip = h.AddrOfPinnedObject();

                // Convert the byte array to an IntPtr
                Marshal.PtrToStructure(ip, obj);
            }
            finally
            {
                h.Free();
            }
            return hr;
        }

        public static HRESULT MFSetAttribute2UINT32asUINT64(IMFAttributes pAttributes, Guid g, int nNumerator, int nDenominator)
        {
            long ul = nNumerator;

            ul <<= 32;
            ul |= (UInt32)nDenominator;

            return (HRESULT)pAttributes.SetUINT64(g, ul);
        }

        public static HRESULT MFGetAttribute2UINT32asUINT64(IMFAttributes pAttributes, Guid g, out int nNumerator, out int nDenominator)
        {
            long ul;

            HRESULT hr = (HRESULT)pAttributes.GetUINT64(g, out ul);
            Marshal.ThrowExceptionForHR(hr);

            nDenominator = (int)ul;
            nNumerator = (int)(ul >> 32);
            return hr;
        }

        public static bool AreMediaTypesEqual(IMFMediaType pType1, IMFMediaType pType2)
        {
            if ((pType1 == null) && (pType2 == null))
            {
                return true; // Both are NULL.
            }
            else if ((pType1 == null) || (pType2 == null))
            {
                return false; // One is NULL.
            }

            MFMediaEqual dwFlags;
            int hr = pType1.IsEqual(pType2, out dwFlags);

            return (hr == 0);
        }

        public static int MulDiv(long a, long b, long c)
        {
            // Max Rate = Refresh Rate / Frame Rate
            long l = a * b;

            return (int)(l / c);
        }

        public static int MFTimeToMsec(long time)
        {
            const long ONE_SECOND = 10000000; // One second in hns
            const int ONE_MSEC = 1000;       // One msec in hns

            return (int)(time / (ONE_SECOND / ONE_MSEC));
        }

        public static HRESULT GetFrameRate(IMFMediaType pMediaType, out MFRatio fps)
        {
            long i64;

            HRESULT hr = (HRESULT)pMediaType.GetUINT64(MFAttributesClsid.MF_MT_FRAME_RATE, out i64);

            fps.Numerator = (int)(i64 >> 32);
            fps.Denominator = (int)i64;

            return hr;
        }

        public static int MFGetAttributeUINT32(IMFAttributes pAttributes, Guid guidKey, int unDefault)
        {
            HRESULT hr;
            int unRet;

            hr = (HRESULT)pAttributes.GetUINT32(guidKey, out unRet);
            if (hr.Succeeded) return unRet;

            return unDefault;
        }

        public static HRESULT SetMixerSourceRect(IMFTransform pMixer, MFVideoNormalizedRect nrcSource)
        {
            if (pMixer == null)
            {
                return E_POINTER;
            }

            HRESULT hr = S_OK;
            IMFAttributes pAttributes = null;

            hr = (HRESULT)pMixer.GetAttributes(out pAttributes);
            if (hr.Failed) return hr;

            return MFSetBlob(pAttributes, MFAttributesClsid.VIDEO_ZOOM_RECT, nrcSource);
        }

        public static HRESULT ValidateVideoArea(MFVideoArea area, int width, int height)
        {
            float fOffsetX = area.OffsetX.GetOffset();
            float fOffsetY = area.OffsetY.GetOffset();

            if (((int)fOffsetX + area.Area.Width > width) ||
                 ((int)fOffsetY + area.Area.Height > height))
            {
                return MF_E_INVALIDMEDIATYPE;
            }
            return S_OK;
        }

        public static DsRect CorrectAspectRatio(DsRect src, MFRatio srcPAR, MFRatio destPAR)
        {
            // Start with a rectangle the same size as src, but offset to the origin (0,0).
            DsRect rc = new DsRect(0, 0, src.right - src.left, src.bottom - src.top);

            // If the source and destination have the same PAR, there is nothing to do.
            // Otherwise, adjust the image size, in two steps:
            //  1. Transform from source PAR to 1:1
            //  2. Transform from 1:1 to destination PAR.

            if ((srcPAR.Numerator != destPAR.Numerator) || (srcPAR.Denominator != destPAR.Denominator))
            {
                // Correct for the source's PAR.

                if (srcPAR.Numerator > srcPAR.Denominator)
                {
                    // The source has "wide" pixels, so stretch the width.
                    rc.right = MulDiv(rc.right, srcPAR.Numerator, srcPAR.Denominator);
                }
                else if (srcPAR.Numerator > srcPAR.Denominator)
                {
                    // The source has "tall" pixels, so stretch the height.
                    rc.bottom = MulDiv(rc.bottom, srcPAR.Denominator, srcPAR.Numerator);
                }
                // else: PAR is 1:1, which is a no-op.


                // Next, correct for the target's PAR. This is the inverse operation of the previous.

                if (destPAR.Numerator > destPAR.Denominator)
                {
                    // The destination has "tall" pixels, so stretch the width.
                    rc.bottom = MulDiv(rc.bottom, destPAR.Denominator, destPAR.Numerator);
                }
                else if (destPAR.Numerator > destPAR.Denominator)
                {
                    // The destination has "fat" pixels, so stretch the height.
                    rc.right = MulDiv(rc.right, destPAR.Numerator, destPAR.Denominator);
                }
                // else: PAR is 1:1, which is a no-op.
            }

            return rc;
        }

        public static HRESULT SetDesiredSampleTime(IMFSample pSample, long hnsSampleTime, long hnsDuration)
        {
            if (pSample == null) return E_POINTER;

            IMFDesiredSample pDesired = (IMFDesiredSample)pSample;

            // This method has no return value.
            pDesired.SetDesiredSampleTimeAndDuration(hnsSampleTime, hnsDuration);

            return S_OK;
        }

        #endregion

        #region API

        [DllImport("dxva2.DLL", ExactSpelling = true, PreserveSig = false), SuppressUnmanagedCodeSecurity]
        public extern static int DXVA2CreateDirect3DDeviceManager9(
                out uint pResetToken,
                out IDirect3DDeviceManager9 ppDXVAManager
                );

        [DllImport("mf.dll"), SuppressUnmanagedCodeSecurity]
        public extern static int MFGetService(
                [In, MarshalAs(UnmanagedType.IUnknown)] object punkObject,
                [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidService,
                [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
                [Out] out IntPtr ppvObject
                );

        [DllImport("evr.dll"), SuppressUnmanagedCodeSecurity]
        public extern static int MFCreateVideoSampleFromSurface(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnkSurface,
            [Out] out IMFSample ppSample
            );

        [DllImport("mfplat.dll"), SuppressUnmanagedCodeSecurity]
        public static extern int MFFrameRateToAverageTimePerFrame(
            [In] int unNumerator,
            [In] int unDenominator,
            out long punAverageTimePerFrame
            );

        [DllImport("mfplat.dll"), SuppressUnmanagedCodeSecurity]
        public static extern int MFCreateMediaType(
            out IMFMediaType ppMFType
        );

        [DllImport("mfplat.dll"), SuppressUnmanagedCodeSecurity]
        public static extern int MFCreateSample(
            out IMFSample ppIMFSample
        );

        [DllImport("mfplat.dll"), SuppressUnmanagedCodeSecurity]
        public static extern int MFCreateMemoryBuffer(
            int cbMaxLength,
            out IMFMediaBuffer ppBuffer
        );

        [DllImport("mfplat.dll"), SuppressUnmanagedCodeSecurity]
        public static extern int MFShutdown();

        [DllImport("mfplat.dll"), SuppressUnmanagedCodeSecurity]
        public static extern int MFStartup(
            int Version,
            MFStartup dwFlags
        );

        [DllImport("mfplat.dll"), SuppressUnmanagedCodeSecurity]
        public static extern int MFCreateAlignedMemoryBuffer(
            [In] int cbMaxLength,
            [In] int cbAligment,
            out IMFMediaBuffer ppBuffer);

        [DllImport("mfplat.dll"), SuppressUnmanagedCodeSecurity]
        public static extern long MFGetSystemTime();


        [DllImport("mfplat.dll", ExactSpelling = true), SuppressUnmanagedCodeSecurity]
        public static extern int MFCreateAMMediaTypeFromMFMediaType(
            [In] IMFMediaType pMFType,
            [In, MarshalAs(UnmanagedType.Struct)] Guid guidFormatBlockType,
            out AMMediaType ppAMType // delete with DeleteMediaType
            );

        [DllImport("mfplat.dll", ExactSpelling = true), SuppressUnmanagedCodeSecurity]
        public static extern int MFInitMediaTypeFromAMMediaType(
            [In] IMFMediaType pMFType,
            [In] AMMediaType pAMType
            );

        [DllImport("mfplat.dll", ExactSpelling = true), SuppressUnmanagedCodeSecurity]
        public static extern int MFInitMediaTypeFromVideoInfoHeader(
            [In] IMFMediaType pMFType,
            VideoInfoHeader pVIH,
            [In] int cbBufSize,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid pSubtype
            );

        [DllImport("mfplat.dll", ExactSpelling = true), SuppressUnmanagedCodeSecurity]
        public static extern int MFInitMediaTypeFromVideoInfoHeader2(
            [In] IMFMediaType pMFType,
            VideoInfoHeader2 pVIH2,
            [In] int cbBufSize,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid pSubtype
            );

        [DllImport("evr.dll", ExactSpelling = true), SuppressUnmanagedCodeSecurity]
        public static extern int MFCreateVideoMediaType(
            MFVideoFormat pVideoFormat,
            out IMFVideoMediaType ppIVideoMediaType
            );

        [DllImport("mfplat.dll", ExactSpelling = true), SuppressUnmanagedCodeSecurity]
        public static extern int MFUnwrapMediaType(
            [In] IMFMediaType pWrap,
            out IMFMediaType ppOrig
            );

        [DllImport("evr.dll", ExactSpelling = true), SuppressUnmanagedCodeSecurity]
        public static extern int MFGetStrideForBitmapInfoHeader(
            int format,
            int dwWidth,
            out int pStride
            );

        #endregion
    }

    #endregion

    #region Filter

    [Guid("FA10746C-9B63-4b6c-BC49-FC300EA5F256")]
    public class EVRRenderer : DSFilter
    {
        public EVRRenderer()
            : base()
        {
        }
    }

    #endregion

}