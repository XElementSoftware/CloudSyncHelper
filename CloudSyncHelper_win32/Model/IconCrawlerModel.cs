﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using XElement.CloudSyncHelper.UI.IconCrawler;
using XElement.CloudSyncHelper.UI.Win32.Model.IconCrawler;

namespace XElement.CloudSyncHelper.UI.Win32.Model
{
#region not unit-tested
    [Export]
    internal class IconCrawlerModel : IPartImportsSatisfiedNotification
    {
        private string GetIdStringFromCrawlInfo( ICrawlInformation crawlInformation )
        {
            var correspondingObjectToCrawl = this.ObjectsToCrawl.Single( otc => crawlInformation == otc );
            return correspondingObjectToCrawl.Id.ToString();
        }

        private IEnumerable<ICrawlInformation> GetInformationToCrawl()
        {
            var searchPattern = String.Format( "*{0}", IMAGE_FILE_EXTENSION );
            var cachedImageFileNames = Directory.EnumerateFiles( this._config.PathToImageCache, 
                                                                 searchPattern, 
                                                                 SearchOption.TopDirectoryOnly );
            var fileNames = cachedImageFileNames.Select( cifn => Path.ChangeExtension( cifn, string.Empty ) ).ToList();

            var filteredObjectsToCrawl = this.ObjectsToCrawl.Where( otc =>
            {
                var id = otc.Id.ToString().ToLower();
                var doesAnyIdMatch = fileNames.Any( fn => fn.ToLower() == id );
                return !doesAnyIdMatch;
            } ).ToList();
            return filteredObjectsToCrawl;
        }

        private IEnumerable<IObjectToCrawl> ObjectsToCrawl
        {
            get
            {
                return this._programInfosModel.ProgramInfoVMs.OfType<IObjectToCrawl>().ToList();
            }
        }

        public void OnImportsSatisfied()
        {
            IEnumerable<ICrawlInformation> input = this.GetInformationToCrawl();
            this.StartCrawlingInBackground( input );
        }

        private void StartCrawling( IEnumerable<ICrawlInformation> input )
        {
            foreach ( var crawlInfo in input )
            {
                var result = this._iconCrawler.CrawlSingle( crawlInfo );
                this.StoreCrawlResult( result );
            }
        }

        private void StartCrawlingInBackground( IEnumerable<ICrawlInformation> input )
        {
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += ( s, e ) => this.StartCrawling( input );
            backgroundWorker.RunWorkerCompleted += ( s, e ) => backgroundWorker.Dispose();
            backgroundWorker.RunWorkerAsync();
        }

        private void StoreCrawlResult( ICrawlResult crawlResult )
        {
            if ( crawlResult.Image != null )
            {
                var noExtensionFileName = this.GetIdStringFromCrawlInfo( crawlResult.Input );
                var noExtensionFilePath = Path.Combine( this._config.PathToImageCache, noExtensionFileName );
                var filePath = Path.ChangeExtension( noExtensionFilePath, IMAGE_FILE_EXTENSION );
                using ( Stream fileStream = new FileStream( filePath, FileMode.Create, FileAccess.Write ) )
                {
                    crawlResult.Image.Save( fileStream, crawlResult.Image.RawFormat );
                }
                crawlResult.Image.Dispose();
            }
        }

        private const string IMAGE_FILE_EXTENSION = ".jpg";

        [Import]
        private IConfig _config = null;

        [Import( typeof( IPriotizableIconCrawler ) )]
        private IIconCrawler _iconCrawler = null;

        [Import]
        private ProgramInfosModel _programInfosModel = null;
    }
#endregion
}
