#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion
using System;
using System.Reflection;

namespace Macro.Common.Media
{
    public class ImapiReturnValues
    {

        public static string GetName(int value)
        {

            Type t = typeof(ImapiReturnValues);
            foreach (var field in t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
            {

                ulong constValue = unchecked((ulong)field.GetValue(null));
                ulong errorValue = unchecked((uint)value);

                if (constValue == errorValue)
                {
                    return field.Name;
                }
            }
            return string.Empty;

        }

        // Parsed with an IronPython, information taken from  http://msdn.microsoft.com/en-us/library/cc835244%28VS.85%29.aspx

        #region Constants


        /// <summary>
        /// The disc did not pass burn verification and may contain corrupt data or be unusable.
        /// </summary>
        public const ulong E_IMAPI_BURN_VERIFICATION_FAILED = 0xC0AA0007L;

        /// <summary>
        /// The request was canceled.
        /// </summary>
        public const ulong E_IMAPI_REQUEST_CANCELLED = 0xC0AA0002;

        /// <summary>
        /// The request requires a current disc recorder to be selected.
        /// </summary>
        public const ulong E_IMAPI_RECORDER_REQUIRED = 0xC0AA0003;

        /// <summary>
        /// No write operation is currently in progress.
        /// </summary>
        public const ulong S_IMAPI_WRITE_NOT_IN_PROGRESS = 0x00AA0302L;

        /// <summary>
        /// The requested write speed was not supported by the drive and the speed was adjusted.
        /// </summary>
        public const ulong S_IMAPI_SPEEDADJUSTED = 0x00AA0004;

        /// <summary>
        /// The requested rotation type was not supported by the drive and the rotation type was adjusted.
        /// </summary>
        public const ulong S_IMAPI_ROTATIONADJUSTED = 0x00AA0005;

        /// <summary>
        /// The requested write speed and rotation type were not supported by the drive and they were both adjusted.
        /// </summary>
        public const ulong S_IMAPI_BOTHADJUSTED = 0x00AA0006;

        /// <summary>
        /// The device accepted the command, but returned sense data, indicating an error.
        /// </summary>
        public const ulong S_IMAPI_COMMAND_HAS_SENSE_DATA = 0x00AA0200;

        /// <summary>
        /// The image has become read-only due to a call to IRawCDImageCreator::CreateResultImage. As a result the object can no ulonger be modified.
        /// </summary>
        public const ulong E_IMAPI_RAW_IMAGE_IS_READ_ONLY = 0x80AA0A00L;

        /// <summary>
        /// No more tracks may be added. CD media is restricted to a range of 1-99 tracks.
        /// </summary>
        public const ulong E_IMAPI_RAW_IMAGE_TOO_MANY_TRACKS = 0x80AA0A01L;

        /// <summary>
        ///Tracks must be added to the image before using this function.
        /// </summary>
        public const ulong E_IMAPI_RAW_IMAGE_NO_TRACKS = 0x80AA0A03L;
        /// <summary>
        ///The requested sector type is not supported.
        /// </summary>
        public const ulong E_IMAPI_RAW_IMAGE_SECTOR_TYPE_NOT_SUPPORTED = 0x80AA0A02L;
        /// <summary>
        ///Tracks may not be added to the image prior to the use of this function.
        /// </summary>
        public const ulong E_IMAPI_RAW_IMAGE_TRACKS_ALREADY_ADDED = 0x80AA0A04L;
        /// <summary>
        ///Adding this track would exceed the limitations of the start of the leadout.
        /// </summary>
        public const ulong E_IMAPI_RAW_IMAGE_INSUFFICIENT_SPACE = 0x80AA0A05L;
        /// <summary>
        ///Adding this track would exceed the 99 index limit.
        /// </summary>
        public const ulong E_IMAPI_RAW_IMAGE_TOO_MANY_TRACK_INDEXES = 0x80AA0A06L;
        /// <summary>
        ///The specified LBA offset is not in the list of track indexes.
        /// </summary>
        public const ulong E_IMAPI_RAW_IMAGE_TRACK_INDEX_NOT_FOUND = 0x80AA0A07L;
        /// <summary>
        ///Index 1 (LBA offset zero) cannot be cleared.
        /// </summary>
        public const ulong E_IMAPI_RAW_IMAGE_TRACK_INDEX_OFFSET_ZERO_CANNOT_BE_CLEARED = 0x80AA0A09L;
        /// <summary>
        ///Each index must have a minimum size of ten sectors.
        /// </summary>
        public const ulong E_IMAPI_RAW_IMAGE_TRACK_INDEX_TOO_CLOSE_TO_OTHER_INDEX = 0x80AA0A0AL;
        /// <summary>
        ///The device reported that the requested mode page (and type) is not present.
        /// </summary>
        public const ulong E_IMAPI_RECORDER_NO_SUCH_MODE_PAGE = 0xC0AA0201;


        /// <summary>
        ///There is no media in the device.
        /// </summary>
        public const ulong E_IMAPI_RECORDER_MEDIA_NO_MEDIA = 0xC0AA0202;


        /// <summary>
        ///The media is not compatible or of unknown physical format.
        /// </summary>
        public const ulong E_IMAPI_RECORDER_MEDIA_INCOMPATIBLE = 0xC0AA0203;
        /// <summary>
        ///The media is inserted upside down.
        /// </summary>
        public const ulong E_IMAPI_RECORDER_MEDIA_UPSIDE_DOWN = 0xC0AA0204;
        /// <summary>
        ///The drive reported that it is in the process of becoming ready. Please try the request again later.
        /// </summary>
        public const ulong E_IMAPI_RECORDER_MEDIA_BECOMING_READY = 0xC0AA0205;
        /// <summary>
        ///The media is currently being formatted. Please wait for the format to complete before attempting to use the media.
        /// </summary>
        public const ulong E_IMAPI_RECORDER_MEDIA_FORMAT_IN_PROGRESS = 0xC0AA0206;
        /// <summary>
        ///The drive reported that it is performing a ulong-running operation, such as finishing a write. The drive may be unusable for a ulong period of time.
        /// </summary>
        public const ulong E_IMAPI_RECORDER_MEDIA_BUSY = 0xC0AA0207;
        /// <summary>
        ///The drive reported that the combination of parameters provided in the mode page for a MODE SELECT command were not supported.
        /// </summary>
        public const ulong E_IMAPI_RECORDER_INVALID_MODE_PARAMETERS = 0xC0AA0208;
        /// <summary>
        ///The drive reported that the media is write protected.
        /// </summary>
        public const ulong E_IMAPI_RECORDER_MEDIA_WRITE_PROTECTED = 0xC0AA0209;
        /// <summary>
        ///The feature page requested is not supported by the device.
        /// </summary>
        public const ulong E_IMAPI_RECORDER_NO_SUCH_FEATURE = 0xC0AA020A;
        /// <summary>
        ///The feature page requested is supported, but is not marked as current.
        /// </summary>
        public const ulong E_IMAPI_RECORDER_FEATURE_IS_NOT_CURRENT = 0xC0AA020B;
        /// <summary>
        ///The drive does not support the GET CONFIGURATION command.
        /// </summary>
        public const ulong E_IMAPI_RECORDER_GET_CONFIGURATION_NOT_SUPPORTED = 0xC0AA020C;
        /// <summary>
        ///The device failed to accept the command within the timeout period. This may be caused by the device having entered an inconsistent state, or the timeout value for the command may need to be increased.
        /// </summary>
        public const ulong E_IMAPI_RECORDER_COMMAND_TIMEOUT = 0xC0AA020D;
        /// <summary>
        ///The device failed to accept the command within the timeout period. This may be caused by the device having entered an inconsistent state, or the timeout value for the command may need to be increased.
        /// </summary>
        public const ulong E_IMAPI_RECORDER_DVD_STRUCTURE_NOT_PRESENT = 0xC0AA020E;
        /// <summary>
        ///The media's speed is incompatible with the device. This may be caused by using higher or lower speed media than the range of speeds supported by the device.
        /// </summary>
        public const ulong E_IMAPI_RECORDER_MEDIA_SPEED_MISMATCH = 0xC0AA020F;
        /// <summary>
        ///The device associated with this recorder during the last operation has been exclusively locked, causing this operation to fail.
        /// </summary>
        public const ulong E_IMAPI_RECORDER_LOCKED = 0xC0AA0210;
        /// <summary>
        ///The client name is not valid.
        /// </summary>
        public const ulong E_IMAPI_RECORDER_CLIENT_NAME_IS_NOT_VALID = 0xC0AA0211;
        /// <summary>
        ///The device reported unexpected or invalid data for a command.
        /// </summary>
        public const ulong E_IMAPI_RECORDER_INVALID_RESPONSE_FROM_DEVICE = 0xC0AA02FF;
        /// <summary>
        ///The write failed because the drive did not receive data quickly enough to continue writing. Moving the source data to the local computer, reducing the write speed, or enabling a "buffer underrun free" setting may resolve this issue.
        /// </summary>
        public const ulong E_IMAPI_LOSS_OF_STREAMING = 0xC0AA0300;
        /// <summary>
        ///The write failed because the drive returned error information that could not be recovered from.
        /// </summary>
        public const ulong E_IMAPI_UNEXPECTED_RESPONSE_FROM_DEVICE = 0xC0AA0301;
        /// <summary>
        ///There is currently a write operation in progress.
        /// </summary>
        public const ulong E_IMAPI_DF2DATA_WRITE_IN_PROGRESS = 0xC0AA0400;
        /// <summary>
        ///There is no write operation currently in progress.
        /// </summary>
        public const ulong E_IMAPI_DF2DATA_WRITE_NOT_IN_PROGRESS = 0xC0AA0401;
        /// <summary>
        ///The requested operation is only valid with supported media.
        /// </summary>
        public const ulong E_IMAPI_DF2DATA_INVALID_MEDIA_STATE = 0xC0AA0402;
        /// <summary>
        ///The provided stream to write is not supported.
        /// </summary>
        public const ulong E_IMAPI_DF2DATA_STREAM_NOT_SUPPORTED = 0xC0AA0403;
        /// <summary>
        ///The provided stream to write is too large for the currently inserted media.
        /// </summary>
        public const ulong E_IMAPI_DF2DATA_STREAM_TOO_LARGE_FOR_CURRENT_MEDIA = 0xC0AA0404;
        /// <summary>
        ///Overwriting non-blank media is not allowed without the ForceOverwrite property set to VARIANT_TRUE.
        /// </summary>
        public const ulong E_IMAPI_DF2DATA_MEDIA_NOT_BLANK = 0xC0AA0405;
        /// <summary>
        ///The current media type is unsupported.
        /// </summary>
        public const ulong E_IMAPI_DF2DATA_MEDIA_IS_NOT_SUPPORTED = 0xC0AA0406;
        /// <summary>
        ///This device does not support the operations required by this disc format.
        /// </summary>
        public const ulong E_IMAPI_DF2DATA_RECORDER_NOT_SUPPORTED = 0xC0AA0407;
        /// <summary>
        ///The client name is not valid.
        /// </summary>
        public const ulong E_IMAPI_DF2DATA_CLIENT_NAME_IS_NOT_VALID = 0xC0AA0408;
        /// <summary>
        ///There is currently a write operation in progress.
        /// </summary>
        public const ulong E_IMAPI_DF2TAO_WRITE_IN_PROGRESS = 0xC0AA0500;
        /// <summary>
        ///There is no write operation currently in progress.
        /// </summary>
        public const ulong E_IMAPI_DF2TAO_WRITE_NOT_IN_PROGRESS = 0xC0AA0501;
        /// <summary>
        ///The requested operation is only valid when media has been "prepared".
        /// </summary>
        public const ulong E_IMAPI_DF2TAO_MEDIA_IS_NOT_PREPARED = 0xC0AA0502;
        /// <summary>
        ///The requested operation is not valid when media has been "prepared" but not released.
        /// </summary>
        public const ulong E_IMAPI_DF2TAO_MEDIA_IS_PREPARED = 0xC0AA0503;
        /// <summary>
        ///The property cannot be changed once the media has been written to.
        /// </summary>
        public const ulong E_IMAPI_DF2TAO_PROPERTY_FOR_BLANK_MEDIA_ONLY = 0xC0AA0504;
        /// <summary>
        ///The table of contents cannot be retrieved from an empty disc.
        /// </summary>
        public const ulong E_IMAPI_DF2TAO_TABLE_OF_CONTENTS_EMPTY_DISC = 0xC0AA0505;
        /// <summary>
        ///Only blank CD-R/RW media is supported.
        /// </summary>
        public const ulong E_IMAPI_DF2TAO_MEDIA_IS_NOT_BLANK = 0xC0AA0506;
        /// <summary>
        ///Only blank CD-R/RW media is supported.
        /// </summary>
        public const ulong E_IMAPI_DF2TAO_MEDIA_IS_NOT_SUPPORTED = 0xC0AA0507;
        /// <summary>
        ///CD-R and CD-RW media support a maximum of 99 audio tracks.
        /// </summary>
        public const ulong E_IMAPI_DF2TAO_TRACK_LIMIT_REACHED = 0xC0AA0508;
        /// <summary>
        ///There is not enough space left on the media to add the provided audio track.
        /// </summary>
        public const ulong E_IMAPI_DF2TAO_NOT_ENOUGH_SPACE = 0xC0AA0509;
        /// <summary>
        ///You cannot prepare the media until you choose a recorder to use.
        /// </summary>
        public const ulong E_IMAPI_DF2TAO_NO_RECORDER_SPECIFIED = 0xC0AA050A;
        /// <summary>
        ///The ISRC provided is not valid.
        /// </summary>
        public const ulong E_IMAPI_DF2TAO_INVALID_ISRC = 0xC0AA050B;
        /// <summary>
        ///The Media Catalog Number provided is not valid.
        /// </summary>
        public const ulong E_IMAPI_DF2TAO_INVALID_MCN = 0xC0AA050C;
        /// <summary>
        ///The provided audio stream is not valid.
        /// </summary>
        public const ulong E_IMAPI_DF2TAO_STREAM_NOT_SUPPORTED = 0xC0AA050D;
        /// <summary>
        ///This device does not support the operations required by this disc format.
        /// </summary>
        public const ulong E_IMAPI_DF2TAO_RECORDER_NOT_SUPPORTED = 0xC0AA050E;
        /// <summary>
        ///The client name is not valid.
        /// </summary>
        public const ulong E_IMAPI_DF2TAO_CLIENT_NAME_IS_NOT_VALID = 0xC0AA050F;
        /// <summary>
        ///There is currently a write operation in progress.
        /// </summary>
        public const ulong E_IMAPI_DF2RAW_WRITE_IN_PROGRESS = 0xC0AA0600;
        /// <summary>
        ///There is no write operation currently in progress.
        /// </summary>
        public const ulong E_IMAPI_DF2RAW_WRITE_NOT_IN_PROGRESS = 0xC0AA0601;
        /// <summary>
        ///The requested operation is only valid when media has been "prepared".
        /// </summary>
        public const ulong E_IMAPI_DF2RAW_MEDIA_IS_NOT_PREPARED = 0xC0AA0602;
        /// <summary>
        ///The requested operation is not valid when media has been "prepared" but not released.
        /// </summary>
        public const ulong E_IMAPI_DF2RAW_MEDIA_IS_PREPARED = 0xC0AA0603;
        /// <summary>
        ///The client name is not valid.
        /// </summary>
        public const ulong E_IMAPI_DF2RAW_CLIENT_NAME_IS_NOT_VALID = 0xC0AA0604;
        /// <summary>
        ///Only blank CD-R/RW media is supported.
        /// </summary>
        public const ulong E_IMAPI_DF2RAW_MEDIA_IS_NOT_BLANK = 0xC0AA0606;
        /// <summary>
        ///Only blank CD-R/RW media is supported.
        /// </summary>
        public const ulong E_IMAPI_DF2RAW_MEDIA_IS_NOT_SUPPORTED = 0xC0AA0607;
        /// <summary>
        ///There is not enough space on the media to add the provided session.
        /// </summary>
        public const ulong E_IMAPI_DF2RAW_NOT_ENOUGH_SPACE = 0xC0AA0609;
        /// <summary>
        ///You cannot prepare the media until you choose a recorder to use.
        /// </summary>
        public const ulong E_IMAPI_DF2RAW_NO_RECORDER_SPECIFIED = 0xC0AA060A;
        /// <summary>
        ///The provided audio stream is not valid.
        /// </summary>
        public const ulong E_IMAPI_DF2RAW_STREAM_NOT_SUPPORTED = 0xC0AA060D;
        /// <summary>
        ///The requested data block type is not supported by the current device.
        /// </summary>
        public const ulong E_IMAPI_DF2RAW_DATA_BLOCK_TYPE_NOT_SUPPORTED = 0xC0AA060E;
        /// <summary>
        ///The stream does not contain a sufficient number of sectors in the leadin for the current media.
        /// </summary>
        public const ulong E_IMAPI_DF2RAW_STREAM_LEADIN_TOO_SHORT = 0xC0AA060F;
        /// <summary>
        ///This device does not support the operations required by this disc format.
        /// </summary>
        public const ulong E_IMAPI_DF2RAW_RECORDER_NOT_SUPPORTED = 0xC0AA0610;
        /// <summary>
        ///The format is currently using the disc recorder for an erase operation. Please wait for the erase to complete before attempting to set or clear the current disc recorder.
        /// </summary>
        public const ulong E_IMAPI_ERASE_RECORDER_IN_USE = 0x80AA0900;
        /// <summary>
        ///The erase format only supports one recorder. You must clear the current recorder before setting a new one.
        /// </summary>
        public const ulong E_IMAPI_ERASE_ONLY_ONE_RECORDER_SUPPORTED = 0x80AA0901;
        /// <summary>
        ///The drive did not report sufficient data for a READ DISC INFORMATION command. The drive may not be supported, or the media may not be correct.
        /// </summary>
        public const ulong E_IMAPI_ERASE_DISC_INFORMATION_TOO_SMALL = 0x80AA0902;
        /// <summary>
        ///The drive did not report sufficient data for a MODE SENSE (page 0x2A) command. The drive may not be supported, or the media may not be correct.
        /// </summary>
        public const ulong E_IMAPI_ERASE_MODE_PAGE_2A_TOO_SMALL = 0x80AA0903;
        /// <summary>
        ///The drive reported that the media is not erasable.
        /// </summary>
        public const ulong E_IMAPI_ERASE_MEDIA_IS_NOT_ERASABLE = 0x80AA0904;
        /// <summary>
        ///The drive failed the erase command.
        /// </summary>
        public const ulong E_IMAPI_ERASE_DRIVE_FAILED_ERASE_COMMAND = 0x80AA0905;
        /// <summary>
        /// The drive did not complete the erase in one hour. The drive may require a power cycle, media removal, or other manual intervention to resume proper operation.
        /// </summary>
        public const ulong E_IMAPI_ERASE_TOOK_ulongER_THAN_ONE_HOUR = 0x80AA0906;
        /// <summary>
        /// The drive returned an unexpected error during the erase. The media may be unusable, the erase may be complete, or the drive may still be in the process of erasing the disc.
        /// Note  Currently, this value will also be returned if an attempt to perform an erase on CD-RW or DVD-RW media via the IDiscFormat2Erase interface fails as a result of the media being bad.
        /// </summary>
        public const ulong E_IMAPI_ERASE_UNEXPECTED_DRIVE_RESPONSE_DURING_ERASE = 0x80AA0907;
        /// <summary>
        ///The drive returned an error for a START UNIT (spinup) command. Manual intervention may be required.
        /// </summary>
        public const ulong E_IMAPI_ERASE_DRIVE_FAILED_SPINUP_COMMAND = 0x80AA0908;
        /// <summary>
        ///The current media type is unsupported.
        /// </summary>
        public const ulong E_IMAPI_ERASE_MEDIA_IS_NOT_SUPPORTED = 0xC0AA0909;
        /// <summary>
        ///This device does not support the operations required by this disc format.
        /// </summary>
        public const ulong E_IMAPI_ERASE_RECORDER_NOT_SUPPORTED = 0xC0AA090A;
        /// <summary>
        ///The client name is not valid.
        /// </summary>
        public const ulong E_IMAPI_ERASE_CLIENT_NAME_IS_NOT_VALID = 0xC0AA090B;


        #endregion

        #region Constants in Imapi2fserror.h.

        /// <summary>
        ///Internal error occurred: %1!ls!.
        /// </summary>
        public const ulong IMAPI_E_FSI_INTERNAL_ERROR = 0xC0AAB100;
        /// <summary>
        ///The value specified for parameter '%1!ls!' is not valid.
        /// </summary>
        public const ulong IMAPI_E_INVALID_PARAM = 0xC0AAB101;
        /// <summary>
        ///FileSystemImage object is in read only mode.
        /// </summary>
        public const ulong IMAPI_E_READONLY = 0xC0AAB102;
        /// <summary>
        ///No output file system specified.
        /// </summary>
        public const ulong IMAPI_E_NO_OUTPUT = 0xC0AAB103;
        /// <summary>
        ///The specified Volume Identifier is either too ulong or contains one or more invalid characters.
        /// </summary>
        public const ulong IMAPI_E_INVALID_VOLUME_NAME = 0xC0AAB104;
        /// <summary>
        ///Invalid file dates. %1!ls! time is earlier than %2!ls! time.
        /// </summary>
        public const ulong IMAPI_E_INVALID_DATE = 0xC0AAB105;
        /// <summary>
        ///The file system must be empty for this function.
        /// </summary>
        public const ulong IMAPI_E_FILE_SYSTEM_NOT_EMPTY = 0xC0AAB106;
        /// <summary>
        ///You cannot change the file system specified for creation, because the file system from the imported session and the file system in the current session do not match.
        /// </summary>
        public const ulong IMAPI_E_FILE_SYSTEM_CHANGE_NOT_ALLOWED = 0xC0AAB163L;
        /// <summary>
        ///Specified path '%1!ls!' does not identify a file.
        /// </summary>
        public const ulong IMAPI_E_NOT_FILE = 0xC0AAB108;
        /// <summary>
        ///Specified path '%1!ls!' does not identify a directory.
        /// </summary>
        public const ulong IMAPI_E_NOT_DIR = 0xC0AAB109;
        /// <summary>
        ///The directory '%1!s!' is not empty.
        /// </summary>
        public const ulong IMAPI_E_DIR_NOT_EMPTY = 0xC0AAB10A;
        /// <summary>
        ///ls!' is not part of the file system. It must be added to complete this operation.
        /// </summary>
        public const ulong IMAPI_E_NOT_IN_FILE_SYSTEM = 0xC0AAB10B;
        /// <summary>
        ///Path '%1!s!' is badly formed or contains invalid characters.
        /// </summary>
        public const ulong IMAPI_E_INVALID_PATH = 0xC0AAB110;
        /// <summary>
        ///The name '%1!ls!' specified is not legal: Name of file or directory object created while the UseRestrictedCharacterSet property is set may only contain ANSI characters.
        /// </summary>
        public const ulong IMAPI_E_RESTRICTED_NAME_VIOLATION = 0xC0AAB111;
        /// <summary>
        ///ls!' name already exists.
        /// </summary>
        public const ulong IMAPI_E_DUP_NAME = 0xC0AAB112;
        /// <summary>
        ///Attempt to add '%1!ls!' failed: cannot create a file-system-specific unique name for the %2!ls! file system.
        /// </summary>
        public const ulong IMAPI_E_NO_UNIQUE_NAME = 0xC0AAB113;
        /// <summary>
        ///Cannot find item '%1!ls!' in FileSystemImage hierarchy.
        /// </summary>
        public const ulong IMAPI_E_ITEM_NOT_FOUND = 0xC0AAB118;
        /// <summary>
        ///The file '%1!s!' not found in FileSystemImage hierarchy.
        /// </summary>
        public const ulong IMAPI_E_FILE_NOT_FOUND = 0xC0AAB119;
        /// <summary>
        ///The directory '%1!s!' not found in FileSystemImage hierarchy.
        /// </summary>
        public const ulong IMAPI_E_DIR_NOT_FOUND = 0xC0AAB11A;
        /// <summary>
        ///Adding '%1!ls!' would result in a result image having a size larger than the current configured limit.
        /// </summary>
        public const ulong IMAPI_E_IMAGE_SIZE_LIMIT = 0xC0AAB120;
        /// <summary>
        ///Value specified for FreeMediaBlocks property is too small for estimated image size based on current data.
        /// </summary>
        public const ulong IMAPI_E_IMAGE_TOO_BIG = 0xC0AAB121;
        /// <summary>
        ///The image is not aligned on a 2kb sector boundary.
        /// </summary>
        public const ulong IMAPI_E_IMAGEMANAGER_IMAGE_NOT_ALIGNED = 0xC0AAB200L;
        /// <summary>
        ///The image has not been set using the IIsoImageManager::SetPath or IIsoImageManager::SetStream methods prior to calling the IIsoImageManager::Validate method.
        /// </summary>
        public const ulong IMAPI_E_IMAGEMANAGER_NO_IMAGE = 0xC0AAB202L;
        /// <summary>
        ///The provided image is too large to be validated as the size exceeds MAXulong.
        /// </summary>
        public const ulong IMAPI_E_IMAGEMANAGER_IMAGE_TOO_BIG = 0xC0AAB203L;
        /// <summary>
        ///Data stream supplied for file '%1!ls!' is inconsistent: expected %2!I64d! bytes, found %3!I64d!.
        /// </summary>
        public const ulong IMAPI_E_DATA_STREAM_INCONSISTENCY = 0xC0AAB128;
        /// <summary>
        ///Cannot read data from stream supplied for file '%1!ls!'.
        /// </summary>
        public const ulong IMAPI_E_DATA_STREAM_READ_FAILURE = 0xC0AAB129;
        /// <summary>
        ///The following error was encountered when trying to create data stream for file '%1!ls!':
        /// </summary>
        public const ulong IMAPI_E_DATA_STREAM_CREATE_FAILURE = 0xC0AAB12A;
        /// <summary>
        ///Failure enumerating files in the directory tree is inaccessible due to permissions.
        /// </summary>
        public const ulong IMAPI_E_DIRECTORY_READ_FAILURE = 0xC0AAB12BL;
        /// <summary>
        ///This file system image has too many directories for the %1!ls! file system.
        /// </summary>
        public const ulong IMAPI_E_TOO_MANY_DIRS = 0xC0AAB130;
        /// <summary>
        ///ISO9660 is limited to 8 levels of directories.
        /// </summary>
        public const ulong IMAPI_E_ISO9660_LEVELS = 0xC0AAB131;
        /// <summary>
        ///Data file is too large for '%1!ls!' file system.
        /// </summary>
        public const ulong IMAPI_E_DATA_TOO_BIG = 0xC0AAB132;
        /// <summary>
        ///Cannot initialize %1!ls! stash file.
        /// </summary>
        public const ulong IMAPI_E_STASHFILE_OPEN_FAILURE = 0xC0AAB138;
        /// <summary>
        ///Error seeking in '%1!ls!' stash file.
        /// </summary>
        public const ulong IMAPI_E_STASHFILE_SEEK_FAILURE = 0xC0AAB139;
        /// <summary>
        ///Error encountered writing to '%1!ls!' stash file.
        /// </summary>
        public const ulong IMAPI_E_STASHFILE_WRITE_FAILURE = 0xC0AAB13A;
        /// <summary>
        ///Error encountered reading from '%1!ls!' stash file.
        /// </summary>
        public const ulong IMAPI_E_STASHFILE_READ_FAILURE = 0xC0AAB13B;
        /// <summary>
        ///The working directory '%1!ls!' is not valid.
        /// </summary>
        public const ulong IMAPI_E_INVALID_WORKING_DIRECTORY = 0xC0AAB140;
        /// <summary>
        ///Cannot set working directory to '%1!ls!'. Space available is %2!I64d! bytes, approximately %3!I64d! bytes required.
        /// </summary>
        public const ulong IMAPI_E_WORKING_DIRECTORY_SPACE = 0xC0AAB141;
        /// <summary>
        ///Attempt to move the data stash file to directory '%1!ls!' was not successful.
        /// </summary>
        public const ulong IMAPI_E_STASHFILE_MOVE = 0xC0AAB142;
        /// <summary>
        ///The boot object could not be added to the image.
        /// </summary>
        public const ulong IMAPI_E_BOOT_IMAGE_DATA = 0xC0AAB148;
        /// <summary>
        ///A boot object can only be included in an initial disc image.
        /// </summary>
        public const ulong IMAPI_E_BOOT_OBJECT_CONFLICT = 0xC0AAB149;
        /// <summary>
        ///The emulation type requested does not match the boot image size.
        /// </summary>
        public const ulong IMAPI_E_BOOT_EMULATION_IMAGE_SIZE_MISMATCH = 0xC0AAB14A;
        /// <summary>
        ///Optical media is empty.
        /// </summary>
        public const ulong IMAPI_E_EMPTY_DISC = 0xC0AAB150;
        /// <summary>
        ///The specified disc does not contain one of the supported file systems.
        /// </summary>
        public const ulong IMAPI_E_NO_SUPPORTED_FILE_SYSTEM = 0xC0AAB151;
        /// <summary>
        ///The specified disc does not contain a '%1!ls!' file system.
        /// </summary>
        public const ulong IMAPI_E_FILE_SYSTEM_NOT_FOUND = 0xC0AAB152;
        /// <summary>
        ///Consistency error encountered while importing the '%1!ls!' file system.
        /// </summary>
        public const ulong IMAPI_E_FILE_SYSTEM_READ_CONSISTENCY_ERROR = 0xC0AAB153;
        /// <summary>
        ///The '%1!ls!'file system on the selected disc contains a feature not supported for import: %2!ls!.
        /// </summary>
        public const ulong IMAPI_E_FILE_SYSTEM_FEATURE_NOT_SUPPORTED = 0xC0AAB154;
        /// <summary>
        ///Could not import %2!ls! file system from disc. The file '%1!ls!' already exists within the image hierarchy as a directory.
        /// </summary>
        public const ulong IMAPI_E_IMPORT_TYPE_COLLISION_FILE_EXISTS_AS_DIRECTORY = 0xC0AAB155;
        /// <summary>
        ///Cannot seek to block %1!I64d! on source disc.
        /// </summary>
        public const ulong IMAPI_E_IMPORT_SEEK_FAILURE = 0xC0AAB156;
        /// <summary>
        ///Import from previous session failed due to an error reading a block on the media (most likely block %1!u!).
        /// </summary>
        public const ulong IMAPI_E_IMPORT_READ_FAILURE = 0xC0AAB157;
        /// <summary>
        ///Current disc is not the same one from which file system was imported.
        /// </summary>
        public const ulong IMAPI_E_DISC_MISMATCH = 0xC0AAB158;
        /// <summary>
        ///IMAPI does not allow multi-session with the current media type.
        /// </summary>
        public const ulong IMAPI_E_IMPORT_MEDIA_NOT_ALLOWED = 0xC0AAB159;
        /// <summary>
        ///IMAPI cannot do multi-session with the current media because it does not support a compatible UDF revision for write.
        /// </summary>
        public const ulong IMAPI_E_UDF_NOT_WRITE_COMPATIBLE = 0xC0AAB15A;
        /// <summary>
        ///IMAPI does not support the multisession type requested.
        /// </summary>
        public const ulong IMAPI_E_INCOMPATIBLE_MULTISESSION_TYPE = 0xC0AAB15B;
        /// <summary>
        ///Operation failed due to an incompatible layout of the previous session imported from the medium.
        /// </summary>
        public const ulong IMAPI_E_INCOMPATIBLE_PREVIOUS_SESSION = 0xC0AAB133;
        /// <summary>
        ///IMAPI supports none of the multisession type(s) provided on the current media.
        /// </summary>
        public const ulong IMAPI_E_NO_COMPATIBLE_MULTISESSION_TYPE = 0xC0AAB15C;
        /// <summary>
        ///MultisessionInterfaces property must be set prior calling this method.
        /// </summary>
        public const ulong IMAPI_E_MULTISESSION_NOT_SET = 0xC0AAB15D;
        /// <summary>
        ///Could not import %2!ls! file system from disc. The directory '%1!ls!' already exists within the image hierarchy as a file.
        /// </summary>
        public const ulong IMAPI_E_IMPORT_TYPE_COLLISION_DIRECTORY_EXISTS_AS_FILE = 0xC0AAB15E;
        /// <summary>
        ///One of multisession parameters cannot be retrieved or has a wrong value.
        /// </summary>
        public const ulong IMAPI_E_BAD_MULTISESSION_PARAMETER = 0xC0AAB162;
        /// <summary>
        ///This feature is not supported for the current file system revision. The image will be created without this feature.
        /// </summary>
        public const ulong IMAPI_S_IMAGE_FEATURE_NOT_SUPPORTED = 0x00AAB15FL;

        #endregion

        #region EraseMedia
        /// <summary>
        /// Pointer is not valid.
        /// </summary>
        public const ulong E_POINTER = 0x80004003;

        /// <summary>
        /// Unspecified failure.
        /// </summary>
        public const ulong E_FAIL = 0x80004005;

        /// <summary>
        /// Failed to allocate the required memory.
        /// </summary>
        public const ulong E_OUTOFMEMORY = 0x8007000E;

        /// <summary>
        /// The drive did not complete the erase in one hour. The drive may require a power cycle, media removal, or other manual intervention to resume proper operation.
        ///Note  Currently, this value will also be returned if an attempt to perform an erase on CD-RW or DVD-RW media via the IDiscFormat2Erase interface fails as a result of the media being bad.
        /// </summary>
        public const ulong E_IMAPI_ERASE_TOOK_LONGER_THAN_ONE_HOUR = 0x80AA0906;

        #endregion

        #region RECORDER

        /// <summary>
        /// The device is currently being used by another application.
        /// </summary>
        public const ulong E_ACCESS_DENIED = 0x80070005;

        /// <summary>
        /// One or more arguments are not valid.
        /// </summary>
        public const ulong E_INVALIDARG = 0x80070057;

        /// <summary>
        /// The specified handle is invalid.
        /// </summary>
        public const ulong HRESULT_FROM_WIN32_ERROR_INVALID_HANDLE = 6;

        /// <summary>
        /// The specified network resource or device is no longer available.
        /// </summary>
        public const ulong HRESULT_FROM_WIN32_ERROR_DEV_NOT_EXIST = 55;

        #endregion


    }
}
