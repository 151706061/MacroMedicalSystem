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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Xml;
using System.Xml.Serialization;
using Macro.Common;
using Macro.Common.Media;
using Macro.Common.Media.IMAPI2;
using Macro.Desktop;
using Macro.Desktop.Trees;
using Macro.Dicom;
using Macro.Dicom.Utilities.Xml;
using Macro.ImageViewer.Utilities.Media.PortableViewer;
using Path = System.IO.Path;


namespace Macro.ImageViewer.Utilities.Media
{
    [ExtensionPoint()]
    public sealed class MediaWriteComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    { }

    [AssociateView(typeof(MediaWriteComponentViewExtensionPoint))]
    public class MediaWriterComponent : ApplicationComponent, IMediaWriterComponent
    {
        #region 属性

        private bool _isWriting = false;
        private ITree _tree = null;
        private string _volumeName = "Gold";
        private IDiscRecorder2 _selectedMediaWriter = null;
        private long _diskSize = 0;
        private long _diskUseAbleSize = 0;
        private BackgroundWorker backgroundBurnWorker = null;
        private BurnStatus _burnData = new BurnStatus();
        private string _labelStatusText = null;
        private int _writeStagePercent = 0;
        private long _requiredMediaSpace = 0;
        private int _currentMediaSpacePercent = 0;
        private string stageToTempFolder = null;
        private BackgroundWorker LoadImageWorker = null;
        MediaWriterOptionsComponent optionComponent = null;
        private bool _isEreasing = false;
        private string _mediaType = string.Empty;
        #endregion

