using System.Runtime.InteropServices;

namespace Macro.Common.Media.IMAPI2
{
    public enum IMAPI_FORMAT2_DATA_MEDIA_STATE
    {
        /// <summary>
        /// Indicates that the interface does not know the media state.
        /// </summary>
        [TypeLibVar((short)0x40)]
        IMAPI_FORMAT2_DATA_MEDIA_STATE_UNKNOWN = 0,
        /// <summary>
        /// Write operations can occur on used portions of the disc.
        /// </summary>
        IMAPI_FORMAT2_DATA_MEDIA_STATE_OVERWRITE_ONLY = 1,
        /// <summary>
        /// Media is randomly writable. This indicates that a single session can be written to this disc.
        /// </summary>
        IMAPI_FORMAT2_DATA_MEDIA_STATE_RANDOMLY_WRITABLE = 1,
        /// <summary>
        /// Media has never been used, or has been erased.
        /// </summary>
        IMAPI_FORMAT2_DATA_MEDIA_STATE_BLANK = 2,
        /// <summary>
        /// Media is appendable (supports multiple sessions).
        /// </summary>
        IMAPI_FORMAT2_DATA_MEDIA_STATE_APPENDABLE = 4,
        /// <summary>
        /// Media can have only one additional session added to it, or the media does not support multiple sessions.
        /// </summary>
        IMAPI_FORMAT2_DATA_MEDIA_STATE_FINAL_SESSION = 8,
        /// <summary>
        /// Reports information (but not errors) about the media state.
        /// </summary>
        IMAPI_FORMAT2_DATA_MEDIA_STATE_INFORMATIONAL_MASK = 15,
        /// <summary>
        /// Media is not usable by this interface. The media might require an erase or other recovery.
        /// </summary>
        IMAPI_FORMAT2_DATA_MEDIA_STATE_DAMAGED = 0x400, //1024
        /// <summary>
        /// Media must be erased prior to use by this interface.
        /// </summary>
        IMAPI_FORMAT2_DATA_MEDIA_STATE_ERASE_REQUIRED = 0x800,//2048
        /// <summary>
        /// Media has a partially written last session, which is not supported by this interface.
        /// </summary>
        IMAPI_FORMAT2_DATA_MEDIA_STATE_NON_EMPTY_SESSION = 0x1000, //4096
        /// <summary>
        /// Media or drive is write-protected.
        /// </summary>
        IMAPI_FORMAT2_DATA_MEDIA_STATE_WRITE_PROTECTED = 0x2000, //8192
        /// <summary>
        /// Media cannot be written to (finalized).
        /// </summary>
        IMAPI_FORMAT2_DATA_MEDIA_STATE_FINALIZED = 0x4000, //16384
        /// <summary>
        /// Media is not supported by this interface.
        /// </summary>
        IMAPI_FORMAT2_DATA_MEDIA_STATE_UNSUPPORTED_MEDIA = 0x8000, //32768
        /// <summary>
        /// Reports an unsupported media state.
        /// </summary>
        IMAPI_FORMAT2_DATA_MEDIA_STATE_UNSUPPORTED_MASK = 0xfc00
    }
}