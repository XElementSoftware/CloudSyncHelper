﻿using System;
using XElement.CloudSyncHelper.UI.IconCrawler;

namespace XElement.CloudSyncHelper.UI.Win32.Model.IconCrawler
{
    internal interface IObjectToCrawl : ICrawlInformation
    {
        Guid Id { get; }
    }
}