        public MediaWriterComponent()
        {
            stageToTempFolder = System.Environment.CurrentDirectory;
            LoadImageWorker = new BackgroundWorker();
            LoadImageWorker.DoWork += selectFilesSize;
            LoadImageWorker.ProgressChanged += LoadImageWorker_ProgressChanged;
            LoadImageWorker.WorkerReportsProgress = true;
            LoadImageWorker.WorkerSupportsCancellation = true;

            if (backgroundBurnWorker == null)
            {
                backgroundBurnWorker = new BackgroundWorker();
                this.backgroundBurnWorker.WorkerReportsProgress = true;
                this.backgroundBurnWorker.WorkerSupportsCancellation = true;
                this.backgroundBurnWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundBurnWorker_DoWork);
                this.backgroundBurnWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundBurnWorker_RunWorkerCompleted);
                this.backgroundBurnWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundBurnWorker_ProgressChanged);

            }

        }

        public void ClearStudies()
        {
            if (_tree != null)
            {
                Tree.Items.Clear();
                Tree = null;
                _requiredMediaSpace = 0;
                _currentMediaSpacePercent = 0;
                NotifyPropertyChanged("RequiredMediaSpace");
                NotifyPropertyChanged("CurrentMediaSpacePercent");
            }
        }

        public void EjectMedia()
        {
            if (_selectedMediaWriter == null)
            {
                this.Host.ShowMessageBox("请选择光驱", MessageBoxActions.Ok);
                return;
            }
            try
            {
                SelectedMediaWriter.EjectMedia();
            }
            catch (COMException e)
            {
                this.Host.ShowMessageBox(ImapiReturnValues.GetName(e.ErrorCode), MessageBoxActions.Ok);
            }
        }

        public void OpenOptions()
        {
            if (optionComponent == null)
            {
                optionComponent = new MediaWriterOptionsComponent();
                optionComponent.PropertyChanged += propertyChanged;
            }

            ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, optionComponent, "Media Write Options");

        }

        public void WriteMedia()
        {

            if (!CanWrite)
            {
                ExceptionHandler.Report(new Exception(SR.NoSupportRecord), this.Host.DesktopWindow);
                return;
            }

            if (Tree == null || Tree.Items.Count == 0)
            {
                ExceptionHandler.Report(new Exception(SR.NoDataWillBurned), this.Host.DesktopWindow);
                return;
            }

            GetDiskSize();

            if (_diskUseAbleSize == 0 || _diskUseAbleSize == -1)
            {
                ExceptionHandler.Report(new Exception(SR.BadDisc), this.Host.DesktopWindow);
                return;
            }

            if (_requiredMediaSpace >= _diskUseAbleSize)
            {
                ExceptionHandler.Report(new Exception(SR.BadDisc), this.Host.DesktopWindow);
                return;
            }

            if (!Directory.Exists(StagingFolderPath))
            {
                ExceptionHandler.Report(new Exception(SR.BadDisc), this.Host.DesktopWindow);
                return;
            }

            if (backgroundBurnWorker.IsBusy)
            {
                ExceptionHandler.Report(new Exception(SR.StageWriting), this.Host.DesktopWindow);
            }
            else
            {
                backgroundBurnWorker.RunWorkerAsync();
            }
        }

        public void EreaseDisc()
        {
            if (backgroundBurnWorker.IsBusy)
            {
                if (IsWriting || _isEreasing)
                {
                    ExceptionHandler.Report(new Exception("正在刻录或者擦除中"), this.Host.DesktopWindow);
                    return;
                }

                backgroundBurnWorker.CancelAsync();
            }

            _isEreasing = true;
            backgroundBurnWorker.RunWorkerAsync();

        }

        public void Cancel()
        {
            if (this.CanCancel && backgroundBurnWorker != null && backgroundBurnWorker.IsBusy)
            {
                backgroundBurnWorker.CancelAsync();
                IsWriting = false;
                CurrentWriteStageName = SR.StageCancelled;
            }
        }

        public bool CanCancel
        {
            get { return !IsWriting; }
        }

        public bool CanWrite
        {
            get
            {
                MsftDiscFormat2Data format2Data = null;
                try
                {
                    if (SelectedMediaWriter == null)
                    {
                        this.Host.ShowMessageBox("没有光驱", MessageBoxActions.Ok);
                        return false;
                    }

                    format2Data = new MsftDiscFormat2Data();

                    if (format2Data.IsRecorderSupported(SelectedMediaWriter))
                    {
                        return true;
                    }
                }
                catch (COMException e)
                {
                    this.Host.ShowMessageBox(ImapiReturnValues.GetName(e.ErrorCode), MessageBoxActions.Ok);
                }
                finally
                {

                    if (format2Data != null)
                    {
                        Marshal.ReleaseComObject(format2Data);
                    }
                }

                return false;
            }
        }

        public void DetectMedia()
        {
            GetDiskSize();
            NotifyPropertyChanged("DiscSpaceWarning");
            NotifyPropertyChanged("CurrentMediaSpacePercent");
        }

        public bool IsWriting
        {
            get { return _isWriting; }
            set { _isWriting = value; }
        }

        public string CurrentMediaDescription
        {
            get
            {
                string size = string.Empty;
                if (_diskSize == -1 || _diskUseAbleSize == -1)
                {
                    return "没有光盘";
                }
                size = string.Format("光盘类型:{0} 总容量:{1},空间容量:{2}", _mediaType, UpdateCaption(_diskSize), UpdateCaption(_diskUseAbleSize));
                return size;
            }
            set
            {
                _mediaType = value;
                NotifyPropertyChanged(CurrentMediaDescription);
            }
        }

        public int CurrentMediaSpacePercent
        {
            get
            {
                if (_requiredMediaSpace != 0 && _diskUseAbleSize > 0)
                {
                    _currentMediaSpacePercent = (int)((_requiredMediaSpace / (double)_diskUseAbleSize) * 100);
                }
                else
                {
                    _currentMediaSpacePercent = 0;
                }

                return _currentMediaSpacePercent;
            }
        }

        public string CurrentWriteStageName
        {
            get
            {
                return _labelStatusText;
            }
            set
            {
                _labelStatusText = value;
                NotifyPropertyChanged("CurrentWriteStageName");
            }
        }

        public int CurrentWriteStagePercent
        {
            get
            {
                return _writeStagePercent;
            }
            set
            {
                _writeStagePercent = value;
                NotifyPropertyChanged("CurrentWriteStagePercent");
            }
        }

        public string RequiredMediaSpace
        {
            get { return string.Format("Request:{0}", UpdateCaption(_requiredMediaSpace)); }
            set
            {
                if (_requiredMediaSpace != long.Parse(value))
                {
                    _requiredMediaSpace = long.Parse(value);
                    NotifyPropertyChanged("RequiredMediaSpace");
                }
            }
        }

        public bool EjectOnCompleted
        {
            get { return MediaWriterSettings.Default.EjectMediaOnCompleted; }
        }

        public IList<IDiscRecorder2> MediaWriters
        {
            get
            {

                MsftDiscMaster2 discMaster2 = null;
                try
                {
                    discMaster2 = new MsftDiscMaster2();
                    if (!discMaster2.IsSupportedEnvironment)
                    {
                        this.Host.ShowMessageBox("没有安装IMAPI", MessageBoxActions.Ok);
                        return null;
                    }

                    List<IDiscRecorder2> listRecorders = new List<IDiscRecorder2>();
                    foreach (string item in discMaster2)
                    {
                        IDiscRecorder2 msRecorder = new MsftDiscRecorder2();
                        msRecorder.InitializeDiscRecorder(item);
                        //string strPath = msRecorder.VolumePathNames.GetValue(0).ToString();
                        //string path = string.Format("{0} [{1}]", strPath, msRecorder.ProductId);
                        listRecorders.Add(msRecorder);

                    }

                    return listRecorders;
                }
                catch (COMException e)
                {
                    this.Host.ShowMessageBox("获得光驱列表出错," + ImapiReturnValues.GetName(e.ErrorCode), MessageBoxActions.Ok);

                }
                finally
                {
                    if (discMaster2 != null)
                    {
                        Marshal.ReleaseComObject(discMaster2);
                    }
                }

                return null;
            }
        }

        public string NumberOfStudies
        {
            get
            {

                int count = 0;
                if (Tree != null)
                {
                    foreach (var item in Tree.Items)
                    {
                        if (((IStudyTreeItem)item).Ischecked)
                        {
                            count++;
                        }
                    }
                }
                return string.Format("{0} studies", count);
            }
        }

        public IDiscRecorder2 SelectedMediaWriter
        {
            get
            {
                return _selectedMediaWriter;
            }
            set
            {
                if (_selectedMediaWriter != value)
                {
                    _selectedMediaWriter = value;

                }
            }
        }

        public string StagingFolderPath
        {
            get
            {

                if (MediaWriterSettings.Default.StageToTempFolder)
                {
                    return stageToTempFolder;
                }
                return MediaWriterSettings.Default.UserStagingFolder;
            }
        }

        public ITree Tree
        {
            get
            {
                return _tree;
            }
            set
            {
                if (_tree != value)
                {
                    _tree = value;
                    if (LoadImageWorker.IsBusy)
                    {
                        LoadImageWorker.CancelAsync();
                    }
                    LoadImageWorker.RunWorkerAsync();
                    NotifyPropertyChanged("Tree");
                    NotifyPropertyChanged("NumberOfStudies");
                }

            }
        }

        public string VolumeName
        {
            get
            {
                return _volumeName;
            }
            set
            {
                _volumeName = value;
            }
        }

        private void propertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            MediaWriterSettings.Default.Reload();
            NotifyPropertyChanged("StagingFolderPath");
        }

        private string UpdateCaption(long totalDiscSize)
        {
            return totalDiscSize < 1000000000 ?
                string.Format("{0}MB", totalDiscSize / 1000000) :
                string.Format("{0:F2}GB", (float)totalDiscSize / 1000000000.0);

        }

        /// <summary>
        /// 后台工作线程
        /// </summary>       
        private void backgroundBurnWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (SelectedMediaWriter == null)
            {
                return;
            }

            if (_isEreasing)
            {
                DoEreaseDisc();
            }
            else
            {
                Writing();
            }


        }
        /// <summary>
        /// 进度条更新
        /// </summary>
        private void backgroundBurnWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //int percent = e.ProgressPercentage;
            var burnData = (BurnStatus)e.UserState;

            if (burnData.Task == BURN_MEDIA_TASK.BURN_MEDIA_TASK_FILE_SYSTEM)
            {
                CurrentWriteStageName = burnData.StatusMessage;
            }
            else if (burnData.Task == BURN_MEDIA_TASK.BURN_MEDIA_TASK_WRITING)
            {
                switch (burnData.CurrentAction)
                {
                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_VALIDATING_MEDIA:
                        CurrentWriteStageName = "Validating current media...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_FORMATTING_MEDIA:
                        CurrentWriteStageName = "Formatting media...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_INITIALIZING_HARDWARE:
                        CurrentWriteStageName = "Initializing hardware...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_CALIBRATING_POWER:
                        CurrentWriteStageName = "Optimizing laser intensity...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_WRITING_DATA:
                        long writtenSectors = burnData.LastWrittenLba - burnData.StartLba;

                        if (writtenSectors > 0 && burnData.SectorCount > 0)
                        {
                            var percent = (int)((100 * writtenSectors) / burnData.SectorCount);
                            CurrentWriteStageName = string.Format("Progress: {0}%", percent);
                            CurrentWriteStagePercent = percent;
                        }
                        else
                        {
                            CurrentWriteStageName = "Progress 0%";
                            CurrentWriteStagePercent = 0;
                        }
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_FINALIZATION:
                        CurrentWriteStageName = "Finalizing writing...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_COMPLETED:
                        CurrentWriteStageName = "Completed!";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_VERIFYING:
                        CurrentWriteStageName = "Verifying";
                        break;
                }
            }
            else if (burnData.Task == BURN_MEDIA_TASK.BURN_MEDIA_TASK_Erease)
            {
                CurrentWriteStageName = "Erease...!";
                CurrentWriteStagePercent = e.ProgressPercentage;
            }

        }
        /// <summary>
        /// 后天线程完成
        /// </summary>
        private void backgroundBurnWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CurrentWriteStageName = "Completed";
        }
        /// <summary>
        /// 获得光碟容量
        /// </summary>
        private void GetDiskSize()
        {
            IDiscFormat2Data format2Data = null;
            try
            {

                format2Data = new MsftDiscFormat2Data();

                if (format2Data.IsRecorderSupported(SelectedMediaWriter) == false)
                {
                    this.Host.ShowMessageBox("没有光碟", MessageBoxActions.Ok);
                    return;
                }

                if (!format2Data.IsCurrentMediaSupported(SelectedMediaWriter))
                {
                    this.Host.ShowMessageBox("没有光碟", MessageBoxActions.Ok);
                    _diskSize = -1;
                    _diskUseAbleSize = -1;
                    return;
                }
                format2Data.Recorder = SelectedMediaWriter;
                IMAPI_MEDIA_PHYSICAL_TYPE mediaType = format2Data.CurrentPhysicalMediaType;
                CurrentMediaDescription = EnumeratorMediaPhysicalLType.GetMediaTypeString(mediaType);
                Int64 totalSectors = format2Data.TotalSectorsOnMedia;
                Int64 freeSectors = format2Data.FreeSectorsOnMedia;
                _diskSize = totalSectors * 2048;
                _diskUseAbleSize = freeSectors * 2048;
            }
            catch (COMException e)
            {
                this.Host.ShowMessageBox(ImapiReturnValues.GetName(e.ErrorCode), MessageBoxActions.Ok);
                _diskSize = 0;
                _diskUseAbleSize = 0;
            }
            finally
            {
                if (format2Data != null)
                {
                    Marshal.ReleaseComObject(format2Data);
                }
            }
        }

        #region 加载文件
        /// <summary>
        /// 选择文件的大小
        /// </summary>
        private void selectFilesSize(object sender, DoWorkEventArgs e)
        {
            long size = 0;

            if (Tree != null)
            {
                foreach (IStudyTreeItem item in Tree.Items)
                {
                    StudyTreeItem treeitem = (StudyTreeItem)item;
                    if (!treeitem.Ischecked)
                    {
                        continue;
                    }

                    foreach (ISeriesTreeItem seriss in treeitem.Tree.Items)
                    {
                        SeriesTreeItem seriesitem = (SeriesTreeItem)seriss;
                        if (!seriesitem.Ischecked)
                        {
                            continue;
                        }

                        foreach (var sop in seriesitem.Series.GetSopInstances())
                        {
                            if (LoadImageWorker.CancellationPending == true)
                            {
                                break;
                            }

                            if (File.Exists(sop.FilePath))
                            {
                                FileInfo info = new FileInfo(sop.FilePath);
                                size += info.Length;
                            }

                            LoadImageWorker.ReportProgress(0, size);
                        }
                    }
                }

                if (MediaWriterSettings.Default.PortablePath!=null)
                {

                    GetFilesSize(MediaWriterSettings.Default.PortablePath,ref size);
                    LoadImageWorker.ReportProgress(0, size);
                }
               
            }
        }
        /// <summary>
        /// 加载选择的文件更新进度
        /// </summary>
        private void LoadImageWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            long size = (long)e.UserState;
            RequiredMediaSpace = size.ToString();
        }
        #endregion

        #region 刻录
        /// <summary>
        /// 刻录到光碟上
        /// </summary>
        /// <param name="data">数据</param>
        private void DoBurn(List<IBurnMediaData> data)
        {
            if (data == null || data.Count == 0 || SelectedMediaWriter == null)
            {
                return;
            }

            MsftDiscFormat2Data discFormat2Data = null;
            IStream fileSystem = null;

            try
            {

                //
                // Create and initialize the IDiscFormat2Data
                //
                discFormat2Data = new MsftDiscFormat2Data
                {
                    Recorder = SelectedMediaWriter,
                    ClientName = "ApplicationName",
                    ForceMediaToBeClosed = false
                };


                //if (!discFormat2Data.IsCurrentMediaSupported(SelectedMediaWriter))
                //{
                //    this.Host.ShowMessageBox("没有光碟", MessageBoxActions.Ok);
                //    return;
                //}
                //
                // Set the verification level
                //
                var burnVerification = (IBurnVerification)discFormat2Data;
                burnVerification.BurnVerificationLevel = IMAPI_BURN_VERIFICATION_LEVEL.IMAPI_BURN_VERIFICATION_FULL;

                //
                // Check if media is blank, (for RW media)
                //
                object[] multisessionInterfaces = null;
                if (!discFormat2Data.MediaHeuristicallyBlank)
                {
                    multisessionInterfaces = discFormat2Data.MultisessionInterfaces;
                }
                //
                // Create the file system
                //
                if (!CreateMediaFileSystem(SelectedMediaWriter, multisessionInterfaces, data, out fileSystem))
                {
                    return;
                }
                //
                // add the Update event handler
                //
                discFormat2Data.Update += discFormatData_Update;

                //
                // Write the data here
                //
                discFormat2Data.Write(fileSystem);
                //
                // remove the Update event handler
                //
                discFormat2Data.Update -= discFormatData_Update;

                if (EjectOnCompleted)
                {
                    SelectedMediaWriter.EjectMedia();
                }
            }
            catch (COMException e)
            {
                this.Host.ShowMessageBox(e.Message, MessageBoxActions.Ok);
            }
            finally
            {

                if (fileSystem != null)
                {
                    Marshal.FinalReleaseComObject(fileSystem);
                }

                if (discFormat2Data != null)
                {
                    Marshal.ReleaseComObject(discFormat2Data);
                }
            }
        }

        /// <summary>
        /// 创建文件流
        /// </summary>
        private bool CreateMediaFileSystem(IDiscRecorder2 discRecorder, object[] multisessionInterfaces, List<IBurnMediaData> data, out IStream dataStream)
        {
            MsftFileSystemImage fileSystemImage = null;
            try
            {
                fileSystemImage = new MsftFileSystemImage();
                fileSystemImage.ChooseImageDefaults(discRecorder);
                fileSystemImage.FileSystemsToCreate = FsiFileSystems.FsiFileSystemJoliet | FsiFileSystems.FsiFileSystemISO9660;
                fileSystemImage.VolumeName = VolumeName;

                fileSystemImage.Update += fileSystemImage_Update;

                //
                // If multisessions, then import previous sessions
                //
                if (multisessionInterfaces != null)
                {
                    fileSystemImage.MultisessionInterfaces = multisessionInterfaces;
                    fileSystemImage.ImportFileSystem();
                }

                //
                // Get the image root
                //
                IFsiDirectoryItem rootItem = fileSystemImage.Root;

                //
                // Add Files and Directories to File System Image
                //
                foreach (BurnMediaData mediaItem in data)
                {
                    //
                    // Check if we've cancelled
                    //
                    if (backgroundBurnWorker.CancellationPending)
                    {
                        break;
                    }

                    //
                    // Add to File System
                    //
                    mediaItem.AddToFileSystem(rootItem);
                }

                fileSystemImage.Update -= fileSystemImage_Update;

                //
                // did we cancel?
                //
                if (backgroundBurnWorker.CancellationPending)
                {
                    dataStream = null;
                    return false;
                }

                dataStream = fileSystemImage.CreateResultImage().ImageStream;
            }
            catch (COMException exception)
            {
                this.Host.ShowMessageBox(exception.Message, MessageBoxActions.Ok);
                dataStream = null;
                return false;
            }
            finally
            {
                if (fileSystemImage != null)
                {
                    Marshal.ReleaseComObject(fileSystemImage);
                }
            }

            return true;
        }

        /// <summary>
        /// 更新创建文件流的进度
        /// </summary>
        void fileSystemImage_Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender, [In, MarshalAs(UnmanagedType.BStr)]string currentFile, [In] int copiedSectors, [In] int totalSectors)
        {
            var percentProgress = 0;
            if (copiedSectors > 0 && totalSectors > 0)
            {
                percentProgress = (copiedSectors * 100) / totalSectors;
            }

            if (!string.IsNullOrEmpty(currentFile))
            {
                var fileInfo = new FileInfo(currentFile);
                _burnData.StatusMessage = "Adding \"" + fileInfo.Name + "\" to image...";

                //
                // report back to the ui
                //
                _burnData.Task = BURN_MEDIA_TASK.BURN_MEDIA_TASK_FILE_SYSTEM;
                backgroundBurnWorker.ReportProgress(percentProgress, _burnData);
            }

        }

        /// <summary>
        /// 更新刻录进度
        /// </summary>
        private void discFormatData_Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender, [In, MarshalAs(UnmanagedType.IDispatch)] object progress)
        {
            //
            // Check if we've cancelled
            //
            if (backgroundBurnWorker.CancellationPending)
            {
                var format2Data = (IDiscFormat2Data)sender;
                format2Data.CancelWrite();
                return;
            }

            var eventArgs = (IDiscFormat2DataEventArgs)progress;

            _burnData.Task = BURN_MEDIA_TASK.BURN_MEDIA_TASK_WRITING;

            // IDiscFormat2DataEventArgs Interface
            _burnData.ElapsedTime = eventArgs.ElapsedTime;
            _burnData.RemainingTime = eventArgs.RemainingTime;
            _burnData.TotalTime = eventArgs.TotalTime;

            // IWriteEngine2EventArgs Interface
            _burnData.CurrentAction = eventArgs.CurrentAction;
            _burnData.StartLba = eventArgs.StartLba;
            _burnData.SectorCount = eventArgs.SectorCount;
            _burnData.LastReadLba = eventArgs.LastReadLba;
            _burnData.LastWrittenLba = eventArgs.LastWrittenLba;
            _burnData.TotalSystemBuffer = eventArgs.TotalSystemBuffer;
            _burnData.UsedSystemBuffer = eventArgs.UsedSystemBuffer;
            _burnData.FreeSystemBuffer = eventArgs.FreeSystemBuffer;

            //
            // Report back to the UI
            //
            backgroundBurnWorker.ReportProgress(0, _burnData);
        }

        /// <summary>
        /// 刻录
        /// </summary>
        private void Writing()
        {
            List<IBurnMediaData> data = new List<IBurnMediaData>();
            MediaFileSet mediaFileSet = new MediaFileSet();
            mediaFileSet.Id = "Gold";
            List<MediaFileSetStudy> fileSetStudies = new List<MediaFileSetStudy>();
            string RootPath = "CD";
            string studysPath = "IMAGES";
            string studyitemPath = "Study{0}";
            string seriesitemPath = "Series{0}";
            string filename = "IM{0}";
            string xmlSavePath = "studies";
            string XmlName = "Study{0}.xml";
            int study = -1;
            int series = -1;
            int Sopcount = -1;

            string studyIndexName = "index.xml";

            string xmlPath = Path.Combine(this.StagingFolderPath, xmlSavePath);
            if (Directory.Exists(xmlPath))
            {
                Directory.Delete(xmlPath, true);
            }
            Directory.CreateDirectory(xmlPath);

            if (MediaWriterSettings.Default.PortablePath == null || !Directory.Exists(MediaWriterSettings.Default.PortablePath))
            {
                this.Host.ShowMessageBox("指定光盘浏览器目录不存在", MessageBoxActions.Ok);
                return;
            }
            FileDirectoryUtility.CopyFiles(MediaWriterSettings.Default.PortablePath, this.StagingFolderPath, true, true);
            RootPath = Path.Combine(this.StagingFolderPath, studysPath);
            DicomDirectory dicomDirectory = new DicomDirectory(VolumeName);
            dicomDirectory.SourceApplicationEntityTitle = VolumeName;
            dicomDirectory.FileSetId = "File Set Desc";
            try
            {
                foreach (IStudyTreeItem item in Tree.Items)
                {
                    StudyTreeItem treeitem = (StudyTreeItem)item;
                    if (!treeitem.Ischecked)
                    {
                        continue;
                    }
                    study++;
                    MediaFileSetStudy fileSet = new MediaFileSetStudy();
                    string tempstudypath = Path.Combine(RootPath, string.Format(studyitemPath, study));
                    StudyXml studyXml = new StudyXml();
                    foreach (ISeriesTreeItem seriss in treeitem.Tree.Items)
                    {
                        SeriesTreeItem seriesitem = (SeriesTreeItem)seriss;
                        if (!seriesitem.Ischecked)
                        {
                            continue;
                        }
                        series++;
                        string tempseriespath = Path.Combine(tempstudypath, string.Format(seriesitemPath, series));
                        if (Directory.Exists(tempseriespath))
                        {
                            Directory.Delete(tempseriespath, true);
                        }
                        Directory.CreateDirectory(tempseriespath);

                        foreach (var sop in seriesitem.Series.GetSopInstances())
                        {
                            if (LoadImageWorker.CancellationPending == true)
                            {
                                break;
                            }

                            if (File.Exists(sop.FilePath))
                            {
                                Sopcount++;
                                string temp = Path.Combine(tempseriespath, string.Format(filename, Sopcount));
                                File.Copy(sop.FilePath, temp);
                                int start = temp.IndexOf(studysPath);
                                string dirFilePath = temp.Substring(start);
                                dicomDirectory.AddFile(temp, dirFilePath);
                                string fileName = string.Format("..\\{0}", dirFilePath);
                                DicomFile file = new DicomFile(temp);
                                file.Load();
                                file.Filename = fileName;
                                studyXml.AddFile(file);
                                fileSet.UID = sop.StudyInstanceUid;
                            }
                        }
                    }
                    //保存studyXml
                    string xmlFileName = string.Format(XmlName, study);
                    string path = Path.Combine(xmlPath, xmlFileName);
                    StudyXmlOutputSettings settings = new StudyXmlOutputSettings();
                    settings.IncludeSourceFileName = true;
                    XmlDocument document = studyXml.GetMemento(settings);

                    // Create an XML declaration. 
                    XmlDeclaration xmldecl;
                    xmldecl = document.CreateXmlDeclaration("1.0", null, null);
                    xmldecl.Encoding = "UTF-8";
                    document.Save(path);

                    fileSet.Value = Path.Combine(xmlSavePath, xmlFileName); ;
                    fileSetStudies.Add(fileSet);
                }

                if (!Directory.Exists(RootPath))
                {
                    throw new Exception(string.Format("复制检查图像到{0}目录出错", StagingFolderPath));
                }

                IBurnMediaData info = new BurnMediaData();
                info.Type = MediaType.Dir;
                info.Path = RootPath;
                data.Add(info);

                IBurnMediaData xml = new BurnMediaData();
                xml.Path = xmlPath;
                xml.Type = MediaType.Dir;
                data.Add(xml);

                string studyindexfilename = Path.Combine(this.StagingFolderPath, studyIndexName);
                if (File.Exists(studyindexfilename))
                {
                    File.Delete(studyindexfilename);
                }
                Stream stream = File.Open(studyindexfilename, FileMode.Create);
                mediaFileSet.StudyIndex = fileSetStudies.ToArray();
                XmlSerializer serializer = new XmlSerializer(typeof(MediaFileSet));
                serializer.Serialize(stream, mediaFileSet);
                IBurnMediaData studyindex = new BurnMediaData();
                studyindex.Path = studyindexfilename;
                studyindex.Type = MediaType.File;
                data.Add(studyindex);

                string dirfilename = Path.Combine(this.StagingFolderPath, "DICOMDIR");
                dicomDirectory.Save(dirfilename);
                info = new BurnMediaData();
                info.Type = MediaType.File;
                info.Path = dirfilename;
                data.Add(info);
                DoBurn(data);
            }
            catch (COMException ex)
            {
                this.Host.ShowMessageBox(ImapiReturnValues.GetName(ex.ErrorCode), MessageBoxActions.Ok);

            }
            catch (Exception e)
            {
                this.Host.ShowMessageBox(e.Message, MessageBoxActions.Ok);
            }
            finally
            {
                this.IsWriting = false;
            }


        }
        #endregion

        #region 擦除
        /// <summary>
        /// 更新擦除进度
        /// </summary>
        void discFormatErase_Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender, int elapsedSeconds, int estimatedTotalSeconds)
        {
            _burnData.Task = BURN_MEDIA_TASK.BURN_MEDIA_TASK_Erease;
            var percent = elapsedSeconds * 100 / estimatedTotalSeconds;
            //
            // Report back to the UI
            //
            backgroundBurnWorker.ReportProgress(percent);
        }

        /// <summary>
        /// 擦除光碟中数据
        /// </summary>
        private void DoEreaseDisc()
        {
            if (SelectedMediaWriter == null)
            {
                return;
            }

            MsftDiscFormat2Erase discFormatErase = null;
            try
            {

                //
                // Create the IDiscFormat2Erase and set properties
                //
                discFormatErase = new MsftDiscFormat2Erase
                {
                    Recorder = SelectedMediaWriter,
                    ClientName = "ApplicationName",
                    FullErase = true
                };

                if (discFormatErase.IsCurrentMediaSupported(SelectedMediaWriter))
                {
                    this.Host.ShowMessageBox("没有光碟", MessageBoxActions.Ok);
                    return;
                }

                //
                // Setup the Update progress event handler
                //
                discFormatErase.Update += discFormatErase_Update;
                //
                // Erase the media here
                //
                discFormatErase.EraseMedia();
                //
                // Remove the Update progress event handler
                //
                discFormatErase.Update -= discFormatErase_Update;
                //
                // Eject the media 
                //
                if (EjectOnCompleted)
                {
                    SelectedMediaWriter.EjectMedia();
                }
            }
            catch (COMException exception)
            {
                //
                // If anything happens during the format, show the message
                //
                this.Host.ShowMessageBox(ImapiReturnValues.GetName(exception.ErrorCode), MessageBoxActions.Ok);
            }
            finally
            {
                if (discFormatErase != null)
                {
                    Marshal.ReleaseComObject(discFormatErase);
                }
            }
        }

        #endregion

        #region 获得指定目录下所有文件大小

        /// <summary>
        /// 复制指定目录的所有文件
        /// </summary>
        /// <param name="sourceDir">原始目录</param>
        /// <param name="targetDir">目标目录</param>
        /// <param name="overWrite">如果为true,覆盖同名文件,否则不覆盖</param>
        /// <param name="copySubDir">如果为true,包含目录,否则不包含</param>
        public static void GetFilesSize(string sourceDir, ref long size)
        {
            //复制当前目录文件
            foreach (string sourceFileName in Directory.GetFiles(sourceDir))
            {
                FileInfo info = new FileInfo(sourceFileName);
                size += info.Length;
            }
            //复制子目录
            foreach (string sourceSubDir in Directory.GetDirectories(sourceDir))
            {
                GetFilesSize(sourceSubDir, ref size);
            }

        }

        #endregion
    }
}
